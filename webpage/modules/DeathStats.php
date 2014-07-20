<?php
	if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "Long Games"; else $gametype = $_GET['gametype'];
	if($gametype == "Long Games")
		$qry = "select m.playerid, m.playername, count(*) games,
			sum(rs.deathtype = 'Mod Killed') modkill, 
			sum(rs.faction=1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.faction=1 and rs.deathtype='Survived') vsurv,
			sum(rs.faction between 11 and 19 and rs.deathtype='Survived') wsurv,
			sum(rs.faction=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vnk,
			sum(rs.faction=1 and roletype=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vanillank,
			sum(rs.faction=1 and rs.deathtype in ('Lynched','Eaten')) vl,
			avg(if(rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglifev,
			avg(if(rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglifew,
			avg(if(rs.deathtype in ('Night Killed','Day Killed') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avgnkv
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
			sum(rs.deathtype = 'Mod Killed') modkill, 
			sum(rs.faction=1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.faction=1 and rs.deathtype='Survived') vsurv,
			sum(rs.faction between 11 and 19 and rs.deathtype='Survived') wsurv,
			sum(rs.faction=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vnk,
			sum(rs.faction=1 and roletype=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vanillank,
			sum(rs.faction=1 and rs.deathtype in ('Lynched','Eaten')) vl,
			avg(if(rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglifev,
			avg(if(rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglifew,
			avg(if(rs.deathtype in ('Night Killed','Day Killed') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avgnkv
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
			sum(rs.deathtype = 'Mod Killed') modkill, 
			sum(rs.faction=1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.faction=1 and rs.deathtype='Survived') vsurv,
			sum(rs.faction between 11 and 19 and rs.deathtype='Survived') wsurv,
			sum(rs.faction=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vnk,
			sum(rs.faction=1 and roletype=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vanillank,
			sum(rs.faction=1 and rs.deathtype in ('Lynched','Eaten')) vl,
			avg(if(rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglifev,
			avg(if(rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglifew,
			avg(if(rs.deathtype in ('Night Killed','Day Killed') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avgnkv
			from playerlist pl
			join roleset rs using (gameid, slot)
			join game g using (gameid)
			join player p on p.playerid=pl.playerid
			join player m on m.playerid=p.mainplayerid
			where g.gametype in ('Vanilla+','Mish-Mash') and pl.dayout is null
			group by p.mainplayerid 
			order by count(*) desc";
	elseif($gametype == "Turbos")
		$qry = "select m.playerid, m.playername, count(*) games,
			sum(rs.deathtype = 'Mod Killed') modkill, 
			sum(rs.faction=1) vgames, sum(rs.faction between 11 and 19) wgames,
			sum(rs.faction=1 and rs.deathtype='Survived') vsurv,
			sum(rs.faction between 11 and 19 and rs.deathtype='Survived') wsurv,
			sum(rs.faction=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vnk,
			sum(rs.faction=1 and roletype=1 and rs.deathtype in ('Night Killed','Day Killed'))/sum(rs.faction=1) vanillank,
			sum(rs.faction=1 and rs.deathtype in ('Lynched','Eaten')) vl,
			avg(if(rs.faction = 1, rs.deathday, NULL)/g.gamelength) avglifev,
			avg(if(rs.faction between 11 and 19, rs.deathday, NULL)/g.gamelength) avglifew,
			avg(if(rs.deathtype in ('Night Killed','Day Killed') and rs.faction = 1, rs.deathday, NULL)/g.gamelength) avgnkv
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

<h2>Death Stats - <?= $gametype?></h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Death Stats"/>
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
<input type="hidden" id="colvillage" name="colvillage" value=4 />
<input type="hidden" id="colwolf" name="colwolf" value=8 />
Minimum: <input type="text" id="min" name="min" value=20 /><br/>
Maximum: <input type="text" id="max" name="max"/><br/>
Villager Minimum: <input type="text" id="villagermin" name="villagermin"/><br/>
Villager Maximum: <input type="text" id="villagermax" name="villagermax"/><br/>
Wolf Minimum: <input type="text" id="wolfmin" name="wolfmin"/><br/>
Wolf Maximum: <input type="text" id="wolfmax" name="wolfmax"/><br/>
<table class="data" border="1" class="display">
	<thead>
	<tr>
	<th rowspan="2"></th>
	<th rowspan="2">Name</th>
	<th colspan="2">Total</th>
	<th colspan="4">Villager</th>	
	<th colspan="3">Wolf</th>
	</tr>
	<tr>
	<th>Games</th>
	<th>Mod Kills</th>
	<th>Games</th>
	<th>NK %</th>
	<th>Vanilla NK %</th>
	<th>NK Day/Game Length</th>
	<th>Games</th>
	<th>Survival Rate</th>
	<th>Avg Death Day</th>
	</tr>
	</thead>
	<tbody>
	
<?php
	function division($a, $b) {
		if($b == 0) {
			return NULL; }
		else {
			return $a/$b; }}
	while($player = $result->fetch_assoc()) { 
		echo "<tr><td></td>
			<td><a href='index.php?report=Player&playerid=".$player['playerid']."'>".$player['playername']."</a></td>
			<td>".$player['games']."</td>
			<td>".$player['modkill']."</td>
			<td>".$player['vgames']."</td>
			<td>".number_format($player['vnk'],3)."</td>
			<td>".number_format($player['vanillank'],3)."</td>
			<td>".number_format($player['avglifev'],3)."</td>
			<td>".$player['wgames']."</td>
			<td>".number_format(division($player['wsurv'],$player['wgames']),3)."</td>
			<td>".number_format($player['avglifew'],3)."</td>
			</tr>";
	}
?>

	</tbody>
</table>

<b>Vanilla NK %:</b> NK'd as Vanillager/Vanillager games<br/>
