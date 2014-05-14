#!/usr/bin/env python
# -*- coding: UTF-8 -*-

# enable debugging
import cgitb
import cgi, os, sys, string, time, datetime
cgitb.enable()
sys.stderr = sys.stdout 

#import MySQLdb
#db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)
import login

cursor = login.db.cursor(login.MySQLdb.cursors.DictCursor)
cursor.execute("set autocommit=0")

import textjsondb

#TOPDIRECTORY = 'pog'
#PASSWORDS = []

##########################################################

def ValidDate(date_text):
	try:
		datetime.datetime.strptime(date_text, '%Y-%m-%d')
	except ValueError:
		return False
	return True

def PlayerInDB(playername):
	cursor.execute("select playername from player where playername = %s", playername)
	if cursor.rowcount == 0:
		return [False, playername]
	else:
		return [True, cursor.fetchone()['playername']] # to get capitalization the same as db

def makeRadio(name, values, selectedValue=None):
	OPT = '<input type="radio" name="{0}"{2} value="{1}">{1}\n'
	return ''.join(OPT.format(name, v, " checked" if v==selectedValue else "") for v in values)

def makeSelect(name, values, selectedValue=None):
	SEL = '<select name="{0}">\n{1}</select>\n'
	OPT = '<option value="{0}"{1}>{2}</option>\n'
	return SEL.format(name, ''.join(OPT.format(values[i], " SELECTED" if values[i]==selectedValue else "", values[i]) for i in range(len(values))))

def ListSubs(subs, playerlist):
	# subs is list of dicts: {'op', 'subname', 'subday'}
	formsub = ''
	if len(subs)>0:
		formsub = '<tr><td align="right">Subs:</td><td><table bgcolor=f9ecb6><tr><th>Original Player</th><th>Sub Name</th><th>Sub Day</th></tr>'
		for s in subs:
			formsub += """<tr><td>%s</td><td><input type="text" name="subname" size="15" value="%s"></td>
				<td>%s</td></tr>""" % (
				makeSelect('subop', ['']+playerlist, s['op']), s['subname'], makeSelect('subday', range(1,12), s['subday']))
		formsub += '</table></tr>'
	formsub += '<tr><td></td><td>'
	if len(subs) > 0:
		formsub += 'To remove entry, set the Original Player field to blank. Hit "Check for validity" to refresh.<br>'
	formsub += '<input type="submit" name="addsub" value="Add Sub"><br><br></td></tr>'
	return formsub

def GetActorOrSeer(playerlist, role, actor):
	# if actor is already set, use him, else return first seer in playerlist, for convenience
	if actor == '':
		if 'SEER' in [x.upper() for x in role]:
			return playerlist[[y.upper() for y in role].index('SEER')]
	else:
		return actor
	
def ListActions(actions, playerlist, role):
	# actions is list of dicts: {'actor','target','night','ability'}
	formaction = ''
	if len(actions) > 0:
		formaction = '<tr><td align="right">Actions:</td><td><table bgcolor=efe1f1><tr><th>Actor</th><th>Target</th><th>Night</th><th>Ability</th></tr>'
		for a in actions:
			formaction += """<tr><td>%s</td><td>%s</td><td>%s</td><td>%s</td></tr>""" % (
				makeSelect('actor', ['']+playerlist, GetActorOrSeer(playerlist, role, a['actor'])),
				makeSelect('target', ['']+playerlist, a['target']),
				makeSelect('night', [str(x) for x in range(10)], a['night']),
				makeSelect('ability', ['Peek','Angel','Vig','Roleblock'], a['ability']))
		formaction += '</table></tr>'
	formaction += '<tr><td></td><td>'
	if len(actions) > 0:
		formaction += 'To remove entry, set the Actor field to blank. Hit "Check for validity" to refresh.<br>'
	formaction += '<input type="submit" name="addaction" value="Add Action"><br><br></td></tr>'
	return formaction

