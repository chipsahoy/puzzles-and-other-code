using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Diagnostics;
using POG.Forum;
using System.IO;
using POG.Utils;

namespace POG.Werewolf
{
	public class PogSqlite : POG.Werewolf.IPogDb
	{
		static Int32 _schemaVersion = 2;
		String _connect;
		String _dbName;

		public PogSqlite()
		{
		}
		public void Connect(string dbName)
		{
			_dbName = dbName;
			String connect = String.Format("Data Source={0};Version=3;foreign keys=True;Synchronous=Off", dbName);
			_connect = connect;
			CreateOrUpgradeSchema(dbName);
		}
		// With each schema change, create a new UpgradeXToY method. 
		// It should modify a schema of version N to version N + 1.
		// Preserve data when possible. Leave the db sane no matter what.
		// Add the delegate to the Upgrades[] below.
		delegate void UpgradeSchema(SQLiteConnection db);
		void SetUserVersion(SQLiteConnection db, Int32 version)
		{
			String sql = String.Format(@"PRAGMA user_version = {0};", version);
			using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
			{
				cmd.Parameters.Add(new SQLiteParameter("@p1", version));                
				int e = cmd.ExecuteNonQuery();
			}
		}
		void Upgrade1To2(SQLiteConnection db)
		{
			String[] updates = {
@"ALTER TABLE Day ADD COLUMN startpost INTEGER DEFAULT 1",
@"ALTER TABLE Player ADD COLUMN startpost INTEGER DEFAULT 1",
@"ALTER TABLE Post ADD COLUMN title TEXT",
@"ALTER TABLE Post ADD COLUMN content TEXT",
			};
			foreach (String sql in updates)
			{
				using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
				{
					int e = cmd.ExecuteNonQuery();
				}
			}
		}
		void CreateTables(SQLiteConnection db)
		{
			String[] tables = 
			{
@"CREATE TABLE IF NOT EXISTS Thread (
	threadid INTEGER NOT NULL PRIMARY KEY,
	url TEXT,
	turbo INTEGER,
	active INTEGER DEFAULT 1
);",
@"CREATE TABLE IF NOT EXISTS Day (
	threadid INTEGER REFERENCES Thread(threadid) ON DELETE CASCADE,
	day INTEGER,
	startpost INTEGER DEFAULT 1,
	endtime TIMESTAMP,
	PRIMARY KEY(threadid, day)
);",

@"CREATE TABLE IF NOT EXISTS Poster (
	posterid INTEGER NOT NULL PRIMARY KEY,
	postername TEXT COLLATE NOCASE
);",
@"CREATE INDEX IF NOT EXISTS 
postersname
ON
Poster (postername)
;",

@"CREATE TABLE IF NOT EXISTS GameRole (
	roleid INTEGER PRIMARY KEY AUTOINCREMENT,
	threadid INTEGER REFERENCES Thread(threadid) ON DELETE CASCADE,
	birthtime INTEGER DEFAULT 1,
	deathtime INTEGER
);",
@"CREATE INDEX IF NOT EXISTS 
rolesthread
ON
GameRole (threadid)
;",
@"CREATE TABLE IF NOT EXISTS Post (
	postid INTEGER NOT NULL PRIMARY KEY,
	threadid INTEGER REFERENCES Thread(threadid) ON DELETE CASCADE,
	posterid INTEGER REFERENCES Poster(posterid) ON DELETE CASCADE,
	postnumber INTEGER,
	posttime TIMESTAMP,
	title TEXT,
	content TEXT
);",
@"CREATE INDEX IF NOT EXISTS 
poststhreadposter
ON
Post (threadid, posterid)
;",
@"CREATE INDEX IF NOT EXISTS 
poststhreadpostnumber
ON
Post (threadid, postnumber)
;",
@"CREATE INDEX IF NOT EXISTS 
poststhreadposttime
ON
Post (threadid, posttime)
;",
@"CREATE TABLE IF NOT EXISTS Bolded (
	postid INTEGER NOT NULL REFERENCES Post(postid) ON DELETE CASCADE,
	position INTEGER,
	bolded TEXT,
	ignore INTEGER,
	PRIMARY KEY(postid, position)
);",
@"CREATE TABLE IF NOT EXISTS Player (
	roleid INTEGER REFERENCES GameRole(roleid) ON DELETE CASCADE,
	posterid INTEGER REFERENCES Poster(posterid) ON DELETE CASCADE,
	startpost INTEGER,
	endtime TIMESTAMP,
	PRIMARY KEY(roleid, posterid)
);",
@"CREATE TABLE IF NOT EXISTS Alias (
	threadid INTEGER REFERENCES Thread(threadid) ON DELETE CASCADE,
	bolded TEXT COLLATE NOCASE,
	posterid TEXT COLLATE NOCASE,
	PRIMARY KEY(threadid, bolded)
);",
@"CREATE INDEX IF NOT EXISTS 
aliasesthread
ON
Alias (threadid)
;",
			};
			foreach (String sql in tables)
			{
				using (SQLiteCommand cmd = new SQLiteCommand(sql, db))
				{
					int e = cmd.ExecuteNonQuery();
				}
			}
		}
		private void CreateOrUpgradeSchema(String _dbName)
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
					String sqlVersion = "PRAGMA user_version;";
					Int32 dbVersion = 0;
					using (SQLiteCommand cmd = new SQLiteCommand(sqlVersion, dbWrite))
					{
						using (SQLiteDataReader r = cmd.ExecuteReader())
						{
							if (r.Read())
							{
								dbVersion = r.GetInt32(0);
							}
						}
					}
					if (dbVersion > _schemaVersion)
					{
						throw new BadSQLiteFileVersionException(dbVersion, _schemaVersion);
					}
					UpgradeSchema[] upgrades = { null, Upgrade1To2 };
					if (dbVersion > 0)
					{
						for (int i = dbVersion; i < _schemaVersion; i++)
						{
							UpgradeSchema upgrade = upgrades[i];
							if (upgrade != null)
							{
								upgrade(dbWrite);
							}
							SetUserVersion(dbWrite, i + 1);
						}
					}
					else
					{
						CreateTables(dbWrite);
						SetUserVersion(dbWrite, _schemaVersion);
					}
					trans.Commit();
				}
			}
			//Trace.TraceInformation("after create tables");
		}
		public void WriteThreadDefinition(Int32 threadId, String url, Boolean turbo)
		{
			String sql = 
@"
INSERT OR IGNORE INTO Thread (
threadid,
url,
turbo)
VALUES (@p1, @p2, @p3);
UPDATE Thread SET
url = @p2,
turbo = @p3
WHERE
(threadid = @p1);
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
			}
			watch.Stop();
			//Trace.TraceInformation("after WriteThreadDefinition {0}", watch.Elapsed.ToString());
		}
		public void WriteDayBoundaries(Int32 threadId, Int32 day, Int32 startPost, DateTime endTime)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
			{
				dbWrite.Open();
				String sql = 
@"INSERT OR REPLACE INTO Day (
threadid,
day,
startpost, 
endtime)
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
			//Trace.TraceInformation("after WriteDayInfo {0}", watch.Elapsed.ToString());
		}
		public void ReplacePlayerList(Int32 threadId, IEnumerable<String> players)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
			{
				dbWrite.Open();
				using (SQLiteTransaction trans = dbWrite.BeginTransaction())
				{

					String sqlDelete =
@"DELETE 
FROM GameRole 
where (threadid = @p1);";
					using (SQLiteCommand cmd = new SQLiteCommand(sqlDelete, dbWrite, trans))
					{
						cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
						int e = cmd.ExecuteNonQuery();
					}
					String sql =

@"INSERT INTO GameRole (
threadid)
VALUES (@p1);
SELECT last_insert_rowid();";

					string sqlPlayer =
@"INSERT INTO Player (roleid, posterid) VALUES(@p1, @p2);";

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

			//Trace.TraceInformation("after ReplacePlayerList {0}", watch.Elapsed.ToString());
		}
		public IEnumerable<CensusEntry> ReadRoster(Int32 threadId)
		{
			SortableBindingList<CensusEntry> census = new SortableBindingList<CensusEntry>();
			String sql = 
@"
SELECT Player.roleid, GameRole.deathtime, Player.posterid, Player.endtime, Poster.postername
FROM GameRole, Player, Poster
WHERE
(GameRole.threadid = @p1)
AND (Player.roleid = GameRole.roleid)
AND (Player.posterid = Poster.posterid)
ORDER BY GameRole.roleid ASC, 
(CASE WHEN Player.endtime IS NULL THEN 0 ELSE 1 END),
Player.endtime ASC
;
";
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
						Int32 currentRole = -1;
						String newestName = String.Empty;
						while (r.Read())
						{
							CensusEntry ce = new CensusEntry();
							Int32 role = r.GetInt32(0);
							if (currentRole == role)
							{
								ce.Alive = "Sub Out";
								ce.Replacement = newestName;
							}
							else
							{
								currentRole = role;
								if (!r.IsDBNull(1))
								{
									ce.EndPostTime = r.GetDateTime(1);
									ce.Alive = "Dead";
								}
							}
							Int32 playerId = r.GetInt32(2);
							if (!r.IsDBNull(3))
							{
								ce.EndPostTime = r.GetDateTime(3);
							}
							ce.Name = r.GetString(4);
							newestName = ce.Name;

							census.Add(ce);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("After ReadRoster {0}", watch.Elapsed.ToString());
			return census;
		}
		public void WriteRoster(Int32 threadId, IEnumerable<CensusEntry> census)
		{
			Dictionary<String, List<CensusEntry>> roleList = new Dictionary<String, List<CensusEntry>>();
			HashSet<String> subOut = new HashSet<string>();
			HashSet<String> subIn = new HashSet<string>();
			HashSet<String> all = new HashSet<string>();

			foreach (CensusEntry ce in census)
			{
				if ((ce.Name == null) || (ce.Name == String.Empty))
				{
					continue;
				}
				if (!roleList.ContainsKey(ce.Name))
				{
					all.Add(ce.Name);
					roleList.Add(ce.Name, new List<CensusEntry> { ce });
					if ((ce.Replacement != null) && (ce.Replacement != String.Empty))
					{
						subOut.Add(ce.Name);
						subIn.Add(ce.Replacement);
					}
				}
			}
			HashSet<String> originals = new HashSet<string>(all.Except(subIn));
			// merge subs
			foreach(String original in originals)
			{
				String current = roleList[original][0].Replacement;
				while ((current != String.Empty) && (current != null))
				{
					if (!roleList.ContainsKey(current))
					{
						break;
					}
					roleList[original].AddRange(roleList[current]);
					String next = roleList[current].Last().Replacement;
					roleList.Remove(current);
					current = next;
				}
			}

			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
			{
				dbWrite.Open();
				using (SQLiteTransaction trans = dbWrite.BeginTransaction())
				{

					String sqlDelete =
@"DELETE 
FROM GameRole 
where (threadid = @p1);";
					using (SQLiteCommand cmd = new SQLiteCommand(sqlDelete, dbWrite, trans))
					{
						cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
						int e = cmd.ExecuteNonQuery();
					}
					String sql =

@"INSERT INTO GameRole (
threadid, deathtime)
VALUES (@p1, @p2);
SELECT last_insert_rowid();";
					using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
					{
						SQLiteParameter pThreadId = new SQLiteParameter("@p1");
						cmd.Parameters.Add(pThreadId);
						pThreadId.Value = threadId;

						foreach (List<CensusEntry> role in roleList.Values)
						{
							Int32 id = -1;
							DateTime? deathTime = null;
							CensusEntry lastPlayer = role.Last();
							if (lastPlayer.Alive == "Dead")
							{
								deathTime = lastPlayer.EndPostTime;
							}
							SQLiteParameter pDeath = new SQLiteParameter("@p2", System.Data.DbType.DateTime);
							pDeath.Value = deathTime;
							cmd.Parameters.Add(pDeath);
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

							string sqlPlayer =
@"INSERT INTO Player 
(roleid, posterid, startpost, endtime) 
VALUES(@p1, @p2, @p3, @p4);";
							using (SQLiteCommand cmdPlayer = new SQLiteCommand(sqlPlayer, dbWrite, trans))
							{
								SQLiteParameter pRoleId = new SQLiteParameter("@p1");
								pRoleId.Value = id;
								foreach (CensusEntry player in role)
								{
									SQLiteParameter pPosterId = new SQLiteParameter("@p2");
									Int32 playerId = GetPlayerId(player.Name);
									if (playerId < 0)
									{
										break;
									}
									pPosterId.Value = playerId;
									cmdPlayer.Parameters.Add(pRoleId);
									cmdPlayer.Parameters.Add(pPosterId);
									cmdPlayer.Parameters.Add(new SQLiteParameter("@p3", 1));
									SQLiteParameter pEndTime = new SQLiteParameter("@p4", System.Data.DbType.DateTime);
									if (player.Alive != "Alive")
									{
										DateTime? endTime = null;
										if (player.EndPostTime != null)
										{
											endTime = player.EndPostTime.Value.ToUniversalTime();
										}
										pEndTime.Value = endTime;
									}
									cmdPlayer.Parameters.Add(pEndTime);
									
									int e = cmdPlayer.ExecuteNonQuery();
								}
							}
						}
					}
					trans.Commit();
				}
			}
			watch.Stop();

			//Trace.TraceInformation("after ReplacePlayerList {0}", watch.Elapsed.ToString());
		}
		Int32 GetRoleId(int threadId, string name)
		{
			string sql = @"
SELECT GameRole.roleid
FROM GameRole, Player, Poster
WHERE
Poster.postername = @p1
AND
Player.posterid = Poster.posterid
AND
GameRole.roleid = Player.roleid
AND
GameRole.threadId = @p2
";
			Int32 id = -1;
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", name));
					cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
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
			//Trace.TraceInformation("After GetPlayerId {0}", watch.Elapsed.ToString());
			return id;
		}

		public void SubPlayer(int threadId, string oldName, string newName)
		{
			string sqlRoleId = @"
SELECT GameRole.roleid
FROM GameRole, Poster, Player
WHERE
GameRole.threadId = @p1
AND
Poster.postername = @p2
AND
Player.posterid = Poster.posterid
AND
GameRole.roleid = Player.roleid
";
			string sqlUpdate = @"
UPDATE Player
SET posterid = @p1
WHERE
roleid = @p2
";
			Int32 newPosterId = GetPlayerId(newName);
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				Int32 roleId = -1;
				using (SQLiteCommand cmd = new SQLiteCommand(sqlRoleId, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
					cmd.Parameters.Add(new SQLiteParameter("@p2", oldName));
					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							roleId = r.GetInt32(0);
						}
					}
				}
				if (roleId != -1)
				{
					using (SQLiteCommand cmd = new SQLiteCommand(sqlUpdate, dbRead))
					{
						cmd.Parameters.Add(new SQLiteParameter("@p1", newPosterId));
						cmd.Parameters.Add(new SQLiteParameter("@p2", roleId));
						int rows = cmd.ExecuteNonQuery();
						if (rows != 1)
						{
							Trace.TraceError("*** Sub UPDATE failed. '{0}' for '{1}'. query: {2}\n", oldName, newName, cmd.CommandText);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("After SubPlayer {0}", watch.Elapsed.ToString());
		}

		public void KillPlayer(int threadId, string name, int postNumber)
		{
			Int32 id = GetRoleId(threadId, name);
			string sql =
@"DELETE FROM GameRole
WHERE
roleid = @p1
;";
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
			{
				dbWrite.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", id));
					int e = cmd.ExecuteNonQuery();
				}
			}
			watch.Stop();
		}
		
		public Int32 GetPlayerId(string player)
		{
			String sql = 
@"SELECT posterid
FROM Poster
WHERE (postername = @p1)
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
			//Trace.TraceInformation("After GetPlayerId {0}", watch.Elapsed.ToString());
			return id;
		}
		public void WriteAlias(Int32 threadId, String bolded, Int32 playerId)
		{
			String sql = 
@"
INSERT OR REPLACE INTO Alias (
threadid,
bolded,
posterid
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
					cmd.Parameters.Add(new SQLiteParameter("@p3", playerId));
					int e = cmd.ExecuteNonQuery();
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after WriteAlias {0}", watch.Elapsed.ToString());
		}
		public void SetIgnoreOnBold(Int32 postId, Int32 boldPosition, Boolean ignore)
		{
			String sql = 
@"UPDATE OR IGNORE Bolded
SET ignore = @p1
WHERE (postid = @p2) AND (position = @p3);";
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
			//Trace.TraceInformation("after ignore vote {0}", watch.Elapsed.ToString());
		}
		public void WriteUnhide(Int32 threadId, String player, Int32 startPostId, DateTimeOffset endTime)
		{
			String sql = 
@"SELECT Bolded.postid, Bolded.position
FROM Bolded INNER JOIN Post ON (Bolded.postid = Post.postid)
INNER JOIN Poster ON (Post.posterid = Poster.posterid)
WHERE (Poster.postername = @p1) AND (Post.threadid = @p2) AND
(Bolded.postid >= @p4) AND (Post.posttime <= @p3) AND
(Bolded.ignore <> 0)
ORDER BY Bolded.postid ASC, Bolded.position ASC LIMIT 1";
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
			//Trace.TraceInformation("After WriteUnhide {0}", watch.Elapsed.ToString());
			if (postNewId > 0)
			{
				SetIgnoreOnBold(postNewId, position, false);
			}
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

@"INSERT OR IGNORE INTO Poster (posterid, postername) VALUES (@p3, @p8);

INSERT OR IGNORE INTO Post (
postid,
threadid,
posterid,
postnumber,
content,
title,
posttime)
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
						cmd.Parameters.Add(pPosterId);
						cmd.Parameters.Add(pPostNumber);
						cmd.Parameters.Add(pContent);
						cmd.Parameters.Add(pTitle);
						cmd.Parameters.Add(pTime);
						cmd.Parameters.Add(pPosterName);

						foreach (Post p in posts)
						{
							pPostId.Value = p.PostId;
							pThreadId.Value = p.ThreadId;
							pPosterId.Value = p.Poster.Id;
							pPostNumber.Value = p.PostNumber;
							pContent.Value = p.Content;
							pTitle.Value = p.Title;
							pTime.Value = p.Time.UtcDateTime;
							pPosterName.Value = p.Poster.Name;
							int e = cmd.ExecuteNonQuery();

							int ix = 0;
							String sqlBold =
@"INSERT OR IGNORE INTO Bolded (
postid,
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

			//Trace.TraceInformation("after AddPostsToDB {0}", watch.Elapsed.ToString());
		}

		//  -- Reads --
		public DateTime? GetPostTime(Int32 threadId, Int32 postNumber)
		{
			DateTime? rc = null;
			String sqlTime = 
@"SELECT posttime
FROM Post
WHERE Post.threadid = @p2 AND
(Post.postnumber == @p4);";
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
							rc = DateTime.SpecifyKind(r.GetDateTime(0), DateTimeKind.Local);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPostTime {0}", watch.Elapsed.ToString());
			return rc;
		}
		public Int32? GetMaxPost(Int32 threadId)
		{
			String sql = 
@"SELECT postnumber FROM Post WHERE threadid=@p1 ORDER BY postid DESC LIMIT 1;";
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
			//Trace.TraceInformation("after get max post # {0}", watch.Elapsed.ToString());
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

		public IEnumerable<String> GetLivePlayers(Int32 threadId, Int32 postNumber)
		{
			List<String> players = new List<string>();
			String sql = 
@"SELECT Poster.postername
FROM Poster
INNER JOIN Player ON (Poster.posterid = Player.posterid)
INNER JOIN GameRole ON (Player.roleid = GameRole.roleid)
WHERE (GameRole.threadid = @p1)
AND ((Player.endtime IS NULL)
OR (Player.endtime > @p2))
ORDER BY Poster.postername ASC;";
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
					cmd.Parameters.Add(new SQLiteParameter("@p2", postNumber));
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
			//Trace.TraceInformation("after GetPlayerList {0}", watch.Elapsed.ToString());
			return players;
		}
		public IEnumerable<VoterInfo> GetVotes(Int32 threadId, Int32 startPost, DateTime endTime, Boolean lockedVotes, object game)
		{
			String sql = 
@"
SELECT GameRole.roleid, Player.posterid, Poster.postername, 
(SELECT COUNT(*)  
	FROM Post WHERE
	(GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
	AND (Post.threadid = GameRole.threadid)
	AND (Post.posterid = Player.posterid)
	AND (Post.postnumber >= @p4) 
	AND (Post.posttime <= @p3)
) AS postcount,
(SELECT MAX(Post.postid)
	FROM Bolded, Post WHERE
	(GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
	AND (Post.threadid = GameRole.threadid)
	AND (Bolded.postid = Post.postid)
	AND (Post.posterid = Player.posterid)
	AND (Post.postnumber >= @p4) 
	AND (Post.posttime <= @p3)
	AND (Bolded.ignore = 0)
) AS bolded
FROM GameRole 
JOIN Player ON (GameRole.roleid = Player.roleid)
JOIN Poster ON (Poster.posterid = Player.posterid)
WHERE (GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
GROUP BY Poster.postername
;
";
			String sqlLocked =
@"
SELECT GameRole.roleid, Player.posterid, Poster.postername, 
(SELECT COUNT(*)  
	FROM Post WHERE
	(GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
	AND (Post.threadid = GameRole.threadid)
	AND (Post.posterid = Player.posterid)
	AND (Post.postnumber >= @p4) 
	AND (Post.posttime <= @p3)
) AS postcount,
(SELECT MIN(Post.postid)
	FROM Bolded, Post WHERE
	(GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
	AND (Post.threadid = GameRole.threadid)
	AND (Bolded.postid = Post.postid)
	AND (Post.posterid = Player.posterid)
	AND (Post.postnumber >= @p4) 
	AND (Post.posttime <= @p3)
	AND (Bolded.ignore = 0)
) AS bolded
FROM GameRole 
JOIN Player ON (GameRole.roleid = Player.roleid)
JOIN Poster ON (Poster.posterid = Player.posterid)
WHERE (GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
GROUP BY Poster.postername
;
";
			if (lockedVotes)
			{
				sql = sqlLocked;
			}
			SortableBindingList<VoterInfo> voters = new SortableBindingList<VoterInfo>();
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
							Int32? boldedPost = null;
							if (!r.IsDBNull(4))
							{
								boldedPost = r.GetInt32(4);
							}
							VoterInfo v = new VoterInfo(name, count, boldedPost);
							voters.Add(v);
						}
					}

				}
				sql = 
@"
SELECT Bolded.bolded, Bolded.position, Post.postnumber, Post.posttime
	FROM Bolded
	JOIN Post ON (Bolded.postid = Post.postid)
	WHERE
	(Bolded.postid = @p1)
	AND (Bolded.ignore = 0)
	ORDER BY Bolded.position DESC
	LIMIT 1
; 
";
				sqlLocked =
@"
SELECT Bolded.bolded, Bolded.position, Post.postnumber, Post.posttime
	FROM Bolded
	JOIN Post ON (Bolded.postid = Post.postid)
	WHERE
	(Bolded.postid = @p1)
	AND (Bolded.ignore = 0)
	ORDER BY Bolded.position ASC
	LIMIT 1
; 
";
				if (lockedVotes)
				{
					sql = sqlLocked;
				}
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					foreach (VoterInfo v in voters)
					{
						Int32 id = v.PostId;
						if (id <= 0)
						{
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
			//Trace.TraceInformation("after post counts {0}", watch.Elapsed.ToString());
			return voters;
		}
        public IEnumerable<VoterInfo2> GetAllVotes(Int32 threadId, Int32 startPost, DateTime endTime, object game)
        {
            String sql =
@"
SELECT GameRole.roleid, Player.posterid, Poster.postername, 
(SELECT COUNT(*)  
	FROM Post WHERE
	(GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
	AND (Post.threadid = GameRole.threadid)
	AND (Post.posterid = Player.posterid)
	AND (Post.postnumber >= @p4) 
	AND (Post.posttime <= @p3)
) AS postcount
FROM GameRole 
JOIN Player ON (GameRole.roleid = Player.roleid)
JOIN Poster ON (Poster.posterid = Player.posterid)
WHERE (GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
GROUP BY Poster.postername
;
";
            SortableBindingList<VoterInfo2> voters = new SortableBindingList<VoterInfo2>();
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
                            VoterInfo2 v = new VoterInfo2(name, playerId, count);
                            voters.Add(v);
                        }
                    }

                }
                string sqlVotes = @"SELECT Bolded.bolded, Bolded.position, Post.postnumber, Post.posttime, Post.postid
	FROM Bolded, Post WHERE
	(Post.threadid = @p1)
	AND (Post.posterid = @p2)
	AND (Bolded.postid = Post.postid)
	AND (Post.postnumber >= @p3) 
	AND (Post.posttime <= @p4)
	AND (Bolded.ignore = 0)
    ORDER BY Post.postnumber ASC, Bolded.Position ASC
";
                using (SQLiteCommand cmd = new SQLiteCommand(sqlVotes, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p3", startPost));
                    cmd.Parameters.Add(new SQLiteParameter("@p4", endTime));
                    foreach (VoterInfo2 v in voters)
                    {
                        cmd.Parameters.Add(new SQLiteParameter("@p2", v.PosterId));
                        using (SQLiteDataReader r = cmd.ExecuteReader())
                        {
                            while (r.Read())
                            {
                                String bolded = r.GetString(0);
                                Int32 position = r.GetInt32(1);
                                Int32 number = r.GetInt32(2);
                                DateTimeOffset time = r.GetDateTime(3);
                                Int32 postId = r.GetInt32(4);
                                v.AddVote(new Vote(v.Name, bolded, number, postId, position, time));
                            }
                        }
                    }

                }
            }
            watch.Stop();
            //Trace.TraceInformation("after post counts {0}", watch.Elapsed.ToString());
            return voters;
        }

		public Boolean GetDayBoundaries(Int32 threadId, Int32 day, out Int32 startPost, 
				out DateTime endTime, out Int32 endPost)
		{
			startPost = 0;
			endTime = DateTime.Now;
			endPost = 0;

			String sql = 
@"
SELECT startpost, endtime FROM Day 
WHERE (threadid = @p1) AND (day = @p2)
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
			//Trace.TraceInformation("after ReadDayBoundaries {0}", watch.Elapsed.ToString());

			String sqlTime = 
@"SELECT postnumber
FROM Post
WHERE (Post.threadid = @p2) AND (Post.posttime <= @p3) AND (Post.postnumber >= @p1)
ORDER BY postid DESC LIMIT 1";
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
			//Trace.TraceInformation("after get EndPost {0}", watch.Elapsed.ToString());
			return true;
		}
		public String GetAlias(Int32 threadId, String bolded)
		{
			String rc = String.Empty;
			// Check our thread.
			String sql = 
@"
SELECT poster.postername FROM Poster 
JOIN Alias ON (Poster.posterid = Alias.posterid) 
WHERE (threadid = @p1) AND (bolded = @p2)
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
			//Trace.TraceInformation("after GetAliasDB {0}", watch.Elapsed.ToString());

			// Check other threads.
			return rc;
		}




		public IEnumerable<POG.Forum.Poster> GetPostersLike(string name)
		{
			name = name.Replace("%", ";%");
			var posters = new List<POG.Forum.Poster>();
			String sql = 
@"
SELECT Poster.postername, Poster.posterid, COUNT(*) AS gamesplayed FROM Poster
LEFT OUTER JOIN Player ON (Poster.posterid = Player.posterid) 
WHERE (postername LIKE @p1 ESCAPE ';')
GROUP BY Poster.postername
ORDER BY gamesplayed DESC, postername ASC;
";
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					String search = name + "%";
					cmd.Parameters.Add(new SQLiteParameter("@p1", search));
					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						while (r.Read())
						{
							String suggestion = r.GetString(0);
							Int32 id = r.GetInt32(1);
							var p = new POG.Forum.Poster(suggestion, id);
							posters.Add(p);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPostersLike {0}", watch.Elapsed.ToString());
			return posters;
		}


		public void AddPosters(IEnumerable<POG.Forum.Poster> posters)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
			{
				dbWrite.Open();
				using (SQLiteTransaction trans = dbWrite.BeginTransaction())
				{
					String sql =

@"INSERT OR IGNORE INTO Poster (posterid, postername) VALUES (@p3, @p8);";

					using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
					{
						SQLiteParameter pPosterId = new SQLiteParameter("@p3");
						SQLiteParameter pPosterName = new SQLiteParameter("@p8");
						cmd.Parameters.Add(pPosterName);
						cmd.Parameters.Add(pPosterId);

						foreach (POG.Forum.Poster p in posters)
						{
							pPosterName.Value = p.Name;
							pPosterId.Value = p.Id;
							int e = cmd.ExecuteNonQuery();
						}
					}
					trans.Commit();
				}
			}
			watch.Stop();

			//Trace.TraceInformation("after AddPosters {0}", watch.Elapsed.ToString());
		}
        public Post GetPost(Int32 postId)
        {
            Post rc = null;
            String sql =
@"SELECT Post.threadid, Post.posterid, Post.postnumber, Post.posttime, Post.title, Post.content, Poster.postername
FROM Post, Poster
WHERE
Post.postid = @p1
AND
Post.posterid = Poster.Posterid
LIMIT 1;";
            Int32 threadId = 0;
            Int32 posterId = 0;
            Int32 postNumber = 0;
            DateTimeOffset postTime = DateTimeOffset.MinValue;
            String title = "";
            String content = "";
            String posterName = "";

            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", postId));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        if (r.Read())
                        {
                            threadId = r.GetInt32(0);
                            posterId = r.GetInt32(1);
                            postNumber = r.GetInt32(2);
                            postTime = DateTime.SpecifyKind(r.GetDateTime(3), DateTimeKind.Local);
                            title = r.GetString(4);
                            content = r.GetString(5);
                            posterName = r.GetString(6);
                        }
                    }
                }
            }
            watch.Stop();
            //Trace.TraceInformation("after GetPostNumber {0}", watch.Elapsed.ToString());
            rc = new Post(threadId, posterName, posterId, postNumber, postTime, postId, title, content, null, null);
            return rc;
        }
        public IEnumerable<Post> GetPosts(Int32 threadId, String poster)
        {
            Int32 posterId = GetPlayerId(poster);
            List<Post> posts = new List<Post>();
            String sql =
@"SELECT postnumber, posttime, title, content, postid
FROM Post
WHERE
threadid = @p1
AND
posterid = @p2
ORDER BY postid ASC
;";

            Stopwatch watch = new Stopwatch();
            watch.Start();
            using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
            {
                dbRead.Open();
                using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
                {
                    cmd.Parameters.Add(new SQLiteParameter("@p1", threadId));
                    cmd.Parameters.Add(new SQLiteParameter("@p2", posterId));
                    using (SQLiteDataReader r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            Int32 postNumber = r.GetInt32(0);
                            DateTimeOffset postTime = DateTime.SpecifyKind(r.GetDateTime(1), DateTimeKind.Local);
                            String title = r.GetString(2);
                            String content = r.GetString(3);
                            Int32 postId = r.GetInt32(4);
                            Post post = new Post(threadId, poster, posterId, postNumber, postTime, postId, title, content, null, null);
                            posts.Add(post);
                        }
                    }
                }
            }
            watch.Stop();
            //Trace.TraceInformation("after GetPostNumber {0}", watch.Elapsed.ToString());
            return posts;
        }

        public Post GetPost(Int32 threadId, Int32 postNumber)
        {
            Int32 postId = GetPostId(threadId, postNumber);
            Post rc = GetPost(postId);
            return rc;
        }
        public Int32 GetPostNumber(Int32 threadId, Int32 postId)
		{
			Int32 rc = 0;
			String sql =
@"SELECT MAX(Post.postnumber)
FROM Post
WHERE Post.threadid = @p2 AND
(Post.postid <= @p4);";

			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
					cmd.Parameters.Add(new SQLiteParameter("@p4", postId));
					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							rc = r.GetInt32(0);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPostNumber {0}", watch.Elapsed.ToString());
			return rc;
		}


		public Int32 GetPostId(Int32 threadId, int postNumber)
		{
			Int32 rc = -1;
			String sql =
@"SELECT Post.postid
FROM Post
WHERE Post.threadid = @p2 AND
(Post.postnumber == @p4);";

			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
					cmd.Parameters.Add(new SQLiteParameter("@p4", postNumber));
					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							rc = r.GetInt32(0);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPostId {0}", watch.Elapsed.ToString());
			return rc;
		}


		public int GetPostBeforeTime(Int32 threadId, DateTime startTime)
		{
			String sql =
@"SELECT Post.postnumber
FROM Post
WHERE (Post.threadid = @p2) AND
(Post.posttime < @p4)
ORDER BY Post.postid DESC
LIMIT 1
;";

			Stopwatch watch = new Stopwatch();
			watch.Start();
			Int32 postNumber = 0;
			using (SQLiteConnection dbRead = new SQLiteConnection(_connect))
			{
				dbRead.Open();
				using (SQLiteCommand cmd = new SQLiteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SQLiteParameter("@p2", threadId));
					SQLiteParameter pTime = new SQLiteParameter("@p4", System.Data.DbType.DateTime);
					pTime.Value = startTime;
					cmd.Parameters.Add(pTime);
					using (SQLiteDataReader r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							postNumber = r.GetInt32(0);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPostBeforeTime {0}", watch.Elapsed.ToString());
			return postNumber;
		}
		public void ChangeBolded(int _threadId, string player, string oldbold, string newbold)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SQLiteConnection dbWrite = new SQLiteConnection(_connect))
			{
				dbWrite.Open();
				using (SQLiteTransaction trans = dbWrite.BeginTransaction())
				{
					String sql = @"UPDATE Bolded SET bolded = @p2 WHERE bolded = @p4;";

					using (SQLiteCommand cmd = new SQLiteCommand(sql, dbWrite, trans))
					{
						cmd.Parameters.Add(new SQLiteParameter("@p2", newbold));
						cmd.Parameters.Add(new SQLiteParameter("@p4", oldbold));

						int e = cmd.ExecuteNonQuery();
					}
					trans.Commit();
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPost {0}", watch.Elapsed.ToString());
		}
	}
}
