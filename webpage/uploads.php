<?php
include('./login.php');

ini_set('display_errors', 'On');
error_reporting(E_ALL);
?>

<h2>Recently Uploaded Games</h2>
<table class="data" border="1">
<thead>
	<tr>
	<th>Date</th>
	<th>Uploaded by</th>
	<th>Game</th>
</thead>
<tbody>

<?php	
	$qry = "select date(uploadtime) dt, message, gamename, gameid, url from editslog e join game g using (url) where left(message,6) <> 'script' order by uploadtime desc limit 50";
	
	$result = $db->query($qry);
	
	while($game = $result->fetch_assoc())
	{
		echo("<tr>
		<td>".$game['dt']."</td>
		<td>".$game['message']."</td>
		<td><a href='index.php?report=Game&gameid=".$game['gameid']."'>".$game['gamename']."</a> <a href=".$game['url'].">(link)</a></td>
		</tr>");		
	}
?>
</tbody>
</table>

