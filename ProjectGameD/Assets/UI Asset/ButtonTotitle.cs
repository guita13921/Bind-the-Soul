using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonTotitle : MonoBehaviour
{
public CharacterData characterData;

    void Start()
    {
        if(characterData.deathCount ==5 ){
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
