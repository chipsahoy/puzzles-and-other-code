# pokernet.dk setup

create database pokernetdk;
use pokernetdk;

create user 'pokernetdk'@'localhost' identified by 'pokernetdk';

grant all privileges on `pokernetdk`.* to `pokernetdk`@`localhost`;
flush privileges;

--#############################################################
-- create tables

CREATE TABLE game (gameid int, gamename varchar(100) not null, numplayers int not null, factions int not null, startdate date not null, 
gametype enum('Vanilla','Vanilla+','Mish-Mash','Slow Game','Turbo') not null, gamelength int, url varchar(200), 
PRIMARY KEY (gameid)) engine=innodb;

create table moderator (gameid int, modid int, isprimary int not null, primary key (gameid, modid), index (modid)) engine=innodb;

create table actions (gameid int not null, slot int not null, ability varchar(20), night int not null, target int not null, 
index (gameid, slot, night)) engine=innodb;

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

create table thread (threadid int primary key, op int, posts int, title varchar(100), url varchar(200) not null, unique index (url)) default charset=utf8;
insert into thread select threadid, op, NULL as posts, NULL as title, url from fennecfox.Thread where subforumid=1000;

--############################################
-- covert from old db to new

insert into game
select g.threadid, g.gamename, count(*) numplayers, count(distinct t.teamid) factions, g.startdate, gametype, g.gamelength, h.url
from fennecfox.Game g
join fennecfox.Team t using (threadid)
join fennecfox.GameRole gr using (teamid)
join fennecfox.Thread h using (threadid)
join fennecfox.SubForum sf on sf.subforumid = g.subforumid
where sf.forumid=2
group by g.threadid;

-- player
insert into player
select posterid, postername, posterid
from fennecfox.Poster
where forumid=2;

-- moderator
insert into moderator
select g.threadid, g.modid, 1 isprimary
from fennecfox.Game g
join fennecfox.Poster p on g.modid=p.posterid
join fennecfox.SubForum f on f.forumid=p.forumid and f.subforumid=g.subforumid
where p.forumid=2 and g.subforumid=1000;

-- gimmicks
update player p join fennecfox.Gimmick g on g.posteridgimmick = p.playerid set p.mainplayerid = g.posteridmain
where g.posteridmain is not null and g.posteridgimmick is not null;

-- team
insert into team
select g.threadid, f.factionid, count(*) numplayers, t.victory, t.teamid
from fennecfox.Team t
join fennecfox.Game g using (threadid)
join fennecfox.Affiliation a using (affiliationid)
join faction f on a.affiliationname=f.factionname
join fennecfox.GameRole r on r.teamid=t.teamid
where g.subforumid=1000
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

