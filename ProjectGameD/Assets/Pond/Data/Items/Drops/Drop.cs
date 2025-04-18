using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace SG
{
    [CreateAssetMenu(menuName = "Drops")]
    public class Drop : ScriptableObject
    {
        public GameObject Prefab;
        public float DropChance;
        public AudioSource audioSource;

    }
}