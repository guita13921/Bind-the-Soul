using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DestroyObject : MonoBehaviour
{
    [SerializeField]
    EventSystem eventSystem;

    public void DestroyGameObject()
    {
        Destroy(gameObject);
        eventSystem.enabled = true;
    }
}
