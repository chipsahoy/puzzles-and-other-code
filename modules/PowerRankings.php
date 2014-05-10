<?php
$q = mysql_query("SELECT 
postername,
posterid,
survivalranking,
mislynchranking,
seerhuntranking,
combinedrankingwolf,
game_count,
village_games,
wolf_games,
lynchranking,
nkranking,
nksurvivalranking,
combinedrankingvillager,
vanillagepercent,
wolfpercent,
seerpercent,
seer_games,
seerranking
FROM
(
SELECT 
p.posterid, 
p.postername, 
survivalranking,
mislynchranking,
seerhuntranking,
combinedrankingwolf,
lynchranking,
nkranking,
nksurvivalranking,
combinedrankingvillager,
vanillagepercent,
wolfpercent,
seerpercent,
seerranking,
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g, Role r WHERE r.roleid = pt.roletypeid AND r.rolename != 'Seer' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND t.teamid = pt.teamid AND g.threadid = t.threadid) +
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g, Gimmick g1, Role r WHERE r.roleid = pt.roletypeid AND r.rolename != 'Seer' AND g1.posteridmain = p.posterid AND g1.posteridgimmick = p2.posterid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND t.teamid = pt.teamid AND g.threadid = t.threadid) as village_games,

(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g, Role r WHERE r.roleid = pt.roletypeid AND r.rolename = 'Seer' AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND t.teamid = pt.teamid AND g.threadid = t.threadid) +
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g, Gimmick g1, Role r WHERE r.roleid = pt.roletypeid AND r.rolename = 'Seer' AND g1.posteridmain = p.posterid AND g1.posteridgimmick = p2.posterid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND pt.roleid=p2.roleid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND t.teamid = pt.teamid AND g.threadid = t.threadid) as seer_games,

(select count(*) from GameRole pt, Player p2, Team t, Game g where pt.roleid=p2.roleid AND p.posterid = p2.posterid AND t.teamid = pt.teamid and g.threadid = t.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game')) +
(select count(*) from GameRole pt, Player p2, Team t, Game g, Gimmick g1 where g1.posteridmain = p.posterid AND g1.posteridgimmick = p2.posterid AND pt.roleid=p2.roleid AND t.teamid = pt.teamid and g.threadid = t.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game')) as game_count,

(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g WHERE pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid=p2.roleid AND p.posterid = p2.posterid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid) +
(select count(*) From GameRole pt, Team t, Affiliation a, Player p2, Game g, Gimmick g1 WHERE g1.posteridmain = p.posterid AND g1.posteridgimmick = p2.posterid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND pt.roleid=p2.roleid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND g.threadid = t.threadid) as wolf_games

FROM
Poster p, DerivedRankings dr
WHERE
p.posterid = dr.posterid
HAVING
game_count > 0) as a");
// TODO: I'm pretty sure that GameRole rows get written only for the final posterid to inhabit the role, if there are subs
// total games is taking into account games you played for a while and subbed out of, but all the lynch stats use GameRole rows
// which won't exist for those cases, making the results slighted wrong. I think this can be fixed by finding the GameRole rows 
// both for the posterid and for the teamid/subbedposterid combo but it will require some funky joins I guess
?>
<h2>Vanilla Power Rankings</h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Power Rankings"/>
</form>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=8 />
<input type="hidden" id="colwolf" name="colwolf" value=3 />
Minimum: <input type="text" id="min" name="min" /><br/>
Maximum: <input type="text" id="max" name="max"/><br/>
Villager Minimum: <input type="text" id="villagermin" name="villagermin" value=20 /><br/>
Villager Maximum: <input type="text" id="villagermax" name="villagermax"/><br/>
Wolf Minimum: <input type="text" id="wolfmin" name="wolfmin" value=8 /><br/>
Wolf Maximum: <input type="text" id="wolfmax" name="wolfmax"/><br/>

<table class="data" border="1" class="display">
	<thead>
	<tr>
	<th rowspan="2"></th>
	<th rowspan="2">Name</th>
	<th colspan="1">Total</th>
	<th colspan="5">Wolf</th>
	<th colspan="5">Vanilla Villager</th>
	<th colspan="2">Seer</th>
	<th colspan="3">Total</th>
	</tr>
	<tr>
	<th>Games Played</th>	
	<th>Wolf Games</th>
	<th>Survival Ranking</th>
	<th>Mislynch Ranking</th>
	<th>Seer Hunt Ranking</th>
	<th>Combined Ranking</th>
	<th>Village Games</th>
	<th>Lynch Ranking</th>
	<th>NK Before Seer Ranking</th>
	<th>NK/Survived Ranking</th>
	<th>Combined Ranking</th>
	<th>Seer Games</th>
	<th>Seer Ranking</th>
	<th>Equal Ranking w/o Seer</th>
	<th>6/9*Vanilla + 2/9*Wolf + 1/9*Seer</th>
	<th>Rand Ranking (Player Dependent)</th>
	</tr>
	</thead>
	<tbody>
	
<?php while($player = mysql_fetch_assoc($q)) { ?>
    <tr>
   <td></td>
   <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
   <td><?= $player['game_count'] ?></td>   
   <td><?= $player['wolf_games'] ?></td>
		<td><?php echo number_format( $player['survivalranking'],3) ?></td>
		<td><?php echo number_format( $player['mislynchranking'],3) ?></td>
		<td><?php echo number_format( $player['seerhuntranking'],3) ?></td>
		<td><?php echo number_format( $player['combinedrankingwolf'],3) ?></td>
		<td><?php echo $player['village_games'] ?></td>
		<td><?php echo number_format( $player['lynchranking'],3) ?></td>
		<td><?php echo number_format( $player['nkranking'],3) ?></td>
		<td><?php echo number_format( $player['nksurvivalranking'],3) ?></td>
		<td><?php echo number_format( $player['combinedrankingvillager'],3) ?></td>
		<td><?php echo $player['seer_games'] ?></td>
		<td><?php echo number_format( $player['seerranking'],3) ?></td>
		<td><?php echo number_format(.5*($player['combinedrankingvillager']/1.32943) + .5*($player['combinedrankingwolf']/1.45833),3) ?></td>
		<td><?php echo number_format((6/9)*($player['combinedrankingvillager']/1.32943) + (2/9)*($player['combinedrankingwolf']/1.45833) + (1/9)*$player['seerranking'],3) ?></td>
		<td><?php echo number_format($player['vanillagepercent']*($player['combinedrankingvillager']/1.32943) + $player['wolfpercent']*($player['combinedrankingwolf']/1.45833) + $player['seerpercent']*($player['seerranking']/1.45833),3) ?></td>
    </tr>
<?php } // while ?>
	</tbody>
</table>
