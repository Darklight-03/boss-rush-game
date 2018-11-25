﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class priestController : playerBase {

    private GameObject staff;
    private float bowdistance;
    public float PMOVEMENT_SPEED;
    public float AUTO_SPEED;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        MOVEMENT_SPEED = PMOVEMENT_SPEED;
        staff = gameObject.transform.GetChild(0).gameObject;
        bowdistance = (staff.transform.position - (Vector3)rb.position).magnitude;
        interfaceplayertext.text = "You: Priest";
    }

    protected override void shiftAbilityInit(){
        SHIFT_CD = GLOBAL_CD;
        SHIFT_NAME = "notyetimplemented";
    }

    protected override void lmbAbilityInit(){
        LMB_NAME = "green fire";
    }

    protected override void rmbAbilityInit(){
        RMB_CD = GLOBAL_CD;
        RMB_NAME = "notyetimplemented";
    }

    protected override void eAbilityInit(){
        E_CD = 12f;
        E_NAME = "heal";
    }

    protected override void qAbilityInit(){
        Q_CD = GLOBAL_CD;
        Q_NAME = "notyetimplmented";
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        // use angle to rotate bow
        staff.transform.rotation = Quaternion.AngleAxis(Mathf.Rad2Deg * angle, transform.forward);
        staff.transform.position = rb.position + -1 * direction.normalized * bowdistance;
    }

    protected override void LMBClicked(){

    }

    protected override void LMBReleased(){
         GameObject arrow = (GameObject)Instantiate(Resources.Load<GameObject>("p_auto"), staff.transform.position, staff.transform.rotation, GetComponent<Transform>());
        arrow.GetComponent<Rigidbody2D>().velocity = direction.normalized * AUTO_SPEED * -1;
    }

    protected override void LShiftAbility(Vector2 input){
        //input is vector from player to mouse but can be changed or added to
        Debug.Log("notimplementedyet");
    }

    protected override void RMBAbility(){
        Debug.Log("notimplementedyet");
    }
    
    protected override void QAbility(){
        Debug.Log("notimplementedyet");
    }

    protected override void EAbility(){
        GameObject[] healPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach(var p in healPlayers){
            if((mousePosition - (Vector2)p.transform.position).magnitude < .30){
                Debug.Log(p.transform.position);
                //HEAL THE P
                (p).GetComponent<playerBase>().Heal(1000);
            }
        }
    }

    protected override void Dead(){
        base.Dead();
    }

}
