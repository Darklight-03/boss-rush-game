using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private Transform t;

	// Use this for initialization
	void Start () {
    t = GetComponent<Transform>();
        Vector2 archerpos = new Vector2(2, -2);
        Vector2 bosspos = new Vector2(-2, 2);
    GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("knight"),archerpos,Quaternion.identity);
	 // GameObject obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
    //GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
    //GameObject boss = (GameObject)Instantiate(Resources.Load<GameObject>("boss"),bosspos,Quaternion.identity,t);
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
