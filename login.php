<?php
#$connection = mysql_connect('localhost', 'poguser', 'werewolf');
#mysql_query("use pog");

$db = new mysqli('localhost', 'poguser', 'werewolf', 'pog');
if($db->connect_errno > 0){
    die('Unable to connect to database [' . $db->connect_error . ']');
}
?>
