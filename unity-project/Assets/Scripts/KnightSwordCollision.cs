using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightSwordCollision : MonoBehaviour {

    EdgeCollider2D swordcoll;
    SocketNetworkManager snm;

	// Use this for initialization
	void Start () {
        swordcoll = GetComponent<EdgeCollider2D>();
        snm = new SocketNetworkManager();
	}

    
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Boss" && this.transform.parent.gameObject.GetComponent<knightController>().stabbing)
        {
            //Debug.Log("damaged boss");
            this.transform.parent.gameObject.GetComponent<knightController>().stabbing = false;
            snm.sendMessage("dd", "{ \"dmg\": " + "5" + " , \"dirx\": " + 0 + ", \"diry\": " + 0 + " }");
            BossHandle script1 = collision.gameObject.GetComponent<BossHandle>();
            BossHandleOP script2 = collision.gameObject.GetComponent<BossHandleOP>();
            if (script1 != null)
                script1.TakeDamage(5);
            if (script2 != null)
                script2.TakeDamage(5);
        }
    }
}
