using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class archerController : MonoBehaviour {
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
    public bool isPlayer = true;
    int knocked;
    Vector2 realvelocity;
    bool clicked;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;
    private Vector2 prevPos;
    private Vector2 prevRot;




	// Use this for initialization
	void Start () {
        snm = GetComponent<SocketNetworkManager>();
        if (!isPlayer)
        {
            SocketNetworkManager.UpdateOtherPlayerPos += UpdateOtherPlayerPos;
        }
        rb = GetComponent<Rigidbody2D>();
        bow = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
        render = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        if (isPlayer)
        {
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            forces = new List<Vector2>();
            healthbar = GameObject.FindWithTag("Health-bar");
            healthbarback = GameObject.FindWithTag("Health-bar-background");
            interfaceplayertext = GameObject.FindWithTag("Player-text").GetComponent<Text>();
            interfaceplayertext.text = "You: Archer";
            healthbarsize = healthbar.transform.localScale;
            hbarupdatetime = 0;
            knocked = 0;
            realvelocity = new Vector2(0, 0);
            clicked = false;
        }
        f2 = Resources.Load<Sprite>("bow2");
        f1 = Resources.Load<Sprite>("bow");
        bowrender = bow.GetComponent<SpriteRenderer>();
	}

    void OnEnable()
    {
        if (!isPlayer)
        {
            Debug.Log("event handler set");
            SocketNetworkManager.UpdateOtherPlayerPos += UpdateOtherPlayerPos;
        }
    }

    void OnDisable()
    {
        if (!isPlayer)
        {
            SocketNetworkManager.UpdateOtherPlayerPos -= UpdateOtherPlayerPos;
        }
    }

  // called in fixed interval
  void FixedUpdate(){
        if (isPlayer)
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
  }
	
	// Update is called once per frame
	void Update () {
        if (isPlayer)
        {
            bool oDown = Input.GetKeyDown(KeyCode.O);
            bool pDown = Input.GetKeyDown(KeyCode.P);

            if (oDown)
            {
                Debug.Log("oPressed");
                snm.createLobby();
            }
            if (pDown)
            {
                Debug.Log("pPressed");
                snm.joinLobby(0);
            }


            /* ROTATION */
            // get position of main sprite and mouse
            Vector2 pos = rb.position;
            Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // get directional vector and convert to angle
            Vector2 direction = pos - mouse;
            float angle = Mathf.Atan2(direction.y, direction.x);

            // use angle to rotate bow
            bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.forward);
            bow.transform.position = pos + -1 * direction.normalized * bowdistance;

            /* ARROW */
            if (Input.GetMouseButton(0))
            {
                if (!clicked)
                {
                    clicked = true;
                    bowrender.sprite = f2;
                }
            }
            else if (clicked)
            {
                bowrender.sprite = f1;
                GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("arrow"), bow.transform.position, bow.transform.rotation, GetComponent<Transform>());
                arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized * ARROW_SPEED * -1;
                clicked = false;
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

            if (prevPos != rb.position || Vector2.Angle(prevRot, direction) > Vector2.Angle(new Vector2(1, 0.5f), Vector2.right))
            {
                snm.sendMessage("pp", "{ \"x\": " + rb.position.x.ToString() + " , \"y\": " + rb.position.y.ToString() + ", \"rx\": " + direction.normalized.x.ToString() + ", \"ry\": " + direction.normalized.y.ToString() + " }");
                prevPos = rb.position;
                prevRot = direction;
            }
        }
	}

  // makes player invisible and unresponsive so that they could potentially be
  // revived
  void Dead(){
    bowrender.enabled = false;
    health.enabled = false;
    render.enabled = false;
    enabled = false;
  }


    // simply adds a force to the list to be applied next update.
  void applyForce(Vector2 force){
    forces.Add(force);
  }

  void OnMouseDown(){
        if (isPlayer)
        {
            
        }
  }

  void OnCollisionEnter2D(Collision2D collision){
        if (isPlayer)
        {
            rb.velocity = rb.velocity * -0.5f;
        }
  }

  // reduces player health, if its 0 then call Dead(), if not then apply
  // a knockback force given by dir
  public void TakeDamage(float dmg, Vector2 dir){
        if (isPlayer)
        {
            var hsize = new Vector3((health.getCurrentHP() / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
            healthbar.transform.localScale = hsize;
            hbarupdatetime = 20;
            if (!health.TakeDamage(dmg))
            {
                Dead();
            }
            else
            {
                applyForce(dir);
                knocked = 20;
            }
        }
  }

    void UpdateOtherPlayerPos(string id, float x, float y, float rx, float ry)
    {
        if (SocketNetworkManager.id != id)
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
}
