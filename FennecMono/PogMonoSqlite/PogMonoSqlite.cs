using System;
using System.Collections.Generic;
using Mono.Data.Sqlite;
using System.Linq;
using System.Diagnostics;
using POG.Forum;
using System.IO;
using POG.Utils;

namespace POG.Werewolf
{
	public class PogSqlite : POG.Werewolf.IPogDb
	{
		static Int32 _schemaVersion = 1;
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
		delegate void UpgradeSchema(SqliteConnection db);
		void SetUserVersion(SqliteConnection db, Int32 version)
		{
			String sql = String.Format(@"PRAGMA user_version = {0};", version);
			using (SqliteCommand cmd = new SqliteCommand(sql, db))
			{
				cmd.Parameters.Add(new SqliteParameter("@p1", version));                
				int e = cmd.ExecuteNonQuery();
			}
		}
		void Upgrade1To2(SqliteConnection db)
		{
			String[] updates = {
@"ALTER TABLE Day ADD COLUMN startpost INTEGER DEFAULT 1",
@"ALTER TABLE Player ADD COLUMN startpost INTEGER DEFAULT 1",
@"ALTER TABLE Post ADD COLUMN title TEXT",
@"ALTER TABLE Post ADD COLUMN content TEXT",
			};
			foreach (String sql in updates)
			{
				using (SqliteCommand cmd = new SqliteCommand(sql, db))
				{
					int e = cmd.ExecuteNonQuery();
				}
			}
		}
		void CreateTables(SqliteConnection db)
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
	starttime TIMESTAMP,
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
	posttime TIMESTAMP
);",
@"CREATE INDEX IF NOT EXISTS 
poststhreadposter
ON
Post (threadid, posterid)
;",
@"CREATE VIRTUAL TABLE IF NOT EXISTS PostContent USING FTS4 (
	title TEXT,
	content TEXT,
	editreason TEXT
);",
@"CREATE TABLE IF NOT EXISTS PostMeta (
	postid INTEGER NOT NULL REFERENCES Post(postid) ON DELETE CASCADE,
	contentid INTEGER NOT NULL REFERENCES PostContent(ROWID) ON DELETE CASCADE,
	editorid INTEGER,
	edittime TIMESTAMP,
	PRIMARY KEY(postid, contentid)
);",
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
	starttime TIMESTAMP,
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
				using (SqliteCommand cmd = new SqliteCommand(sql, db))
				{
					int e = cmd.ExecuteNonQuery();
				}
			}
		}
		private void CreateOrUpgradeSchema(String _dbName)
		{

			if (!File.Exists(_dbName))
			{
				SqliteConnection.CreateFile(_dbName);
			}
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteTransaction trans = dbWrite.BeginTransaction())
				{
					String sqlVersion = "PRAGMA user_version;";
					Int32 dbVersion = 0;
					using (SqliteCommand cmd = new SqliteCommand(sqlVersion, dbWrite))
					{
						using (SqliteDataReader r = cmd.ExecuteReader())
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
							SetUserVersion(dbWrite, i);
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
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p2", url));
					cmd.Parameters.Add(new SqliteParameter("@p3", turbo));
					int e = cmd.ExecuteNonQuery();
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after WriteThreadDefinition {0}", watch.Elapsed.ToString());
		}
		public void WriteDayBoundaries(Int32 threadId, Int32 day, DateTime startTime, DateTime endTime)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				String sql = 
@"INSERT OR REPLACE INTO Day (
threadid,
day,
starttime, 
endtime)
VALUES (@p1, @p2, @p3, @p4);";
				using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p2", day));
					SqliteParameter pStart = new SqliteParameter("@p3", System.Data.DbType.DateTime);
					pStart.Value = startTime.ToUniversalTime();
					cmd.Parameters.Add(pStart);
					SqliteParameter pEod = new SqliteParameter("@p4", System.Data.DbType.DateTime);
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
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteTransaction trans = dbWrite.BeginTransaction())
				{

					String sqlDelete =
@"DELETE 
FROM GameRole 
where (threadid = @p1);";
					using (SqliteCommand cmd = new SqliteCommand(sqlDelete, dbWrite, trans))
					{
						cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
						int e = cmd.ExecuteNonQuery();
					}
					String sql =

@"INSERT INTO GameRole (
threadid)
VALUES (@p1);
SELECT last_insert_rowid();";

					string sqlPlayer =
