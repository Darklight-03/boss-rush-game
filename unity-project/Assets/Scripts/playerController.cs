using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
  private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    SpriteRenderer render;
    Health health;
    public float speed;
    public Animation animation;
    // Use this for initialization
    void Start () {
      animation = this.GetComponent<Animation>();
      rb = GetComponent<Rigidbody2D>();
      health = GetComponent<Health>();
      render = GetComponent<SpriteRenderer>();
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

    public void TakeDamage(float i){
      if(health.TakeDamage(i)){
        StartCoroutine(damageAnimation());
      }
      else{
        Destroy(this);
      }
    }

    IEnumerator damageAnimation(){
      for(int i = 10;i>0;i--){
        Color lerp = Color.Lerp(Color.white,Color.red,(float)i/10);
        render.color = lerp;
        yield return null;
      }
    }

}
