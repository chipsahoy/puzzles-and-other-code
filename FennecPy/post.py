__author__ = 'Paul'

class Post:
    def __init__(self, threadid=0, postnumber=0, postid=0, postername='0', posterid=0,
                 ts=0, postlink='', title='', content='', edit=''):
        self.threadid = threadid
        self.postnumber = postnumber
        self.postid = postid
        self.postername = postername
        self.posterid = posterid
        self.ts = ts
        self.postlink = postlink
        self.title = title
        self.content = content
        self.edit = edit