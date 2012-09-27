using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using POG.Forum;

namespace POG.Werewolf
{
    public class AutoComplete
    {
        IPogDb _db;
        Action<Action> _synchronousInvoker;
        TwoPlusTwoForum _forum;
        public AutoComplete(TwoPlusTwoForum forum, Action<Action> synchronousInvoker)
        {
            _forum = forum;
            _forum.NameCompletionEvent += new EventHandler<NameCompletionEventArgs>(_forum_NameCompletionEvent);
            _synchronousInvoker = synchronousInvoker;
            String dbName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\POG\\pogposts.sqlite";
            _db = new PogSqlite();
            _db.Connect(dbName);
        }

        void _forum_NameCompletionEvent(object sender, NameCompletionEventArgs e)
        {
            IEnumerable<Poster> posters = e.Names;
            _db.AddPosters(posters);
            IEnumerable<Poster> names = _db.GetPostersLike(e.Fragment);
            OnCompletionList(e.Fragment, names);
        }

        public void SetNameFragment(string name)
        {
            _forum.GetPostersLike(name);
        }
        public event EventHandler<NameCompletionEventArgs> CompletionList;
        private void OnCompletionList(String fragment, IEnumerable<Poster> names)
        {
            var handler = CompletionList;
            if (handler != null)
            {
                _synchronousInvoker.Invoke(
                    () => handler(this, new NameCompletionEventArgs(fragment, names))
                );
            }
        }
    }
}
