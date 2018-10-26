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


io.on('connection', function(socket){
  console.log('user connected');

  /*
  socket.on('chat message', function(msg){
    io.emit('chat message', msg);
  });
  */

  socket.on('disconnect', function(){
    console.log('user disconnected');
  });
  
});

http.listen(port, function(){
  console.log('listening on *:' + port);
});
