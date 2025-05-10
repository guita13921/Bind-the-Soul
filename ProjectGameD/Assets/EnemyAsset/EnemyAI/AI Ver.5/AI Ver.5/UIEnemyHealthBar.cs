using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class UIEnemyHealthBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        float timeUntillBarHidden = 0;
        public Transform mainCamera;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            mainCamera = FindObjectOfType<CameraHandler>().transform;
        }

        public void SetHealth(int health)
        {
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

            slider.maxValue = maxHealth;
            slider.value = maxHealth;
        }

        private void Update()
        {
            // Ensure the health bar always faces the camera
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.forward);
            }

            // Manage health bar visibility
            timeUntillBarHidden -= Time.deltaTime;

            if (slider != null)
            {
                if (timeUntillBarHidden <= 0)
                {
                    timeUntillBarHidden = 0;
                    //slider.gameObject.SetActive(false);
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