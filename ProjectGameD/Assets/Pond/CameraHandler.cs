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
        public Transform cameraTransform;
        public Transform cameraPivotTranform;
        private Transform myTransform;
        private Vector3 cameraTransformPosition;
        public LayerMask ignoreLayers;
        public LayerMask envirometLayer;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;

        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float lockedPivotPosition = 2.25f;
        public float unlockedPivotPosition = 1.65f;

        public Transform currentLockOnTarget;
        List<CharacterManager> avilableTargets = new List<CharacterManager>();
        public Transform nearestLockOnTarget;
        public Transform leftLockTarget;
        public Transform rightLockTarget;
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
            myTransform.position = targetPosition;
        }
        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYIput)
        {
            if (inputHander.lockOnFlag == false && currentLockOnTarget == null)
            {
                lookAngle += (mouseXInput * lookSpeed) / delta;
                pivotAngle -= (mouseYIput * pivotSpeed) / delta;
                pivotAngle = Mathf.Clamp(pivotAngle, minimumPivot, maximumPivot);

                Vector3 rotation = Vector3.zero;
                rotation.y = lookAngle;
                Quaternion targetRotation = Quaternion.Euler(rotation);
                myTransform.rotation = targetRotation;

                rotation = Vector3.zero;
                rotation.x = pivotAngle;

                targetRotation = Quaternion.Euler(rotation);
                cameraPivotTranform.localRotation = targetRotation;
            }
            else
            {
                float velocity = 0;
                Vector3 dir = currentLockOnTarget.position - transform.position;
                dir.Normalize();
                dir.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(dir);
                transform.rotation = targetRotation;

                dir = currentLockOnTarget.position - cameraPivotTranform.position;
                dir.Normalize();

                targetRotation = Quaternion.LookRotation(dir);
                Vector3 eulerAngle = targetRotation.eulerAngles;
                eulerAngle.y = 0;
                cameraPivotTranform.localEulerAngles = eulerAngle;
            }

        }
        public void HandleLockOn()
        {
            float shortestDistance = Mathf.Infinity;
            float shortestDistanceOfLeftTarget = Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);
            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                if (character != null)
                {
                    {
                        Vector3 lockTragetDirection = character.transform.position - targetTransform.position;
                        float distanceFromTarget = Vector3.Distance(targetTransform.position, character.transform.position);
                        float viewAbleAngle = Vector3.Angle(lockTragetDirection, cameraTransform.forward);
                        RaycastHit hit;

                        if (character.transform.root != targetTransform.transform.root
                        && viewAbleAngle > -50 && viewAbleAngle < 50
                        && distanceFromTarget <= maximumLockOnDistance)
                        {
                            if (Physics.Linecast(playerManager.lockOnTransform.position, character.lockOnTransform.position, out hit))
                            {
                                Debug.DrawLine(playerManager.lockOnTransform.position, character.lockOnTransform.position);
                                if (hit.transform.gameObject.layer == envirometLayer)
                                {
                                    //Cannot lock onto target, object in the way
                                }
                                else
                                {
                                    avilableTargets.Add(character);
                                }
                            }

                        }
                    }
                }
                for (int k = 0; k < avilableTargets.Count; i++)
                {
                    float distanceFromTarget = Vector3.Distance(targetTransform.position, avilableTargets[k].transform.position);
                    if (distanceFromTarget < shortestDistance)
                    {
                        shortestDistance = distanceFromTarget;
                        nearestLockOnTarget = avilableTargets[k].lockOnTransform;
                    }
                    if (inputHander.lockOnFlag)
                    {
                        Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(avilableTargets[k].transform.position);
                        var distanceFromLeftTarget = currentLockOnTarget.transform.position.x - avilableTargets[k].transform.position.x;
                        var distanceFromRightTarget = currentLockOnTarget.transform.position.x + avilableTargets[k].transform.position.x;
                        if (relativeEnemyPosition.x > 0.00 && distanceFromLeftTarget < shortestDistanceOfLeftTarget)
                        {
                            shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                            leftLockTarget = avilableTargets[k].lockOnTransform;
                        }
                        if (relativeEnemyPosition.x < 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget)
                        {
                            shortestDistanceOfRightTarget = distanceFromRightTarget;
                            rightLockTarget = avilableTargets[k].lockOnTransform;
                        }
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
    }

}
