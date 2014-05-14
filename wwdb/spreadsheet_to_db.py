# original ww spreadsheet -> db

# tab delimited file; sheet is ordered by url
# columns:
# gamename, url, startdate, gametype, mod, playername, affiliation, role, death, victor, gameendday, include, por, rules, sub for, subday, NAs

import MySQLdb
db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)

cursor = db.cursor(MySQLdb.cursors.DictCursor)

def TrimQuotes(x):
	if x[0] in ('"',"'"):
		x = x[1:]
	if x[-1] in ('"',"'"):
		x = x[:-1]
	return x

def get_index(seq, attr, value):
	return next(index for (index, d) in enumerate(seq) if d[attr] == value)

def DoGame(lines):
	# given the x lines of a game, return game JSON
	gamejson = {'startdate': lines[0][2], 'gamename': lines[0][0], 'gametype': lines[0][3], 
		'url': lines[0][1], 'mod': [lines[0][4]]}
	
	players = []
	subs = []
	victor = []
	factions = []
	actions = []
	
	for i in lines:
		if i[9] == '0' and i[6] not in victor:
			victor.append(i[6])
		if i[9] == '2':
			victor = ['Tie']
		
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
				elif i[8][0] == 'd':
					deathtype = 'Day Killed'
				else:
					print 'invalid death type'
					return None
		
		role = i[7] if i[7] != '' else 'Vanilla'
		
		if len(i) >= 16 and i[14] != '': # if there are subs for this slot
			op = i[14]
			if ',' in i[15]:
				print 'multiple subs in slot'
				return None
			subs.append({'op': i[14], 'subname': i[5], 'subday': int(i[15])})
		else:
			op = i[5]
		
		if len(i) >= 17: # NAs
			targets = TrimQuotes(i[16]).split(',')
			targets = [a.strip() for a in targets]
			for night, j in enumerate(targets):
				if j != '': # leading comma = no n0
					if j in [x['subname'] for x in subs]:
						target = subs[get_index(subs, 'subname', j)]['op']
					else:
						target = j
					actions.append({'actor': op, 'target': target, 'night': night, 'ability': 'Peek' if role.upper()=='SEER' else role})
		
		players.append({'op': op, 'faction': i[6], 'role': role, 'deathtype': deathtype, 'deathday': deathday})
	
	if len(factions) < 2:
		print "len(factions) < 2"
		return None
	
	gamejson['players'] = players
	gamejson['factions'] = factions
	gamejson['actions'] = actions
	gamejson['victor'] = victor
	
	return gamejson

data = [line.strip() for line in open('/home/iversonian/turbospreadsheet.txt')]
len(data)
del data[0] # header

data = [a.replace('\xa0', '') for a in data]

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

i = 0
summaries = []
for j in range(1, len(games)):
	if games[j][1] != games[j-1][1]:
		summaries.append(DoGame(games[i:j]))
		i = j
	else:
		pass

summaries.append(DoGame(games[i:j])) # get the last one

# check names against db:
players = []
for i in summaries:
	if i['mod'][0] not in players:
		players.append(i['mod'][0])
	for j in i['players']:
		if j['op'] not in players:
			players.append(j['op'])
	if 'subs' in i:
		for j in i['subs']:
			if j['subname'] not in players:
				players.append(j['subname'])
			if j['op'] not in players:
				players.append(j['op'])
	if 'actions' in i:
		for j in i['actions']:
			if j['target'] not in players:
				players.append(j['target'])
			if j['player'] not in players:
				players.append(j['player'])

notfound = []
for i in players:
	cursor.execute('select * from player where playername = %s', i)
	if cursor.rowcount != 1:
		notfound.append(i)

print notfound

for k, i in enumerate(summaries):
	if 'actions' in i:
		for j in i['actions']:
			if j['target'] in notfound:
				print i['gamename'] + ' ' + str(k) + ' ' + j['target']


summaries[0]['actions']
JSONtoDB(summaries[281], cursor, 'turbo ss 6')

for i in summaries:
	JSONtoDB(i, cursor, 'turbo ss prod 1')


# add these to db:
insert into player values(85804, 'Lardarse', 85804);
insert into player values(16700, 'Slow Play Ray', 16700);
insert into player values(8555, 'adanthar', 8555);
insert into player values(83680, 'Shine', 83680);
insert into player values(24230, 'spoohunter', 24230);
insert into player values(9836, 'caretaker1', 9836);
insert into player values(32712, 'Hawklet', 32712);
insert into player values(32359, 'yasher', 32359);
insert into player values(29413, 'onoble', 29413);
insert into player values(61642, 'AFH24', 61642);
insert into player values(65009, 'Turkish Mickey', 65009);
insert into player values(1000001, 'BradleyT', 1000001);
insert into player values(1000002, 'MEbenhoe', 1000002);
insert into player values(1000003, 'AWice', 1000003);
insert into player values(1000004, 'xorbie', 1000004);
insert into player values(1000005, 'Manimal', 1000005);
insert into player values(1000006, 'krupa-', 1000006);
insert into player values(1000007, 'Double Ice', 1000007);

