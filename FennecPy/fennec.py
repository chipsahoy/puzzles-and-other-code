import vbulletin
import fennecdb

def LobbyCallback(page, html):
    print ("Lobby Callback!!")
    print (html)

def ThreadCallback():
    print("thread callback.")

x = vbulletin.VBulletin()
x.login()
#x.MakePost('1204368', 'title', '1234')
#msg = pm.PrivateMessage('Oreos', 'Chips Ahoy', 'test pm', 'abcd')
#x.SendPM(msg)
#l = x.Lobby
#l.ReadLobby(1, 1, True, LobbyCallback)

url = r'http://forumserver.twoplustwo.com/59/puzzles-other-games/2015-mafia-championship-game-4-a-1531336/'
t = x.ThreadReader
posts = t.getpage(url, 1, ThreadCallback)

d = fennecdb.FennecDb('localhost', 'fennecfox', 'werewolf', 'fennec')
d.writethreaddefinition(1531336, url, False)
d.addposts(posts)
