using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Available teams to choose from
/// </summary>
public enum Team
{
	Red,
	Blue,
	None,
}

/// <summary>
/// This is the main ship class
/// It handles health, respawn, communication between scripts and synchronizing the ship through the network via OnPhotonSerializeView
/// </summary>
public class Ship : ShipBase
{
	public static Ship LocalPlayer;

	public bool IsLocalPlayer = true;

	/// <summary>
	/// This prefab is instantiated for the local player to show the crosshair
	/// </summary>
	public GameObject CrosshairPrefab;


	/// <summary>
	/// Big explosion that gets instantiated when the ship dies
	/// </summary>
	public GameObject ExplosionPrefab;

	float m_Health = 100;

	/// <summary>
	/// The health is between 50 and 0
	/// </summary>
	/// <value>
	/// The ships health.
	/// </value>
	public float Health
	{
		get
		{
			return m_Health;
		}
	}

	/// <summary>
	/// The string for the kill count custom property
	/// </summary>
	const string KillCountProperty = "KillCount";

	int m_killCount = 0;
	/// <summary>
	/// Gets or sets the kill count. Depending on whether we are online or offline, this is either
	/// stored in the local m_killCount variable or in the players custom properties. 
	/// This is all done in the helper functions Helper.GetCustomProperty and Helper.SetCustomProperty
	/// </summary>
	public int KillCount
	{
		get
		{
			return Helper.GetCustomProperty<int>( PhotonView, KillCountProperty, m_killCount, 0 );
		}
		set
		{
			Helper.SetCustomProperty<int>( PhotonView, KillCountProperty, ref m_killCount, value );
		}
	}

	/// <summary>
	/// The string for the flag grab count custom property
	/// </summary>
	const string FlagGrabCountProperty = "FlagGrabCount";

	int m_flagGrabCount = 0;
	public int FlagGrabCount
	{
		get
		{
			return Helper.GetCustomProperty<int>( PhotonView, FlagGrabCountProperty, m_flagGrabCount, 0 );
		}
		set
		{
			Helper.SetCustomProperty<int>( PhotonView, FlagGrabCountProperty, ref m_flagGrabCount, value );
		}
	}

	/// <summary>
	/// The string for the flag grab count custom property
	/// </summary>
	const string FlagCaptureCountProperty = "FlagCaptureCount";

	int m_flagCaptureCount = 0;
	public int FlagCaptureCount
	{
		get
		{
			return Helper.GetCustomProperty<int>( PhotonView, FlagCaptureCountProperty, m_flagCaptureCount, 0 );
		}
		set
		{
			Helper.SetCustomProperty<int>( PhotonView, FlagCaptureCountProperty, ref m_flagCaptureCount, value );
		}
	}

	bool m_IsVisible = true;


	/// <summary>
	/// Gets a value indicating whether this instance is visible.
	/// </summary>
	/// <value>
	/// <c>true</c> if this ship is flying; if it's dead, <c>false</c>.
	/// </value>
	public bool IsVisible
	{
		get
		{
			return m_IsVisible;
		}
	}

	Team m_Team;

	/// <summary>
	/// Each ship is either in the red or in the blue team
	/// </summary>
	/// <value>
	/// The team this ship belongs to
	/// </value>
	public Team Team
	{
		get
		{
			return m_Team;
		}
	}

	void Start()
	{
		KillCount = 0;
		FlagGrabCount = 0;
		FlagCaptureCount = 0;

		if( PhotonView.isMine == true )
		{
			if( IsLocalPlayer == true )
			{
				LocalPlayer = this;
			}

			CreateCrosshair();
		}
	}

	void CreateCrosshair()
	{
		GameObject newCrosshair = (GameObject)Instantiate( CrosshairPrefab );
		newCrosshair.name = "Crosshair";
		newCrosshair.transform.parent = transform;
		newCrosshair.transform.localPosition = Vector3.zero;
		newCrosshair.transform.localRotation = Quaternion.identity;

		BroadcastMessage( "OnCrosshairCreated", SendMessageOptions.DontRequireReceiver );
	}

	public void SendRespawn()
	{
		Transform spawnPoint = GamemodeManager.CurrentGamemode.GetSpawnPoint( Team );

		if( PhotonNetwork.offlineMode == true )
		{
			OnRespawn( spawnPoint.transform.position, spawnPoint.transform.rotation );
		}
		else
		{
			PhotonView.RPC( "OnRespawn", PhotonTargets.All, spawnPoint.transform.position, spawnPoint.transform.rotation );
		}
	}



	
	public void SetTeam( Team team )
	{
		//This method gets called right after a ship is created

		m_Team = team;

		ShipVisuals.SetTeamColors( team );

		DecalIcon icon = GetComponent<DecalIcon>();

		if( icon != null )
		{
			if( team == Team.Blue )
			{
				icon.Color = Color.blue;
			}
			else
			{
				icon.Color = Color.red;
			}
		}
	}

