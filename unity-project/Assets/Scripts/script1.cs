using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class playerController1 : MonoBehaviour
{
    private Rigidbody2D rb;

    // Use this for initialization
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // called in fixed interval
    void FixedUpdate()
    {
        // input x and y
        float ix = Input.GetAxis("Horizontal");
        float iy = Input.GetAxis("Vertical");

        // get velocity input
        var inputvelocity = new Vector2(ix, iy);

        // later can add velocity vectors together for knockback and stuff
        rb.velocity = inputvelocity * 0.5f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
