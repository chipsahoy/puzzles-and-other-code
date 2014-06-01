<h2>Aggregate Stats</h2>

<?php
	function division($a, $b) {
		if($b == 0) {
			return NULL; }
		else {
			return $a/$b; }}

	$players = $db->query("SELECT count(distinct mainplayerid) FROM playerlist pl join player p using (playerid)")->fetch_row();
	echo "Players in Database: ".$players[0]."<br/>";

	$qry = "select count(*) n, sum(gametype in ('Vanilla','Slow Game')) vanillas, sum(gametype = 'Slow Game') slow,
		sum(gametype = 'Vanilla+') vplus, sum(gametype='Mish-Mash') mashes, sum(gametype='Turbo') turbos, sum(gametype='Turbo Mishmash') turbomash from game";
	$games = $db->query($qry)->fetch_assoc();
	$qry = "select sum(victory<>2) games, sum(victory=1) wins, sum(victory=0) losses from team t join game g using (gameid) where gametype in ('Vanilla','Slow Game') and faction=1;";
	$stats = $db->query($qry)->fetch_assoc();
	$qry = "select sum(victory<>2) games, sum(victory=1) wins, sum(victory=0) losses from team t join game g using (gameid) where gametype in ('Slow Game') and faction=1;";
	$stats2 = $db->query($qry)->fetch_assoc();
	if ($stats2['games']==0) {
		$stats2['games'] = 1;
	}
	
	echo "Games Played: ".$games['n']."<br/>
		Vanillas Played: ".$games['vanillas']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(division($stats['wins'],$stats['games']), 3)."</div>
		<div style='margin-left:25px;'> Slow Games: ".$games['slow'].
		"<div style='margin-left:25px;'> Village Win %: ".number_format(division($stats2['wins'],$stats2['games']), 3)."</div>
		</div>";
	
	$qry = "select numplayers, count(*) games, sum(t.faction=1) vwin, sum(t.faction=11) wwin
		from game g
		join team t using (gameid)
		where gametype in ('Vanilla','Slow Game') and t.victory=1
		group by numplayers having count(*) >= 10";
	$result = $db->query($qry);
	echo "<div style='margin-left:25px;'>";
	while($stats = $result->fetch_assoc()) { 
		echo $stats['numplayers']."'ers Played: ".$stats['games']."<br/>
			<div style='margin-left:25px;'> Village Win %: ".number_format(round($stats['vwin']/$stats['games'], 3), 3)."</div>";
		
	}
	echo "</div>";

	$qry = "select sum(victory<>2) games, sum(victory=1) wins, sum(victory=0) losses from team t join game g using (gameid) where gametype in ('Turbo') and faction=1;";
	$stats = $db->query($qry)->fetch_assoc();
	echo "Turbos Played: ".$games['turbos']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(division($stats['wins'],$stats['games']), 3)."</div>";
	$qry = "select numplayers, count(*) games, sum(t.faction=1) vwin, sum(t.faction=11) wwin
		from game g
		join team t using (gameid)
		where gametype in ('Turbo') and t.victory=1
		group by numplayers having count(*) >= 10";
	$result = $db->query($qry);
	echo "<div style='margin-left:25px;'>";
	while($stats = $result->fetch_assoc()) { 
		echo $stats['numplayers']."'ers Played: ".$stats['games']."<br/>
			<div style='margin-left:25px;'> Village Win %: ".number_format(round($stats['vwin']/$stats['games'], 3), 3)."</div>";
		
	}
	echo "</div>";

	$qry = "select count(distinct g.gameid) games, sum(faction=1 && victory=1) vwin, sum(faction between 11 and 19 && victory=1) wwin, 
		sum(faction between 20 and 29 && victory=1) nwin, sum(faction between 20 and 29) ngames
		from team t join game g using (gameid) where gametype in ('Vanilla+')";
	$stats = $db->query($qry)->fetch_assoc();
	$qry = "select sum(numplayers) p, count(*) n from game where gametype in ('Vanilla+')";
	$stats2 = $db->query($qry)->fetch_assoc();
	echo "Vanilla+'s Played: ".$games['vplus']."<br/>
		<div style='margin-left:25px;'> Players per game: ".number_format(division($stats2['p'],$stats2['n']), 1)."</div>
		<div style='margin-left:25px;'> Village Win %: ".number_format(division($stats['vwin'],$stats['games']), 3)."</div>
		<div style='margin-left:25px;'> Wolf Win %: ".number_format(division($stats['wwin'],$stats['games']), 3)."</div>
		<div style='margin-left:25px;'> Neutral Win %: ".number_format(division($stats['nwin'],$stats['ngames']), 3)."</div>";
	
	$qry = "select count(distinct g.gameid) games, sum(faction=1 && victory=1) vwin, sum(faction between 11 and 19 && victory=1) wwin, 
		sum(faction between 20 and 29 && victory=1) nwin, sum(faction between 20 and 29) ngames
		from team t join game g using (gameid) where gametype in ('Mish-Mash')";
	$stats = $db->query($qry)->fetch_assoc();
	$qry = "select sum(numplayers) p, count(*) n from game where gametype in ('Mish-Mash')";
	$stats2 = $db->query($qry)->fetch_assoc();
	echo "Mish-Mashes Played: ".$games['mashes']."<br/>
		<div style='margin-left:25px;'> Players per game: ".number_format(division($stats2['p'],$stats2['n']), 1)."</div>
		<div style='margin-left:25px;'> Village Win %: ".number_format(division($stats['vwin'],$stats['games']), 3)."</div>
		<div style='margin-left:25px;'> Wolf Win %: ".number_format(division($stats['wwin'],$stats['games']), 3)."</div>
		<div style='margin-left:25px;'> Neutral Win %: ".number_format(division($stats['nwin'],$stats['ngames']), 3)."</div>";
?>

