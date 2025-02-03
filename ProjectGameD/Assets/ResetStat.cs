using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetStat : MonoBehaviour
{
    [SerializeField]
    CharacterData characterData;

    void Start()
    {
        characterData.ResetToDefault();
        characterData.deathCount = 0;
    }
}
