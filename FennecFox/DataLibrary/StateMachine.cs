using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FennecFox
{
	public class StateMachineHost
	{
		private Queue<StateMachine> m_newStateMachines = new Queue<StateMachine>();
		private List<StateMachine> m_runningStateMachines = new List<StateMachine>();
		private AutoResetEvent m_haveMessages = new AutoResetEvent(false);
		private bool m_runThread = true;

		public StateMachineHost(string name)
		{
			Thread t = new Thread(StateMachineThread);
			t.Name = "State Machine Host " + name;
			t.IsBackground = true;
			t.Start();
		}

		public void Attach(StateMachine sm)
		{
			lock (m_newStateMachines)
			{
				m_newStateMachines.Enqueue(sm);
			}
		}

		public void WakeUp()
		{
			m_haveMessages.Set();
		}

		public void Close()
		{
			m_runThread = false;
		}

		private void StateMachineThread()
		{
			while (m_runThread)
			{
				bool found = false;
				lock (m_newStateMachines)
				{
					while (m_newStateMachines.Count > 0)
					{
						StateMachine sm = m_newStateMachines.Dequeue();
						sm.Initialize();
						m_runningStateMachines.Add(sm);
					}
				}

				foreach (StateMachine sm in m_runningStateMachines)
				{
					found |= sm.DoEvent();
				}

				if (!found)
				{
					m_haveMessages.WaitOne(1000);
				}
			}
		}
	}

	public class StateMachine
	{
		protected class Event
		{
			public Event(string name)
			{
				EventName = name;
			}

			public string EventName { get; private set; }
		}

		protected class Event<T> : Event
		{
			public Event(string name, T param)
				: base(name)
			{
				Param = param;
			}

			public T Param { get; private set; }
		}

		protected class Event<T1, T2> : Event
		{
			public Event(string name, T1 param1, T2 param2)
				: base(name)
			{
				Param1 = param1;
				Param2 = param2;
			}

			public T1 Param1 { get; private set; }

			public T2 Param2 { get; private set; }
		}

		private static Event evtEnter = new Event("EventEnter");
		private static Event evtExit = new Event("EventExit");
		private static Event evtQueryParent = new Event("QueryParent");

		protected delegate State State(Event evt);

		private StateMachineHost m_host = null;
		private State m_currentState = null;
		private State m_desiredState = null;
		private object m_eventLock = new object();
		private object m_stateLock = new object();
		private Stack<Queue<Event>> m_EventQueues = new Stack<Queue<Event>>();
		private Dictionary<string, Queue<Event>> m_EventQueueMap = new Dictionary<string, Queue<Event>>();

		public string Name { get; private set; }

		public virtual void Initialize() // runs on host thread.
		{
		}

		protected StateMachine(string name, StateMachineHost host)
		{
			Name = name;
			m_host = host;
			PushQueue("User");
			host.Attach(this);
		}

		protected void PushQueue(string name)
		{
			Queue<Event> q = new Queue<Event>();
			m_EventQueueMap[name] = q;
			m_EventQueues.Push(q);
		}

		protected void PopQueue()
		{
			Queue<Event> qStack = m_EventQueues.Pop();
			qStack.Clear();
		}

		protected void StartOneShotTimer(int delayMS, Event evt)
		{
		}

		protected void CancelTimer(string eventName)
		{
		}

		protected void SetInitialState(State initialState)
		{
			Stack<State> newList = new Stack<State>();
			State s = initialState;
			while (s != null)
			{
				newList.Push(s);
				s = s(evtQueryParent);
			}
			while (newList.Count > 0)
			{
				s = newList.Pop();
				s(evtEnter);
			}
			lock (m_stateLock)
			{
				m_currentState = initialState;
			}
		}

		protected void ChangeState(State newState)
		{
			lock (m_stateLock)
			{
				if (newState != m_currentState)
				{
					m_desiredState = newState;
				}
			}
		}

		protected void PostEvent(Event evt)
		{
			lock (m_eventLock)
			{
				Queue<Event> q = m_EventQueues.Peek();
				if (q != null)
				{
					//System.Console.WriteLine("Queue event: " + evt.Name);
					q.Enqueue(evt);
					m_host.WakeUp();
				}
			}
		}

		protected void PostEvent(string queueType, Event evt)
		{
			lock (m_eventLock)
			{
				Queue<Event> q = m_EventQueueMap[queueType];
				if (q != null)
				{
					//System.Console.WriteLine("Queue event: " + evt.Name);
					q.Enqueue(evt);
					m_host.WakeUp();
				}
			}
		}


		// private methods

		private void DoChange()
		{
			List<State> oldList = new List<State>();
			State sCurrent = null;
			State sDesired = null;
			State sNewDesired = null;

			lock (m_stateLock)
			{
				sCurrent = m_currentState;
				sDesired = m_desiredState;
				m_desiredState = null;
			}
			while (sCurrent != null)
			{
				oldList.Add(sCurrent);
				sCurrent = sCurrent(evtQueryParent);
			}
			List<State> newList = new List<State>();
			while (sDesired != null)
			{
				newList.Add(sDesired);
				sDesired = sDesired(evtQueryParent);
			}

			// Find the least common ancestor.
			foreach (State sOld in oldList)
			{
				int ixLCA = newList.IndexOf(sOld);
				if (ixLCA != -1)
				{
					for (int i = ixLCA - 1; i >= 0; i--)
					{
						State sNew = newList[i];
						sNew(evtEnter);
						lock (m_stateLock) // check if our destination changed.
						{
							sNewDesired = m_desiredState;
						}
						if ((sNewDesired != null) && (sNewDesired != sDesired))
						{
							lock (m_stateLock)
							{
								m_currentState = sNew;
							}
							return;
						}
					}
					break;
				}
				else
				{
					sOld(evtExit);
					lock (m_stateLock) // check if our destination changed.
					{
						sNewDesired = m_desiredState;
					}
					if ((sNewDesired != null) && (sNewDesired != newList[0]))
					{
						State sParent = sOld(evtQueryParent);
						lock (m_stateLock)
						{
							m_currentState = sParent;
						}
						return;
					}
				}
			}
			lock (m_stateLock)
			{
				m_currentState = newList[0];
			}
		}

		public bool DoEvent()
		{
			Event evt = GetEvent();
			if (evt != null)
			{
				DispatchEvent(evt);
				return true;
			}
			return false;
		}

		private Event GetEvent()
		{
			int evtCount = 0;
			Event evt = null;

			lock (m_eventLock)
			{
				foreach (Queue<Event> q in m_EventQueues.Reverse())
				{
					evtCount = q.Count;
					if (evtCount > 0)
					{
						evt = q.Dequeue();
						//System.Console.WriteLine("Dequeue event " + evt.Name);
						break;
					}
				}
			}
			return evt;
		}

		private void DispatchEvent(Event evt)
		{
			if (evt != null)
			{
				State s = null;
				lock (m_stateLock)
				{
					s = m_currentState;
				}
				while (s != null)
				{
					s = s(evt);
				}
				while (true)
				{
					State sDesired = null;
					lock (m_stateLock)
					{
						sDesired = m_desiredState;
					}
					if (null == sDesired)
					{
						break;
					}
					DoChange();
				}
			}
		}
	}
}