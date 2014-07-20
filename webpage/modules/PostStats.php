<?php
$q = mysql_query("SELECT 
p.posterid, 
p.postername, 
(select count(*) from GameRole pt, Player p2 where pt.roleid=p2.roleid AND p2.posterid = p.posterid) as game_count,
(select count(*) from GameRole pt, Team t, Player p2, Affiliation a where p2.posterid = p.posterid AND pt.roleid=p2.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL') as wolf_games,
(select count(*) from GameRole pt, Team t, Player p2, Affiliation a where p2.posterid = p.posterid AND pt.roleid=p2.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL') as village_games,
(SELECT AVG(postcount/(dpt.endday - dpt.startday)) FROM DerivedPosterThread dpt WHERE dpt.posterid = p.posterid) as posts_per_day,
(SELECT AVG(postcount/(dpt.endday - dpt.startday)) FROM DerivedPosterThread dpt, GameRole pt, Player p2, Team t, Affiliation a WHERE t.threadid = dpt.threadid AND dpt.posterid = p.posterid AND p.posterid = p2.posterid AND p2.roleid = pt.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL') as villager_posts_per_day,
(SELECT AVG(postcount/(dpt.endday - dpt.startday)) FROM DerivedPosterThread dpt, GameRole pt, Player p2, Team t, Affiliation a WHERE t.threadid = dpt.threadid AND dpt.posterid = p.posterid AND p.posterid = p2.posterid AND p2.roleid = pt.roleid AND pt.teamid = t.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL') as wolf_posts_per_day
FROM
Poster as p
HAVING 
game_count > 0;");

?>

<h2>Post Stats</h2>
Q: Why is this missing stats???<br/>
A: Because it's currently reloading (it takes about an hour!)<br/>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=4 />
<input type="hidden" id="colwolf" name="colwolf" value=3 />
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
	<th>Wolf Games</th>
	<th>Village Games</th>
	<th>Posts Per Day</th>
	<th>Villager Posts Per Day</th>
	<th>Wolf Posts Per Day</th>
	<th>Villager/Wolf Posts Per Day</th>
	</tr>
	</thead>
	<tbody>
<?php while($player = mysql_fetch_assoc($q)) { 
?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
        <td><?= $player['game_count'] ?></td>
		<td><?= $player['wolf_games'] ?></td>
		<td><?= $player['village_games'] ?></td>
		<td><?= $player['posts_per_day'] ?></td>
		<td><?= $player['villager_posts_per_day'] ?></td>
		<td><?= $player['wolf_posts_per_day'] ?></td>
		<td><?= $player['villager_posts_per_day']/$player['wolf_posts_per_day'] ?></td>
    </tr>
<?php } // while ?>
	</tbody>
</table>