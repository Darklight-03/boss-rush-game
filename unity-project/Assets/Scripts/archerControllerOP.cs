using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class archerControllerOP : playerBaseOP {
    private GameObject bow;
    public float ARROW_SPEED;
    private float bowdistance;
    private Sprite f1;
    private Sprite f2;
    private SpriteRenderer bowrender;


    // Use this for initialization
    protected override void Start ()
    {
        base.Start();
        bow = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (bow.transform.position - (Vector3)rb.position).magnitude;
        bowrender = bow.GetComponent<SpriteRenderer>();
        f2 = Resources.Load<Sprite>("bow2");
        f1 = Resources.Load<Sprite>("bow");
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SocketNetworkManager.UpdateOtherPlayerPos += UpdateOtherPlayerPosH;
        SocketNetworkManager.PlayerAnimHandle += PlayerAnimHandleH;
        SocketNetworkManager.SpawnProjHandle += SpawnProjHandleH;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SocketNetworkManager.UpdateOtherPlayerPos -= UpdateOtherPlayerPosH;
        SocketNetworkManager.PlayerAnimHandle -= PlayerAnimHandleH;
        SocketNetworkManager.SpawnProjHandle -= SpawnProjHandleH;
    }

    void TakeDamageHandleH(string sender, float dmg)
    {
        StartCoroutine(TakeDamageHandle(sender, dmg));
    }
    IEnumerator TakeDamageHandle(string sender, float dmg)
    {
        if (id == sender)
        {
            // display health, if dead, etc (knockback is handled on the other players client
            TakeDamage(dmg, Vector2.zero);
        }
        yield break;
    }

    void UpdateOtherPlayerPosH(string sender, float x, float y, float rx, float ry)
    {
        StartCoroutine(UpdateOtherPlayerPos(sender, x, y, rx, ry));
    }
    IEnumerator UpdateOtherPlayerPos(string sender, float x, float y, float rx, float ry)
    {
        if (id == sender)
        {
            Vector2 pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;

            Vector2 dir = new Vector2(rx, ry);

            // use angle to rotate bow
            bow.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * Mathf.Atan2(dir.y, dir.x), Vector3.forward);
            bow.transform.position = pos + -1 * dir.normalized * bowdistance;
        }
        yield break;
    }

    void PlayerAnimHandleH(string sender, string name)
    {
        StartCoroutine(PlayerAnimHandle(sender, name));
    }
    IEnumerator PlayerAnimHandle(string sender, string name)
    {
        if (id == sender)
        {
            // handle the weird way the bow works
            if (name == "drawbow")
            {
                bowrender.sprite = f1;
            }
            else if (name == "relebow")
            {
                bowrender.sprite = f2;
            }
            else if (name == "dashanim")
            {
                StartCoroutine(dashAnim(rb.position));
            }
            // handle actual animations
            else
            {
                // animation.Play(name)
            }
        }
        yield break;
    }

    void SpawnProjHandleH(string sender, string name, Vector2 pos, Vector2 dir, Quaternion rot)
    {
        StartCoroutine(SpawnProjHandle(sender, name, pos, dir, rot));
    }
    IEnumerator SpawnProjHandle(string sender, string name, Vector2 pos, Vector2 dir, Quaternion rot)
    {
        if (id == sender)
        {
            // for now just do arrows, name could specify the projectile
            GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>(name), pos, rot);
            arrow.GetComponent<Rigidbody2D>().velocity = dir.normalized * ARROW_SPEED * -1;
        }
        yield break;
    }

    // called in fixed interval
    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        
    }

    // makes player invisible and unresponsive so that they could potentially be
    // revived
    protected override void Dead()
    {
        base.Dead();
    }


    // simply adds a force to the list to be applied next update.
    void applyForce(Vector2 force)
    {
        forces.Add(force);
    }

    void OnMouseDown()
    {

    }

    void OnCollisionEnter2D(Collision2D collision)
    {

    }


    IEnumerator dashAnim(Vector3 opos)
    {
        for (int i = -10; i <= 10; i++)
        {
            Color c = Color.Lerp(Color.white, Color.green, (float)Mathf.Abs(Mathf.Abs(i) - 10) / 10);
            render.color = c;
            yield return null;
        }
    }
}
