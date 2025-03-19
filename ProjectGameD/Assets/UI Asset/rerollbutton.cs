using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RerollButton : MonoBehaviour
{
    public CharacterData characterData;
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
    }

    void Update()
    {
        if (button != null && characterData != null)
        {
            button.interactable = characterData.rerollpoint > 0;
        }
    }
}
