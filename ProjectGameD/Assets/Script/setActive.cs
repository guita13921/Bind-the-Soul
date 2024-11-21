using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setActive : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        gameObject.SetActive(true);

        // Check if the parent exists
        if (transform.parent != null)
        {
            Debug.Log("Parent exists: " + transform.parent.name);
        }
        else
        {
            Debug.Log("No parent found.");
        }
    }

    // Update is called once per frame
}
