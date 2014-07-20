<?php
	$qry = "select p.mainplayerid playerid, (select playername from player p2 where p2.playerid=p.mainplayerid) playername, 
		sum(gametype<>'Turbo') n, sum(gametype in ('Vanilla','Slow Game')) v, sum(gametype='Vanilla+') vp, sum(gametype='Mish-Mash') mash, sum(gametype='Turbo') turbo,
		sum(isprimary and gametype<>'Turbo') primarymod, min(g.startdate) firstgame, max(g.startdate) lastgame
		from moderator m join game g using (gameid)
		join player p on p.playerid=m.modid
		group by p.mainplayerid order by 3 desc";
	$result = $db->query($qry);
?>

<h2>Moderators</h2>
<table class="data" border=1>
	<thead>
	<tr>
	<th rowspan="2"></th>
	<th rowspan="2">Name</th>
	<th colspan="5">Games Moderated</th>
	<th rowspan="2">First Game Moderated</th>
	<th rowspan="2">Last Game Moderated</th>
	</tr>
	<tr>
	<th>Long Games</th>
	<th>Lead Mod</th>
	<th>Vanilla</th>
	<th>V+/Mish-Mash</th>
	<th>Turbo</th>
	</tr>
	</thead>
	<tbody>
<?php 
	while($player = $result->fetch_assoc())
	{ 
?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&playerid=<?=$player['playerid']?>"><?=$player['playername']?></a></td>
        <td><?= $player['n'] ?></td>	
		<td><?= $player['primarymod'] ?></td>	
		<td><?= $player['v'] ?></td>	
		<td><?= $player['vp']+$player['mash'] ?></td>	
		<td><?= $player['turbo'] ?></td>	
		<td><?= $player['firstgame'] ?></td>	
		<td><?= $player['lastgame'] ?></td>
    </tr>
<?php } ?>
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
