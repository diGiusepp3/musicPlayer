<?php

ob_clean(); // verwijder mogelijke eerdere output
header('Content-Type: application/json; charset=utf-8');
require_once $_SERVER['DOCUMENT_ROOT'] . '/ini.inc';

$response = [];

// Check of database werkt
if ($mysqli->ping()) {
    $response['status'] = 'ok';
} else {
    $response['status'] = 'error';
    $response['message'] = $mysqli->error;
}

echo json_encode($response);
