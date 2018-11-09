using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
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
    private GameObject healthbar;
    private GameObject healthbarbg;
    private Text bossname;
    int hit;
    int hbarupdatetime;
    Vector3 healthbarsize;
    public RectTransform image;


    // Use this for initialization
    void Start ()
    {
        snm = GetComponent<SocketNetworkManager>();
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
        bossname = GameObject.FindWithTag("Boss-name").GetComponent<Text>();
        healthbar = GameObject.FindWithTag("Boss-health");
        healthbarbg = GameObject.FindWithTag("Boss-healthbh");
        hit = 0;
        healthbarsize = healthbar.transform.localScale;
        image = this.GetComponentInChildren<RectTransform>();
    }

    private void OnEnable()
    {
        SocketNetworkManager.UpdateBossPositionHandle += UpdateBossPositionHandle;
        SocketNetworkManager.BossAnimHandle += BossAnimHandle;
        SocketNetworkManager.DealDamageHandle += DealDamageHandleH;
    }

    private void OnDisable()
    {
        SocketNetworkManager.UpdateBossPositionHandle -= UpdateBossPositionHandle;
        SocketNetworkManager.BossAnimHandle -= BossAnimHandle;
        SocketNetworkManager.DealDamageHandle -= DealDamageHandleH;
    }

    void Update()
    {
        /* HEALTH BAR */
        if (hbarupdatetime == 0)
        {
            healthbarbg.transform.localScale = healthbar.transform.localScale;
            hbarupdatetime = 100;
        }
        else
        {
            hbarupdatetime--;
        }
    }

    void TakeDamage(float dmg)
    {

        var hsize = new Vector3((health.getCurrentHP() / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hit = 25;
        hbarupdatetime = 20;

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

    IEnumerator UpdateBossPositionHandle(float x, float y, float ry, float rz, float ty, float tz)
    {
        Vector2 pos = transform.position;
        pos.x = x;
        pos.y = y;
        transform.position = pos;

        image.localEulerAngles = new Vector3(0, ry, rz);
        this.transform.localEulerAngles = new Vector3(0, ty, tz);
        yield break;
    }

    IEnumerator BossAnimHandle(string name)
    {
        animation.Play(name);
        yield break;
    }

    void DealDamageHandleH(string sender, float dmg, Vector2 dir)
    {
        StartCoroutine(DealDamageHandle(sender, dmg, dir));
    }
    IEnumerator DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        // dir could be used for knockback or something like that.
        // display health, if dead, etc
        TakeDamage(dmg);
        yield break;
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
            TakeDamage(10);
            // do stuff only for the circle collider
        }
    }
}
