

-- postcount
truncate postcount;

insert into postcount
select p.posterid, p.threadid, count(*) posts
from fennecfox.Post p
join fennecfox.Thread t using (threadid)
join game g on g.gameid=t.threadid
group by p.posterid, p.threadid;

-- derivedrankings
-- nah


