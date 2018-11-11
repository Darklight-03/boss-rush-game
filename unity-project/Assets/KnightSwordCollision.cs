using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSwordCollision : MonoBehaviour {

    BoxCollider2D sword;

	// Use this for initialization
	void Start () {
        sword = GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision((Collider2D)sword, (Collider2D)collision.gameObject.GetComponent<BoxCollider2D>());
        }

    }
}
