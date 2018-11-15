using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public int knockback;
    private Rigidbody2D rb;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        knockback = 1;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector3 playerpos = collision.gameObject.transform.position;
        Vector3 spikepos =rb.transform.position;
        Vector3 dir = (playerpos - spikepos).normalized * knockback;
        if (collision.gameObject.tag == "Player")
        {
            if (collision.gameObject.name == "Archer(Clone)")
            {
                collision.gameObject.GetComponent<archerController>().TakeDamage(10, dir);
            }
            else if (collision.gameObject.name == "knight(Clone)")
            {
                collision.gameObject.GetComponent<knightController>().TakeDamage(10, dir);
            }
            //else if (collision.gameObject.name == "Priest")
            //{
            //    //collision.gameObject.GetComponent<priestController>().TakeDamage(10, dir);
            //}
        }
    }
}
