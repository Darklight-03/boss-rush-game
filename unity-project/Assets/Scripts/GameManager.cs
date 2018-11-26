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
    public GameObject selectMenu;
    public GameObject StartGameButton;
    private bool gameStarted = false;
    private List<Vector2> playerInitPos = new List<Vector2>(3);
    private List<string> playerClasses = new List<string>();

    // Use this for initialization
    void Start () {
        snm = GetComponent<SocketNetworkManager>();
        t = GetComponent<Transform>();
        obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread 1"), t);
        //boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"), t);

        playerInitPos.Add(new Vector2(2, -2));
        playerInitPos.Add(new Vector2(0, -2));
        playerInitPos.Add(new Vector2(-2, -2));
        playerClasses.Add("ArcherOP");
        playerClasses.Add("KnightOP");
        playerClasses.Add("PriestOP");
        if (SocketNetworkManager.isHost)
            StartGameButton.SetActive(true);
    }

    private void OnEnable()
    {
        SocketNetworkManager.StartGameHandle += StartGameHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.StartGameHandle -= StartGameHandle;
    }

    void StartGame()
    {
        Dictionary<string, int> plord = selectMenu.GetComponent<ClassSelectManager>().plord;
        selectMenu.SetActive(false);
        gameStarted = true;
        foreach (KeyValuePair<string, newPly> a in SocketNetworkManager.newplayers)
        {
            StartPlayer(a.Value, plord[a.Value.theirid]);
        }
        //Debug.Log("instantiate " + SocketNetworkManager.playernum.ToString() + " at " + playerInitPos[SocketNetworkManager.playernum].ToString());
        player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"), playerInitPos[SocketNetworkManager.playernum], Quaternion.identity);
        if (SocketNetworkManager.isHost)
        {
            boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"), t);
        }
        else
        {
            boss = Instantiate(Resources.Load<GameObject>("bossOP"), new Vector2(-2, 2), Quaternion.identity, t);
        }
    }

	
	// Update is called once per frame
	void Update () {
    }

    void StartPlayer(newPly a, int ord)
    {
        //// argument will specify class later
        GameObject player = Instantiate(Resources.Load<GameObject>(a._plclass), playerInitPos[a.theirnum], Quaternion.identity);
        player.GetComponent<playerBase>.playernum = a.theirnum;
        player.GetComponent<playerBase>.id = a.theirid;
        player.GetComponent<playerBase>.healthbar_id = ord;
    }


    IEnumerator StartGameHandle()
    {
        bool allready = true;
        foreach (KeyValuePair<string, newPly> a in SocketNetworkManager.newplayers)
        { 
            if (a.Value._plclass == "None")
            {
                allready = false;
                yield break;
            }
        }
        if (allready)
            StartGame();
        else
            snm.logText("Cannot start not all players have selected a class");
        yield return null;
    }

    public void OnStartButtonPress()
    {
        StartGameButton.SetActive(false);
        snm.sendMessage("startgame", "{ }");
        StartGame();
    }
}

