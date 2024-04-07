using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Goap.Interfaces;
using UnityEngine;


namespace EnemyAI.GOAP.Actions{
    public class CommonData : IActionData
    {
        public ITarget Target{get; set;}
        public float Timer{get; set;}
    }
}