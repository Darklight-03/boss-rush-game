using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {
  private Transform t;
  private WebSocket socket;
  private int lobbyid;
  private bool stoplistener;

	// Use this for initialization
	void Start () {
    t = GetComponent<Transform>();
    GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
		stoplistener = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnDestroy() {
    stoplistener = true;
    socket.close();
  }

  void socketInit () {
    socket = new WebSocket(new Uri("ws://10.254.16.97:3000/"), "you-good-protocol");
    socket.connect();
    StartCoroutine(listener);
  }

  void listener () {
    while (!stoplistener) {
      string msg = socket.recvString();
      Debug.Log(msg);
    }
  }

  void createLobby () {
    socket.SendString("{ msgtype:\"create lobby\" }");
  }
  
  void joinLobby (int lobbyid) {
    this.lobbyid = lobbyid;
    socket.SendString(string.Format("{ msgtype:\"join lobby\", lobbyid: {0} }", lobbyid)); 
  }
  
  void sendMessage (string content) {
    socket.SendString(string.Format("{ msgtype:\"general message\", lobbyid: {0}, content: {1} }", lobbyid, content));
   }
}


/*
var lobbyid = -1;
var socket = new WebSocket('ws://localhost:3000/', 'you-good-protocol');

socket.onopen = function (event) {
  console.log(event);
}


socket.onmessage = function(event) {
  console.log(event);
  msg = JSON.parse(event.data);
  switch(msg['msgtype']) {
    case 'create lobby':
      console.log(msg['lobbyid']);
      lobbyid = msg['lobbyid'];
    break;

    case 'join lobby':
      console.log(msg['ret']);
    break;

    case 'general message':
      console.log(msg);
    break;

    case 'get lobbies':
      console.log(msg);
    break;

  }
}


function createLobby() {
  var input = document.getElementById("unamec");
  if (input.value != '') {
    socket.send(JSON.stringify({ msgtype:'create lobby', uname: input.value }));
    input.value = '';
  }
  else {
  }
}

function joinLobby() {
  var inputName = document.getElementById("unamej");
  var inputID = document.getElementById("lobbyid");
  if (inputName.value != '' && inputID.value != '') {
    socket.send(JSON.stringify({ msgtype:'join lobby', uname: inputName.value, lobbyid: inputID.value }));
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
    socket.send(JSON.stringify({ msgtype:'general message', lobbyid: lobbyid, content: inputMessage.value }));
    inputMessage.value = '';
  }
  else {
  }
}
*/
