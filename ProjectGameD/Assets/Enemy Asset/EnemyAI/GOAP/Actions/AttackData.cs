using UnityEngine;
using CrashKonijn.Goap.Classes.References;
using EnemyAI.GOAP.Actions;

namespace EnemyAI.GOPA.Actions{

public class AttackData : CommonData
{
    public static readonly int ATTACK = Animator.StringToHash("Attack");

    [GetComponent]
    public Animator Animator { get; set; }
    public BoxCollider boxCollider { get; set; }
    public new float DelayTimer { get; set; } = 1.5f; 
}

}