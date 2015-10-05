using UnityEngine;
using System.Collections;

public class DamageCS : Photon.MonoBehaviour {
	


	public float damage = 100f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	


	}



		public void OnCollisionEnter(Collision other) 
	
	{

	if (other.transform.tag == "Ship") {
			other.transform.GetComponent<PhotonView>().RPC ("TakeDamage", PhotonTargets.All, damage); 
		}

		}
	}



