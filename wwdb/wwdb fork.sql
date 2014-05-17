/*
iversonian
4/16/14
wwdb update

Player/role: not a series of players, but rather of ordinal values, with a player associated with it
Gimmicks: aggregate by main account from the start

Each forum has its own db, own webpage, to contain the ebolaids
*/

--#############################################################

create database pog;
alter database pog default character set 'utf8';

create user 'dev'@'localhost' identified by 'dev';

grant all privileges on dev.* to dev@localhost;
flush privileges;

--#############################################################
-- create tables

use pog;

CREATE TABLE game (gameid int, gamename varchar(100) not null, numplayers int not null, factions int not null, startdate date not null, 
gametype enum('Vanilla','Vanilla+','Mish-Mash','Slow Game','Turbo') not null, gamelength int, url varchar(200), 
PRIMARY KEY (gameid)) engine=innodb;

create table moderator (gameid int, modid int, isprimary int not null, primary key (gameid, modid), index (modid)) engine=innodb;

create table actions (gameid int not null, slot int not null, ability varchar(20), night int not null, target int not null, 
index (gameid, slot, night), index (gameid, target)) engine=innodb;

create table player (playerid int, playername varchar(50) not null, mainplayerid int not null, 
primary key (playerid), unique (playername), index (mainplayerid)) engine=innodb;

create table roleset (gameid int, slot int, faction int, roletype int, deathtype char(17), deathday int, players int, roleid int,
primary key (gameid, slot)) engine=innodb;

create table playerlist (gameid int, slot int, ordinal int, playerid int, dayin int, dayout int, primary key (gameid, slot, ordinal)) engine=innodb;

create table team (gameid int, faction int, players int, victory int, teamid int, primary key (gameid, faction)) engine=innodb;

create table forum (forumid int, forumname varchar(50), primary key (forumid)) engine=innodb;

create table faction (factionid int, factionname varchar(30), primary key (factionid)) engine=innodb;
insert into faction values (1, 'Village'), (10, 'Wolves'), (11, 'Wolves1'), (12, 'Wolves2'), (13, 'Wolves3'), (14, 'Wolves4'), (15, 'Wolves5'), 
(20, 'Neutral'), (21, 'Neutral1'), (22, 'Neutral2'), (23, 'Neutral3'), (24, 'Neutral4'), (25, 'Neutral5'), (26, 'Neutral6'), 
(30, 'SK'), (31, 'SK1'), (32, 'SK2'), (33, 'SK3'), (40, 'Jester'), (99, 'None');

create table roles (roleid int, rolename varchar(50), description varchar(255), primary key (roleid)) engine=innodb;
insert into roles
select roleid, rolename, NULL
from fennecfox.Role;

create table postcount (posterid int, threadid int, posts int, primary key (posterid, threadid));

create table editslog (gameid int, uploadtime timestamp, message varchar(100), gamejson text, index (gameid)) engine=innodb;

create view victor as
select g.gameid, if(sum(t.victory=2)>0, 'Tie', group_concat(if(t.victory=1, f.factionname, NULL))) victor,
sum(t.victory=1) numvictors, sum(t.victory=2)>0 tiegame
from game g
join team t using (gameid)
join faction f on f.factionid=t.faction
group by g.gameid;

--create view thread as select * from fennecfox.Thread;
--create table thread (threadid int primary key, op int, posts int, title varchar(100), url varchar(200) not null, unique index (url)) default charset=utf8;
--insert into thread select threadid, op, if(replies=0,NULL,replies)+1 posts, title, url from fennecfox.Thread;

--############################################
-- covert from old db to new

insert into game
select g.threadid, g.gamename, count(*) numplayers, count(distinct t.teamid) factions, g.startdate, gametype, g.gamelength, h.url
from fennecfox.Game g
join fennecfox.Team t using (threadid)
join fennecfox.GameRole gr using (teamid)
join fennecfox.Thread h using (threadid)
join fennecfox.SubForum sf on sf.subforumid = g.subforumid
where sf.forumid=1
group by g.threadid;

-- player
insert into player
select posterid, postername, posterid
from fennecfox.Poster
where forumid=1;

-- moderator
insert into moderator
select g.threadid, g.modid, 1 isprimary
from fennecfox.Game g
join fennecfox.Poster p on g.modid=p.posterid
join fennecfox.SubForum f on f.forumid=p.forumid and f.subforumid=g.subforumid
where p.forumid=1;

-- gimmicks
update player p join fennecfox.Gimmick g on g.posteridgimmick = p.playerid set p.mainplayerid = g.posteridmain
where g.posteridmain is not null and g.posteridgimmick is not null;

