using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class priestControllerOP : playerBaseOP
{
    private GameObject staff;
    private float bowdistance;
    public float PMOVEMENT_SPEED;
    public float AUTO_SPEED;
    public float radius = 0.3f;
    public float linewidth = 0.2f;
    public int vertexcount = 40;
    LineRenderer lineRenderer;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        staff = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (staff.transform.position - (Vector3)rb.position).magnitude;
        lineRenderer = GetComponent<LineRenderer>();
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
            Vector2 pos;
            Vector2 direction;
            float angle;
            pos = transform.position;
            pos.x = x;
            pos.y = y;
            transform.position = pos;

            direction = new Vector2(rx, ry);
            angle = Mathf.Atan2(direction.y, direction.x);
            staff.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
            staff.transform.position = pos + -1 * direction.normalized * bowdistance;
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
            if (name == "EAbility")
            {
                StartCoroutine(EAbilityAnim(pos));
            }
            else if (name == "p_autoOP")
            {
                GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("p_autoOP"), pos, rot);
                arrow.GetComponent<Rigidbody2D>().velocity = dir.normalized * AUTO_SPEED * -1;
            } 
        }
        yield break;
    }

    IEnumerator EAbilityAnim(Vector3 c)
    {
        GameObject circle = (GameObject)Instantiate(Resources.Load<GameObject>("HealCircle"), c, Quaternion.identity);

        float x = circle.gameObject.transform.position.x;
        float y = circle.gameObject.transform.position.y;
        circle.transform.position = new Vector3(x, y, -3f);

        yield return new WaitForSeconds(1f);

        UnityEngine.Object.Destroy(circle);

        yield return null;
    }

    protected override void FixedUpdate()
    {

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

    void OnCollisionEnter2D(Collision2D collision)
    {

    }

    // reduces player health, if its 0 then call Dead(), if not then apply
    // a knockback force given by dir
    public override void TakeDamage(float dmg, Vector2 dir)
    {
         base.TakeDamage(dmg, dir);
    }

}

