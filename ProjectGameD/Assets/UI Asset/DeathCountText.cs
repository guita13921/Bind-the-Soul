using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathCountText : MonoBehaviour
{
    // Start is called before the first frame update

    private TextMeshProUGUI textMeshPro;
        public CharacterData characterData;
    [SerializeField]bool first = true;

    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
                int deathcountt =characterData.deathCount;

        if (first) deathcountt--;

        textMeshPro.text =(5 - deathcountt).ToString();
    }


}
