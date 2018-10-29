var app = require('express')();
var http = require('http').Server(app);
var io = require('socket.io')(http);
var port = process.env.PORT || 3000;

app.get('/', function(req, res){
  res.sendFile(__dirname + '/index.html');
});

app.get('/index.css', function(req, res){
  res.sendFile(__dirname + '/index.css');
});

app.get('/index-script.js', function(req, res){
  res.sendFile(__dirname + '/index-script.js');
});

app.get('/assets/logo.png', function(req, res){
  res.sendFile(__dirname + '/assets/logo.png');
});


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