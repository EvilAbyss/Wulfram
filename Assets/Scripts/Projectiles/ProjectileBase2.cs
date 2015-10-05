using UnityEngine;
using System.Collections;

/// <summary>
/// Defines base functionality that is used in all projectiles. This demo only has one type of projectile, but try to add more yourself :)
/// </summary>
public class ProjectileBase2 : MonoBehaviour
{
	/// <summary>
	/// How fast is the projectile flying?
	/// </summary>
	public float Speed;
	
	/// <summary>
	/// How long is the projectile alive until it is destroyed automatically
	/// </summary>
	public float LifeTime;
	
	/// <summary>
	/// Reference to the hit effect that is played when the projectile hits a ship or the environment
	/// </summary>
	public GameObject HitFX;
	
	public double m_CreationTime;
	public Vector3 m_StartPosition;
	public int m_ProjectileId;
	
	public Ship m_Owner;
	
	/// <summary>
	/// The owner of this projectile is the ship who fired it
	/// </summary>
	public Ship Owner
	{
		get
		{
			return m_Owner;
		}
		set
		{
			m_Owner = value;
		}
	}
	
	
	public int ProjectileId
	{
		get
		{
			return m_ProjectileId;
		}
	}
	
	void Start()
	{
		//Do Physics.SphereCastAll
	}
	
	public void SetStartPosition( Vector3 position )
	{
		m_StartPosition = position;
	}
	
	public void SetCreationTime( double time )
	{
		m_CreationTime = time;
	}
	
	public void SetProjectileId( int id )
	{
		m_ProjectileId = id;
	}
	
	public void Update()
	{
		float timePassed = (float)( PhotonNetwork.time - m_CreationTime );
		transform.position = m_StartPosition + transform.forward * Speed * timePassed;
		
		if( timePassed > LifeTime )
		{
			Destroy( gameObject );
		}
		
		if( transform.position.y < 0f )
		{
			Destroy( gameObject );
			CreateHitFx();
		}
	}
	
	public void CreateHitFx()
	{
		Instantiate( HitFX, transform.position, transform.rotation );
	}
	
	public void OnProjectileHit2()
	{
		Destroy( gameObject );
		CreateHitFx();
	}
	
	public void OnCollisionEnter( Collision collision )
	{
		if( collision.collider.tag == "Obstacle" )
		{
			OnProjectileHit2();
		}
		else if( collision.collider.tag == "Ship" )
		{
			Ship ship = collision.collider.GetComponent<Ship>();
			
			if( ship.PhotonView.isMine == false )
			{
				return;
			}
			
			if( m_Owner.Team == Team.None || ship.Team != m_Owner.Team )
			{
				ship.ShipCollision.OnProjectileHit2( this );
				OnProjectileHit2();

				m_Owner.ShipShooting2.SendProjectileHit( m_ProjectileId );
				
			}
		}
	}
}