@"INSERT INTO Player (roleid, posterid) VALUES(@p1, @p2);";

					using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite, trans))
					{
						SqliteParameter pThreadId = new SqliteParameter("@p1");
						cmd.Parameters.Add(pThreadId);
						pThreadId.Value = threadId;

						foreach (String player in players)
						{
							Int32 id = -1;
							using (SqliteDataReader r = cmd.ExecuteReader())
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

							using (SqliteCommand cmdPlayer = new SqliteCommand(sqlPlayer, dbWrite, trans))
							{
								SqliteParameter pRoleId = new SqliteParameter("@p1");
								pRoleId.Value = id;
								SqliteParameter pPosterId = new SqliteParameter("@p2");
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteTransaction trans = dbWrite.BeginTransaction())
				{

					String sqlDelete =
@"DELETE 
FROM GameRole 
where (threadid = @p1);";
					using (SqliteCommand cmd = new SqliteCommand(sqlDelete, dbWrite, trans))
					{
						cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
						int e = cmd.ExecuteNonQuery();
					}
					String sql =

@"INSERT INTO GameRole (
threadid, deathtime)
VALUES (@p1, @p2);
SELECT last_insert_rowid();";
					using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite, trans))
					{
						SqliteParameter pThreadId = new SqliteParameter("@p1");
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
							SqliteParameter pDeath = new SqliteParameter("@p2", System.Data.DbType.DateTime);
							pDeath.Value = deathTime;
							cmd.Parameters.Add(pDeath);
							using (SqliteDataReader r = cmd.ExecuteReader())
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
(roleid, posterid, starttime, endtime) 
VALUES(@p1, @p2, @p3, @p4);";
							using (SqliteCommand cmdPlayer = new SqliteCommand(sqlPlayer, dbWrite, trans))
							{
								SqliteParameter pRoleId = new SqliteParameter("@p1");
								pRoleId.Value = id;
								DateTime? startTime = null;
								foreach (CensusEntry player in role)
								{
									SqliteParameter pPosterId = new SqliteParameter("@p2");
									Int32 playerId = GetPlayerId(player.Name);
									if (playerId < 0)
									{
										break;
									}
									pPosterId.Value = playerId;
									cmdPlayer.Parameters.Add(pRoleId);
									cmdPlayer.Parameters.Add(pPosterId);
									SqliteParameter pStartTime = new SqliteParameter("@p3", System.Data.DbType.DateTime);
									pStartTime.Value = startTime;
									cmdPlayer.Parameters.Add(pStartTime);
									SqliteParameter pEndTime = new SqliteParameter("@p4", System.Data.DbType.DateTime);
									if (player.Alive != "Alive")
									{
										DateTime? endTime = null;
										if (player.EndPostTime != null)
										{
											endTime = player.EndPostTime.Value.ToUniversalTime();
										}
										pEndTime.Value = endTime;
										startTime = endTime;
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", player));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p2", bolded));
					cmd.Parameters.Add(new SqliteParameter("@p3", playerId));
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
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", ignore));
					cmd.Parameters.Add(new SqliteParameter("@p2", postId));
					cmd.Parameters.Add(new SqliteParameter("@p3", boldPosition));
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", player));
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					SqliteParameter pEndTime = new SqliteParameter("@p3", System.Data.DbType.DateTime);
					pEndTime.Value = endTime.ToUniversalTime().DateTime;
					cmd.Parameters.Add(pEndTime);
					cmd.Parameters.Add(new SqliteParameter("@p4", startPostId));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteTransaction trans = dbWrite.BeginTransaction())
				{
					String sql =

@"INSERT OR IGNORE INTO Poster (posterid, postername) VALUES (@p3, @p8);

INSERT OR IGNORE INTO Post (
postid,
threadid,
posterid,
postnumber,
posttime)
VALUES (@p1, @p2, @p3, @p4, @p7);";

					using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite, trans))
					{
						SqliteParameter pPostId = new SqliteParameter("@p1");
						SqliteParameter pThreadId = new SqliteParameter("@p2");
						SqliteParameter pPosterId = new SqliteParameter("@p3");
						SqliteParameter pPostNumber = new SqliteParameter("@p4");
						SqliteParameter pContent = new SqliteParameter("@p5");
						SqliteParameter pTitle = new SqliteParameter("@p6");
						SqliteParameter pTime = new SqliteParameter("@p7", System.Data.DbType.DateTime);
						SqliteParameter pPosterName = new SqliteParameter("@p8");
						cmd.Parameters.Add(pPostId);
						cmd.Parameters.Add(pThreadId);
						cmd.Parameters.Add(pPosterName);
						cmd.Parameters.Add(pPosterId);
						cmd.Parameters.Add(pPostNumber);
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
@"INSERT OR IGNORE INTO Bolded (
postid,
position,
bolded,
ignore)
VALUES (@p1, @p2, @p3, @p4);";
							if (p.Bolded != null)
							{
								using (SqliteCommand cmdBold = new SqliteCommand(sqlBold, dbWrite, trans))
								{
									SqliteParameter pForeignPostId = new SqliteParameter("@p1");
									SqliteParameter pIx = new SqliteParameter("@p2");
									SqliteParameter pBolded = new SqliteParameter("@p3");
									SqliteParameter pIgnore = new SqliteParameter("@p4");
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sqlTime, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p4", postNumber));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p2", postNumber));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
		public IEnumerable<VoterInfo> GetVotes(Int32 threadId, DateTime startTime, DateTime endTime, object game)
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
	AND (Post.posttime >= @p4) 
	AND (Post.posttime <= @p3)
) AS postcount,
(SELECT MAX(Post.postid)
	FROM Bolded, Post WHERE
	(GameRole.threadid = @p2)
	AND ((Player.endtime IS NULL) OR (player.endtime > @p4))
	AND (Post.threadid = GameRole.threadid)
	AND (Bolded.postid = Post.postid)
	AND (Post.posterid = Player.posterid)
	AND (Post.posttime >= @p4) 
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
			SortableBindingList<VoterInfo> voters = new SortableBindingList<VoterInfo>();
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					SqliteParameter pEndTime = new SqliteParameter("@p3", System.Data.DbType.DateTime);
					pEndTime.Value = endTime.ToUniversalTime();
					cmd.Parameters.Add(pEndTime);
					SqliteParameter pStartTime = new SqliteParameter("@p4", System.Data.DbType.DateTime);
					pStartTime.Value = startTime.ToUniversalTime();
					cmd.Parameters.Add(pStartTime);

					using (SqliteDataReader r = cmd.ExecuteReader())
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
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					foreach (VoterInfo v in voters)
					{
						Int32 id = v.PostId;
						if (id <= 0)
						{
							continue;
						}
						cmd.Parameters.Add(new SqliteParameter("@p1", id));
						using (SqliteDataReader r = cmd.ExecuteReader())
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

		public Boolean GetDayBoundaries(Int32 threadId, Int32 day, out DateTime startTime,
				out DateTime endTime, out Int32 endPost)
		{
			startTime = DateTime.MinValue;
			endTime = DateTime.Now;
			endPost = 0;

			String sql = 
@"
SELECT starttime, endtime FROM Day 
WHERE (threadid = @p1) AND (day = @p2)
LIMIT 1;
";
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();

				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p2", day));

					using (SqliteDataReader r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							startTime = DateTime.SpecifyKind(r.GetDateTime(0), DateTimeKind.Local);
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
WHERE (Post.threadid = @p2) AND (Post.posttime <= @p3) AND (Post.posttime >= @p1)
ORDER BY postid DESC LIMIT 1";
			watch.Reset();
			watch.Start();
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sqlTime, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", startTime));
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					SqliteParameter pEndTime = new SqliteParameter("@p3", System.Data.DbType.DateTime);
					pEndTime.Value = endTime.ToUniversalTime();
					cmd.Parameters.Add(pEndTime);
					using (SqliteDataReader r = cmd.ExecuteReader())
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p1", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p2", bolded));
					using (SqliteDataReader r = cmd.ExecuteReader())
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




		public IEnumerable<Poster> GetPostersLike(string name)
		{
			name = name.Replace("%", ";%");
			List<Poster> posters = new List<Poster>();
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					String search = name + "%";
					cmd.Parameters.Add(new SqliteParameter("@p1", search));
					using (SqliteDataReader r = cmd.ExecuteReader())
					{
						while (r.Read())
						{
							String suggestion = r.GetString(0);
							Int32 id = r.GetInt32(1);
							Poster p = new Poster(suggestion, id);
							posters.Add(p);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPostersLike {0}", watch.Elapsed.ToString());
			return posters;
		}


		public void AddPosters(IEnumerable<Poster> posters)
		{
			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SqliteConnection dbWrite = new SqliteConnection(_connect))
			{
				dbWrite.Open();
				using (SqliteTransaction trans = dbWrite.BeginTransaction())
				{
					String sql =

@"INSERT OR IGNORE INTO Poster (posterid, postername) VALUES (@p3, @p8);";

					using (SqliteCommand cmd = new SqliteCommand(sql, dbWrite, trans))
					{
						SqliteParameter pPosterId = new SqliteParameter("@p3");
						SqliteParameter pPosterName = new SqliteParameter("@p8");
						cmd.Parameters.Add(pPosterName);
						cmd.Parameters.Add(pPosterId);

						foreach (Poster p in posters)
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p4", postId));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p4", postNumber));
					using (SqliteDataReader r = cmd.ExecuteReader())
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
		public Post GetPost(int threadId, int postNumber)
		{
			Post p = null; 
			//p.Poster; p.PostId; p.Time; 
			//p.Title; p.Bolded; p.Content; p.Edit; p.PostLink;
			String sql = 
@"SELECT Post.postid, Post.posterid, Poster.postername, Post.posttime
FROM Post
JOIN Poster ON (Post.posterid = Poster.posterid)        
WHERE Post.threadid = @p2 AND
(Post.postnumber == @p4);";

			Stopwatch watch = new Stopwatch();
			watch.Start();
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					cmd.Parameters.Add(new SqliteParameter("@p4", postNumber));
					using (SqliteDataReader r = cmd.ExecuteReader())
					{
						if (r.Read())
						{
							Int32 postId = r.GetInt32(0);
							Int32 posterId = r.GetInt32(1);
							String name = r.GetString(2);
							DateTime time = DateTime.SpecifyKind(r.GetDateTime(3), DateTimeKind.Local);
							p = new Post(threadId, name, posterId, postNumber, time, String.Empty, String.Empty,
								String.Empty, null, null);
						}
					}
				}
			}
			watch.Stop();
			//Trace.TraceInformation("after GetPost {0}", watch.Elapsed.ToString());
			return p;
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
			using (SqliteConnection dbRead = new SqliteConnection(_connect))
			{
				dbRead.Open();
				using (SqliteCommand cmd = new SqliteCommand(sql, dbRead))
				{
					cmd.Parameters.Add(new SqliteParameter("@p2", threadId));
					SqliteParameter pTime = new SqliteParameter("@p4", System.Data.DbType.DateTime);
					pTime.Value = startTime;
					cmd.Parameters.Add(pTime);
					using (SqliteDataReader r = cmd.ExecuteReader())
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
	}
}
