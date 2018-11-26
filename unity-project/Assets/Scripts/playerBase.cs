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
        return ((nextavailable-Time.time)/cd)*1f;
    }
}

public abstract class playerBase : MonoBehaviour {
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
    protected float MOVEMENT_SPEED=0.1f;
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
    private Vector2 prevPos = new Vector2(0,0);
    private Vector2 prevRot = new Vector2(0,0);

    public int playernum;
    public string id;
    public int healthbar_id;


	// Use this for initialization
  protected abstract void lmbAbilityInit();
  protected abstract void shiftAbilityInit();
  protected abstract void rmbAbilityInit();
  protected abstract void eAbilityInit();
  protected abstract void qAbilityInit();


  protected virtual void Start ()
  {
        snm = GetComponent<SocketNetworkManager>();
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        rb = GetComponent<Rigidbody2D>();
        render = GetComponent<SpriteRenderer>();
        health = GetComponent<Health>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        direction = new Vector2(0,0);
        angle = 0.0f;
        forces = new List<Vector2>();
        healthbar = GameObject.FindWithTag("Health-bar");
        healthbarback = GameObject.FindWithTag("Health-bar-background");
        interfaceplayertext = GameObject.FindWithTag("Player-text").GetComponent<Text>();
        interfaceplayertext.text = "You: base";
        healthbarsize = healthbar.transform.localScale;
        hbarupdatetime = 0;
        knocked = 0;
        realvelocity = new Vector2(0, 0);
        clicked = false;
        hit = 0;

        /* ABILITIES */
        GameObject[] icons = GameObject.FindGameObjectsWithTag("ability-icons");
        Array.Sort(icons,CompareIcons);

        lmbAbilityInit();
        shiftAbilityInit();
        rmbAbilityInit();
        eAbilityInit();
        qAbilityInit();

        List<cooldown> cds = new List<cooldown>();
        glcd = new cooldown(LMB_NAME,GLOBAL_CD,"LMB",icons[0]);
        rmbcd = new cooldown(RMB_NAME,RMB_CD,"RMB",icons[1]);
        lshiftcd = new cooldown(SHIFT_NAME, SHIFT_CD, "LShift", icons[2]);
        ecd = new cooldown(E_NAME,E_CD,"e",icons[3]);
        qcd = new cooldown(Q_NAME,Q_CD,"q",icons[4]);
        cds.Add(glcd);
        cds.Add(lshiftcd);
        cds.Add(rmbcd);
        cds.Add(ecd);
        cds.Add(qcd);

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

  protected int CompareIcons(GameObject x, GameObject y){
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
                    c.r.transform.localScale = new Vector3(c.getRatio()*HOTBARITEM_SIZE,HOTBARITEM_SIZE,HOTBARITEM_SIZE);
                    c.r.color = new Color(255,255,255,c.getRatio());
                  }
                  else{
                  //Debug.Log("a");
                    if(!c.p){
                   // Debug.Log("b");
                      for(int p = 0;p<c.goc.Length;p++){
                      //Debug.Log("c");
                        c.addR(Instantiate(Resources.Load<GameObject>("Square"),c.goc[p].GetComponent<Transform>().position,Quaternion.identity,canvas.transform).GetComponent<SpriteRenderer>());
                      }
                      c.p = true;
                    }
                    c.rc.ForEach(delegate (SpriteRenderer r){
                        r.transform.localScale = new Vector3(c.getRatio()*HOTBARITEM_SIZE,HOTBARITEM_SIZE,HOTBARITEM_SIZE);
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
//    for(int o = 0;o<c.goc.Length;o++){
//      for(float i = 0;i<5;i++){
//          Color a = new Color(255,255,255,1);
//          Color b = new Color(255,255,255,0);
//          float e = i/5;
//          Color lc = Color.Lerp(a,b,e);
//          cr[o].color = lc;
            yield return null;
//      }
//      Destroy(cr[o].gameObject);
//    }
    }
    IEnumerator cdFinished(cooldown c, SpriteRenderer cr){
      cr.transform.localScale = new Vector3(HOTBARITEM_SIZE, HOTBARITEM_SIZE, HOTBARITEM_SIZE);
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

    protected void gcd(){
        globalcd.setCd();
    }
	
    // called in fixed interval
    protected virtual void FixedUpdate()
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
	protected virtual void Update ()
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
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get directional vector and convert to angle
        direction = pos - mousePosition;
        angle = Mathf.Atan2(direction.y, direction.x);



        /* ABILITIES */
        while(globalcd.isReady()){

            if(Input.GetKey("q")){
                if(qcd.isReady()){
                    QAbility();
                    gcd();
                    qcd.setCd();
                }
                break;
            }
            else if(Input.GetKey("e")){
                if(ecd.isReady()){
                    EAbility();
                    gcd();
                    ecd.setCd();
                }
                break;
            }
            else if(Input.GetKey(KeyCode.LeftShift)){
                if(lshiftcd.isReady()){
                    LShiftAbility(direction);
                    gcd();
                    lshiftcd.setCd();
                }
                break;
            }

            else if(Input.GetMouseButton(1)){
                if(rmbcd.isReady()){
                    RMBAbility();
                    gcd();
                    rmbcd.setCd();
                }
                break;
            }
           
              /* ARROW */
            else if(Input.GetMouseButton(0)){
                if(glcd.isReady()){
                    if(!clicked){
                        clicked = true;
                        LMBClicked();
                    }
                }
            }
            
            else if(clicked){ 
                clicked = false;
                LMBReleased();
                gcd();
                glcd.setCd();
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
    
    

    }

    protected abstract void LMBClicked();
    protected abstract void LMBReleased();
    protected abstract void LShiftAbility(Vector2 input);
    protected abstract void QAbility();
    protected abstract void EAbility();
    protected abstract void RMBAbility();

    // makes player invisible and unresponsive so that they could potentially be
    // revived
    protected virtual void Dead()
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
    public virtual void TakeDamage(float dmg, Vector2 dir)
    {
        snm.sendMessage("td", "{ \"dmg\": " + dmg + " }");
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

    public void Heal(float amount)
    {
        snm.sendMessage("td", "{ \"dmg\": " + -1 * amount + " }");
        health.Heal(amount);
        var hsize = new Vector3((((health.getCurrentHP() + amount) % health.getMaxHP()) / health.getMaxHP()) * (healthbarsize.x), healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hbarupdatetime = 20;
    }
}

