using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounderZ : MonoBehaviour
{
    // Start is called before the first frame update
    float drawback = float.Parse("0.2");
    // Update is called once per frames
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            Debug.Log("Enter");
            other.transform.position = new Vector3(other.transform.position.x, other.transform.position.y, other.transform.position.z-drawback);
        }
    }
}
