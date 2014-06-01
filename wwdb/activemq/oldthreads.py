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
lobby='fennecfox.lobby'
readlobby = 'fennecfox.pleasereadlobby'
readthreads = 'fennecfox.pleasereadthread'
#Set up server connection
conn=stomp.Connection([('mq.checkmywwstats.com',61613)])
print('set up Connection')

conn.start()
print('started connection')

conn.connect(wait=True)
print('connected')

pageNumber = 739
endPage = 745

cursor.execute("SELECT DATE_FORMAT(NOW(), '%a, %d %b %Y %H:%i:%s') as curtime")
timerow = cursor.fetchone()
milliseconds = (int)(time.time()*1000 + 50000)
conn.send(message="read lobby!", destination=readlobby, headers={'expires': milliseconds, 'BaseURL':'http://forumserver.twoplustwo.com/46/sporting-events/','startPage':pageNumber,'endPage':endPage, 'recentFirst':'False', 'CurrentUTC':timerow['curtime'] + " GMT"}, ack='auto')
print("read page" + repr(pageNumber));
pageNumber += 1
	
print('slept')
conn.stop()
print('disconnected')
