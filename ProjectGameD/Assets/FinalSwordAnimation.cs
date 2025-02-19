using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinalSwordAnimation : MonoBehaviour
{
    public Animator animator;
    public GameObject sword;
    public GameObject player;
    bool startmove = false;
    public float moveSpeed = 2;
    public AudioSource audioSource;
    public Animator cam;
    public AudioSource run;
    public AudioSource lightS;
    public float fadeDuration = 3f; // Time to fade out sound

    void Start() { }

    public void Swrod()
    {
        //sword.transform.position = new Vector3( sword.transform.position.x, 1.926469f,  sword.transform.position.z);
        //sword.transform.rotation = Quaternion.Euler(98.22501f, 18.675f, 117.289f);
        Rigidbody rb = sword.GetComponent<Rigidbody>();

        rb.constraints = RigidbodyConstraints.None; // Unfreeze all constraints
        Collider col = sword.GetComponent<Collider>();
        col.enabled = true; // Activate collider

        audioSource.Play();
    }

    public void Run()
    {
        //   animator.Play("FinalRun");
    }

    public void Run2()
    {
        animator.Play("FinalRun2");
        startmove = true;
        run.Play();
        lightS.Play();
        StartCoroutine(FadeOutRunSound());
    }

    void Update()
    {
        if (startmove && player != null)
        {
            player.transform.position += player.transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    private IEnumerator FadeOutRunSound()
    {
        float startVolume = run.volume;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            run.volume = Mathf.Lerp(startVolume, 0, t / fadeDuration);
            yield return null;
        }

        run.volume = 0;
        run.Stop();
    }
}
