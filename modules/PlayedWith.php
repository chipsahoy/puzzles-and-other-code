<?php
	$query = "SELECT posterid FROM Poster WHERE postername = '".mysql_real_escape_string($_GET['Player'])."'";
	$result = mysql_query($query);
	$result_arrayid = mysql_fetch_assoc($result);
?>
<h3>Most Played With</h3>
<?php
$result = mysql_query("SELECT p22.posterid, p22.postername, count(*) as total FROM Poster p22, Player p2, Player p1, GameRole pt1, GameRole pt2, Team t1, Team t2 WHERE p22.posterid = p2.posterid AND t1.threadid = t2.threadid AND t1.teamid = pt1.teamid AND t2.teamid = pt2.teamid AND p2.roleid = pt2.roleid AND pt1.roleid = p1.roleid AND p1.posterid = ".$result_arrayid['posterid']." AND p1.posterid != p2.posterid GROUP BY p22.postername");
$bros = array();
while($result_array = mysql_fetch_assoc($result)) {
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['villabros'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND (UPPER(LEFT(a.affiliationname,3)) = 'WOL' OR UPPER(LEFT(a.affiliationname,3)) = 'NEU') AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['wolfbros'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE t.victory = 1 AND pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['villawins'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE t.victory = 0 AND pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['villalosses'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE t.victory = 2 AND pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['villaties'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE t.victory = 1 AND pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND (UPPER(LEFT(a.affiliationname,3)) = 'WOL' OR UPPER(LEFT(a.affiliationname,3)) = 'NEU')AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['wolfwins'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE t.victory = 0 AND pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND (UPPER(LEFT(a.affiliationname,3)) = 'WOL' OR UPPER(LEFT(a.affiliationname,3)) = 'NEU')AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['wolflosses'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Team t, Player p1, Player p2, Affiliation a WHERE t.victory = 2 AND pt2.teamid = t.teamid AND t.teamid = pt1.teamid AND t.affiliationid = a.affiliationid AND (UPPER(LEFT(a.affiliationname,3)) = 'WOL' OR UPPER(LEFT(a.affiliationname,3)) = 'NEU')AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['wolfties'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Player p2, Team t1, Team t2, Player p1 WHERE pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND t1.threadid = t2.threadid AND t2.teamid != t1.teamid AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['versusgames'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Player p2, Team t1, Team t2, Player p1 WHERE t1.victory = 1 AND t2.victory = 0 AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND t1.threadid = t2.threadid AND t2.teamid != t1.teamid AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['versuswins'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Player p2, Team t1, Team t2, Player p1 WHERE t1.victory = 0 AND t2.victory = 1 AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND t1.threadid = t2.threadid AND t2.teamid != t1.teamid AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['versuslosses'] = mysql_result(mysql_query($query),0);
	$query = "SELECT count(*) FROM GameRole pt1, GameRole pt2, Player p2, Team t1, Team t2, Player p1 WHERE ((t1.victory = 0 AND t2.victory = 0) OR (t1.victory = 1 AND t2.victory = 1) OR t2.victory = 2 OR t1.victory = 2) AND pt1.teamid = t1.teamid AND pt2.teamid = t2.teamid AND t1.threadid = t2.threadid AND t2.teamid != t1.teamid AND p1.roleid = pt1.roleid AND p2.roleid = pt2.roleid AND p2.posterid = ".$result_array['posterid']." AND p1.posterid != p2.posterid AND p1.posterid = ".$result_arrayid['posterid'];
	$result_array['versusties'] = mysql_result(mysql_query($query),0);

	$bros[] = $result_array;
}
?>
<table class="data" border=1>
	<thead>
		<tr><th rowspan=2></th><th rowspan=2>Name</th><th rowspan=2>Total Games</th><th rowspan=2>Link to Games</th><th colspan=5>Villa Bros</th><th colspan=5>Wolf/Neutral Bros</th><th colspan=5>Versus</th></tr>
		<tr><th>Games</th><th>Wins</th><th>Losses</th><th>Ties</th><th>%</th><th>Games</th><th>Wins</th><th>Losses</th><th>Ties</th><th>%</th><th>Games</th><th>Wins</th><th>Losses</th><th>Ties</th><th>%</th></tr>
	</thead>
	<tbody>
		<?php
		foreach($bros as $bro)
		{
			echo "<tr><td></td><td><a href=\"index.php?report=Player&Player=".htmlentities($bro['postername'], ENT_QUOTES, 'UTF-8')."\">".$bro['postername']."</a></td><td>".$bro['total']."</td><td><a href=\"index.php?report=Common+Games&Player1=".htmlentities(mysql_real_escape_string($_GET['Player']), ENT_QUOTES, 'UTF-8')."&Player2=".htmlentities($bro['postername'], ENT_QUOTES, 'UTF-8')."\">(details)</a></td><td>".$bro['villabros']."</td><td>".$bro['villawins']."</td><td>".$bro['villalosses']."</td><td>".$bro['villaties']."</td><td>";
			if($bro['villawins']+$bro['villalosses'] > 0) echo number_format(round($bro['villawins']/($bro['villawins']+$bro['villalosses']),3),3); else echo 0;
			echo "</td><td>".$bro['wolfbros']."</td><td>".$bro['wolfwins']."</td><td>".$bro['wolflosses']."</td><td>".$bro['wolfties']."</td><td>";
			if($bro['wolfwins']+$bro['wolflosses'] > 0) echo number_format(round($bro['wolfwins']/($bro['wolfwins']+$bro['wolflosses']),3),3); else echo 0;
			echo "</td><td>".$bro['versusgames']."</td><td>".$bro['versuswins']."</td><td>".$bro['versuslosses']."</td><td>".$bro['versusties']."</td><td>";
			if($bro['versuswins']+$bro['versuslosses'] > 0) echo number_format(round($bro['versuswins']/($bro['versuswins']+$bro['versuslosses']),3),3); else echo 0;
			echo "</td></tr>";
		}	
	 ?>
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