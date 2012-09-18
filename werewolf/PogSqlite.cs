using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using POG.Forum;
using System.IO;
using POG.Utils;

namespace POG.Werewolf
{
    public class PogSqlite : POG.Werewolf.IPogDb
    {
        String _connect;
        String _dbName;

        public PogSqlite()
        {
        }
        public void Connect(string dbName)
        {
            _dbName = dbName;
            String connect = String.Format("Data Source={0};Version=3;", dbName);
            _connect = connect;
            CreateMissingTables(dbName);
        }
        public void WriteDayBoundaries(Int32 threadId, String url, Boolean turbo, Int32 day, Int32 startPost, DateTime endTime)
        {
            String sql = @"
				INSERT OR REPLACE INTO [threads] (
                id,
                url,
                isTurbo)
                VALUES (@p1, @p2, @p3);
			";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", url));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", turbo));
                    int e = cmd.ExecuteNonQuery();
                }
                sql = @"INSERT OR REPLACE INTO [days] (
                threadId,
                day,
				startPost, 
				endTime)
                VALUES (@p1, @p2, @p3, @p4);";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", day));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", startPost));
                    SQLiteParameter pEod = new SQLiteParameter("@p4", System.Data.DbType.DateTime);
                    pEod.Value = endTime.ToUniversalTime();
                    cmd.Parameters.Add(pEod);
                    int e = cmd.ExecuteNonQuery();
                }
            }
            watch.Stop();
            Trace.TraceInformation("after WriteDayInfo {0}", watch.Elapsed.ToString());
        }
        public void ReplacePlayerList(Int32 threadId, IEnumerable<String> players)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                var pragma = new SQLiteCommand("PRAGMA foreign_keys = true;", dbWrite);
                pragma.ExecuteNonQuery();
                using (SQLiteTransaction trans = dbWrite.BeginTransaction())
                {

                    String sqlDelete =
                        @"DELETE 
						FROM roles 
						where (threadId = @p1);";
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlDelete, dbWrite, trans))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                        int e = cmd.ExecuteNonQuery();
                    }
                    String sql =

                        @"INSERT INTO [roles] (
                    threadId)
                    VALUES (@p1);
                    SELECT last_insert_rowid();";

                    string sqlPlayer =
                        @"INSERT INTO [players] (roleId, playerId) VALUES(@p1, @p2);";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
                    {
                        SQLiteParameter pThreadId = new SQLiteParameter("@p1");
                        cmd.Parameters.Add(pThreadId);
                        pThreadId.Value = threadId;

                        foreach (String player in players)
                        {
                            Int32 id = -1;
                            using (SQLiteDataReader r = cmd.ExecuteReader())
                            {
                                if (r.Read())
                                {
                                    id = r.GetInt32(0);
                                }
                            }
                            if (id == -1)
                            {
                                continue;
                            }

                            using (SQLiteCommand cmdPlayer = new SQLiteCommand(sqlPlayer, dbWrite, trans))
                            {
                                SQLiteParameter pRoleId = new SQLiteParameter("@p1");
                                pRoleId.Value = id;
                                SQLiteParameter pPosterId = new SQLiteParameter("@p2");
                                pPosterId.Value = GetPlayerId(player);
                                cmdPlayer.Parameters.Add(pRoleId);
                                cmdPlayer.Parameters.Add(pPosterId);
                                int e = cmdPlayer.ExecuteNonQuery();
                            }
                        }
                    }
                    trans.Commit();
                }
            }
            watch.Stop();

            Trace.TraceInformation("after ReplacePlayerList {0}", watch.Elapsed.ToString());
        }

        private Int32 GetPlayerId(string player)
        {
            String sql = @"SELECT id
            FROM posters
            WHERE (name = @p1)
            LIMIT 1";
            Int32 id = -1;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", player));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            id = r.GetInt32(0);
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("After GetPlayerId {0}", watch.Elapsed.ToString());
            return id;
        }
        public void SubPlayer(Int32 threadId, String player, String sub, Int32 post)
        {
        }
        public void KillPlayer(Int32 threadId, String player, String cause, String team, Int32 post)
        {
        }
        public void WriteAlias(Int32 threadId, String bolded, String player)
        {
            String sql = @"
				INSERT OR REPLACE INTO [aliases] (
				threadId,
                bolded,
                player
				)
                VALUES (@p1, @p2, @p3);
			";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", bolded));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", player));
                    int e = cmd.ExecuteNonQuery();
                }
            }
            watch.Stop();
            Trace.TraceInformation("after WriteAlias {0}", watch.Elapsed.ToString());
        }
        public void SetIgnoreOnBold(Int32 postId, Int32 boldPosition, Boolean ignore)
        {
            String sql = @"UPDATE OR IGNORE bolds
                    SET ignore = @p1
                    WHERE (postId = @p2) AND (position = @p3);";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", ignore));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", postId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", boldPosition));
                    int e = cmd.ExecuteNonQuery();
                }
            }
            watch.Stop();
            Trace.TraceInformation("after ignore vote {0}", watch.Elapsed.ToString());
        }
        public void WriteUnhide(Int32 threadId, String player, Int32 startPostId, DateTimeOffset endTime)
        {
            String sql = @"SELECT bolds.postId, bolds.position
            FROM bolds INNER JOIN posts ON (bolds.postId = posts.id)
            INNER JOIN posters ON (posts.posterId = posters.id)
            WHERE (posters.name = @p1) AND (posts.threadId = @p2) AND
            (bolds.postId >= @p4) AND (posts.time <= @p3) AND
            (bolds.ignore <> 0)
            ORDER BY bolds.postId ASC, bolds.position ASC LIMIT 1";
            Int32 postNewId = -1;
            Int32 position = -1;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", player));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = endTime.ToUniversalTime().DateTime;
                    cmd.Parameters.Add(pEndTime);
                    cmd.Parameters.Add(new SQLiteParameter("@p4", startPostId));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            postNewId = r.GetInt32(0);
                            position = r.GetInt32(1);
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("After WriteUnhide {0}", watch.Elapsed.ToString());
            if (postNewId > 0)
            {
                SetIgnoreOnBold(postNewId, position, false);
            }
        }
        static Int32 _schemaVersion = 1;
        private void CreateMissingTables(String _dbName)
        {
            
            if (!File.Exists(_dbName))
            {
                SQLiteConnection.CreateFile(_dbName);
            }
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteTransaction trans = dbWrite.BeginTransaction())
                {
                    String[] tables = 
            {
@"CREATE TABLE IF NOT EXISTS [meta] (
    [version] INTEGER DEFAULT " + _schemaVersion.ToString() + @" PRIMARY KEY
);",

@"CREATE TABLE IF NOT EXISTS [threads] (
    [id] INTEGER NOT NULL PRIMARY KEY,
    [url] TEXT,
    [title] TEXT,
    [isTurbo] INTEGER,
    [isActive] INTEGER DEFAULT 1
);",
@"CREATE TABLE IF NOT EXISTS [days] (
    [threadId] INTEGER REFERENCES threads(id) ON DELETE CASCADE,
    [day] INTEGER,
    [startPost] INTEGER,
    [endTime] TIMESTAMP,
    PRIMARY KEY(threadId, day)
);",

