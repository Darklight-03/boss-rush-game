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
    private List<string> playerClasses = new List<string>();

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
        playerClasses.Add("ArcherOP");
        playerClasses.Add("KnightOP");
        playerClasses.Add("PriestOP");
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
            Debug.Log("l Pressed");
            snm.sendMessage("sg", "{ }");
            StartGame();
        }
        if (oDown)
        {
            Debug.Log("o Pressed");
            snm.createLobby();
        }
        if (pDown)
        {
            Debug.Log("p Pressed");
            snm.joinLobby(0);
        }
    }

    void StartPlayer(string id, int cl, int num)
    {
        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(Resources.Load<GameObject>(playerClasses[cl]), playerInitPos[num], Quaternion.identity);
            Debug.Log("instantiate " + num + " at " + playerInitPos[num].ToString());
            player2.GetComponent<archerControllerOP>().playernum = num;
            player2.GetComponent<archerControllerOP>().id = id;
        }
        else if (player3 == null)
        {
            player3 = Instantiate(Resources.Load<GameObject>(playerClasses[cl]), playerInitPos[num], Quaternion.identity);
            Debug.Log("instantiate " + num + " at " + playerInitPos[num].ToString());
            player3.GetComponent<archerControllerOP>().playernum = num;
            player3.GetComponent<archerControllerOP>().id = id;
        }
    }

    IEnumerator NewPlayerHandle(string id, int cl, int num)
    {
        Debug.Log("new player joined lobby");
        yield return new WaitUntil(() => gameStarted);
        StartPlayer(id, cl, num);
    }

    void StartGameHandle()
    {
        StartGame();
    }
}

