#!/usr/bin/env python
import stomp
import time
import dateutil.parser
import logging
import sys
import random
import MySQLdb
import json
import datetime

#connect to database and set up the cursor to accept commands
connection = MySQLdb.connect(host="mysql.checkmywwstats.com", user="pogwwdb", passwd="werewolf", db="fennecfox", charset="utf8", use_unicode=True)
cursor = connection.cursor(MySQLdb.cursors.DictCursor)

logging.basicConfig()

class MyListener(stomp.ConnectionListener):
	def on_error(self, jsonmessage, message):
		print('received an error %s' % message)
	def on_message(self, jsonmessage, message):
		for k,v in jsonmessage.iteritems():
			pass
#			print('header: key %s , value %s' %(k,v))
#		print('received message\n %s'% message)
		
		jsonmessage = json.loads(message)
#		print jsonmessage
		print('Post: %s %s %s %s' %(jsonmessage['PostId'], jsonmessage['ThreadId'], jsonmessage['PostNumber'], datetime.datetime.utcnow()))

		#Check to see if the poster is in the database, and get his/her posterid
		cursor.execute("SELECT posterid FROM Poster WHERE posterid = %s",int(jsonmessage['Poster']['Id']))
		#If not create the new poster and get their new id
		if(cursor.rowcount == 0):
			cursor.execute("INSERT INTO Poster (posterid, postername) VALUES (%s,%s)",(int(jsonmessage['Poster']['Id']),jsonmessage['Poster']['Name']))

		#Insert the post into the database
		cursor.execute("SELECT postid FROM post2 WHERE postid = %s",int(jsonmessage['PostId']))
		if(cursor.rowcount == 0):
			cursor.execute("INSERT INTO post2 (postid, postnumber, threadid, posterid, content, postTime, title) VALUES (%s,%s,%s,%s,%s,%s,%s)",(int(jsonmessage['PostId']), int(jsonmessage['PostNumber']),int(jsonmessage['ThreadId']), int(jsonmessage['Poster']['Id']), jsonmessage['Content'].encode('utf-8'), jsonmessage['Time'], jsonmessage['Title'].encode('utf-8')))
#		if(jsonmessage['Edit'] != None):
			#check to see if editor in database
#			cursor.execute("SELECT posterid FROM Player WHERE postername = %s",jsonmessage['Edit']['Who'])
			#If not create the new poster and get their new id
#			if(cursor.rowcount == 0):
#				cursor.execute("INSERT INTO Player (posterid, postername) VALUES (%s)",jsonmessage['Edit']['Who'])
#				cursor.execute("SELECT posterid FROM Player WHERE postername = %s",jsonmessage['Edit']['Who'])
#			result = cursor.fetchone()
#			edittime = dateutil.parser.parse(jsonmessage['Edit']['When'])
#			cursor.execute("SELECT * FROM post2 p, Edit, Poster WHERE p.postid = Edit.postid AND p.content=%s AND p.postid=%s AND Edit.reason=%s AND Edit.edittime=%s AND Edit.editor = Poster.posterid AND Poster.posterid=%s",(jsonmessage['Content'].encode('utf-8'), int(jsonmessage['PostId']), jsonmessage['Edit']['Reason'].encode('utf-8'), edittime, jsonmessage['Edit']['Who']))
#			if(cursor.rowcount == 0): #check to see if current edit stored
#				cursor.execute("SELECT content FROM post2 WHERE postid=%s", int(jsonmessage['PostId']))
#				result = cursor.fetchone()
#				if(result["content"] != jsonmessage['Content'].encode('utf-8')):
#					oldcontent = result["content"]
#				else:
#					oldcontent = None
#				cursor.execute("SELECT posterid FROM Poster WHERE postername=%s", jsonmessage['Edit']['Who'])
#				result = cursor.fetchone()
#				editor = int(result["posterid"])
#				cursor.execute("INSERT IGNORE INTO Edit (postid, oldcontent, editor, edittime, reason) VALUES (%s,%s,%s,%s,%s)",(int(jsonmessage['PostId']), oldcontent, editor, jsonmessage['Edit']['When'], jsonmessage['Edit']['Reason'].encode('utf-8')))
#			cursor.execute("UPDATE post2 SET content=%s WHERE postid=%s", (jsonmessage['Content'].encode('utf-8'), int(jsonmessage['PostId'])))
#		cursor.execute("UPDATE post2 SET postnumber=%s, content=%s, title=%s WHERE postid=%s",(int(jsonmessage['PostNumber']), jsonmessage['Content'].encode('utf-8'), jsonmessage['Title'].encode('utf-8'), int(jsonmessage['PostId'])))


#Queue
dest='fennecfox.posts'
#Set up server connection
conn=stomp.Connection([('mq.checkmywwstats.com',61613)])
print('set up Connection')
conn.set_listener('somename',MyListener())
print('Set up listener')

conn.start()
print('started connection')

conn.connect(wait=True)
print('connected')
conn.subscribe(destination=dest, ack='auto')
print('subscribed')

cursor.execute("set autocommit=1")

#wait forever
while(True):
	time.sleep(10)
print('slept')
conn.stop()
print('disconnected')
