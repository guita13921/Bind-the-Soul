using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerAttack : MonoBehaviour
    {

        AnimatorHander animatorHander;
        InputHander inputHander;
        public string lastAttack;
        public string lastAttack2;


        private void Awake()
        {
            animatorHander = GetComponentInChildren<AnimatorHander>();
            inputHander = GetComponent<InputHander>();

            if (animatorHander == null)
            {
                Debug.LogError("❌ ไม่พบ AnimatorHander ใน " + gameObject.name);
            }
            else
            {
                Debug.Log("✅ พบ AnimatorHander ใน " + gameObject.name);
            }
        }
        private IEnumerator HandleLightLastAttack()
        {
            yield return null;
            lastAttack = lastAttack2;

        }
        public void HandleWeaponCombo(WeaponItem weapon)
        {
            if (inputHander.comboflang)
            {
                animatorHander.anim.SetBool("CanDoCombo", false);
                if (lastAttack == weapon.OH_Light_Attack_1)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_2, true);
                    lastAttack2 = weapon.OH_Light_Attack_2;

                    StartCoroutine(HandleLightLastAttack());
                }

                if (lastAttack == weapon.OH_Light_Attack_2)
                {
                    animatorHander.anim.SetBool("CanDoCombo", false);
                    animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_3, true);
                }
            }

        }

        public void HandleLightAttack(WeaponItem weapon)
        {
            if (weapon == null)
            {
                Debug.LogError("❌ Weapon เป็นค่า null ใน HandleLightAttack!");
                return;
            }

            if (animatorHander == null)
            {
                Debug.LogError("❌ AnimatorHander เป็นค่า null ใน PlayerAttack!");
                return;
            }

            animatorHander.PlayTargetAnimation(weapon.OH_Light_Attack_1, true);
            lastAttack = weapon.OH_Light_Attack_1;
        }

        public void HandleHeavyAttack(WeaponItem weapon)
        {
            if (weapon == null)
            {
                Debug.LogError("❌ Weapon เป็นค่า null ใน HandleLightAttack!");
                return;
            }

            if (animatorHander == null)
            {
                Debug.LogError("❌ AnimatorHander เป็นค่า null ใน PlayerAttack!");
                return;
            }
            animatorHander.PlayTargetAnimation(weapon.OH_Heavy_Attack_1, true);
            lastAttack = weapon.OH_Heavy_Attack_1;
        }
    }

}
