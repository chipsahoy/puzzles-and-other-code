<h3><ul>Werewolf Database Control Panel</ul></h3><br><br>

<?php
	include('../login.php');
	$qry = "select concat(rolename, ' (', count(rs.roletype), ')') rolename, roleid from roles r 
		left join roleset rs on r.roleid=rs.roletype where r.roleid <> 1 group by r.roleid order by 1";
	$result = $db->query($qry);
?>

<form method="post" action="addrole.php">
	Existing roles (# of appearances):<br>
	<select name='roles'>
<?php
	while($role = $result->fetch_assoc()) {
		echo "<option value = 1>" . $role['rolename'] . "</option>\n";
	}
?>	
	</select>
	<br><br>
	<input type="text" name="rolename" size="30" value="">
	<input type="submit" name="addrole" value="Add new role"><br><br>
</form>
	

<br><br>
<a href=".">Return to Control Panel</a>

