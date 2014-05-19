<?php 
	$playerid = $_GET['playerid'];
	#$playername = mysql_fetch_assoc(mysql_query("SELECT playername FROM player WHERE playerid = ".mysql_real_escape_string($playerid)));
	$result = $db->query("SELECT playername FROM player WHERE playerid=".$playerid)->fetch_assoc();
	$playername = $result['playername'];
	
	echo "<h2>".$playername."</h2>";

	# modded games
	$Games = array();
	
	$qry = ("select p.playername, g.gameid, url, gamename, gametype, startdate, numplayers, gamelength, 
		if(tiegame, 'Tie', v.victor) victor, if(m.isprimary, 'Yes', '') isprimary
		from game g join moderator m using (gameid) join player p on p.playerid=m.modid join victor v using (gameid)
		where p.mainplayerid=".$playerid." and g.gametype <> 'Turbo' group by g.gameid order by startdate desc");
	$result = $db->query($qry);
	
	if ($result->num_rows > 0):
?>

<h3>Moderated Games</h3>
<table class="data" border="1">
<thead>
	<tr>
	<th></th>
	<th>Start Date</th>
	<th>Name</th>
	<th>Type</th>
	<th># of Players</th>
	<th>Length</th>
	<th>Victor</th>
	<th>Lead Mod</th>
	<th>Account</th>
	</tr>
</thead>
<tbody>
	<?php
	while($game = $result->fetch_assoc())
	{
		echo("<tr><td></td>
			<td>".$game['startdate']."</td>
			<td><a href=\"index.php?report=Game&gameid=".
				str_replace('#', '%23', htmlentities($game['gameid'], ENT_QUOTES, 'UTF-8'))."\">".
				$game['gamename']."</a> <a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
			<td>".$game['gametype']."</td>
			<td>".$game['numplayers']."</td>
			<td>".$game['gamelength']."</td>
			<td>".$game['victor']."</td>
			<td>".$game['isprimary']."</td>
			<td>".$game['playername']."</td>
			</tr>");
	}
	endif;
?>
</tbody>
</table>

<?php
	$qry = "select if(p.playerid=p.mainplayerid,'',p.playername) playername, gamename, gametype, startdate, gameid, factionname, deathday, 
		deathtype, victory, rolename, pc.posts postcount, url, pl.ordinal, pl.dayin, pl.dayout
		from game g
		join team t using (gameid)
		join roleset rs using (gameid, faction)
		join faction f on f.factionid=t.faction
		join playerlist pl using (gameid, slot)
		join roles r on r.roleid=rs.roletype
		join player p on p.playerid=pl.playerid
		left join postcount pc on pc.threadid=g.gameid and pc.posterid=pl.playerid
		where p.mainplayerid =" .$playerid. " and g.gametype <> 'Turbo' order by startdate desc, deathday desc";
	$result = $db->query($qry);
	
	echo "<h3>Long Games</h3>";
	
?>
	<table class="data" border=1>
	<thead>
		<tr>
		<th></th>
		<th>Start Date</th>
		<th>Game</th>
		<th>Gimmick</th>
		<th>Type</th>
		<th>Affiliation</th>
		<th>Role</th>
		<th>Method of Death</th>
		<th>Survived Until</th>
		<th>Result</th>
		<th>Subbed In/Out</th>
		<th>Post Count</th>
	</thead>
	<tbody>
		<?php
		while($game = $result->fetch_assoc())
		{
			if ($game['victory'] == 1) $victory = 'Win';
				elseif($game['victory'] == 0) $victory = 'Loss';
				else $victory = 'Tie';
			if ($game['rolename'] == 'Vanilla') $role = '';
				else $role = $game['rolename'];
			$sub = '';
			if ($game['ordinal'] > 1 or $game['dayout'] != NULL):
			{
				$sub = '/';
				if ($game['ordinal'] > 1) $sub = $game['dayin'].$sub;
					else $sub = '-/';
				if ($game['dayout'] != NULL) $sub = $sub.$game['dayout'];
					else $sub = $sub.'-';
			}
			endif;
			
			echo "<tr>
			<td></td>
			<td>".$game['startdate']."</td>
			<td><a href=\"index.php?report=Game&gameid=".str_replace('#', '%23', htmlentities($game['gameid'], ENT_QUOTES, 'UTF-8'))."\">".
				$game['gamename']."</a> <a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
			<td>".$game['playername']."</td>
			<td>".$game['gametype']."</td>
			<td>".$game['factionname']."</td>
			<td>".$role."</td>
			<td>".$game['deathtype']."</td>
			<td>".$game['deathday']."</td>
			<td>".$victory."</td>
			<td>".$sub."</td>
			<td>".$game['postcount']."</td>
			</tr>";
		}	
	 ?>
	</tbody>
</table>


<?php
	# turbos
	$turbos = array();

	$qry = "select if(p.playerid=p.mainplayerid,'',p.playername) playername, gamename, gametype, startdate, gameid, factionname, deathday, 
		deathtype, victory, rolename, pc.posts postcount, url, pl.ordinal, pl.dayin, pl.dayout
		from game g
		join team t using (gameid)
		join roleset rs using (gameid, faction)
		join faction f on f.factionid=t.faction
		join playerlist pl using (gameid, slot)
		join roles r on r.roleid=rs.roletype
		join player p on p.playerid=pl.playerid
		left join postcount pc on pc.threadid=g.gameid and pc.posterid=pl.playerid
		where p.mainplayerid =" .$playerid. " and g.gametype = 'Turbo' order by startdate desc, deathday desc";
	$result = $db->query($qry);
	
	if ($result->num_rows > 0):
		echo "<h3>Turbos</h3>"
?>
	<table class="data" border=1>
	<thead>
		<tr>
		<th></th>
		<th>Start Date</th>
		<th>Game</th>
		<th>Gimmick</th>
		<th>Affiliation</th>
		<th>Role</th>
		<th>Method of Death</th>
		<th>Survived Until</th>
		<th>Result</th>
		<th>Subbed In/Out</th>
		<th>Post Count</th>
	</thead>
	<tbody>
		<?php
		while($game = $result->fetch_assoc())
		{
			if ($game['victory'] == 1) $victory = 'Win';
				elseif($game['victory'] == 0) $victory = 'Loss';
				else $victory = 'Tie';
			if ($game['rolename'] == 'Vanilla') $role = '';
				else $role = $game['rolename'];
			$sub = '';
			if ($game['ordinal'] > 1 or $game['dayout'] != NULL):
			{
				$sub = '/';
				if ($game['ordinal'] > 1) $sub = $game['dayin'].$sub;
					else $sub = '-/';
				if ($game['dayout'] != NULL) $sub = $sub.$game['dayout'];
					else $sub = $sub.'-';
			}
			endif;
			
			echo "<tr>
			<td></td>
			<td>".$game['startdate']."</td>
			<td><a href=\"index.php?report=Game&gameid=".$game['gameid']."\">".
				$game['gamename']."</a> <a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
			<td>".$game['playername']."</td>
			<td>".$game['factionname']."</td>
			<td>".$role."</td>
			<td>".$game['deathtype']."</td>
			<td>".$game['deathday']."</td>
			<td>".$victory."</td>
			<td>".$sub."</td>
			<td>".$game['postcount']."</td>
			</tr>";
		}	
	 ?>
	</tbody>
</table>
<?php
	endif;
	include "./modules/PlayedWith.php";
?>
