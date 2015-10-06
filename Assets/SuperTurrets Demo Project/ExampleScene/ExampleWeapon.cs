using UnityEngine;
using System.Collections;

/// <summary>
/// Example weapon provided with SuperTurrets.
/// </summary>
public class ExampleWeapon : AbstractWeapon
{
	public AudioSource 		shotAudioSource;
			
	public AudioClip 		shotSound;
	
	public float			shotFrequency;
	
	public Rigidbody 		bullet;
	
	public float			shotForce;
	
	public ParticleSystem	weaponParticleSystem;
	
	public Light			shotLight;
	
	private float			timeCounter = 0f;
	
	// Update is called once per frame
	void Update ()
	{
		if (timeCounter > shotFrequency && !WeaponReady)
		{
			WeaponReady = true;
		}else{
			timeCounter += Time.deltaTime;	
		}
	}

	#region implemented abstract members of AbstractWeapon
	public override void Shot (Vector3 shotOrigin, GameObject target, Transform cannonTransform)
	{
		//if(WeaponReady)
		{
			WeaponReady = false;
			
			Vector3 shotDirection = cannonTransform.forward;
			
			// Note: In a real project, you must not instante many objects a runtime, instead, you may consider creating a shoot pool.
			Rigidbody newBullet = Instantiate(bullet,shotOrigin,cannonTransform.rotation) as Rigidbody;
			
			newBullet.AddForce(shotForce*shotDirection);
			
			if(shotSound != null && shotAudioSource != null)
				shotAudioSource.PlayOneShot(shotSound);
			
			if(weaponParticleSystem != null)
				weaponParticleSystem.Play();
			
			StartCoroutine(LightFlash());
			
			// Reset timmer
			timeCounter = 0f;
		}
	}
	
	private IEnumerator LightFlash()
	{
		if (shotLight != null)
		{
			shotLight.enabled = true;
			yield return new WaitForSeconds(0.1f);
			shotLight.enabled = false;
			
		}
		
		yield return new WaitForSeconds(0f);
	}
	
	#endregion
}
