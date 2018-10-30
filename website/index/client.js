//var WebSocketClient = require('websocket').client;
var lobbyid = -1;
var socket = new WebSocket('ws://localhost:3000/', 'you-good-protocol');

socket.onopen = function (event) {
  console.log(event);
}


socket.onmessage = function(event) {
  console.log(event);
  msg = JSON.parse(event.data);
  switch(msg['msgtype']) {
    case 'create lobby':
      console.log(msg['lobbyid']);
      lobbyid = msg['lobbyid'];
    break;

    case 'join lobby':
      console.log(msg['ret']);
    break;

    case 'general message':
      console.log(msg);
    break;

    case 'get lobbies':
      console.log(msg);
    break;

  }
}


function createLobby() {
  var input = document.getElementById("unamec");
  if (input.value != '') {
    socket.send(JSON.stringify({ msgtype:'create lobby', uname: input.value }));
    input.value = '';
  }
  else {
  }
}

function joinLobby() {
  var inputName = document.getElementById("unamej");
  var inputID = document.getElementById("lobbyid");
  if (inputName.value != '' && inputID.value != '') {
    socket.send(JSON.stringify({ msgtype:'join lobby', uname: inputName.value, lobbyid: inputID.value }));
    lobbyid = inputID.value;
    inputName.value = '';
    inputID.value = '';
  }
  else {
  }
}

function sendMessage() {
  var inputMessage = document.getElementById("mess");
  if (inputMessage.value != '') {
    socket.send(JSON.stringify({ msgtype:'general message', lobbyid: lobbyid, content: inputMessage.value }));
    inputMessage.value = '';
  }
  else {
  }
}
