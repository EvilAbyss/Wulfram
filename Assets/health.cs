using UnityEngine;
using System.Collections;

public class health : MonoBehaviour {

	public float hitPoints = 100f;
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
	
	/*void OnGUI() {
		if( GetComponent<PhotonView>().isMine && gameObject.tag == "Player" ) {
			if( GUI.Button(new Rect (Screen.width-100, 0, 100, 40), "Suicide!") ) {
				Die ();
			}
		}
	}*/
	
	void Die() {
		if( GetComponent<PhotonView>().instantiationId==0 ) {
			Destroy(gameObject);
		}
		else {
			if( GetComponent<PhotonView>().isMine ) {
				if( gameObject.tag == "Player" ) {		// This is my actual PLAYER object, then initiate the respawn process
		//test
				}
				
				PhotonNetwork.Destroy(gameObject);
			}
		}
	}
}
