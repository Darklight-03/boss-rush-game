using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    Health health;
    public float speed;
    public Animation animation;
    Vector2 prevPos;


    // Use this for initialization
    void Start ()
    {
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        SocketNetworkManager.DealDamageHandle += DealDamageHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.DealDamageHandle -= DealDamageHandle;
    }

    void DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        // display health, if dead, etc
    }

    // called in fixed interval
    void FixedUpdate()
    {

        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;

        rb.velocity = v2 - v1;

    }

    // Update is called once per frame
    void Update ()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        if ((v1-v2).magnitude < 3)
        {
            animation.Play("huijian");
        }

        if (Vector2.Distance(prevPos, rb.position) > 0.1f)
        {
            snm.sendMessage("bp", "{ \"x\": " + rb.position.x.ToString() + " , \"y\": " + rb.position.y.ToString() + ", \"rx\": " + 0.ToString() + ", \"ry\": " + 0.ToString() + " }");
            prevPos = rb.position;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "projectile")
        {
            Debug.Log(health.getCurrentHP());
            if (health.TakeDamage(10))
            {

            }
            else
            {
                Destroy(this);
            }
            // do stuff only for the circle collider
        }
    }
}
