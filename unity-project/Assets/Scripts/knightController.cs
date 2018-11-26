using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class knightController : playerBase
{
    private GameObject shield;
    private GameObject sword;
    private float bowdistance;
    private float sworddistance;
    public float KMOVEMENT_SPEED = 0.1f;
    bool invincible;
    string weapon;
    public bool stabbing = false;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        MOVEMENT_SPEED = KMOVEMENT_SPEED;
        shield = gameObject.transform.GetChild(0).gameObject;
        sword = gameObject.transform.GetChild(1).gameObject;
        bowdistance = (shield.transform.position - (Vector3)rb.position).magnitude;
        sworddistance = (sword.transform.position - (Vector3)rb.position).magnitude;
        interfaceplayertext.text = "You: Knight";
        invincible = false;
        weapon = "shield";
        //at start of game, knight has shield enabled by default
        shield.GetComponent<SpriteRenderer>().enabled = true;
        sword.GetComponent<SpriteRenderer>().enabled = false;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void shiftAbilityInit()
    {
        SHIFT_CD = GLOBAL_CD;
        SHIFT_NAME = "notyetimplemented";
    }

    protected override void lmbAbilityInit()
    {
        LMB_NAME = "sword";
    }

    protected override void rmbAbilityInit()
    {
        RMB_CD = GLOBAL_CD;
        RMB_NAME = "notyetimplemented";
    }

    protected override void eAbilityInit()
    {
        E_CD = GLOBAL_CD;
        E_NAME = "notyetimplemented";
    }

    protected override void qAbilityInit()
    {
        Q_CD = GLOBAL_CD;
        Q_NAME = "switch";
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        shield.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
        shield.transform.position = rb.position + -1 * direction.normalized * bowdistance;
        sword.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * (angle), transform.forward);
        sword.transform.position = rb.position + -1 * direction.normalized * sworddistance;
    }

    protected override void LMBReleased()
    {
        if (weapon == "sword")
        {
            snm.sendMessage("pa", "{ \"name\": \"" + "stab" + "\" }");
            StartCoroutine(stabAnimation(10));
        }
    }

    protected override void LMBClicked()
    {
        Debug.Log("notyetimplemented");
    }
    protected override void LShiftAbility(Vector2 input)
    {
        Debug.Log("notyetimplemented");
    }
    protected override void EAbility()
    {
        Debug.Log("notyetimplemented");
    }
    protected override void RMBAbility()
    {
        Debug.Log("notyetimplemented");
    }

    protected override void QAbility()
    {
        snm.sendMessage("pa", "{ \"name\": \"" + "switch" + "\" }");
        if (weapon == "shield")
        {

            weapon = "sword";
            shield.GetComponent<SpriteRenderer>().enabled = false;
            //shield.SetActive(false);
            sword.GetComponent<SpriteRenderer>().enabled = true;
            //sword.GetComponent<EdgeCollider2D>(). = true;
            //sword.SetActive(true);
            //for (int i = 0; i < sword.transform.childCount; i++)
            //    sword.transform.GetChild(i).gameObject.SetActive(true);

        }
        else
        {
            weapon = "shield";
            sword.GetComponent<SpriteRenderer>().enabled = false;
            //sword.GetComponent<EdgeCollider2D>().enabled = false;
            //sword.SetActive(false);
            shield.GetComponent<SpriteRenderer>().enabled = true;
            //shield.SetActive(true);
            //for (int i = 0; i < shield.transform.childCount; i++)
            //    shield.transform.GetChild(i).gameObject.SetActive(true);
        }
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