@"CREATE TABLE IF NOT EXISTS [posters] (
    [id] INTEGER NOT NULL PRIMARY KEY,
    [name] TEXT COLLATE NOCASE
);",

@"CREATE TABLE IF NOT EXISTS [roles] (
    [id] INTEGER PRIMARY KEY,
    [threadId] INTEGER REFERENCES threads(id) ON DELETE CASCADE,
    [birthPostNumber] INTEGER DEFAULT 1,
    [deathPostNumber] INTEGER,
    [team] TEXT,
    [role] TEXT,
    [pm] TEXT
);",

@"CREATE TABLE IF NOT EXISTS [posts] (
    [id] INTEGER NOT NULL PRIMARY KEY,
    [threadId] INTEGER REFERENCES threads(id) ON DELETE CASCADE,
    [posterId] INTEGER REFERENCES posters(id) ON DELETE CASCADE,
    [number] INTEGER,
    [content] TEXT,
    [title] TEXT,
    [time] TIMESTAMP
);",
@"CREATE TABLE IF NOT EXISTS [bolds] (
    [postId] INTEGER NOT NULL REFERENCES posts(id) ON DELETE CASCADE,
    [position] INTEGER,
    [bolded] TEXT,
    [ignore] INTEGER,
    PRIMARY KEY(postId, position)
);",
@"CREATE TABLE IF NOT EXISTS [players] (
    [roleId] INTEGER REFERENCES roles(id) ON DELETE CASCADE,
    [playerId] INTEGER REFERENCES posters(id) ON DELETE CASCADE,
    [startPostNumber] INTEGER DEFAULT 1,
    [endPostNumber] INTEGER,
    PRIMARY KEY(roleId, playerId)
);",
@"CREATE TABLE IF NOT EXISTS [aliases] (
    [threadId] INTEGER REFERENCES threads(id) ON DELETE CASCADE,
    [bolded] TEXT COLLATE NOCASE,
    [player] TEXT COLLATE NOCASE,
    PRIMARY KEY(threadId, bolded)
);",
            };
                    foreach (String sql in tables)
                    {
                        using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
                        {
                            int e = cmd.ExecuteNonQuery();
                        }
                    }

                    trans.Commit();
                }
            }
            Trace.TraceInformation("after create tables");
        }
        public void AddPosts(Posts posts)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteTransaction trans = dbWrite.BeginTransaction())
                {
                    String sql =

                        @"INSERT OR IGNORE INTO [posters] (id, name) VALUES (@p3, @p8);

INSERT OR REPLACE INTO [posts] (
                    id,
                    threadId,
                    posterId,
                    number,
                    content,
                    title,
                    time)
                    VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7);";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
                    {
                        SQLiteParameter pPostId = new SQLiteParameter("@p1");
                        SQLiteParameter pThreadId = new SQLiteParameter("@p2");
                        SQLiteParameter pPosterId = new SQLiteParameter("@p3");
                        SQLiteParameter pPostNumber = new SQLiteParameter("@p4");
                        SQLiteParameter pContent = new SQLiteParameter("@p5");
                        SQLiteParameter pTitle = new SQLiteParameter("@p6");
                        SQLiteParameter pTime = new SQLiteParameter("@p7", System.Data.DbType.DateTime);
                        SQLiteParameter pPosterName = new SQLiteParameter("@p8");
                        cmd.Parameters.Add(pPostId);
                        cmd.Parameters.Add(pThreadId);
                        cmd.Parameters.Add(pPosterName);
                        cmd.Parameters.Add(pPosterId);
                        cmd.Parameters.Add(pPostNumber);
                        cmd.Parameters.Add(pContent);
                        cmd.Parameters.Add(pTitle);
                        cmd.Parameters.Add(pTime);

                        foreach (Post p in posts)
                        {
                            pPostId.Value = p.PostId;
                            pThreadId.Value = p.ThreadId;
                            pPosterName.Value = p.Poster.Name;
                            pPosterId.Value = p.Poster.Id;
                            pPostNumber.Value = p.PostNumber;
                            pContent.Value = p.Content;
                            pTitle.Value = p.Title;
                            pTime.Value = p.Time.UtcDateTime;
                            int e = cmd.ExecuteNonQuery();

                            int ix = 0;
                            String sqlBold =
                                @"INSERT OR IGNORE INTO [bolds] (
                                postId,
                                position,
                                bolded,
                                ignore)
                                VALUES (@p1, @p2, @p3, @p4);";
                            if (p.Bolded != null)
                            {
                                using (SQLiteCommand cmdBold = new SQLiteCommand(sqlBold, dbWrite, trans))
                                {
                                    SQLiteParameter pForeignPostId = new SQLiteParameter("@p1");
                                    SQLiteParameter pIx = new SQLiteParameter("@p2");
                                    SQLiteParameter pBolded = new SQLiteParameter("@p3");
                                    SQLiteParameter pIgnore = new SQLiteParameter("@p4");
                                    cmdBold.Parameters.Add(pForeignPostId);
                                    cmdBold.Parameters.Add(pIx);
                                    cmdBold.Parameters.Add(pBolded);
                                    cmdBold.Parameters.Add(pIgnore);

                                    foreach (Bold b in p.Bolded)
                                    {
                                        pForeignPostId.Value = p.PostId;
                                        pIx.Value = ix;
                                        pBolded.Value = b.Content;
                                        pIgnore.Value = b.Ignore;
                                        e = cmdBold.ExecuteNonQuery();
                                        ix++;
                                    }
                                }
                            }
                        }
                    }
                    trans.Commit();
                }
            }
            watch.Stop();

            Trace.TraceInformation("after AddPostsToDB {0}", watch.Elapsed.ToString());
        }

        //  -- Reads --
        public DateTime? GetPostTime(Int32 threadId, Int32 postNumber)
        {
            DateTime? rc = null;
            String sqlTime = @"SELECT time
                        FROM posts
                        WHERE posts.threadId = @p2 AND
                        (posts.number == @p4);";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sqlTime, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", postNumber));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            //Trace.TraceInformation("VC: Found new start post " + value.ToString());
                            rc = DateTime.SpecifyKind(r.GetDateTime(0), DateTimeKind.Local);
                        }
                        else
                        {
                            //Trace.TraceInformation("VC: Could not find start post " + value.ToString());
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("after GetPostTime {0}", watch.Elapsed.ToString());
            return rc;
        }
        public Int32? GetMaxPost(Int32 threadId)
        {
            String sql = @"SELECT number FROM posts WHERE threadId=@p1 ORDER BY id DESC LIMIT 1;";
            object o;
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    o = cmd.ExecuteScalar();
                }
            }
            watch.Stop();
            Trace.TraceInformation("after get max post # {0}", watch.Elapsed.ToString());
            long rc;
            if ((o == null) || (o is System.DBNull))
            {
                return null;
            }
            else
            {
                rc = (long)o;
            }
            return (Int32)rc;
        }

        public List<String> GetLivePlayers(Int32 threadId, Int32 postNumber)
        {
            List<String> players = new List<string>();
            String sql = @"SELECT posters.Name
						FROM posters
                        INNER JOIN players ON (posters.id = players.playerId)
                        INNER JOIN [roles] ON (players.roleId = roles.id)
						WHERE (roles.threadId = @p1)
						ORDER BY posters.Name ASC;";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            String player = r.GetString(0);
                            players.Add(player);
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("after GetPlayerList {0}", watch.Elapsed.ToString());
            return players;
        }
        public SortableBindingList<Voter> GetVotes(Int32 threadId, Int32 startPost, DateTime endTime, object game)
        {
            String sql = @"
SELECT [roles].id, players.playerId, posters.name, 
(SELECT COUNT(*)  
    FROM posts WHERE
    (posts.posterId = players.playerId)
    AND (posts.number >= @p4) 
    AND (posts.time <= @p3)
) AS postcount,
(SELECT MAX(postId)
    FROM bolds, posts WHERE
    (bolds.postId = posts.id)
    AND (posts.posterId = players.playerId)
    AND (posts.number >= @p4) 
    AND (posts.time <= @p3)
    AND (bolds.ignore = 0)
) AS bolded
FROM [roles] 
JOIN [players] ON ([roles].id = players.roleId)
JOIN [posters] ON (posters.id = players.playerId)
WHERE ([roles].threadId = @p2) 
GROUP BY [posters].name
;
";
            SortableBindingList<Voter> voters = new SortableBindingList<Voter>();
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = endTime.ToUniversalTime();
                    cmd.Parameters.Add(pEndTime);
                    cmd.Parameters.Add(new SQLiteParameter("@p4", startPost));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            Int32 roleId = r.GetInt32(0);
                            Int32 playerId = r.GetInt32(1);
                            String name = r.GetString(2);
                            Int32 count = r.GetInt32(3);
                            Voter v = new Voter(game, roleId, playerId, name);
                            v.PostCount = count;
                            if (!r.IsDBNull(4))
                            {
                                Int32 boldedPost = r.GetInt32(4);
                                v.PostId = boldedPost;
                            }
                            voters.Add(v);
                        }
                    }

                }
                sql = @"
