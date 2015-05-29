import MySQLdb as mdb

__author__ = 'Paul'

class FennecDb:
    def __init__(self, host, username, password, database):
        self.host = host
        self.username = username
        self.password = password
        self.database = database

    def getconnection(self):
        try:
            con = mdb.connect(self.host, self.username, self.password, self.database,  use_unicode=True, charset="utf8")
        except mdb.Error as e:
            print("Error %d: %s" % (e.args[0],e.args[1]))
        return con


    def writethreaddefinition(self, threadid, url, turbo):
        con = self.getconnection()
        with con:
            cur = con.cursor()
            sql0 = """INSERT IGNORE INTO Thread (threadid, url, turbo) VALUES (%s, %s, %s);"""
            rc = cur.execute(sql0, (threadid, url, turbo))

    def addposts(self, posts):
        print('hi')
        con = self.getconnection()
        with con:
            cur = con.cursor()
            sql = """INSERT IGNORE INTO Poster (posterid, postername) VALUES (%s, %s);"""
            sql2 = """
INSERT IGNORE INTO Post (
postid,
threadid,
posterid,
postnumber,
content,
title,
posttime)
VALUES (%s, %s, %s, %s, %s, %s, %s);"""
            for post in posts:
                rc = cur.execute(sql, (post.posterid, post.postername))
                rc = cur.execute(sql2, (post.postid, post.threadid, post.posterid,
                                  post.postnumber, post.content, post.title, post.ts))

