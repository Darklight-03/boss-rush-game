﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    SpriteRenderer render;
    public Health health;
    public float speed;
    public Animation animation;
    Vector2 prevPos;


    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SocketNetworkManager.DealDamageHandle += DealDamageHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.DealDamageHandle -= DealDamageHandle;
    }

    IEnumerator DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        if (sender != SocketNetworkManager.id)
        {
            // dir could be used for knockback or something like that.
            // display health, if dead, etc
            Debug.Log("recieved damage message");
            if (health.TakeDamage(dmg))
            {
                StartCoroutine(damageAnimation());
            }
            else
            {
                Destroy(GetComponent<Transform>().parent);
            }
        }
        yield break;
    }

    // called in fixed interval
    void FixedUpdate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;

        //rb.velocity = v2 - v1;
    }

    // Update is called once per frame
    void Update ()
    {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        if ((v1-v2).magnitude < 3)
        {
            snm.sendMessage("ba", "{ \"name\": \"" + "huijian" + "\" }");
            animation.Play("huijian");
        }

        if (Vector2.Distance(prevPos, rb.position) > 0.1f)
        {
            snm.sendMessage("bp", "{ \"x\": " + rb.position.x.ToString() + " , \"y\": " + rb.position.y.ToString() + ", \"rx\": " + "0" + ", \"ry\": " + "0" + " }");
            prevPos = rb.position;
        }
	}

    IEnumerator damageAnimation()
    {
      for(int i = 10;i>0;i--){
        Color lerp = Color.Lerp(Color.white,Color.red,(float)i/10);
        render.color = lerp;
        yield return null;
      }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "arrowOP(Clone)")
        {
            Destroy(collider.gameObject);
            return;
        }
        if (collider.gameObject.tag == "projectile")
        {
            Destroy(collider.gameObject);
            snm.sendMessage("dd", "{ \"dmg\": " + "10" + " , \"dirx\": " + 0 + ", \"diry\": " + 0 + " }");
            if (health.TakeDamage(10))
            {
                StartCoroutine(damageAnimation());
            }
            else
            {
                Destroy(this.gameObject);
            }
            // do stuff only for the circle collider
        }
    }
}
