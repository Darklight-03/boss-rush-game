using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour {
  private Rigidbody2D rb;
    Collider2D swordcol;
    Animator myanmitor;
    public float speed;
    public Animation animation;
    public GameObject daoguang;
    public RectTransform image;
    // Use this for initialization
    void Start () {
        animation = this.GetComponent<Animation>();
        rb = GetComponent<Rigidbody2D>();
        swordcol = GameObject.Find("short-sword").GetComponent<Collider2D>();

    }

  // called in fixed interval
  void FixedUpdate(){
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        rb.velocity = v2 - v1;
        
        if(Mathf.Abs(v2.x - v1.x)>Mathf.Abs(v2.y-v1.y))
        {
            if(v2.x>v1.x)
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
    void Update () {
        GameObject player = GameObject.FindWithTag("Player");
        Vector2 v1 = transform.position;
        Vector2 v2 = player.transform.position;
        if ((v1-v2).magnitude < 3)
        {
            animation.Play("huijian");
            daoguang.SetActive(true);
        }
        else
        {
            daoguang.SetActive(false);
        }
	}
}
