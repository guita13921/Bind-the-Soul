using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ProjectileAttack : MonoBehaviour
{
    [SerializeField]
    GameObject followArrow;
    public CharacterData characterData;

    public void SpwanBull()
    {
        if (characterData.Q2_SmallBullet)
            Instantiate(followArrow, this.transform.position, Quaternion.identity);
    }
}
