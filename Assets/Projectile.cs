
using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
	float projectileSpeed = 10;

	float minY = -5.0f;

	public GameObject HitFX;
	


		void Update () 
	{

		transform.Translate (0,0, projectileSpeed * Time.deltaTime);
	}
	
	void OnTriggerEnter (Collider other)
	{
		if (other.gameObject.tag == "Terrain")
		{
			PhotonNetwork.Destroy (gameObject);
			Instantiate (HitFX, transform.position, transform.rotation);
		}
		
		if (other.gameObject.tag == "Ship")
		{
			PhotonNetwork.Destroy (gameObject);
			Instantiate (HitFX, transform.position, transform.rotation);
		}
	}


}
