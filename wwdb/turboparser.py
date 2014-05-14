# Vanilla game report parser
# Version 0.131
# iversonian

# version history
# 0.1 9.29.13
#	i am born
# 0.11 
#	allows one letter deathtypes
# 0.12 10.7.13
#	the deathtype & deathday order can be swapped
#	ok to use the threadid instead of the full url
#	added field in template for recording EOD post #'s, although it doesn't do anything with it yet
#	added template
# 0.13 11.21.13
#	tidied it up a bit. made adjustments for slight differences/errors in how actual users will input data
#	allow player list to have before/after sub
#	allow double spacing after "player list:"
#	updated to allow urls to not have a trailing "/"
# 0.131 12.17.13
#   bug fix, handling quotes


# to run script: $ python turboparser.py -i inputsummaryfile.txt -o outputscriptfile.sql

# x = open('turbos1.txt').read()
# v = ParseText(x)

######################################
# template
'''
Vanilla werewolf game summary
Report template version 0.13

Game name: 
Game type: turbo
url: 
Start date: 
Mod: 

Player list:

Subs:

Peeks: 

EOD:

Victor: 
'''

# sample
'''
Vanilla werewolf game summary
Report template version 0.13

Game name: 
Game type: turbo
url: 1376281
Start date: 
Mod: Krayz

Player list:
bhuber2010; v; n3 nk
Felix the Cat; v; d4 lynch
gambit8888; v; d3 l
iamnotawerewolf; v; d1 l
ihcjay; s; n1 nk
mexineil; w
Noah; w; d2 lynch
ReddBoiler; v
well named; v; n2 nk

Subs:

Peeks: bhuber2010

EOD:
n1 168
n2 248
n3 301
n4 345

Victor: wolves
'''

'''
User's Manual
version 0.12

Fill in the info, as given in the examples posted above.

Qualifying games: 9/13/17er vanilla/slow/turbo games.

Capitalization does not matter anywhere, even for player names.

Subs format: playerin; playerout; subday

Valid values:
Game type: {Vanilla, Slow Game, turbo}
Deathtype: {lynch, nk, survived, conceded, eaten, modkill, l, n, s, c, e, m}
Deathday: {d1, d2, ..., n1, n2, ..., 1, 2, ...}

A game summary can be complete without the following parts filled in:
game name, start date, mod, subs, peeks, EOD
The rest are mandatory.
'''
######################################

from optparse import OptionParser

def ParseGameSummary():
	parser = OptionParser()
	parser.add_option("-i", dest="inputfile", type="string", help="input txt file")
	parser.add_option("-o", dest="outputdir", type="string", help="Output SQL file")
	
	(options,args) = parser.parse_args()
	
	#open inputs and output
	inputFile = open(options.inputfile, "r")
	outputFile = open(options.outputdir, "w")

	text = inputFile.read().lower()
	text = text.replace("'", "''") # mysql escape for '
	
	numGames = 0
	# for each game in the input file
	while text.find('vanilla werewolf game summary') >= 0:
		gameSummary = ParseText(text[:text.find('end summary')])
		GenerateUploadScript(gameSummary, outputFile)
		text = text[text.find('end summary') + 11:]
		numGames += 1
		print gameSummary[2]
	
	print str(numGames) + ' games processed'

######################################

