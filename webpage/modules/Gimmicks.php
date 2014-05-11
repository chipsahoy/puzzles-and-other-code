<?php
	$qry = "select p.playerid, g.playername gimmick, p.playername main, count(*) n
		from player p join player g on g.mainplayerid=p.playerid
		join playerlist pl on pl.playerid=g.playerid
		where p.playerid != g.playerid group by g.playerid order by 2";
	$result = $db->query($qry);
?>

<h2>Gimmicks</h2>
<table class="data" border="1">
<thead>
	<tr>
	<th></th>
	<th>Gimmick</th>
	<th>Main Acct</th>
	<th>Games</th>
	</tr>
</thead>
<tbody>

<?php 
	while($player = $result->fetch_assoc())
	{ 
?>
    <tr>
        <td></td>
        <td><?= $player['gimmick'] ?></td>	
        <td><a href="index.php?report=Player&playerid=<?=$player['playerid']?>"><?= $player['main']?></a></td>
		<td><?= $player['n'] ?></td>	
	</tr>
<?php
} // while ?>
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

