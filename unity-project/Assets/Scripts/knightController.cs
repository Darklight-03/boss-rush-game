using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class knightController : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    private GameObject shield;
    private GameObject sword;
    private float bowdistance;
    private float sworddistance;
    private Health health;
    private SpriteRenderer render;
    private GameObject healthbar;
    private GameObject healthbarback;
    private Vector3 healthbarsize;
    private Text interfaceplayertext;
    int hbarupdatetime;
    int knocked;
    public float MOVEMENT_SPEED = 0.1f;
    List<Vector2> forces;
    Vector2 realvelocity;
    bool invincible;
    public int GLOBAL_CD = 20;
    bool clicked;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;
    int gcd;
    int autocd;
    int hit;
    private Vector2 prevPos = new Vector2(0, 0);
    private Vector2 prevRot = new Vector2(0, 0);

    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        shield = gameObject.transform.GetChild(0).gameObject;
        sword = gameObject.transform.GetChild(1).gameObject;
        bowdistance = (shield.transform.position - (Vector3)rb.position).magnitude;
        sworddistance = (sword.transform.position - (Vector3)rb.position).magnitude;
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
        healthbar = GameObject.FindWithTag("Health-bar");
        healthbarback = GameObject.FindWithTag("Health-bar-background");
        interfaceplayertext = GameObject.FindWithTag("Player-text").GetComponent<Text>();
        interfaceplayertext.text = "You: Knight";
        hbarupdatetime = 0;
        knocked = 0;
        forces = new List<Vector2>();
        realvelocity = new Vector2(0, 0);
        invincible = false;
        healthbarsize = healthbar.transform.localScale;
        hit = 0;
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {
        forces.Clear();
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

    // Update is called once per frame
    void Update ()
    {
        /* ON HIT */
        if (hit >= 0)
        {
            Color lerpedColor = Color.Lerp(Color.white, Color.red, Mathf.Sqrt(hit) / Mathf.Sqrt(25));
            render.color = lerpedColor;
            hit--;
        }

        /* ROTATION */
        // get position of main sprite and mouse
        Vector2 pos = rb.position;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get directional vector and convert to angle
        Vector2 direction = pos - mouse;
        float angle = Mathf.Atan2(direction.y, direction.x);

        // use angle to rotate sword, shield
        shield.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
        shield.transform.position = pos + -1 * direction.normalized * bowdistance;
        sword.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * (angle), transform.forward);
        sword.transform.position = pos + -0.5f * (new Vector2(direction.normalized.y, direction.normalized.x));
        /* ABILITIES */
        while (gcd < 0)
        {

            if (Input.GetKey("q"))
            {
                gcd = GLOBAL_CD;
                break;
            }
            if (Input.GetKey("e"))
            {
                gcd = GLOBAL_CD;
                break;
            }
            if (Input.GetKey(KeyCode.LeftShift))
            {
                if (0 < 0)
                {

                }
                gcd = GLOBAL_CD;
                break;
            }

            /* ARROW */
            if (Input.GetMouseButton(0))
            {
                if (!clicked)
                {
                    clicked = true;
                    //snm.sendMessage("pa", "{ \"name\": \"" + "" + "\" }");
                }
            }
            else if (clicked)
            {
                //snm.sendMessage("pa", "{ \"name\": \"" + "drawbow" + "\" }");
                //snm.sendMessage("sp", "{ \"name\": \"" + "arrowOP" + "\" , \"x\": " + bow.transform.position.x + " , \"y\": " + bow.transform.position.y + ", \"rx\": " + direction.x + ", \"ry\": " + direction.y + " }");
                //GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("arrow"), bow.transform.position, bow.transform.rotation, GetComponent<Transform>());
                //arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized * ARROW_SPEED * -1;
                clicked = false;
                gcd = GLOBAL_CD;
                break;
            }
            break;
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

        if (Vector2.Distance(prevPos, rb.position) > 0.1f || Vector2.Angle(prevRot, direction) > Vector2.Angle(new Vector2(1, 0.1f), Vector2.right))
        {
            snm.sendMessage("pp", "{ \"x\": " + rb.position.x.ToString() + " , \"y\": " + rb.position.y.ToString() + ", \"rx\": " + direction.normalized.x.ToString() + ", \"ry\": " + direction.normalized.y.ToString() + " }");
            prevPos = rb.position;
            prevRot = direction;
        }

        gcd--;
        autocd--;
    }

    // makes player invisible and unresponsive so that they could potentially be
    // revived
    void Dead()
    {
        bowrender.enabled = false;
        health.enabled = false;
        render.enabled = false;
        enabled = false;
    }

    // simply adds a force to the list to be applied next update.
    void applyForce(Vector2 force)
    {
        forces.Add(force);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.velocity = rb.velocity * -0.5f;
    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public void TakeDamage(float dmg, Vector2 dir)
    {
        snm.sendMessage("td", "{ \"dmg\": " + dmg + " }");
        var hsize = new Vector3((health.getCurrentHP() / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
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

}

