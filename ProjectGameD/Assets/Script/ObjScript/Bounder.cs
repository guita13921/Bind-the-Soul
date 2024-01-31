using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bounder : MonoBehaviour
{
    // Start is called before the first frame update
    float drawback = float.Parse("0.2");
    // Update is called once per frames
    void OnTriggerEnter(Collider other){
        if(other.gameObject.tag == "Player"){
            Debug.Log("Enter");
            other.transform.position = new Vector3(other.transform.position.x-drawback, other.transform.position.y, other.transform.position.z-drawback);
        }
    }

}
