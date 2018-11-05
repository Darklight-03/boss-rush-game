using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class archerControllerOP : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    private GameObject bow;
    private Health health;
    private SpriteRenderer render;
    private GameObject healthbar;
    private GameObject healthbarback;
    private Text interfaceplayertext;
    float bowdistance;
    Vector3 healthbarsize;
    List<Vector2> forces;
    int hbarupdatetime;
    public float MOVEMENT_SPEED;
    public float ARROW_SPEED;
    //public bool isPlayer = true;
    int knocked;
    Vector2 realvelocity;
    bool clicked;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;
    private Vector2 prevPos;
    private Vector2 prevRot;
    public string id;
    public int healthbar_id;
    public int playernum;
    int hit;




    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        rb = GetComponent<Rigidbody2D>();
        bow = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
        render = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        f2 = Resources.Load<Sprite>("bow2");
        f1 = Resources.Load<Sprite>("bow");
        bowrender = bow.GetComponent<SpriteRenderer>();
        healthbar = GameObject.FindWithTag("P" + healthbar_id + "-health");
        interfaceplayertext = GameObject.FindWithTag("P" + healthbar_id + "-name").GetComponent<Text>();
        healthbarback = GameObject.FindWithTag("P" + healthbar_id + "-healthbg");
        interfaceplayertext.text = "Player " + healthbar_id;
        healthbarsize = healthbar.transform.localScale;
        hbarupdatetime = 0;
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

  // called in fixed interval
    void FixedUpdate()
    {

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

    void OnMouseDown()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public void TakeDamage(float dmg, Vector2 dir)
    {
        Debug.Log("took " + dmg + " damage");
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
            Vector2 pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;

            Vector2 dir = new Vector2(rx, ry);

            // use angle to rotate bow
            bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x), Vector3.forward);
            bow.transform.position = pos + -1 * dir.normalized * bowdistance;
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
            if (name == "drawbow")
            {
                bowrender.sprite = f1;
            }
            else if (name == "relebow")
            {
                bowrender.sprite = f2;
            }
            else if (name == "dashanim")
            {
                StartCoroutine(dashAnim(rb.position));
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
            // for now just do arrows, name could specify the projectile
            GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>(name), pos, bow.transform.rotation, GetComponent<Transform>());
            arrow.GetComponent<Rigidbody2D>().velocity = dir.normalized * ARROW_SPEED * -1;
        }
        yield break;
    }

    IEnumerator dashAnim(Vector3 opos)
    {
        for (int i = -10; i <= 10; i++)
        {
            Color c = Color.Lerp(Color.white, Color.green, (float)Mathf.Abs(Mathf.Abs(i) - 10) / 10);
            render.color = c;
            Debug.Log(Mathf.Abs(Mathf.Abs(i) - 15));
            yield return null;
        }
    }
}
