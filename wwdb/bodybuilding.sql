-- create new database for bodybuilding.dk forum

create database bodybuilding default character set 'utf8';

create user `bodybuilding`@`localhost` identified by 'bodybuilding';

grant all privileges on bodybuilding.* to bodybuilding@localhost;
grant select on fennecfox.* to bodybuilding@localhost;
flush privileges;

use bodybuilding;

create table game like pog.game;
create table moderator like pog.moderator;
create table actions like pog.actions;
create table roleset like pog.roleset;
create table playerlist like pog.playerlist;
create table team like pog.team;
create table faction like pog.faction;
create table roles like pog.roles;
create table player like pog.player;
create table editslog like pog.editslog;
create table postcount like pog.postcount;

create view victor as
select g.gameid, if(sum(t.victory=2)>0, 'Tie', group_concat(if(t.victory=1, f.factionname, NULL))) victor,
sum(t.victory=1) numvictors, sum(t.victory=2)>0 tiegame
from game g
join team t using (gameid)
join faction f on f.factionid=t.faction
group by g.gameid;

