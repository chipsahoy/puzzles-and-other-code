
-- mod for game 3, hadn't posted in pog ever
insert into player values (6913, 'poker-penguin', 6913);
insert into moderator values (5340033, 6913, 1);

# add moocher as a comod for wild west game
insert into moderator values (1327565, 131995, 0);

# get rid of quotes
update game set gamename= 'Wild, Wild West' where gameid=1327565;

# rainingmen
update player set mainplayerid=322212 where playerid=322212;
update playerlist set playerid=322212 where playeraccount=128574;


-- ######################################

sudo nano /etc/mysql/my.cnf

/*
# issues: ties
# -- won't work with new roles
caps safe
change playerlist to editable fields
deal with quotes in names/titles
http://forumserver.twoplustwo.com/59/puzzles-other-games/storm-swords-game-thrones-werewolf-game-game-thread-1350588/
^ this game doesn't load
*/

# Lin/ivers

village wins this:
http://forumserver.twoplustwo.com/59/puzzles-other-games/june-8th-ww-game-thread-503862/index22.html

# search tie games

-- figure out why there are dupes, posterid null
-- e.g. roleid 5131, gameid=563562


triple sub, startday=5, endday=4
gameid 1372095 slot 30 posterid 121114 roleid 17909 teamid 1899


check for ties as victory condition 2




-- join gimmicks

update player set mainplayerid= where playerid=;

update playerlist set playerid= where playeraccount=;





#####################
#### reads list

create table readslist (gameid int, punterid int, gameday int, ord int, playerid int, primary key (gameid, punterid, gameday, ord));


