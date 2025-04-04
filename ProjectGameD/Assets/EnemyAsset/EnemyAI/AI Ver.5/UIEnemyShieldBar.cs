using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{

    public class UIEnemyShieldBar : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        float timeUntillBarHidden = 0;
        public Transform mainCamera;

        private void Awake()
        {
            slider = GetComponentInChildren<Slider>();
            mainCamera = Camera.main.transform; // Get the main camera
        }

        public void SetShield(int ShieldPoint)
        {
            slider.value = ShieldPoint;
            timeUntillBarHidden = 5;
        }

        public void SetCurrentShield(int currentShieldPoint)
        {
            slider.value = currentShieldPoint;
        }

        public void SetMaxShield(int maxShieldPoint)
        {
            if (slider == null)
            {
                Debug.LogError("Slider is null in SetMaxShield!");
                return;
            }

            slider.maxValue = maxShieldPoint;
            slider.value = maxShieldPoint;
        }

        private void Update()
        {
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.forward);
            }


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
