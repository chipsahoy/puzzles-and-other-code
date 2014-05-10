<?php
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole pt, Player p2 where pt.roleid=p2.roleid AND p2.posterid = p.posterid) as game_count,
(select count(*) from GameRole pt, Team t, Player p2, Affiliation a  where p2.posterid = p.posterid AND pt.roleid=p2.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL') as wolf_games,
(select count(*) from GameRole pt, Team t, Player p2, Affiliation a  where p2.posterid = p.posterid AND pt.roleid=p2.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL') as village_games,
(SELECT count(*) FROM Player p2 WHERE p2.posterid = p.posterid AND endday is not NULL) as subbed_out_count,
(SELECT count(*) FROM Player p2 WHERE p2.posterid = p.posterid AND startday != 0) as subbed_in_count,
(SELECT count(*) FROM Player p2, GameRole pt, Team t, Affiliation a WHERE p2.posterid = p.posterid AND endday is not NULL AND p2.roleid = pt.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL') as wolf_subbed_out_count,
(SELECT count(*) FROM Player p2, GameRole pt, Team t, Affiliation a WHERE p2.posterid = p.posterid AND endday is not NULL AND p2.roleid = pt.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL') as village_subbed_out_count,
(SELECT AVG(endday) FROM Player p2 WHERE p2.posterid = p.posterid AND endday  is not NULL) as average_day_subbed_out,
(SELECT AVG(startday) FROM Player p2 WHERE p2.posterid = p.posterid AND startday != 0) as average_day_subbed_in
FROM
Poster p
HAVING 
game_count > 0;");
?>
<h2>Sub Stats</h2>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=6 />
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
	<th>Subbed Out %</th>
	<th>Wolf Games</th>
	<th>Wolf Subbed Out %</th>
	<th>Village Games</th>
	<th>Village Subbed Out %</th>
	<th>Wolf Subbed Out %/Village Subbed Out %</th>
	<th>Average Day Subbed Out</th>
	<th>Subbed In %</th>
	<th>Average Day Subbed In</th>
	</tr>
	</thead>
	<tbody>
<?php while($player = mysql_fetch_assoc($q)) { ?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
        <td><?= $player['game_count'] ?></td>
        <td><?= number_format(round($player['subbed_out_count']/$player['game_count'], 3), 3) ?></td>
		<td><?= $player['wolf_games'] ?></td>
		<td><?php if($player['wolf_games'] > 0) echo number_format(round($player['wolf_subbed_out_count']/$player['wolf_games'], 3), 3); else echo 0; ?></td>
		<td><?= $player['village_games'] ?></td>
		<td><?php if($player['village_games'] > 0) echo number_format(round($player['village_subbed_out_count']/$player['village_games'], 3), 3); else echo 0; ?></td>
		<td><?php if($player['wolf_games'] > 0 && $player['village_games'] > 0) echo number_format(round(($player['wolf_subbed_out_count']/$player['wolf_games'])/($player['village_subbed_out_count']/$player['village_games']), 3), 3); else echo 0; ?></td>
		<td><?= $player['average_day_subbed_out'] ?></td>
		<td><?= number_format(round($player['subbed_in_count']/$player['game_count'], 3), 3) ?></td>
		<td><?= $player['average_day_subbed_in'] ?></td>
    </tr>
<?php } // while ?>
	</tbody>
</table>
<b>(Wolf) Subbed (Out/In) %:</b> (Wolf) Sub Games/ (Wolf) Total Games<br/>
<b>Average Day Subbed Out:</b> Average of (Sub Day/Game Length)