<?php
	# sanitize gets
	$gameid = $_GET['gameid'];
	
	$qry = "select gameid, gamename, gametype, startdate, url
		from game g
		where g.gameid = " .$gameid;
	$result = $db->query($qry)->fetch_assoc();
	
	echo "<h2>".$result['gamename']."</h2><h3>".$result['gametype']." <a href=".htmlentities($result['url'], ENT_QUOTES, 'UTF-8').">(link)</a></h3>";

	$qry = "select modid, playername modname, mainplayerid from moderator m join player p on p.playerid=m.modid where m.gameid=".$gameid." order by isprimary desc";
	$result2 = $db->query($qry);
	
	echo "<h3>Moderated by ";
	while($mod = $result2->fetch_assoc())
	{
		echo "<a href=\"index.php?report=Player&playerid=".$mod['mainplayerid']."\">".$mod['modname']."</a> ";
	}
	
	echo "</h3>";

	echo "<b>".$result['startdate']."</b><br/>";
	
	$victor = $db->query("select victor from victor where gameid=".$_GET['gameid'])->fetch_assoc();
	if ($victor['victor'] == 'Tie') echo "Tie Game<br/>";
		else echo $victor['victor']." Win<br/>";
	
	$qry = "select p.mainplayerid playerid, playername, if(r.rolename='Vanilla','',r.rolename) rolename, rs.deathtype, deathday, factionname,
			(select concat(group_concat(p2.playername order by pl2.ordinal), ' (', group_concat(pl2.dayin order by pl2.ordinal), ')') x from playerlist pl2 
			join player p2 on p2.playerid=pl2.playerid where pl2.gameid=g.gameid and pl2.slot=pl.slot and pl2.ordinal > 1 group by pl2.slot) subs,
			(select pl3.playerid from playerlist pl3 where pl3.gameid=g.gameid and pl3.slot=pl.slot and pl3.ordinal = 2) subid,
			(select group_concat(concat(ability,' n', night) order by night separator ', ') from actions a where a.gameid=g.gameid and a.target=rs.slot) targeted
			from game g
			join roleset rs using (gameid)
			join playerlist pl using (gameid, slot)
			join player p on p.playerid=pl.playerid
			join team t using (gameid, faction)
			join roles r on r.roleid = rs.roletype
			join faction f on f.factionid=t.faction
			where pl.ordinal=1 and g.gameid = ".$gameid;
	
	$result = $db->query($qry);
	$array = array();
	
	while($resultarray = $result->fetch_assoc())
	{
		$array[] = $resultarray;
	}
?>

<table class="data" border="1">
	<thead><tr>
	<th></th>
	<th>Player</th>
	<th>Subs (day in)</th>
	<th>Affiliation</th>
	<th>Role</th>
	<th>Method of Death</th>
	<th>Survived Until</th>
	<th>Targeted</th>
	</tr></thead>
	<tbody>
	<?php
		foreach($array as $item)
		{
			echo("<tr><td></td>
			<td><a href=\"index.php?report=Player&playerid=".$item['playerid']."\">".$item['playername']."</a></td>
			<td><a href=\"index.php?report=Player&playerid=".$item['subid']."\">".$item['subs']."</a></td>
			<td>".$item['factionname']."</td>
			<td>".$item['rolename']."</td>
			<td>".$item['deathtype']."</td>
			<td>".$item['deathday']."</td>
			<td>".$item['targeted']."</td>
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
