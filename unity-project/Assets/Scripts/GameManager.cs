using System.Collections;
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
        Vector2 archerpos = new Vector2(2, -2);
        Vector2 bosspos = new Vector2(-2, 2);
    GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),archerpos,Quaternion.identity);
	  GameObject obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
    //GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
    GameObject boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"),bosspos,Quaternion.identity,t);


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
        string msg;
        while (true)
        {
            msg = w.RecvString();
            if (msg != null)
            {
                Debug.Log(msg);
            }
            yield return null;
        }
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}

