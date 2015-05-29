class PrivateMessage:

    def __init__(self, fromName, toNames, title, content,
            ts = None, id = None):
        self.fromName = fromName
        self.ToNames = toNames
        self.title = title
        self.content = content

