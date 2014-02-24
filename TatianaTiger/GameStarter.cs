using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POG.Utils;
using POG.Forum;
using System.Diagnostics;
using System.Windows.Forms;
using POG.Werewolf;
using System.Text.RegularExpressions;

namespace TatianaTiger
{
	class GameStarter : StateMachine
	{
		#region consts
		readonly String WOLF = "Wolf";
		readonly String VILLAGE = "Village";
		readonly String VANILLA = "vanilla";
		readonly String SEER = "seer";
		#endregion
		#region fields
		VBulletinForum _forum;
		private POG.Werewolf.IPogDb _db;
		System.Timers.Timer _timer = new System.Timers.Timer();
		System.Timers.Timer _timerPM = new System.Timers.Timer();
		DateTimeOffset _EarliestPMTime = DateTimeOffset.Now;
		DateTimeOffset _maxTime = DateTimeOffset.MaxValue;
		Int32 _lastPMProcessed = 0;
		PrivateMessage _requestPM;
		List<String> _requestNames;
		List<String> _validNames = new List<string>();
		List<String> _canPM = new List<string>();
		List<String> _PMSent = new List<string>();
		List<Player> _playerRoles = new List<Player>();
		List<Player> _peeks = new List<Player>();
		Dictionary<String, Player> _playerByName = new Dictionary<string,Player>();
		Player _peek;
		String _killMessage;
		DateTime _nextNight;
		int _d1Duration;
		int _dDuration;
		int _n1Duration;
		int _nDuration;
		Int32 _threadId = 1204368;
		String _url = "http://forumserver.twoplustwo.com/59/puzzles-other-games/vote-counter-testing-thread-1204368/";
		Boolean _majorityLynch = false;
		ElectionInfo _count;

		private POG.Werewolf.AutoComplete _autoComplete;
		int _pendingLookups = 0;
		#endregion

		public GameStarter(VBulletinForum forum, StateMachineHost host, POG.Werewolf.IPogDb db, POG.Werewolf.AutoComplete autoComplete)
			: base("GameStarter", host)
		{
			_forum = forum;
			_db = db;
			_autoComplete = autoComplete;
			_timer.AutoReset = false;
			_timer.Elapsed += new System.Timers.ElapsedEventHandler(_timer_Elapsed);
			_timerPM.AutoReset = false;
			_timerPM.Elapsed += new System.Timers.ElapsedEventHandler(_timerPM_Elapsed);
			_EarliestPMTime = _EarliestPMTime.AddMinutes(-1);
			SetInitialState(StateIdle);
		}

		void _timerPM_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			PostEvent(new Event("SendPMTimer"));
		}
		#region states
		State StateTop(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
						_timer.Interval = GetTimerInterval();
						_timer.Start();
					}
					break;

				case "EventExit":
					{
						_timer.Stop();
						_timerPM.Stop();
					}
					break;

