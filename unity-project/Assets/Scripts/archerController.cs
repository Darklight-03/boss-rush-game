using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class archerController : MonoBehaviour {
  private Rigidbody2D rb;
  private GameObject bow;
  private Health health;
  private SpriteRenderer render;
  float bowdistance;
  List<Vector2> forces;

	// Use this for initialization
	void Start () {
    rb = GetComponent<Rigidbody2D>();
    bow = gameObject.transform.GetChild(0).gameObject;
    bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
    render = GetComponent<SpriteRenderer>();
    health = GetComponent<Health>();
    forces = new List<Vector2>();
	}

  // called in fixed interval
  void FixedUpdate(){
    /* MOVEMENT */
    // input x and y
    float ix = Input.GetAxis("Horizontal");
    float iy = Input.GetAxis("Vertical");
    
    // get velocity input
    var inputvelocity = new Vector2(ix,iy);

    // later can add velocity vectors together for knockback and stuff
    rb.position = rb.position + inputvelocity*0.5f; 

    var forcesSum = new Vector2(0,0);
    foreach(Vector2 v in forces){
      forcesSum += v;
    }

    rb.AddForce(forcesSum);

    forces.Clear();

    /* ROTATION */ 
    // get position of main sprite and mouse
    Vector2 pos = rb.position;
    Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    // get directional vector and convert to angle
    Vector2 direction = pos - mouse;
    float angle = Mathf.Atan2(direction.y,direction.x);
  
    // use angle to rotate bow
    bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg*angle,Vector3.forward);
    bow.transform.position = pos + -1*direction.normalized*bowdistance;
  }
	
	// Update is called once per frame
	void Update () {

	}

  // makes player invisible and unresponsive so that they could potentially be
  // revived
  void Dead(){
    render.enabled = false;
    enabled = false;
  }

  // simply adds a force to the list to be applied next update.
  void applyForce(Vector2 force){
    forces.Add(force);
  }

  // reduces player health, if its 0 then call Dead(), if not then apply
  // a knockback force given by dir
  public void TakeDamage(float dmg, Vector2 dir){
    if(!health.TakeDamage(dmg)){
      Dead();
    }
    else{
      applyForce(dir); 
    }
  }
}
