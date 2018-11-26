using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollision : MonoBehaviour {
    public int knockback;
    private Rigidbody2D rb;
    SocketNetworkManager snm;
    
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        knockback = 1;
        snm = new SocketNetworkManager();
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
            collision.gameObject.GetComponent<playerBase>().TakeDamage(10, dir);
        }
    }
}