# '<td></td>' % deathday[i] + \
def ListPlayers(playerlist, affiliation, role, deathday, deathtype):
	if len(playerlist)==0:
		return ''
	else:
		formpl = "<tr><td align='right'>Death Table:</td><td><table bgcolor=#dcedcf><tr><td></td><th>Name</th><th>Affiliation</th>\
			<th>Deathday</th><th>Deathtype</th><th>Role</th></tr>"
		for i, player in enumerate(playerlist):
			formpl += "<tr><td>" + str(i+1) + ".</td><td>" + '<input type="text" name="playerlist" size="15" value="' + player + '"></td><td>' + \
			makeSelect('affiliation', ['Village','Wolves']+['Wolves'+str(f) for f in range(2,6)]+
				['Neutral']+['Neutral'+str(f) for f in range(2,9)], affiliation[i]) + '</td>' + \
			'<td><input type="text" name="deathday" size="5" value="%s">' % deathday[i] + '</td><td>' + \
			makeSelect('deathtype', ['','Lynched','Night Killed','Survived','Eaten','Conceded','Mod Killed','Day Killed'], deathtype[i]) + '</td>' + \
			'<td><input type="text" name="role" size="15" value="%s"></td></tr>' % ('' if role[i] == 'Vanilla' else role[i])
		formpl += "</table></td></tr>"
		return formpl

def VictorType(victor):
	if len(victor) == 0:
		return ''
	elif len(victor) == 1 and victor[0] in ('Village','Wolves','Tie',''):
		return victor[0]
	else:
		return 'Other/Multiple:'

def GenerateForm(game):
	print "<font color='red'>"
	for m in game.errmsg:
		print str(m) + "<br>"
	print "</font>"
	form1 = """<br>
		<form method="post" action="submitgame.cgi">
		<table width="100%%">
		<tr><td><input type="submit" name="clear" value="Clear Form"></td></tr>
		<tr><td align="right" width="10%%">URL:</td>
			<td><input type="text" name="url" size="50" value="%s"><input type="submit" name="retrievegame" value="Retrieve Game"></td></tr>
		<tr><td align="right">Game Name:</td>
			<td><input type="text" name="gamename" size="50" value="%s"></td></tr>
		<tr><td align="right">Mod(s):</td>
			<td><input type="text" name="mods" size="25" value="%s" placeholder="Separate names with a semicolon"></td></tr>
		<tr><td align="right">Start Date:</td>
			<td><input type="text" name="startdate" size="25" placeholder="YYYY-MM-DD" value="%s"></td></tr>
		<tr><td align="right">Game Type:</td><td>
		%s</td></tr>
		<tr><td align='right'>Victor:</td><td>%s<input type="text" name="victortxt" size="15" value="%s"></td>
		</tr>
		<tr><td align="right">Player list:</td>
			<td><table><tr><td><textarea name="playerlisttext" rows="9" cols="20">%s</textarea></td>
			<td>The player list should include only the <u>original</u> players<br> that occupied each player slot. Subs are noted separately.<br><br>
			<input type="submit" name="deathtable" value="Build death table with this player list"></td></tr></table></tr>
		""" % (game.url, game.gamename, '; '.join(game.mods), game.startdate, 
			makeRadio('gametype', ['Vanilla','Slow Game','Vanilla+','Mish-Mash','Turbo'], game.gametype), 
			makeSelect('victordrop', ['','Village','Wolves','Tie','Other/Multiple:'], VictorType(game.victor)), 
			'' if len(game.victor)==1 and game.victor[0] in ('Village','Wolves','Tie','') else '; '.join(game.victor),
			'\n'.join(game.playerlisttext))
	
	players = ListPlayers(game.playerlist, game.affiliation, game.role, game.deathday, game.deathtype) if (len(game.playerlist) > 0) else ''
	
	form2 = '<tr><td></td><td><input type="submit" name="filldt" value="Fill in death table with implied values"></td></tr>'
			
	subs = ListSubs(game.subs, game.playerlist)
	actions = ListActions(game.actions, game.playerlist, game.role)
	
	form3 = """<tr><td></td><td><br><br>
			<input type="hidden" name="action" value="go">
			<input type="submit" name="checkgame" value="Check for validity"><br><br>
			<input type="hidden" name="delete" value="Delete game from database"><br><br>
			<input type="text" name="commit" size="20" value="" placeholder="Commit message"><br>
			<input type="hidden" name="savegame" value="Save Work">
			<input type="submit" name="sendgame" value="Submit Game"></td></tr>
			</table></form><br><br>"""
		
	instructions = """How to use this form:<p>
		1. Enter the URL of the game thread, then hit "Retrieve Game". Make sure that the url is in the correct form (url of page 1 of the thread).<br>
		2. If the game already exists in the database, it will display. Otherwise, you start from scratch.<br>
		3. After entering the player list, hit the "Build" button next to it. This will build the death table<br>
		4. Set fields as appropriate. Leave "role" blank for vanillas. You may leave deathday/type blank if the player survived, was eaten, or conceded, and hit the "Fill in death table with implied values".<br>
		5. If there are subs in the game, hit "Add Sub" to add fields.<br>
		6. If there are night actions, hit "Add Actions" to add fields.<br>
		7. Hit "Check for validity" to see if there are any issues with your game summary.<br>
		8. If there are no error messages, enter a commit message (e.g. your name) and hit Submit. The message is to avoid duplicate submissions and to help with debugging.<br>
		9. Follow the link to the url of the game page in the database. Check to ensure it is correct. You can simply overwrite the game with a new submission to correct errors.<br>
		<br><br>"""
	print form1 + players + form2 + subs + actions + form3 + instructions

