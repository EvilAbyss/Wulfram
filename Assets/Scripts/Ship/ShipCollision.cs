using UnityEngine;
using System.Collections;

/// <summary>
/// Handles collision with the terrain
/// </summary>
public class ShipCollision : ShipBase
{
	void OnCollisionEnter( Collision collision )
	{
		if( PhotonView.isMine == false )
		{
			return;
		}

		if( collision.collider.tag == "Obstacle" )
		{
			//The ship should bounce back from the terrain a little bit. Otherwise it would just
			//move forward into the obstacle
			ShipMovement.AddImpact( collision.contacts[ 0 ].normal );

			Ship.DealDamage( 10, null );
		}
	}

	/// <summary>
	/// Called by the projectile that hits this ship
	/// </summary>
	/// <param name="projectile">The projectile.</param>
	public void OnProjectileHit( ProjectileBase projectile )
	{
		Ship.DealDamage( 10, projectile.Owner );
	}
	public void OnProjectileHit2( ProjectileBase2 projectile )
	{
		Ship.DealDamage( 10, projectile.Owner );
	}


}