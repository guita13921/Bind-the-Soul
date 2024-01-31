using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFadeWOChild : MonoBehaviour
{
    public float fadeSpeed;
    public float fadeAmount;
    float originalOpacity;
    [SerializeField] Material Mat;
    public bool DoFade;
    [SerializeField] public double timeRemaining;
    // Start is called before the first frame update
    void Start()
    {
        timeRemaining = 3;
        fadeSpeed = 10;
        fadeAmount = 0.1f;
        Mat = GetComponent<Renderer>().material;
        originalOpacity = Mat.color.a;
    }

    // Update is called once per frame
    void Update(){
    if (timeRemaining > 0){
            timeRemaining -= Time.deltaTime;
        }else{
            DoFade = false;
        }

        if(DoFade){
            FadeNow();
        }else{
            ResetFade();
        }
    }

    void FadeNow(){
        Color currentColor = Mat.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
            Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
        Mat.color = smoothColor;
    }

    void ResetFade(){
        Color currentColor = Mat.color;
        Color smoothColor = new Color(currentColor.r, currentColor.g, currentColor.b,
            Mathf.Lerp(currentColor.a, originalOpacity,fadeSpeed * Time.deltaTime));
        Mat.color = smoothColor;
    }
}
