<?php
	if(!array_key_exists('gametype', $_GET) || $_GET['gametype'] == "") $gametype = "All"; else $gametype = $_GET['gametype'];
	if($gametype == "All")
		$qry = "select * from derivedrecords dr join player p using (playerid) where gametype='Total' order by games desc";
	elseif($gametype == "Vanilla/Slow Games")
		$qry = "select * from derivedrecords dr join player p using (playerid) where gametype='vanilla' order by games desc";
	elseif($gametype == "Vanilla+/Mish-Mashes")
		$qry = "select * from derivedrecords dr join player p using (playerid) where gametype='mash' order by games desc";
	elseif($gametype == "Turbos")
		$qry = "select * from derivedrecords dr join player p using (playerid) where gametype='turbo' order by games desc";
	$result = $db->query($qry);
?>
<h2>Player Records - <?= $gametype?></h2>
<form action="index.php" method="get">
			<input type="hidden" name="report" value="Player Records"/>
			Gametype: <select name="gametype" onchange="this.form.submit();">
				<option></option>
				<option>All</option>
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
	<th colspan="4">Neutral</th>
	</tr>
	<tr>
	<th>Games</th>	
	<th>Wins</th>
	<th>Losses</th>
	<th>Ties</th>
	<th>%</th>
	<th>Games</th>
	<th>Wins</th>
	<th>Losses</th>
	<th>%</th>
	<th>Games</th>
	<th>Wins</th>
	<th>Losses</th>
	<th>%</th>
	<th>Games</th>
	<th>Wins</th>
	<th>Losses</th>
	<th>%</th>
	</tr>
	</thead>
	<tbody>
<?php 
	while($player = $result->fetch_assoc()) { 
	#$elo = mysql_fetch_assoc(mysql_query("SELECT newelo FROM ELO e2, Game g2 WHERE e2.threadid = g2.threadid AND g2.startdate=(SELECT MAX(startdate) FROM ELO e, Game g WHERE g.threadid = e.threadid AND e.posterid = ".$player['posterid'].") AND e2.posterid= ".$player['posterid'])); 
	$elo = '';
?>
	
	<tr>
	<td></td>
	<td><a href="index.php?report=Player&playerid=<?= htmlentities($player['playerid'], ENT_QUOTES, 'UTF-8') ?>
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
	<td><?= $player['ngames'] ?></td>
	<td><?= $player['nwin'] ?></td>
	<td><?= $player['nloss'] ?></td>
	<td><?php if($player['nwin']+$player['nloss'] > 0) echo number_format(round($player['nwin']/($player['nwin']+$player['nloss']), 3), 3); 
		else echo "0"; ?></td>
		
    </tr>
<?php } // while ?>
	</tbody>
</table>

