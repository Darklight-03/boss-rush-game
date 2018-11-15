using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class cooldown{
    public string name;
    public float cd;
    public float nextavailable;
    public string key;
    public SpriteRenderer r;
    public List<SpriteRenderer> rc;
    public GameObject go;
    public GameObject[] goc;
    public bool p;

    public cooldown(string name, float cd, string key, GameObject go){
        this.name = name;
        this.cd = cd;
        this.key = key;
        nextavailable = 0;
        this.go = go;
    }

    public cooldown(float cd, GameObject[] go){
        this.name="GLOBALCD";
        this.cd = cd;
        this.key = "none";
        nextavailable = 0;
        this.goc = go;
        this.rc = new List<SpriteRenderer>();
        this.p = false;
    }

    public void addR(SpriteRenderer sr){
      rc.Add(sr); 
    }

    public bool isReady(){
        return Time.time > nextavailable;
    }
    public void setCd(){
        nextavailable = Time.time + cd;
    }
    public float getRatio(){
        return ((nextavailable-Time.time)/cd)*0.9f + 0.1f;
    }
}

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
    public float DASH_CD = 10f;
    public float GLOBAL_CD = 0.3f;
    int knocked;
    Vector2 realvelocity;
    bool clicked;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;
    private Canvas canvas;
    int numarrows;
    cooldown dashcd;
    cooldown glcd;
    cooldown globalcd;
    int hit;
    private Vector2 prevPos = new Vector2(0,0);
    private Vector2 prevRot = new Vector2(0,0);




	// Use this for initialization

	void Start ()
  {
        snm = GetComponent<SocketNetworkManager>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
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

        /* ABILITIES */
        GameObject[] icons = GameObject.FindGameObjectsWithTag("ability-icons");
        Array.Sort(icons,CompareIcons);

        List<cooldown> cds = new List<cooldown>();
        glcd = new cooldown("auto",GLOBAL_CD,"LMB",icons[0]);
        cds.Add(glcd);
        dashcd = new cooldown("dash",DASH_CD,"LShift",icons[1]);
        cds.Add(dashcd);

        /* SET CD TEXT */
        cds.ForEach(delegate(cooldown c){
            Text t = c.go.GetComponentInChildren(typeof(Text)) as Text;
            t.text = c.key;
        });

        /* GLOBAL CD */
        globalcd = new cooldown(GLOBAL_CD,icons);
        cds.Add(globalcd);
        StartCoroutine(cdUpdater(cds));

	}

  int CompareIcons(GameObject x, GameObject y){
    return x.name.CompareTo(y.name);
  }

    IEnumerator cdUpdater(List<cooldown> l){
      while(true){
        while(l.Count>0){
            int i = 1;
            l.ForEach(delegate(cooldown c){
                if(!c.isReady()){
                  if(!Equals(c.name,"GLOBALCD")){
                    // display square or update it at correct position
                    if(c.r==null){
                        c.r = Instantiate(Resources.Load<GameObject>("Square"),c.go.GetComponent<Transform>().position,Quaternion.identity,canvas.transform).GetComponent<SpriteRenderer>();
                    }
                    c.r.color = new Color(255,255,255,c.getRatio());
                  }
                  else{
                  Debug.Log("a");
                    if(!c.p){
                    Debug.Log("b");
                      for(int p = 0;p<c.goc.Length;p++){
                      Debug.Log("c");
                        c.addR(Instantiate(Resources.Load<GameObject>("Square"),c.goc[p].GetComponent<Transform>().position,Quaternion.identity,canvas.transform).GetComponent<SpriteRenderer>());
                      }
                      c.p = true;
                    }
                    c.rc.ForEach(delegate (SpriteRenderer r){
                      r.color = new Color(255,255,255,c.getRatio());
                    });
                  }
                }else{
                    // if square still being rendered, play flash animation then
                    // stop rendering.
                  if(!Equals(c.name,"GLOBALCD")){
                    if(c.r!=null){
                        StartCoroutine(cdFinished(c,c.r));
                        c.r = null;
                    }
                  }
                  else{
                    for(int p = 0;i<c.goc.Length;i++){
                      if(!c.p){
                        StartCoroutine(cdFinished(c,c.rc));
                        c.p = false;
                      }
                    }
                  }
                }
                i++;
            });
        yield return null;
        }
      }
    }
    IEnumerator cdFinished(cooldown c, List<SpriteRenderer> cr){
      for(int o = 0;o<c.goc.Length;o++){
        for(float i = 0;i<5;i++){
            Color a = new Color(255,255,255,1);
            Color b = new Color(255,255,255,0);
            float e = i/5;
            Color lc = Color.Lerp(a,b,e);
            cr[o].color = lc;
            yield return null;
        }
        Destroy(cr[o].gameObject);
      }
    }
    IEnumerator cdFinished(cooldown c, SpriteRenderer cr){
        for(float i = 0;i<5;i++){
            Color a = new Color(255,255,255,1);
            Color b = new Color(255,255,255,0);
            float e = i/5;
            Color lc = Color.Lerp(a,b,e);
            cr.color = lc;
            yield return null;
        }
        Destroy(cr.gameObject);
    }

    void OnEnable()
    {

    }

    void OnDisable()
    {

    forces.Clear(); 
  }
    void gcd(){
      glcd.setCd();
      globalcd.setCd();
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
        while(glcd.isReady()){

          if(Input.GetKey("q")){
            addArrow();
            gcd();
            break;
          }
          else if(Input.GetKey("e")){
            poisonArrow();
            gcd();
            break;
          }
          else if(Input.GetKey(KeyCode.LeftShift)){
            if(dashcd.isReady()){
              dash(direction);
              gcd();
              dashcd.setCd();
            }
            break;
          }
       
          /* ARROW */
          else if(Input.GetMouseButton(0)){
            if(!clicked){
              clicked = true;
              bowrender.sprite = f2;
              snm.sendMessage("playeranimation", "{ \"name\": \"" + "relebow" + "\" }");
            }
          }else if(clicked){ 
            bowrender.sprite = f1;
            snm.sendMessage("playeranimation", "{ \"name\": \"" + "drawbow" + "\" }");
            snm.sendMessage("spawnprojectile", "{ \"name\": \"" + "arrowOP" + "\" , \"x\": " + bow.transform.position.x + " , \"y\": " + bow.transform.position.y + ", \"rx\": " + direction.x + ", \"ry\": " + direction.y + " }");
            GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("arrow"),bow.transform.position,bow.transform.rotation,GetComponent<Transform>());
            arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized*ARROW_SPEED*-1;
            clicked = false;
            gcd();
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
            snm.sendMessage("playerposition", "{ \"x\": " + rb.position.x.ToString() + " , \"y\": " + rb.position.y.ToString() + ", \"rx\": " + direction.normalized.x.ToString() + ", \"ry\": " + direction.normalized.y.ToString() + " }");
            prevPos = rb.position;
            prevRot = direction;
        }
    
    

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
    snm.sendMessage("playeranimation", "{ \"name\": \"" + "dashanim" + "\" }");
    float m = direction.magnitude;
    var v = direction.normalized;
    if(m>MAX_DASH){ 
      m = MAX_DASH;
    }
    v*=m;
    v = rb.position - v;
    StartCoroutine(dashAnim(rb.position,v));
  }

  IEnumerator dashAnim(Vector3 opos, Vector3 mpos){
    for(int i = -10;i<=10;i++){
      Color c = Color.Lerp(Color.white, Color.green, (float)Mathf.Abs(Mathf.Abs(i)-10)/10);
      render.color = c;
      Vector3 curpos = Vector3.Lerp(opos,mpos,(float)i/10);
      rb.position = curpos;
      yield return null;
    }
  }


    // makes player invisible and unresponsive so that they could potentially be
    // revived
     void Dead()
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
        rb.velocity = rb.velocity * -0.5f;
    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public void TakeDamage(float dmg, Vector2 dir)
    {
        snm.sendMessage("takedamage", "{ \"dmg\": " + dmg + " }");
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

}

