using System.Collections;
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
            GameObject player = collision.gameObject;
            if (player.name == "Archer(Clone)")
            {
                player.GetComponent<archerController>().TakeDamage(10, Vector2.zero);
            }
            else if (player.name == "Knight(Clone)")
            {
                Debug.Log("damaged knight");
                player.GetComponent<knightController>().TakeDamage(10, Vector2.zero);
            }
            else if (player.name == "Priest(Clone)")
            {
                Debug.Log("Come uncomment this");
                //player.GetComponent<priestController>().TakeDamage(10, Vector2.zero);
            }
            else
            {
                Debug.Log(player.name);
                Debug.Log("Unknown sword collision HELP");
            }
        }
    }
}
