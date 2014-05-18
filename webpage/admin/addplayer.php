<h3><ul>Werewolf Database Control Panel - POG</ul></h3><br><br>

<?php
	include('../login.php');
	$db->set_charset("utf8");

	$playername = $_POST['playername'];
	
	if ($_POST['addplayer']) {
/* 		$qry = "select * from player where playername = '" . $playername . "'";
		$result = $db->query($qry);
		if ($result->num_rows > 0) {
			echo "Player '".$playername."' already exists.";
		} elseif ($playername == '' or strlen($playername) < 3) { 
			echo "Player name must be at least 3 characters in length.";
		} else {
			$db->query("insert into player (playername) values ('".$playername."')");
			$db->query("update player set mainplayerid=playerid where playername = '".$playername."'");
			echo "Role '".$playername."' added.";
		}
*/	
		echo "This function is disabled for POG.";
	}
	if ($_POST['setgimmick']) {
		$player1 = $_POST['player1'];
		$player2 = $_POST['player2'];
		$result = $db->query("select p.playerid, p.playername, count(p2.playerid) gimmicks from player p left join player p2 on p2.mainplayerid=p.playerid and p2.playerid<>p.playerid
			where p.playerid = " . $player1 . " group by p.playerid")->fetch_assoc();
		if ($result['playerid'] == $result['mainplayerid'] and $player2 == 0) {
			echo $result['playername'] . " is already not a gimmick.";
		} elseif ($result['gimmicks'] > 0 and $player2 != 0) {
			echo $result['playername'] . " already has gimmicks associated with this account. Dissociate those gimmicks first.";
		} elseif ($player2 == 0) {
			$db->query("update player set mainplayerid=playerid where playerid=" . $player1);
			echo $result['playername'] . " is not a gimmick.";
		} else {
			$db->query("update player set mainplayerid=" . $player2 . " where playerid=" . $player1);
			echo $result['playername'] . " set as gimmick.";
		}
	} 

	
?>

<br><br>
<a href="players.php">Return to Players</a><br>
<a href="controlpanel.html">Return to Control Panel</a>