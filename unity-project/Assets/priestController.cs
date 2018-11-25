using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class priestController : MonoBehaviour {

    private Rigidbody2D rb;
    private GameObject staff;
    private float bowdistance;
    int knocked;
    public float MOVEMENT_SPEED;
    public float AUTO_SPEED;
    public float GLOBAL_CD = 0.3f;
    List<Vector2> forces;
    Vector2 realvelocity;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        staff = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (staff.transform.position - (Vector3)rb.position).magnitude;
        knocked = 0;
        forces = new List<Vector2>();
        realvelocity = new Vector2(0, 0);
    }

    void FixedUpdate()
    {
        /* MOVEMENT */
        // input x and y
        float ix = Input.GetAxis("Horizontal");
        float iy = Input.GetAxis("Vertical");

        // get velocity input
        var inputvelocity = new Vector2(ix, iy);

        // later can add velocity vectors together for knockback and stuff
        if (knocked == 0)
        {
            rb.position = rb.position + inputvelocity * MOVEMENT_SPEED;
            //rb.velocity = (inputvelocity*MOVEMENT_SPEED);
        }
        else
        {
            knocked--;
        }

        var forcesSum = new Vector2(0, 0);
        foreach (Vector2 v in forces)
        {
            forcesSum += v;
        }

        rb.AddForce(forcesSum);
        realvelocity = rb.velocity + inputvelocity;

        forces.Clear();
    }

    void applyForce(Vector2 force)
    {
        forces.Add(force);
    }

    // Update is called once per frame
    void Update()
    {
        /* ROTATION */
        // get position of main sprite and mouse
        Vector2 pos = rb.position;
        Vector2 mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // get directional vector and convert to angle
        Vector2 direction = pos - mouse;
        float angle = Mathf.Atan2(direction.y, direction.x);

        // use angle to rotate bow
        staff.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
        staff.transform.position = pos + -1 * direction.normalized * bowdistance;
        if (Input.GetMouseButton(0))
        {
            GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("p_auto"), staff.transform.position, staff.transform.rotation, GetComponent<Transform>());
            arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized * AUTO_SPEED * -1;
        }
    }
}
