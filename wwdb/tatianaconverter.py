# tatiana game -> db converter

import MySQLdb
#db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)
db = MySQLdb.connect(host="localhost", user="dev", passwd="dev", db='dev', charset="utf8", use_unicode=True)
cursor = db.cursor(MySQLdb.cursors.DictCursor)

def ReplaceWithOP(player, playerlist, subs):
	if player not in playerlist: # if player was subbed
		player = subs[[a['subname']==player for a in subs].index(True)]['op']
	return player

def MakeGameSummary(threadid, posterid=388864):
	if posterid==388864:
		modname = 'Tatiana Tiger'
	elif posterid==256337:
		modname = 'Oswald Jacoby'
	else:
		print "unexpected mod"
		return None
	
	cursor.execute("select url from fennecfox.thread2 where threadid=%s", threadid)
	if cursor.rowcount == 0:
		print "Does not exist in the thread2 table"
		return None
	url = cursor.fetchall()[0]['url']
	cursor.execute("select postid, postnumber, cast(date(posttime) as char) posttime, title, content \
		from fennecfox.post2 where threadid=%s and posterid=%s order by postnumber", (threadid, posterid))
	if cursor.rowcount < 3:
		print "Not enough posts"
		return None
	posts = cursor.fetchall()
	if posts[-1]['title'] != 'Mod: Game Over':
		print "Game over post missing"
		return None
	
	gamename = posts[0]['title']
	startdate = str(posts[0]['posttime'])
	x = posts[0]['content']
	x = x[x.find('<u>Live Players:</u>')+26:]
	x = x[:x.find('<br>\r\n<br>')]
	playerlist = x.split('<br>')
	playerlist = [a.strip() for a in playerlist]
	# check player names against db. if mismatch, skip game
	for p in playerlist:
		cursor.execute("select playerid from player where playername = %s", p)
		if cursor.rowcount == 0:
			print "Player in playerlist not in db"
			return None
	
	players = []
	
	lynchposts = [i for i, a in enumerate(posts) if 'Lynch result' in a['title']]
	nkposts = [i for i, a in enumerate(posts) if 'It is day!' in a['title']]
	subposts = [i for i, a in enumerate(posts) if 'is subbing in for' in a['content']]
	subs = []
	
	# get subs
	for j, i in enumerate(subposts):
		x = posts[i]['content']
		while x.find('is subbing in for') > 0:
			x = x[x.find('<b>')+3:]
			subin = x[:x.find('</b>')].strip()
			x = x[x.find('<b>')+3:]
			subout = x[:x.find('</b>')].strip()
			subday = 1
			for postnumber in lynchposts:
				if j > postnumber:
					subday += 1
			if subout not in playerlist:
				print "subout not in playerlist"
				return None
			subs.append({'op':subout, 'subname':subin, 'subday':subday})
	
	for deathday, i in enumerate(lynchposts):
		x = posts[i]['content']
		x = x[x.find('<br>\n<b>')+8:]
		player = x[:x.find('</b>')].strip()
		player = ReplaceWithOP(player, playerlist, subs)
		x = x[x.find('"><b>')+5:]
		faction = x[:x.find('</b>')].strip()
		if faction == 'Wolf':
			faction = 'Wolves'
		x = x[x.find('"><b>')+5:]
		role = x[:x.find('</b>')].strip()
		players.append({'deathday': deathday+1, 'deathtype': 'Lynched', 'faction': faction, 'role': role, 'op': player})
	
	for deathday, i in enumerate(nkposts):
		x = posts[i]['content']
		x = x[x.find('wolves killed <b>')+17:]
		player = x[:x.find('</b>')].strip()
		player = ReplaceWithOP(player, playerlist, subs)
		x = x[x.find('"><b>')+5:]
		role = x[:x.find('</b>')].strip()
		players.append({'deathday': deathday+1, 'deathtype': 'Night Killed', 'faction': 'Village', 'role': role, 'op': player})
	
	# gameover post; last lynch
	x = posts[-1]['content']
	if x.find('resigned') < 0:
		x = x[x.find('<br>\n<b>')+8:]
		player = x[:x.find('</b>')].strip()
		player = ReplaceWithOP(player, playerlist, subs)
		x = x[x.find('"><b>')+5:]
		faction = x[:x.find('</b>')].strip()
		if faction == 'Wolf':
			faction = 'Wolves'
		x = x[x.find('"><b>')+5:]
		role = x[:x.find('</b>')].strip()
		players.append({'deathday': len(lynchposts)+1, 'deathtype': 'Lynched', 'faction': faction, 'role': role, 'op': player})
	
	if x.find('Wolves Win!') > 0:
		victor = ['Wolves']
	else:
		victor = ['Village']
	
	x = x[x.find('Remaining Players')+32:]
	
	while x.find(' : ') < x.find('Peeks:'):
		player = x[:x.find(' : ')].strip()
		player = ReplaceWithOP(player, playerlist, subs)
		x = x[x.find(' : ')+3:]
		faction = x[:x.find(' ')].strip()
		if faction == 'Wolf':
			faction = 'Wolves'
		role = x[x.find(' ')+1:x.find('<br>')].strip()
		x = x[x.find('\n')+1:]
		if victor[0] == 'Wolves' and faction == 'Wolves':
			deathtype = 'Survived'
		elif victor[0] == 'Wolves' and faction == 'Village':
			deathtype = 'Eaten'
		elif victor[0] == 'Village' and faction == 'Village':
			deathtype = 'Survived'
		elif victor[0] == 'Village' and faction == 'Wolves':
			deathtype = 'Conceded'
		else:
			print 'wat @ deathtype'
			print victor
			print faction
		players.append({'deathday': len(lynchposts)+1, 'deathtype': deathtype, 'faction': faction, 'role': role, 'op': player})
	
	x = x[x.find('Peeks:</u>')+20:]
	# find das seer
	for p in players:
		if p['role'].upper() == 'SEER':
			seername = p['op']
	peeks = []
	night = 0
	while x.find(' : ') > 0:
		player = x[:x.find(' : ')].strip()
		player = ReplaceWithOP(player, playerlist, subs)
		x = x[x.find(' : ')+3:]
		x = x[x.find('\n')+1:]
		peeks.append({'ability': 'Peek', 'target': player, 'night': night, 'actor': seername})
		night += 1
		
	summary = {'gamename':gamename, 'startdate':startdate, 'factions': ['Wolves', 'Village'], 'url':url, 'gametype':'Turbo', 
		'victor': victor, 'mod':[modname]}
	if len(subs) > 0:
		summary['subs'] = subs
	summary['players'] = players
	summary['actions'] = peeks
	
	return summary


cursor.execute("select threadid, t.title, t.url from fennecfox.thread2 t join fennecfox.post2 p using (threadid) \
	where op=388864 and threadid > (select max(gameid) from moderator where modid=388864) and t.op=p.posterid and p.postnumber=1 \
	order by threadid")
tatianagames = cursor.fetchall()
len(tatianagames)

import textjsondb

#MakeGameSummary(tatianagames[0]['threadid'], 388864)

#textjsondb.JSONtoDB(MakeGameSummary(tatianagames[0]['threadid']), cursor, 'tatiana batch 2 try 1')

for j, i in enumerate(tatianagames):
	summary = MakeGameSummary(i['threadid'], 388864)
	if summary is not None:
		print j
		textjsondb.JSONtoDB(summary, cursor, 'tatiana batch 3')
		cursor.execute("commit")


#cursor.execute("select threadid, t.title, t.url from fennecfox.thread2 t join fennecfox.post2 p using (threadid) \
#	where op=256337 and threadid not in (1426309,1411072,1432617) and t.op=p.posterid and p.postnumber=1")


# http://checkmywwstats.com/dev/index.php?report=Game&gameid=1437996
# tatiana missed a sub