#####################################################################

class Game:
	""" defines a werewolf game """
	def __init__(self):
		self.errmsg = []
		self.url=''
		self.gamename=''
		self.startdate=''
		self.commit=''
		self.gametype=None
		self.playerlisttext=''
		self.playerlist=[]
		self.affiliation=[]
		self.role=[]
		self.deathday=[]
		self.deathtype=[]
		self.mods=[]
		self.victor=[]
		self.subs = []
		self.actions = []
	
	def ImportFromForm(self, f):
		for fieldname in ('url','gamename','startdate'):
			setattr(self, fieldname, f[fieldname].value)
		self.gametype = f.getvalue('gametype')
		self.SetVictor(f.getvalue('victordrop'), f.getvalue('victortxt'))
		self.mods = [a.strip() for a in f.getvalue('mods').split(';')] if f.getvalue('mods') != '' else []
		self.playerlist = [a.strip() for a in f.getvalue('playerlisttext').strip().split('\n')] if f.getvalue('playerlist') != '' else []
		self.playerlist = f.getlist("playerlist")
		self.affiliation = f.getlist("affiliation")
		self.role = f.getlist("role")
		self.deathday = f.getlist("deathday")
		self.deathtype = f.getlist("deathtype")
		if 'subop' in f:
			subop = f.getlist("subop")
			subname = f.getlist("subname")
			subday = f.getlist("subday")
			self.subs = []
			for i, s in enumerate(subop):
				self.subs.append({'op':subop[i], 'subname':subname[i], 'subday':subday[i]})
		if 'actor' in f:
			actor = f.getlist("actor")
			target = f.getlist("target")
			night = f.getlist("night")
			ability = f.getlist("ability")
			self.actions = []
			for i, a in enumerate(actor):
				self.actions.append({'actor':actor[i], 'target':target[i], 'night':night[i], 'ability':ability[i]})
	
	def SetVictor(self, vd, vt):
		if len(vd) == 0:
			self.victor = []
		elif vd in ('Village','Wolves','Tie'):
			self.victor = [vd]
		else:
			self.victor = [a.strip() for a in vt.split(';')] if vt != '' else []
	
	def CheckPlayerlistAgainstDB(self):
		for i, p in enumerate(self.playerlist):
			dbcheck = PlayerInDB(p)
			self.playerlist[i] = dbcheck[1]
			if dbcheck[0] == False:
				self.errmsg.append("<font color='red'>Player " + p + " is not in the database.</font>")
	
	def RebuildPlayerlist(self, newplstr):
		""" Playerlist has changed. Update the (size of the) rest of the variables to reflect that. """
		self.playerlist = [a.strip() for a in newplstr.strip().split('\n')]
		self.affiliation = ['Village']*len(self.playerlist)
		self.role = ['Vanilla']*len(self.playerlist)
		self.deathday = ['']*len(self.playerlist)
		self.deathtype = ['']*len(self.playerlist)
		self.subs = []
		self.action = []
		self.CheckPlayerlistAgainstDB()
	
	def AddSub(self):
		self.subs.append({'op':'', 'subname':'', 'subday':''})
	
	def AddAction(self):
		if len(self.actions) >= 1:
			night = str(int(self.actions[-1]['night'])+1)
		else:
			night = ''
		self.actions.append({'actor':'', 'target':'', 'night':night, 'ability':''})
		
	def FillInDeathTable(self):
	# infer missing deathtype/deathday values
		if self.victor == []:
			self.errmsg.append("<font color='red'>Victor is a required field.</font>")
		else:
			for i in range(len(self.deathday)):
				if self.deathday[i] == '':
					self.deathday[i] = max([0 if a=='' else a for a in self.deathday])
				if self.deathtype[i] == '': # fill in s/e/c deathtypes
					if self.affiliation[i].upper() in [a.upper() for a in self.victor]:
						self.deathtype[i] = 'Survived'
					else:
						if self.affiliation[i] == 'Village':
							self.deathtype[i] = 'Eaten'
						else:
							self.deathtype[i] = 'Conceded'				
	
	def CheckValidity(self):
		# missing values:
		if self.gamename == '':
			self.errmsg.append("<font color='red'>Game Name is a required field.</font>")
		if self.mods == []:
			self.errmsg.append("<font color='red'>Mod(s) is a required field.</font>")
		if not ValidDate(self.startdate):
			self.errmsg.append("<font color='red'>Start Date is not a valid date.</font>")
		if self.gametype == None:
			self.errmsg.append("<font color='red'>Game Type not set.</font>")
		if self.victor == []:
			self.errmsg.append("<font color='red'>Victor(s) is a required field.</font>")
		
		if len(self.victor) > 0:
			factions = [a.upper() for a in set(self.affiliation)]
			for v in self.victor:
				if v.upper() not in factions:
					self.errmsg.append("<font color='red'>The winning team '%s' is not in the role set.</font>" %v)
		
		# within the playerlist
		if len(self.playerlist) < 3:
			self.errmsg.append("<font color='red'>Game summary is incomplete.</font>")
		else:
			factions = set(self.affiliation)
			if 'Village' not in factions:
				self.errmsg.append('Game requires a village faction.')
			if 'Wolves' not in factions:
				self.errmsg.append('Game requires at least one wolf faction.')

		self.CheckPlayerlistAgainstDB()
		
		for i, modname in enumerate(self.mods):
			dbcheck = PlayerInDB(modname)
			self.mods[i] = dbcheck[1]
			
			if dbcheck[0] == False:
				self.errmsg.append("<font color='red'>Moderator " + modname + " is not in the database.</font>")
		
		# get rid of [blank] sub/actor fields
		self.subs = filter(lambda a: a['op'] != '', self.subs)
		self.actions = filter(lambda a: a['actor'] != '', self.actions)
		
		#if self.gametype in ('Vanilla','Slow Game','Turbo'):
		#	self.FillInDeathTable()
		
		# incomplete death table
		if sum([a == '' for a in self.deathday]) > 0 or sum([a == '' for a in self.deathtype]) > 0:
			self.errmsg.append("<font color='red'>Death table is incomplete.</font>")
		
		# check for valid role names
		for i in self.role:
			if i not in ('','Vanilla'):
				cursor.execute("select * from roles where rolename = %s", i)
				if cursor.rowcount == 0:
					self.errmsg.append("The role '%s' does not exist in the database. Add it using the control panel." % i)
		
		# check if this game is a valid vanilla setup
		#if self.gametype in ('Vanilla','Slow Game','Turbo'):
			#pass
		
		return len(self.errmsg) == 0
	
	def ExportToJSON(self):
		j = {'url':self.url, 'gamename':self.gamename, 'gametype':self.gametype, 'startdate':self.startdate, 'mod':self.mods, 'victor':self.victor, 'factions':list(set(self.affiliation))}
		players = []
		for i, op in enumerate(self.playerlist):
			p = {'op':op, 'faction':self.affiliation[i], 'role': 'Vanilla' if self.role[i] == '' else self.role[i], 'deathday':self.deathday[i], 'deathtype':self.deathtype[i]}
			players.append(p)
		j['players'] = players
		
		subs = []
		for s in self.subs:
			subs.append({'op':s['op'], 'subname':s['subname'], 'subday':s['subday']})
		if len(subs) > 0:
			j['subs'] = subs
		
		actions = []
		for a in self.actions:
			actions.append({'actor':a['actor'], 'target':a['target'], 'night':a['night'], 'ability':a['ability']})
		if len(actions) > 0:
			j['actions'] = actions
		
		return j

	def ImportFromJSON(self, j):
		return True