-- team
insert into team
select g.threadid, f.factionid, count(*) numplayers, t.victory, t.teamid
from fennecfox.Team t
join fennecfox.Game g using (threadid)
join fennecfox.Affiliation a using (affiliationid)
join wwdb.faction f on a.affiliationname=f.factionname
join fennecfox.GameRole r on r.teamid=t.teamid
group by t.teamid;

-- roleset
set @ord=0;
drop table if exists temp1;
create table temp1 (x int primary key)
select @ord:=@ord+1 as x
from game limit 200;

drop table if exists temp3;
create temporary table temp3
select t1.gameid, t1.faction, sum(t2.players) psum, t1.players p
from team t1 join team t2 using (gameid) 
where t2.faction <= t1.faction 
group by t1.gameid, t1.faction;

insert into roleset
select g.gameid, x slotid, t3.faction, NULL, NULL, NULL, NULL, NULL
from game g
join temp3 t3 on t3.gameid=g.gameid
join temp1 t1
where t1.x between psum-p+1 and psum
order by 1, 3;

drop table if exists temp3, temp1;

set @ord=0;
drop table if exists temp1;
create temporary table temp1 (x int primary key)
select @ord:=@ord+1 x, gameid, slot, faction
from roleset r
order by gameid, faction, slot;

set @ord=0;
drop table if exists temp2;
create temporary table temp2 (x int primary key)
select @ord:=@ord+1 x, t.threadid gameid, m.faction, pt.roleid, dt.deathtypename, pt.deathday
from fennecfox.GameRole pt
join fennecfox.Team t using (teamid)
join fennecfox.Game g using (threadid)
join fennecfox.DeathType dt on dt.deathtypeid=pt.deathtypeid
join team m using (teamid)
where g.subforumid < 500
order by t.threadid, m.faction;

update roleset r
join temp1 t1 using (gameid, slot)
join temp2 t2 using (x)
set r.roleid=t2.roleid, r.deathtype=t2.deathtypename, r.deathday=t2.deathday;

update roleset set players = 1;

drop table if exists temp1;
create temporary table temp1 (ordinal int, index (gameid, slot))
select r.gameid, r.slot, 0 ordinal, p.posterid, gr.roleid, startday, endday, t.teamid
from fennecfox.GameRole gr
join roleset r on r.roleid = gr.roleid
join fennecfox.Player p on p.roleid=gr.roleid
join fennecfox.Team t on t.teamid=gr.teamid and t.threadid=r.gameid
where p.forumid=1 and p.posterid is not null;

drop table if exists temp2;
create temporary table temp2 select gameid, slot from temp1 group by gameid, slot having count(*) = 1;

-- get all the non-subs first
update temp1 t1 join temp2 using (gameid, slot) set t1.ordinal = 1;

-- players who started day 0, get lowest endday
drop table if exists temp2;
create temporary table temp2
select gameid, slot, min(endday) firstendday
from temp1 
where ordinal = 0 and startday = 0
group by gameid, slot;

-- check for dupes
select * from temp1 t1 join temp2 t2 using (gameid, slot)
where t2.firstendday=t1.endday group by t1.gameid, t1.slot having count(*) > 1;

update temp1 t1 join temp2 t2 using (gameid, slot) set t1.ordinal = 1 where t2.firstendday=t1.endday;

-- set to ordinal=2 where they're the only ones left
drop table if exists temp2;
create temporary table temp2
select gameid, slot, min(startday) firststartday, count(*) n
from temp1
where ordinal = 0
group by gameid, slot;

-- check for dupes
select * from temp1 t1 join temp2 t2 using (gameid, slot)
where t2.firststartday=t1.endday group by t1.gameid, t1.slot having count(*) > 1;

update temp1 t1 join temp2 t2 using (gameid, slot) set t1.ordinal = 2 where t2.firststartday=t1.startday and t1.ordinal=0;

-- get the triple-subs
update temp1 set ordinal = 3 where ordinal = 0;
-- should be done now

insert into playerlist
select gameid, slot, ordinal, posterid as playerid, startday, endday
from temp1;

update roleset r join fennecfox.GameRole g on r.roleid=g.roleid set r.roletype = roletypeid;

--####################################
-- misc

-- why were these here?
delete from team where gameid not in (select gameid from game);

-- unique urls in game
alter table game add unique index (url);

-- consolidate w/n factions
alter table team drop primary key;
alter table team add column oldfaction int;
update team set oldfaction = faction;
insert into faction values (27, 'Neutral7');
insert into faction values (28, 'Neutral8');

