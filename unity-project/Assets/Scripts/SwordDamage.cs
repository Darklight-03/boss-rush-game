using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour {

    private Rigidbody2D rb;


    // Use this for initialization
    void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter2D(Collider2D collision)
    {

        //Vector3 playerpos = collision.gameObject.transform.position;
        //Vector3 spikepos = rb.transform.position;
        //Vector3 dir = (playerpos - spikepos).normalized * knockback;
        if (collision.gameObject.tag == "Player" && !collision.gameObject.name.Contains("OP"))
        {
          collision.gameObject.GetComponent<archerController>().TakeDamage(10, Vector2.zero);
        }
        


    }

}