	public void DealDamage( float damage, Ship damageDealer )
	{
		m_Health -= damage;

		OnHealthChanged( damageDealer );
	}




	public void SendHeal()
	{
		if( PhotonNetwork.offlineMode == true )
		{
			OnHeal();
		}
		else
		{
			if( PhotonView.isMine == true )
			{
				PhotonView.RPC( "OnHeal", PhotonTargets.All );
			}
		}
	}

	public void OnKilledShip( Ship otherShip )
	{
		KillCount = KillCount + 1;
	}

	void OnHealthChanged()
	{
		OnHealthChanged( null );
	}

	void OnHealthChanged( Ship damageDealer )
	{
		if( m_Health <= 0 )
		{
			m_Health = 0;
			Instantiate( ExplosionPrefab, transform.position, Quaternion.identity );
			SetVisibility( false );

			//If our local ship dies, call the respawn function after 2 seconds and award
			//the kill to the damage dealer
			if( PhotonView.isMine == true )
			{
				Invoke( "SendRespawn", 2f );
			}
		}

		//The ShipVisuals component shows the smoke when the ship is damaged
		ShipVisuals.OnHealthChanged( m_Health );
	}

	void SetVisibility( bool visible )
	{
		m_IsVisible = visible;

		Renderer[] renderers = GetComponentsInChildren<Renderer>();

		for( int i = 0; i < renderers.Length; ++i )
		{
			renderers[ i ].enabled = visible;
		}
	}

	
	void OnTriggerEnter(Collider other){
		Transform spawnPoint = GamemodeManager.CurrentGamemode.GetSpawnPoint( Team );


		if (other.gameObject.tag == "Projectile2" || other.gameObject.tag == "Projectile3") {
			Instantiate( ExplosionPrefab, transform.position, Quaternion.identity );
			SetVisibility( false );
			if( PhotonView.isMine == true )
			{
				Ship.DealDamage( 50, null );
				//OnRespawn( spawnPoint.transform.position, spawnPoint.transform.rotation );
			}
		
			}
			

}
		
	


	[RPC]
	void OnHeal()
	{
		m_Health = 50;
		OnHealthChanged();
	}

	[RPC]
	void OnRespawn( Vector3 spawnPosition, Quaternion spawnRotation )
	{
		transform.position = spawnPosition;
		transform.rotation = spawnRotation;

		SetVisibility( true );
		m_Health = 50;
		OnHealthChanged();

		//The OnRespawn functions of other components make sure that all values are properly reset to their initial values again
		ShipMovement.OnRespawn( spawnRotation.eulerAngles.y );
		ShipVisuals.OnRespawn();

		if( GetComponent<ShipInput>() != null )
		{
			GetComponent<ShipInput>().OnRespawn();
		}

		CameraShipFollow cameraFollow = Camera.main.GetComponent<CameraShipFollow>();

		if( cameraFollow != null && cameraFollow.GetTarget() == this )
		{
			cameraFollow.OnRespawn();
		}
	}

	void OnPhotonInstantiate( PhotonMessageInfo info )
	{
		//This method gets called right after a GameObject is created through PhotonNetwork.Instantiate
		//The fifth parameter in PhotonNetwork.instantiate sets the instantiationData and every client
		//can access them through the PhotonView. In our case we use this to send which team the ship
		//belongs to. This methodology is very useful to send data that only has to be sent once.

		if( PhotonView.isMine == false )
		{
			SetTeam( (Team)PhotonView.instantiationData[ 0 ] );
		}
	}

	void OnPhotonSerializeView( PhotonStream stream, PhotonMessageInfo info )
	{
		//Multiple components need to synchronize values over the network.
		//The SerializeState methods are made up, but they're useful to keep
		//all the data separated into their respective components

		SerializeState( stream, info );

		ShipVisuals.SerializeState( stream, info );
		ShipMovement.SerializeState( stream, info );
	}

	void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{
		if( stream.isWriting == true )
		{
			stream.SendNext( m_Health );
		}
		else
		{
			float oldHealth = m_Health;
			m_Health = (float)stream.ReceiveNext();

			if( m_Health != oldHealth )
			{
				OnHealthChanged();
			}
		}
	}
} 
