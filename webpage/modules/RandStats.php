<?php
	$q = mysql_query("SELECT 
			p.posterid, 
			p.postername, 
			(select count(*) from GameRole gc, Player p2 where gc.roleid=p2.roleid AND p2.posterid=p.posterid) as game_count,
			(select count(*) FROM Game g, Team t, GameRole pt, Player p2 WHERE (g.gametype = 'Vanilla' or g.gametype= 'Slow Game') AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as vanilla_games,
			(select count(*) FROM Game g, Team t, GameRole pt, Player p2 WHERE g.gametype = 'Vanilla+' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as vanilla_plus_games,
			(select count(*) FROM Game g, Team t, GameRole pt, Player p2 WHERE g.gametype = 'Mish-Mash' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as mishmash_games,
			(select count(*) FROM Game g, Team t, GameRole pt, Player p2, Affiliation a WHERE a.affiliationid = a.affiliationid AND g.gametype = 'Mish-Mash' AND g.threadid = t.threadid AND pt.teamid = t.teamid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as mishmash_villager_games,
			(select count(*) From GameRole pt, Player p2, Team t, Affiliation a WHERE a.affiliationid = a.affiliationid AND pt.teamid = t.teamid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as village_games,
			(select count(*) From GameRole pt, Player p2, Team t, Affiliation a WHERE a.affiliationid = a.affiliationid AND pt.teamid = t.teamid AND UPPER(LEFT(a.affiliationname,3)) = 'WOL' AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as wolf_games,
			(select count(*) FROM GameRole pt, Player p2, Team t, Game g, Affiliation a, Role r WHERE a.affiliationid = a.affiliationid AND pt.roletypeid = r.roleid AND pt.teamid = t.teamid AND t.threadid = g.threadid AND (g.gametype = 'Vanilla' OR g.gametype = 'Slow Game') AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND r.rolename LIKE '%Seer%' AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as vanilla_seer_games,
			(select count(*) FROM GameRole pt, Player p2, Team t, Role r, Affiliation a WHERE pt.teamid = t.teamid AND pt.roletypeid = r.roleid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND r.rolename LIKE '%Seer%' AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as seer_games,
			(select count(*) FROM GameRole pt, Player p2, Team t, Game g, Role r, Affiliation a WHERE a.affiliationid = a.affiliationid AND pt.roletypeid = r.roleid AND pt.teamid = t.teamid AND UPPER(LEFT(a.affiliationname,3)) = 'VIL' AND (r.rolename LIKE '%Seer%' OR r.rolename LIKE '%Angel%' OR r.rolename LIKE '%Vig%') AND g.threadid = t.threadid AND g.gametype = 'Mish-Mash' AND p2.posterid = p.posterid AND pt.roleid = p2.roleid) as pr_in_mishmashes
			FROM
			Poster p
			HAVING 
			game_count > 0;");
?>
<h2>Rand Stats</h2>
<b>Game Number Filtering</b><br/>
<input type="hidden" id="colgame" name="colgame" value=2 />
<input type="hidden" id="colvillage" name="colvillage" value=5 />
<input type="hidden" id="colwolf" name="colwolf" value=12 />
<input type="hidden" id="villagermin" name="villagermin"/>
<input type="hidden" id="villagermax" name="villagermax"/>
<input type="hidden" id="wolfmin" name="wolfmin"/>
<input type="hidden" id="wolfmax" name="wolfmax"/>
Minimum: <input type="text" id="min" name="min" value=20 /><br/>
Maximum: <input type="text" id="max" name="max"/>
<table class="data" border="1">
<thead>
	<tr>
	<th rowspan="2"></th>
	<th rowspan="2">Name</th>
	<th rowspan="2">Games Played</th>
	<th colspan="3">Type of Games Played %</th>
	<th colspan="5">Rand %</th>
	</tr>
	<tr>
	<th>Vanilla</th>
	<th>Vanilla+</th>
	<th>Mishmash</th>
	<th>Village</th>
	<th>Wolf</th>
	<th>Seer</th>
	<th>Vanilla Seer</th>
	<th>Mish-Mash PR</th>
	</tr>
</thead>
<tbody>
<?php while($player = mysql_fetch_assoc($q)) { ?>
    <tr>
        <td></td>
        <td><a href="index.php?report=Player&Player=<?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?>"><?= htmlentities($player['postername'], ENT_QUOTES, 'UTF-8') ?></a></td>
        <td><?= $player['game_count'] ?></td>
        <td><?= number_format(round($player['vanilla_games']/$player['game_count'], 3), 3) ?></td>
		<td><?= number_format(round($player['vanilla_plus_games']/$player['game_count'], 3), 3) ?></td>
        <td><?= number_format(round($player['mishmash_games']/$player['game_count'], 3), 3) ?></td>
		<td><?= number_format(round($player['village_games']/$player['game_count'], 3), 3) ?></td>
        <td><?= number_format(round($player['wolf_games']/$player['game_count'], 3), 3) ?></td>
		<td><?= number_format(round($player['seer_games']/$player['game_count'], 3), 3) ?></td>
		<td><?php if($player['vanilla_games'] > 0) echo number_format(round($player['vanilla_seer_games']/$player['vanilla_games'], 3), 3); else echo 0; ?></td>
		<td><?php if($player['mishmash_games'] > 0) echo number_format(round($player['pr_in_mishmashes']/$player['mishmash_villager_games'], 3), 3); else echo 0;?></td>
    </tr>
<?php } // while ?>
</tbody>
</table>
<b>(Vanilla) Seer %:</b> Games played as Seer/Total (Vanilla) Games<br/>
<b>Mish-Mash PR %(only includes Seers, Angels and Vigilantes):</b> Mish-Mash PR count as villager/Mish-Mashes played as villager.
