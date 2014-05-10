#!/usr/bin/env python
import stomp
import time
import logging
import sys
import random
import MySQLdb
import json

#connect to database and set up the cursor to accept commands
connection = MySQLdb.connect("mysql.checkmywwstats.com", "pogwwdb", "werewolf", "fennecfox")
cursor = connection.cursor(MySQLdb.cursors.DictCursor)
cursor.execute("SET time_zone = '+00:00'")
logging.basicConfig()

connection.set_character_set('utf8')
cursor.execute('SET NAMES utf8;')
cursor.execute('SET CHARACTER SET utf8;')
cursor.execute('SET character_set_connection=utf8;')

#Queue
rand='rickyraccoon.rand'
#Set up server connection
conn=stomp.Connection([('mq.checkmywwstats.com',61613)])
print('set up Connection')

conn.start()
print('started connection')

conn.connect(wait=True)
print('connected')

#wait forever
while(True):
	cursor.execute("SELECT * FROM Post WHERE posttime > DATE_SUB(NOW(), INTERVAL 1 HOUR) AND content LIKE '%Ricky Rand:%' AND rickyread=0")
	if cursor.rowcount > 0:
		print("found")
		rickyrow = cursor.fetchone()
		print("updating post")
		cursor.execute("UPDATE Post SET rickyread=1 WHERE postid = " + str(rickyrow['postid']))
		print("updated post")
		signupthreadid = rickyrow['threadid']
		content = rickyrow['content']
		content = content.split("Ricky Rand:")[1].split("\t")[0]
		print content
		content.strip()
		content = content.split("\n")
		for line in content:
			line = line[:-2]
		gamename = content[1]
		playerlist = ",".join(content[2:-1])
		print "playerlist:\n" + playerlist
		milliseconds = (int)(time.time()*1000 + 50000)
		conn.send(message="new rand!", destination=rand, headers={'expires': milliseconds, 'playerlist':playerlist,'gamename':gamename,'username':'Ricky Raccoon', 'password':'DGmZSnD8', 'signupthreadid':signupthreadid}, ack='auto')
		time.sleep(30)
	
print('slept')
conn.stop()
print('disconnected')
