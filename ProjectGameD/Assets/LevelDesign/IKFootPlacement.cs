using UnityEngine;

namespace SG
{

    [RequireComponent(typeof(Animator))]
    public class IKFootPlacement : MonoBehaviour
    {
        [Header("Feet IK Settings")]
        public LayerMask groundLayer;
        public float raycastDistance = 1.5f;
        public float footOffset = 0.1f;
        public float footRotationSpeed = 5f;

        private Animator animator;

        private Vector3 leftFootIKPos, rightFootIKPos;
        private Quaternion leftFootIKRot, rightFootIKRot;
        private float leftFootWeight, rightFootWeight;

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        private void OnAnimatorIK(int layerIndex)
        {
            if (animator == null) return;

            leftFootWeight = animator.GetFloat("IKLeftFootWeight");
            rightFootWeight = animator.GetFloat("IKRightFootWeight");

            // Process each foot
            AdjustFootTarget(AvatarIKGoal.LeftFoot, ref leftFootIKPos, ref leftFootIKRot);
            AdjustFootTarget(AvatarIKGoal.RightFoot, ref rightFootIKPos, ref rightFootIKRot);

            MoveFootToIKPoint(AvatarIKGoal.LeftFoot, leftFootIKPos, leftFootIKRot, leftFootWeight);
            MoveFootToIKPoint(AvatarIKGoal.RightFoot, rightFootIKPos, rightFootIKRot, rightFootWeight);
        }

        private void AdjustFootTarget(AvatarIKGoal foot, ref Vector3 footPos, ref Quaternion footRot)
        {
            Vector3 footPosition = animator.GetIKPosition(foot);
            Ray ray = new Ray(footPosition + Vector3.up, Vector3.down);

            if (Physics.Raycast(ray, out RaycastHit hit, raycastDistance, groundLayer))
            {
                footPos = hit.point + Vector3.up * footOffset;
                footRot = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
            }
        }

        private void MoveFootToIKPoint(AvatarIKGoal foot, Vector3 pos, Quaternion rot, float weight)
        {
            animator.SetIKPositionWeight(foot, weight);
            animator.SetIKRotationWeight(foot, weight);

            animator.SetIKPosition(foot, pos);
            animator.SetIKRotation(foot, rot);
        }
    }
}