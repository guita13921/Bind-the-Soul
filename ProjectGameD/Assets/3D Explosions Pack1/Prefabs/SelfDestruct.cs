using UnityEngine;
using System.Collections;


public class SelfDestruct : MonoBehaviour {
	
	[SerializeField] public float selfdestruct_in;

	void Start () {
		if ( selfdestruct_in != 0){ 
			Destroy (gameObject, selfdestruct_in);
		}
	}
}
