using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class EnemyUIYellowBar : MonoBehaviour
    {
        public Slider slider;
        UIEnemyHealthBar parentHealthBar;

        public float timer;

        private void OnEnable()
        {
            if (timer <= 0)
            {
                timer = 2f;
            }
        }
        public void SetParentHealthBar(UIEnemyHealthBar healthBar)
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
                return;
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