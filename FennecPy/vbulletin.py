import re
import hashlib
import requests
import pm
import lobby
import thread

class VBulletin:
    username = 'Oreos'
    password = 'sj7kbW7uaTPJ99McPYav'.encode('utf-8')
    baseurl = 'http://forumserver.twoplustwo.com/'
    loginurl = 'login.php?do=login'
    securitytoken = ''

    def __init__(self):
        self.password = hashlib.md5(self.password).hexdigest()
        self.s = requests.Session()
        self.postsPerPage = 100

    def login(self):
        post = {'vb_login_username': self.username,
                'do': 'login',
                'vb_login_md5password': self.password,
                'vb_login_md5password_utf': self.password,
                'securitytoken': 'guest',
                's': ''}
        r = self.s.get(self.baseurl + self.loginurl, data=post)

        p = r'"umaxposts.*?value="(-?\\d+)"[ ](class="[A-z0-9]*")?[ ]*selected="selected"'
        match = re.search(p, r.text)
        if match:
            postsperpage = match.group(1)
            if postsperpage == -1:
                postsperpage = 15
            self.postsPerPage = postsperpage
        self.GetSecurityToken()
        self.s.post(self.baseurl + 'profile.php?do=editoptions')
        self.Lobby = lobby.LobbyReader(self.s, self.baseurl +
                '59/puzzles-other-games/')
        self.ThreadReader = thread.ThreadReader(self.s)

    def GetOurUserId(self):
        cookie = self.s.cookies['bbuserid']
        return cookie

    def GetSecurityToken(self):
        r = self.s.get(self.baseurl + 'private.php')
        p = r'var SECURITYTOKEN = "(.+)"'
        match = re.search(p, r.text)
        if match:
            self.securitytoken = match.group(1)

    def MakePost(self, thread, title, body):
        prm = {'do': 'postreply', 't': thread}
        post = {
                'securitytoken': self.securitytoken,
                'ajax': '1',
                'message_backup': body,
                'message': body,
                'wysiwyg': '0',
                's': '',
                'do': 'postreply',
                't': thread,
                'p': 'who care',
                'specifiedpost': '0',
                'parseurl': '1',
                'posthash': 'invalid posthash',
                'poststarttime': '0',
                'multiquoteempty': '',
                'sbutton': "Submit Reply",
                'emailupdate': '0',
                'folderid': '0',
                'rating': '0',
                'loggedinuser': self.GetOurUserId(),
                'fromquickreply': '1',
                'styleid': '0',
                }
        r = self.s.post(self.baseurl + 'newreply.php',
                params=prm, data=post)

    def SendPM(self, msg):
        prm = {'do': 'insertpm', 'pmid': ''}
        post = {
                'recipients': msg.ToNames,
                'bccrecipients': '',
                'title': msg.title,
                'message': msg.content,
                'wysiwyg': '0',
                'iconid': '0',
                's': '',
                'securitytoken': self.securitytoken,
                'do': 'insertpm',
                'pmid': '',
                'forward': '',
                'sbutton': 'Submit Button',
                'receipt': '1',
                'savecopy': '1',
                'parseurl': '1'
                }
        r = self.s.post(self.baseurl + 'private.php',
                params=prm, data=post)





