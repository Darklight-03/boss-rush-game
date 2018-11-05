using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    private GameObject healthbar;
    private GameObject healthbarbg;
    private Text bossname;
    int hit;
    int hbarupdatetime;
    Vector3 healthbarsize;


    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
        bossname = GameObject.FindWithTag("Boss-name").GetComponent<Text>();
        healthbar = GameObject.FindWithTag("Boss-health");
        healthbarbg = GameObject.FindWithTag("Boss-healthbh");
        hit = 0;
        healthbarsize = healthbar.transform.localScale;

    }

    private void OnEnable()
    {
        SocketNetworkManager.DealDamageHandle += DealDamageHandleH;
    }

    private void OnDisable()
    {
        SocketNetworkManager.DealDamageHandle -= DealDamageHandleH;
    }

    void DealDamageHandleH(string sender, float dmg, Vector2 dir)
    {
        StartCoroutine(DealDamageHandle(sender, dmg, dir));
    }
    IEnumerator DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        if (sender != SocketNetworkManager.id)
        {
            // dir could be used for knockback or something like that.
            // display health, if dead, etc
            TakeDamage(dmg);
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

        /* HEALTH BAR */
        if (hbarupdatetime == 0)
        {
            healthbarbg.transform.localScale = healthbar.transform.localScale;
            hbarupdatetime = 100;
        }
        else
        {
            hbarupdatetime--;
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

    void TakeDamage(float dmg)
    {

        var hsize = new Vector3((health.getCurrentHP() / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hit = 25;
        hbarupdatetime = 20;

        if (health.TakeDamage(10))
            {
                damageAnimation();
            }
            else
            {
                Destroy(this.gameObject);
            }
            // do stuff only for the circle collider
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
            TakeDamage(10);
            snm.sendMessage("dd", "{ \"dmg\": " + "10" + " , \"dirx\": " + 0 + ", \"diry\": " + 0 + " }");
            
            // do stuff only for the circle collider
        }
    }
}
