using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class projectile : MonoBehaviour {

  SpriteRenderer render;
	// Use this for initialization
	void Start () {
    render = GetComponent<SpriteRenderer>();
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void OnCollisionEnter2D(Collision2D collision){
    if(collision.gameObject.tag != "Player")
    {
      Destroy(this.gameObject);
    }
  }
}
