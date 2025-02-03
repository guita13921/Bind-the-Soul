using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soulbondcheck : MonoBehaviour
{
    public CharacterData characterData;

    void Start()
    {
        if (characterData.deathCount > 5)
            gameObject.SetActive(false);
    }
}
