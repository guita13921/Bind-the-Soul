using System.Collections.Generic;
using UnityEngine;

public class BuffManager : MonoBehaviour
{
    private List<Buff> activeBuffs = new List<Buff>();

    public void AddBuff(Buff newBuff)
    {
        Buff existingBuff = activeBuffs.Find(buff => buff.name == newBuff.name);
        if (existingBuff != null)
        {
            if (!newBuff.isPermanent && newBuff.duration > existingBuff.duration)
            {
                existingBuff.duration = newBuff.duration;
            }
            return;
        }

        activeBuffs.Add(newBuff);
        newBuff.onApply?.Invoke();
    }

    public void RemoveBuff(Buff buffToRemove)
    {
        if (activeBuffs.Contains(buffToRemove))
        {
            buffToRemove.onRemove?.Invoke();
            activeBuffs.Remove(buffToRemove);
        }
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        for (int i = activeBuffs.Count - 1; i >= 0; i--)
        {
            if (!activeBuffs[i].isPermanent)
            {
                activeBuffs[i].Update(deltaTime);
                if (activeBuffs[i].duration <= 0)
                {
                    RemoveBuff(activeBuffs[i]);
                }
            }
        }
    }

    public List<Buff> GetActiveBuffs()
    {
        return activeBuffs;
    }
}
