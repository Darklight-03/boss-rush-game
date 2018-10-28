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
    // msg { uname:"username" }
    console.log(msg);
    var obj = JSON.parse(msg);
    console.log(obj);
    var lobby = [ { uname: obj['uname'] } ];
    console.log(lobby);
    var thislobbyid = lobbies.length;
    lobbies[thislobbyid] = lobby;
    socket.emit('create lobby', { lobbyid: thislobbyid } );
    console.log(lobbies);
  });

  socket.on('join lobby', function(msg){
    // msg { uname:"username", lobbyid:14 }
    console.log(msg);
    var obj = JSON.parse(msg);
    if (obj['lobbyid'] >= lobbies.length) {
      socket.emit('join lobby', { ret: 'fail' } );
    }
    else {
      var lobby = lobbies[obj['lobbyid']];
      lobby[lobby.length] = { uname: obj['uname'] };
      lobbies[obj['lobbyid']] = lobby;
      socket.emit('join lobby', { ret: 'success' } );
    }
    console.log(lobbies);
  });

  socket.on('disband lobby', function(msg){
    
  });

  socket.on('general message', function(msg){
    
  });
  
  socket.on('disconnect', function(){
    console.log('user disconnected');
  });
  
});

http.listen(port, function(){
  console.log('listening on *:' + port);
});
