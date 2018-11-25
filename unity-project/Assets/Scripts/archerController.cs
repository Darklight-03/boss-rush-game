using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;
using UnityEngine;

public class archerController : playerBase {
    private GameObject bow;
    float bowdistance;
    public float ARROW_SPEED;
    public float MAX_DASH = 2f;
    public float DASH_CD = 10f;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;

	// Use this for initialization

    protected override void Start ()
    {
        base.Start();
        bow = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
        interfaceplayertext.text = "You: Archer";
        f2 = Resources.Load<Sprite>("bow2");
        f1 = Resources.Load<Sprite>("bow");
        bowrender = bow.GetComponent<SpriteRenderer>();
    }

    protected override void shiftAbilityInit(){
        SHIFT_CD = DASH_CD;
        SHIFT_NAME = "dash";
    }

    protected override void lmbAbilityInit(){
        LMB_NAME = "arrow";
    }

    protected override void rmbAbilityInit(){
        RMB_CD = GLOBAL_CD;
        RMB_NAME = "notyetimplemented";
    }

    protected override void eAbilityInit(){
        E_CD = GLOBAL_CD;
        E_NAME = "notyetimplemented";
    }

    protected override void qAbilityInit(){
        Q_CD = GLOBAL_CD;
        Q_NAME = "notyetimplemented";
    }

    // called in fixed interval
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
	
    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();

        // use angle to rotate bow
        bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, Vector3.forward);
        bow.transform.position = rb.position + -1 * direction.normalized * bowdistance;
    }

    protected override void LMBClicked(){
        bowrender.sprite = f2;
        snm.sendMessage("pa", "{ \"name\": \"" + "relebow" + "\" }");
    }

    protected override void LMBReleased(){
        bowrender.sprite = f1;
        snm.sendMessage("pa", "{ \"name\": \"" + "drawbow" + "\" }");
        snm.sendMessage("sp", "{ \"name\": \"" + "arrowOP" + "\" , \"x\": " + bow.transform.position.x + " , \"y\": " + bow.transform.position.y + ", \"rx\": " + direction.x + ", \"ry\": " + direction.y + " }");
        GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("arrow"),bow.transform.position,bow.transform.rotation,GetComponent<Transform>());
        arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized*ARROW_SPEED*-1;
    }

    protected override void LShiftAbility(Vector2 input){
        snm.sendMessage("pa", "{ \"name\": \"" + "dashanim" + "\" }");
        float m = input.magnitude;
        var v = input.normalized;
        if(m>MAX_DASH){ 
            m = MAX_DASH;
        }
        v*=m;
        v = rb.position - v;
        StartCoroutine(dashAnim(rb.position,v));
    }

    protected override void QAbility(){
        Debug.Log("q ability not exist yet");
    }

    protected override void EAbility(){
        Debug.Log("e ability not exist yet");
    }
    
    protected override void RMBAbility(){
        Debug.Log("RMB ability not exist yet");
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
    protected override void Dead()
    {
        base.Dead();
        bowrender.enabled = false;
    }


}

