<?php
	$playerid = $_GET['playerid'];
	if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "Long Games"; else $gametype = $_GET['gametype'];
	if($gametype == "Long Games")
		$qry = "select pm.playerid, pm.playername, count(*) games
		from playerlist pl1
		join playerlist pl2 using (gameid)
		join player p1 on p1.playerid=pl1.playerid
		join player p2 on p2.playerid=pl2.playerid
		join player pm on p2.mainplayerid=pm.playerid
		join game g on g.gameid=pl1.gameid
		where p1.mainplayerid=".$playerid." and p2.mainplayerid <> p1.mainplayerid and g.gametype not in ('Turbo','Turbo Mishmash')
		and pl1.dayout is null and pl2.dayout is null
		group by p2.mainplayerid order by 3 desc";
	elseif($gametype == "Turbos")
		$qry = "select pm.playerid, pm.playername, count(*) games
		from playerlist pl1
		join playerlist pl2 using (gameid)
		join player p1 on p1.playerid=pl1.playerid
		join player p2 on p2.playerid=pl2.playerid
		join player pm on p2.mainplayerid=pm.playerid
		join game g on g.gameid=pl1.gameid
		where p1.mainplayerid=".$playerid." and p2.mainplayerid <> p1.mainplayerid and g.gametype in ('Turbo','Turbo Mishmash')
		and pl1.dayout is null and pl2.dayout is null
		group by p2.mainplayerid order by 3 desc";
	$result = $db->query($qry);
?>

<h3>Most Played With - <?= $gametype?></h3>
<form action="index.php" method="get">
	<input type="hidden" name="report" value="Player"/>
	<input type="hidden" name="playerid" value="<?php echo $playerid;?>" />
	Gametype: <select name="gametype" onchange="this.form.submit();">
		<option></option>
		<option>Long Games</option>
		<option>Turbos</option>
	</select>
</form>

<table class="data" border=1>
	<thead>
		<tr>
		<th rowspan=2></th>
		<th rowspan=2>Name</th>
		<th rowspan=2>Total Games</th>
		<th rowspan=2>Link to Games</th>
		<th colspan=3>Villa Bros</th>
		<th colspan=3>Wolf/Neutral Bros</th>
		<th colspan=3>V vs W/N</th>
		<th colspan=3>W/N vs V/W/N</th>
		</tr><tr>
		<th>Games</th>
		<th>Wins</th>
		<th>Losses</th>
		<th>Games</th>
		<th>Wins</th>
		<th>Losses</th>
		<th>Games</th>
		<th>Wins</th>
		<th>Losses</th>
		<th>Games</th>
		<th>Wins</th>
		<th>Losses</th>
		</tr>
	</thead>
	<tbody>
<?php
	while($otherplayer = $result->fetch_assoc()) {
		if($gametype == "Long Games")
			$qry = "select teammate, villager, count(victory) games, sum(victory=1) wins, sum(victory=0) losses
			from (select r1.faction=r2.faction teammate, r1.faction=1 villager, t.victory
			from player p1, player p2, playerlist pl1, playerlist pl2, roleset r1, roleset r2, team t, game g
			where p1.mainplayerid=".$playerid." and p2.mainplayerid=".$otherplayer['playerid']." and pl1.gameid=pl2.gameid
			and pl1.playerid=p1.playerid and pl2.playerid=p2.playerid and t.gameid=pl1.gameid and t.faction=r1.faction
			and r1.gameid=pl1.gameid and r1.slot=pl1.slot and r2.gameid=pl2.gameid and r2.slot=pl2.slot
			and g.gameid=t.gameid and g.gametype not in ('Turbo','Turbo Mishmash')
			and pl1.dayout is null and pl2.dayout is null
			union all select 0, 0, NULL
			union all select 0, 1, NULL
			union all select 1, 0, NULL
			union all select 1, 1, NULL) x
			group by teammate, villager
			order by teammate desc, villager desc";
		elseif($gametype == "Turbos")
			$qry = "select teammate, villager, count(victory) games, sum(victory=1) wins, sum(victory=0) losses
			from (select r1.faction=r2.faction teammate, r1.faction=1 villager, t.victory
			from player p1, player p2, playerlist pl1, playerlist pl2, roleset r1, roleset r2, team t, game g
			where p1.mainplayerid=".$playerid." and p2.mainplayerid=".$otherplayer['playerid']." and pl1.gameid=pl2.gameid
			and pl1.playerid=p1.playerid and pl2.playerid=p2.playerid and t.gameid=pl1.gameid and t.faction=r1.faction
			and r1.gameid=pl1.gameid and r1.slot=pl1.slot and r2.gameid=pl2.gameid and r2.slot=pl2.slot
			and g.gameid=t.gameid and g.gametype in ('Turbo','Turbo Mishmash')
			and pl1.dayout is null and pl2.dayout is null
			union all select 0, 0, NULL
			union all select 0, 1, NULL
			union all select 1, 0, NULL
			union all select 1, 1, NULL) x
			group by teammate, villager
			order by teammate desc, villager desc";		
		$result2 = $db->query($qry);
		
		echo "<tr><td></td><td><a href='index.php?report=Player&playerid=".$otherplayer['playerid']."'>".$otherplayer['playername']."</a></td><td>".$otherplayer['games'].
			"</td><td><a href='index.php?report=Common+Games&player1=".$playerid."&player2=".$otherplayer['playerid']."&gametype=".$gametype."'>(details)</a></td>";
		$other = $result2->fetch_assoc();
		echo "<td>".$other['games']."</td><td>".$other['wins']."</td><td>".$other['losses']."</td>";
		$other = $result2->fetch_assoc();
		echo "<td>".$other['games']."</td><td>".$other['wins']."</td><td>".$other['losses']."</td>";
		$other = $result2->fetch_assoc();
		echo "<td>".$other['games']."</td><td>".$other['wins']."</td><td>".$other['losses']."</td>";
		$other = $result2->fetch_assoc();
		echo "<td>".$other['games']."</td><td>".$other['wins']."</td><td>".$other['losses']."</td>";
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