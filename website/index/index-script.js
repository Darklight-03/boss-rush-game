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
    var obj = JSON.parse(msg);
    console.log(obj['lobbyid']);
    lobbyid = obj['lobbyid'];
  });
}
