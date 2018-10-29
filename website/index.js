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

io.on('connection', function(socket){
  console.log('user connected');

  
  socket.on('create lobby', function(msg){
    // msg { uname:"username" }
    var lobby = [ { sock: socket, uname: msg['uname'] } ];
    var thislobbyid = lobbies.length;
    lobbies[thislobbyid] = lobby;
    socket.emit('create lobby', { lobbyid: thislobbyid } );
  });

  socket.on('join lobby', function(msg){
    // msg { uname:"username", lobbyid:14 }
    if (msg['lobbyid'] >= lobbies.length) {
      socket.emit('join lobby', { ret: 'fail' } );
    }
    else {
      var lobby = lobbies[msg['lobbyid']];
      lobby[lobby.length] = { sock: socket, uname: msg['uname'] };
      lobbies[msg['lobbyid']] = lobby;
      socket.emit('join lobby', { ret: 'success' } );
    }
  });

  socket.on('disband lobby', function(msg){
    // msg { lobbyid:14 }
    lobbies.splice(msg['lobbyid'], 1);
    socket.emit('disband lobby', { ret: 'success' } );
  });

  socket.on('general message', function(msg){
    // msg { lobbyid:14, content:"hello there" }
    var lobby = lobbies[msg['lobbyid']];
    for (i = 0; i < lobby.length; i++) {   
      lobby[i]['sock'].emit('general message', msg['content']);
    }
  });
  
  socket.on('disconnect', function(){
    console.log('user disconnected');
    var lobsl = lobbies.length;
    for (i = 0; i < lobsl; i++) {
      var lobl = lobbies[i].length;
      for (j = 0; j < lobl; j++) {
        if (lobbies[i][j]['sock']['id'] == socket['id']) {
          lobbies[i].splice(j, 1);
          lobl--;
          break;
        }
      }
      if (lobl == 0) {
        lobbies.splice(i, 1);
        lobsl--;
        break;
      }
    }
  });

  socket.on('get lobbies', function(){
    socket.emit('get lobbies', lobbies);
  });
  
});

http.listen(port, function(){
  console.log('listening on *:' + port);
});
