
<?php
include('./login.php');

ini_set('display_errors', 'On');
error_reporting(E_ALL);
?>
<html>
	<head>
		<title>WerewolfDB - 
		<?php if(array_key_exists('Player', $_GET)) echo $_GET['Player']; elseif(array_key_exists('Game', $_GET)) 
			echo $_GET['Game']; elseif(array_key_exists('report', $_GET)) echo $_GET['report']; ?>
		</title>

		<h2><a href="index.php">Werewolf Database</a></h2>

		<script type="text/javascript" language="javascript" src="./jquery-1.7.2.min.js"></script>
		<script type="text/javascript" language="javascript" src="./jquery.dataTables.min.js"></script>
		<script type="text/javascript" charset="utf-8">
		
		/* Custom filtering function which will filter data in column four between two values */
			$.fn.dataTableExt.afnFiltering.push(
				function( oSettings, aData, iDataIndex ) {
					var iMin = document.getElementById('min').value * 1;
					var iMax = document.getElementById('max').value * 1;
					var iWolfMin = document.getElementById('wolfmin').value * 1;
					var iVillageMin = document.getElementById('villagermin').value * 1;
					var iWolfMax = document.getElementById('wolfmax').value * 1;
					var iVillageMax = document.getElementById('villagermax').value * 1;
					var iVersion = aData[document.getElementById('colgame').value] * 1;
					var iWolf = aData[document.getElementById('colwolf').value] * 1;
					var iVillager = aData[document.getElementById('colvillage').value] * 1;
					bool = false;
					if ( iMin == "" && iMax == "" )
					{
						bool = true;
					}
					else if ( iMin == "" && iVersion <= iMax )
					{
						bool = true;
					}
					else if ( iMin <= iVersion && "" == iMax )
					{
						bool = true;
					}
					else if ( iMin <= iVersion && iVersion <= iMax )
					{
						bool = true;
					}
					if(bool == false)
						return false;
					bool = false;
					if ( iWolfMin == "" && iWolfMax == "" )
					{
						bool = true;
					}
					else if ( iWolfMin == "" && iWolf <= iWolfMax )
					{
						bool = true;
					}
					else if ( iWolfMin <= iWolf && "" == iWolfMax )
					{
						bool = true;
					}
					else if ( iWolfMin <= iWolf && iWolf <= iWolfMax )
					{
						bool = true;
					}
					if(bool == false)
						return false;
					bool = false;
					if ( iVillageMin == "" && iVillageMax == "" )
					{
						bool = true;
					}
					else if ( iVillageMin == "" && iVillager <= iVillageMax )
					{
						bool = true;
					}
					else if ( iVillageMin <= iVillager && "" == iVillageMax )
					{
						bool = true;
					}
					else if ( iVillageMin <= iVillager && iVillager <= iVillageMax )
					{
						bool = true;
					}
					if(bool == true)
						return true
					else
						return false;
				}
			);
			$(document).ready(function() {
				var oTable = $('.data').dataTable({
					"bJQueryUI": true,
					"sPaginationType": "full_numbers",
					"aLengthMenu": [[25, 50, 100, -1], [25, 50, 100, "All"]],
					"iDisplayLength" : 25,
					"fnDrawCallback": function ( oSettings ) {
						if (oSettings.bSorted)
						{
							for ( var i=0, iLen=oSettings.aiDisplayMaster.length ; i<iLen ; i++ )
							{
								$('td:eq(0)', oSettings.aoData[ oSettings.aiDisplayMaster[i] ].nTr ).html( i+1 );
							}
						}
					},
					"aoColumnDefs": [
						{ "bSortable": false, "aTargets": [ 0 ] }
					]
					/*, "aaSorting": [[ 1, 'asc' ]] */
				});
				$('#min').keyup( function() { oTable.fnDraw(); } );
				$('#max').keyup( function() { oTable.fnDraw(); } );
				$('#wolfmin').keyup( function() { oTable.fnDraw(); } );
				$('#wolfmax').keyup( function() { oTable.fnDraw(); } );
				$('#villagermin').keyup( function() { oTable.fnDraw(); } );
				$('#villagermax').keyup( function() { oTable.fnDraw(); } );
			} );
		</script>
		<style type="text/css" title="currentStyle">
			@import "css/demo_page.css";
			@import "css/demo_table_jui.css";
			@import "css/smoothness/jquery-ui-1.8.21.custom.css";
		</style>
	</head>
	<body>
		<form action="index.php" method="get">
			<select name="report" onchange="this.form.submit();">
				<option selected></option>
				<option>All Players</option>
				<option>Gimmicks</option>
				<option>Death Stats</option>
				<option>Games</option>
				<option>Turbos</option>
				<option>Lynch Stats</option>
				<option>Miscellaneous</option>
				<option>Moderators</option>
				<option>Player Records</option>
				<option>Post Stats</option>
				<option>Power Rankings</option>
				<option>Rand Stats</option>	
				<option>Sub Stats</option>
				<option>Thread Stats</option>
			</select>
		</form>
		
		<?php
			if(array_key_exists('report', $_GET))
			{
				if($_GET['report'] == 'Moderators') include('./modules/Moderators.php');
				else if($_GET['report'] == 'Player') include('./modules/Player.php');
				else if($_GET['report'] == 'Game') include('./modules/Game.php');
				else if($_GET['report'] == 'Games') include "./modules/Games.php";
				else if($_GET['report'] == 'Turbos') include "./modules/Turbos.php";
				else if($_GET['report'] == 'Player Records') include "./modules/PlayerRecords.php";
				else if($_GET['report'] == 'All Players') include "./modules/AllPlayers.php";
				else if($_GET['report'] == 'Gimmicks') include "./modules/Gimmicks.php";
				else if($_GET['report'] == 'Rand Stats') include "./modules/RandStats.php";
				else if($_GET['report'] == 'Lynch Stats') include "./modules/LynchStats.php";
				else if($_GET['report'] == 'Sub Stats') include "./modules/SubStats.php";
				else if($_GET['report'] == 'Death Stats') include "./modules/DeathStats.php";
				else if($_GET['report'] == 'Common Games') include "./modules/CommonGames.php";
				else if($_GET['report'] == 'PlayedWith') include "./modules/PlayedWith.php";
				else if($_GET['report'] == 'Post Stats') include "./modules/PostStats.php";
				else if($_GET['report'] == 'Power Rankings') include "./modules/PowerRankings.php";
				else if($_GET['report'] == 'Thread Stats') include "./modules/ThreadStats.php";
			}
		?>
		<p></p>
		<a href="downloads.html">Latest Fennec Fox and Ricky Raccoon</a>
		<?php
			if(!array_key_exists('report', $_GET) || $_GET['report'] == 'Miscellaneous') include "./modules/Miscellaneous.php";
		?>
	</body>

</html>
