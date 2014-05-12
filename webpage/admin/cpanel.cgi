#!/usr/bin/env python
# -*- coding: UTF-8 -*-

# enable debugging
import cgitb
import cgi, os, sys, string, time, datetime
cgitb.enable()
sys.stderr = sys.stdout 

import MySQLdb
db = MySQLdb.connect(host="localhost", user="poguser", passwd="werewolf", db='pog', charset="utf8", use_unicode=True)

cursor = db.cursor(MySQLdb.cursors.DictCursor)
cursor.execute("set autocommit=0")

print "Content-Type: text/html;charset=utf-8"
print
print "<head><title>WWDB Control Panel</title></head>"
print """<script type="text/javascript"> 
	function stopRKey(evt) { 
	var evt = (evt) ? evt : ((event) ? event : null); 
	var node = (evt.target) ? evt.target : ((evt.srcElement) ? evt.srcElement : null); 
	if ((evt.keyCode == 13) && (node.type=="text"))  {return false;} }
	document.onkeypress = stopRKey;
	</script>"""
print "<h3><ul>Werewolf Database Control Panel</ul></h3>"
