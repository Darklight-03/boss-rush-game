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
    public float MOVEMENT_SPEED=0.1f;
    public float ARROW_SPEED;
    public float MAX_DASH = 2f;
    public int DASH_CD = 500;
    public int GLOBAL_CD = 20;
    int knocked;
    Vector2 realvelocity;
    bool clicked;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;
    int numarrows;
    int dashcd;
    int gcd;
    int autocd;
    int hit;
    private Vector2 prevPos = new Vector2(0,0);
    private Vector2 prevRot = new Vector2(0,0);



	// Use this for initialization
	void Start ()
  {
        snm = GetComponent<SocketNetworkManager>();
        rb = GetComponent<Rigidbody2D>();
        bow = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
        render = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
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
        f2 = Resources.Load<Sprite>("bow2");
        f1 = Resources.Load<Sprite>("bow");
        bowrender = bow.GetComponent<SpriteRenderer>();
        hit = 0;
        numarrows = 0;
	}

    void OnEnable()
    {

    }

    void OnDisable()
    {

    forces.Clear(); 
  }
	
    // called in fixed interval
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
        if (hit >= 0){
          Color lerpedColor = Color.Lerp(Color.white, Color.red, Mathf.Sqrt(hit)/Mathf.Sqrt(25));
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

        // use angle to rotate bow
        bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.forward);
        bow.transform.position = pos + -1 * direction.normalized * bowdistance;

        /* ABILITIES */
        while(gcd<0){

          if(Input.GetKey("q")){
            addArrow();
            gcd = GLOBAL_CD;
            break;
          }
          if(Input.GetKey("e")){
            poisonArrow();
            gcd = GLOBAL_CD;
            break;
          }
          if(Input.GetKey(KeyCode.LeftShift)){
            if(dashcd<0){
              dash(direction);
            }
            gcd = GLOBAL_CD;
            break;
          }
       
          /* ARROW */
          if(Input.GetMouseButton(0)){
            if(!clicked){
              clicked = true;
              bowrender.sprite = f2;
              snm.sendMessage("pa", "{ \"name\": \"" + "relebow" + "\" }");
            }
          }else if(clicked){ 
            bowrender.sprite = f1;
            snm.sendMessage("pa", "{ \"name\": \"" + "drawbow" + "\" }");
            snm.sendMessage("sp", "{ \"name\": \"" + "arrowOP" + "\" , \"x\": " + bow.transform.position.x + " , \"y\": " + bow.transform.position.y + ", \"rx\": " + direction.x + ", \"ry\": " + direction.y + " }");
            GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("arrow"),bow.transform.position,bow.transform.rotation,GetComponent<Transform>());
            arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized*ARROW_SPEED*-1;
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
    
    

        dashcd--;
        gcd--;
        autocd--;
	}

  void addArrow(){
    // adds an arrow to player inventory if they don't have one.
    if(numarrows==0)
      numarrows++;
  }
  void poisonArrow(){
    // 
  }
  void dash(Vector2 direction){
    float m = direction.magnitude;
    var v = direction.normalized;
    if(m>MAX_DASH){ 
      m = MAX_DASH;
    }
    v*=m;
    v = rb.position - v;
    dashcd = DASH_CD;
    StartCoroutine(dashAnim(rb.position,v));
  }

  IEnumerator dashAnim(Vector3 opos, Vector3 mpos){
    for(int i = -10;i<=10;i++){
      Color c = Color.Lerp(Color.white, Color.green, (float)Mathf.Abs(Mathf.Abs(i)-10)/10);
      render.color = c;
      Vector3 curpos = Vector3.Lerp(opos,mpos,(float)i/10);
      Debug.Log(Mathf.Abs(Mathf.Abs(i)-15));
      rb.position = curpos;
      yield return null;
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
            applyForce(dir);
            knocked = 20;
        }
    }
}
