using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjFadeing : MonoBehaviour
{
    public float fadeSpeed;
    public float FadeAmount;
    public bool DoFade;
    [SerializeField] public List<float> originalOpacity = new List<float>();
    [SerializeField] public List<Material> material = new List<Material>();
    [SerializeField] public List<GameObject> objectToFind = new List<GameObject>();
    int numberOfChild;
    [SerializeField] public double timeRemaining;

    void Start()
    {
        timeRemaining = 3;
        fadeSpeed = 10;
        FadeAmount = 0.1f;
        numberOfChild = transform.childCount;
        for (int i = 0; i < numberOfChild; i++) {
            objectToFind.Add(transform.GetChild(i).gameObject);
            material.Add(objectToFind[i].GetComponent<Renderer>().material);
            originalOpacity.Add(material[i].color.a);
        } 
    }


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
        /*
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
        */
        List<Color> currentColor = new List<Color>();
        List<Color> smoothColor = new List<Color>();

        for (int i = 0; i < numberOfChild; i++) {
            currentColor.Add(material[i].color);
            smoothColor.Add(new Color(currentColor[i].r, currentColor[i].g, currentColor[i].b, Mathf.Lerp(currentColor[i].a, FadeAmount, fadeSpeed * Time.deltaTime)));
            material[i].color = smoothColor[i];
        }
    }

    void ResetFade(){
        /*
        Color currentColor0 = material0.color;
        Color smoothColor = new Color(currentColor0.r, currentColor0.g, currentColor0.b, 
            Mathf.Lerp(currentColor0.a, originalOpacity0, fadeSpeed * Time.deltaTime));
        material0.color = smoothColor;

       Color currentColor1 = material1.color;
        Color smoothColor1 = new Color(currentColor1.r, currentColor1.g, currentColor1.b, 
            Mathf.Lerp(currentColor1.a, originalOpacity1, fadeSpeed * Time.deltaTime));
        material1.color = smoothColor1;

        Color currentColor2 = material2.color;
        Color smoothColor2 = new Color(currentColor2.r, currentColor2.g, currentColor2.b, 
            Mathf.Lerp(currentColor2.a, originalOpacity2, fadeSpeed * Time.deltaTime));
        material2.color = smoothColor2;
        */
        List<Color> currentColor = new List<Color>(numberOfChild);
        List<Color> smoothColor = new List<Color>(numberOfChild);
        for (int i = 0; i < numberOfChild; i++) 
        {
            currentColor.Add(material[i].color);
            smoothColor.Add(new Color(currentColor[i].r, currentColor[i].g, currentColor[i].b, 
                Mathf.Lerp(currentColor[i].a, originalOpacity[i], fadeSpeed * Time.deltaTime)));
            material[i].color = smoothColor[i];
        } 
    }
}
