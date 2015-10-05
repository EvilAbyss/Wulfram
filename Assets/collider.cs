using UnityEngine;
using System.Collections;

public class collider : Photon.MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		
		

	}
	
	
	
	// Update is called once per frame
	void Update () {
		
		
	


		
	}



	void OnTriggerEnter(Collider other) {
		
		this.gameObject.transform.parent = other.gameObject.transform;
		this.gameObject.transform.Translate (0,0.5F,0);
	}
}