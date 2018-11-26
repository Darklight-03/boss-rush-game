using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class knightControllerOP : playerBaseOP
{
    private GameObject shield;
    private GameObject sword;
    private float bowdistance;
    private float sworddistance;
    int knocked;
    bool invincible;
    private Vector2 prevPos = new Vector2(0, 0);
    private Vector2 prevRot = new Vector2(0, 0);
    string weapon;
    public bool stabbing = false;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        shield = gameObject.transform.GetChild(0).gameObject;
        sword = gameObject.transform.GetChild(1).gameObject;
        bowdistance = (shield.transform.position - (Vector3)rb.position).magnitude;
        sworddistance = (sword.transform.position - (Vector3)rb.position).magnitude;
        invincible = false;
        weapon = "shield";
        //at start of game, knight has shield enabled by default
        shield.GetComponent<SpriteRenderer>().enabled = true;
        sword.GetComponent<SpriteRenderer>().enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SocketNetworkManager.UpdateOtherPlayerPos += UpdateOtherPlayerPosH;
        SocketNetworkManager.PlayerAnimHandle += PlayerAnimHandleH;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        SocketNetworkManager.TakeDamageHandle -= TakeDamageHandleH;
        SocketNetworkManager.UpdateOtherPlayerPos -= UpdateOtherPlayerPosH;
        SocketNetworkManager.PlayerAnimHandle -= PlayerAnimHandleH;
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
            Vector2 pos;
            Vector2 direction;
            float angle;
            pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;

            direction = new Vector2(rx, ry);
            angle = Mathf.Atan2(direction.y, direction.x);
            shield.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
            shield.transform.position = pos + -1 * direction.normalized * bowdistance;
            sword.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * (angle), transform.forward);
            sword.transform.position = pos + -1 * direction.normalized * sworddistance;
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
            if (name == "switch")
            {
                if (weapon == "shield")
                {
                    weapon = "sword";
                    shield.GetComponent<SpriteRenderer>().enabled = false;
                    sword.GetComponent<SpriteRenderer>().enabled = true;
                }
                else
                {
                    weapon = "shield";
                    sword.GetComponent<SpriteRenderer>().enabled = false;
                    shield.GetComponent<SpriteRenderer>().enabled = true;
                }
            }
            else if (name == "stab")
            {
                StartCoroutine(stabAnimation(10));
            }
            // handle actual animations
            else
            {
                // animation.Play(name)
            }
        }
        yield break;
    }

    protected override void FixedUpdate()
    {

    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    IEnumerator stabAnimation(int val)
    {
        stabbing = true;
        float olddist = sworddistance;
        sworddistance = olddist + 0.5f;
        while (val >= 0)
        {
            val -= 1;
            yield return new WaitForEndOfFrame();
        }
        sworddistance = olddist;
        stabbing = false;
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        
    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public override void TakeDamage(float dmg, Vector2 dir)
    {
        if (weapon == "shield")
        {
            dmg = dmg / 2;
        }
        base.TakeDamage(dmg, dir);
    }

}

