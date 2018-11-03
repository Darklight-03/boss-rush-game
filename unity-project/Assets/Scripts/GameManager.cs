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
    private List<GameObject> players = new List<GameObject>(2);
    private List<Vector2> playerInitPos = new List<Vector2>(3);

    // Use this for initialization
    void Start () {
        snm = GetComponent<SocketNetworkManager>();
        t = GetComponent<Transform>();
        SocketNetworkManager.NewPlayerHandle += NewPlayerHandle;
        SocketNetworkManager.StartGameHandle += StartGameHandle;
        playerInitPos.Add(new Vector2(2, -2));
        playerInitPos.Add(new Vector2(0, -2));
        playerInitPos.Add(new Vector2(-2, -2));
    }

    private void OnDestroy()
    {
        SocketNetworkManager.NewPlayerHandle -= NewPlayerHandle;
        SocketNetworkManager.StartGameHandle -= StartGameHandle;
    }

    void StartGame()
    {
        gameStarted = true;
        player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"), playerInitPos[SocketNetworkManager.playernum], Quaternion.identity);
        obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
        //boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"), new Vector2(-2, 2), Quaternion.identity, t);
        for (int i = 0; i < players.Count; i++)
        {
            StartPlayer(players[i]);
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

    void StartPlayer(GameObject newPlayer)
    {
        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(newPlayer, playerInitPos[newPlayer.GetComponent<archerControllerOP>().playernum], Quaternion.identity);
        }
        else if (player3 == null)
        {
            player3 = Instantiate(newPlayer, playerInitPos[newPlayer.GetComponent<archerControllerOP>().playernum], Quaternion.identity);
        }
    }

    void NewPlayerHandle(string id, int cl, int num)
    {
        GameObject newPlayer = Resources.Load<GameObject>("ArcherOP");
        newPlayer.GetComponent<archerControllerOP>().id = id;
        newPlayer.GetComponent<archerControllerOP>().playernum = num;
        if (gameStarted)
        {
            StartPlayer(newPlayer);
        }
        else
        {
            Debug.Log("myid        = " + SocketNetworkManager.id);
            Debug.Log("newplayerid = " + id);
            players.Add(newPlayer);
        }
    }

    void StartGameHandle()
    {
        StartGame();
    }
}

