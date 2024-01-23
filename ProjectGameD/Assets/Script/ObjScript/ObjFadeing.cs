using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFadeing : MonoBehaviour
{
    public float fadeSpeed, FadeAmount;
    float originalOpacity0;
    float originalOpacity1;
    float originalOpacity2;

    [SerializeField] Material material0;
    [SerializeField] Material material1;
    [SerializeField] Material material2;
    public bool DoFade = false;
    public GameObject objectToFind0;
    public GameObject objectToFind1;
    public GameObject objectToFind2;

    void Start()
    {
        objectToFind0 = transform.GetChild(0).gameObject;
        objectToFind1 = transform.GetChild(1).gameObject;
        objectToFind2 = transform.GetChild(2).gameObject;

        material0 = objectToFind0.GetComponent<Renderer>().material;
        material1 = objectToFind1.GetComponent<Renderer>().material;
        material2 = objectToFind2.GetComponent<Renderer>().material;

        originalOpacity0 = material0.color.a;
        originalOpacity1 = material1.color.a;
        originalOpacity2 = material2.color.a;
    }


    void Update()
    {
        if(DoFade){
            FadeNow();
        }else{
            ResetFade();
        }
    }

    void FadeNow(){
        Color currentColor0 = material0.color;
        Color smoothColor = new Color(currentColor0.r, currentColor0.g, currentColor0.b, 
            Mathf.Lerp(currentColor0.a, FadeAmount, fadeSpeed * Time.deltaTime));
        material0.color = smoothColor;

        Color currentColor1 = material1.color;
        Color smoothColor1 = new Color(currentColor1.r, currentColor1.g, currentColor1.b, 
            Mathf.Lerp(currentColor1.a, FadeAmount, fadeSpeed * Time.deltaTime));
        material1.color = smoothColor;

        Color currentColor2 = material2.color;
        Color smoothColor2 = new Color(currentColor2.r, currentColor2.g, currentColor2.b, 
            Mathf.Lerp(currentColor2.a, FadeAmount, fadeSpeed * Time.deltaTime));
        material2.color = smoothColor;
    }

    void ResetFade(){
        Color currentColor0 = material0.color;
        Color smoothColor = new Color(currentColor0.r, currentColor0.g, currentColor0.b, 
            Mathf.Lerp(currentColor0.a, originalOpacity0, fadeSpeed * Time.deltaTime));
        material0.color = smoothColor;

       Color currentColor1 = material1.color;
        Color smoothColor1 = new Color(currentColor1.r, currentColor1.g, currentColor1.b, 
            Mathf.Lerp(currentColor1.a, originalOpacity1, fadeSpeed * Time.deltaTime));
        material1.color = smoothColor;

        Color currentColor2 = material2.color;
        Color smoothColor2 = new Color(currentColor2.r, currentColor2.g, currentColor2.b, 
            Mathf.Lerp(currentColor2.a, originalOpacity2, fadeSpeed * Time.deltaTime));
        material2.color = smoothColor;
    }
}
