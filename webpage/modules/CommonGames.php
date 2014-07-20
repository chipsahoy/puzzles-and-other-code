<?php
	$player1 = $_GET['player1'];
	$player2 = $_GET['player2'];
	if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "Long Games"; else $gametype = $_GET['gametype'];
	$result = $db->query("select playername from player where playerid=".$player1)->fetch_assoc();
	$p1name = $result['playername'];
	$result = $db->query("select playername from player where playerid=".$player2)->fetch_assoc();
	$p2name = $result['playername'];
	
	if($gametype == "Long Games")
		$qry = "select g.gameid, g.gamename, g.startdate, g.gametype, g.url, f.factionname, if(t.victory=1,'Win',if(t.victory=0,'Loss','Tie')) result,
		if(p1.playerid<>p1.mainplayerid,p1.playername,'') gimmick1, if(p2.playerid<>p2.mainplayerid,p2.playername,'') gimmick2,
		r1.deathtype dt1, r1.deathday dd1, o1.rolename r1, r2.deathtype dt2, r2.deathday dd2, o2.rolename r2,
		pl1.ordinal ord1, pl1.dayin in1, pl1.dayout out1, pl2.ordinal ord2, pl2.dayin in2, pl2.dayout out2
		from playerlist pl1
		join playerlist pl2 using (gameid)
		join roleset r1 on r1.gameid=pl1.gameid and r1.slot=pl1.slot
		join roleset r2 on r2.gameid=pl2.gameid and r2.slot=pl2.slot
		join roles o1 on o1.roleid=r1.roletype
		join roles o2 on o2.roleid=r2.roletype
		join player p1 on p1.playerid=pl1.playerid
		join player p2 on p2.playerid=pl2.playerid
		join player pm on p2.mainplayerid=pm.playerid
		join game g on g.gameid=pl1.gameid
		join team t on t.gameid=g.gameid and t.faction=r1.faction
		join faction f on f.factionid=r1.faction
		where p1.mainplayerid=".$player1." and p2.mainplayerid=".$player2." and r1.faction=r2.faction
		and pl1.dayout is null and pl2.dayout is null
		and g.gametype not in ('Turbo','Turbo Mishmash') order by g.startdate desc";
	elseif($gametype == "Turbos")
		$qry = "select g.gameid, g.gamename, g.startdate, g.gametype, g.url, f.factionname, if(t.victory=1,'Win',if(t.victory=0,'Loss','Tie')) result,
		if(p1.playerid<>p1.mainplayerid,p1.playername,'') gimmick1, if(p2.playerid<>p2.mainplayerid,p2.playername,'') gimmick2,
		r1.deathtype dt1, r1.deathday dd1, o1.rolename r1, r2.deathtype dt2, r2.deathday dd2, o2.rolename r2,
		pl1.ordinal ord1, pl1.dayin in1, pl1.dayout out1, pl2.ordinal ord2, pl2.dayin in2, pl2.dayout out2
		from playerlist pl1
		join playerlist pl2 using (gameid)
		join roleset r1 on r1.gameid=pl1.gameid and r1.slot=pl1.slot
		join roleset r2 on r2.gameid=pl2.gameid and r2.slot=pl2.slot
		join roles o1 on o1.roleid=r1.roletype
		join roles o2 on o2.roleid=r2.roletype
		join player p1 on p1.playerid=pl1.playerid
		join player p2 on p2.playerid=pl2.playerid
		join player pm on p2.mainplayerid=pm.playerid
		join game g on g.gameid=pl1.gameid
		join team t on t.gameid=g.gameid and t.faction=r1.faction
		join faction f on f.factionid=r1.faction
		where p1.mainplayerid=".$player1." and p2.mainplayerid=".$player2." and r1.faction=r2.faction
		and pl1.dayout is null and pl2.dayout is null
		and g.gametype in ('Turbo','Turbo Mishmash') order by g.startdate desc";
	$result = $db->query($qry);
?>

<h2>Common Games - <?php echo $gametype?></h2>
<h3>Same Team</h3>
	<table class="data" border="1" class="display">
	<thead>
		<tr>
		<th rowspan="2"></th>
		<th rowspan="2">Start Date</th>
		<th rowspan="2">Game</th>
		<th rowspan="2">Type</th>
		<th rowspan="2">Affiliation</th>
		<th rowspan="2">Result</th>
		<th colspan="5"><?php echo $p1name;?></th>
		<th colspan="5"><?php echo $p2name;?></th>
		</tr><tr>
		<th>Gimmick</th>
		<th>Role</th>
		<th>Method of Death</th>
		<th>Survived Until</th>
		<th>Subbed In/Out</th>
		<th>Gimmick</th>
		<th>Role</th>
		<th>Method of Death</th>
		<th>Survived Until</th>
		<th>Subbed In/Out</th>
		</tr>
	</thead>
	<tbody>

