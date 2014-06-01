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
print('looking for threads with bad counts')
cursor.execute("""SELECT url, t.threadid as tid, greatest(1, count(*)-10) startpost,
((t.replies + 1) - COUNT(*)) AS missing, t.replies
FROM thread2 t
JOIN post2 p ON t.threadid = p.threadid
GROUP BY t.threadid
HAVING missing > 100 and ((t.replies + 1) - COUNT(*))/t.replies > .5
ORDER BY (replies - count(*))/replies desc
limit 100
""")

print cursor.rowcount
rows = cursor.fetchall()
for row in rows:
	url = row['url']
	threadid = row['tid']
	#cursor.execute("select min(postnumber) startpost from post2 where postnumber not in (%s) and threadid=%s", (str(range(1,30))[1:-1]), threadid))
	#startpost = cursor.fetchone()['startpost']
	cursor.execute("SELECT DATE_FORMAT(NOW(), '%a, %d %b %Y %H:%i:%s') as curtime")
	timerow = cursor.fetchone()
	milliseconds = (int)(time.time()*1000 + 50000)
	print('fetching ' + url)
	conn.send(message="read thread!", destination=readthreads, 
		headers={'expires': milliseconds, 'URL':url,'threadid':int(threadid),'startPost':1, 'CurrentUTC':timerow['curtime'] + " GMT"}, ack='auto')

	
print('slept')
conn.stop()
print('disconnected')
