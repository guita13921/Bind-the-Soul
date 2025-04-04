using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class StaminaBar : MonoBehaviour
    {
        public Slider Slider;
        private void Start()
        {
            Slider = GetComponent<Slider>();
        }
        public void SetMaxStamina(int maxStamina)
        {
            Slider.maxValue = maxStamina;
            Slider.value = maxStamina;
        }

        public void SetcurrentStamina(int currentStamina)
        {
            Slider.value = currentStamina;
        }
    }

}
