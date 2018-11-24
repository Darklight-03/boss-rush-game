using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class knightControllerOP : MonoBehaviour
{
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
    int gcd;
    int autocd;
    int hit;
    private Vector2 prevPos = new Vector2(0, 0);
    private Vector2 prevRot = new Vector2(0, 0);
    string weapon;
    public bool stabbing = false;
    public string id;
    public int healthbar_id;
    public int playernum;

    // Use this for initialization
    void Start()
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
        healthbar = GameObject.FindWithTag("P" + healthbar_id + "-health");
        interfaceplayertext = GameObject.FindWithTag("P" + healthbar_id + "-name").GetComponent<Text>();
        healthbarback = GameObject.FindWithTag("P" + healthbar_id + "-healthbg");
        interfaceplayertext.text = "Player " + healthbar_id;
        hbarupdatetime = 0;
        knocked = 0;
        forces = new List<Vector2>();
        realvelocity = new Vector2(0, 0);
        invincible = false;
        healthbarsize = healthbar.transform.localScale;
        hit = 0;
        weapon = "shield";
        //at start of game, knight has shield enabled by default
        shield.GetComponent<SpriteRenderer>().enabled = true;
        sword.GetComponent<SpriteRenderer>().enabled = false;
    }

    void OnEnable()
    {
        SocketNetworkManager.TakeDamageHandle += TakeDamageHandleH;
        SocketNetworkManager.UpdateOtherPlayerPos += UpdateOtherPlayerPosH;
        SocketNetworkManager.PlayerAnimHandle += PlayerAnimHandleH;
        SocketNetworkManager.SpawnProjHandle += SpawnProjHandleH;
    }

    void OnDisable()
    {
        SocketNetworkManager.TakeDamageHandle -= TakeDamageHandleH;
        SocketNetworkManager.UpdateOtherPlayerPos -= UpdateOtherPlayerPosH;
        SocketNetworkManager.PlayerAnimHandle -= PlayerAnimHandleH;
        SocketNetworkManager.SpawnProjHandle -= SpawnProjHandleH;
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

    void UpdateOtherPlayerPosH(string sender, float x, float y, float rx, float ry)
    {
        StartCoroutine(UpdateOtherPlayerPos(sender, x, y, rx, ry));
    }
    IEnumerator UpdateOtherPlayerPos(string sender, float x, float y, float rx, float ry)
    {
        if (id == sender)
        {
            Vector2 pos;
            Vector2 direction;
            float angle;
            pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;

            direction = new Vector2(rx, ry);
            angle = Mathf.Atan2(direction.y, direction.x);
            shield.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
            shield.transform.position = pos + -1 * direction.normalized * bowdistance;
            sword.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * (angle), transform.forward);
            sword.transform.position = pos + -1 * direction.normalized * sworddistance;
        }
        yield break;
    }

    void PlayerAnimHandleH(string sender, string name)
    {
        StartCoroutine(PlayerAnimHandle(sender, name));
    }
    IEnumerator PlayerAnimHandle(string sender, string name)
    {
        if (id == sender)
        {
            // handle the weird way the bow works
            if (name == "switch")
            {
                if (weapon == "shield")
                {
                    weapon = "sword";
                    shield.GetComponent<SpriteRenderer>().enabled = false;
                    sword.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    weapon = "shield";
                    sword.GetComponent<SpriteRenderer>().enabled = false;
                    shield.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            else if (name == "stab")
            {
                StartCoroutine(stabAnimation(10));
            }
            // handle actual animations
            else
            {
                // animation.Play(name)
            }
        }
        yield break;
    }

    void SpawnProjHandleH(string sender, string name, Vector2 pos, Vector2 dir)
    {
        StartCoroutine(SpawnProjHandle(sender, name, pos, dir));
    }
    IEnumerator SpawnProjHandle(string sender, string name, Vector2 pos, Vector2 dir)
    {
        if (id == sender)
        {

        }
        yield break;
    }

    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update()
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

        gcd--;
        autocd--;
    }

    IEnumerator stabAnimation(int val)
    {
        stabbing = true;
        float olddist = sworddistance;
        sworddistance = olddist + 0.5f;
        while (val >= 0)
        {
            val -= 1;
            yield return new WaitForEndOfFrame();
        }
        sworddistance = olddist;
        stabbing = false;
    }

    // makes player invisible and unresponsive so that they could potentially be
    // revived
    void Dead()
    {
        healthbarback.transform.localScale = healthbar.transform.localScale;
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
        if (weapon == "shield")
        {
            dmg = dmg / 2;
        }
        var hsize = new Vector3(((health.getCurrentHP() - dmg) / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
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

