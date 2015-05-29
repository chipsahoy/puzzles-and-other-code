from lxml import etree

class LobbyReader:
    def __init__(self, s, url):
        self.s = s
        self.baseurl = url

    def ReadLobby(self, pageStart, pageEnd, recentFirst, callback):
        for x in range(pageStart, pageEnd + 1):
            page = self.GetPage(x, recentFirst, callback)
            callback(x, page)

    def GetPage(self, page, recentFirst, callback):
        url = self.baseurl
        if page > 1:
            url = url + "index" + page + ".html"
        if not recentFirst:
            url = url + "?sort=dateline&order=asc&daysprune=-1"
        r = self.s.get(url)
        self.ParseLobbyPage(r.text, callback)

    def ParseLobbyPage(self, html, callback):
        parser = etree.HTMLParser()
        tree = etree.fromstring(html, parser)
        timeNode = tree.xpath("//div[@class='smallfont'][@align='center']")
        timeString = etree.tostring(timeNode[-1], method="text")
        print (timeString)
        threads = tree.xpath(
                "//tbody[contains(@id, 'threadbits_forum_')]/tr")
        for t in threads:
            ft = self.HtmlToThread(t)

    def HtmlToThread(self, node):
        title = node.xpath(
                "td[3]/div[1]/a[contains(@id, 'thread_title_')]")
        print (title[0].text)






