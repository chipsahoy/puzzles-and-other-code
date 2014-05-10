<?php
	$query = "SELECT posterid FROM Poster WHERE postername = '".mysql_real_escape_string($_GET['Player1'])."'";
	$result = mysql_query($query);
	$result_arrayid = mysql_fetch_assoc($result);
	$query = "SELECT posterid FROM Poster WHERE postername = '".mysql_real_escape_string($_GET['Player2'])."'";
	$result = mysql_query($query);
	$result_arrayid2 = mysql_fetch_assoc($result);
	
	$query = "SELECT p.startday, p.endday, g.gamename, g.gametype, g.startdate, g.threadid, t2.url, g.threadid, t.victory, t.teamid, a.affiliationname, r.rolename, dt.deathtypename, pt.deathday FROM DeathType dt, Role r, Affiliation as a, Thread as t2, Game as g, GameRole as pt, Team as t, Player p WHERE dt.deathtypeid = pt.deathtypeid AND pt.roletypeid = r.roleid AND t.affiliationid = a.affiliationid AND g.threadid = t.threadid AND t2.threadid = g.threadid AND t2.subforumid = g.subforumid AND pt.teamid = t.teamid AND pt.roleid = p.roleid AND p.posterid = ".$result_arrayid['posterid'];
	$result = mysql_query($query);
	$playedgames = array();
	while($result_array = mysql_fetch_assoc($result))
	{
		$playedgames[] = $result_array;
	}
	$query = "SELECT p.startday, p.endday, g.gamename, g.gametype, g.startdate, g.threadid, t2.url, g.threadid, t.victory, t.teamid, a.affiliationname, r.rolename, dt.deathtypename, pt.deathday FROM DeathType dt, Role r, Affiliation as a, Thread as t2, Game as g, GameRole as pt, Team as t, Player p WHERE dt.deathtypeid = pt.deathtypeid AND pt.roletypeid = r.roleid AND t.affiliationid = a.affiliationid AND g.threadid = t.threadid AND t2.threadid = g.threadid AND t2.subforumid = g.subforumid AND pt.teamid = t.teamid AND pt.roleid = p.roleid AND p.posterid = ".$result_arrayid2['posterid'];
	$result = mysql_query($query);
	$playedgames2 = array();
	while($result_array = mysql_fetch_assoc($result))
	{
		$playedgames2[] = $result_array;
	}
?>
<h2>Common Games</h2>
<h3>Same Team</h3>
	<table class="data" border="1" class="display">
	<thead>
		<tr>
		<th rowspan="2"></th><th rowspan="2">Start Date</th><th rowspan="2">Game</th><th rowspan="2">Type</th><th rowspan="2">Affiliation</th><th rowspan="2">Result</th>
		<th colspan="5"><?php echo $_GET['Player1'];?></th>
		<th colspan="5"><?php echo $_GET['Player2'];?></th>
		</tr>
		<tr><th>Role</th><th>Method of Death</th><th>Survived Until</th><th>Subbed In Day?</th><th>Subbed Out Day?</th><th>Role</th><th>Method of Death</th><th>Survived Until</th><th>Subbed In Day?</th><th>Subbed Out Day?</th></tr>
	</thead>
	<tbody>
		<?php
		foreach($playedgames as $game)
		{
			foreach($playedgames2 as $game2)
			{
				if($game['threadid'] == $game2['threadid'] && $game['teamid'] == $game2['teamid'])
				{
					if($game['victory'] == 1) $victory = 'Win';
					elseif($game['victory'] == 0) $victory = 'Loss';
					else $victory = 'Tie';
					if($game['startday'] == 0) $game['startday'] = "";
					if($game2['startday'] == 0) $game2['startday'] = "";
					echo "<tr>
						<td></td>
						<td>".$game['startdate']."</td>
						<td><a href=\"index.php?report=Game&Game=".htmlentities($game['gamename'], ENT_QUOTES, 'UTF-8')."\">".$game['gamename']."</a> <a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
						<td>".$game['gametype']."</td>
						<td>".$game['affiliationname']."</td>
						<td>".$victory."</td>
						<td>".$game['rolename']."</td>
						<td>".$game['deathtypename']."</td>
						<td>".$game['deathday']."</td>
						<td>".$game['startday']."</td>
						<td>".$game['endday']."</td>
						<td>".$game2['rolename']."</td>
						<td>".$game2['deathtypename']."</td>
						<td>".$game2['deathday']."</td>
						<td>".$game2['startday']."</td>
						<td>".$game2['endday']."</td>
					</tr>";
				}
			}
		}	
	 ?>
	</tbody>
</table>
<h3>Different Team</h3>
	<table class="data" border=1>
	<thead>
		<tr>
			<th rowspan="2"></th>
			<th rowspan="2">Start Date</th>
			<th rowspan="2">Game</th>
			<th rowspan="2">Type</th>
			
			<th colspan="7"><?php echo $_GET['Player1'];?></th>
			<th colspan="7"><?php echo $_GET['Player2'];?></th>
		</tr>
		<tr>
			<th>Affiliation</th>
			<th>Result</th>
			<th>Role</th>
			<th>Method of Death</th>
			<th>Survived Until</th>
			<th>Subbed In Day?</th>
			<th>Subbed Out Day?</th>
			<th>Affiliation</th>
			<th>Result</th>
			<th>Role</th>
			<th>Method of Death</th>
			<th>Survived Until</th>
			<th>Subbed In Day?</th>
			<th>Subbed Out Day?</th>
			</tr>
	</thead>
	<tbody>
		<?php
		foreach($playedgames as $game)
		{
			foreach($playedgames2 as $game2)
			{
				if($game['threadid'] == $game2['threadid'] && $game['teamid'] != $game2['teamid'])
				{
					if($game['victory'] == 1) $victory = 'Win';
					elseif($game['victory'] == 0) $victory = 'Loss';
					else $victory = 'Tie';
					if($game2['victory'] == 1) $victory2 = 'Win';
					elseif($game2['victory'] == 0) $victory2 = 'Loss';
					else $victory2 = 'Tie';
					if($game['startday'] == 0) $game['startday'] = "";
					if($game2['startday'] == 0) $game2['startday'] = "";
					echo "<tr>
						<td></td>
						<td>".$game['startdate']."</td>
						<td><a href=\"index.php?report=Game&Game=".htmlentities($game['gamename'], ENT_QUOTES, 'UTF-8')."\">".$game['gamename']."</a> <a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
						<td>".$game['gametype']."</td>
						<td>".$game['affiliationname']."</td>
						<td>".$victory."</td>
						<td>".$game['rolename']."</td>
						<td>".$game['deathtypename']."</td>
						<td>".$game['deathday']."</td>
						<td>".$game['startday']."</td>
						<td>".$game['endday']."</td>
						<td>".$game2['affiliationname']."</td>
						<td>".$victory2."</td>
						<td>".$game2['rolename']."</td>
						<td>".$game2['deathtypename']."</td>
						<td>".$game2['deathday']."</td>
						<td>".$game2['startday']."</td>
						<td>".$game2['endday']."</td>
					</tr>";
				}
			}
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