﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordDamage : MonoBehaviour {

    private Rigidbody2D rb;


    // Use this for initialization
    void Start () {
	    
	}
	

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player" && !collision.gameObject.name.Contains("OP"))
        {
            collision.gameObject.GetComponent<playerBase>().TakeDamage(10, Vector2.zero);
        }
    }
}
