using UnityEngine;
using System.Collections;

public class ExampleBullet : MonoBehaviour
{
	public ParticleSystem bulletExplosion;
	public float bulletDamage = 1f;
	public GameObject			bulletLight;
	
	/// <summary>
	/// Square distance to delete bullet if is to this distance from camera.
	/// </summary>
	private const float SQR_DISTANCE_TO_DELETE = 10000f;
	
	// Cache vars for performance
	private Transform myTransform;
	private Transform cameraTransform;
	
	private bool exploding;
	
	void Awake()
	{
		myTransform 	= transform;
		cameraTransform = Camera.main.transform;
		bulletLight.SetActive(false);
	}
	
	// Update is called once per frame
	void Update ()
	{
		if(Vector3.SqrMagnitude(myTransform.position-cameraTransform.position) > SQR_DISTANCE_TO_DELETE)
		{
			Destroy(gameObject);
		}else
		{
			if (!exploding)
			{
				Ray bulletRay = new Ray(myTransform.position,myTransform.forward);
				
				RaycastHit hit = new RaycastHit();
				
				if (Physics.Raycast(bulletRay,out hit,0.8f))
				{
					Enemy enemy = hit.transform.GetComponent<Enemy>();
					if(enemy != null)
						enemy.Damage(bulletDamage);
					
					Explode(myTransform.forward);
				}
			}
		}
	}
	
	public void Explode(Vector3 hitDirection)
	{
		exploding = true; 
		
		if(bulletExplosion != null)
		{
			bulletExplosion.transform.parent = null;
			bulletExplosion.transform.LookAt(Vector3.Reflect(hitDirection,hitDirection));
			bulletExplosion.Play();
			bulletLight.transform.parent = null;
			bulletLight.SetActive(true);
			Destroy(bulletExplosion.gameObject,0.4f);
			Destroy(bulletLight,0.1f);
			Destroy(gameObject);
			
		}else
		{
			Destroy(gameObject);
		}
	}
}
