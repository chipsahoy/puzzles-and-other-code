#!/usr/bin/env python
# -*- coding: UTF-8 -*-

# enable debugging
import cgitb
import MySQLdb
import cgi, os, sys, string, time
#cgitb.enable()
#sys.stderr = sys.stdout 

db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='fennecfox', charset="utf8", use_unicode=True)

cursor = db.cursor(MySQLdb.cursors.DictCursor)

##########################################################

def GenerateForm(f):
	op = f.getvalue('op')
	title = f.getvalue('title')
	replies1 = f.getvalue('replies1')
	replies2 = f.getvalue('replies2')
	
	if op is None:
		op = ''
	if title is None:
		title = ''
	if replies1 is None:
		replies1 = ''
	if replies2 is None:
		replies2 = ''
	
	#, Arrow, Exclamation, Smile, Question, Spade, Unhappy, Talking, Thumbs up, Post, Club, Cool, Wink, Diamond, Red face, Thumbs down, Angry
	icon = """<tr><td align="right">Icon:</td><td><select name="affiliation">
			<option value="Poll">Poll</option>
			<option value="Heart">Heart</option>
			<option value="Lightbulb">Lightbulb</option>"""
	form1 = """<br>
		<form method="post" action="turbothreads.cgi">
		<table width="100%%">
		<br>
		<tr><td align="right" width="10%%">OP:</td>
			<td><input type="text" name="op" size="20" value="%s" placeholder=""></td></tr>
		<tr><td align="right">Title contains:</td>
			<td><input type="text" name="title" size="20" value="%s"></td></tr>
		<tr><td align="right"></td>
			<td>Between<input type="text" name="replies1" size="5" value="%s">and<input type="text" name="replies2" size="5" value="%s">posts</td></tr>

		<tr><td></td><td><br>
			<input type="hidden" name="action" value="go">
			<input type="submit" name="query" value="Submit Query"><br><br>
		</td></tr>
		</table></form>""" % (op, title, replies1, replies2)
	print form1

def ShowResults(f):
	threadid = FindThread(f.getvalue("op"), f.getvalue("title"), f.getvalue("replies1"), f.getvalue("replies2"))
	print '<table width="1200"><tr><th></th><th align="left" width="15%">OP</th></th><th align="left"  width="45%">Thread Title</th> \
		<th align="left"  width="10%">Replies</th><th align="left"  width="10%">Icon</th><th align="left" width="20%">First Post</th></tr>'
	for j, i in enumerate(threadid):
		cursor.execute("select t.replies, if(t.icontext is null,'',t.icontext) icontext, date(t.lastposttime) lastposttime, \
			t.url, convert(t.title using latin1) title, r.postername p, ifnull(date(p.posttime),'') optime \
			from Thread t join Poster r on t.op=r.posterid \
			left join threadop p on p.threadid=t.threadid \
			where t.threadid=%s" % i)
		thread = cursor.fetchone()
		print '<tr><td>%s</td><td>%s</td><td><a href="%s" target="_blank">%s</a></td><td>%s</td><td>%s</td><td>%s</td></tr>' % (
			j+1, thread['p'], thread['url'], thread['title'], thread['replies'], thread['icontext'], thread['optime'])
	print "</table>"

def FindThread(op, title, replies1, replies2):
	if replies1 is None or replies1=='':
		replies1 = 0
	if replies2 is None or replies2=='':
		replies2 = 999999
	
	if len(op) > 0:
		cursor.execute("select threadid from Thread t where op in (select posterid from Poster where instr(postername,  %s)) \
			and instr(title, %s) and url not in (select url from pog.game) and replies between %s and %s and op not in (0) \
			order by t.threadid desc", (op, title, replies1, replies2))
	else:
		if title == '':
			print "No search terms entered"
			return None
		cursor.execute("select threadid from Thread where instr(title, %s) and op not in (0) and replies between %s and %s \
			and url not in (select url from pog.game) order by threadid desc", (title, replies1, replies2))
	threadid = []
	for i in range(cursor.rowcount):
		threadid.append(cursor.fetchone()['threadid'])
	
	#print "<br>" + str(threadid)
	return threadid


###################################################################

print "Content-Type: text/html;charset=utf-8"
print
print "<h3><ul>POG Turbo Threads Search Tool</ul></h3><br>Shows POG threads meeting the criteria which are not yet included in the werewolf database."

form = cgi.FieldStorage(keep_blank_values=True)

if "action" in form:
	GenerateForm(form)
	ShowResults(form)
else:
	GenerateForm(form)


