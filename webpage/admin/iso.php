<h3><ul>POG Posts ISO View</ul></h3><br>

<?php
include('../login.php');
$db->set_charset("utf8");

# echo strip_tags($text); for searches

$threadid = $_GET['threadid'];
$result = $db->query("select * from fennecfox.Thread where threadid=".$threadid);
if ($result->num_rows == 0) {
	echo "Thread not found";
} else {
	$thread = $result->fetch_assoc();
	echo "Thread: ".$thread['title']."<br>";
}

$result = $db->query("select postid, postnumber, postername, 
	date_format(posttime - interval 8 hour, '%a %e %b %Y %l:%i %p') posttime, content 
	from fennecfox.post2 p join fennecfox.Poster t using (posterid) 
	where t.forumid=1 and p.threadid=".$threadid." limit 1000,50");

echo $result->num_rows.' results<br><br>';

echo '<table class="tborder" cellpadding="4" cellspacing="0" border="1" width="100%" align="center">';

for ($i = 1; $i < $result->num_rows; $i++) {
	$post = $result->fetch_assoc();
	$content = $post['content'];
	#$content = preg_replace('<img class="inlineimg" src="http://forumserver.twoplustwo.com/images/buttons/viewpost.gif" border="0" alt="View Post">','', $content);
	$content = preg_replace('~<img\b[^>]+\bsrc\s?=\s?[\'"](.*?)[\'"]~is', '', $content);
	#$content = preg_replace("/<img src=\"([ a-zA-Z0-9|\/\:\+.%?=_-]*)\">/i", "[img]$1[/img]", $content);
	#$content = preg_replace("/<img[^>]+\>/i", "(image) ", $content); 
	$content = preg_replace("/<iframe[^>]+\>/i", "(youtube) ", $content); 
	$content = preg_replace('<table class="stg_table tborder">', '<table class="stg_table tborder" cellspacing="0" border="1">', $content);
	echo '<tr><td valign="top" width="20%">'.
		'<a href=http://forumserver.twoplustwo.com/showpost.php?p='.$post['postid'].
		' target="_blank">Post # '.$post['postnumber'].'</a><br>'.$post['postername'].'<br>'.$post['posttime'].'<br>Copy to clipboard</td>';
	if ($i % 2 == 1)
		echo '<td bgcolor="#efe1f1">';
	else
		echo '<td bgcolor="#D6EEE1">';
	echo $content.'</td></tr>';
}
echo '</table>';

?>

<br><br>
<a href="xxxxx.php">Return to previous</a><br>
