using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour {
    private Transform t;
    private SocketNetworkManager snm;
    GameObject player;
    GameObject player2;
    GameObject player3;
    GameObject boss;
    GameObject obstacle1;
    private bool gameStarted = false;
    private int[] waitPlayers = new int[0];
    private string[] waitPlayerIds = new string[0];

    // Use this for initialization
    void Start () {
        snm = GetComponent<SocketNetworkManager>();
        t = GetComponent<Transform>();
        SocketNetworkManager.NewPlayerHandle += NewPlayerHandle;
        SocketNetworkManager.StartGameHandle += StartGameHandle;
    }

    private void OnDestroy()
    {
        SocketNetworkManager.NewPlayerHandle -= NewPlayerHandle;
        SocketNetworkManager.StartGameHandle -= StartGameHandle;
    }

    void StartGame()
    {
        gameStarted = true;
        player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"), new Vector2(2, -2), Quaternion.identity);
        obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
        boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"), new Vector2(-2, 2), Quaternion.identity, t);
        for (int i = 0; i < waitPlayers.Length; i++)
        {
            StartPlayer(waitPlayerIds[i], waitPlayers[i]);
        }
    }
	
	// Update is called once per frame
	void Update () {
        bool lDown = Input.GetKeyDown(KeyCode.L);
        bool oDown = Input.GetKeyDown(KeyCode.O);
        bool pDown = Input.GetKeyDown(KeyCode.P);

        if (lDown && SocketNetworkManager.isHost)
        {
            Debug.Log("lPressed");
            snm.sendMessage("sg", "{ }");
            StartGame();
        }
        if (oDown)
        {
            Debug.Log("oPressed");
            snm.createLobby();
        }
        if (pDown)
        {
            Debug.Log("pPressed");
            snm.joinLobby(0);
        }
    }

    void StartPlayer(string id, int cl)
    {
        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(Resources.Load<GameObject>("ArcherOP"), new Vector2(1.5f, -1.5f), Quaternion.identity);
            player2.GetComponent<archerControllerOP>().id = id;
        }
        else if (player3 == null)
        {
            player3 = Instantiate(Resources.Load<GameObject>("ArcherOP"), new Vector2(1.5f, -1.5f), Quaternion.identity);
            player3.GetComponent<archerControllerOP>().id = id;
        }
    }

    void NewPlayerHandle(string id, int cl)
    {
        if (gameStarted)
        {
            StartPlayer(id, cl);
        }
        else
        {
            waitPlayers[waitPlayers.Length] = cl;
            waitPlayerIds[waitPlayerIds.Length] = id;
        }
    }

    void StartGameHandle()
    {
        StartGame();
    }
}

