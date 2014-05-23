<h3><ul>Werewolf Database Control Panel</ul></h3><br><br>

<?php
	include('../login.php');

	$rolename = $_POST['rolename'];

	$qry = "select * from roles where rolename = '" . $rolename . "'";
	$result = $db->query($qry);
	if ($result->num_rows > 0) {
		echo "Role '".$rolename."' already exists.";
	} elseif ($rolename == '' or strlen($rolename) < 3) { 
		echo "Enter a longer role name.";
	} else {
		$db->query("insert into roles (rolename) values ('".$rolename."')");
		echo "Role '".$rolename."' added.";
	}
	
#	$qry = $db->prepare("select roleid, rolename from roles where rolename = ?");
#	$qry->bind_param("s", $rolename);
#	$qry->execute();
#	$qry->bind_result($result2, $result3);
#	$qry->fetch();
?>

<br><br>
<a href="roles.php">Return to Roles</a><br>
<a href=".">Return to Control Panel</a>