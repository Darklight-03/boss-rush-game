using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
  public float hp;
  public float max_hp;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	}

  public bool TakeDamage(float dmg){
    hp -= dmg;
    if(hp<=0){
      return false;
    }
    if(hp>max_hp){
      hp = max_hp;
    }
    return true;
  }

    public void Heal(float amount)
    {
        hp += amount;

        if (hp > max_hp)
        {
            hp = max_hp;
        }

    }

  public void SetHP(float current, float max){
    hp = current;
    max_hp = max;
  }

  public float getCurrentHP(){
    return hp;
  }
  
  public float getMaxHP(){
    return max_hp;
  }
}
