<?php
if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "All"; else $gametype = $_GET['gametype'];
if($gametype == "All")


$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from Player p2 where p2.posterid=p.posterid) as game_count,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) as wolf_games,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) as village_games,
(SELECT count(*) FROM GameRole pt, Player p2, DeathType dt WHERE pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Conceded' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) conceded_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, DeathType dt, Affiliation a WHERE pt.deathtypeid = dt.deathtypeid AND dt.deathtypename != 'Survived' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) died_as_wolf_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, DeathType dt, Affiliation a WHERE pt.deathtypeid = dt.deathtypeid AND dt.deathtypename != 'Survived' AND dt.deathtypename != 'Eaten' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) died_as_villager_count,
(SELECT AVG(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_villager_death_day,
(SELECT AVG(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_wolf_death_day,
(SELECT AVG(pt2.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, GameRole pt2, Team t2, Affiliation a, Affiliation a2, Role r2 WHERE pt2.roletypeid = r2.roleid AND r2.rolename LIKE '%Seer%' AND t2.teamid = pt2.teamid AND t2.threadid = g.threadid AND a2.affiliationid = t2.affiliationid AND UPPER(LEFT(a2.affiliationname,3)) = 'VIL' AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_seer_death_day_when_wolf,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, DeathType dt WHERE pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) nk_as_villager_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, Role r, DeathType dt WHERE pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid AND r.rolename NOT LIKE '%Seer%' AND pt.roletypeid = r.roleid AND r.rolename NOT LIKE '%Vig%' AND r.rolename NOT LIKE '%Angel%') nk_as_vanillager_count,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Role r WHERE pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid AND r.rolename NOT LIKE '%Seer%' AND pt.roletypeid = r.roleid AND r.rolename NOT LIKE '%Vig%' AND r.rolename NOT LIKE '%Angel%') vanillage_games,
(SELECT avg(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, DeathType dt, Affiliation a WHERE g.threadid = t.threadid AND (pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed') AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_day_nked_villager,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Game g WHERE pt.teamid = t.teamid AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) mishmash_count,
(SELECT count(*) FROM GameRole pt, DeathType dt, Player p2 WHERE pt.deathtypeid = dt.deathtypeid AND deathtypename = 'Day Killed' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) day_kill_count,
(SELECT count(*) FROM GameRole pt, Player p2, DeathType dt WHERE pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Mod Killed' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) mod_kill_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, DeathType dt WHERE pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Conceded' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) conceded_count,
(SELECT AVG(deathday/gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a, Role r WHERE t.threadid = g.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roletypeid = r.roleid AND r.rolename = 'Seer' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_day_of_death_as_seer
FROM
Poster p
HAVING 
game_count > 0;");
elseif($gametype == "Vanilla/Slow Games")
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole gc, Player p2, Team t, Game g where p2.posterid=p.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND gc.teamid = t.teamid AND gc.roleid=p2.roleid) as game_count,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) as wolf_games,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) as village_games,
(SELECT count(*) FROM GameRole pt, Player p2, DeathType dt, Team t, Game g WHERE t.teamid = pt.teamid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Conceded' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) conceded_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, DeathType dt, Affiliation a, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename != 'Survived' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) died_as_wolf_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, DeathType dt, Affiliation a, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename != 'Survived' AND dt.deathtypename != 'Eaten' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) died_as_villager_count,
(SELECT AVG(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_villager_death_day,
(SELECT AVG(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_wolf_death_day,
(SELECT AVG(pt2.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, GameRole pt2, Team t2, Affiliation a, Affiliation a2, Role r2 WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND pt2.roletypeid = r2.roleid AND r2.rolename LIKE '%Seer%' AND t2.teamid = pt2.teamid AND t2.threadid = g.threadid AND a2.affiliationid = t2.affiliationid AND UPPER(LEFT(a2.affiliationname,3)) = 'VIL' AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_seer_death_day_when_wolf,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, DeathType dt, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) nk_as_villager_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, Role r, DeathType dt, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid AND r.rolename NOT LIKE '%Seer%' AND pt.roletypeid = r.roleid AND r.rolename NOT LIKE '%Vig%' AND r.rolename NOT LIKE '%Angel%') nk_as_vanillager_count,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Role r, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid AND r.rolename NOT LIKE '%Seer%' AND pt.roletypeid = r.roleid AND r.rolename NOT LIKE '%Vig%' AND r.rolename NOT LIKE '%Angel%') vanillage_games,
(SELECT avg(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, DeathType dt, Affiliation a WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND (pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed') AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_day_nked_villager,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND pt.teamid = t.teamid AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) mishmash_count,
(SELECT count(*) FROM GameRole pt, DeathType dt, Player p2, Game g, Team t WHERE t.teamid = pt.teamid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND deathtypename = 'Day Killed' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) day_kill_count,
(SELECT count(*) FROM GameRole pt, Player p2, DeathType dt, Game g, Team t WHERE t.teamid = pt.teamid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Mod Killed' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) mod_kill_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, DeathType dt, Game g WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Conceded' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) conceded_count,
(SELECT AVG(deathday/gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a, Role r WHERE (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND t.threadid = g.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roletypeid = r.roleid AND r.rolename = 'Seer' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_day_of_death_as_seer
FROM
Poster p
HAVING 
game_count > 0;");
elseif($gametype == "Vanilla+/Mish-Mashes")
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole gc, Player p2, Team t, Game g where p2.posterid=p.posterid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND gc.teamid = t.teamid AND gc.roleid=p2.roleid) as game_count,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) as wolf_games,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) as village_games,
(SELECT count(*) FROM GameRole pt, Player p2, DeathType dt, Team t, Game g WHERE t.teamid = pt.teamid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Conceded' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) conceded_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, DeathType dt, Affiliation a, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename != 'Survived' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) died_as_wolf_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, DeathType dt, Affiliation a, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename != 'Survived' AND dt.deathtypename != 'Eaten' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) died_as_villager_count,
(SELECT AVG(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_villager_death_day,
(SELECT AVG(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_wolf_death_day,
(SELECT AVG(pt2.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, GameRole pt2, Team t2, Affiliation a, Affiliation a2, Role r2 WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND pt2.roletypeid = r2.roleid AND r2.rolename LIKE '%Seer%' AND t2.teamid = pt2.teamid AND t2.threadid = g.threadid AND a2.affiliationid = t2.affiliationid AND UPPER(LEFT(a2.affiliationname,3)) = 'VIL' AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_seer_death_day_when_wolf,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, DeathType dt, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) nk_as_villager_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, Role r, DeathType dt, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed' AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid AND r.rolename NOT LIKE '%Seer%' AND pt.roletypeid = r.roleid AND r.rolename NOT LIKE '%Vig%' AND r.rolename NOT LIKE '%Angel%') nk_as_vanillager_count,
(select count(*) From GameRole pt, Player p2, Team t, Affiliation a, Role r, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid AND r.rolename NOT LIKE '%Seer%' AND pt.roletypeid = r.roleid AND r.rolename NOT LIKE '%Vig%' AND r.rolename NOT LIKE '%Angel%') vanillage_games,
(SELECT avg(pt.deathday/g.gamelength) FROM GameRole pt, Player p2, Team t, Game g, DeathType dt, Affiliation a WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND (pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Night Killed') AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_day_nked_villager,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND pt.teamid = t.teamid AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) mishmash_count,
(SELECT count(*) FROM GameRole pt, DeathType dt, Player p2, Game g, Team t WHERE t.teamid = pt.teamid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND deathtypename = 'Day Killed' AND p2.posterid=p.posterid AND p2.roleid = pt.roleid) day_kill_count,
(SELECT count(*) FROM GameRole pt, Player p2, DeathType dt, Game g, Team t WHERE t.teamid = pt.teamid AND (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Mod Killed' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) mod_kill_count,
(SELECT count(*) FROM GameRole pt, Player p2, Team t, Affiliation a, DeathType dt, Game g WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.deathtypeid = dt.deathtypeid AND dt.deathtypename = 'Conceded' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) conceded_count,
(SELECT AVG(deathday/gamelength) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a, Role r WHERE (g.gametype = 'Vanilla+' OR g.gametype = 'Mish-Mash') AND t.threadid = g.threadid AND pt.teamid = t.teamid AND a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roletypeid = r.roleid AND r.rolename = 'Seer' AND p2.posterid = p.posterid AND p2.roleid = pt.roleid) average_day_of_death_as_seer
FROM
Poster p
HAVING 
game_count > 0;");
?>

<h2>Death Stats - <?= $gametype?></h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Death Stats"/>
			Gametype: <select name="gametype" onchange="this.form.submit();">
				<option></option>
				<option>All</option>
				<option>Vanilla/Slow Games</option>
				<option>Vanilla+/Mish-Mashes</option>
			</select>
</form>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=5 />
<input type="hidden" id="colwolf" name="colwolf" value=12 />
Minimum: <input type="text" id="min" name="min" value=20 /><br/>
Maximum: <input type="text" id="max" name="max"/><br/>
Villager Minimum: <input type="text" id="villagermin" name="villagermin"/><br/>
Villager Maximum: <input type="text" id="villagermax" name="villagermax"/><br/>
Wolf Minimum: <input type="text" id="wolfmin" name="wolfmin"/><br/>
Wolf Maximum: <input type="text" id="wolfmax" name="wolfmax"/><br/>
<table class="data" border="1" class="display">
	<thead>
	<tr>
	<th rowspan="2"></th>
	<th rowspan="2">Name</th>
	<th colspan="3">Total</th>
	<th colspan="7">Villager</th>	
	<th colspan="5">Wolf</th>
	</tr>
	<tr>
	<th>Games</th>
	<th>Day Kill %</th>
	<th>Mod Kill %</th>
	<th>Games</th>
	<th>Death %</th>
	<th>Average Survival</th>
	<th>NK %</th>
	<th>Vanilla NK %</th>
	<th>Day of NK/Game Length</th>
	<th>Seer Average Survival</th>
	<th>Games</th>
	<th>Village Seer Average Survival</th>
	<th>Death %</th>
	<th>Average Survival</th>
	<th>Conceded %</th>
	</tr>
	</thead>
	<tbody>
	
<?php while($player = mysql_fetch_assoc($q)) { ?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
        <td><?= $player['game_count'] ?></td>
		<td><?php if(($gametype == "All" || $gametype == "Vanilla+/Mish-Mashes") && $player['mishmash_count'] > 0) echo number_format(round($player['day_kill_count']/$player['mishmash_count'], 3), 3); else echo 0;?></td>
		<td><?= number_format(round($player['mod_kill_count']/$player['game_count'], 3), 3) ?></td>
        <td><?= $player['village_games'] ?></td>
		<td><?php if ($player['village_games'] > 0) echo number_format(round($player['died_as_villager_count']/$player['village_games'], 3), 3); else echo 0; ?></td>
		<td><?= number_format(round($player['average_villager_death_day'], 3), 3) ?></td>
		<td><?php if ($player['village_games'] > 0) echo number_format(round($player['nk_as_villager_count']/$player['village_games'], 3), 3); else echo 0; ?></td>
		<td><?php if ($player['vanillage_games'] > 0) echo number_format(round($player['nk_as_vanillager_count']/$player['vanillage_games'], 3), 3); else echo 0; ?></td>
		<td><?= number_format(round($player['average_day_nked_villager'], 3), 3) ?></td>
		<td><?= number_format(round($player['average_day_of_death_as_seer'], 3), 3) ?></td>
        <td><?= $player['wolf_games'] ?></td>        
		<td><?php if($player['wolf_games'] > 0) echo number_format(round($player['average_seer_death_day_when_wolf'], 3), 3); else echo 0; ?></td>
		<td><?php if($player['wolf_games'] > 0) echo number_format(round($player['died_as_wolf_count']/$player['wolf_games'], 3), 3) ?></td>
		<td><?php if($player['wolf_games'] > 0) echo number_format(round($player['average_wolf_death_day'], 3), 3); else echo 0; ?></td>
		<td><?php if($player['wolf_games'] > 0) echo number_format(round($player['conceded_count']/$player['wolf_games'], 3), 3) ?></td>
    </tr>
<?php } // while ?>
	</tbody>
</table>

<b>Day/Mod/NK/Death %:</b> Count of each/Games Played<br/>
<b>Vanilla NK %:</b> Vanillager times NKed/Vanillage games<br/>
<b>(Seer) Average Survival (as Wolf):</b> Average of (Day of Death as Seer/Game Length)<br/>
<b>Village Seer Average Survival:</b> As wolf, average of (Day of Death for Villager Seers/Game Length) for all village seers in games you play<br/>
<b>Conceded %:</b> Conceded Count/Wolf Games Played<br/>
