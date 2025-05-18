using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class StaminaBar : MonoBehaviour
    {
        public Slider Slider;

        [SerializeField] private RectTransform sliderRectTransform;
        [SerializeField] private float widthPerUnit = 2f;

        private void Start()
        {
            Slider = GetComponent<Slider>();
        }
        public void SetMaxStamina(int maxStamina)
        {
            Slider.maxValue = maxStamina;
            Slider.value = maxStamina;

            if (sliderRectTransform != null)
            {
                Vector2 size = sliderRectTransform.sizeDelta;
                size.x = maxStamina * widthPerUnit;
                sliderRectTransform.sizeDelta = size;
            }
        }

        public void SetcurrentStamina(int currentStamina)
        {
            Slider.value = currentStamina;
        }
    }

}
