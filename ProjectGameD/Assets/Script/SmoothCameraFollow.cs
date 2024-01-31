using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    private Vector3 _offset;
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime;
    private Vector3 _currentVelocity = Vector3.zero;
    public ObjFadeing _fader;

    private void Awake() => _offset = transform.position - target.position;
    
    void Update(){
        Vector3 targetPosition = target.position + _offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref _currentVelocity,
            smoothTime
        );
        
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if(player != null){
            Vector3 dir = player.transform.position - transform.position;
            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit)){
                if(hit.collider == null)
                    return;
                if(hit.collider.gameObject == player){
                    if(_fader != null){
                    }
                }else{
                    _fader = hit.collider.gameObject.GetComponent<ObjFadeing>();
                    if(_fader != null){
                        Debug.Log("HIT");
                        _fader.timeRemaining = 0.1;
                        _fader.DoFade = true;
                    }
                }
        }
    }
}
}