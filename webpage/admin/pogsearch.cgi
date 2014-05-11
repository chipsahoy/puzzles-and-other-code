#!/usr/bin/env python
# -*- coding: UTF-8 -*-

# enable debugging
import cgitb
import MySQLdb
import cgi, os, sys, string, time
#cgitb.enable()
#sys.stderr = sys.stdout 

db = MySQLdb.connect(host="localhost", user="pogwwdb", passwd="werewolf", db='fennecfox', charset="utf8", use_unicode=True)

cursor = db.cursor(MySQLdb.cursors.DictCursor)

##########################################################

def GenerateForm(f):
	op = f.getvalue('op')
	title = f.getvalue('title')
	if op is None:
		op = ''
	if title is None:
		title = ''
	#, Arrow, Exclamation, Smile, Question, Spade, Unhappy, Talking, Thumbs up, Post, Club, Cool, Wink, Diamond, Red face, Thumbs down, Angry
	icon = """<tr><td align="right">Icon:</td><td><select name="affiliation">
			<option value="Poll">Poll</option>
			<option value="Heart">Heart</option>
			<option value="Lightbulb">Lightbulb</option>"""
	form1 = """<br>
		<form method="post" action="/cgi-bin/pogsearch.cgi">
		<table width="100%%">
		<br>
		<tr><td align="right" width="5%%">OP:</td>
			<td><input type="text" name="op" size="20" value="%s" placeholder=""></td></tr>
		<tr><td align="right">Title:</td>
			<td><input type="text" name="title" size="20" value="%s"></td></tr>
		</td></tr>
		<tr><td></td><td><br>
			<input type="hidden" name="action" value="go">
			<input type="submit" name="query" value="Submit Query"><br><br>
		</td></tr>
		</table></form>""" % (op, title)
	print form1

def ShowResults(f):
	threadid = FindThread(f.getvalue("op"), f.getvalue("title"))
	print '<table width="1000"><tr><th align="left" width="15%">OP</th><th align="left"  width="55%">Thread Title</th><th align="left"  width="10%">Replies</th><th align="left"  width="10%">Icon</th><th align="left"  width="20%">Last Post</th></tr>'
	for i in threadid:
		cursor.execute("select t.replies, if(t.icontext is null,'',t.icontext) icontext, date(t.lastposttime) lastposttime, \
			t.url, convert(t.title using latin1) title, r.postername p \
			from Thread t join Poster r on t.op=r.posterid \
			where t.threadid=%s" % i)
		thread = cursor.fetchone()
		print '<tr><td>%s</td><td><a href="%s" target="_blank">%s</a></td><td>%s</td><td>%s</td><td>%s</td></tr>' % (thread['p'], thread['url'], 
			thread['title'], thread['replies'], thread['icontext'], thread['lastposttime'])
	print "</table>"

def FindThread(op, title):
	if len(op) > 0:
		qry = "select threadid from Thread t where op in (select posterid from Poster where postername like %s) and title like %s order by t.threadid desc"
		cursor.execute(qry, ('%'+str(op)+'%', '%'+str(title)+'%'))
	else:
		if title == '':
			print "No search terms entered"
			return None
		qry = "select threadid from Thread where title like %s order by threadid desc"
		cursor.execute(qry, ('%'+str(title)+'%'))
	threadid = []
	for i in range(cursor.rowcount):
		threadid.append(cursor.fetchone()['threadid'])
	
	#print "<br>" + str(threadid)
	return threadid


###################################################################

print "Content-Type: text/html;charset=utf-8"
print
print "<h3><ul>POG Thread Search</ul></h3>"

form = cgi.FieldStorage(keep_blank_values=True)

if "action" in form:
	GenerateForm(form)
	ShowResults(form)
else:
	GenerateForm(form)


