using UnityEngine;
using System.Collections;

public class health : Photon.MonoBehaviour {

	public float hitPoints = 1000f;
	float currentHitPoints;
	
	// Use this for initialization
	void Start () {
		currentHitPoints = hitPoints;
	}
	
	[RPC]
	public void TakeDamage(float amt) {
		currentHitPoints -= amt;
		
		if(currentHitPoints <= 0) {
			Die();
		}
	}
	
	void OnGUI() {
		if( GetComponent<PhotonView>().isMine && gameObject.tag == "Ship" ) {
			if( GUI.Button(new Rect (Screen.width-100, 0, 100, 40), "Suicide!") ) {

			
					Die ();
			}
		}
	}
	
	void Die() {
		if( GetComponent<PhotonView>().instantiationId==0 ) {
			Destroy(gameObject);
		}
		else {
			if( GetComponent<PhotonView>().isMine ) {
				if( gameObject.tag == "Ship" ) {		// This is my actual PLAYER object, then initiate the respawn process
					Ship.LocalPlayer.SendRespawn();
				}
				
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
