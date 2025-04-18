using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SG
{
    public class DropManager : MonoBehaviour
    {

        void Start()
        {
            PlayerManager.availableDrops = Resources.LoadAll<Drop>("Drops").ToList();
        }

    }
}