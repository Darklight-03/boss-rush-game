const express = require('express')
const app = express()
var path = require('path')
var bodyParser = require('body-parser')
var compression = require('compression')
var http = require('http').Server(app);
const port = 3000
const unityFolder = path.join(__dirname, 'unity-build');
var WebSocket = require('ws');
var io = new WebSocket.Server( { server: http } );

 
app.use(bodyParser.json());
app.use(compression());
app.use(express.urlencoded({ extended: true }));
app.use(express.static(unityFolder));
app.use(defaultRoute);
app.use(express.static('index'));
app.use(express.static('assets'));


http.listen(port);


function defaultRoute(req, response) {
	    response.sendfile(unityFolder + '/index.html');
}

function extraStringify(inobj) {
  return "\"" + JSON.stringify(inobj) + "\""; 
}

//console.log(io);

var lobbies = [];

io.on('connection', function(socket){
  //console.log(socket);
  socket.id = getUniqueID();
  console.log('user connected');
  socket.send(JSON.stringify({ msgtype: 'new connection', content: JSON.stringify({ yourid: socket['id'] }) }));

  socket.on('message', function (msgs) {
    console.log(msgs.toString('utf8'));
    msg = JSON.parse(msgs.toString('utf8'));
    switch (msg['msgtype']) {
      case 'create lobby':
        // msg { }
        var lobby = [ { sock: socket } ];
        var thislobbyid = lobbies.length;
        lobbies[thislobbyid] = lobby;
		    console.log(0 + " " + socket['id']);
        socket.send(JSON.stringify({ msgtype: 'create lobby', content: JSON.stringify({ playernum: 0, lobbyid: thislobbyid }) }));
      break;

      case 'join lobby':
        // msg { lobbyid:14 }
        var lobby = lobbies[msg['lobbyid']];
        if (lobby == undefined) {
          socket.send(JSON.stringify({ msgtype: 'join lobby', content: JSON.stringify({ playernum: -1, ret:'fail' }) }));
        }
        else {
	  for (i = 0; i < lobby.length; i++) {
		  console.log(lobby.length + " " + socket['id']);
            lobby[i]['sock'].send(JSON.stringify({ msgtype: 'new player', content: JSON.stringify({ theirnum: lobby.length, theirid: socket['id'], cl: 0 }) }));
		  console.log(i + " " + lobby[i]['sock']['id']);
	    socket.send(JSON.stringify({ msgtype: 'new player', content: JSON.stringify({ theirnum: i, theirid: lobby[i]['sock']['id'], cl: 0 }) }));
	  }
          lobby[lobby.length] = { sock: socket };
          lobbies[msg['lobbyid']] = lobby;
          socket.send(JSON.stringify({ msgtype: 'join lobby', content: JSON.stringify({ playernum: lobby.length - 1, ret: 'success' }) }));
        }
      break;

      case 'disband lobby':
        // msg { lobbyid:14 }
        lobbies.splice(msg['lobbyid'], 1);
        socket.send(JSON.stringify({ msgtype: 'disband lobby', content: JSON.stringify({ ret: 'success' }) }));
      break;

      case 'general message':
        // msg { lobbyid:14, content:"hello there" }
        var lobby = lobbies[msg['lobbyid']];
        if (lobby != null) {
          for (i = 0; i < lobby.length; i++) {
            if (lobby[i]['sock']['id'] != socket['id']) {
              lobby[i]['sock'].send(JSON.stringify({ msgtype: 'general message', content: JSON.stringify({ sender: socket['id'], ct: msg['ct'], content: JSON.stringify(msg['content']) }) }));
	    }
          }
        }
      break;

      case 'get lobbies':
        socket.send(JSON.stringify({ msgtype: 'get lobbies', content: JSON.stringify({ lob: lobbies }) }));
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


getUniqueID = function () {
    function s4() {
        return Math.floor((1 + Math.random()) * 0x10000).toString(16).substring(1);
    }
    return s4() + s4() + '-' + s4();
};
