using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
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


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
