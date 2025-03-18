using UnityEngine;

public class VoidChecker : MonoBehaviour
{
    private bool isInVoid = false;
    [SerializeField] SphereCollider sphereCollider;

    private void Start()
    {
        sphereCollider.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Void"))
        {
            isInVoid = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Void"))
        {
            isInVoid = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Void"))
        {
            isInVoid = false;
        }
    }

    public bool IsInVoid()
    {
        return isInVoid;
    }
}
