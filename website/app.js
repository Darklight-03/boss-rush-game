const fs = require('fs');
const util = require('util');
const express = require('express');
const app = express();
var path = require('path');
var bodyParser = require('body-parser');
var compression = require('compression');
var http = require('http').Server(app);
const port = 3000
const unityFolder = path.join(__dirname, 'unity-build');
var WebSocket = require('ws');
var io = new WebSocket.Server( { server: http } );

process.env.TZ = 'America/Chicago';
var log_file = fs.createWriteStream(__dirname + '/messages.log', {flags: 'w'});
function log_msg(sock, d) {
  log_file.write((new Date()) + " - " + sock.id + ": " + util.format(d) + '\n');
}

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

var lobbies = [];

io.on('connection', function(socket){
  socket.id = getUniqueID();
  socket.inlobby = false;
  socket.send(JSON.stringify({ msgtype: 'new connection', content: JSON.stringify({ yourid: socket['id'] }) }));

	console.log((new Date()) + ' user ' + socket.id + ' connected');
	log_msg(socket, "connected");

  socket.on('message', function (msgs) {

    console.log(msgs.toString('utf8'));
    log_msg(socket, msgs.toString('utf8'));
		msg = JSON.parse(msgs.toString('utf8'));
		
    switch (msg['msgtype']) {

      case 'create lobby':
        if (socket.inlobby) {
          // fail
          socket.send(JSON.stringify({ msgtype: 'create lobby', content: JSON.stringify({ playernum: -1, lobbyid: -1 }) }));
  			}
        else {
					// success
					socket.inlobby = true;
					var lobby = { name: msg['name'], members: [ socket ] };
					var thislobbyid = lobbies.length;
					lobbies[thislobbyid] = lobby;
					socket.send(JSON.stringify({ msgtype: 'create lobby', content: JSON.stringify({ playernum: 0, lobbyid: thislobbyid }) }));
        }
      break;

      case 'join lobby':
        var lobby = lobbies[msg['lobbyid']];
        if (lobby == undefined || lobby.members.length >= 3 || socket.inlobby) {
          socket.send(JSON.stringify({ msgtype: 'join lobby', content: JSON.stringify({ lobbyid: -1, playernum: -1, ret: 'fail' }) }));
        }
        else {
			// send messages to each lobby member about the new member
			// send messages to the new member about each lobby member
			for (i = 0; i < lobby.members.length; i++) {
				console.log("sent to " + lobby.members[i]['id']);
				lobby.members[i].send(JSON.stringify({ msgtype: 'new player', content: JSON.stringify({ theirnum: lobby.members.length, theirid: socket['id'], cl: 0 }) }));
				console.log("sent to " + socket.id + "-");
				socket.send(JSON.stringify({ msgtype: 'new player', content: JSON.stringify({ theirnum: i, theirid: lobby.members[i]['id'], cl: 0 }) }));
			}
			socket.inlobby = true;
          lobby.members[lobby.members.length] = socket;
          lobbies[msg['lobbyid']] = lobby;
          socket.send(JSON.stringify({ msgtype: 'join lobby', content: JSON.stringify({ lobbyid: msg['lobbyid'], playernum: lobby.members.length - 1, ret: 'success' }) }));
        }
      break;

      case 'disband lobby':
        lobbies.splice(msg['lobbyid'], 1);
        socket.send(JSON.stringify({ msgtype: 'disband lobby', content: JSON.stringify({ ret: 'success' }) }));
      break;

      case 'select class':
        var lobby = lobbies[msg['lobbyid']];
        var good = true;
        for (i = 0; i < lobby.members.length; i++) {
          if (msg['plclass'] == lobby.members[i]['plclass'] && msg['plclass'] != "None") {
            socket.send(JSON.stringify({ msgtype: 'select class', content: JSON.stringify({ ret: 'fail' }) }));
            good = false;
          }
        }
        if (good) {
          for (i = 0; i < lobby.members.length; i++) {
            if (lobby.members[i]['id'] == socket.id) {
              lobby.members[i]['plclass'] = msg['plclass'];
              socket.send(JSON.stringify({ msgtype: 'select class', content: JSON.stringify({ ret: 'success' }) }));
            }
            else {
              lobby.members[i].send(JSON.stringify({msgtype: 'update class', content: JSON.stringify({ player: socket.id, plclass: msg['plclass'] }) }))
            }
          }
        }
      break;

      case 'general message':
        var lobby = lobbies[msg['lobbyid']];
        if (lobby != null) {
          for (i = 0; i < lobby.members.length; i++) {
            if (lobby.members[i]['id'] != socket['id']) {
              lobby.members[i].send(JSON.stringify({ msgtype: 'general message', content: JSON.stringify({ sender: socket['id'], ct: msg['ct'], content: JSON.stringify(msg['content']) }) }));
      }
          }
        }
      break;

      case 'get lobbies':
        var lobbiesInfo = [];
        for (i = 0; i < lobbies.length; i++) {
          var lobbyInfo = {};
          var lobby = lobbies[i];
          for (j = 0; j < lobby.members.length; j++) {
           
    			}
					lobbyInfo['players'] = lobby.members.length;
					lobbyInfo['name'] = lobby.name;
          lobbiesInfo[lobbiesInfo.length] = lobbyInfo;
        }
        socket.send(JSON.stringify({ msgtype: 'get lobbies', content: JSON.stringify({ lobbiesInfo: lobbiesInfo }) }));
      break;

    }
  });

  socket.on('close', function(reasonCode, description) {
    console.log((new Date()) + ' user ' + socket.id + ' disconnected');
    log_msg(socket, "disconnected");
    var lobsl = lobbies.length;
    for (i = 0; i < lobsl; i++) {
      var lobl = lobbies[i].members.length;
      for (j = 0; j < lobl; j++) {
        if (lobbies[i].members[j]['id'] == socket['id']) {
          lobbies[i].members.splice(j, 1);
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
