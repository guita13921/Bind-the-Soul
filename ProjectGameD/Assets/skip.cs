using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class skip : MonoBehaviour
{

    public SceneTransition sceneTransition;
    [SerializeField]string NextScene;
    void Update()
    {
                if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneTransition.NextScene(NextScene); // Adjust the method name as needed
        }
    }
}
