using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class InitalizePlayer : MonoBehaviour
    {
        public Transform playerTransForm;

        void Start()
        {
            PlayerManager.transform = playerTransForm;
        }
    }
}
