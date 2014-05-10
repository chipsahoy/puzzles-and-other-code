#!/usr/bin/env python
import stomp
import time
import logging
import sys
import random
import MySQLdb
import json

# Find cases where the count of posts doesn't match the lobby record.

#connect to database and set up the cursor to accept commands
connection = MySQLdb.connect("mysql.checkmywwstats.com", "pogwwdb", "chipsahoygothacked", "fennecfox")
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
print('looking for threads with bad counts')

cursor.execute("""SELECT DISTINCT Thread.threadid as tid,
url, Thread.replies 
FROM Thread
LEFT OUTER JOIN Post ON Thread.threadid = Post.threadid
WHERE Post.threadid IS NULL and Thread.subforumid=59
ORDER BY Thread.replies DESC
LIMIT 2500
""")
print cursor.rowcount
rows = cursor.fetchall()
for row in rows:
	url = row['url']
	threadid = row['tid']
	cursor.execute("SELECT DATE_FORMAT(NOW(), '%a, %d %b %Y %H:%i:%s') as curtime")
	timerow = cursor.fetchone()
	milliseconds = (int)(time.time()*1000 + 50000)
	print('fetching ' + url)
	conn.send(message="read thread!", destination=readthreads, headers={'expires': milliseconds, 'URL':url,'threadid':int(threadid),'startPost':1, 'CurrentUTC':timerow['curtime'] + " GMT"}, ack='auto')

	
print('slept')
conn.stop()
print('disconnected')
