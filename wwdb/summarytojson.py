import MySQLdb
db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)
#db = MySQLdb.connect(host="localhost", user="dev", passwd="dev", db='dev', charset="utf8", use_unicode=True)
cursor = db.cursor(MySQLdb.cursors.DictCursor)
cursor.execute("set autocommit=0")


def ParseText(x, cursor):
	idx = x.find('url:')
	if idx == -1:
		print 'Missing url'
		return None
	url = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	if url is None:
		return None
	
	idx = x.find('game name:')
	if idx >= 0:
		gamename = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	else:
		gamename = ''
	if gamename == '':
		gamename = RetrieveGame(url, cursor)[0]
	
	idx = x.find('game type:')
	if idx == -1:
		print 'Missing gametype'
		return None
	gametype = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	
	idx = x.find('start date:')
	if idx >= 0:
		startdate = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	else:
		startdate = ''
	
	idx = x.find('mod:')
	if idx >= 0:
		mod = x[x.find(':', idx)+1:x.find('\n', idx)].strip().split(';')
	else:
		mod = ''
	
	idx = x.find('player list:')
	playerdeaths = x[x.find(':', idx)+1:x.find('\n\n', idx+13)].strip().split('\n') # allow for double space after "player list:"
	playerdeaths = [a.strip() for a in playerdeaths]
	if len(playerdeaths) not in (9,13,17):
		print playerdeaths
		print len(playerdeaths)
		print "Report is not a properly formatted summary for a 9/13/17er game\n"
		return None
	
	idx = x.find('subs:')
	if (idx > 0):
		subs = x[x.find(':', idx)+1:x.find('\n\n', idx)].strip().split('\n')
		if subs == ['none']:
			subs = ['']
	else:
		subs = ['']
	
	idx = x.find('peeks:')
	peeks = x[x.find(':', idx)+1:x.find('\n', idx)].strip().split(';')
	peeks = [a.strip().lower() for a in peeks]
	
	idx = x.find('victor:')
	victor = x[x.find(':', idx)+1:x.find('\n', idx)].strip().lower()
	victor = victor[0] # truncate to w/v
	if victor == 'v':
		victor = ['Village']
	else:
		victor = ['Wolves']
	
	## parse players/deaths
	for i in range(0, len(playerdeaths)):
		if playerdeaths[i][-1] == ';':
			playerdeaths[i] = playerdeaths[i][:-1] # if there is a trailing semicolon, get rid of it
	
	playerdeaths = [a.split(';') for a in playerdeaths]
	playerdeaths = [[b.strip() for b in a] for a in playerdeaths]
	
	playername = [a[0] for a in playerdeaths]
	playername = [a.strip() for a in playername]
	affiliation = [a[1].lower() for a in playerdeaths]
	affiliation = [a[0] for a in affiliation]
	role = ['Seer' if a[0]=='s' else 'Vanilla' for a in affiliation]
	affiliation = ['Wolves' if a[0]=='w' else 'Village' for a in affiliation]
	seer = playername[role.index('Seer')]
	
	death = [a[2] if len(a)==3 else 'x0 didnotdie' for a in playerdeaths]
	death = ['x0 didnotdie' if a[0] in ('c','s','e') else a for a in death]
	death = [a.split(' ') for a in death]
	
	deathday = [int(a[0][1:]) for a in death]
	deathday = [max(deathday) if a == 0 else a for a in deathday]
	deathtype = [a[1] for a in death]
	
	# either way is ok
	for i in range(0,len(deathtype)):
		if deathtype[i] == 'didnotdie':
			if affiliation[i] == 'Wolves':
				deathtype[i] = 'Survived' if victor==['Wolves'] else 'Conceded'
			else:
				deathtype[i] = 'Survived' if victor==['Village'] else 'Eaten'
	
	deathtype = [('Night Killed' if a[0] == 'n' else a) for a in deathtype]
	deathtype = [('Lynched' if a[0] == 'l' else a) for a in deathtype]
	deathtype = [('Modkilled' if a[0] == 'm' else a) for a in deathtype]
	
	## parse subs
	subs2 = []
	if subs[0].strip() != '':
		subs = [a.split(';') for a in subs]
		subs = [[b.strip().lower() for b in a] for a in subs]
		for i in range(0,len(subs)):
			if subs[i][2].isdigit():
				subs[i][2] = int(subs[i][2])
			else:
				subs[i][2] = int(subs[i][2][1:])
		
		## check against playerlist
		# if user included the sub in the player list rather than original player, then correct player list
		for i in range(0,len(subs)):
			if subs[i][0] in playername:
				playername = [subs[i][1] if a == subs[i][0] else a for a in playername]
		
		for s in subs:
			subs2.append({'op':s[1], 'subname':s[0], 'subday':s[2]})
	
	players = []
	for i in range(len(playername)):
		players.append({'op':playername[i], 'faction':affiliation[i], 'deathday':deathday[i], 'deathtype':deathtype[i], 'role':role[i]})
	
	actions = []
	for night, i in enumerate(peeks):
		actions.append({'actor':seer, 'target':i, 'night':night, 'ability':'Peek'})
	
	game = {'gamename':gamename, 'mod':mod, 'startdate':startdate, 'gametype':gametype, 'url':url, 'victor':victor, 
		'players':players, 'factions':['Village','Wolves']}
	
	if len(subs2) > 0:
		game['subs'] = subs2
	if len(actions) > 0:
		game['actions'] = actions
	
	return game


def RetrieveGame(url, cursor):
	cursor.execute("select t.title, playername as moderator, ifnull(date(o.posttime),'') startdate \
		from fennecfox.Thread t join player p on p.playerid=t.op \
		left join fennecfox.Post o on o.threadid=t.threadid and o.postnumber=1 \
		where t.url=%s", url)
	if cursor.rowcount > 0:
		thread = cursor.fetchone()
	return (thread['title'], thread['moderator'], thread['startdate'])


import textjsondb

x = x.lower()
j = ParseText(x[:x.find('end summary')], cursor)
JSONtoDB(j, cursor, 'summary template 1')

x = open('turbos_to_add.txt', "r")
x = x.read().lower()

while x.find('vanilla werewolf game summary') >= 0:
	j = ParseText(x[:x.find('end summary')], cursor)
	if j is not None:
		JSONtoDB(j, cursor, 'summary template 3')
		print j
	x = x[x.find('end summary') + 11:]

