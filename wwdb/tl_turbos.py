# TL turbos to db

import MySQLdb
db = MySQLdb.connect(host="localhost", user="root", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)
cursor = db.cursor(MySQLdb.cursors.DictCursor)

games_raw = [line.strip() for line in open('tl_turbos1.txt')]
players_raw = [line.strip() for line in open('tl_turbos2.txt')]


gamedata = []
for i in games_raw:
	gamedata.append(i.split('\t'))

for i in range(len(gamedata)):
	gamedata[i] = [a.strip() for a in gamedata[i]]

playerdata = []
for i in players_raw:
	playerdata.append(i.split('\t'))


for j, i in enumerate(playerdata):
	i = [a.strip() for a in i]
	

def GetPlayers(url):
	p = []
	for x in playerdata:
		if x[1] == url:
			p.append([x[0],x[2],x[3],x[4]])
	return p

summary = []

for i, g in enumerate(gamedata):
	if g[4] == 'no':
		continue
	
	playerlist = GetPlayers(g[0])
	
	cursor.execute("select t.*, p.postername from fennecfox.Thread t join fennecfox.Poster p on t.op=p.posterid where url = %s", g[0])
	x = cursor.fetchone()
	gamename = x['title']
	mod = x['postername']
	victor = ['Village'] if g[3] == 'villa' else ['Wolves']
	
	j = {'url':g[0], 'startdate':g[1], 'mod':[mod], 'victor':victor, 'factions':['Village','Wolves'], 'gamename':gamename, 'gametype':'Turbo'}
	
	gamelength = int(max([x[2] for x in playerlist]))
	
	players = []
	for p in playerlist:
		affiliation = 'Wolves' if p[1] == 'wolf' else 'Village'
		deathday = gamelength if p[2] == '' else int(p[2])
		deathtype = p[3]
		if deathtype == 'Survived':
			if affiliation == 'Wolves' and victor == ['Village']:
				deathtype = 'Conceded'
			elif affiliation == 'Village' and victor == ['Wolves']:
				deathtype = 'Eaten'
		players.append({'op':p[0], 'faction':affiliation, 'role':'Seer' if p[1]=='seer' else 'Vanilla', 'deathtype':deathtype, 'deathday':deathday})
	
	j['players'] = players
	summary.append(j)


import textjsondb

# textjsondb.JSONtoDB(summary[0], cursor, 'TL test 3')

for i in summary:
	textjsondb.JSONtoDB(i, cursor, 'script TL turbos 1')

# peaks
# n0 bingo, n1 caedus n2 shorty http://forumserver.twoplustwo.com/59/puzzles-other-games/sunday-night-turbo-2-a-758298/