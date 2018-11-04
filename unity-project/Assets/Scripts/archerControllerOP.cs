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
    public int playernum;




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
	}

    void OnEnable()
    {
        SocketNetworkManager.TakeDamageHandle += TakeDamageHandle;
        SocketNetworkManager.UpdateOtherPlayerPos += UpdateOtherPlayerPos;
        SocketNetworkManager.PlayerAnimHandle += PlayerAnimHandle;
        SocketNetworkManager.SpawnProjHandle += SpawnProjHandle;
    }

    void OnDisable()
    {
        SocketNetworkManager.TakeDamageHandle -= TakeDamageHandle;
        SocketNetworkManager.UpdateOtherPlayerPos -= UpdateOtherPlayerPos;
        SocketNetworkManager.PlayerAnimHandle -= PlayerAnimHandle;
        SocketNetworkManager.SpawnProjHandle -= SpawnProjHandle;
    }

  // called in fixed interval
    void FixedUpdate()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {

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

    }

    void TakeDamageHandle(string sender, float dmg)
    {
        if (id == sender)
        {
            // display health, if dead, etc (knockback is handled on the other players client
        }
    }

    void UpdateOtherPlayerPos(string sender, float x, float y, float rx, float ry)
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
    }

    void PlayerAnimHandle(string sender, string name)
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
            // handle actual animations
            else
            {
                // animation.Play(name)
            }
        }
    }

    void SpawnProjHandle(string sender, string name, Vector2 pos, Vector2 dir)
    { 
        GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("arrow"), pos, new Quaternion(dir.x, dir.y, 0, 1), GetComponent<Transform>());
        arrow.GetComponent<Rigidbody2D>().velocity = dir.normalized * ARROW_SPEED * -1;
    }
}
