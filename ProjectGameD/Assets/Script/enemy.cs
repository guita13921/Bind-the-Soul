using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
  public Health health;

  void Start(){
    health=GetComponent<Health>();
  }
}
