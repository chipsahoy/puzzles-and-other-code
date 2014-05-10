
--####################################
-- derived records
truncate derivedrecords;

insert into derivedrecords
select pl.playerid, if(g.gametype in ('Vanilla','Slow Game'), 'vanilla', if(g.gametype='Turbo', 'turbo', 'mash')) gametype, count(*) games,
sum(t.victory=1) wins, sum(t.victory=0) losses, sum(t.victory=2) ties,
sum(t.faction = 1) vgames, sum(t.victory=1 && t.faction = 1) vwins, sum(t.victory=0 && t.faction = 1) vloss,
sum(t.faction between 10 and 19) wgames, sum(t.victory=1 && (t.faction between 10 and 19)) wwins, sum(t.victory=0 && (t.faction between 10 and 19)) wloss,
sum(t.faction >= 20) ngames, sum(t.victory=1 && t.faction >=20) nwins, sum(t.victory=0 && t.faction >=20) nloss
from playerlist pl
join roleset rs using (gameid, slot)
join team t using (gameid, faction)
join game g using (gameid)
group by pl.playerid, if(g.gametype in ('Vanilla','Slow Game'), 'vanilla', if(g.gametype='Turbo', 'turbo', 'mash'));


insert into derivedrecords
select pl.playerid, 'Total' gametype, count(*) games,
sum(t.victory=1) wins, sum(t.victory=0) losses, sum(t.victory=2) ties,
sum(t.faction = 1) vgames, sum(t.victory=1 && t.faction = 1) vwins, sum(t.victory=0 && t.faction = 1) vloss,
sum(t.faction between 10 and 19) wgames, sum(t.victory=1 && (t.faction between 10 and 19)) wwins, sum(t.victory=0 && (t.faction between 10 and 19)) wloss,
sum(t.faction >= 20) ngames, sum(t.victory=1 && t.faction >=20) nwins, sum(t.victory=0 && t.faction >=20) nloss
from playerlist pl
join roleset rs using (gameid, slot)
join team t using (gameid, faction)
join game g using (gameid)
where g.gametype <> 'Turbo'
group by pl.playerid;


-- postcount
truncate postcount;

insert into postcount
select p.posterid, p.threadid, count(*) posts
from fennecfox.Post p
join fennecfox.Thread t using (threadid)
join game g on g.gameid=t.threadid
group by p.posterid, p.threadid;

-- derivedrankings
-- nah


