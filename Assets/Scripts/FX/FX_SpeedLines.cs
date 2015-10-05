using UnityEngine;
using System.Collections;

/// <summary>
/// To give better sense of speed, we are spawning speed particles. This script tells the ParticleSystem how
/// many particles should be spawned, depending on the current speed of the player.
/// </summary>
public class FX_SpeedLines : MonoBehaviour
{
	Ship Ship;
	ParticleSystem Particles;

	void Start()
	{
		Ship = transform.parent.parent.GetComponent<Ship>();
		Particles = GetComponent<ParticleSystem>();

		//We only want to show the speed lines for the active player, so disable them if the ship is a remote ship
		if( Ship.PhotonView.isMine == false )
		{
			gameObject.SetActive( false );
		}
	}

	void Update()
	{
		Particles.emissionRate = Mathf.Clamp01( Ship.ShipMovement.GetCurrentSpeed() / 25f - 0.65f ) * 50;
	}
}
