using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class knightController : MonoBehaviour {

    private Rigidbody2D rb;
    private GameObject shield;
    private float bowdistance;
    private KnightHealth health;
    private SpriteRenderer render;
    private GameObject healthbar;
    private GameObject healthbarback;
    private Text playername;
    int hbarupdatetime;
    Vector3 healthbarsize;
    int knocked;
    public float MOVEMENT_SPEED;
    List<Vector2> forces;
    Vector2 realvelocity;
    bool invincible;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        shield = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (shield.transform.position - (Vector3)rb.position).magnitude;
        health = GetComponent<KnightHealth>();
        render = GetComponent<SpriteRenderer>();
        healthbar = GameObject.FindWithTag("Health-bar");
        healthbarback = GameObject.FindWithTag("Health-bar-background");
        playername = GameObject.FindWithTag("Player-text").GetComponent<Text>();
        playername.text = "You: Knight";
        hbarupdatetime = 0;
        knocked = 0;
        forces = new List<Vector2>();
        realvelocity = new Vector2(0, 0);
        invincible = false;
        healthbarsize = healthbar.transform.localScale;

    }

    void FixedUpdate()
    {
        /* MOVEMENT */
        // input x and y
        float ix = Input.GetAxis("Horizontal");
        float iy = Input.GetAxis("Vertical");

        // get velocity input
        var inputvelocity = new Vector2(ix, iy);

        // later can add velocity vectors together for knockback and stuff
        if (knocked == 0)
        {
            rb.position = rb.position + inputvelocity * MOVEMENT_SPEED;
            //rb.velocity = (inputvelocity*MOVEMENT_SPEED);
        }
        else
        {
            knocked--;
        }

        var forcesSum = new Vector2(0, 0);
        foreach (Vector2 v in forces)
        {
            forcesSum += v;
        }

        rb.AddForce(forcesSum);
        realvelocity = rb.velocity + inputvelocity;

        forces.Clear();
    }

    void applyForce(Vector2 force)
    {
        forces.Add(force);
    }

    // makes player invisible and unresponsive so that they could potentially be
    // revived
    void Dead()
    {
        render.enabled = false;
        enabled = false;
    }



    // Update is called once per frame
    void Update () {


        /* ROTATION */
        // get position of main sprite and mouse
        Vector2 pos = rb.position;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get directional vector and convert to angle
        Vector2 direction = pos - mouse;
        float angle = Mathf.Atan2(direction.y, direction.x);

        // use angle to rotate bow
        shield.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
        shield.transform.position = pos + -1 * direction.normalized * bowdistance;

        //Health bar
        if (hbarupdatetime == 0)
        {
            healthbarback.transform.localScale = healthbar.transform.localScale;
            hbarupdatetime = 100;
        }
        else
        {
            hbarupdatetime--;
        }
    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public void TakeDamage(float dmg, Vector2 dir)
    {
        var hsize = new Vector3((health.getCurrentHP() / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hbarupdatetime = 20;
        if (!health.TakeDamage(dmg))
        {
            Dead();
        }
        else if (invincible == false)
        {
            applyForce(dir);
            knocked = 20;
            StartCoroutine(damageflash());
        }
    }

    IEnumerator damageflash()
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        invincible = true;
        yield return new WaitForSeconds(0.5f);
        invincible = false;
        GetComponent<SpriteRenderer>().color = Color.white;

    }

}

