using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitbox : MonoBehaviour {

  private Transform t;
	// Use this for initialization
	void Start () {
	  t = GetComponent<Transform>();	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
  void OnCollisionEnter2D(Collision2D collision){

  } 
}
