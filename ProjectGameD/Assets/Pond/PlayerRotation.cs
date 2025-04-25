using UnityEngine;
using UnityEngine.InputSystem; // ใช้ New Input System

public class PlayerRotation : MonoBehaviour
{
    public LayerMask groundLayer; // Layer สำหรับพื้น

    void Update()
    {
        RotateTowardsMouse();
    }

    void RotateTowardsMouse()
    {
        // ใช้ New Input System อ่านตำแหน่งเม้าส์
        Vector2 mouseScreenPosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mouseScreenPosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = transform.position.y; // ล็อกแกน Y

            Vector3 direction = (targetPosition - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
        }
    }
}
