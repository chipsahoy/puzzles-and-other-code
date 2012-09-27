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
            _synchronousInvoker = synchronousInvoker;
            String dbName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\POG\\pogposts.sqlite";
            _db = new PogSqlite();
            _db.Connect(dbName);
        }

        public void SetNameFragment(string name)
        {
            IEnumerable<Poster> posters = _forum.GetPostersLike(name);
            _db.AddPosters(posters);
            IEnumerable<String> names = _db.GetPostersLike(name);
            OnCompletionList(name, names);
        }
        public event EventHandler<NameCompletionEventArgs> CompletionList;
        private void OnCompletionList(String fragment, IEnumerable<String> names)
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
