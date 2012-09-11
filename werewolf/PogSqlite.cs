using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using POG.Forum;
using System.IO;
using POG.Utils;

namespace POG.Werewolf
{
    internal class PogSqlite
    {
        String _connect;
        String _dbName;

        public PogSqlite(string dbName)
        {
            _dbName = dbName;
            String connect = String.Format("Data Source={0};Version=3;", dbName);
            _connect = connect;
        }
        public void WriteDayBoundaries(Int32 threadId, String url, Int32 day, Int32 startPost, DateTime endTime)
        {
            String sql = @"
				INSERT OR REPLACE INTO [threads] (
                id,
                url,
				startPost, 
				endofDay)
                VALUES (@p1, @p2, @p3, @p4);
			";
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", url));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", startPost));
                    SQLiteParameter pEod = new SQLiteParameter("@p4", System.Data.DbType.DateTime);
                    pEod.Value = endTime.ToUniversalTime();
                    cmd.Parameters.Add(pEod);
                    int e = cmd.ExecuteNonQuery();
                }
            }
            Trace.TraceInformation("after WriteDayInfo");
        }
        public void ReplacePlayerList(Int32 threadId, IEnumerable<String> players)
        {
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteTransaction trans = dbWrite.BeginTransaction())
                {
                    String sqlDelete =
                        @"DELETE 
						FROM players 
						where (threadId = @p1);";
                    using (SQLiteCommand cmd = new SQLiteCommand(sqlDelete, dbWrite, trans))
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                        int e = cmd.ExecuteNonQuery();
                    }
                    String sql =

                        @"INSERT OR REPLACE INTO [players] (
                    threadId,
                    player,
					dead)
                    VALUES (@p1, @p2, @p3);";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
                    {
                        SQLiteParameter pThreadId = new SQLiteParameter("@p1");
                        SQLiteParameter pPoster = new SQLiteParameter("@p2");
                        SQLiteParameter pDead = new SQLiteParameter("@p3");
                        cmd.Parameters.Add(pThreadId);
                        cmd.Parameters.Add(pPoster);
                        cmd.Parameters.Add(pDead);
                        pThreadId.Value = threadId;

                        foreach (String player in players)
                        {
                            pPoster.Value = player;
                            pDead.Value = 0;
                            int e = cmd.ExecuteNonQuery();
                        }
                    }
                    trans.Commit();
                }
            }
            Trace.TraceInformation("after ReplacePlayerList");
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
            Trace.TraceInformation("after WriteAlias");
        }
        public void SetIgnoreOnBold(Int32 postId, Int32 boldPosition, Boolean ignore)
        {
            String sql = @"UPDATE OR IGNORE bolds
                    SET ignore = @p1
                    WHERE (postId = @p2) AND (position = @p3);";
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
            Trace.TraceInformation("after ignore vote");
        }
        public void WriteUnhide(Int32 threadId, String player, Int32 startPostId, DateTimeOffset endTime)
        {
            // looking for a later post that is ignored.
            // needs to match: thread, poster, timestamp.
            String sql = @"SELECT bolds.postId, bolds.position
            FROM bolds INNER JOIN posts ON (bolds.postId = posts.id)
            WHERE (posts.poster = @p1) AND (posts.threadId = @p2) AND
            (bolds.postId >= @p4) AND (posts.time <= @p3) AND
            (bolds.ignore <> 0)
            ORDER BY bolds.postId ASC, bolds.position ASC LIMIT 1";
            Int32 postNewId = -1;
            Int32 position = -1;
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
            Trace.TraceInformation("After WriteUnhide");
            if (postNewId > 0)
            {
                SetIgnoreOnBold(postNewId, position, false);
            }
        }

        public void CreateMissingTablesDB(String _dbName)
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
                @"CREATE TABLE IF NOT EXISTS [posts] (
                    [id] INTEGER NOT NULL PRIMARY KEY,
                    [threadId] INTEGER,
                    [poster] TEXT COLLATE NOCASE,
                    [number] INTEGER,
                    [content] TEXT,
                    [title] TEXT,
                    [time] TIMESTAMP
                );",
                @"CREATE TABLE IF NOT EXISTS [threads] (
                    [id] INTEGER NOT NULL PRIMARY KEY,
                    [url] TEXT,
                    [title] TEXT,
					[startPost] INTEGER,
					[endOfDay] TIMESTAMP,
					[isTurbo] INTEGER
                );",
                @"CREATE TABLE IF NOT EXISTS [bolds] (
                    [postId] INTEGER NOT NULL,
                    [position] INTEGER,
                    [bolded] TEXT,
                    [ignore] INTEGER,
                    PRIMARY KEY(postId, position)
                );",
                @"CREATE TABLE IF NOT EXISTS [players] (
                    [threadId] INTEGER,
                    [player] TEXT COLLATE NOCASE,
					[dead] INTEGER,
                    [role] TEXT,
                    [team] TEXT,
                    [obituaryPost] INTEGER,
					[birthPost] INTEGER,
					[causeOfDeath] TEXT,
                    PRIMARY KEY(threadId, player)
                );",
                @"CREATE TABLE IF NOT EXISTS [aliases] (
                    [threadId] INTEGER,
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
        public void AddPostsToDB(Posts posts)
        {
            using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
            {
                dbWrite.Open();
                using (SQLiteTransaction trans = dbWrite.BeginTransaction())
                {
                    String sql =

                        @"INSERT OR REPLACE INTO [posts] (
                    id,
                    threadId,
                    poster,
                    number,
                    content,
                    title,
                    time)
                    VALUES (@p1, @p2, @p3, @p4, @p5, @p6, @p7);";

                    using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
                    {
                        SQLiteParameter pPostId = new SQLiteParameter("@p1");
                        SQLiteParameter pThreadId = new SQLiteParameter("@p2");
                        SQLiteParameter pPoster = new SQLiteParameter("@p3");
                        SQLiteParameter pPostNumber = new SQLiteParameter("@p4");
                        SQLiteParameter pContent = new SQLiteParameter("@p5");
                        SQLiteParameter pTitle = new SQLiteParameter("@p6");
                        SQLiteParameter pTime = new SQLiteParameter("@p7", System.Data.DbType.DateTime);
                        cmd.Parameters.Add(pPostId);
                        cmd.Parameters.Add(pThreadId);
                        cmd.Parameters.Add(pPoster);
                        cmd.Parameters.Add(pPostNumber);
                        cmd.Parameters.Add(pContent);
                        cmd.Parameters.Add(pTitle);
                        cmd.Parameters.Add(pTime);

                        foreach (Post p in posts)
                        {
                            pPostId.Value = p.PostId;
                            pThreadId.Value = p.ThreadId;
                            pPoster.Value = p.Poster.Name;
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
                    trans.Commit();
                }
            }

            Trace.TraceInformation("after AddPostsToDB");
        }

        //  -- Reads --
        public DateTime? GetPostTime(Int32 threadId, Int32 postNumber)
        {
            DateTime? rc = null;
            String sqlTime = @"SELECT time
                        FROM posts
                        WHERE posts.threadId = @p2 AND
                        (posts.number == @p4);";
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
                Trace.TraceInformation("after GetPostTime");
            }
            return rc;
        }
        public Int32? GetMaxPostDB(Int32 threadId)
        {
            String sql = @"SELECT number FROM posts WHERE threadId=@p1 ORDER BY id DESC LIMIT 1;";
            object o;
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    o = cmd.ExecuteScalar();
                }
            }
            Trace.TraceInformation("after get max post #");
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
            String sql = @"SELECT player
						FROM players 
						WHERE (threadId = @p1)
						ORDER BY player ASC;";
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
            Trace.TraceInformation("after GetPlayerList");
            return players;
        }
        internal void GetVotes(Int32 threadId, Int32 startPost, DateTime endTime, IEnumerable<Voter> voters)
        {
            String sqlPostCount = @"SELECT COUNT()
                        FROM posts
                        WHERE (posts.poster = @p1) AND (posts.threadId = @p2) AND
                        (posts.number >= @p4) AND (posts.time <= @p3);";

            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmdCount = new SQLiteCommand(sqlPostCount, dbRead))
                {
                    SQLiteParameter p1 = new SQLiteParameter("@p1");
                    cmdCount.Parameters.Add(p1);
                    cmdCount.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = endTime.ToUniversalTime();
                    cmdCount.Parameters.Add(pEndTime);
                    cmdCount.Parameters.Add(new SQLiteParameter("@p4", startPost));
                    foreach (Voter p in voters)
                    {
                        //var playerPosts = from post in qry where (String.Equals(post.Poster, p.Name, StringComparison.InvariantCultureIgnoreCase)) select post;
                        //p.SetPosts(playerPosts);
                        p1.Value = p.Name;
                        object o = cmdCount.ExecuteScalar();
                        if (!(o is System.DBNull))
                        {
                            p.PostCount = (Int32)(long)o;
                        }
                        else
                        {
                            p.PostCount = 0;
                        }
                    }

                }
                String sql = @"SELECT bolds.bolded, posts.number, posts.time, posts.id, bolds.position
                        FROM bolds INNER JOIN posts ON (bolds.postId = posts.id)
                        WHERE (posts.poster = @p1) AND (posts.threadId = @p2) AND
                        (posts.number >= @p4) AND (posts.time <= @p3) AND
                        (bolds.ignore = 0)
                        ORDER BY bolds.postId DESC, bolds.position DESC LIMIT 1";
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    SQLiteParameter p1 = new SQLiteParameter("@p1");
                    cmd.Parameters.Add(p1);
                    cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = endTime.ToUniversalTime();
                    cmd.Parameters.Add(pEndTime);
                    cmd.Parameters.Add(new SQLiteParameter("@p4", startPost));
                    foreach (Voter p in voters)
                    {
                        //var playerPosts = from post in qry where (String.Equals(post.Poster, p.Name, StringComparison.InvariantCultureIgnoreCase)) select post;
                        //p.SetPosts(playerPosts);
                        p1.Value = p.Name;
                        using (SQLiteDataReader r = cmd.ExecuteReader())
                        {
                            if (r.Read())
                            {

                                String bolded = r.GetString(0);
                                Int32 postNumber = r.GetInt32(1);
                                DateTime postTime = DateTime.SpecifyKind(r.GetDateTime(2), DateTimeKind.Local);
                                Int32 postId = r.GetInt32(3);
                                Int32 boldPosition = r.GetInt32(4);
                                p.SetVote(bolded, postNumber, postTime, postId, boldPosition);
                            }
                            else
                            {
                                p.ClearVote();
                            }
                        }
                    }
                }
            }
        }
        public void GetDayBoundaries(Int32 threadId, out Int32 day, out Int32 startPost,
                out DateTime endTime, out Int32 endPost)
        {
            day = 0;
            startPost = 0;
            endTime = DateTime.Now;
            endPost = 0;

            String sql = @"
				SELECT startPost, endofDay FROM [threads] 
                WHERE (id = @p1)
                LIMIT 1;
			";

            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();

                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            startPost = r.GetInt32(0);
                            endTime = DateTime.SpecifyKind(r.GetDateTime(1), DateTimeKind.Local);
                        }
                    }
                }
            }
            Trace.TraceInformation("after ReadDayBoundaries");

            String sqlTime = @"SELECT number
                        FROM posts
                        WHERE posts.threadId = @p2 AND (posts.time > @p3)
                        ORDER BY id ASC LIMIT 1";
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sqlTime, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
                    SQLiteParameter pEndTime = new SQLiteParameter("@p3", System.Data.DbType.DateTime);
                    pEndTime.Value = endTime.ToUniversalTime();
                    cmd.Parameters.Add(pEndTime);
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            Int32 nightPost = r.GetInt32(0);
                            endPost = nightPost - 1;
                            //Trace.TraceInformation("VC: first night post: " + nightPost.ToString());
                        }
                    }
                }
            }
            Trace.TraceInformation("after get EndPost");

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
            Trace.TraceInformation("after GetAliasDB");

            // Check other threads.
            return rc;
        }
    }
}