create temporary table temp1
select gameid from team group by gameid having sum(faction=10) and sum(faction=11)=0;

update team set faction = faction+1 where faction between 10 and 18 and gameid in (select gameid from temp1);

create temporary table temp2
select gameid from team where faction=10;

update team set faction = faction+1 where faction between 10 and 18 and gameid in (select gameid from temp2);

create temporary table temp3
select gameid from team group by gameid having sum(faction=20) and sum(faction=21)=0;

update team set faction = faction+1 where faction between 20 and 28 and gameid in (select gameid from temp3);

create temporary table temp4
select gameid from team where faction=20;

update team set faction = faction+1 where faction between 20 and 28 and gameid in (select gameid from temp4);

drop table if exists temp5;
create temporary table temp5
select gameid, max(if(faction between 20 and 28, faction, 20)) maxneutral, min(if(faction>=30,faction,NULL)) minsk
from team group by gameid having sum(faction>=30);

update team t join temp5 x on t.gameid=x.gameid set faction = maxneutral+1 where t.faction = x.minsk;

drop table if exists temp5;
create temporary table temp5
select gameid, max(if(faction between 20 and 28, faction, 20)) maxneutral, min(if(faction>=30,faction,NULL)) minsk
from team group by gameid having sum(faction>=30);

update team t join temp5 x on t.gameid=x.gameid set faction = maxneutral+1 where t.faction = x.minsk;

drop table if exists temp5;
create temporary table temp5
select gameid, max(if(faction between 20 and 28, faction, 20)) maxneutral, min(if(faction>=30,faction,NULL)) minsk
from team group by gameid having sum(faction>=30);

update team t join temp5 x on t.gameid=x.gameid set faction = maxneutral+1 where t.faction = x.minsk;

drop table if exists temp5;
create temporary table temp5
select gameid, max(if(faction between 20 and 28, faction, 20)) maxneutral, min(if(faction>=30,faction,NULL)) minsk
from team group by gameid having sum(faction>=30);

update team t join temp5 x on t.gameid=x.gameid set faction = maxneutral+1 where t.faction = x.minsk;

drop table if exists temp5;
create temporary table temp5
select gameid, max(if(faction between 20 and 28, faction, 20)) maxneutral, min(if(faction>=30,faction,NULL)) minsk
from team group by gameid having sum(faction>=30);

update team t join temp5 x on t.gameid=x.gameid set faction = maxneutral+1 where t.faction = x.minsk;

alter table team add primary key (gameid, faction);
alter table team drop column oldfaction;

update roleset rs join team t on t.gameid=rs.gameid and t.oldfaction=rs.faction set rs.faction = t.faction;

delete from faction where factionid in (10,20);
delete from faction where factionid >=30;

update faction set factionname='Wolves' where factionid=11;
update faction set factionname='Neutral' where factionid=21;

--############################################
-- update player from fennecfox.Poster
insert into player
select posterid as playerid, postername as playername, posterid as mainplayerid
from fennecfox.Poster t
left join player p on p.playerid=t.posterid
where p.playerid is null and t.forumid=1;

--############################################

alter table roleset drop column roleid;
alter table roleset modify column deathtype char(17);
update roleset rs join deathtype dt on rs.deathtype=dt.deathtypeid set rs.deathtype = dt.deathtypename;
drop table deathtype;
update roles set rolename='Vigilante' where roleid=4;
update roleset set roletype = 1 where roletype=215; -- rolename = ''
delete from roles where roleid=215;

-- disallow duplicate rolenames
update roleset set roletype = 4 where roletype=67;
delete from roles where roleid=67;
alter table roles add unique index (rolename);

-- #
alter table team drop column teamid;
alter table game modify column gametype enum('Vanilla','Vanilla+','Mish-Mash','Slow Game','Turbo','Turbo Mishmash');

alter table playerlist add unique (gameid, slot, playerid);

-- finish conversion
--############################################
-- error fixes

update roleset set deathday=8, deathtype=3 where gameid=1297750 and slot=17;
update roleset rs join game g using (gameid) set rs.deathday = g.gamelength where deathday is null;
# game:1349915 is foxtrot uniform

--####################################
-- todo:



-- aggregate the gimmicks
update playerlist pl join player p on pl.playerid=p.playerid set pl.playerid = p.mainplayerid;

-- when someone changes their main acct, need to update tables: player & playerlist

-- update the mod table to include accountid field
-- change json->db to reflect mainaccountid stuff



--####################################
-- pending changes to production:

