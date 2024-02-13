using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class resrt : MonoBehaviour
{

    private void Reload()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (Input.GetKeyDown(KeyCode.R))
            SceneManager.LoadScene(currentSceneIndex);
    }
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        Reload();
        
    }
}
