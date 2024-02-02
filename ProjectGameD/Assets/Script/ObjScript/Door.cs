using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public bool IsOpen = false;
    [SerializeField] private bool IsRotatingDoor = true;
    [SerializeField] private float speed = 1f;
    [SerializeField][Header("Rotation Config")] private float RotaionAmount = 90f;
    [SerializeField] private float ForwardDirection = 0;

    private Vector3 StartRoation;
    private Vector3 Forward;

    private Coroutine AnimationCoroutine;

    private void Awake(){
        StartRoation = transform.rotation.eulerAngles;
        Forward = transform.right;
    }

    public void Open(Vector3 UserPosiotion){
        if(!IsOpen){
            if(AnimationCoroutine != null){
                StopCoroutine(AnimationCoroutine);
            }
            if(IsRotatingDoor){
                float dot = Vector3.Dot(Forward, (UserPosiotion - transform.position).normalized);
                Debug.Log($"Dot: {dot.ToString("N3")}");
                AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
            }
        }
    }

    private IEnumerator DoRotationOpen(float ForwardAmount){
        Quaternion StartRoation = transform.rotation;
        Quaternion endRotation;

        if(ForwardAmount >= ForwardDirection){
            endRotation = Quaternion.Euler(new Vector3(0, StartRoation.y + RotaionAmount, 0));
        }else{
            endRotation = Quaternion.Euler(new Vector3(0, StartRoation.y - RotaionAmount, 0));
        }

        IsOpen = true;

        float time = 0;
        while(time <1){
            transform.rotation = Quaternion.Slerp(StartRoation, endRotation, time);
            yield return null;
            time += Time.deltaTime * speed;
        }

    }
}
