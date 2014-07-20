<h2>All Long Games</h2>
<table class="data" border="1">
<thead>
	<tr>
	<th></th>
	<th>Start Date</th>
	<th>Name</th>
	<th>Moderator</th>
	<th>Type</th>
	<th># of Players</th>
	<th>Length</th>
	<th>Victors</th>
	<th>Posts</th></tr>
</thead>
<tbody>

<?php	
	$qry = "select gameid, url, gamename, startdate, gametype, gamelength, g.numplayers as players, 
	(select playername from moderator m join player p on p.playerid=m.modid where m.gameid=g.gameid and m.isprimary) as modname,
	(select modid from moderator m join player p on p.playerid=m.modid where m.gameid=g.gameid and m.isprimary) as modid,
	left(victor,20) victor,
	(select replies+1 posts from fennecfox.Thread t where t.threadid=g.gameid) posts
	from game g	
	join victor v using (gameid)
	where g.gametype not in ('Turbo','Turbo Mishmash')
	order by g.startdate desc";
	
	$result = $db->query($qry);
	
	while($game = $result->fetch_assoc())
	{
		echo("<tr><td></td>
		<td>".$game['startdate']."</td>
		<td><a href='index.php?report=Game&gameid=".$game['gameid']."'>".$game['gamename']."</a> <a href=".$game['url'].">(link)</a></td>
		<td><a href=\"index.php?report=Player&playerid=".$game['modid']."\">".$game['modname']."</a></td>
		<td>".$game['gametype']."</td>
		<td>".$game['players']."</td>
		<td>".$game['gamelength']."</td>
		<td>".$game['victor']."</td>
		<td>".$game['posts']."</td>
		</tr>");		
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
