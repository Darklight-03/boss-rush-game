using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collision : MonoBehaviour {

    private Collider2D hitbox;
    private bool shielding;
    // Use this for initialization
    void Start()
    {
        hitbox = GetComponent<BoxCollider2D>();
        shielding = false;
    }

    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown(KeyCode.E))
        {
            Debug.Log("e pressed");
            StartCoroutine(shield());
        }
		
	}

    IEnumerator shield()
    {
        shielding = true;
        Debug.Log("shield enabled");
        GetComponentInParent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(1.5f);
        GetComponentInParent<SpriteRenderer>().color = Color.white;
        shielding = false;
        Debug.Log("shield disabled");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Physics2D.IgnoreCollision(hitbox, (Collider2D)collision.gameObject.GetComponent<BoxCollider2D>());
        }

        if (!shielding)
        {
            Physics2D.IgnoreCollision(hitbox, (Collider2D)collision.gameObject.GetComponent<BoxCollider2D>());
        }
    }
}
