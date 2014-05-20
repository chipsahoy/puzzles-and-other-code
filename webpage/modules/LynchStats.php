<?php
	if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "Long Games"; else $gametype = $_GET['gametype'];
	if($gametype == "Long Games")
		$qry = "select m.playerid, m.playername, count(*) games,
			sum(rs.faction = 1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.deathtype in ('Lynched','Conceded')) lynched, sum(rs.deathtype in ('Lynched','Conceded') and rs.faction=1) vlynched,
			sum(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19) wlynched,
			avg(if(rs.deathtype in ('Lynched') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglynchdayv,
			avg(if(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglynchdayw
			from playerlist pl
			join roleset rs using (gameid, slot)
			join game g using (gameid)
			join player p on p.playerid=pl.playerid
			join player m on m.playerid=p.mainplayerid
			where g.gametype not in ('Turbo','Turbo Mishmash') and pl.dayout is null
			group by p.mainplayerid 
			order by count(*) desc";
	elseif($gametype == "Vanillas")
		$qry = "select m.playerid, m.playername, count(*) games,
			sum(rs.faction = 1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.deathtype in ('Lynched','Conceded')) lynched, sum(rs.deathtype in ('Lynched','Conceded') and rs.faction=1) vlynched,
			sum(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19) wlynched,
			avg(if(rs.deathtype in ('Lynched') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglynchdayv,
			avg(if(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglynchdayw
			from playerlist pl
			join roleset rs using (gameid, slot)
			join game g using (gameid)
			join player p on p.playerid=pl.playerid
			join player m on m.playerid=p.mainplayerid
			where g.gametype in ('Vanilla','Slow Game') and pl.dayout is null
			group by p.mainplayerid 
			order by count(*) desc";
	elseif($gametype == "Vanilla+/Mish-Mashes")
		$qry = "select m.playerid, m.playername, count(*) games,
			sum(rs.faction = 1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.deathtype in ('Lynched','Conceded')) lynched, sum(rs.deathtype in ('Lynched','Conceded') and rs.faction=1) vlynched,
			sum(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19) wlynched,
			avg(if(rs.deathtype in ('Lynched') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglynchdayv,
			avg(if(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglynchdayw
			from playerlist pl
			join roleset rs using (gameid, slot)
			join game g using (gameid)
			join player p on p.playerid=pl.playerid
			join player m on m.playerid=p.mainplayerid
			where g.gametype not in ('Vanilla+','Mish-Mash') and pl.dayout is null
			group by p.mainplayerid 
			order by count(*) desc";
	elseif($gametype == "Turbos")
		$qry = "select m.playerid, m.playername, count(*) games,
			sum(rs.faction = 1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.deathtype in ('Lynched','Conceded')) lynched, sum(rs.deathtype in ('Lynched','Conceded') and rs.faction=1) vlynched,
			sum(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19) wlynched,
			avg(if(rs.deathtype in ('Lynched') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglynchdayv,
			avg(if(rs.deathtype in ('Lynched','Conceded') and rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglynchdayw
			from playerlist pl
			join roleset rs using (gameid, slot)
			join game g using (gameid)
			join player p on p.playerid=pl.playerid
			join player m on m.playerid=p.mainplayerid
			where g.gametype in ('Turbo','Turbo Mishmash') and pl.dayout is null
			group by p.mainplayerid 
			order by count(*) desc";
	$result = $db->query($qry);
?>
<h2>Lynch Stats - <?= $gametype?></h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Lynch Stats"/>
			Gametype: <select name="gametype" onchange="this.form.submit();">
				<option></option>
				<option>Long Games</option>
				<option>Vanillas</option>
				<option>Vanilla+/Mish-Mashes</option>
				<option>Turbos</option>
			</select>
</form>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=3 />
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
	<th>Village Games</th>
	<th>Wolf Games</th>
	<th>Lynched as Villager %</th>
	<th>Avg Day Lynched as Villager</th>
	<th>Lynched as Wolf %</th>
	<th>Avg Day Lynched as Wolf</th>
	</tr>
	</thead>
	<tbody>
<?php
	while($player = $result->fetch_assoc()) { 
		$vrate = NULL;
		$wrate = NULL;
		if ($player['vgames'] > 0) {
			$vrate = $player['vlynched']/$player['vgames']; }
		if ($player['wgames'] > 0) {
			$wrate = $player['wlynched']/$player['wgames']; }
		
		echo "<tr><td></td>
			<td><a href='index.php?report=Player&playerid=".$player['playerid']."'>".$player['playername']."</a></td>
			<td>".$player['games']."</td>
			<td>".$player['vgames']."</td>
			<td>".$player['wgames']."</td>
			<td>".number_format($vrate, 3)."</td>
			<td>".number_format($player['avglynchdayv'], 3)."</td>
			<td>".number_format($wrate, 3)."</td>
			<td>".number_format($player['avglynchdayw'], 3)."</td>
			</tr>";
	}
?>
	</tbody>
</table>

<b>Lynched as Wolf/Villager %:</b> Times Lynched as a (Wolf/Villager)/Games Played as a (Wolf/Villager)<br/>
<b>Avg Day Lynched as Wolf/Villager:</b> Average of (Lynch Day as (Wolf/Villager)/Game Length)<br/>
