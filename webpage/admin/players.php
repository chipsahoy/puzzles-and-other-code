<head><meta charset="UTF-8"></head>
<h3><ul>Werewolf Database Control Panel</ul></h3><br><br>

<?php
	include('../login.php');
	$db->set_charset("utf8");
?>

<form method="post" action="addplayer.php">
	All players (games played):<br>
	<select name='player1'>
<?php
	$qry = "select concat(p.playername, ' (', count(pl.playerid), ')', if(p.playerid=p.mainplayerid,'',concat(' Gimmick of ',p2.playername))) playername, 
		p.playerid from player p join player p2 on p2.playerid=p.mainplayerid
		join playerlist pl on p.playerid=pl.playerid group by p.playerid order by p.playername";
	$result = $db->query($qry);
	while($player = $result->fetch_assoc()) {
		echo "<option value = " . $player['playerid'] . ">" . $player['playername'] . "</option>\n";
	}
?>	
	</select><br>
	Set player as gimmick of:<br>
	<select name='player2'>
<?php
	$qry = "select '--Not a gimmick--' as playername, 0 as playerid union select playername, playerid from player 
		where playerid=mainplayerid and playerid in (select distinct playerid from playerlist) order by playerid<>0, playername";
	$result = $db->query($qry);
	while($player = $result->fetch_assoc()) {
		echo "<option value = " . $player['playerid'] . ">" . $player['playername'] . "</option>\n";
	}
?>	
	</select><input type="submit" name="setgimmick" value="Set as gimmick"><br>
	
	<br>
	<input type="text" name="playername" size="30" value="">
	<input type="submit" name="addplayer" value="Add new player"><br><br>
</form>
	

<br><br>
<a href=".">Return to Control Panel</a>

