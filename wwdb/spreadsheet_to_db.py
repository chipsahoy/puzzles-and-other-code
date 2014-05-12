# original ww spreadsheet -> db

# tab delimited file; sheet is ordered by url

# columns:
# gamename, url, startdate, gametype, mod, playername, affiliation, role, death, victor, gameendday, include, por, rules, sub for, subday, NAs

data = [line.strip() for line in open('/home/pbae/puzzles-and-other-code/wwdb/turbospreadsheet.txt')]
len(data)
del data[0] # header

# separate columns
games = []
for i in data:
	games.append(i.split('\t'))

for i in range(len(games)):
	games[i] = [a.strip() for a in games[i]]

# verify urls are all together; there are dupe games :(
urls = []
for j, i in enumerate(games):
	if i[1] not in urls:
		urls.append(i[1])
	else:
		if i[1] != urls[-1]:
			print j


# split the games up and send them to DoGame
DoGame(games[0:12])


def DoGame(lines):
	# given the x lines of a game, return game JSON
	gamejson = {'startdate': lines[0][2], 'gamename': lines[0][0], 'gametype': lines[0][3], 
		'url': lines[0][1], 'mod': [lines[0][4]]}
	
	players = []
	subs = []
	victor = []
	factions = []
	
	for i in lines:
		if i[6] not in factions:
			factions.append(i[6])
		if i[9] == 0 and i[6] not in victor:
			victor.append(i[6])
		if i[8].lower() in ('s',''):
			deathtype = 'Survived'
		elif i[8].lower() == 'c':
			deathtype = 'Conceded'
		elif i[8].lower() == 'e':
			deathtype = 'Eaten'
		
		if i[8].lower() in ('s','c','e',''):
			deathday = int(i[10])
		else:
			if len(i[8]) != 2: # not in form like 'n2'
				print 'bad death type'
				return None
			else:
				deathday = int(i[8][1])
				if i[8][0] == 'l':
					deathtype = 'Lynched'
				elif i[8][0] == 'n':
					deathtype = 'Night Killed'
				elif i[8][0] == 'm':
					deathtype = 'Mod Killed'
				else:
					print 'invalid death type'
					return None
		
		role = i[7] if i[7] != '' else 'Vanilla'
		
		players.append({'op': i[5], 'faction': i[6], 'role': role, 'deathtype': deathtype, 'deathday': deathday})
	
	if len(factions) < 2:
		print "len(factions) < 2"
		return None
	
	gamejson['players'] = players
	
	return gamejson


# 0 gamename, url, startdate, gametype, mod, 
# 5 playername, affiliation, role, death, victor, 
# 10 gameendday, include, por, rules, sub for, subday, NAs

{'startdate': '2014-04-30', 
'subs': [{'subname': u'gambit8888', 'subday': 1, 'op': u'Willi'}, 
{'subname': u'younguns87', 'subday': 1, 'op': u'Chips Ahoy'}], 
'url': u'http://forumserver.twoplustwo.com/59/puzzles-other-games/kruze-willi-sitting-tree-smooch-smooch-1438809/', 
'gamename': u'Kruze and Willi sitting on a tree...Smooch Smooch', 'gametype': 'Turbo', 
'players': [{'deathday': 1, 'faction': u'Village', 'role': u'seer', 'op': u'cjkalt', 'deathtype': 'Lynched'}, 
{'deathday': 2, 'faction': 'Wolves', 'role': u'vanilla', 'op': u'Willi', 'deathtype': 'Lynched'}, 
{'deathday': 1, 'faction': 'Village', 'role': u'vanilla', 'op': u'VarianceMinefield', 'deathtype': 'Night Killed'}, 
{'deathday': 2, 'faction': 'Village', 'role': u'vanilla', 'op': u'LeonardoDicaprio', 'deathtype': 'Night Killed'}, 
{'deathday': 3, 'faction': 'Wolves', 'role': u'vanilla', 'op': u'Chips Ahoy', 'deathtype': 'Lynched'}, 
{'deathday': 3, 'faction': u'Village', 'role': u'vanilla', 'op': u'Dennis Donkey', 'deathtype': 'Survived'}, 
{'deathday': 3, 'faction': u'Village', 'role': u'vanilla', 'op': u'killer_kill', 'deathtype': 'Survived'}, 
{'deathday': 3, 'faction': u'Village', 'role': u'vanilla', 'op': u'KruZe', 'deathtype': 'Survived'}, 
{'deathday': 3, 'faction': u'Village', 'role': u'vanilla', 'op': u'mutigers5591', 'deathtype': 'Survived'}], 
'mod': ['Tatiana Tiger'], 'factions': ['Wolves', 'Village'], 'victor': ['Village']}
