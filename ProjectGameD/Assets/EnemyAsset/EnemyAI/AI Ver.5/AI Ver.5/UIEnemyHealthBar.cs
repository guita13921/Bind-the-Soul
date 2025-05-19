using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        public Slider slider;
        private float timeUntillBarHidden = 0;
        public Transform mainCamera;
        [SerializeField] EnemyUIYellowBar yellowBar;
        [SerializeField] float yellowBarTimer = 3;

        [SerializeField] TextMeshProUGUI damageText;
        [SerializeField] int currentDamageTaken;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            mainCamera = FindObjectOfType<CameraHandler>().transform;
        }
        private void OnDisable()
        {
            currentDamageTaken = 0;
        }

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

            currentDamageTaken = currentDamageTaken + Mathf.RoundToInt(slider.value - health);
            damageText.text = currentDamageTaken.ToString();

            slider.value = health;
            timeUntillBarHidden = 5;
        }

        public void SetMaxHealth(int maxHealth)
        {
            if (slider == null)
            {
                Debug.LogError("Slider is null in SetMaxHealth!");
                return;
            }
            if (yellowBar != null)
            {
                yellowBar.SetMaxStat(maxHealth);
                yellowBar.SetParentHealthBar(this);
            }


            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);
            timeUntillBarHidden = timeUntillBarHidden - Time.deltaTime;
            if (slider != null)
            {
                if (timeUntillBarHidden <= 0)
                {
                    timeUntillBarHidden = 0;
                    slider.gameObject.SetActive(false);
                }
                else
                {
                    if (!slider.gameObject.activeInHierarchy)
                    {
                        slider.gameObject.SetActive(true);
                    }
                }
                if (slider.value <= 0)
                {
                    Destroy(slider.gameObject);
                }
            }
        }
    }
}