def RetrieveGame(cur, url):
	""" Retrieve game data from db: url->Game """
	j = textjsondb.DBtoJSON(url, cur)
	
	if j is None: # show thread title/op(mod) at least, if we have it, then bail
		gameobj = Game()
		gameobj.url = url
		gameobj.errmsg.append("<font color='red'>Game is not yet entered in the database.</font>")
		cur.execute("select t.title, playername as moderator, ifnull(date(o.posttime),'') startdate \
			from fennecfox.Thread t join player p on p.playerid=t.op \
			left join fennecfox.Post o on o.threadid=t.threadid and o.postnumber=1 \
			where t.url=%s", url)
		if cur.rowcount > 0:
			thread = cur.fetchone()
			gameobj.gamename = thread['title']
			gameobj.mods = [thread['moderator']]
			gameobj.startdate = thread['startdate']
		return gameobj
	else:
		return JSONtoGame(j)

def JSONtoGame(j):
	# assuming we have a valid game defined in json, fill in the Game class object
	g = Game()
	g.url = j['url']
	g.startdate = j['startdate']
	g.gamename = j['gamename']
	g.gametype = j['gametype']
	g.mods = j['mod']
	g.victor = j['victor']
	playerlist = []
	subs = []
	affiliation = []
	role = []
	deathday = []
	deathtype = []
	for p in j['players']:
		playerlist.append(p['op'])
		affiliation.append(p['faction'])
		role.append(p['role'])
		deathday.append(p['deathday'])
		deathtype.append(p['deathtype'])
	
	if 'subs' in j:
		for sub in j['subs']:
			subs.append({'op':sub['op'], 'subname':sub['subname'], 'subday':str(sub['subday'])})
	
	g.playerlist = playerlist
	g.subs = subs
	g.affiliation = affiliation
	g.role = role
	g.deathday = deathday
	g.deathtype = deathtype
	
	return g

