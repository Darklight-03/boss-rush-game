using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
<<<<<<< Updated upstream
  private Transform t;
    public Transform canvas;

	// Use this for initialization
	void Start () {
    t = GetComponent<Transform>();
    GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
	  GameObject obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
    //GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
    GameObject boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"),t);
        boss.transform.parent = canvas;
        boss.GetComponent<RectTransform>().localScale = new Vector3(32.0f, 32.0f, 32.0f);
        boss.GetComponent<RectTransform>().localPosition = Vector3.one;
=======

    private Transform t;
    private Transform canvas;
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
        canvas = GameObject.Find("Canvas").transform;
        snm = GetComponent<SocketNetworkManager>();
        t = GetComponent<Transform>();
        obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
        //boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"), t);
        SocketNetworkManager.NewPlayerHandle += NewPlayerHandle;
        SocketNetworkManager.StartGameHandle += StartGameHandle;
        playerInitPos.Add(new Vector2(2, -2));
        playerInitPos.Add(new Vector2(0, -2));
        playerInitPos.Add(new Vector2(-2, -2));
        playerClasses.Add("ArcherOP");
        playerClasses.Add("KnightOP");
        playerClasses.Add("PriestOP");
    }
>>>>>>> Stashed changes


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
