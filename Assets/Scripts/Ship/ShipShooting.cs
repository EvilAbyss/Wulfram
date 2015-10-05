using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class checks whether the ship is shooting or not
/// and creates the laser objects if appropriate.
/// It also keeps track of all the lasers so their destruction can be
/// synchronized over the network
/// </summary>
public class ShipShooting : ShipBase
{
	/// <summary>
	/// What is the minimal time that should pass between to lasers fired
	/// </summary>
	public float ShootDelay;

	/// <summary>
	/// How far ahead of the ship should the laser be created
	/// </summary>
	public float ProjectileSpawnForwardOffset;

	/// <summary>
	/// This value is set by the ShipInput class
	/// </summary>
	/// <value>
	/// <c>true</c> if this instance is shooting; otherwise, <c>false</c>.
	/// </value>
	public bool IsShooting
	{
		get;
		set;
	}

	float m_LastShootTime;
	int m_LastProjectileId;

	List<ProjectileBase> m_Projectiles = new List<ProjectileBase>();

	void Update()
	{
		UpdateShooting();
	}

	void UpdateShooting()
	{
		//We only want to shoot lasers for the local ship here
		//Remote ships create their lasers via RPC
		if( PhotonView.isMine == false )
		{
			return;
		}

		//Don't shoot if we're dead
		if( Ship.IsVisible == false )
		{
			return;
		}

		//Don't shoot if the round is over
		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			return;
		}

		//Well. The player should press the button, right?
		if( IsShooting == false )
		{
			return;
		}

		//Make sure we wait between shooting lasers
		if( Time.realtimeSinceStartup - m_LastShootTime < ShootDelay )
		{
			return;
		}

		//Each laser gets a projectile id unique to their ship, this variable is used to keep track of what id the projectile should get
		m_LastProjectileId++;

		if( PhotonNetwork.offlineMode == true )
		{
			OnShoot( GetProjectileSpawnPosition()
					, ShipVisuals.VisualParent.rotation
					, m_LastProjectileId
					, null );
		}
		else
		{
			//Send the shoot event to everybody including ourselves
			//By sending the lasers start position and start rotation, we have all the data we need
			//to synchronize its position exactly. Photon automatically sends the time when this RPC was
			//fired.
			//Check out Part 1 Lesson 3 http://youtu.be/ozBmZ9FoN_o for more detailed explanations
			PhotonView.RPC( "OnShoot"
							, PhotonTargets.All
							, new object[] { GetProjectileSpawnPosition()
							            , ShipVisuals.VisualParent.rotation
										, m_LastProjectileId 
										}
							);
		}
	}

	[RPC]
	public void OnShoot( Vector3 position, Quaternion rotation, int projectileId, PhotonMessageInfo info )
	{
		double timestamp = PhotonNetwork.time;

		if( info != null )
		{
			timestamp = info.timestamp;
		}

		CreateProjectile( position, rotation, timestamp, projectileId );
	}

	Vector3 GetProjectileSpawnPosition()
	{
		return transform.position + ShipVisuals.VisualParent.forward * ( ProjectileSpawnForwardOffset + 100 * PhotonNetwork.GetPing() * 0.001f );
	}

	public void CreateProjectile( Vector3 position, Quaternion rotation, double createTime, int projectileId )
	{
		m_LastShootTime = Time.realtimeSinceStartup;

		//Every player receives this so we create the prefabs locally instead of calling PhotonNetwork.Instantiate
		//By having its start position, start rotation and time when it was created, we can calculate where the laser is at all times
		//Check out Part 1 Lesson 3 http://youtu.be/ozBmZ9FoN_o for more detailed explanations
		GameObject newProjectileObject = (GameObject)Instantiate( Resources.Load<GameObject>( "Laser1" ), new Vector3( 0, -100, 0 ), rotation );
		newProjectileObject.name = "ZZZ_" + newProjectileObject.name;

		ProjectileBase newProjectile = newProjectileObject.GetComponent<ProjectileBase>();

		newProjectile.SetCreationTime( createTime );
		newProjectile.SetStartPosition( position );
		newProjectile.SetProjectileId( projectileId );

		newProjectile.Owner = Ship;

		m_Projectiles.Add( newProjectile );
	}

	public void SendProjectileHit( int projectileId )
	{
		if( PhotonNetwork.offlineMode == true )
		{
			OnProjectileHit( projectileId );
		}
		else
		{
			PhotonView.RPC( "OnProjectileHit", PhotonTargets.Others, new object[] { projectileId } );
		}
	}

	[RPC]
	public void OnProjectileHit( int projectileId )
	{
		//When we receive a projectile hit, it means that the projectile was destroyed on another client and we have to destroy it to
		//So we try to find the appropriate projectile through its ID and then destroy it as well

		m_Projectiles.RemoveAll( item => item == null );

		ProjectileBase projectile = m_Projectiles.Find( item => item.ProjectileId == projectileId );

		if( projectile != null )
		{
			projectile.OnProjectileHit();
			m_Projectiles.Remove( projectile );
		}
	}
}