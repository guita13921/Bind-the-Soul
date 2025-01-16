using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class damageShow : MonoBehaviour
{
    public TextMeshPro textMesh; // Reference to the TextMesh component

    public void SetDamage(float damage)
    {
        if (textMesh != null)
        {
            textMesh.text = damage.ToString(); // Display the damage value
        }
        else
        {
            Debug.Log("BRUGGG");
        }

        Destroy(gameObject, 0.25f);
    }
}
