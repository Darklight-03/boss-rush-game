using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;


public abstract class playerBaseOP : MonoBehaviour
{
    protected SocketNetworkManager snm;
    protected Rigidbody2D rb;
    protected Health health;
    protected SpriteRenderer render;
    protected GameObject healthbar;
    protected GameObject healthbarback;
    protected Text interfaceplayertext;
    protected Vector3 healthbarsize;
    protected List<Vector2> forces;
    protected int hbarupdatetime;
    protected float MOVEMENT_SPEED = 0.1f;
    protected float GLOBAL_CD = 0.3f;
    protected float SHIFT_CD = 0.0f;
    protected string SHIFT_NAME = "shift";
    protected float RMB_CD = 0.0f;
    protected string RMB_NAME = "rmb";
    protected float E_CD = 0.0f;
    protected string E_NAME = "e";
    protected float Q_CD = 0.0f;
    protected string Q_NAME = "q";
    protected string LMB_NAME = "lmb";
    protected Vector2 mousePosition;
    public float HOTBARITEM_SIZE = 41.0f;
    private int knocked;
    protected Vector2 realvelocity;
    protected bool clicked;
    protected Canvas canvas;
    protected Vector2 direction;
    protected float angle;
    protected cooldown glcd;
    protected cooldown globalcd;
    protected cooldown lshiftcd;
    protected cooldown rmbcd;
    protected cooldown ecd;
    protected cooldown qcd;
    protected int hit;
    private Vector2 prevPos = new Vector2(0, 0);
    private Vector2 prevRot = new Vector2(0, 0);

    public int playernum;
    public string id;
    public int healthbar_id;


    protected virtual void Start()
    {
        snm = GetComponent<SocketNetworkManager>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        direction = new Vector2(0, 0);
        angle = 0.0f;
        forces = new List<Vector2>();
        healthbar = GameObject.FindWithTag("P" + healthbar_id + "-health");
        interfaceplayertext = GameObject.FindWithTag("P" + healthbar_id + "-name").GetComponent<Text>();
        healthbarback = GameObject.FindWithTag("P" + healthbar_id + "-healthbg");
        interfaceplayertext.text = "Player " + healthbar_id;
        healthbarsize = healthbar.transform.localScale;
        hbarupdatetime = 0;
        knocked = 0;
        realvelocity = new Vector2(0, 0);
        clicked = false;
        hit = 0;
    }


    protected virtual void OnEnable()
    {
        SocketNetworkManager.TakeDamageHandle += TakeDamageHandleH;
    }

    protected virtual void OnDisable()
    {
        SocketNetworkManager.TakeDamageHandle -= TakeDamageHandleH;
    }

    void TakeDamageHandleH(string sender, float dmg)
    {
        StartCoroutine(TakeDamageHandle(sender, dmg));
    }
    IEnumerator TakeDamageHandle(string sender, float dmg)
    {
        if (id == sender)
        {
            // display health, if dead, etc (knockback is handled on the other players client
            TakeDamage(dmg, Vector2.zero);
        }
        yield break;
    }


    // called in fixed interval
    protected virtual void FixedUpdate()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        /* ON HIT */
        if (hit >= 0)
        {
            Color lerpedColor = Color.Lerp(Color.white, Color.red, Mathf.Sqrt(hit) / Mathf.Sqrt(25));
            render.color = lerpedColor;
            hit--;
        }

        /* HEALTH BAR */
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

    // makes player invisible and unresponsive so that they could potentially be
    // revived
    protected virtual void Dead()
    {
        healthbarback.transform.localScale = healthbar.transform.localScale;
        health.enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        //render.enabled = false;
        this.gameObject.SetActive(false);
    }


    // simply adds a force to the list to be applied next update.
    void applyForce(Vector2 force)
    {
        forces.Add(force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public virtual void TakeDamage(float dmg, Vector2 dir)
    {
        //snm.sendMessage("takedamage", "{ \"dmg\": " + dmg + " }");
        var hsize = new Vector3(((health.getCurrentHP() - dmg) / health.getMaxHP()) * (healthbarsize.x), healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hit = 25;
        hbarupdatetime = 20;
        if (!health.TakeDamage(dmg))
        {
            Dead();
        }
        else
        {
            //applyForce(dir);
            knocked = 20;
        }
    }

    public virtual void Heal(float amount)
    {
        //snm.sendMessage("takedamage", "{ \"dmg\": " + -1 * amount + " }");
        health.Heal(amount);
        var hsize = new Vector3((((health.getCurrentHP() + amount) % health.getMaxHP()) / health.getMaxHP()) * (healthbarsize.x), healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hbarupdatetime = 20;
    }

}