<?php
	while($game = $result->fetch_assoc()) {
		$sub1 = '';
		if ($game['ord1'] > 1 or $game['out1'] != NULL): {
			$sub1 = '/';
			if ($game['ord1'] > 1) $sub1 = $game['in1'].$sub1;
				else $sub1 = '-/';
			if ($game['out1'] != NULL) $sub1 = $sub1.$game['out1'];
				else $sub1 = $sub1.'-'; }
		endif;
		$sub2 = '';
		if ($game['ord2'] > 1 or $game['out2'] != NULL): {
			$sub2 = '/';
			if ($game['ord2'] > 1) $sub2 = $game['in2'].$sub2;
				else $sub2 = '-/';
			if ($game['out2'] != NULL) $sub2 = $sub2.$game['out2'];
				else $sub2 = $sub2.'-'; }
		endif;
		
		echo "<tr>
			<td></td>
			<td>".$game['startdate']."</td>
			<td><a href='index.php?report=Game&gameid=".$game['gameid']."'>".$game['gamename']."</a>
				<a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
			<td>".$game['gametype']."</td>
			<td>".$game['factionname']."</td>
			<td>".$game['result']."</td>
			<td>".$game['gimmick1']."</td>
			<td>".$game['r1']."</td>
			<td>".$game['dt1']."</td>
			<td>".$game['dd1']."</td>
			<td>".$sub1."</td>
			<td>".$game['gimmick2']."</td>
			<td>".$game['r2']."</td>
			<td>".$game['dt2']."</td>
			<td>".$game['dd2']."</td>
			<td>".$sub2."</td>
			</tr>";
	}
?>
	</tbody>
</table>

<?php
	if($gametype == "Long Games")
		$qry = "select g.gameid, g.gamename, g.startdate, g.gametype, g.url, f1.factionname faction1, f2.factionname faction2, 
		if(t1.victory=1,'Win',if(t1.victory=0,'Loss','Tie')) result1, if(t2.victory=1,'Win',if(t2.victory=0,'Loss','Tie')) result2,
		if(p1.playerid<>p1.mainplayerid,p1.playername,'') gimmick1, if(p2.playerid<>p2.mainplayerid,p2.playername,'') gimmick2,
		r1.deathtype dt1, r1.deathday dd1, o1.rolename r1, r2.deathtype dt2, r2.deathday dd2, o2.rolename r2,
		pl1.ordinal ord1, pl1.dayin in1, pl1.dayout out1, pl2.ordinal ord2, pl2.dayin in2, pl2.dayout out2
		from playerlist pl1
		join playerlist pl2 using (gameid)
		join roleset r1 on r1.gameid=pl1.gameid and r1.slot=pl1.slot
		join roleset r2 on r2.gameid=pl2.gameid and r2.slot=pl2.slot
		join roles o1 on o1.roleid=r1.roletype
		join roles o2 on o2.roleid=r2.roletype
		join player p1 on p1.playerid=pl1.playerid
		join player p2 on p2.playerid=pl2.playerid
		join player pm on p2.mainplayerid=pm.playerid
		join game g on g.gameid=pl1.gameid
		join team t1 on t1.gameid=g.gameid and t1.faction=r1.faction
		join team t2 on t2.gameid=g.gameid and t2.faction=r2.faction
		join faction f1 on f1.factionid=r1.faction
		join faction f2 on f2.factionid=r2.faction
		where p1.mainplayerid=".$player1." and p2.mainplayerid=".$player2." and r1.faction<>r2.faction
		and pl1.dayout is null and pl2.dayout is null
		and g.gametype not in ('Turbo','Turbo Mishmash') order by g.startdate desc";
	elseif($gametype == "Turbos")
		$qry = "select g.gameid, g.gamename, g.startdate, g.gametype, g.url, f1.factionname faction1, f2.factionname faction2, 
		if(t1.victory=1,'Win',if(t1.victory=0,'Loss','Tie')) result1, if(t2.victory=1,'Win',if(t2.victory=0,'Loss','Tie')) result2,
		if(p1.playerid<>p1.mainplayerid,p1.playername,'') gimmick1, if(p2.playerid<>p2.mainplayerid,p2.playername,'') gimmick2,
		r1.deathtype dt1, r1.deathday dd1, o1.rolename r1, r2.deathtype dt2, r2.deathday dd2, o2.rolename r2,
		pl1.ordinal ord1, pl1.dayin in1, pl1.dayout out1, pl2.ordinal ord2, pl2.dayin in2, pl2.dayout out2
		from playerlist pl1
		join playerlist pl2 using (gameid)
		join roleset r1 on r1.gameid=pl1.gameid and r1.slot=pl1.slot
		join roleset r2 on r2.gameid=pl2.gameid and r2.slot=pl2.slot
		join roles o1 on o1.roleid=r1.roletype
		join roles o2 on o2.roleid=r2.roletype
		join player p1 on p1.playerid=pl1.playerid
		join player p2 on p2.playerid=pl2.playerid
		join player pm on p2.mainplayerid=pm.playerid
		join game g on g.gameid=pl1.gameid
		join team t1 on t1.gameid=g.gameid and t1.faction=r1.faction
		join team t2 on t2.gameid=g.gameid and t2.faction=r2.faction
		join faction f1 on f1.factionid=r1.faction
		join faction f2 on f2.factionid=r2.faction
		where p1.mainplayerid=".$player1." and p2.mainplayerid=".$player2." and r1.faction<>r2.faction
		and pl1.dayout is null and pl2.dayout is null
		and g.gametype in ('Turbo','Turbo Mishmash') order by g.startdate desc";
	$result = $db->query($qry);
