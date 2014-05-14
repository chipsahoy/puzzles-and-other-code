<h3>Active Games</h3>
<table class="data" border="1">
<thead>
	<tr>
	<th></th>
	<th>Game</th>
	<th>OP</th>
	<th>Replies</th>
	</tr>
</thead>
<tbody>
	
<?php
	$qry = "SELECT t.title, t.threadid, t.replies, p.playername, p.playerid, t.url FROM fennecfox.Thread t, player p 
		WHERE p.playerid = t.op AND t.lastposttime > DATE_SUB(NOW(), INTERVAL 1 DAY) AND t.icontext = 'Spade'";
	$result = $db->query($qry);
	
	while($g = $result->fetch_assoc())
	{
		echo "<tr>
			<td></td>
			<td><a href='".$g['url']."'>".$g['title']."</a></td>
			<td><a href='index.php?report=Player&playerid=".htmlentities($g['playerid'], ENT_QUOTES, 'UTF-8')."'>".$g['playername']."</a></td>
			<td>".$g['replies']."</td>
			</tr>";
	}

	#echo mysqli_fetch_row(mysqli_query($db, "SELECT count(distinct playerid) FROM playerlist"))[0];
	
?>
</tbody>
</table>

<h2>Miscellaneous</h2>
<h3>Games</h3>

