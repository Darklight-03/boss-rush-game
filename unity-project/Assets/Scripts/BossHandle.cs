using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BossHandle : MonoBehaviour
{
    private SocketNetworkManager snm;
    private Rigidbody2D rb;
    private Transform canvas;
    private GameObject healthbar;
    private GameObject healthbarbg;
    private Text bossname;
    private Vector2 prevv1;
    private Vector2 prevv2;
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
    private GameObject imageO;
    public bool state;
    public bool isMove;
    public bool isstone;
    public GameObject player;
    public GameObject[] gameObjects;

    // Use this for initialization
    void Start()
    {
        snm = GetComponent<SocketNetworkManager>();
        canvas = GameObject.Find("Canvas").transform;
        state = true;
        isMove = true;
        isstone = false;
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        health = GetComponent<Health>();
        render = GetComponent<SpriteRenderer>();
        imageO = gameObject.transform.GetChild(2).gameObject;
        bossname = GameObject.FindWithTag("Boss-name").GetComponent<Text>();
        healthbar = GameObject.FindWithTag("Boss-health");
        healthbarbg = GameObject.FindWithTag("Boss-healthbh");
        hit = 0;
        healthbarsize = healthbar.transform.localScale;
        gameObjects = GameObject.FindGameObjectsWithTag("Player");
        player = GameObject.FindWithTag("Player");//delete after merge
        gameObject.transform.parent = canvas;
        gameObject.GetComponent<RectTransform>().localScale = new Vector3(25.0f, 25.0f, 25.0f);
        gameObject.GetComponent<RectTransform>().localPosition = Vector3.one;
    }

    private void OnEnable()
    {
        SocketNetworkManager.DealDamageHandle += DealDamageHandleH;
    }

    private void OnDisable()
    {
        SocketNetworkManager.DealDamageHandle -= DealDamageHandleH;
    }

    void DealDamageHandleH(string sender, float dmg, Vector2 dir)
    {
        StartCoroutine(DealDamageHandle(sender, dmg, dir));
    }
    IEnumerator DealDamageHandle(string sender, float dmg, Vector2 dir)
    {
        if (sender != SocketNetworkManager.id)
        {
            // dir could be used for knockback or something like that.
            // display health, if dead, etc
            TakeDamage(dmg);
        }
        yield break;
    }

    void TakeDamage(float dmg)
    {
        var hsize = new Vector3(((health.getCurrentHP() - dmg) / health.getMaxHP()) * (healthbarsize.x), healthbarsize.y, healthbarsize.z);
        healthbar.transform.localScale = hsize;
        hit = 25;
        hbarupdatetime = 20;

        if (health.TakeDamage(10))
        {
            StartCoroutine(damageAnimation());
        }
        else
        {
            Dead();
        }
        // do stuff only for the circle collider
    }

    IEnumerator damageAnimation()
    {
        for (int i = 10; i > 0; i--)
        {
            Color lerp = Color.Lerp(Color.white, Color.red, (float)i / 10);
            imageO.GetComponent<Image>().color = lerp;
            yield return null;
        }
    }

    void Dead()
    {
        healthbarbg.transform.localScale = healthbar.transform.localScale;
        health.enabled = false;
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            gameObject.transform.GetChild(i).gameObject.SetActive(false);
        }
        //render.enabled = false;
        this.gameObject.SetActive(false);
    }

    // called in fixed interval
    void FixedUpdate()
    {
        Vector2 v1 = transform.position;
        float temp = float.MaxValue - 1000;
        foreach (GameObject g in gameObjects)
        {
            Vector2 vg1 = g.transform.position;
            float max1 = (v1 - vg1).magnitude;
            if (max1 < temp)
            {
                temp = max1;
                player = g;
            }
        }

        if (isMove)
        {
            Vector2 v2 = player.transform.position;
            rb.velocity = v2 - v1;

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

            if (Vector2.Distance(prevv1, v1) > 0.1f || Vector2.Distance(prevv2, v2) > 0.1f)
            {
                snm.sendMessage("bp", "{ \"x\": " + v1.x + " , \"y\": " + v1.y + ", \"rx\": " + v2.x + ", \"ry\": " + v2.y + " }");
                prevv1 = v1;
                prevv2 = v2;
            }
        }
        if(isstone)
        {
            Vector2 v2 = player.transform.position;
           // v1 = new Vector2(v1.x + 5, v1.y+5);

            if (Mathf.Abs(v2.x - v1.x) > Mathf.Abs(v2.y - v1.y))
            {

                if (v2.x > v1.x)
                {
                    v2 = new Vector2(v1.x, v2.y);
                }
                else
                {
                    v2 = new Vector2(v1.x, v2.y);
                }
            }
            else
            {
                //v1 = new Vector2(v1.x, v1.y - 2);
                if (v2.y > v1.y)
                {
                    v2 = new Vector2(v2.x, v1.y);
                }
                else
                {
                    v2 = new Vector2(v2.x, v1.y);
                }
            }
            if (Mathf.Abs(v2.x - v1.x)<=1.5f)
            {
                v1 = new Vector2(v1.x - 4f,v1.y);
            }
            if (Mathf.Abs(v2.y - v1.y) <= 1.5f)
            {
                v1 = new Vector2(v1.x , v1.y- 4f);
            }
  
            rb.velocity = v2 - v1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        if ((v1 - v2).magnitude < 3 && state)
        {
            snm.sendMessage("ba", "{ \"name\": \"" + "huijian" + "\" }");
            animation.Play("huijian");
            
            state = false;
            Invoke("PlayGameeffects", 0.3f);
            Invoke("ChangeStae", 1f);
        }

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
    public void PlayGameeffects()
    {
        daoguang.SetActive(true);
        Invoke("CloseGameeffects", 0.2f);
    }
    public void CloseGameeffects()
    {
        daoguang.SetActive(false);
    }
    public void ChangeStae()
    {
        state = true;
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
        if(collider.tag=="stone")
        {
            isstone = true;
       
            isMove = false;

        }
        if(collider.tag=="Player")
        {
            isMove = false;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            isMove = true;
        }
        if (collision.tag == "stone")
        {
            isstone = false;
            isMove = true;
        }
    }
}
