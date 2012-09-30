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

        public Int32 GetPosterId(String name, Action<String, Int32> callback)
        {
            Int32 id = 0;
            id = _db.GetPlayerId(name);
            if (id <= 0)
            {
                _forum.GetPostersLike(name,
                    (fragment, posters) =>
                    {
                        _synchronousInvoker(() =>
                            {
                                _db.AddPosters(posters);
                                foreach (Poster p in posters)
                                {
                                    if(p.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                                    {
                                        callback(p.Name, p.Id);
                                        return;
                                    }
                                }
                                callback(name, 0);
                            }
                        );
                    }
                );
            }
            return id;
        }
        public void LookupNameFragment(string name, Action<String, IEnumerable<Poster>> callback)
        {
                _forum.GetPostersLike(name,
                    (fragment, posters) =>
                    {
                        _synchronousInvoker(() =>
                            {
                                _db.AddPosters(posters);
                                IEnumerable<Poster> rc = _db.GetPostersLike(name);
                                callback(name, rc);
                            }
                        );
                    }
                );
        }
    }
}
