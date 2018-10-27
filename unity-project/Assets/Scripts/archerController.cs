using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class archerController : MonoBehaviour {
  private Rigidbody2D rb;
  private GameObject bow;
  float bowdistance;

	// Use this for initialization
	void Start () {
    rb = GetComponent<Rigidbody2D>();
    bow = gameObject.transform.GetChild(0).gameObject;
    bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
	}

  // called in fixed interval
  void FixedUpdate(){
    // input x and y
    float ix = Input.GetAxis("Horizontal");
    float iy = Input.GetAxis("Vertical");
    
    // get velocity input
    var inputvelocity = new Vector2(ix,iy);

    // later can add velocity vectors together for knockback and stuff
    rb.velocity = inputvelocity*0.5f; 

    // rotate towards mouse
    
    Vector2 pos = rb.position;
    Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

    Vector2 direction = pos - mouse;
    float angle = Mathf.Atan2(direction.y,direction.x);
//    if(direction.y<0){
//      angle*=-1;
//    }

    bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg*angle,Vector3.forward);
    bow.transform.position = pos + -1*direction.normalized*bowdistance;
    //bow.transform.RotateAround(pos,Vector3.forward,Mathf.Rad2Deg*angle+90);
    //bow.transform.eulerAngles = new Vector3(0,0,Mathf.Rad2Deg*angle+90);
    //rb.MoveRotation(Mathf.Rad2Deg*angle+90);
  }
	
	// Update is called once per frame
	void Update () {

	}
}
