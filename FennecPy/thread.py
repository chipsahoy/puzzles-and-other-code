from lxml import etree
from urllib.parse import parse_qs, urlparse
import misc
import post
import datetime
__author__ = 'Paul'

class ThreadReader:
    def __init__(self, s):
        self.s = s

    def readpages(self, url, start, end, callback):
        rc = self.getpage(url, start, callback)
        return rc

    def getpage(self, url, start, callback):
        outpages = 0
        local = url
        if r"showthread.php?" in url:
            local += "&page=" + repr(start)
        else:
            local += "index" + repr(start) + ".html"
        doc = self.s.get(local)
        posts = self.parsethreadpage(doc.text)
        return posts

    def parsethreadpage(self, doc):
        parser = etree.HTMLParser()
        tree = etree.fromstring(doc, parser)
        #(//div[class="smallfont", align="center'])[last()] All times are GMT ... The time is now <span class="time">time</span>"."

        timenode = tree.xpath(r"//div[@class='smallfont'][@align='center']/span[@class='time']/..")[-1]
        timetext = etree.tounicode(timenode, method="text")
        servertime = misc.parsepagetime(timetext, datetime.datetime.utcnow())
        return

        postlist = []
        xpath = r"//div[@id='posts']/div/div/div/div/table/tr[2]/td[2]/div[contains(@id, 'post_message_')]"
        posts = tree.xpath(xpath)
        for post in posts:
            p = self.htmltopost(post, servertime)
            postlist.append(p)
        return postlist

    def htmltopost(self, html, pagetime):
        self.removecomments(html)
        c = etree.tounicode(html, method='html', pretty_print=True)

        postnumber = 0
        postnumbernode = html.xpath(r"../../../tr[1]/td[2]/a[last()]")
        if postnumbernode:
            postnumber = int(etree.tounicode(postnumbernode[-1], method="text"))
            postlinknode = postnumbernode[-1].attrib['href']
            parsed = urlparse(postlinknode)
            postid = int(parse_qs(parsed.query)['p'][0])

        titlenode = html.xpath(r"../div[@class='smallfont']/strong")
        title = etree.tounicode(titlenode[-1], method="text").strip()

        posternode = html.xpath(r"../../td[1]/div/a[starts-with(@class,'bigusername')]")
        poster = etree.tounicode(posternode[-1], method="text").strip()

        timenode = html.xpath(r"../../../tr[1]/td[1]")
        timestring = etree.tounicode(timenode[-1], method="text").strip()
        ts = misc.parseitemtime(pagetime, timestring)

        p = post.Post(content=c, postnumber=postnumber, title=title, postername=poster,
                      postid=postid, ts=ts)
        print(postnumber, postid, poster, title)
        return p

    def removecomments(self, html):
        comments = html.xpath("//comment()")
        for c in comments:
            parent = c.getparent()
            parent.remove(c)
