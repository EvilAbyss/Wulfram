using UnityEngine;
using System.Collections;

public class FireBullet : MonoBehaviour {
	
	public Transform socketProjectile;
	public GameObject projectile;
	public GameObject pewpew;
	private GameObject myProjectile;
	private GameObject mypewpew;
	public AudioClip GunShotSound;
	PhotonView pv;
	public float damage = 25f;
	
	public float timeBetweenShots = 1.0f;
	private float timestamp;
	
	float cooldown = 1;
	
	void Update () 
	{
		cooldown -= Time.deltaTime;
		pv = this.GetComponent<PhotonView>();
		//create projectile
		if (Time.time >= timestamp &&  (Input.GetButton("Fire1") && pv.isMine)) {
			if(cooldown > 0) {
				return;
			}
			GetComponent<AudioSource>().PlayOneShot (GunShotSound);
			
			myProjectile = (GameObject)PhotonNetwork.Instantiate (projectile.name, socketProjectile.position, socketProjectile.rotation, 0);
			timestamp = Time.time + timeBetweenShots;
			
			
			
		}
		
		
		
	}
	
	
}
