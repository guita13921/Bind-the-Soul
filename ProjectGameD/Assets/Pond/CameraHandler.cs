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
        private Vector3 cameraFollowVelocity = Vector3.zero;
        public LayerMask envirometLayer;

        public static CameraHandler singleton;

        public float lookSpeed = 0.1f;
        public float followSpeed = 0.1f;
        public float pivotSpeed = 0.03f;
        [SerializeField] private float rotationSpeed = 8f; // Adjust as needed

        private float targetPosition;
        private float defaultPosition;
        private float lookAngle;
        private float pivotAngle;
        public float minimumPivot = -35;
        public float maximumPivot = 35;

        public float cameraSphereRadius = 0.2f;
        public float cameraCollisionOffSet = 0.2f;
        public float minimumCollisionOffset = 0.2f;

        public float lockedPivotPosition = 1.65f;
        public float unlockedPivotPosition = 1.65f;

        public CharacterManager currentLockOnTarget;
        List<CharacterManager> avilableTargets = new List<CharacterManager>();
        public CharacterManager nearestLockOnTarget;
        public CharacterManager leftLockTarget;
        public CharacterManager rightLockTarget;
        public float maximumLockOnDistance = 30;

        //Camera Shake
        [SerializeField] public float hitStopDuration = 0.05f; // Duration of the freeze
        [SerializeField] public float hitStopTimeScale = 0f;    // Freeze time (0 = full stop)
        private bool isHitStopping = false;

        private void Awake()
        {
            singleton = this;
            myTransform = transform;
            defaultPosition = cameraTransform.localPosition.z;
            inputHander = FindObjectOfType<InputHander>();
            playerManager = FindObjectOfType<PlayerManager>();
        }

        private void Start()
        {
            //envirometLayer = LayerMask.NameToLayer("Environment");
        }

        public void FollowTarget(float delta)
        {
            Vector3 targetPosition = Vector3.Lerp(myTransform.position, targetTransform.position, delta / followSpeed);
            transform.position = Vector3.Lerp(transform.position, targetPosition, delta / 0.1f); // smooth camera movement

            HandleCameraCollisions(delta);
        }

        private void HandleCameraCollisions(float delta)
        {
            Vector3 desiredPosition = new Vector3(0, 0, defaultPosition); // Desired local Z offset
            Vector3 direction = cameraTransform.position - cameraPivotTranform.position;
            direction.Normalize();

            RaycastHit hit;
            float maxDistance = Mathf.Abs(defaultPosition);

            // Do the SphereCast to detect potential collisions
            if (Physics.SphereCast(cameraPivotTranform.position, cameraSphereRadius, direction, out hit, maxDistance, ignoreLayers))
            {
                float hitDistance = Vector3.Distance(cameraPivotTranform.position, hit.point);
                float adjustedDistance = Mathf.Clamp(hitDistance - cameraCollisionOffSet, minimumCollisionOffset, maxDistance);
                desiredPosition.z = -adjustedDistance;
            }

            // Smoothly interpolate the camera position
            Vector3 currentLocalPos = cameraTransform.localPosition;
            currentLocalPos.z = Mathf.Lerp(currentLocalPos.z, desiredPosition.z, delta / 0.2f);
            cameraTransform.localPosition = currentLocalPos;
        }

        public void HandleCameraRotation(float delta, float mouseXInput, float mouseYInput)
        {
            if (inputHander.lockOnFlag && currentLockOnTarget != null)
            {
                Vector3 direction = currentLockOnTarget.transform.position - transform.position;
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
            float shortestDistanceOfLeftTarget = -Mathf.Infinity;
            float shortestDistanceOfRightTarget = Mathf.Infinity;
            avilableTargets.Clear();

            Collider[] colliders = Physics.OverlapSphere(targetTransform.position, 26);

            for (int i = 0; i < colliders.Length; i++)
            {
                CharacterManager character = colliders[i].GetComponent<CharacterManager>();
                CharacterStats characterStats = colliders[i].GetComponent<CharacterStats>();

                // Basic null and death checks
                if (character == null || characterStats == null || characterStats.isDead)
                    continue;

                // Extra safety checks to avoid missing reference errors
                if (character.transform == null || character.lockOnTransform == null)
                    continue;

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
                if (target == null || target.transform == null || !target.gameObject.activeInHierarchy)
                    continue;

                float distanceFromTarget = Vector3.Distance(targetTransform.position, target.transform.position);

                if (distanceFromTarget < shortestDistance)
                {
                    shortestDistance = distanceFromTarget;
                    nearestLockOnTarget = target;
                    nearestLockOnTarget = target;
                }

                if (inputHander.lockOnFlag && currentLockOnTarget != null)
                {
                    //Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(target.transform.position);
                    //float distanceFromLeftTarget = currentLockOnTarget.transform.position.x - target.transform.position.x;
                    //float distanceFromRightTarget = currentLockOnTarget.transform.position.x + target.transform.position.x;
                    //Vector3 relativeEnemyPosition = currentLockOnTarget.InverseTransformPoint(target.transform.position);
                    //float distanceFromLeftTarget = currentLockOnTarget.transform.position.x - target.transform.position.x;
                    //float distanceFromRightTarget = currentLockOnTarget.transform.position.x + target.transform.position.x;

                    Vector3 relativeEnemyPosition = inputHander.transform.InverseTransformPoint(avilableTargets[k].transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.y;

                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget
                        && avilableTargets[k] != currentLockOnTarget)
                        Vector3 relativeEnemyPosition = inputHander.transform.InverseTransformPoint(avilableTargets[k].transform.position);
                    float distanceFromLeftTarget = relativeEnemyPosition.x;
                    float distanceFromRightTarget = relativeEnemyPosition.y;

                    if (relativeEnemyPosition.x <= 0.00 && distanceFromLeftTarget > shortestDistanceOfLeftTarget
                        && avilableTargets[k] != currentLockOnTarget)
                    {
                        shortestDistanceOfLeftTarget = distanceFromLeftTarget;
                        leftLockTarget = avilableTargets[k];
                        leftLockTarget = avilableTargets[k];
                    }

                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget
                        && avilableTargets[k] != currentLockOnTarget)
                    else if (relativeEnemyPosition.x >= 0.00 && distanceFromRightTarget < shortestDistanceOfRightTarget
                        && avilableTargets[k] != currentLockOnTarget)
                            {
                                shortestDistanceOfRightTarget = distanceFromRightTarget;
                                rightLockTarget = avilableTargets[k];
                                rightLockTarget = avilableTargets[k];
                            }
                }
            }


            // 🔁 Handle auto-switch if current target is dead or missin

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

        public void Shake(float duration, float magnitude)
        {
            StartCoroutine(ShakeCoroutine(duration, magnitude));
        }

        public IEnumerator ShakeCoroutine(float duration, float magnitude)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                float x = UnityEngine.Random.Range(-1f, 1f) * magnitude;
                float y = UnityEngine.Random.Range(-1f, 1f) * magnitude;

                transform.localPosition = this.transform.position + new Vector3(x, y, 0f);

                elapsed += Time.unscaledDeltaTime;
                yield return null;
            }

            transform.localPosition = this.transform.position;
        }

        public IEnumerator HitStopCoroutine(float duration)
        {
            if (isHitStopping) yield break; // Prevent overlapping hitstops

            isHitStopping = true;

            float originalTimeScale = Time.timeScale;
            Time.timeScale = hitStopTimeScale;
            float timer = 0f;

            // Ensure Time.unscaledDeltaTime is used during hitstop
            while (timer < duration)
            {
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            Time.timeScale = originalTimeScale;
            isHitStopping = false;
        }
    }


}