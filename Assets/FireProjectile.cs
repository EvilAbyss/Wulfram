using UnityEngine;
using System.Collections;

public class FireProjectile : ProjectileBase {
	
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
				pv = this.GetComponent<PhotonView> ();
				//create projectile
				if (Time.time >= timestamp && (Input.GetButton ("Fire2") && pv.isMine)) {
						if (cooldown > 0) {
								return;
						}
						GetComponent<AudioSource>().PlayOneShot (GunShotSound);

						myProjectile = (GameObject)PhotonNetwork.Instantiate (projectile.name, socketProjectile.position, socketProjectile.rotation, 0);
						timestamp = Time.time + timeBetweenShots;

	
				}
		}


		public void OnCollisionEnter( Collision collision )
		{
			if( collision.collider.tag == "Obstacle" )
			{
				OnProjectileHit();
			}
			else if( collision.collider.tag == "Ship")
			{
				
				Ship ship = collision.collider.GetComponent<Ship>();

				if( ship.PhotonView.isMine == false )
				{
				ship.SendRespawn();
				Debug.Log("Respawned Player");
					return;
				}
				
				if( m_Owner.Team == Team.None || ship.Team != m_Owner.Team )
				{
					ship.ShipCollision.OnProjectileHit( this );
					OnProjectileHit();
					m_Owner.ShipShooting.SendProjectileHit( m_ProjectileId );
				}
			}

	}}



		
		
		
		
	
