using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{

    public class UIEnemyShieldBar : MonoBehaviour
    {
        [SerializeField] private Slider sliderFill;
        [SerializeField] private Slider sliderRegen;
        float timeUntillBarHidden = 0;
        public Transform mainCamera;


        public void SetShield(int ShieldPoint)
        {
            sliderFill.value = ShieldPoint;
            timeUntillBarHidden = 5;

            sliderRegen.value = ShieldPoint;
        }

        public void SetCurrentShield(int currentShieldPoint)
        {
            sliderFill.value = currentShieldPoint;
        }

        public void SetMaxShield(int maxShieldPoint)
        {
            if (sliderFill == null)
            {
                Debug.LogError("Slider is null in SetMaxShield!");
                return;
            }

            sliderFill.maxValue = maxShieldPoint;
            sliderFill.value = maxShieldPoint;
        }

        public void SetCurrentShieldRegen(int currentShieldPoint)
        {
            sliderRegen.value = currentShieldPoint;
        }

        public void SetMaxShieldRegen(int maxShieldPoint)
        {
            if (sliderRegen == null)
            {
                Debug.LogError("Slider is null in SetMaxShield!");
                return;
            }

            sliderRegen.maxValue = maxShieldPoint;
            sliderRegen.value = maxShieldPoint;
        }

        private void Update()
        {
            if (mainCamera != null)
            {
                transform.LookAt(transform.position + mainCamera.forward);
            }


            timeUntillBarHidden -= Time.deltaTime;

            if (sliderFill != null)
            {
                if (timeUntillBarHidden <= 0)
                {
                    timeUntillBarHidden = 0;
                    //sliderFill.gameObject.SetActive(false);
                }
                else
                {
                    if (!sliderFill.gameObject.activeInHierarchy)
                    {
                        sliderFill.gameObject.SetActive(true);
                    }
                }

                if (sliderFill.value <= 0)
                {
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
