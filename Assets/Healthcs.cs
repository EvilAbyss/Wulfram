using UnityEngine;
using System.Collections;

public class Healthcs : MonoBehaviour {
	
	public float hitPoints = 100f;
	float currentHitPoints;
	
	// Use this for initialization
	void Start () {
		currentHitPoints = hitPoints;
	}


	void OnTriggerEnter(Collider other){
		if (other.tag == "Projectile") {
			Die ();

		}

		}






	void Die() {
		if( GetComponent<PhotonView>().instantiationId==0 ) {
			Destroy(gameObject);
		}
		else {
			if( PhotonNetwork.isMasterClient ) {
				PhotonNetwork.Destroy(gameObject);

			}
		}
	}
}