?>
<h3>Different Team</h3>
	<table class="data" border=1>
	<thead>
		<tr>
			<th rowspan="2"></th>
			<th rowspan="2">Start Date</th>
			<th rowspan="2">Game</th>
			<th rowspan="2">Type</th>
			<th colspan="7"><?php echo $p1name;?></th>
			<th colspan="7"><?php echo $p2name;?></th>
		</tr>
		<tr>
			<th>Affiliation</th>
			<th>Result</th>
			<th>Gimmick</th>
			<th>Role</th>
			<th>Method of Death</th>
			<th>Survived Until</th>
			<th>Subbed In/Out</th>
			<th>Affiliation</th>
			<th>Result</th>
			<th>Gimmick</th>
			<th>Role</th>
			<th>Method of Death</th>
			<th>Survived Until</th>
			<th>Subbed In/Out</th>
			</tr>
	</thead>
	<tbody>

<?php
	while($game = $result->fetch_assoc()) {
		$sub1 = '';
		if ($game['ord1'] > 1 or $game['out1'] != NULL): {
			$sub1 = '/';
			if ($game['ord1'] > 1) $sub1 = $game['in1'].$sub1;
				else $sub1 = '-/';
			if ($game['out1'] != NULL) $sub1 = $sub1.$game['out1'];
				else $sub1 = $sub1.'-'; }
		endif;
		$sub2 = '';
		if ($game['ord2'] > 1 or $game['out2'] != NULL): {
			$sub2 = '/';
			if ($game['ord2'] > 1) $sub2 = $game['in2'].$sub2;
				else $sub2 = '-/';
			if ($game['out2'] != NULL) $sub2 = $sub2.$game['out2'];
				else $sub2 = $sub2.'-'; }
		endif;
		
		echo "<tr>
			<td></td>
			<td>".$game['startdate']."</td>
			<td><a href='index.php?report=Game&gameid=".$game['gameid']."'>".$game['gamename']."</a>
				<a href=".htmlentities($game['url'], ENT_QUOTES, 'UTF-8').">(link)</a></td>
			<td>".$game['gametype']."</td>
			<td>".$game['faction1']."</td>
			<td>".$game['result1']."</td>
			<td>".$game['gimmick1']."</td>
			<td>".$game['r1']."</td>
			<td>".$game['dt1']."</td>
			<td>".$game['dd1']."</td>
			<td>".$sub1."</td>
			<td>".$game['faction2']."</td>
			<td>".$game['result2']."</td>
			<td>".$game['gimmick2']."</td>
			<td>".$game['r2']."</td>
			<td>".$game['dt2']."</td>
			<td>".$game['dd2']."</td>
			<td>".$sub2."</td>
			</tr>";
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