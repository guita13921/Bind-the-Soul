using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace SG
{
    public class HealthBar : MonoBehaviour
    {
        public Slider slider;

        private float timeUntilBarIsHidden = 0;
        [SerializeField] UIYellowBar yellowBar;
        [SerializeField] float yellowBarTimer = 3;

        // [SerializeField] TextMeshProUGUI damageText;
        // [SerializeField] int currentDamageTaken;

        [SerializeField] private RectTransform sliderRectTransform;
        [SerializeField] private float widthPerUnit = 2f;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
        }

        /*  private void OnDisable()
          {
              currentDamageTaken = 0;
          }*/

        public void SetHealth(int health)
        {
            if (yellowBar != null)
            {
                yellowBar.gameObject.SetActive(true);

                yellowBar.timer = yellowBarTimer;

                if (health > slider.value)
                {
                    yellowBar.slider.value = health;
                }
            }

            //currentDamageTaken = currentDamageTaken + Mathf.RoundToInt(slider.value - health);
            //damageText.text = currentDamageTaken.ToString();

            slider.value = health;
            timeUntilBarIsHidden = 1;
        }
        public void SetMaxHealth(int MaxHealth)
        {
            slider.maxValue = MaxHealth;
            slider.value = MaxHealth;

            if (yellowBar != null)
            {
                yellowBar.SetMaxStat(MaxHealth);
                yellowBar.SetParentHealthBar(this);
            }

            if (sliderRectTransform != null)
            {
                Vector2 size = sliderRectTransform.sizeDelta;
                size.x = MaxHealth * widthPerUnit;
                sliderRectTransform.sizeDelta = size;
            }
        }

        /*  public void SetCurrentHealth(int currentHealth)
          {
              slider.value = currentHealth;
          }*/

        private void Update()
        {
            //transform.LookAt(transform.position + Camera.main.transform.forward);
            //timeUntilBarIsHidden = timeUntilBarIsHidden - Time.deltaTime;
            if (slider != null)
            {
                /*if (timeUntilBarIsHidden <= 0)
                {
                    timeUntilBarIsHidden = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    if (!slider.gameObject.activeInHierarchy)
                    {
                        slider.gameObject.SetActive(true);
                    }
                }*/
                if (slider.value <= 0)
                {
                    Destroy(slider.gameObject);
                }
            }
        }


    }
}