<?php
$q = "SELECT 
                        count(*) as game_count, avg(gamelength) as avg_gamelength, 
                        (SELECT count(*) FROM game WHERE gametype in ('Vanilla','Slow Game')) as vanilla_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Vanilla') as village_win_vanilla_count,
                        (SELECT count(*) FROM fennecfox.Game WHERE gametype='Vanilla+') as vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as village_win_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as wolf_win_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as wolf_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'NEU' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as neutral_win_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'NEU' AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as neutral_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,2)) = 'SK' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as sk_win_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,2)) = 'SK' AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as sk_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'JES' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as jester_win_vanilla_plus_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'JES' AND t.threadid = g.threadid AND g.gametype = 'Vanilla+') as jester_vanilla_plus_count,
                        (SELECT count(*) FROM fennecfox.Game WHERE gametype='Slow Game') as slow_game_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Slow Game') as village_win_slow_game_count,
                        (SELECT count(*) FROM fennecfox.Game WHERE gametype='Mish-Mash') as mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as village_win_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as wolf_win_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as wolf_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'NEU' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as neutral_win_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'NEU' AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as neutral_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,2)) = 'SK' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as sk_win_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,2)) = 'SK' AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as sk_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'JES' AND t.victory = 1 AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as jester_win_mishmash_count,
						(SELECT count(*) FROM fennecfox.Game g, fennecfox.Team t, fennecfox.Affiliation a WHERE a.affiliationid = t.affiliationid AND UPPER(LEFT(a.affiliationname,3)) = 'JES' AND t.threadid = g.threadid AND g.gametype = 'Mish-Mash') as jester_mishmash_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 9) as 9er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 13) as 13er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team, fennecfox.Affiliation WHERE Team.affiliationid = Affiliation.affiliationid AND gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid AND UPPER(LEFT(Affiliation.affiliationname,3)) = 'VIL' AND Team.victory = 1 GROUP BY gamename) as gamesizes WHERE gamesize = 10) as village_win_13er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 15) as 15er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team, fennecfox.Affiliation WHERE Team.affiliationid = Affiliation.affiliationid AND gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid AND UPPER(LEFT(Affiliation.affiliationname,3)) = 'VIL' AND Team.victory = 1 GROUP BY gamename) as gamesizes WHERE gamesize = 13) as village_win_17er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 17) as 17er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 19) as 19er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 21) as 21er_count,
						(SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team, fennecfox.Affiliation WHERE Team.affiliationid = Affiliation.affiliationid AND gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid AND UPPER(LEFT(Affiliation.affiliationname,3)) = 'VIL' AND Team.victory = 1 GROUP BY gamename) as gamesizes WHERE gamesize = 16) as village_win_21er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 23) as 23er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 25) as 25er_count,
                        (SELECT count(*) FROM (SELECT count(*) as gamesize FROM fennecfox.Game, fennecfox.GameRole, fennecfox.Team WHERE gametype='Vanilla' AND Game.threadid = Team.threadid AND GameRole.teamid = Team.teamid GROUP BY gamename) as gamesizes WHERE gamesize = 27) as 27er_count,
                        (SELECT count(*) FROM fennecfox.GameRole) as player_count,
                        (SELECT count(*) FROM fennecfox.GameRole, fennecfox.Team, fennecfox.Game WHERE GameRole.teamid = Team.teamid AND Team.threadid = Game.threadid AND Game.gametype='Vanilla') as vanilla_player_count,
                        (SELECT count(*) FROM fennecfox.GameRole, fennecfox.Team, fennecfox.Game WHERE GameRole.teamid = Team.teamid AND Team.threadid = Game.threadid AND Game.gametype='Vanilla+') as vanilla_plus_player_count,
                        (SELECT count(*) FROM fennecfox.GameRole, fennecfox.Team, fennecfox.Game WHERE GameRole.teamid = Team.teamid AND Team.threadid = Game.threadid AND Game.gametype='Slow Game') as slow_game_player_count,
                        (SELECT count(*) FROM fennecfox.GameRole, fennecfox.Team, fennecfox.Game WHERE GameRole.teamid = Team.teamid AND Team.threadid = Game.threadid AND Game.gametype='Mish-Mash') as mishmash_player_count
						FROM game g;";
	$result = $db->query($q);
	$games = $result->fetch_assoc();
	echo "Games Played: ".$games['game_count']."<br/>
		Average Days per Game: ".$games['avg_gamelength']."<br/>
		Vanillas Played: ".$games['vanilla_count']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(round($games['village_win_vanilla_count']/$games['vanilla_count'], 3), 3)."</div>
		<div style='margin-left:25px;'>9'ers Played: ".$games['9er_count']."<br/>
		13'ers Played: ".$games['13er_count']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(round($games['village_win_13er_count']/$games['13er_count'], 3), 3)."</div>
		15'ers Played: ".$games['15er_count']."<br/>
		17'ers Played: ".$games['17er_count']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(round($games['village_win_17er_count']/$games['17er_count'], 3), 3)."</div>
		19'ers Played: ".$games['19er_count']."<br/>
		21'ers Played: ".$games['21er_count']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(round($games['village_win_21er_count']/$games['21er_count'], 3), 3)."</div>
		23'ers Played: ".$games['23er_count']."<br/>
		25'ers Played: ".$games['25er_count']."<br/>
		27'ers Played: ".$games['27er_count']."</div>
		Vanilla+'s Played: ".$games['vanilla_plus_count']."<br/>
		<div style='margin-left:25px;'> Village Win %: ".number_format(round($games['village_win_vanilla_plus_count']/$games['vanilla_plus_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> Wolf Win %: ".number_format(round($games['wolf_win_vanilla_plus_count']/$games['wolf_vanilla_plus_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> Neutral Win %: ".number_format(round($games['neutral_win_vanilla_plus_count']/$games['neutral_vanilla_plus_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> SK Win %: ".number_format(round($games['sk_win_vanilla_plus_count']/$games['sk_vanilla_plus_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> Jester Win %: ".number_format(round($games['jester_win_vanilla_plus_count']/$games['jester_vanilla_plus_count'], 3), 3)."</div>
		Mish-Mashes Played: ".$games['mishmash_count']."<br/>			
		<div style='margin-left:25px;'> Village Win %: ".number_format(round($games['village_win_mishmash_count']/$games['mishmash_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> Wolf Win %: ".number_format(round($games['wolf_win_mishmash_count']/$games['wolf_mishmash_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> Neutral Win %: ".number_format(round($games['neutral_win_mishmash_count']/$games['neutral_mishmash_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> SK Win %: ".number_format(round($games['sk_win_mishmash_count']/$games['sk_mishmash_count'], 3), 3)."</div>
		<div style='margin-left:25px;'> Jester Win %: ".number_format(round($games['jester_win_mishmash_count']/$games['jester_mishmash_count'], 3), 3)."</div>
		Total Players: ".$games['player_count']."<br/>
		Players per Game: ".number_format(round($games['player_count']/$games['game_count'], 3), 3)."<br/>
		<div style='margin-left:25px;'>Players per Vanilla: ".number_format(round($games['vanilla_player_count']/$games['vanilla_count'], 3), 3)."<br/>
		Players per Vanilla+: ".number_format(round($games['vanilla_plus_player_count']/$games['vanilla_plus_count'], 3), 3)."<br/>
		Players per Mish-Mash: ".number_format(round($games['mishmash_player_count']/$games['mishmash_count'], 3), 3)."<br/>
		Players per Slow Game: ".number_format(round($games['slow_game_player_count']/$games['slow_game_count'], 3), 3)."</div>";
	
	$players = $db->query("SELECT count(distinct playerid) FROM playerlist")->fetch_row();
	$players = $players[0];
	$villagerwinsplayer = $db->query("SELECT count(*) FROM player")->fetch_row();
	$villagerwinsplayer = $villagerwinsplayer[0];
	$villagerlossesplayer = $db->query("SELECT count(*) FROM player")->fetch_row();
	$villagerlossesplayer = $villagerlossesplayer[0];
	$wolfwinsplayer = $db->query("SELECT count(*) FROM player")->fetch_row();
	$wolfwinsplayer = $wolfwinsplayer[0];
	$wolflossesplayer = $db->query("SELECT count(*) FROM player")->fetch_row();
	$wolflossesplayer = $wolflossesplayer[0];
	$neutralwinsplayer = $db->query("SELECT count(*) FROM player")->fetch_row();
	$neutralwinsplayer = $neutralwinsplayer[0];
	$neutrallossesplayer = $db->query("SELECT count(*) FROM player")->fetch_row();
	$neutrallossesplayer = $neutrallossesplayer[0];

	#$villagerwinsplayer = mysql_result(mysql_query("SELECT count(*) FROM Team, Affiliation, GameRole WHERE Team.affiliationid = Affiliation.affiliationid AND victory = 1 AND affiliationname LIKE 'Village' AND Team.teamid = GameRole.teamid"), 0);
	#$villagerlossesplayer = mysql_result(mysql_query("SELECT count(*) FROM Team, Affiliation, GameRole WHERE Team.affiliationid = Affiliation.affiliationid AND victory = 0 AND affiliationname LIKE 'Village' AND Team.teamid = GameRole.teamid"), 0);
	#$wolfwinsplayer = mysql_result(mysql_query("SELECT count(*) FROM Team, Affiliation, GameRole WHERE Team.affiliationid = Affiliation.affiliationid AND victory = 1 AND affiliationname LIKE 'Wolves%' AND Team.teamid = GameRole.teamid"), 0);
	#$wolflossesplayer = mysql_result(mysql_query("SELECT count(*) FROM Team, Affiliation, GameRole WHERE Team.affiliationid = Affiliation.affiliationid AND victory = 0 AND affiliationname LIKE 'Wolves%' AND Team.teamid = GameRole.teamid"), 0);
	#$neutralwinsplayer = mysql_result(mysql_query("SELECT count(*) FROM Team, Affiliation, GameRole WHERE Team.affiliationid = Affiliation.affiliationid AND victory = 1 AND affiliationname LIKE 'Neutral%' AND Team.teamid = GameRole.teamid"), 0);
	#$neutrallossesplayer = mysql_result(mysql_query("SELECT count(*) FROM Team, Affiliation, GameRole WHERE Team.affiliationid = Affiliation.affiliationid AND victory = 0 AND affiliationname LIKE 'Neutral%' AND Team.teamid = GameRole.teamid"), 0);

	echo "<h3>Players</h3>
			Players in Database: ".$players."<br/>
			Total Villager Wins: ".$villagerwinsplayer."<br/>
			Total Villager Losses: ".$villagerlossesplayer."<br/>
			Total Villager Win Percentage: ".number_format(round($villagerwinsplayer/($villagerlossesplayer+$villagerwinsplayer),3),3)."<br/>
			Total Wolf Wins: ".$wolfwinsplayer."<br/>
			Total Wolf Losses: ".$wolflossesplayer."<br/>
			Total Wolf Win Percentage: ".number_format(round($wolfwinsplayer/($wolflossesplayer+$wolfwinsplayer),3),3)."<br/>
			Total Neutral Wins: ".$neutralwinsplayer."<br/>
			Total Neutral Losses: ".$neutrallossesplayer."<br/>
			Total Neutral Win Percentage: ".number_format(round($neutralwinsplayer/($neutrallossesplayer+$neutralwinsplayer),3),3)."<br/>";
?>

<input type="hidden" id="max" name="max"/>
<input type="hidden" id="min" name="min"/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=5 />
<input type="hidden" id="colwolf" name="colwolf" value=12 />
<input type="hidden" id="villagermin" name="villagermin"/>
<input type="hidden" id="villagermax" name="villagermax"/>
<input type="hidden" id="wolfmin" name="wolfmin"/>
<input type="hidden" id="wolfmax" name="wolfmax"/>
