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
    private Queue<int> plnums = new Queue<int>();
    private Queue<string> plids = new Queue<string>();
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
        Debug.Log("SHIT " + plnums.Count.ToString());
        for (int i = 0; i < plnums.Count; i++)
        {
            StartPlayer();
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
        int plnum = plnums.Dequeue();
        string plid = plids.Dequeue();
        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(Resources.Load<GameObject>("ArcherOP"), playerInitPos[plnum], Quaternion.identity);
            Debug.Log("instantiate " + plnum + " at " + playerInitPos[plnum].ToString());
            player2.GetComponent<archerControllerOP>().playernum = plnum;
            player2.GetComponent<archerControllerOP>().id = plid;
        }
        else if (player3 == null)
        {
            player3 = Instantiate(Resources.Load<GameObject>("ArcherOP"), playerInitPos[plnum], Quaternion.identity);
            Debug.Log("instantiate " + plnum + " at " + playerInitPos[plnum].ToString());
            player3.GetComponent<archerControllerOP>().playernum = plnum;
            player3.GetComponent<archerControllerOP>().id = plid;
            Debug.Log(player2.GetComponent<archerControllerOP>().id);
            Debug.Log(player3.GetComponent<archerControllerOP>().id);
        }
    }

    void NewPlayerHandle(string id, int cl, int num)
    {
        Debug.Log("handling " + num.ToString());
        if (gameStarted)
        {
            plnums.Enqueue(num);
            plids.Enqueue(id);
            StartPlayer();
        }
        else
        {
            plnums.Enqueue(num);
            plids.Enqueue(id);
        }
    }

    void StartGameHandle()
    {
        StartGame();
    }
}

