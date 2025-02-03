using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffStorage", menuName = "Character/buff")]
public class StoredBuff : ScriptableObject
{
    public List<Buff> storedBuffs;
    public bool Firsttime = true;

    public void Storebuff(Buff buff)
    {
        if (Firsttime)
        {
            storedBuffs = new List<Buff>();
        }

        Firsttime = false;

        storedBuffs.Add(buff);
    }
}
