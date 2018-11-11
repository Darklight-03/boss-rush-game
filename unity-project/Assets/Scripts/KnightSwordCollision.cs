using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSwordCollision : MonoBehaviour {

    EdgeCollider2D swordcoll;

	// Use this for initialization
	void Start () {
        swordcoll = GetComponent<EdgeCollider2D>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss" && this.transform.parent.gameObject.GetComponent<knightController>().stabbing)
        {
            Debug.Log("damaged boss");
            playerController script1 = collision.gameObject.GetComponent<playerController>();
            playerControllerOP script2 = collision.gameObject.GetComponent<playerControllerOP>();
            if (script1 != null)
                script1.TakeDamage(10);
            if (script2 != null)
                script2.TakeDamage(10);
        }
    }
}
