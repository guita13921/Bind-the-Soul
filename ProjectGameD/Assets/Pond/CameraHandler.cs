using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

namespace SG
{
    public class CameraHandler : MonoBehaviour
    {
        InputHander inputHander;
        PlayerManager playerManager;
        public Transform targetTransform;
        public Transform cameraTransform; //Move Camera
        public Transform cameraPivotTranform; //Rotate
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;
        public LayerMask envirometLayer;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        [SerializeField] private float rotationSpeed = 8f; // Adjust as needed

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float lockedPivotPosition = 1.65f;
        public float unlockedPivotPosition = 1.65f;

        public CharacterManager currentLockOnTarget;
        List<CharacterManager> avilableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;
        public float maximumLockOnDistance = 30;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            ignoreLayers = ~(1 << 8 | 1 << 9 | 1 << 10);
            inputHander = FindObjectOfType<InputHander>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            envirometLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            transform.position = Vector3.Lerp(transform.position, targetPosition, delta / 0.1f); // smooth camera movement
        }


        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (inputHander.lockOnFlag && currentLockOnTarget != null)
            {
                Vector3 direction = currentLockOnTarget.transform.position - transform.position;
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
            }
            else
            {
                // Free rotation (non-lock-on)
                lookAngle += (mouseXInput * lookSpeed) / delta;
                pivotAngle -= (mouseYInput * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * delta);
            }
        }

        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            avilableTargets.Clear();

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                CharacterStats characterStats = colliders[i].GetComponent<CharacterStats>();

                if (character != null && characterStats != null && !characterStats.isDead)
                {
                    Vector3 lockTargetDirection = character.transform.position - targetTransform.position;
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                    float viewableAngle = Vector3.Angle(lockTargetDirection, cameraTransform.forward);

                    if (character.transform.root != targetTransform.transform.root
                        && viewableAngle > -50 && viewableAngle < 50
                        && distanceFromTarget <= maximumLockOnDistance)
                    {
                        if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out RaycastHit hit))
                        {
                            if (hit.transform.gameObject.layer != envirometLayer)
                            {
                                avilableTargets.Add(character);
                            }
                        }
                    }
                }
            }

            for (int k = 0; k < avilableTargets.Count; k++)
            {
                var target = avilableTargets[k];
                if (target == null || target.transform == null)
                    continue;

                float distanceFromTarget = Vector3.Distance(targetTransform.position, target.transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = target;
                }

                if (inputHander.lockOnFlag && currentLockOnTarget != null)
                {
                    //Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(target.transform.position);
                    //float distanceFromLeftTarget = currentLockOnTarget.transform.position.x - target.transform.position.x;
                    //float distanceFromRightTarget = currentLockOnTarget.transform.position.x + target.transform.position.x;

                    Vector3 relativeEnemyPosition = inputHander.transform.InverseTransformPoint(avilableTargets[k].transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.y;

                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget
                        && avilableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = avilableTargets[k];
                    }

                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget
                        && avilableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfRightTarget = distanceFromRightTarget;
                        rightLockTarget = avilableTargets[k];
                    }
                }
            }


            // 🔁 Handle auto-switch if current target is dead or missin
            if (inputHander.lockOnFlag)
            {
                if (currentLockOnTarget == null)
                {
                    currentLockOnTarget = nearestLockOnTarget;

                    if (nearestLockOnTarget == null)
                    {
                        inputHander.lockOnFlag = false;
                    }
                }
            }
        }

        public void ClearLockOnTargets()
        {
            avilableTargets.Clear();
            nearestLockOnTarget = null;
            currentLockOnTarget = null;
        }

        public void SetCameraHeight()
        {
            Vector3 velocity = Vector3.zero;
            Vector3 newLockedposition = new Vector3(0, lockedPivotPosition);
            Vector3 newUnlockedPosition = new Vector3(0, unlockedPivotPosition);

            if (currentLockOnTarget != null)
            {
                cameraPivotTranform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTranform.transform.localPosition, newLockedposition, ref velocity, Time.deltaTime);
            }
            else
            {
                cameraPivotTranform.transform.localPosition = Vector3.SmoothDamp(cameraPivotTranform.transform.localPosition, newUnlockedPosition, ref velocity, Time.deltaTime);
            }
        }

        public void SetDefaultTransform()
        {
            playerManager.lockOnTransform.position = playerManager.DefaultlockOnTransform.position;
        }

    }

}