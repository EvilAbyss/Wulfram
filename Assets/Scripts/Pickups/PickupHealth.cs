using UnityEngine;
using System.Collections;

/// <summary>
/// This class defines the pickup behavior for the health pickups
/// </summary>
public class PickupHealth : PickupBase
{
	/// <summary>
	/// The time it takes to re-spawn after it has been collected
	/// </summary>
	public float RespawnTime;

	float m_RespawnTimer = -1;
	bool m_IsPickedUp = false;

	void Update()
	{
		//The re-spawn time is only bigger than zero, if the flag has been collected previously
		if( m_RespawnTimer >= 0 )
		{
			m_RespawnTimer -= Time.deltaTime;

			if( m_RespawnTimer <= 0f )
			{
				m_RespawnTimer = -1f;
				SetVisibility( true );
				m_IsPickedUp = false;
			}
		}
	}

	void SetVisibility( bool visible )
	{
		Renderer[] renderers = GetComponentsInChildren<Renderer>();

		for( int i = 0; i < renderers.Length; ++i )
		{
			renderers[ i ].enabled = visible;
		}
	}

	public override bool CanBePickedUpBy( Ship ship )
	{
		return m_IsPickedUp == false;
	}

	public override void OnPickup( Ship ship )
	{
		if( m_IsPickedUp == false )
		{
			ship.SendHeal();
			SetVisibility( false );
			m_IsPickedUp = true;
			m_RespawnTimer = RespawnTime;
		}
	}
}