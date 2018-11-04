using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerOP : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    Health health;
    public float speed;
    public Animation animation;


    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        SocketNetworkManager.UpdateBossPositionHandle += UpdateBossPositionHandle;
        SocketNetworkManager.BossAnimHandle += BossAnimHandle;
        SocketNetworkManager.DealDamageHandle += DealDamageHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.UpdateBossPositionHandle -= UpdateBossPositionHandle;
        SocketNetworkManager.BossAnimHandle -= BossAnimHandle;
        SocketNetworkManager.DealDamageHandle -= DealDamageHandle;
    }



    IEnumerator UpdateBossPositionHandle(float x, float y, float rx, float ry)
    {
        Vector2 pos = transform.position;
        pos.x = x;
        pos.y = y;
        transform.position = pos;

        Vector2 dir = new Vector2(rx, ry);

        // use angle to do something probably with the sword
        yield break;
    }

    IEnumerator BossAnimHandle(string name)
    {
        animation.Play(name);
        yield break;
    }

    IEnumerator DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        // dir could be used for knockback or something like that.
        // display health, if dead, etc
        Debug.Log("recieved damage message");
        if (health.TakeDamage(dmg))
        {

        }
        else
        {
            Destroy(this.gameObject); 
        }
        yield break;
    }

    // called in fixed interval
    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update ()
    {

	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "arrowOP")
        {
            Debug.Log("collision with other players arrow");
            Destroy(collider.gameObject);
            return;
        }
        if (collider.gameObject.tag == "projectile")
        {
            health.TakeDamage(10);
            Debug.Log("collision with arrow");
            snm.sendMessage("dd", "{ \"dmg\": " + "10" + " , \"dirx\": " + 0 + ", \"diry\": " + 0 + " }");
            Destroy(collider.gameObject);
        }
    }
}
