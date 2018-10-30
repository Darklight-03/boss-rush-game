﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {
  private Transform t;
    WebSocket w;
    int lobbyid;

    // Use this for initialization
    IEnumerator Start () {
    t = GetComponent<Transform>();
    GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
	  GameObject obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
    //GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
    GameObject boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"),t);


            w = new WebSocket(new Uri("ws://10.254.16.97:3000/"));
            yield return StartCoroutine(w.Connect());
        StartCoroutine(listener());
        joinLobby(0);
        sendMessage("this is a message");
    }

    void OnDestroy()
    {
        w.Close();
    }

    void createLobby()
    {
        w.SendString("{ \"msgtype\":\"create lobby\" }");
    }

    void joinLobby(int lobbyid)
    {
        this.lobbyid = lobbyid;
        w.SendString("{ \"msgtype\":\"join lobby\", \"lobbyid\": " + lobbyid.ToString() + " }");
    }

    void sendMessage(string content)
    {
        w.SendString("{ \"msgtype\":\"general message\", \"lobbyid\": " + lobbyid.ToString() + ", \"content\": \"" + content + "\" }");
    }

    IEnumerator listener()
    {
        while (true)
        {
            string msg = w.RecvString();
            Debug.Log(msg);
            yield return null;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}

