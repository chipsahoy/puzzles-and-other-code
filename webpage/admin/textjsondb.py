# iversonian
# 04/30/14

#######################################
# json -> db

def JSONtoDB(j, cur, msg='added'):
	RemoveGameFromDB(j['url'], cur) # delete existing game
	
	gameid = GetThreadIDFromURL(j['url'])
	gamelength = max([x['deathday'] for x in j['players']])
	
	# game
	cur.execute("insert into game values (%s, %s, %s, %s, %s, %s, %s, %s)",
		(gameid, j['gamename'], len(j['players']), len(j['factions']), j['startdate'], j['gametype'], gamelength, j['url']))
	
	# team (deal with ties later)
	for i in j['factions']:
		n = sum(item['faction']==i for item in j['players'])
		cur.execute("insert into team values (%s, (select factionid from faction where factionname = %s), %s, %s, NULL)",
			(gameid, i, n, int(i.lower() in (a.lower() for a in j['victor']))))
	
	# mod
	for n, i in enumerate(j['mod']):
		cur.execute('select playerid from player where playername=%s', i)
		modid = cur.fetchone()['playerid']
		cur.execute("insert into moderator values (%s, %s, %s)", (gameid, modid, int(n == 0)))
	
	# roleset & playerlist
	for n, i in enumerate(j['players']):
		cur.execute("""insert into roleset values (%s, %s, (select factionid from faction where factionname = %s),
			(select roleid from roles where rolename = %s), (select deathtypeid from deathtype where deathtypename=%s), %s, %s, NULL)""",
			(gameid, n+1, i['faction'], i['role'], i['deathtype'], i['deathday'], 1))
		cur.execute("""insert into playerlist values (%s, %s, %s, NULL, (select playerid from player where playername = %s), %s, NULL)""", 
			(gameid, n+1, 1, i['op'], 0))
	
	# subs
	if 'subs' in j:
		for i in j['subs']:
			cur.execute("""select pl.slot, rs.players from playerlist pl join roleset rs using (gameid, slot) 
				where gameid=%s and ordinal=1 and playeraccount=(select playerid from player where playername = %s)""",
				(gameid, i['op']))
			result = cur.fetchone()
			cur.execute("insert into playerlist values (%s, %s, %s, NULL, (select playerid from player where playername = %s), %s, NULL)",
				(gameid, result['slot'], result['players']+1, i['subname'], i['subday']))
			cur.execute("update playerlist set dayout = %s where gameid=%s and slot=%s and ordinal=%s",
				(i['subday'], gameid, result['slot'], result['players']))
			cur.execute("update roleset set players = players + 1 where gameid=%s and slot=%s", (gameid, result['slot']))
	# actions
	
	# set main player id in playerlist table
	cur.execute("update playerlist pl join player p on p.playerid=pl.playeraccount set pl.playerid=p.mainplayerid where pl.gameid=%s", gameid)
	
	SaveJSONToLog(j['url'], cur, msg, str(j))
	cur.execute("commit")
	return 1

#######################################
# db -> json

def DBtoJSON(url, cur):
	cur.execute("select * from game where url='%s'" % url)
	if cur.rowcount == 0:
		return None
	
	g = cur.fetchone()
	gameid = g['gameid']
	jsontxt = {'startdate': str(g['startdate']), 'gamename': g['gamename'], 'gametype': g['gametype'], 'url': g['url']}
	
	jsontxt['victor'] = []
	jsontxt['factions'] = []
	jsontxt['players'] = []
	jsontxt['mod'] = []
	subs = [] # add only if there are subs
	
	cur.execute("select playername from moderator m join player p on m.modid=p.playerid where m.gameid=%s order by isprimary desc" %gameid)
	for i in range(0,cur.rowcount):
		jsontxt['mod'].append(str(cur.fetchone()['playername']))
	
	cur.execute("select factionname, victory from faction f join team t on t.faction=f.factionid join game g using (gameid) where g.gameid=%s" %gameid)
	for i in range(0,cur.rowcount):
		x = cur.fetchone()
		jsontxt['factions'].append(str(x['factionname']))
		if x['victory'] == 1:
			jsontxt['victor'].append(str(x['factionname']))
	
	cur.execute("select p.playername, f.factionname, rs.slot, r.rolename, rs.deathday, dt.deathtypename, \
		(select max(ordinal)-1 from playerlist x where x.gameid=rs.gameid and x.slot=rs.slot) subs \
		from roleset rs join playerlist pl using (gameid, slot) join player p using (playerid) join faction f on f.factionid=rs.faction \
		join roles r on r.roleid=rs.roletype join deathtype dt on dt.deathtypeid=rs.deathtype where ordinal = 1 and rs.gameid=%s" %gameid)
	result = cur.fetchall()
	
	for p in result:
		jsontxt['players'].append({'op':p['playername'], 'faction':p['factionname'], 'role':p['rolename'], 'deathday':int(p['deathday']), 
			'deathtype':p['deathtypename']})
		if p['subs'] > 0:
			cur.execute('select p.playername, dayin, dayout from playerlist pl join player p using (playerid) \
				where pl.gameid=%s and pl.slot=%s and ordinal > 1 order by ordinal' % (gameid, p['slot']))
			if cur.rowcount == 0:
				print "Subs are missing for gameid: %s" % gameid
				exit()
			for i in range(cur.rowcount):
				x = cur.fetchone()
				subs.append({'op':p['playername'], 'subname':x['playername'], 'subday':x['dayin']})
	
	if len(subs) > 0:
		jsontxt['subs'] = subs		
	
	return jsontxt

# j = DBtoJSON('http://forumserver.twoplustwo.com/59/puzzles-other-games/3-game-mafia-champions-ww-invitational-game-thread-1428519/',cursor)

#######################################
# misc functions

def GetThreadIDFromURL(url):
	if '-' not in url:
		return None
	url = url[url.rindex('-')+1:]
	if '/' not in url:
		return None
	url = url[0:url.index('/')]
	if not url.isdigit():
		return None
	return int(url)

def SaveJSONToLog(url, cur, msg='', jsontxt=None):
	if jsontxt is None:
		jsontxt = str(DBtoJSON(url, cur))
	cur.execute('insert into editslog values (now(), %s, %s, %s)', (msg, url, jsontxt))
	cur.execute('commit')

def RemoveGameFromDB(url, cur, commit=False):
	cur.execute("select * from game where url = %s", url)
	if cur.rowcount > 0:
		#SaveJSONToLog(url, cur, 'deleted', None)
		gameid = cur.fetchone()['gameid']
		cur.execute("delete from actions where gameid=%s", gameid)
		cur.execute("delete from playerlist where gameid=%s", gameid)
		cur.execute("delete from roleset where gameid=%s", gameid)
		cur.execute("delete from moderator where gameid=%s", gameid)
		cur.execute("delete from team where gameid=%s", gameid)
		cur.execute("delete from game where gameid=%s", gameid)
	if commit:
		cur.execute("commit")


