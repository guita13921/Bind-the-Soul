using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorChange : MonoBehaviour
{
    // Start is called before the first frame update
    Image image; // Assign this in the Inspector
    public CharacterData characterData;

    void Start()
    {
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (characterData.QSkillType)
        {
            case 1:
                image.color = new Color(255f, 0f, 0f);
                break;
            case 2:
                image.color = new Color(0f, 255f, 0f); // Change to red

                break;
            case 3:
                image.color = new Color(0f, 0f, 255f); // Change to red
                break;
            default:
                image.color = Color.white; // Default color
                break;
        }
    }
}
