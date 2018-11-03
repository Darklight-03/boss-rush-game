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
    int pl2num = -1;
    int pl3num = -1;
    string pl2id;
    string pl3id;
    private List<Vector2> playerInitPos = new List<Vector2>(3);

    // Use this for initialization
    void Start () {
        snm = GetComponent<SocketNetworkManager>();
        t = GetComponent<Transform>();
        obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
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
        Debug.Log("instantiate " + SocketNetworkManager.playernum.ToString() + " at " + playerInitPos[SocketNetworkManager.playernum].ToString());
        player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"), playerInitPos[SocketNetworkManager.playernum], Quaternion.identity);
        //boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"), new Vector2(-2, 2), Quaternion.identity, t);
        if (pl2num != -1)
        {
            StartGame();
        }
        if (pl3num != -1)
        {
            StartGame();
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

    void StartPlayer()
    {
        Debug.Log("called");

        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(Resources.Load<GameObject>("ArcherOP"), playerInitPos[pl2num], Quaternion.identity);
            Debug.Log("instantiate " + pl2num + " at " + playerInitPos[pl2num].ToString());
            player2.GetComponent<archerControllerOP>().playernum = pl2num;
            player2.GetComponent<archerControllerOP>().id = pl2id;
        }
        else if (player3 == null)
        {
            player3 = Instantiate(Resources.Load<GameObject>("ArcherOP"), playerInitPos[pl3num], Quaternion.identity);
            Debug.Log("instantiate " + pl3num + " at " + playerInitPos[pl3num].ToString());
            player3.GetComponent<archerControllerOP>().playernum = pl3num;
            player3.GetComponent<archerControllerOP>().id = pl3id;
            Debug.Log(player2.GetComponent<archerControllerOP>().id);
            Debug.Log(player3.GetComponent<archerControllerOP>().id);
        }
    }

    void NewPlayerHandle(string id, int cl, int num)
    {
        Debug.Log("handling " + num.ToString());
        if (pl2num == -1)
        {
            pl2num = num;
            pl2id = id;
        }
        else
        {
            pl3num = num;
            pl3id = id;
        }

        if (gameStarted)
        {

            StartPlayer();
        }
        else
        {
            
        }
    }

    void StartGameHandle()
    {
        StartGame();
    }
}

