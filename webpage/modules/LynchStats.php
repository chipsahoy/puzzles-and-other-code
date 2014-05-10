<?php
if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "All"; else $gametype = $_GET['gametype'];
if($gametype == "All")
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole pt, Player p2 where pt.roleid=p2.roleid AND p.posterid = p2.posterid) as game_count,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2 WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid) as village_games,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2 WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid) as wolf_games,
(select count(*) FROM GameRole pt, Player p2, DeathType dt WHERE p2.posterid = p.posterid AND pt.roleid = p2.roleid AND pt.deathtypeid = dt.deathtypeid AND (dt.deathtypename = 'Lynched' or dt.deathtypename = 'Conceded')) as lynched_count,
(select count(*) FROM GameRole pt, Team t, Affiliation a, Player p2, DeathType d WHERE pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid) as lynched_villa_count,
(select count(*) FROM GameRole pt, Team t, Affiliation a, Player p2, DeathType d WHERE pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid) as lynched_wolf_count,
(select AVG(pt.deathday/g.gamelength) FROM GameRole pt, Team t, Game g, Affiliation a, Player p2, DeathType d WHERE g.threadid = t.threadid AND pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid) as avg_day_wolf_lynch,
(select AVG(pt.deathday/g.gamelength) FROM GameRole pt, Team t, Game g, Affiliation a, Player p2, DeathType d WHERE g.threadid = t.threadid AND pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid) as avg_day_villa_lynch,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_villager,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_villager_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a1, Affiliation a2, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a1.affiliationid = t1.affiliationid AND upper(left(a1.affiliationname, 3)) = 'VIL' AND a2.affiliationid = t2.affiliationid AND upper(left(a2.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as villa_lynches_as_villager,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a1, Affiliation a2, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a1.affiliationid = t1.affiliationid AND upper(left(a1.affiliationname, 3)) = 'VIL' AND a2.affiliationid = t2.affiliationid AND upper(left(a2.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as villa_lynches_as_villager_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_wolf,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_wolf_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND t2.affiliationid != t1.affiliationid AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as nonteam_lynches_as_wolf,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND t2.affiliationid != t1.affiliationid AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as nonteam_lynches_as_wolf_d1
FROM
Poster p
HAVING 
game_count > 0;");
elseif($gametype == "Vanilla/Slow Games")
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole pt, Player p2, Team t, Game g where pt.roleid=p2.roleid AND p.posterid = p2.posterid AND t.teamid = pt.teamid and g.threadid = t.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game')) as game_count,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND t.teamid = pt.teamid AND g.threadid = t.threadid) as village_games,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid) as wolf_games,
(select count(*) FROM GameRole pt, Player p2, DeathType dt, Team t, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND pt.roleid = p2.roleid AND pt.deathtypeid = dt.deathtypeid AND (dt.deathtypename = 'Lynched' or dt.deathtypename = 'Conceded')) as lynched_count,
(select count(*) FROM GameRole pt, Team t, Affiliation a, Player p2, DeathType d, Game g WHERE pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid) as lynched_villa_count,
(select count(*) FROM GameRole pt, Team t, Affiliation a, Player p2, DeathType d, Game g WHERE pt.deathtypeid = d.deathtypeid AND (d.deathtypename = 'Lynched' or d.deathtypename = 'Conceded') AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid) as lynched_wolf_count,
(select AVG(pt.deathday/g.gamelength) FROM GameRole pt, Team t, Game g, Affiliation a, Player p2, DeathType d WHERE g.threadid = t.threadid AND pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game')) as avg_day_wolf_lynch,
(select AVG(pt.deathday/g.gamelength) FROM GameRole pt, Team t, Game g, Affiliation a, Player p2, DeathType d WHERE g.threadid = t.threadid AND pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game')) as avg_day_villa_lynch,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t1.threadid) as total_lynches_as_villager,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t1.threadid) as total_lynches_as_villager_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a1, Affiliation a2, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a1.affiliationid = t1.affiliationid AND upper(left(a1.affiliationname, 3)) = 'VIL' AND a2.affiliationid = t2.affiliationid AND upper(left(a2.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t1.threadid) as villa_lynches_as_villager,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a1, Affiliation a2, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a1.affiliationid = t1.affiliationid AND upper(left(a1.affiliationname, 3)) = 'VIL' AND a2.affiliationid = t2.affiliationid AND upper(left(a2.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t1.threadid) as villa_lynches_as_villager_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_wolf,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_wolf_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND t2.affiliationid != t1.affiliationid AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as nonteam_lynches_as_wolf,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND t2.affiliationid != t1.affiliationid AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as nonteam_lynches_as_wolf_d1
FROM
Poster p
HAVING 
game_count > 0;");
elseif($gametype == "Vanilla+/Mish-Mashes")
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole pt, Player p2, Team t, Game g where pt.roleid=p2.roleid AND p.posterid = p2.posterid AND t.teamid = pt.teamid and g.threadid = t.threadid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash')) as game_count,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND t.teamid = pt.teamid AND g.threadid = t.threadid) as village_games,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid) as wolf_games,
(select count(*) FROM GameRole pt, Player p2, DeathType dt, Team t, Game g WHERE (g.gametype = 'Mish-Mash' OR g.gametype = 'Vanilla+') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND pt.roleid = p2.roleid AND pt.deathtypeid = dt.deathtypeid AND (dt.deathtypename = 'Lynched' or dt.deathtypename = 'Conceded')) as lynched_count,
(select count(*) FROM GameRole pt, Team t, Affiliation a, Player p2, DeathType d, Game g WHERE pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid) as lynched_villa_count,
(select count(*) FROM GameRole pt, Team t, Affiliation a, Player p2, DeathType d, Game g WHERE pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid) as lynched_wolf_count,
(select AVG(pt.deathday/g.gamelength) FROM GameRole pt, Team t, Game g, Affiliation a, Player p2, DeathType d WHERE g.threadid = t.threadid AND pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash')) as avg_day_wolf_lynch,
(select AVG(pt.deathday/g.gamelength) FROM GameRole pt, Team t, Game g, Affiliation a, Player p2, DeathType d WHERE g.threadid = t.threadid AND pt.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid = p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash')) as avg_day_villa_lynch,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t1.threadid) as total_lynches_as_villager,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t1.threadid) as total_lynches_as_villager_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a1, Affiliation a2, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a1.affiliationid = t1.affiliationid AND upper(left(a1.affiliationname, 3)) = 'VIL' AND a2.affiliationid = t2.affiliationid AND upper(left(a2.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t1.threadid) as villa_lynches_as_villager,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a1, Affiliation a2, Player p2, DeathType d, Game g WHERE pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a1.affiliationid = t1.affiliationid AND upper(left(a1.affiliationname, 3)) = 'VIL' AND a2.affiliationid = t2.affiliationid AND upper(left(a2.affiliationname, 3)) = 'VIL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename= 'Lynched' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday) AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t1.threadid) as villa_lynches_as_villager_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_wolf,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as total_lynches_as_wolf_d1,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND t2.affiliationid != t1.affiliationid AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as nonteam_lynches_as_wolf,
(select count(*) FROM GameRole pt1, GameRole pt2, Team t1, Team t2, Affiliation a, Player p2, DeathType d, Game g WHERE t1.threadid = g.threadid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND pt1.roleid = p2.roleid AND p2.posterid = p.posterid AND t1.threadid = t2.threadid AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND a.affiliationid = t1.affiliationid AND upper(left(a.affiliationname, 3)) = 'WOL' AND t2.affiliationid != t1.affiliationid AND pt2.deathtypeid = d.deathtypeid AND d.deathtypename != 'Survived' AND pt2.deathday = 1 AND (pt1.deathday = null OR pt2.deathday <= pt1.deathday)) as nonteam_lynches_as_wolf_d1
FROM
Poster p
HAVING 
game_count > 0;");
// TODO: I'm pretty sure that GameRole rows get written only for the final posterid to inhabit the role, if there are subs
// total games is taking into account games you played for a while and subbed out of, but all the lynch stats use GameRole rows
// which won't exist for those cases, making the results slighted wrong. I think this can be fixed by finding the GameRole rows 
// both for the posterid and for the teamid/subbedposterid combo but it will require some funky joins I guess
?>
<h2>Lynch Stats - <?= $gametype?></h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Lynch Stats"/>
			Gametype: <select name="gametype" onchange="this.form.submit();">
				<option></option>
				<option>All</option>
				<option>Vanilla/Slow Games</option>
				<option>Vanilla+/Mish-Mashes</option>
			</select>
</form>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=3 />
<input type="hidden" id="colwolf" name="colwolf" value=4 />
Minimum: <input type="text" id="min" name="min" value=20 /><br/>
Maximum: <input type="text" id="max" name="max"/><br/>
Villager Minimum: <input type="text" id="villagermin" name="villagermin"/><br/>
Villager Maximum: <input type="text" id="villagermax" name="villagermax"/><br/>
Wolf Minimum: <input type="text" id="wolfmin" name="wolfmin"/><br/>
Wolf Maximum: <input type="text" id="wolfmax" name="wolfmax"/><br/>

<table class="data" border="1" class="display">
	<thead>
	<tr>
	<th></th>
	<th>Name</th>
	<th>Games Played</th>
	<th>Village Games</th>
	<th>Wolf Games</th>
	<th>Lynch Non-Villagers as Villager %</th>
	<th>Lynch Non-Villagers as Villager % Day 1</th>
	<th>Lynch Non-Team Members as Wolf %</th>
	<th>Lynch Non-Team Members as Wolf % Day 1</th>
	<th>Lynched %</th>
	<th>Lynched as Villager %</th>
	<th>Avg Day Lynched as Villager</th>
	<th>Lynched as Wolf %</th>
	<th>Avg Day Lynched as Wolf</th>
	</tr>
	</thead>
	<tbody>
	
<?php while($player = mysql_fetch_assoc($q)) { ?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
        <td><?= $player['game_count'] ?></td>
        <td><?= $player['village_games'] ?></td>
        <td><?= $player['wolf_games'] ?></td>
        <td><?php if($player['total_lynches_as_villager'] > 0) echo number_format(round(($player['total_lynches_as_villager'] - $player['villa_lynches_as_villager'])/$player['total_lynches_as_villager'], 3), 3); else echo 0;?></td>
		<td><?php if($player['total_lynches_as_villager_d1'] > 0) echo number_format(round(($player['total_lynches_as_villager_d1'] - $player['villa_lynches_as_villager_d1'])/$player['total_lynches_as_villager_d1'], 3), 3); else echo 0; ?></td>
        <td><?php if($player['total_lynches_as_wolf'] > 0) echo number_format(round($player['nonteam_lynches_as_wolf']/$player['total_lynches_as_wolf'], 3), 3); else echo 0; ?></td>
        <td><?php if($player['total_lynches_as_wolf_d1'] > 0) echo number_format(round($player['nonteam_lynches_as_wolf_d1']/$player['total_lynches_as_wolf_d1'], 3), 3); else echo 0; ?></td>
        <td><?= number_format(round($player['lynched_count']/$player['game_count'], 3), 3) ?></td>
        <td><?php if($player['village_games'] > 0) echo number_format(round($player['lynched_villa_count']/$player['village_games'], 3), 3); else echo 0; ?></td>
        <td><?= number_format(round($player['avg_day_villa_lynch'], 3), 3) ?></td>
        <td><?php if($player['wolf_games'] > 0) echo number_format(round($player['lynched_wolf_count']/$player['wolf_games'], 3), 3) ?></td>
        <td><?= number_format(round($player['avg_day_wolf_lynch'], 3), 3) ?></td>
    </tr>
<?php } // while ?>
	</tbody>
</table>

<b>Lynch Non-Villagers as Villager %:</b> Non-Villagers lynched when you're alive in a game as a villager/Total lynches when you're alive in a game as a villager<br/>
<b>Lynch Non-Team Members as Wolf %:</b> Lynches of other teams when you are alive as a wolf/Total lynches when you're alive as a wolf<br/>
<b>Lynched %:</b> Times Lynched/Games Played<br/>
<b>Lynched as Wolf/Villager %:</b> Times Lynched as a (Wolf/Villager)/Games Played as a (Wolf/Villager)<br/>
<b>Avg Day Lynched as Wolf/Villager:</b> Average of (Lynch Day as (Wolf/Villager)/Game Length)<br/>
