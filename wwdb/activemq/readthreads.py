#!/usr/bin/env python
import stomp
import time
import logging
import sys
import random
import MySQLdb
import json

#connect to database and set up the cursor to accept commands
#connection = MySQLdb.connect(host="mysql.checkmywwstats.com", user="pogwwdb", passwd="werewolf", db="fennecfox", charset="utf8", use_unicode=True)
connection = MySQLdb.connect(host="127.0.0.1", port=3307, user="pogwwdb", passwd="werewolf", db="fennecfox")
cursor = connection.cursor(MySQLdb.cursors.DictCursor)
cursor.execute("SET time_zone = '+00:00'")
logging.basicConfig()

connection.set_character_set('utf8')
cursor.execute('SET NAMES utf8;')
cursor.execute('SET CHARACTER SET utf8;')
cursor.execute('SET character_set_connection=utf8;')

# test con
cursor.execute('SELECT * from Post limit 1')

class MyListener(stomp.ConnectionListener):
	def on_error(self, headers, message):
		print('received an error %s' % message)
	def on_message(self, headers, message):
		for k,v in headers.iteritems():
			print('header: key %s , value %s' %(k,v))
		print('received message\n %s'% message)
		
		jsonmessage = json.loads(message)
		print jsonmessage
		#Check for a moved thread.
		if(jsonmessage['Views'] == 0):
			return
		
		#Check to see if the OP is in the database, and get his/her posterid
		cursor.execute("SELECT posterid FROM Poster WHERE posterid = %s", int(jsonmessage['OP']['Id']))
		#If not create the new poster and get their new id
		if(cursor.rowcount == 0):
			cursor.execute("INSERT INTO Poster (posterid, postername) VALUES (%s, %s)",
				(int(jsonmessage['OP']['Id']), jsonmessage['OP']['Name']))
		
		#Check to see if the lastposted is in the database, and get his/her posterid
		cursor.execute("SELECT posterid FROM Poster WHERE posterid= %s",int(jsonmessage['LastPoster']['Id']))
		#If not create the new poster and get their new id
		if(cursor.rowcount == 0):
			cursor.execute("INSERT INTO Poster (posterid, postername) VALUES (%s, %s)",
				(int(jsonmessage['LastPoster']['Id']), jsonmessage['LastPoster']['Name']))
		
		#initialize
		readThread=False
		posts = 0
		
		#Check to see if the thread is already in the database
		cursor.execute("SELECT * FROM Thread WHERE threadid = %s",jsonmessage['ThreadId'])
		
		#If not, create it and flag for reading
		if(cursor.rowcount == 0):
			readThread = True
			if 'ThreadIconText' in jsonmessage:
				cursor.execute("INSERT INTO Thread (threadid, OP, views, replies, URL, lastPoster, lastPostTime, title, iconText) VALUES (%s,%s,%s,%s,%s,%s,%s,%s,%s)",
					(int(jsonmessage['ThreadId']),jsonmessage['OP']['Id'],int(jsonmessage['Views']),int(jsonmessage['ReplyCount']),jsonmessage['URL'],jsonmessage['LastPoster']['Id'],jsonmessage['LastPostTime'],jsonmessage['Title'],jsonmessage['ThreadIconText']))
			else:
				cursor.execute("INSERT INTO Thread (threadid, OP, views, replies, URL, lastPoster, lastPostTime, title) VALUES (%s,%s,%s,%s,%s,%s,%s,%s)",
					(int(jsonmessage['ThreadId']),jsonmessage['OP']['Id'],int(jsonmessage['Views']),int(jsonmessage['ReplyCount']),jsonmessage['URL'],jsonmessage['LastPoster']['Id'],jsonmessage['LastPostTime'],jsonmessage['Title']))
		
		else:
			#check if thread replies on forum = # of replies in database, if not ask to read thread from last post in database
			row = cursor.fetchone()
			if(int(row['views']) != int(jsonmessage['Views'])):
				cursor.execute("UPDATE Thread SET views=%s WHERE threadid=%s", 
					(int(jsonmessage['Views']), int(jsonmessage['ThreadId'])))
			if(int(row['lastposttime']) != int(jsonmessage['LastPostTime'])):
				cursor.execute("UPDATE Thread SET lastposttime='%s' WHERE threadid='%s'", 
					(int(jsonmessage['LastPostTime']), int(jsonmessage['ThreadId'])))
			if(int(row['replies']) != int(jsonmessage['ReplyCount'])):
				cursor.execute("UPDATE Thread SET replies=%s, lastPoster=%s, lastPostTime=%s WHERE threadid=%s",(int(jsonmessage['ReplyCount']), jsonmessage['LastPoster']['Id'], jsonmessage['LastPostTime'], int(jsonmessage['ThreadId'])))
				readThread=True
				posts = int(row['replies']) + 1
		
			#find all posts in the thread less than 30 minutes old to check for edits!
			# this takes a long time. is it really necessary?
			#cursor.execute("SELECT MIN(postnumber) as lasteditablepost FROM Post WHERE threadid=%s AND TIMESTAMPDIFF(MINUTE,postTime,NOW()) < 30",int(jsonmessage['ThreadId']))
			#postrow = cursor.fetchone()
			#print postrow["lasteditablepost"]
			#if(postrow["lasteditablepost"] != None):
			#	posts = postrow["lasteditablepost"]
			#	readThread=True
		
		#read thread if flagged
		if(readThread):
			cursor.execute("SELECT DATE_FORMAT(NOW(), '%a, %d %b %Y %H:%i:%s') as curtime")
			timerow = cursor.fetchone()
			milliseconds = (int)(time.time()*1000 + 50000)
			conn.send(message="read thread!", destination=readthreads, 
				headers={'expires': milliseconds, 'URL':jsonmessage['URL'],
				'threadid':int(jsonmessage['ThreadId']),'startPost':posts+1, 
				'CurrentUTC':timerow['curtime'] + " GMT"}, ack='auto')


#Queue
lobby='fennecfox.lobby'
readlobby = 'fennecfox.pleasereadlobby'
readthreads = 'fennecfox.pleasereadthread'
#Set up server connection
conn=stomp.Connection([('mq.checkmywwstats.com',61613)])
print('set up Connection')
conn.set_listener('somename',MyListener())
print('Set up listener')

conn.start()
print('started connection')

conn.connect(wait=True)
print('connected')
conn.subscribe(destination=lobby, ack='auto')
print('subscribed')

#wait forever
while(True):
	time.sleep(6)
	
print('slept')
conn.stop()
print('disconnected')