###################################################################

print "Content-Type: text/html;charset=utf-8"
print
print "<head><title>WWDB Game Entry</title></head>"
print """<script type="text/javascript"> 
	function stopRKey(evt) { 
	var evt = (evt) ? evt : ((event) ? event : null); 
	var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null); 
	if ((evt.keyCode == 13) && (node.type=="text"))  {return false;} }
	document.onkeypress = stopRKey;
	</script>"""
print "<h3><ul>Werewolf Database Game Submission/Edit Form (beta)</ul></h3>"
#print '<font face="courier new">'

form = cgi.FieldStorage(keep_blank_values=True)
game = Game()

if "action" in form:
	game.ImportFromForm(form)
else:
	game = Game()

if "sendgame" in form:
	j = game.ExportToJSON()
	if len(form.getvalue('commit')) < 1:
		print "<font color='red'>Enter a commit message.</font><br>"
	else:
		commitmsg = form.getvalue('commit').strip()
		if game.CheckValidity():
			cursor.execute('rollback')
			#cursor.execute("SET GLOBAL general_log = 'ON'")
			cursor.execute("select * from editslog where url=%s and message=%s", (game.url, commitmsg))
			if cursor.rowcount > 0:
				print "<font color='red'>Use a different commit message.</font><br>"
			else:
				result = textjsondb.JSONtoDB(j, cursor, commitmsg)
				cursor.execute("select gameid from game where url=%s", game.url)
				if result == 1 and cursor.rowcount == 1:
					print "<font color='blue'><br>Game uploaded successfully.</font><br> \
						<a href='../index.php?report=Game&gameid=%s' target='_blank'>Link to game in database</a><br>" % (cursor.fetchone()['gameid'])
				else:
					print "<font color='red'><br>Game was not uploaded successfully. Please send details to iversonian for debugging.</font><br>"
					#cursor.execute("SET GLOBAL general_log = 'OFF'")
	GenerateForm(game)
	print "For debugging:<br>" + str(j)
elif "retrievegame" in form:
	game = RetrieveGame(cursor, form['url'].value)
	GenerateForm(game)
elif "deathtable" in form:
	# rebuild playerlist/roles/etc
	game.RebuildPlayerlist(form.getvalue("playerlisttext"))
	GenerateForm(game)
elif "addsub" in form:
	game.AddSub()
	GenerateForm(game)
elif "addaction" in form:
	game.AddAction()
	GenerateForm(game)
elif "filldt" in form:
	if game.gametype in ('Vanilla','Slow Game','Turbo'):
		game.FillInDeathTable()
	else:
		print "<font color='red'>That function is only applicable for vanilla games.</font><br>"
	GenerateForm(game)
elif "checkgame" in form:
	if game.CheckValidity():
		print "<font color='blue'>Looks good</font><br>"
	GenerateForm(game)
	j = game.ExportToJSON()
	print "For debugging:<br>" + str(j)
elif "clear" in form:
	GenerateForm(Game())
else:
	GenerateForm(game)

