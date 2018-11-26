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
    public GameObject StartGameButton;
    private bool gameStarted = false;
    private List<Vector2> playerInitPos = new List<Vector2>(3);
    private List<string> playerClasses = new List<string>();

    // Use this for initialization
    void Start () {
        snm = GetComponent<SocketNetworkManager>();
        t = GetComponent<Transform>();
        obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
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
        gameStarted = true;
        for (int i = 0; i < SocketNetworkManager.newplayers.Count; i++)
        {
            newPly t = SocketNetworkManager.newplayers.Dequeue();
            StartPlayer(t.theirid, t.cl, t.theirnum);
        }
        //Debug.Log("instantiate " + SocketNetworkManager.playernum.ToString() + " at " + playerInitPos[SocketNetworkManager.playernum].ToString());
        player = (GameObject)Instantiate(Resources.Load<GameObject>("knight"), playerInitPos[SocketNetworkManager.playernum], Quaternion.identity);
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

    void StartPlayer(string id, int cl, int num)
    {
        cl = 1;
        // argument will specify class later
        if (player2 == null)
        {
            player2 = Instantiate(Resources.Load<GameObject>(playerClasses[1]), playerInitPos[num], Quaternion.identity);
            Debug.Log("instantiate " + id + " at " + playerInitPos[num].ToString());
            if (cl == 0)
            {
                player2.GetComponent<archerControllerOP>().playernum = num;
                player2.GetComponent<archerControllerOP>().id = id;
                player2.GetComponent<archerControllerOP>().healthbar_id = 1;
            }
            else if (cl == 1)
            {
                Debug.Log(num);
                player2.GetComponent<knightControllerOP>().playernum = num;
                player2.GetComponent<knightControllerOP>().id = id;
                player2.GetComponent<knightControllerOP>().healthbar_id = 1;
            }
        }
        else if (player3 == null)
        {
            player3 = Instantiate(Resources.Load<GameObject>(playerClasses[1]), playerInitPos[num], Quaternion.identity);
            Debug.Log("instantiate " + id + " at " + playerInitPos[num].ToString());
            if (cl == 0)
            {
                player3.GetComponent<archerControllerOP>().playernum = num;
                player3.GetComponent<archerControllerOP>().id = id;
                player3.GetComponent<archerControllerOP>().healthbar_id = 2;
            }
            else if (cl == 1)
            {
                Debug.Log(num);
                player3.GetComponent<knightControllerOP>().playernum = num;
                player3.GetComponent<knightControllerOP>().id = id;
                player3.GetComponent<knightControllerOP>().healthbar_id = 2;
            }
        }
    }


    IEnumerator StartGameHandle()
    {
        StartGame();
        yield return null;
    }

    public void OnStartButtonPress()
    {
        StartGameButton.SetActive(false);
        snm.sendMessage("sg", "{ }");
        StartGame();
    }
}

