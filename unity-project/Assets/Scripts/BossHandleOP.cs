using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BossHandleOP : MonoBehaviour
{
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    private Transform canvas;
    private GameObject healthbar;
    private GameObject healthbarbg;
    private Text bossname;
    SpriteRenderer render;
    public Health health;
    int hit;
    int hbarupdatetime;
    Vector3 healthbarsize;
    Collider2D swordcol;
    Animator myanmitor;
    public float speed;
    public Animation animation;
    public GameObject daoguang;
    public RectTransform image;
    public bool state;
    public GameObject player;
    public GameObject[] gameObjects;

    // Use this for initialization
    void Start()
    {
        snm = GetComponent<SocketNetworkManager>();
        canvas = GameObject.Find("Canvas").transform;
        state = true;
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
        bossname = GameObject.FindWithTag("Boss-name").GetComponent<Text>();
        healthbar = GameObject.FindWithTag("Boss-health");
        healthbarbg = GameObject.FindWithTag("Boss-healthbh");
        hit = 0;
        healthbarsize = healthbar.transform.localScale;
        gameObject.transform.parent = canvas;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(25.0f, 25.0f, 25.0f);
        gameObject.GetComponent<RectTransform>().localPosition = Vector3.one;
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

    IEnumerator UpdateBossPositionHandle(float x, float y, float rx, float ry)
    {
        Vector2 v1 = new Vector2(x, y);
        Vector2 v2 = new Vector2(rx, ry);
        transform.position = v1;

        if (Mathf.Abs(v2.x - v1.x) > Mathf.Abs(v2.y - v1.y))
        {
            if (v2.x > v1.x)
            {
                this.transform.localEulerAngles = new Vector3(0, 0, 0);
                this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D = new Vector3(2.817f, -0.024f, 90.036f);
                image.localEulerAngles = new Vector3(0, 0, 0);
            }
            else
            {
                this.transform.localEulerAngles = new Vector3(0, 180f, 0);
                this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D = new Vector3(2.817f, -0.024f, 0f);
                image.localEulerAngles = new Vector3(0, 180, 0);
            }
        }
        else
        {
            if (v2.y > v1.y)
            {
                this.transform.localEulerAngles = new Vector3(0, 0, 90f);
                this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D = new Vector3(2.817f, -0.024f, 90.036f);
                image.localEulerAngles = new Vector3(0, 0, -90);
            }
            else
            {
                this.transform.localEulerAngles = new Vector3(0, 0, -90f);
                this.transform.GetChild(0).GetComponent<RectTransform>().anchoredPosition3D = new Vector3(2.817f, -0.024f, 0f);
                image.localEulerAngles = new Vector3(0, 0, 90);
            }
        }
        yield break;
    }

    IEnumerator BossAnimHandle(string name)
    {
        if (name == "huijian")
        {
            animation.Play("huijian");

            state = false;
            Invoke("PlayGameeffects", 0.3f);
            Invoke("ChangeState", 1f);
        }
        else
        {
            animation.Play(name);
        }
        yield break;
    }

    public void PlayGameeffects()
    {
        daoguang.SetActive(true);
        Invoke("CloseGameeffects", 0.2f);
    }
    public void CloseGameeffects()
    {
        daoguang.SetActive(false);
    }
    public void ChangeState()
    {
        state = true;
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

    void TakeDamage(float dmg)
    {

        var hsize = new Vector3((health.getCurrentHP() / health.getMaxHP()) * healthbarsize.x, healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hit = 25;
        hbarupdatetime = 20;

        if (health.TakeDamage(10))
        {
            damageAnimation();
        }
        else
        {
            Destroy(this.gameObject);
        }
        // do stuff only for the circle collider
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

    // called in fixed interval
    void FixedUpdate()
    {
    }

    // Update is called once per frame
    void Update()
    {
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
            TakeDamage(10);
            snm.sendMessage("dd", "{ \"dmg\": " + "10" + " , \"dirx\": " + 0 + ", \"diry\": " + 0 + " }");

            // do stuff only for the circle collider
        }
    }
}
