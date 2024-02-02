using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerAction : MonoBehaviour
{
    [SerializeField] private TextMeshPro UseText;
    [SerializeField] private Transform Camera;
    [SerializeField] float MaxUseDistance = 5f;

    public void OnUse(){
        if(Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit,MaxUseDistance)){
            if(hit.collider.TryGetComponent<Door>(out Door door)){
                if(door.IsOpen){
                }else{
                    door.Open(transform.position);
                }
            }
        }
    }


    private void Update(){
        if (Input.GetKeyDown("e")){
            OnUse();
            Debug.Log("e key was pressed");
        }
        
        if(Physics.Raycast(Camera.position, Camera.forward, out RaycastHit hit, MaxUseDistance)
            && hit.collider.TryGetComponent<Door>(out Door door)){
                if(door.IsOpen){
                }else{
                    UseText.SetText("Open \"E\"");
                }
                UseText.gameObject.SetActive(true);
                UseText.transform.position = hit.point - (hit.point - Camera.position).normalized * 0.01f;
                UseText.transform.rotation = Quaternion.LookRotation((hit.point = Camera.position).normalized);

        }else{
            UseText.gameObject.SetActive(false);
        }
    }

}
