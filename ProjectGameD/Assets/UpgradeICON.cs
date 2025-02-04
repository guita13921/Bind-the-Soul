using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeICON : MonoBehaviour
{
    public TextMeshProUGUI tmpText;
    public Sprite[] icon;
    public Image image;

    void Start()
    {
        image.color = new Color(255f, 255f, 255f); // Change to the appropriate index

        if (tmpText != null && image != null && icon.Length > 0)
        {
            switch (tmpText.text)
            {
                case "Vampirism":
                    image.sprite = icon[0]; //blade image
                    break;
                case "Iron Body": //body image
                    image.sprite = icon[1]; // Change to the appropriate index
                    break;
                case "Super Special": //claw iamge
                    image.sprite = icon[2]; // Change to the appropriate index
                    image.color = new Color(255f, 0f, 0f); // Change to the appropriate index

                    break;
                case "Threshold Revival":
                    image.sprite = icon[3]; // Change to the appropriate index
                    image.color = new Color(102f, 199f, 0f); // Change to the appropriate index

                    break;
                case "Acceleration":
                    image.sprite = icon[4]; // Change to the appropriate index
                    break;
                case "Overgrowth":
                    image.sprite = icon[1]; // Change to the appropriate index
                    image.color = new Color(0f, 255f, 0f); // Change to the appropriate index

                    break;
                case "Lethal Strike":
                    image.sprite = icon[0]; //blade image
                    image.color = new Color(255f, 255f, 0f); // Change to the appropriate index

                    break;
                case "Superspeed":
                    image.sprite = icon[5]; //perosn sliding image
                    break;
                case "Swift Step":
                    image.sprite = icon[6]; //perosn dashing image
                    break;
                case "Unyielding spirit":
                    image.sprite = icon[1]; // Change to the appropriate index
                    image.color = new Color(255f, 0f, 0f); // Change to the appropriate index

                    break;
                case "Last Stand":
                    image.sprite = icon[9]; // pound image
                    image.color = new Color(255f, 0f, 0f); // Change to the appropriate index

                    break;
                case "Evolution: Red Blade":
                    image.sprite = icon[2]; // Change to the appropriate index
                    image.color = new Color(255f, 0f, 0f); // Change to the appropriate index
                    gameObject.transform.localScale = new Vector3(
                        -gameObject.transform.localScale.x,
                        gameObject.transform.localScale.y,
                        gameObject.transform.localScale.z
                    );
                    break;
                case "Evolution: Homing Bullet":
                    image.sprite = icon[7]; // Change to the appropriate index

                    break;
                case "Evolution: Aura":
                    image.sprite = icon[8]; // black hole image
                    image.color = new Color(158f, 0f, 255f); // Change to the appropriate index

                    break;

                case "Killing Strike":
                    image.sprite = icon[0];
                    image.color = new Color(255f, 0f, 0f); // Change to the appropriate index

                    break;
                case "Maximum Output":
                    image.sprite = icon[10]; // beam image

                    break;

                case "Super Lethal":
                    image.sprite = icon[2];
                    image.color = new Color(255f, 255f, 0); // Change to the appropriate index

                    break;

                case "Stackable": //bunch of item
                    image.sprite = icon[11];

                    break;
                case "Little Bee":
                    image.sprite = icon[2];
                    image.color = new Color(100, 255f, 69); // Change to the appropriate index

                    break;
            }
        }
        else
        {
            Debug.LogError("Missing references or empty icon array!");
        }
    }
}
