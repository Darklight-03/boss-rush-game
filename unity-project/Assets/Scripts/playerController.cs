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
 

    }

  // called in fixed interval
  void FixedUpdate(){


    }
	
	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.R))
        {
            animation.Play("huijian");
        }
	}

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
}
