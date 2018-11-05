using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionIgnore : MonoBehaviour {

    private Collider2D boss;
    // Use this for initialization
    void Start()
    {
        boss = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(boss, (Collider2D)collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
