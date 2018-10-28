var socket = io();
var lobbyid = -1;

$(function () {
    
  // send messages
  /*$('form').submit(function(){
    socket.emit('chat message', $('#m').val());
    $('#m').val('');
    return false;
  });*/
  
  // recieve messages
  /*socket.on('chat message', function(msg){
    $('#messages').append($('<li>').text(msg));
    window.scrollTo(0, document.body.scrollHeight);
  });

  socket.on('uinfo', function(msg){
    $('#messages').append($('<li class="uinfo">').text(msg.content)); 
    window.scrollTo(0, document.body.scrollHeight);
  });*/

});

function createLobby() {
	socket.emit('create lobby', { uname: "aaaa" } );

  socket.on('create lobby', function(msg){
    console.log(msg['lobbyid']);
    lobbyid = msg['lobbyid'];
  });
}

function joinLobby() {
  socket.emit('join lobby', { uname: "second", lobbyid: 0 } );

  socket.on('join lobby', function(msg){
    console.log(msg['ret']);
  });
}

/*
var form = document.getElementById("f1");
function handleForm(event) {
  event.preventDefault();
  console.log(event);
}
form.addEventListener('submit', handleForm);
*/

function fcreateLobby() {
  var input = document.getElementById("unamec");
  if (input.value != '') {
    socket.emit('create lobby', { uname: input.value } );
    input.value = '';
  }
  else {
  }
}

function fjoinLobby() {
  var inputName = document.getElementById("unamej");
  var inputID = document.getElementById("lobbyid");
  if (inputName.value != '' && inputID.value != '') {
    socket.emit('join lobby', { uname: inputName.value, lobbyid: inputID.value } );
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
    socket.emit('general message', { lobbyid: lobbyid, content: inputMessage.value } );
  }
  else {
  }
}
    
socket.on('create lobby', function(msg){
  console.log(msg['lobbyid']);
  lobbyid = msg['lobbyid'];
});
socket.on('join lobby', function(msg){
  console.log(msg['ret']);
});
socket.on('general message', function(msg){
  console.log(msg);
});
