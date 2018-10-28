const express = require('express');
var app = express();
var http = require('http').Server(app);
var io = require('socket.io')(http);
var port = process.env.PORT || 3000;

app.get('/', function(req, res){
  res.sendFile(__dirname + '/index/index.html');
});

app.use(express.static('index'));
app.use(express.static('assets'));


var lobbies = [];
console.log(lobbies);

io.on('connection', function(socket){
  console.log('user connected');

  /*
  socket.on('chat message', function(msg){
    io.emit('chat message', msg);
  });
  */
  
  
  socket.on('create lobby', function(msg){
    console.log(lobbies);
    // msg { uname:"username" }
    console.log('msg=');
    //console.log(msg);
    var lobby = [ { sock: socket, uname: msg['uname'] } ];
    //console.log(lobby);
    var thislobbyid = lobbies.length;
    lobbies[thislobbyid] = lobby;
    socket.emit('create lobby', { lobbyid: thislobbyid } );
    console.log(lobbies);
  });

  socket.on('join lobby', function(msg){
    // msg { uname:"username", lobbyid:14 }
    //console.log(msg);
    if (msg['lobbyid'] >= lobbies.length) {
      socket.emit('join lobby', { ret: 'fail' } );
    }
    else {
      var lobby = lobbies[msg['lobbyid']];
      lobby[lobby.length] = { sock: socket, uname: msg['uname'] };
      lobbies[msg['lobbyid']] = lobby;
      socket.emit('join lobby', { ret: 'success' } );
    }
    console.log(lobbies);
  });

  socket.on('disband lobby', function(msg){
    lobbies.splice(msg['lobbyid'], 1);
    socket.emit('disband lobby', { ret: 'success' } );
  });

  socket.on('general message', function(msg){
    console.log(msg);
    var lobby = lobbies[msg['lobbyid']];
    console.log(lobby);
    for (i = 0; i < lobby.length; i++) {
      console.log(lobby[i]['uname']);
      
      lobby[i]['sock'].emit('general message', msg['content']);
    }
  });
  
  socket.on('disconnect', function(){
    console.log('user disconnected');
  });
  
});

http.listen(port, function(){
  console.log('listening on *:' + port);
});
