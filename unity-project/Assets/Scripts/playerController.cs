using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
  private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    Health health;
    public float speed;
    public Animation animation;
    // Use this for initialization
    void Start () {
      animation = this.GetComponent<Animation>();
      rb = GetComponent<Rigidbody2D>();
      health = GetComponent<Health>();
    }

  // called in fixed interval
  void FixedUpdate(){

        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;

        rb.velocity = v2 - v1;

    }

    // Update is called once per frame
    void Update () {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        if ((v1-v2).magnitude < 3)
        {
            animation.Play("huijian");
        }
	}
    void OnCollisionEnter2D(Collision2D collision){
      if(collision.gameObject.tag == "projectile"){
        Debug.Log(health.getCurrentHP());
        if(health.TakeDamage(10)){
        }else{
          Destroy(this);
        }
           // do stuff only for the circle collider
      }
    }

}
