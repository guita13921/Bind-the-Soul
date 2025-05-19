using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UIYellowBar : MonoBehaviour
    {
        public Slider slider;
        HealthBar parentHealthBar;

        public float timer;

        private void OnEnable()
        {
            if (timer <= 0)
            {
                timer = 1f;
            }
        }
        public void SetParentHealthBar(HealthBar healthBar)
        {
            parentHealthBar = healthBar;
        }


        public void SetMaxStat(int MaxStat)
        {
            slider.maxValue = MaxStat;
            slider.value = MaxStat;
        }
        private void Update()
        {
            if (parentHealthBar == null || parentHealthBar.slider == null)
            {
                return; // ป้องกัน NullReferenceException
            }
            if (timer <= 0)
            {
                if (slider.value > parentHealthBar.slider.value)
                {
                    slider.value = slider.value - 0.5f;
                }
                else if (slider.value <= parentHealthBar.slider.value)
                {
                    gameObject.SetActive(false);
                }
            }
            else
            {
                timer = timer - Time.deltaTime;
            }
        }

    }
}
