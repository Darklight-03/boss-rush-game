using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerControllerOP : MonoBehaviour {
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    SpriteRenderer render;
    Health health;
    public float speed;
    public Animation animation;


    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        SocketNetworkManager.UpdateBossPositionHandle += UpdateBossPositionHandle;
        SocketNetworkManager.BossAnimHandle += BossAnimHandle;
        SocketNetworkManager.DealDamageHandle += DealDamageHandle;
    }

    private void OnDisable()
    {
        SocketNetworkManager.UpdateBossPositionHandle -= UpdateBossPositionHandle;
        SocketNetworkManager.BossAnimHandle -= BossAnimHandle;
        SocketNetworkManager.DealDamageHandle -= DealDamageHandle;
    }



    IEnumerator UpdateBossPositionHandle(float x, float y, float rx, float ry)
    {
        Vector2 pos = transform.position;
        pos.x = x;
        pos.y = y;
        transform.position = pos;

        Vector2 dir = new Vector2(rx, ry);

        // use angle to do something probably with the sword
        yield break;
    }

    IEnumerator BossAnimHandle(string name)
    {
        animation.Play(name);
        yield break;
    }

    IEnumerator DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        // dir could be used for knockback or something like that.
        // display health, if dead, etc
        Debug.Log("recieved damage message");
        if (health.TakeDamage(dmg))
        {
            StartCoroutine(damageAnimation());
        }
        else
        {
            Destroy(this.gameObject); 
        }
        yield break;
    }

    // called in fixed interval
    void FixedUpdate()
    {

    }

    // Update is called once per frame
    void Update ()
    {

	}

    IEnumerator damageAnimation()
    {
        for (int i = 10; i > 0; i--)
        {
            Color lerp = Color.Lerp(Color.white, Color.red, (float)i / 10);
            render.color = lerp;
            yield return null;
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.name == "arrowOP(Clone)")
        {
            Destroy(collider.gameObject);
            return;
        }
        if (collider.gameObject.tag == "projectile")
        {
            Destroy(collider.gameObject);
            snm.sendMessage("dd", "{ \"dmg\": " + "10" + " , \"dirx\": " + 0 + ", \"diry\": " + 0 + " }");
            if (health.TakeDamage(10))
            {
                StartCoroutine(damageAnimation());
            }
            else
            {
                Destroy(this.gameObject);
            }
            // do stuff only for the circle collider
        }
    }
}
