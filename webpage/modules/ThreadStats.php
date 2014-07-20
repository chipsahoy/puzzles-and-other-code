<?php
$q = mysql_query("SELECT * FROM DerivedPosterThread dpt, Poster p, GameRole pt, Player p2, Team t, Thread t2, Game g, Affiliation a WHERE a.affiliationid = t.affiliationid AND g.threadid = t2.threadid AND t2.threadid = t.threadid AND pt.teamid = t.teamid AND t.threadid = dpt.threadid AND p2.posterid = p.posterid AND pt.roleid = p2.roleid AND dpt.posterid = p.posterid HAVING postcount > 250 ORDER BY t.threadid;");
?>

<h2>Thread Stats</h2>
Q: Why is this missing stats???<br/>
A: Because it's currently reloading (it takes about an hour!)<br/>
<table class="data" border="1" class="display">
	<thead>
	<tr>
	<th></th>
	<th>Name</th>
	<th>Game</th>
	<th>Role</th>
	<th>Posts</th>
	<th>% of Total Thread</th>
	<th>% of Total Thread times Players</th>
	</tr>
	</thead>
	<tbody>
<?php while($player = mysql_fetch_assoc($q)) { 
	$q2 = mysql_query("SELECT count(*) as playercount FROM Team t, GameRole pt WHERE pt.teamid = t.teamid AND t.threadid = ".$player['threadid']);
	$result = mysql_fetch_assoc($q2);
	$player['playercount'] = $result['playercount'];
?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
		<td><?php echo "<a href=\"index.php?report=Game&Game=".$player['gamename']."\">".$player['gamename']."</a> <a href=".htmlentities($player['url'], ENT_QUOTES, 'UTF-8').">(link)</a>" ?></td>
		<td><?= $player['affiliationname'] ?></td>
		<td><?= $player['postcount'] ?></td>
		<td><?= 100*$player['postcount']/$player['replies'] ?></td>
		<td><?= 100*($player['postcount']/$player['replies'])*$player['playercount'] ?></td>
    </tr>
<?php } // while ?>
	</tbody>
</table>
<input type="hidden" id="max" name="max"/>
<input type="hidden" id="min" name="min"/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=5 />
<input type="hidden" id="colwolf" name="colwolf" value=12 />
<input type="hidden" id="villagermin" name="villagermin"/>
<input type="hidden" id="villagermax" name="villagermax"/>
<input type="hidden" id="wolfmin" name="wolfmin"/>
<input type="hidden" id="wolfmax" name="wolfmax"/>