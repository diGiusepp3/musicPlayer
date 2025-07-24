<?php
// ini.inc

// Error logs
ini_set('display_errors', 0);
ini_set('log_errors', 1);
error_log('/var/www/datadrive/subdomains/music.datadrive.be/logs/_errors.log');


// Databaseconfiguratie
$db_host = 'localhost';
$db_name = 'datadrivebe_music';
$db_user = 'matthias';
$db_pass = '7824';

// MySQLi-verbinding
$mysqli = new mysqli($db_host, $db_user, $db_pass, $db_name);

// Controleer verbinding
if ($mysqli->connect_error) {
    die("Databaseverbinding mislukt: " . $mysqli->connect_error);
}
 
// Stel charset in (voor UTF-8 ondersteuning)
if (!$mysqli->set_charset("utf8mb4")) {
    die("Fout bij instellen van charset: " . $mysqli->error);
}