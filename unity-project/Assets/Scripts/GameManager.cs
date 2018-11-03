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

    void StartPlayer(string id, int cl, int num)
    {
        Debug.Log("called");

        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(Resources.Load<GameObject>("ArcherOP"), playerInitPos[num], Quaternion.identity);
            Debug.Log("instantiate " + num + " at " + playerInitPos[num].ToString());
            player2.GetComponent<archerControllerOP>().playernum = num;
            player2.GetComponent<archerControllerOP>().id = id;
        }
        else if (player3 == null)
        {
            player3 = Instantiate(Resources.Load<GameObject>("ArcherOP"), playerInitPos[num], Quaternion.identity);
            Debug.Log("instantiate " + num + " at " + playerInitPos[num].ToString());
            player3.GetComponent<archerControllerOP>().playernum = num;
            player3.GetComponent<archerControllerOP>().id = id;
        }
    }

    IEnumerator NewPlayerHandle(string id, int cl, int num)
    {
        Debug.Log("handling " + num.ToString());
        while (!gameStarted)
        {
            yield return new WaitForEndOfFrame();
        }
        StartPlayer(id, cl, num);
    }

    void StartGameHandle()
    {
        StartGame();
    }
}