def GenerateUploadScript(v, output):
	# v: gamename, gametype, url, startdate, mod, players, subs, peeks, victor
	
	###### insert into Game
	gamename = v[0]
	if gamename == '':
		gamename = '(select title from fennecfox.Thread where threadid=' + v[2] + ')'
		
	# if startdate is missing, use OP timestamp
	startdate = v[3]
	if v[3] == '':
		startdate = '(select date(posttime) from fennecfox.Post where threadid=' + v[2] + ' and postnumber=1)'
	
	# find modid
	if v[4][0].strip() != '':
		modid = "(select posterid from fennecfox.Poster where postername='" + v[4][0].strip() + "')"
	else:
		modid = '(select posterid from fennecfox.Poster p join fennecfox.Thread t on t.op=p.posterid where threadid=' + v[2] + ' and postnumber=1)'
		
	#output.write("insert ignore into Thread (threadid, url, subforumid) values (" + threadid + ", '" + v[2] + "',59);\n")
	output.write("insert into Game (gamename,startdate,gametype,modid,threadid,gamelength,subforumid) values ('" + \
		gamename + "','" + startdate + "','" + v[1] + "'," + modid + "," + v[2] + "," + str(max(v[5][2])) + ",59);\n")
	
	###### insert into Team
	# add villager team
	output.write("insert into Team (affiliationid, affiliationnumber, victory, threadid) values (1,0," + \
		str(1 if v[8] == 'v' else 0) + "," + v[2] + ");\n")
	# add wolf team
	output.write("insert into Team (affiliationid, affiliationnumber, victory, threadid) values (2,0," + \
		str(1 if v[8] == 'w' else 0) + "," + v[2] + ");\n")
	
	###### insert into GameRole & Player
	villagerTeamId = "(select teamid from Team where affiliationid = 1 and threadid =" + v[2] + ")"
	wolfTeamId = "(select teamid from Team where affiliationid = 2 and threadid =" + v[2] + ")"
	
	sub_idx = 0
		
	for i in range(0, len(v[5][0])):
		if v[5][3][i] == 'e':
			deathtypeid = 1
		elif v[5][3][i] == 'n':
			deathtypeid = 2
		elif v[5][3][i] == 's':
			deathtypeid = 3
		elif v[5][3][i] == 'l':
			deathtypeid = 4
		elif v[5][3][i] == 'm':
			deathtypeid = 5
		elif v[5][3][i] == 'c':
			deathtypeid = 7
		else:
			print 'invalid deathtype' + ' ' + v[5][3][i] + ' ' + str(i) + '\n' + str(v)
			exit()
	
		output.write("insert into GameRole (teamid, deathtypeid, deathday, roletypeid) values (" + \
			(villagerTeamId if v[5][1][i] in ('v','s') else wolfTeamId) + "," + str(deathtypeid) + "," + \
			str(v[5][2][i]) + "," + str(1 if v[5][1][i] != 's' else 18) + ");\n")
		
		output.write("insert into Player (roleid, posterid, startday) VALUES ((select max(roleid) from GameRole), " + \
			"(select posterid from Poster where postername = '" + v[5][0][i] + "'), 0);\n")
			
	##### subs
	
	for i in range(0, len(v[6])):
		subout = v[6][i][1]
		subin = v[6][i][0]
		# get the affiliation of the guy who subbed out
		teamid = wolfTeamId if v[5][1][v[5][0].index(subout)] == 'w' else villagerTeamId	
		
		roleid = "(select max(roleid) from Poster r join Player p using (posterid) join GameRole g using (roleid) join Team t using (teamid) " + \
			"where t.teamid=" + teamid + " and r.postername = '" + subout + "')"
		
		output.write("insert into Player (roleid, posterid, startday) VALUES (" + roleid + \
			", (select posterid from Poster where postername = '" + subin + "')," + str(v[6][i][2]) + ");\n")
	
		# append the sub to the playerlist in case he's later subbed out
		
		v[5][0].append(subin)
		v[5][1].append(v[5][1][v[5][0].index(subout)])
	
		output.write("UPDATE Player SET startday = " + str(v[6][i][2]) + " WHERE roleid=" + roleid + \
			" AND posterid=(SELECT posterid FROM Poster WHERE postername = '" + subin + "');\n")
		output.write("UPDATE Player SET endday = " + str(v[6][i][2]) + " WHERE roleid=" + roleid + \
			" AND posterid=(SELECT posterid FROM Poster WHERE postername = '" + subout + "');\n")

######################################

def ParseText(x):
	idx = x.find('game name:')
	gamename = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	
	idx = x.find('game type:')
	gametype = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	
	# strip out the threadid from url
	idx = x.find('url:')
	url = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	
	if not url.isdigit():
		url = url[url.rindex('-')+1:]
		if url[-1] == '/':
			url = url[:-1]
		if not url.isdigit():
			print 'ur url is all like, messed up and stuff'
			exit()
		
	idx = x.find('start date:')
	if idx > 0:
		startdate = x[x.find(':', idx)+1:x.find('\n', idx)].strip()
	else:
		startdate = ''
	
	idx = x.find('mod:')
	mod = x[x.find(':', idx)+1:x.find('\n', idx)].strip().split(';')
	
	idx = x.find('player list:')
	playerdeaths = x[x.find(':', idx)+1:x.find('\n\n', idx+13)].strip().split('\n') # allow for double space after "player list:"
	playerdeaths = [a.strip() for a in playerdeaths]
	if len(playerdeaths) not in (9,13,17):
		print playerdeaths
		print "Report is not a properly formatted summary for a 9/13/17er game\n"
	
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
	
	death = [a[2] if len(a)==3 else 'x0 didnotdie' for a in playerdeaths]
	death = ['x0 didnotdie' if a[0] in ('c','s','e') else a for a in death]
	death = [a.split(' ') for a in death]
	
	deathday = [int(a[0][1:]) for a in death]
	deathday = [max(deathday) if a == 0 else a for a in deathday]
	deathtype = [a[1] for a in death]
	
	# either way is ok
	for i in range(0,len(deathtype)):
		if deathtype[i] == 'didnotdie':
			if affiliation[i] == 'w':
				deathtype[i] = 's' if victor=='w' else 'c'
			else:
				deathtype[i] = 's' if victor=='v' else 'e'
	
	deathtype = [a[0] for a in deathtype]
	deathtype = [('n' if a == 'k' else a) for a in deathtype]
		
	## parse subs
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
	
	else:
		subs = ''
	
	players = [playername, affiliation, deathday, deathtype]
	
	return(gamename, gametype, url, startdate, mod, players, subs, peeks, victor)

######################################
import re

re.match("^[A-Za-z0-9_-]*$", 'testest')

# run main
ParseGameSummary()

