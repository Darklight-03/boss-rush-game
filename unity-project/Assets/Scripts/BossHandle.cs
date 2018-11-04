using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandle : MonoBehaviour
{
    private Rigidbody2D rb;
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
        state = true;
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        //swordcol = GameObject.Find("short-sword").GetComponent<Collider2D>();
        
        gameObjects = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log(gameObjects.Length);
        player = GameObject.FindWithTag("Player");//delete after merge
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
            if(max1 < temp)
            {
                temp = max1;
                player = g;
            }
        }

       
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


    }

    // Update is called once per frame
    void Update()
    {
        //player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        if ((v1 - v2).magnitude < 3 && state)
        {
            animation.Play("huijian");
            
            state = false;
            Invoke("PlayGameeffects", 0.3f);
            Invoke("ChangeStae", 1f);
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
}