				case "SendPMTimer":
					{
						UnqueuePM();
					}
					break;
			}
			return null;
		}
		State StateIdle(Event e)
		{
			switch (e.EventName)
			{

				case "EventMinute":
					{
						PollPMs();
					}
					break;

				case "StrangerPM":
					{
						Event<PrivateMessage> pme = e as Event<PrivateMessage>;
						String msg = pme.Param.Content;
						List<String> rawList = msg.Split(
							new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(p => p.Trim())
							.Distinct().OrderBy(p => p.ToUpper()).ToList();
						_validNames.Clear();
						_requestNames = rawList;
						_requestPM = pme.Param;
						CheckNames();
						ChangeState(StateStartGameRequested);
					}
					break;
			}
			return StateTop;
		}
		String _threadTitle;
		State StateStartGameRequested(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
					}
					break;
				case "EventExit":
					{
					}
					break;
				case "NamesChecked":
					{
						if (_validNames.Count != 9)
						{
							ReplyFailure(_requestPM, _requestNames, _validNames, "In order to start a turbo I need a PM with exactly 9 names; one name on each line.");
							ChangeState(StateIdle);
						}
						else
						{
							String title = "";
							foreach(string request in _requestNames)
							{
								if (request.ToLowerInvariant().Trim().StartsWith("title"))
								{
									title = request.Trim().Substring("title".Length).Trim();
								}

							}
							if (title.Length > 0)
							{
								_threadTitle = title;
							}
							else
							{
								_threadTitle = String.Format("Turbo Game {0:yy.MM.dd HH:mm}", DateTime.Now.ToUniversalTime());
								QueueAnnouncement("Next time give your turbo a title by including 'title MY CUSTOM TITLE' as the first line of your PM.");
							}
							if (CheckCanReceivePM())
							{
								PostEvent(new Event("Rand"));
							}
							else
							{
								ReplyFailure(_requestPM, _validNames, _canPM, "Some players can't receive PMs. They need to clear space or join pog messages group.");
								ChangeState(StateIdle);
							}
						}
					}
					break;

				case "PostOP":
					{
						ReplyFailure(_requestPM, new List<String>(), _canPM, "So far so good!");
						ChangeState(StateIdle);
					}
					break;

				case "Rand":
					{
						int i;
						Player p;
						for (i = 1; i < 3; i++)
						{
							String wolf = Misc.RandomItemFromList(_canPM);
							p = new Player(i, false, wolf, WOLF, VANILLA);
							_playerRoles.Add(p);
							_canPM.Remove(wolf);
						}
						String seer = Misc.RandomItemFromList(_canPM);
						p = new Player(i++, false, seer, VILLAGE, SEER);
						_playerRoles.Add(p);
						_canPM.Remove(seer);
						foreach (String vanilla in _canPM)
						{
							p = new Player(i++, false, vanilla, VILLAGE, VANILLA);
							_playerRoles.Add(p);
						}
						_canPM.Clear();
						foreach (var pr in _playerRoles)
						{
							_playerByName[pr.Name] = pr;

						}
						Int32 peek = Misc.RandomItemFromRange(4, 10);
						_peeks.Clear();
						_peeks.Add(LookupPlayer(peek));
						PostEvent(new Event("SendPMs"));
					}
					break;

				case "SendPMs":
					{
						List<String> seers = (from p in _playerRoles where (p.Role == SEER) select p.Name).ToList();
						List<String> vanillas = (from p in _playerRoles where (p.Team == VILLAGE) && (p.Role == VANILLA) select p.Name).ToList();
						List<string> wolves = (from p in _playerRoles where p.Team == WOLF select p.Name).ToList();

						String seerMsg = MakeSeerPmMessage();
						String wolfMsg = MakeWolfPmMessage();
						String vanillaMsg = MakeVanillaPmMessage();

						PrivateMessage pmSeer = new PrivateMessage(null, seers, "Turbo Role PM", seerMsg);
						PrivateMessage pmWolf = new PrivateMessage(wolves, null, "Turbo Role PM", wolfMsg);
						PrivateMessage pmVanilla = new PrivateMessage(null, vanillas, "Turbo Role PM", vanillaMsg);
						List<PrivateMessage> pms = new List<PrivateMessage>();
						pms.Add(pmSeer);
						pms.Add(pmWolf);
						pms.Add(pmVanilla);
						PrivateMessage pm = Misc.RandomItemFromList(pms);
						pms.Remove(pm);
						QueuePM(pm);
						pm = Misc.RandomItemFromList(pms);
						pms.Remove(pm);
						QueuePM(pm);
						pm = Misc.RandomItemFromList(pms);
						QueuePM(pm);
						ChangeState(StateNight0);
					}
					break;

			}
			return StateTop;
		}
		String MakeSeerPmMessage()
		{
			String peek = _peeks[0].Name;
			String seerMsg = String.Format("You are the seer. Your random n0 villager peek is {0}\r\n", peek);
			seerMsg += "\r\nEach night send a PM with the exact name of the player you want to peek.\r\n";
			return seerMsg;
		}
		String MakeWolfPmMessage()
		{
			List<string> wolves = (from p in _playerRoles where p.Team == WOLF select p.Name).ToList();
			String wolfMsg = "You are the wolves:\r\n";
			foreach (var w in wolves)
			{
				wolfMsg += w + "\r\n";
			}
			wolfMsg += "\r\nEach night send a PM with the exact name of the player you want to kill.\r\n";
			wolfMsg += "If the game is hopeless send a PM that only says 'resign'.";
			return wolfMsg;
		}
		String MakeVanillaPmMessage()
		{
			String rc = "You are a vanilla villager.";
			return rc;
		}

		Boolean _hyper = false;
		State StatePlaying(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
					}
					break;
				case "EventExit":
					{
						_peeks.Clear();
						_playerByName.Clear();
						_playerRoles.Clear();
						_requestNames.Clear();
						_peek = null;
						_killMessage = "";
						//_threadId
						_majorityLynch = false;
						_count = null;
						_day = 0;
						_kill = "";
						lock (_pmQueue)
						{
							_pmQueue.Clear();
						}
						_EarliestPMTime = TruncateSeconds(DateTime.Now);
					}
					break;


				case "CorrectionPM":
					{
						PrivateMessage pm = (e as Event<PrivateMessage>).Param;
						List<String> rawList = pm.Content.Split(
							new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
							.Select(p => p.Trim())
							.Distinct().ToList();
						StringBuilder reply = new StringBuilder();
						foreach (var line in rawList)
						{
							Match m = Regex.Match(pm.Content, "(.*)=(.*)");
							if (m.Success)
							{
								String l = m.Groups[1].Value.Trim();
								String r = m.Groups[2].Value.Trim();
								var leftPlayer = LookupPlayer(l);
								var rightPlayer = LookupPlayer(r);
								if ((leftPlayer == null) && (rightPlayer == null))
								{
									reply.AppendFormat("failure: neither '{0}' nor '{1}' is a recognized player.\r\n", l, r);
									continue;
								}
								if ((leftPlayer != null) && (rightPlayer != null))
								{
									reply.AppendFormat("failure: both '{0}' and '{1}' are players.\r\n", l, r);
									continue;
								}
								var p = (leftPlayer != null) ? leftPlayer : rightPlayer;
								String nickname = (leftPlayer == null) ? l : r;
								if (_count != null)
								{
									_count.AddVoteAlias(nickname, p.Name);
									reply.AppendFormat("success: '{0}' now means {1}.\r\n", nickname, p.Name);
									continue;
								}
								reply.AppendFormat("failure: there is no game right now.");
								break;
							}
							else
							{
								reply.AppendFormat("no '=' so ignoring line '{0}'", line);
							}
						}
						if (reply.Length == 0)
						{
							reply.AppendLine("Your pm had 'correction' in the title, but no corrections were found.");
							reply.AppendLine("format: bolded=playername, one correction on each line.");
						}
						QueuePM(new String[] { pm.From }, "Vote Correction Results", reply.ToString());
					}
					break;

				case "StrangerPM":
					{
						PrivateMessage pm = (e as Event<PrivateMessage>).Param;
						QueuePM(new string[] { pm.From }, pm.Title,
							String.Format("Thank you for your pm. Unfortunately, I am currently busy auto-modding this game:{0}.", _url));
					}
					break;

				case "SubPM":
					{
						PrivateMessage pm = (e as Event<PrivateMessage>).Param;
						if (LookupPlayer(pm.From) != null)
						{
							QueuePM(new string[] { pm.From }, pm.Title,
								String.Format("Sorry, I can't sub you back in."));
							break;
						}
						Player role = ParseSubPM(pm);
						if (role == null)
						{
							QueuePM(new string[] { pm.From }, pm.Title,
								String.Format("I couldn't find the player you are asking to sub in for: '{0}'", pm.Content));
							break;
						}
						List<String> notify = new List<string>() { role.Name, pm.From };
						StringBuilder sb = new StringBuilder();
						String subMsg = String.Format("[b]{0}[/b] is subbing in for [b]{1}[/b]\n\n", pm.From, role.Name);
						_count.SubPlayer(role.Name, pm.From);
						_playerByName[pm.From] = role;
						role.Name = pm.From;
						QueueAnnouncement(subMsg);
						sb.AppendLine(subMsg);
						sb.AppendLine("Role Info:\n");
						if (role.Team == WOLF)
						{
							sb.AppendLine(MakeWolfPmMessage());
						}
						else
						{
							if (role.Role == SEER)
							{
								sb.AppendLine(MakeSeerPmMessage());
								sb.AppendLine("\r\n[u]Peeks:[/u]\r\n");
								foreach (var peek in _peeks)
								{
									sb.AppendFormat("{0} : {1} {2}\r\n", peek.Name, peek.Team, peek.Role);
								}

							}
							else
							{
								sb.AppendLine(MakeVanillaPmMessage());
							}
						}
						PrivateMessage pmSub = new PrivateMessage(notify, null, "Substitution in Turbo", sb.ToString());
						QueuePM(pmSub);
					}
					break;

				case "DeadPM":
					{
						PrivateMessage pm = (e as Event<PrivateMessage>).Param;
						QueuePM(new string[] { pm.From }, pm.Title,
							"Thank you for your pm. Unfortunately, you are dead in this game so I can't help you.");
					}
					break;

				case "WolfResignPM":
					{
						PrivateMessage pm = (e as Event<PrivateMessage>).Param;
						String msg = String.Format("{0} has resigned for the wolves. The village wins.", pm.From);
						PostEvent(new Event<String>("GameOver", msg));
					}
					break;

				case "GameOver":
					{
						String msg = (e as Event<String>).Param;
						var live = from p in _playerRoles where p.Dead == false select p;
						StringBuilder sb = new StringBuilder();
						sb.AppendLine(msg);
						sb.AppendLine("\r\n\r\n[u]Remaining Players:[/u]\r\n");
						foreach (var p in live)
						{
							sb.AppendFormat("{0} : {1} {2}\r\n", p.Name, p.Team, p.Role);
							KillPlayer(p.Name);
						}
						sb.AppendLine("\r\n\r\n[u]Peeks:[/u]\r\n");
						foreach (var p in _peeks)
						{
							sb.AppendFormat("{0} : {1} {2}\r\n", p.Name, p.Team, p.Role);
						}
						_forum.LockThread(_threadId, false);
						_forum.MakePost(_threadId, "Mod: Game Over", sb.ToString(), 0, false);
						ChangeState(StateIdle);
					}
					break;
			}
			return StateTop;
		}
		State StateNight0(Event e)
		{
			switch(e.EventName)
			{
				case "EventEnter":
					{
						_majorityLynch = false;
                        _count.CheckMajority = false;
						if (_forum.Username.Equals("Oreos", StringComparison.InvariantCultureIgnoreCase))
						{
							_hyper = true;
							_d1Duration = 2;
							_dDuration = 2;
							_n1Duration = 1;
							_nDuration = 1;
						}
						else
						{
							_hyper = false;
							_d1Duration = 22;
							_dDuration = 17;
							_n1Duration = 7;
							_nDuration = 4;
						}
					}
					break;

				case "EventExit":
					{
					}
					break;

				case "EventMinute":
					{
						if (_pmQueue.Count == 0)
						{
							SetDayDuration();
							AnnounceDay(0);
							ChangeState(StateDay);
						}
					}
					return null;
			}
			return StatePlaying;
		}
		Int32 _day = 0;
		Int32 _lastCountPostNumber;
		DateTime _lastCountTime;
		List<String> _lastCountLeaders = new List<string>();
        Boolean _missingPlayers = false;

		State StateDay(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
						_lastCountPostNumber = 0;
						_lastCountTime = DateTime.MinValue;
						_lastCountLeaders.Clear();
						_day++;
						AnnounceDay(_day);
						_count.SetDayBoundaries(_day, _count.LastPost + 1, _nextNight);
						_count.ChangeDay(_day);
					}
					break;

				case "EventExit":
					{
					}
					break;

				case "EventMinute":
					{
						if (_count != null)
						{
							Trace.TraceInformation("another minute, need a vote count.");
							_count.CheckThread(() =>
							{
								PostEvent(new Event("CountUpdated"));
							});
						}
						PollPMs();
					}
					return null;

				case "CountUpdated":
					{
						Trace.TraceInformation("Deciding to post another vote count.");
						String announce = GetAnnouncements();
						String postableCount = announce +  _count.GetPostableVoteCount();
						Int32 count;
						List<String> leaders = _count.GetVoteLeaders(out count).ToList();

						DateTime now = DateTime.Now;
						now = TruncateSeconds(now);
						if ((now > _nextNight) || (count >= GetMajorityCount()))
						{
							String lynch = Misc.RandomItemFromList(leaders);
							Player player = KillPlayer(lynch);

							String teamColor = player.Team == VILLAGE ? "green" : "red";
							String roleColor = player.Role == SEER ? "purple" : teamColor;
							String result = (leaders.Count > 1) ? "The lynch was randed to break a tie.\r\n" : "";
							result += String.Format(
								"[b]{0}[/b] was lynched. Team: [color={1}][b]{2}[/b][/color] Role: [color={3}][b]{4}[/b][/color]\r\n",
								player.Name, teamColor, player.Team, roleColor, player.Role);
							String winner = "";
							if (WolfCount == 0)
							{
								winner = "[color=green][b]Village Wins![/b][/color]\r\n";
							}
							else
							{
								if (WolfCount >= (LiveCount - WolfCount - 1))
								{
									winner = "[color=red][b]Wolves Win![/b][/color]\r\n";
								}
							}
							String post = postableCount + "\r\n\r\n" + result;
							if (winner == "")
							{
                                var postcounts = _count.GetPostCounts();
                                var missing = from p in postcounts where p.Item2 == 0 select p.Item1;
                                if (missing.Any())
                                {
                                    _missingPlayers = true;
                                }
                                else
                                {
                                    _missingPlayers = false;
                                }
                                SetNightDuration();
								post += "PM your night action or it will be randed.\r\nDeadline: " + MakeGoodBad(_nextDay);
								post += "\r\n\r\n[b]It is night![/b]";
								_forum.MakePost(_threadId, "Mod: Lynch result", post, 0, true);
								ChangeState(StateNightNeedAction);
							}
							else
							{
								post += winner;
								PostEvent(new Event<String>("GameOver", post));
							}
						}
						else
						{
							Boolean postCount = false;
							Int32 lastPost = _count.LastPost;
							if ((lastPost - _lastCountPostNumber) > 5)
							{
								if (((lastPost - 1) / 50) != ((_lastCountPostNumber - 1) / 50))
								{
									// totp!
									postCount = true;
								}
								DateTime realNight = _nextNight.AddMinutes(1);
								TimeSpan whole = realNight - _lastCountTime;
								TimeSpan part = now - _lastCountTime;
								if ((part.TotalSeconds / whole.TotalSeconds) > 0.33)
								{
									postCount = true;
								}
							}
							HashSet<String> movers = new HashSet<string>(_lastCountLeaders);
							movers.SymmetricExceptWith(leaders);
							if (movers.Any())
							{
								// vote leader changed.
								postCount = true;
							}
							if (announce.Length > 0)
							{
								postCount = true;
							}
							if (postCount)
							{
								_lastCountLeaders = leaders;
								_lastCountTime = now;
								_lastCountPostNumber = lastPost + 1;
								_forum.MakePost(_threadId, "Vote Count", postableCount, 0, false);
							}
						}
					}
					break;
			}
			return StatePlaying;
		}
		DateTime _nextDay;
		String _kill;
		State StateNight(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
						_kill = "";
					}
					break;

				case "EventExit":
					{
						_majorityLynch = true;
                        _count.CheckMajority = true;
						SetDayDuration();
						Int32 villas = SeerCount + VanillaCount;
						Int32 wolves = WolfCount;
						if ((wolves + 1) >= villas)
						{
							_count.LockedVotes = true;
						}
					}
					break;

				case "EventMinute":
					{
						PollPMs();
						if (_count != null)
						{
							_count.CheckThread(() => // keep reading the thread so we have the right start post.
							{
								PostEvent(new Event("CountUpdated"));
							});
						}
						PostEvent(new Event("CheckForTimeout"));
					}
					return null;
			}
			return StatePlaying;
		}
		State StateCallDay(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
						if (_kill == "")
						{
							// rand a kill
							var villagers = (from p in _playerRoles where (p.Dead == false) && (p.Team == VILLAGE) select p);
							var kill = Misc.RandomItemFromList(villagers);
							Trace.TraceInformation("*** Randed a wolf kill of '{0}'", kill.Name);
							_kill = kill.Name;
						}
						String seerName = (from p in _playerRoles where p.Role == SEER select p.Name).First();
						if (IsSeerAlive() && (_peek == null))
						{
							var possibles = GetPeekCandidates(seerName);
							if (possibles.Any())
							{
								_peek = Misc.RandomItemFromList(possibles);
								Trace.TraceInformation("*** Randed a seer peek of '{0}'", _peek.Name);
							}
						}
						var seer = LookupPlayer(seerName);

						var k = LookupPlayer(_kill);
						KillPlayer(k.Name);
						String roleColor = (k.Role == SEER) ? "purple" : "green";
						_killMessage = String.Format("The wolves killed [b]{0}[/b]. Role: [color={1}][b]{2}[/b][/color].\r\n",
							k.Name, roleColor, k.Role);
						
						if ((seer.Dead == false) && (_kill.Equals(seerName, StringComparison.InvariantCultureIgnoreCase) == false))
						{
							String msg;
							if (_peek != null)
							{
								_peeks.Add(_peek);
								msg = String.Format("Peek result for {0}: {1}\r\n", _peek.Name, _peek.Team);
							}
							else
							{
								msg = "There's nobody left to peek.";
							}
							QueuePM(new String[] { seer.Name }, "Night Action Success", msg);
						}

						ChangeState(StateDay);
					}
					break;

				case "EventExit":
					{
					}
					break;

				case "EventMinute":
					{
					}
					break;
			}
			return StateNight;
		}

		State StateNightNeedAction(Event e)
		{
			switch (e.EventName)
			{
				case "EventEnter":
					{
						_peek = null;
					}
					break;

				case "EventExit":
					{
					}
					break;

				case "CheckForTimeout":
					{
						DateTime now = DateTime.Now;
						now = TruncateSeconds(now);
						if (now > _nextDay)
						{
							PostEvent(new Event("NightTimeout"));
						}
					}
					return null;
				case "NightTimeout":
					{
						ChangeState(StateCallDay);
					}
					return null;

				case "WolfKillPM":
					{
						Event<PrivateMessage> pme = e as Event<PrivateMessage>;
						PrivateMessage pm = pme.Param;
						var player = LookupPlayer(pm.From);
						if (_kill == "")
						{
							ParseKillPM(pm);
						}
						if ((_kill != "") && (!IsSeerAlive() || (_peek != null)) && !_missingPlayers)
						{
							ChangeState(StateCallDay);
							return null;
						}
					}
					break;

				case "SeerPM":
					{
						Event<PrivateMessage> pme = e as Event<PrivateMessage>;
						PrivateMessage pm = pme.Param;
						if (_peek == null)
						{
							_peek = ParsePeekPM(pm);
						}
						if ((_kill != "") && (_peek != null))
						{
							ChangeState(StateCallDay);
							return null;
						}
					}
					break;
			}
			return StateNight;
		}
		#endregion
		#region helpers
		void _timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			PostEvent(new Event("EventMinute"));
			_timer.Interval = GetTimerInterval();
			_timer.Start();
		}
		private double GetTimerInterval()
		{
			DateTime now = DateTime.Now;
			double rc = 50 + ((60 - now.Second) * 1000) - now.Millisecond;
			if (rc < 1000) rc += (60 * 1000);
			return rc;
		}
		void ReplyFailure(PrivateMessage pm, IEnumerable<String> failed, IEnumerable<String> players, String msg)
		{
			String reply= "Sorry, this failed. " + msg + "\r\n\r\n";
			reply+= String.Format("These {0} were good:\r\n", players.Count());
			foreach (var player in players)
			{
				reply += player + "\r\n";
			}
			reply += String.Format("\r\nThese {0} failed:\r\n", failed.Count());
			foreach(var player in failed)
			{
				reply += player + "\r\n";
			}
			PrivateMessage rpm = new PrivateMessage(new String[] { pm.From }, null, pm.Title, reply);
			_forum.SendPM(rpm);
		}
		void PollPMs()
		{
			List<PrivateMessage> pms = new List<PrivateMessage>();
			List<PrivateMessage> rc = new List<PrivateMessage>();
			_forum.CheckPMs(0, 1, null, (folderpage, errMessage, cookie) =>
			{
				for (int i = 0; i < folderpage.MessagesThisPage; i++)
				{
					PMHeader header = folderpage[i];
					if ((_EarliestPMTime <= header.Timestamp) && (_maxTime >= header.Timestamp) && (header.Id > _lastPMProcessed))
					{
						_db.AddPosters(new Poster[] {new Poster(header.Sender, header.SenderId)});
						_forum.ReadPM(header.Id, null, (id, pm, cake) =>
						{
							pms.Add(pm);
						});
					}

				}
			}
			);
			Int32 lastPMId = 0;
			foreach (var pm in pms)
			{
				if (pm.Id <= _lastPMProcessed)
				{
					continue;
				}
				rc.Add(pm);
				lastPMId = Math.Max(lastPMId, pm.Id);
			}
			if (lastPMId > _lastPMProcessed)
			{
				_lastPMProcessed = lastPMId;
			}
			rc.Sort((x, y) => x.Id.CompareTo(y.Id));
			foreach (var pm in rc)
			{
				if (pm.Title.ToLowerInvariant().Trim().StartsWith("correct"))
				{
					PostEvent(new Event<PrivateMessage>("CorrectionPM", pm));
					continue;
				}
				if ((pm.Title.ToLowerInvariant().Trim().StartsWith("sub")) || (pm.Content.ToLowerInvariant().Trim().StartsWith("sub ")))
				{
					PostEvent(new Event<PrivateMessage>("SubPM", pm));
					continue;
				}
				var player = LookupPlayer(pm.From);
				if (player == null)
				{
					PostEvent(new Event<PrivateMessage>("StrangerPM", pm));
					continue;
				}
				if (player.Dead == true)
				{
					PostEvent(new Event<PrivateMessage>("DeadPM", pm));
					continue;
				}
				if (player.Team == WOLF)
				{
					if (pm.Content.StartsWith("resign", StringComparison.InvariantCultureIgnoreCase))
					{
						PostEvent(new Event<PrivateMessage>("WolfResignPM", pm));
						continue;
					}
					PostEvent(new Event<PrivateMessage>("WolfKillPM", pm));
					continue;
				}
				if (player.Role == SEER)
				{
					PostEvent(new Event<PrivateMessage>("SeerPM", pm));
					continue;
				}
				PostEvent(new Event<PrivateMessage>("VanillaPM", pm));
			}
		}
		Boolean CheckCanReceivePM()
		{
			_canPM.Clear();
			Boolean failed = false;
			for (int i = _validNames.Count - 1; i >= 0; i--)
			{
				String name = _validNames[i];
				if (_forum.CanUserReceivePM(name))
				{
					_canPM.Add(name);
					_validNames.RemoveAt(i);
				}
				else
				{
					failed = true;
				}
			}
			return !failed;
		}
		internal void CheckNames()
		{
			for(int i = _requestNames.Count - 1; i >= 0; i--)
			{
				String name = _requestNames[i];
				name = name.Split('\t')[0];
				name = name.Trim();
				int ix = i;
				if (_autoComplete.GetPosterId(name,
					(poster, id) =>
					{
						if (id > 0)
						{
							_validNames.Add(poster);
							_requestNames.Remove(name);
						}
						_pendingLookups--;
						if (0 == _pendingLookups)
						{
							PostEvent(new Event("NamesChecked"));
						}
					}
					) > 0)
				{
					_validNames.Add(name);
					_requestNames.RemoveAt(ix);
				}
				else
				{
					_pendingLookups++;
				}
			}
			if (0 == _pendingLookups)
			{
				PostEvent(new Event("NamesChecked"));
			}

		}
		DateTime TruncateSeconds(DateTime dt)
		{
			DateTime rc = new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, 0);
			return rc;
		}
		Int32 WolfCount
		{
			get
			{
				int wolfCount = (from p in _playerRoles where (p.Dead == false) && (p.Team == WOLF) select p).Count();
				return wolfCount;
			}
		}
		Int32 SeerCount
		{
			get
			{
				int seerCount = (from p in _playerRoles where (p.Dead == false) && (p.Team == VILLAGE) && (p.Role == SEER) select p).Count();
				return seerCount;
			}
		}
		Int32 VanillaCount
		{
			get
			{
				int vanillaCount = (from p in _playerRoles where (p.Dead == false) && (p.Team == VILLAGE) && (p.Role == VANILLA) select p).Count();
				return vanillaCount;
			}
		}
		Int32 LiveCount
		{
			get
			{
				var live = from p in _playerRoles where p.Dead == false select p.Name;
				return live.Count();
			}
		}
		String MakeGoodBad(DateTime dt)
		{
			int minuteGood = dt.Minute;
			int minuteBad = (minuteGood + 1) % 60;
			String rc = String.Format("[highlight][color=green]:{0} good[/color] [color=red]:{1} bad[/color][/highlight]",
			   minuteGood.ToString("00"), minuteBad.ToString("00"));
			return rc;
		}
		void AnnounceDay(Int32 day)
		{
			var sb = new StringBuilder();
			String announce = GetAnnouncements();
			if (announce != "")
			{
				sb.AppendLine(announce);
			}
			if (_killMessage != "")
			{
				sb.AppendLine(_killMessage);
				sb.AppendLine();
			}
			var live = from p in _playerRoles where (p.Dead == false) orderby p.Name.ToUpperInvariant() select p.Name;
			string wolf = (WolfCount > 1) ? "wolves" : "wolf";
			string seer = (SeerCount > 0) ? "1 seer, " : "";
			string vanilla = (VanillaCount > 1) ? "villagers" : "villager";
			sb.AppendFormat("{0} players: {1} {2}, {3}{4} vanilla {5}", live.Count(), 
				WolfCount, wolf, 
				seer, 
				VanillaCount, vanilla);
			sb.AppendLine();
			sb.AppendLine();

			sb.AppendLine("[u]Live Players:[/u]");
			foreach (string p in live)
			{
				sb.AppendLine(p);
			}
			sb.AppendLine();
			sb.AppendLine(MakeGoodBad(_nextNight));
			sb.AppendLine();
			if (_majorityLynch)
			{
				int majCount = 1 + (live.Count() / 2);
				sb.AppendFormat("{0} votes is majority.", majCount);
			}
			else
			{
				sb.AppendLine("No majority lynch today.");
			}
			if (day == 0)
			{
				sb.Append("\n\n[sub]Note: [b][color=red]Error[/color][/b] vote can be fixed by sending me a PM with title 'correction' and body x=y. ");
				sb.AppendLine(@"Where x is what the person bolded and y is the player they meant to vote. This is useful at must lynch.[/sub]");
				sb.AppendLine("[sub]Note: You can sub by sending me a PM with title 'sub'. Put the player you are replacing in the body.[/sub]\n");
			}
			if ((_count != null) && _count.LockedVotes)
			{
				sb.AppendLine();
				sb.AppendLine();
				sb.AppendLine("[highlight]Caution: Votes are locked in today. Your first vote is final.[/highlight]");
				sb.AppendLine();
			}
			sb.AppendLine();
			sb.AppendLine("[b]It is day![/b]");
			if (day == 0)
			{
				if (_hyper == false)
				{
					String url = _forum.NewThread(59, _threadTitle, sb.ToString(), 15, false);
					_threadId = Misc.TidFromURL(url);
					_url = url;
				}
				else
				{
					_forum.LockThread(_threadId, false);
					_forum.MakePost(_threadId, "Mod: It is day!", sb.ToString(), 0, false);
				}

				ThreadReader t = _forum.Reader();
				Action<Action> invoker = a => a();
				_count = new ElectionInfo(invoker, t, _db, _forum.ForumURL,
					_url,
					_forum.PostsPerPage, Language.English);
				_count.Census.Clear();
                _count.CheckMajority = false;

				var players = from p in _playerRoles where p.Dead == false select p.Name;
				foreach (var p in players)
				{
					CensusEntry ce = new CensusEntry();
					ce.Name = p;
					ce.Alive = "Alive";
					_count.Census.Add(ce);
				}
				_count.CommitRosterChanges();
			}
			else
			{
				if (day != 1)
				{
					_forum.LockThread(_threadId, false);
					_forum.MakePost(_threadId, "Mod: It is day!", sb.ToString(), 0, false);
				}
			}
		}
		Boolean IsSeerAlive()
		{
			var seer = (from p in _playerRoles where (p.Dead == false) && (p.Role == SEER) select p).FirstOrDefault();
			if (seer == null)
			{
				return false;
			}
			return true;
		}
		Int32 GetMajorityCount()
		{
			if (!_majorityLynch) return Int32.MaxValue;
			var live = from p in _playerRoles where p.Dead == false select p.Name;
			int majCount = 1 + (live.Count() / 2);
			return majCount;
		}
		Player KillPlayer(string who)
		{
			_count.KillPlayer(who);
			Player player = LookupPlayer(who);
			player.Dead = true;

			var live = from p in _playerRoles where p.Dead == false select p.Name;
			Trace.TraceInformation("*** Begin Live Player List ***");
			foreach (var p in live)
			{
				Trace.TraceInformation("live: {0}", p);
			}
			Trace.TraceInformation("*** End Live Player List ***");
			return player;
		}
		Player LookupPlayer(Int32 id)
		{
			Player rc = (from p in _playerRoles where p.Id == id select p).FirstOrDefault();
			return rc;
		}
		Player LookupPlayer(String name)
		{
			var player = (from p in _playerByName
						  where
							  p.Key.Equals(name, StringComparison.InvariantCultureIgnoreCase)
						  select p.Value).FirstOrDefault();
			return player;
		}
		private bool ParseKillPM(PrivateMessage pm)
		{
			List<String> rawList = pm.Content.Split(
				new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.Distinct().ToList();
			foreach (var line in rawList)
			{
				var player = LookupPlayer(line);
				if ((player != null) && (player.Dead == false) && (player.Team != WOLF))
				{
					_kill = player.Name;
					return true;
				}

			}
			var wolves = from p in _playerRoles where (p.Dead == false) && (p.Team == WOLF) select p.Name;
			var villagers = from p in _playerRoles where (p.Dead == false) && (p.Team == VILLAGE) orderby p.Name.ToUpperInvariant() select p.Name;
			String msg = "Submit your kill promptly. It should be a PM that contains only the name of the villager you want to kill.\r\n\r\n live villagers:\r\n";
			foreach (var v in villagers)
			{
				msg += v + "\r\n";
			}
			QueuePM(wolves, "NIGHT ACTION FAILED", msg);
			return false;
		}
		DateTime _lastPMTime = DateTime.MinValue;
		Queue<PrivateMessage> _pmQueue = new Queue<PrivateMessage>();
		void SendPM(PrivateMessage pm)
		{
			_forum.SendPM(pm);
			_lastPMTime = DateTime.Now;
		}
		void SetPMTimer()
		{
			if (!_timerPM.Enabled)
			{
				DateTime ok = _lastPMTime.AddSeconds(31);
				TimeSpan ts = ok - DateTime.Now;
				if (ts.Ticks < 0)
				{
					UnqueuePM();
				}
				if (_pmQueue.Count > 0)
				{
					_timerPM.Interval = ts.TotalMilliseconds;
					_timerPM.Start();
				}
			}
		}
		void UnqueuePM()
		{
			PrivateMessage pm = null;
			lock (_pmQueue)
			{
				if (_pmQueue.Count > 0)
				{
					pm = _pmQueue.Dequeue();
				}
			}
			if(pm != null)
			{
				SendPM(pm);
				SetPMTimer();			
			}
		}
		void QueuePM(PrivateMessage pm)
		{
			lock (_pmQueue)
			{
				_pmQueue.Enqueue(pm);
			}
			SetPMTimer();
		}
		private void QueuePM(IEnumerable<string> bcc, string title, string msg)
		{
			PrivateMessage pm = new PrivateMessage(null, bcc, title, msg);
			QueuePM(pm);
		}
		List<String> _annoucementQueue = new List<string>();
		void QueueAnnouncement(string msg)
		{
			lock (_annoucementQueue)
			{
				_annoucementQueue.Add(msg);
			}
		}
		String GetAnnouncements()
		{
			StringBuilder rc = new StringBuilder();
			lock (_annoucementQueue)
			{
				foreach (var msg in _annoucementQueue)
				{
					rc.AppendLine(msg);
				}
				_annoucementQueue.Clear();
			}
			return rc.ToString();
		}
		private Player ParseSubPM(PrivateMessage pm)
		{
			List<String> rawList = pm.Content.Split(
				new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.Distinct().ToList();
			foreach (var line in rawList)
			{
				string l = line;
				if (pm.Content.ToLowerInvariant().Trim().StartsWith("sub "))
				{
					l = l.Substring(4).Trim();
				}
				var player = LookupPlayer(l);
				if ((player != null) && (player.Dead == false))
				{
					return player;
				}
			}
			return null;
		}
		private Player ParsePeekPM(PrivateMessage pm)
		{
			List<String> rawList = pm.Content.Split(
				new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
				.Select(p => p.Trim())
				.Distinct().ToList();
			foreach (var line in rawList)
			{
				var player = LookupPlayer(line);
				if ((player != null) && (player.Dead == false))
				{
					return player;
				}

			}
			var peekCandidates = GetPeekCandidates(pm.From);
			
			String msg = "Submit your peek promptly. It should be a PM that contains only the name of the player you want to peek. \r\n\r\nUnpeeked players:\r\n";
			foreach (var v in peekCandidates)
			{
				msg += v + "\r\n";
			}
			QueuePM(new String[] {pm.From}, "NIGHT ACTION FAILED", msg);
			return null;
		}
		IEnumerable<Player> GetPeekCandidates(String seerName)
		{
			var players = from p in _playerRoles where (p.Dead == false) orderby p.Name.ToUpperInvariant() select p;
			var peekCandidates = players.Except(_peeks).
				Where(p => p.Name.Equals(seerName, StringComparison.InvariantCultureIgnoreCase) == false);
			return peekCandidates;
		}
		void SetDayDuration()
		{
			Int32 m = (_day < 1) ? _d1Duration : _dDuration;
			_nextNight = TruncateSeconds(DateTime.Now).AddMinutes(m);
		}
		void SetNightDuration()
		{
			Int32 m = IsSeerAlive() ? _n1Duration : _nDuration; 
		   
			DateTime now = TruncateSeconds(DateTime.Now);
			_nextDay = now.AddMinutes(m);
			_EarliestPMTime = now.AddMinutes(-2);
		}
		#endregion
	}
}
