using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerAttack : MonoBehaviour
    {

        AnimatorHander animatorHander;

        private void Awake()
        {
            animatorHander = GetComponentInChildren<AnimatorHander>();

            if (animatorHander == null)
            {
                Debug.LogError("❌ ไม่พบ AnimatorHander ใน " + gameObject.name);
            }
            else
            {
                Debug.Log("✅ พบ AnimatorHander ใน " + gameObject.name);
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
        }
    }

}
