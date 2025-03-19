using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyObject : MonoBehaviour
{
    [SerializeField]
    private EventSystem eventSystem;

    void Start()
    {
        // Automatically find the EventSystem in the scene if it's not already assigned
        if (eventSystem == null)
        {
            eventSystem = FindObjectOfType<EventSystem>();
        }
    }

    public void DestroyGameObject()
    {        if (eventSystem != null)
        {
            eventSystem.enabled = true;
        }
        Destroy(gameObject);

    }
}
