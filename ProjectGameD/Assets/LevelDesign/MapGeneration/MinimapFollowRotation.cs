using UnityEngine;

namespace SG
{

    using UnityEngine;

    public class MinimapFollowRotation : MonoBehaviour
    {
        [SerializeField] private Transform playerCamera;  // TPS camera (third-person)

        void Awake()
        {
            // Find the camera if not set in the inspector
            if (playerCamera == null)
                playerCamera = FindAnyObjectByType<CameraHandler>().transform;
        }

        void LateUpdate()
        {
            // Rotate the minimap UI around its center to match the player camera's Y rotation
            transform.rotation = Quaternion.Euler(0f, 0f, playerCamera.eulerAngles.y);
        }
    }


}