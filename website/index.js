const express = require('express');
var app = express();
var http = require('http').Server(app);
var WebSocketServer = require('websocket').server;
var port = 3000;

app.get('/', function(req, res){
  res.sendFile(__dirname + '/index/index.html');
});

app.use(express.static('index'));
app.use(express.static('assets'));

io = new WebSocketServer({
  httpServer: http
});

var lobbies = [];

io.on('request', function(request){
  var socket = request.accept('you-good-protocol', request.origin);
  socket.id = getUniqueID();
  console.log('user connected');

  socket.on('message', function(fmsg) {
    msg = JSON.parse(fmsg['utf8Data']);
    switch (msg['msgtype']) {
      case 'create lobby':
        // msg { uname:"username" }
        var lobby = [ { sock: socket, uname: msg['uname'] } ];
        var thislobbyid = lobbies.length;
        lobbies[thislobbyid] = lobby;
        //socket.sendUTF(JSON.stringify({ msgtype:'create lobby', { lobbyid: thislobbyid } );
        socket.sendUTF(JSON.stringify({ msgtype: 'create lobby', lobbyid: thislobbyid }));
      break;

      case 'join lobby':
        // msg { uname:"username", lobbyid:14 }
        if (msg['lobbyid'] >= lobbies.length) {
          socket.sendUTF(JSON.stringify({ msgtype: 'join lobby', ret:'fail' }));
        }
        else {
          var lobby = lobbies[msg['lobbyid']];
          lobby[lobby.length] = { sock: socket, uname: msg['uname'] };
          lobbies[msg['lobbyid']] = lobby;
          socket.sendUTF(JSON.stringify({ msgtype: 'join lobby', ret: 'success' }));
        } 
      break;

      case 'disband lobby':
        // msg { lobbyid:14 }
        lobbies.splice(msg['lobbyid'], 1);
        socket.sendUTF(JSON.stringify({ msgtype: 'disband lobby', ret: 'success' })); 
      break;

      case 'general message':
        // msg { lobbyid:14, content:"hello there" }
        var lobby = lobbies[msg['lobbyid']];
        for (i = 0; i < lobby.length; i++) {   
          lobby[i]['sock'].sendUTF(JSON.stringify({ msgtype: 'general message', content: msg['content'] }));
        }       
      break;

      case 'get lobbies':
        socket.sendUTF(JSON.stringify({ msgtype: 'get lobbies', lob: lobbies }));
      break;

    }
    console.log(lobbies);
  });

  socket.on('close', function(reasonCode, description) {
    console.log((new Date()) + ' Peer ' + socket.remoteAddress + ' disconnected.');
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
    console.log(lobbies);
  });

});

http.listen(port, function(){
  console.log('listening on *:' + port);
});

getUniqueID = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4();
};
