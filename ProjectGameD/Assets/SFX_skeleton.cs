using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX_skeleton : MonoBehaviour
{
    [SerializeField] GameObject Skeleton_move;

    // Start is called before the first frame update
   

    public void Skeleton_Walk(){
        GameObject sfx = Instantiate(Skeleton_move);

    }
}
