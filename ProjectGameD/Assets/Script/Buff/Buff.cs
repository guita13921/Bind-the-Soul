using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buff
{
    public string name;
    public bool isPermanent;
    public float duration;
    public float effectValue;
    public Sprite icon;
    public System.Action onApply;
    public System.Action onRemove;

    public Buff(
        string name,
        float duration,
        float effectValue,
        Sprite icon,
        System.Action onApply = null,
        System.Action onRemove = null
    )
    {
        this.name = name;
        this.isPermanent = false;
        this.duration = duration;
        this.effectValue = effectValue;
        this.icon = icon;
        this.onApply = onApply;
        this.onRemove = onRemove;
    }

    public Buff(string name, float effectValue, Sprite icon, System.Action onApply = null)
    {
        this.name = name;
        this.isPermanent = true;
        this.effectValue = effectValue;
        this.icon = icon;
        this.onApply = onApply;
    }

    public void Update(float deltaTime)
    {
        if (!isPermanent)
        {
            duration -= deltaTime;
        }
    }
}
