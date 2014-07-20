<h2>All Players</h2>
<b>Game Number Filtering</b><br/>
Minimum: <input type="text" id="min" name="min"/><br/>
Maximum: <input type="text" id="max" name="max"/><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=5 />
<input type="hidden" id="colwolf" name="colwolf" value=12 />
<input type="hidden" id="villagermin" name="villagermin"/>
<input type="hidden" id="villagermax" name="villagermax"/>
<input type="hidden" id="wolfmin" name="wolfmin"/>
<input type="hidden" id="wolfmax" name="wolfmax"/>
<table class="data" border="1">
<thead>
	<tr>
	<th></th>
	<th>Name</th>
	<th>Long Games</th>
	<th>Vanillas</th>
	<th>Turbos</th>
	<th>First Game</th>
	<th>Last Game</th>
	<th>Main Acct</th>
	</tr>
</thead>
<tbody>
	
<?php 
	$qry = "select p.mainplayerid playerid, (select playername from player p2 where p2.playerid=p.mainplayerid) playername, 
		count(distinct if(g.gametype in ('Turbo','Turbo Mishmash'),NULL,g.gameid)) games, sum(g.gametype in ('Vanilla','Slow Game')) vanillas, sum(g.gametype in ('Turbo','Turbo Mishmash')) turbos, 
		min(g.startdate) firstgame, max(g.startdate) lastgame, '-' as mainacct
		from player p join playerlist pl using (playerid) join game g using (gameid)
		group by p.mainplayerid
		union
		select p.mainplayerid playerid, p.playername, count(distinct if(g.gametype in ('Turbo','Turbo Mishmash'),NULL,g.gameid)) games, 
		sum(g.gametype in ('Vanilla','Slow Game')) vanillas, 
		sum(g.gametype in ('Turbo','Turbo Mishmash')) turbos, min(g.startdate) firstgame, max(g.startdate) lastgame, m.playername mainacct
		from player p
		join playerlist pl on p.playerid=pl.playerid
		join game g using (gameid)
		join player m on m.playerid=p.mainplayerid
		where p.playerid != p.mainplayerid
		group by p.playerid
		order by 3 desc";
	
	$result = $db->query($qry);
	
	while($player = $result->fetch_assoc()) 
	{
		echo "<tr>
			<td></td>
			<td><a href=index.php?report=Player&playerid=".$player['playerid'].">".$player['playername']."</a></td>
			<td>".$player['games']."</td>
			<td>".$player['vanillas']."</td>
			<td>".$player['turbos']."</td>
			<td>".$player['firstgame']."</td>	
			<td>".$player['lastgame']."</td>
			<td>".$player['mainacct']."</td>
			</tr>";
	}
?>
</tbody>
</table>
