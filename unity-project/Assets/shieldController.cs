using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shieldController : MonoBehaviour {

    BoxCollider2D shield;
    


	// Use this for initialization
	void Start () {
        shield = GetComponent<BoxCollider2D>();
        shield.enabled = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("e pressed");
            StartCoroutine(raise_shield());
        }
    }

    IEnumerator raise_shield()
    {
        shield.enabled = true;
        Debug.Log("shield enabled");
        yield return new WaitForSeconds(1.5f);
        shield.enabled = false;
        Debug.Log("shield disabled");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("collision ignored");
            Physics2D.IgnoreCollision((Collider2D)shield, (Collider2D)collision.gameObject.GetComponent<BoxCollider2D>());
        }
        else
        {
            Debug.Log("collision successful");
        }

    }
}
