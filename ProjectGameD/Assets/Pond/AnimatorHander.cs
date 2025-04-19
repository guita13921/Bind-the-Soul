using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering.Universal;

namespace SG
{
    public class AnimatorHander : MonoBehaviour
    {
        [SerializeField] private PlayerManager playerManager;
        public Animator anim;
        private InputHander inputHander;
        private PlayerLocomotion playerLocomotion;
        private int vertical;
        private int horizontal;
        public bool canRotate;
        PlayerSoundManager playerSoundManager;


        void Awake()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            playerSoundManager = GetComponentInParent<PlayerSoundManager>();
        }

        public void Initialize()
        {
            playerManager = GetComponentInParent<PlayerManager>();
            anim = GetComponent<Animator>();
            inputHander = GetComponentInParent<InputHander>(); // แก้ไขชื่อคลาส
            playerLocomotion = GetComponentInParent<PlayerLocomotion>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");

            // ตรวจสอบว่า Animator มีอยู่หรือไม่
            if (anim == null)
            {
                Debug.LogError("❌ Animator ไม่พบใน " + gameObject.name);
            }
        }

        public void UpdateAnimatorValues(float verticalMovement, float horizontalMovement, bool isSprinting)
        {
            if (anim == null) return; // ป้องกันข้อผิดพลาด

            float v = verticalMovement > 0.55f ? 1 : verticalMovement > 0 ? 0.5f : verticalMovement < -0.55f ? -1 : verticalMovement < 0 ? -0.5f : 0;
            float h = horizontalMovement > 0.55f ? 1 : horizontalMovement > 0 ? 0.5f : horizontalMovement < -0.55f ? -1 : horizontalMovement < 0 ? -0.5f : 0;

            if (isSprinting)
            {
                v = 2;
                h = horizontalMovement;
            }

            anim.SetFloat(vertical, v, 0.1f, Time.deltaTime);
            anim.SetFloat(horizontal, h, 0.1f, Time.deltaTime);
        }

        public void PlayTargetAnimation(string targetAnim, bool isInteracting, bool canRotate = false)
        {
            if (anim == null) return;

            anim.applyRootMotion = isInteracting;
            //anim.SetBool("canRotate", canRotate);
            anim.SetBool("isInteracting", isInteracting);
            anim.CrossFade(targetAnim, 0.0f);
        }

        public void CanRotate() => canRotate = true;
        public void StopRotation() => canRotate = false;
        public void EnableCombo()
        {
            anim.SetBool("CanDoCombo", true);
        }

        public void DisableCombo()
        {
            anim.SetBool("CanDoCombo", false);
        }

        public void EnableIsInvulnerable()
        {
            anim.SetBool("IsInvulnerable", true);
        }

        public void DisableIsnvulnerable()
        {
            anim.SetBool("IsInvulnerable", false);
        }

        public void EnableIsParrying()
        {
            playerManager.isParrying = true;
        }

        public void DusableIsParrying()
        {
            playerManager.isParrying = false;
        }

        private void OnAnimatorMove()
        {
            if (playerManager == null || playerManager.isInteracting == false) return;
            if (playerLocomotion == null)
            {
                Debug.LogWarning("⚠️ PlayerLocomotion เป็นค่า null ใน AnimatorHandler");
                return;
            }

            if (playerLocomotion.TryGetComponent(out Rigidbody rb))
            {
                float delta = Time.deltaTime;
                rb.drag = 0;
                Vector3 deltaPosition = anim.deltaPosition;
                deltaPosition.y = 0;
                Vector3 velocity = deltaPosition / delta;
                rb.velocity = velocity;
            }
            else
            {
                Debug.LogWarning("⚠️ Rigidbody ไม่พบใน PlayerLocomotion");
            }
        }
    }
}
