using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    SpriteRenderer render;
    Health health;
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
        // dir could be used for knockback or something like that.
        // display health, if dead, etc
        if (health.TakeDamage(dmg))
        {

        }
        else
        {
            Destroy(this);
        }
        yield break;
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
            snm.sendMessage("ba", "{ \"name\": \"" + "huijian" + "\" }");
            animation.Play("huijian");
        }

        if (Vector2.Distance(prevPos, rb.position) > 0.1f)
        {
            snm.sendMessage("bp", "{ \"x\": " + rb.position.x.ToString() + " , \"y\": " + rb.position.y.ToString() + ", \"rx\": " + "0" + ", \"ry\": " + "0" + " }");
            prevPos = rb.position;
        }
	}

    public void TakeDamage(float i){
      if(health.TakeDamage(i)){
        StartCoroutine(damageAnimation());
      }
      else{
        Destroy(this);
      }
    }

    IEnumerator damageAnimation(){
      for(int i = 10;i>0;i--){
        Color lerp = Color.Lerp(Color.white,Color.red,(float)i/10);
        render.color = lerp;
        yield return null;
      }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "arrowOP")
        {
            Destroy(collision.gameObject);
            return;
        }
        if (collision.gameObject.tag == "projectile")
        {
            Destroy(collision.gameObject);
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
