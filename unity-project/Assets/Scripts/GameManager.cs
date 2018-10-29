using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  private Transform t;

	// Use this for initialization
	void Start () {
    t = GetComponent<Transform>();
    GameObject player = (GameObject)Instantiate(Resources.Load<GameObject>("Archer"),t);
	GameObject obstacle1 = (GameObject)Instantiate(Resources.Load<GameObject>("rockspread"), t);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
