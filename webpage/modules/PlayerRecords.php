<?php
	# if you sub out, you don't get credit
	if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "Long Games"; else $gametype = $_GET['gametype'];
	if($gametype == "Long Games")
		$qry = "select m.playerid, m.playername, 'Total' as gametype, count(*) games,
				sum(t.victory=1) wins, sum(t.victory=0) losses, sum(t.victory=2) ties,
				sum(t.faction = 1) vgames, sum(t.victory=1 && t.faction = 1) vwin, sum(t.victory=0 && t.faction = 1) vloss,
				sum(t.faction between 10 and 19) wgames, sum(t.victory=1 && (t.faction between 10 and 19)) wwin, sum(t.victory=0 && (t.faction between 10 and 19)) wloss,
				sum(t.faction >= 20) ngames, sum(t.victory=1 && t.faction >=20) nwin, sum(t.victory=0 && t.faction >=20) nloss
				from playerlist pl
				join roleset rs using (gameid, slot)
				join team t using (gameid, faction)
				join game g using (gameid)
				join player p on p.playerid=pl.playerid
				join player m on m.playerid=p.mainplayerid
				where g.gametype not in ('Turbo','Turbo Mishmash') and pl.dayout is null
				group by p.mainplayerid 
				order by count(*) desc";
	elseif($gametype == "Vanilla/Slow Games")
		$qry = "select m.playerid, m.playername, 'Total' as gametype, count(*) games,
				sum(t.victory=1) wins, sum(t.victory=0) losses, sum(t.victory=2) ties,
				sum(t.faction = 1) vgames, sum(t.victory=1 && t.faction = 1) vwin, sum(t.victory=0 && t.faction = 1) vloss,
				sum(t.faction between 10 and 19) wgames, sum(t.victory=1 && (t.faction between 10 and 19)) wwin, sum(t.victory=0 && (t.faction between 10 and 19)) wloss
				from playerlist pl
				join roleset rs using (gameid, slot)
				join team t using (gameid, faction)
				join game g using (gameid)
				join player p on p.playerid=pl.playerid
				join player m on m.playerid=p.mainplayerid
				where g.gametype in ('Vanilla','Slow Game') and pl.dayout is null
				group by p.mainplayerid 
				order by count(*) desc";
	elseif($gametype == "Vanilla+/Mish-Mashes")
		$qry = "select m.playerid, m.playername, 'Total' as gametype, count(*) games,
				sum(t.victory=1) wins, sum(t.victory=0) losses, sum(t.victory=2) ties,
				sum(t.faction = 1) vgames, sum(t.victory=1 && t.faction = 1) vwin, sum(t.victory=0 && t.faction = 1) vloss,
				sum(t.faction between 10 and 19) wgames, sum(t.victory=1 && (t.faction between 10 and 19)) wwin, sum(t.victory=0 && (t.faction between 10 and 19)) wloss,
				sum(t.faction >= 20) ngames, sum(t.victory=1 && t.faction >=20) nwin, sum(t.victory=0 && t.faction >=20) nloss
				from playerlist pl
				join roleset rs using (gameid, slot)
				join team t using (gameid, faction)
				join game g using (gameid)
				join player p on p.playerid=pl.playerid
				join player m on m.playerid=p.mainplayerid
				where g.gametype in ('Vanilla+','Mish-Mash') and pl.dayout is null
				group by p.mainplayerid 
				order by count(*) desc";
	elseif($gametype == "Turbos")
		$qry = "select m.playerid, m.playername, 'Total' as gametype, count(*) games,
				sum(t.victory=1) wins, sum(t.victory=0) losses, sum(t.victory=2) ties,
				sum(t.faction = 1) vgames, sum(t.victory=1 && t.faction = 1) vwin, sum(t.victory=0 && t.faction = 1) vloss,
				sum(t.faction between 10 and 19) wgames, sum(t.victory=1 && (t.faction between 10 and 19)) wwin, sum(t.victory=0 && (t.faction between 10 and 19)) wloss
				from playerlist pl
				join roleset rs using (gameid, slot)
				join team t using (gameid, faction)
				join game g using (gameid)
				join player p on p.playerid=pl.playerid
				join player m on m.playerid=p.mainplayerid
				where g.gametype = 'Turbo' and pl.dayout is null
				group by p.mainplayerid 
				order by count(*) desc";
	$result = $db->query($qry);
