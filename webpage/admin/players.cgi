#!/usr/bin/env python
# -*- coding: UTF-8 -*-

# enable debugging
import cgitb
import cgi, os, sys, string, time, datetime
cgitb.enable()
sys.stderr = sys.stdout 

import MySQLdb
db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)

cursor = db.cursor(MySQLdb.cursors.DictCursor)
cursor.execute("set autocommit=0")

###################################################################
def makeSelect(name, values, idx_values, selectedValue=None):
	SEL = '<select name="{0}">\n{1}</select>\n'
	OPT = '<option value="{0}"{1}>{2}</option>\n'
	return SEL.format(name, ''.join(OPT.format(idx_values[i], " SELECTED" if values[i]==selectedValue else "", 
		values[i].encode('utf-8')) for i in range(len(values))))

###################################################################

print "Content-Type: text/html;charset=utf-8"
print
print "<head><title>WWDB Control Panel</title></head>"
print """<script type="text/javascript"> 
	function stopRKey(evt) { 
	var evt = (evt) ? evt : ((event) ? event : null); 
	var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null); 
	if ((evt.keyCode == 13) && (node.type=="text"))  {return false;} }
	document.onkeypress = stopRKey;
	</script>"""
print "<h3><ul>Werewolf Database Control Panel - POG</ul></h3><br><br>"

cursor.execute("select * from player order by playername")
result = cursor.fetchall()
players1 = []
playerid1 = []
players2=['(Not a gimmick)']
playerid2=[0]
for i in result:
	playerid1.append(i['playerid'])
	players1.append(i['playername'])
	if i['playerid'] == i['mainplayerid']:
		playerid2.append(i['playerid'])
		players2.append(i['playername'])

cursor.execute("select concat(rolename, ' (', count(*), ')') rolename, roleid from roles r left join roleset rs on r.roleid=rs.roletype where r.roleid <> 1 group by r.roleid order by 1")
result = cursor.fetchall()
roles = []
roleid = []
for i in result:
	roles.append(i['rolename'])
	roleid.append(i['roleid'])

cursor.execute("select gameid, url from game order by 2")
result = cursor.fetchall()
games = []
gameid = []
for i in result:
	games.append(i['url'])
	gameid.append(i['gameid'])

print """<form method="post" action="/players.html">
	Enter administrator password:
	<input type="text" name="passwd" size="20" value=""><br><br>
	_____________________________________________________________<br><br>
	<input type="text" name="addplayer" size="20" value="">
	<input type="submit" name="addplayer" value="Add Player"><br><br>
	Set:<br>
	%s<br>
	as a gimmick (alt account) of:<br>
	%s &nbsp
	<input type="submit" name="setgimmick" value="Update"><br><br>
	_____________________________________________________________<br><br>
	Existing roles (# of appearances):<br>
	%s<br><br>
	<input type="text" name="addrole" size="20" value="">
	<input type="submit" name="addplayer" value="Add new role"><br><br>
	_____________________________________________________________<br><br>
	Existing games:<br>
	%s<br>
	<input type="submit" name="deletegame" value="Delete game"><br><br>
	</form>""" % (
	makeSelect('player1', players1, playerid1), makeSelect('player2', players2, playerid2), 
	makeSelect('roles', roles, roleid), makeSelect('games', games, gameid))


