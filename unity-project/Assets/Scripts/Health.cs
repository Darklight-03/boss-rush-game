using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour {
  float hp;
  float max_hp;

	// Use this for initialization
	void Start () {
    hp = 100;
    max_hp = 100;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  public bool TakeDamage(float dmg){
    hp -= dmg;
    if(hp<0){
      return false;
    }
    if(hp>max_hp){
      hp = max_hp;
    }
    return true;
  }

  public void SetHP(float current, float max){
    hp = current;
    max_hp = max;
  }
}