?>
<h2>Player Records - <?= $gametype?></h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Player Records"/>
			Gametype: <select name="gametype" onchange="this.form.submit();">
				<option></option>
				<option>Long Games</option>
				<option>Vanilla/Slow Games</option>
				<option>Vanilla+/Mish-Mashes</option>
				<option>Turbos</option>
			</select>
</form>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=3 />
<input type="hidden" id="colvillage" name="colvillage" value=8 />
<input type="hidden" id="colwolf" name="colwolf" value=12 />
Minimum: <input type="text" id="min" name="min" value=0 /><br/>
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
	<th rowspan="2">ELO Ranking</th>
	<th colspan="5">Total</th>
	<th colspan="4">Villager</th>
	<th colspan="4">Wolf</th>
	<?php if (in_array($gametype, array("Vanilla+/Mish-Mashes","All"))) { ?>
	<th colspan="4">Neutral</th><?php } # endif?>
	</tr><tr>
	<th>Games</th><th>Wins</th><th>Losses</th><th>Ties</th><th>%</th>
	<th>Games</th><th>Wins</th><th>Losses</th><th>%</th>
	<th>Games</th><th>Wins</th><th>Losses</th><th>%</th>
	<?php if (in_array($gametype, array("Vanilla+/Mish-Mashes","All"))) { ?>
	<th>Games</th><th>Wins</th><th>Losses</th><th>%</th><?php } # endif?>
	</tr></thead><tbody>
<?php 
	while($player = $result->fetch_assoc()) { 
	#$elo = mysql_fetch_assoc(mysql_query("SELECT newelo FROM ELO e2, Game g2 WHERE e2.threadid = g2.threadid AND g2.startdate=(SELECT MAX(startdate) FROM ELO e, Game g WHERE g.threadid = e.threadid AND e.posterid = ".$player['posterid'].") AND e2.posterid= ".$player['posterid'])); 
	$elo = '';
?>
	
	<tr>
	<td></td>
	<td><a href="index.php?report=Player&playerid=<?= $player['playerid'] ?>
		"><?= htmlentities($player['playername'], ENT_QUOTES, 'UTF-8') ?></a></td>
	<td><?= $elo ?></td>
	<td><?= $player['games'] ?></td>
	<td><?= $player['wins'] ?></td>
	<td><?= $player['losses'] ?></td>
	<td><?= $player['ties'] ?></td>
	<td><?php if($player['wins']+$player['losses'] > 0) echo number_format(round($player['wins']/($player['wins']+$player['losses']), 3), 3); 
		else echo 0; ?></td>
	<td><?= $player['vgames'] ?></td>
	<td><?= $player['vwin'] ?></td>
	<td><?= $player['vloss'] ?></td>
	<td><?php if($player['vwin']+$player['vloss'] > 0) echo number_format(round($player['vwin']/($player['vwin']+$player['vloss']), 3), 3); 
		else echo "0"; ?></td>
	<td><?= $player['wgames'] ?></td>
	<td><?= $player['wwin'] ?></td>
	<td><?= $player['wloss'] ?></td>
	<td><?php if($player['wwin']+$player['wloss'] > 0) echo number_format(round($player['wwin']/($player['wwin']+$player['wloss']), 3), 3); 
		else echo "0"; ?></td>
	<?php if (in_array($gametype, array("Vanilla+/Mish-Mashes","All"))) { ?>
	<td><?= $player['ngames'] ?></td>
	<td><?= $player['nwin'] ?></td>
	<td><?= $player['nloss'] ?></td>
	<td><?php if($player['nwin']+$player['nloss'] > 0) echo number_format(round($player['nwin']/($player['nwin']+$player['nloss']), 3), 3); 
		else echo "0"; ?></td>
	<?php } # endif?>
    </tr>
<?php } // while ?>
	</tbody>
</table>

