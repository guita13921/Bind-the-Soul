using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
public class sfx_controller : MonoBehaviour
{
    [SerializeField]public AudioSource audiosource;
    [SerializeField]public AudioClip walk;
    [SerializeField]RaycastHit hit;
    [SerializeField]public float range;
    [SerializeField]public LayerMask layerMask;
    // Start is called before the first frame update
    public void Footstep(){
        PlayFootstepSoundL(walk);
    }

    void PlayFootstepSoundL(AudioClip audio){
        audiosource.PlayOneShot(audio);
    }
}
