
-- backups
/*
mysqldump -uroot -pwerewolf pogwwdb > pogwwdb_backup_20140511.sql
mysqldump -uroot -pwerewolf fennecfox --ignore-table=fennecfox.Post --ignore-table=fennecfox.post2 --ignore-table=fennecfox.posttemp > fennecfox_bak.sql
mysqldump -uroot -pwerewolf turbo > turbo_schema_bak.sql
*/

-- ######################################

/*
# issues: ties
# -- won't work with new roles
caps safe
change playerlist to editable fields
deal with quotes in names/titles
http://forumserver.twoplustwo.com/59/puzzles-other-games/storm-swords-game-thrones-werewolf-game-game-thread-1350588/
^ this game doesn't load
*/

http://checkmywwstats.com/pog/index.php?report=Game&gameid=13208987
errbody listed twice


# Lin/ivers

village wins this:
http://forumserver.twoplustwo.com/59/puzzles-other-games/june-8th-ww-game-thread-503862/index22.html

# search tie games

-- figure out why there are dupes, posterid null
-- e.g. roleid 5131, gameid=563562


triple sub, startday=5, endday=4
gameid 1372095 slot 30 posterid 121114 roleid 17909 teamid 1899


check for ties as victory condition 2

-- playerlist on:
http://forumserver.twoplustwo.com/59/puzzles-other-games/wolf-pups-game-thread-1002970/index14.html#post25611050



#####################
#### reads list

create table readslist (gameid int, punterid int, gameday int, ord int, playerid int, primary key (gameid, punterid, gameday, ord));


