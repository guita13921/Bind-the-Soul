using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatConfig : MonoBehaviour
{
    [SerializeField] private EnemyAI3 scriptToAccess; // Reference to your original script
    [SerializeField] private Slider Health_Bar_Erase;
    [SerializeField] private Slider Health_Bar;
    [SerializeField] float  IN_KnockBackTime;
    [SerializeField] float  IN_CoolDownAttack;
    [SerializeField] int    IN_numberOfRandomVariations; 
    [SerializeField] int    IN_Health; 
    [SerializeField] int    IN_Speed; 
    [SerializeField] int    Damage;

    public void Start()
    {
        scriptToAccess.SetStat(IN_KnockBackTime, IN_CoolDownAttack, IN_numberOfRandomVariations);
        Health_Bar_Erase.maxValue = IN_Health;
        Health_Bar.maxValue = IN_Health;
    }
}