SELECT bolds.bolded, bolds.position, posts.number, posts.time
    FROM bolds
    JOIN posts ON (bolds.postId = posts.id)
    WHERE
    (bolds.postId = @p1)
    AND (bolds.ignore = 0)
    ORDER BY bolds.position DESC
    LIMIT 1
; 
";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    foreach (Voter v in voters)
                    {
                        Int32 id = v.PostId;
                        if (id <= 0)
                        {
                            v.ClearVote();
                            continue;
                        }
                        cmd.Parameters.Add(new SQLiteParameter("@p1", id));
                        using (SQLiteDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                String bolded = r.GetString(0);
                                Int32 position = r.GetInt32(1);
                                Int32 number = r.GetInt32(2);
                                DateTimeOffset time = r.GetDateTime(3);
                                v.SetVote(bolded, number, time, id, position);
                            }
                        }
                    }

                }
            }
            watch.Stop();
            Trace.TraceInformation("after post counts {0}", watch.Elapsed.ToString());
            return voters;
        }

        public Boolean GetDayBoundaries(Int32 threadId, Int32 day, out Int32 startPost,
                out DateTime endTime, out Int32 endPost)
        {
            startPost = 0;
            endTime = DateTime.Now;
            endPost = 0;

            String sql = @"
				SELECT startPost, endTime FROM [days] 
                WHERE (threadId = @p1) AND (day = @p2)
                LIMIT 1;
			";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", day));

                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            startPost = r.GetInt32(0);
                            endTime = DateTime.SpecifyKind(r.GetDateTime(1), DateTimeKind.Local);
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("after ReadDayBoundaries {0}", watch.Elapsed.ToString());

            String sqlTime = @"SELECT number
                        FROM posts
                        WHERE (posts.threadId = @p2) AND (posts.time <= @p3) AND (posts.number >= @p1)
                        ORDER BY id DESC LIMIT 1";
            watch.Reset();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sqlTime, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", startPost));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = endTime.ToUniversalTime();
                    cmd.Parameters.Add(pEndTime);
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            endPost = r.GetInt32(0);
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("after get EndPost {0}", watch.Elapsed.ToString());
            return true;
        }
        public String GetAlias(Int32 threadId, String bolded)
        {
            String rc = String.Empty;
            // Check our thread.
            String sql = @"
				SELECT player FROM [aliases] 
                WHERE (threadId = @p1) AND (bolded = @p2)
                LIMIT 1;
			";
            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", bolded));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            rc = r.GetString(0);
                        }
                    }
                }
            }
            watch.Stop();
            Trace.TraceInformation("after GetAliasDB {0}", watch.Elapsed.ToString());

            // Check other threads.
            return rc;
        }
    }
}
