using UnityEngine;
using System.Collections;

/// <summary>
/// This script destroys a GameObject with a ParticleSystem after the effect has finished
/// </summary>
public class DestroyWhenParticlesAreDone : MonoBehaviour
{
	ParticleSystem Particles;

	void Start()
	{
		Particles = GetComponent<ParticleSystem>();
		Destroy( this.gameObject, Particles.duration );
	}
}