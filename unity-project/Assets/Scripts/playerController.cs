using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
  private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    public float speed;
    public Animation animation;
    // Use this for initialization
    void Start () {
        animation = this.GetComponent<Animation>();
    rb = GetComponent<Rigidbody2D>();
        swordcol = GameObject.Find("short-sword").GetComponent<Collider2D>();

    }

  // called in fixed interval
  void FixedUpdate(){
    // input x and y
    float ix = Input.GetAxis("Horizontal");
    float iy = Input.GetAxis("Vertical");
    
    // get velocity input
    var inputvelocity = new Vector2(ix,iy);

        // later can add velocity vectors together for knockback and stuff
        rb.AddForce(inputvelocity * speed);
       

    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.R))
        {
            animation.Play("huijian");
        }
	}
}
