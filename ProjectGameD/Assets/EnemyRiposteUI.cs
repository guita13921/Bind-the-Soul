using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class EnemyRiposteUI : MonoBehaviour
    {
        public EnemyManager enemyManager;
        public GameObject riposteUI;
        public Transform uiTransform;
        //public Vector3 offset = new Vector3(0, 2.5f, 0); // ปรับให้เหมาะกับตำแหน่ง HP

        void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
            if (enemyManager == null || riposteUI == null) return;

            // ติดตามตำแหน่ง Enemy
            // uiTransform.position = enemyManager.transform.position + offset;

            // หมุนให้หากล้อง
            //uiTransform.forward = Camera.main.transform.forward;

            // แสดง/ซ่อน UI ตามสถานะ
            riposteUI.SetActive(enemyManager.canBeRiposted);
        }
    }
}