using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TESTbossController : MonoBehaviour {


   
    bool swingit;
    Transform sword;
    public GameObject[] gameObjects;
    Vector3 direction;
    Vector3 bossposition;
    public float MOVE_SPEED;
    public float SWING_SPEED = 50;
    private bool find = true;



	// Use this for initialization
	void Start () {
        sword = GetComponent<Transform>().GetChild(0);
        swingit = true;
        gameObjects = GameObject.FindGameObjectsWithTag("Player");

    }
	
	// Update is called once per frame
	void Update () {

            if(swingit)
            {
                Debug.Log("swung");
                StartCoroutine(swing());
            }

        GetComponent<Rigidbody2D>().velocity = (Vector2)direction;
        


	}

    private void FixedUpdate()
    {
        if (find)
        {
            StartCoroutine(findDirection());
        }


    }

    IEnumerator swing()
    {
        Debug.Log("swinging");
        swingit = false;
        for (int i = 0; i < SWING_SPEED; ++i)
        {
            sword.RotateAround(GetComponent<BoxCollider2D>().transform.position, Vector3.forward,(float)((float)1/(float)SWING_SPEED)*360f);
            yield return null;
        }
        yield return new WaitForSeconds(1.5f);
        swingit = true;
        yield return null;
    }

    IEnumerator findDirection()
    {
        find = false;
        bossposition = GetComponent<Transform>().position;
        float x = 0, y = 0, z = 0;
        Vector3 p1, p2, p3;

        //vectors from boss' location to each player's location
        p1 = gameObjects[0].transform.position - bossposition;
        p2 = gameObjects[1].transform.position - bossposition;
        p3 = gameObjects[2].transform.position - bossposition;

        //finding avg x and y values to determine general direction boss should travel
        x = (p1.x + p2.x + p3.x) / 3;
        y = (p1.y + p2.y + p3.y) / 3;

        direction.x = x;
        direction.y = y;
        direction.z = 0;

        //convert to unit vector
        direction = direction / direction.magnitude * MOVE_SPEED;

        yield return new WaitForSeconds(1.2f);
        find = true;
        yield return null;
    }
}
