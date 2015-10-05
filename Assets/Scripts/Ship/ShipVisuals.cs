using UnityEngine;
using System.Collections;

/// <summary>
/// This class deals with the visual representation of the ship
/// I created it as a child to the actual ship object because I wanted to be able to
/// rotate and roll the visual representation further, without affecting the movement direction
/// As a general tip, I found that it's always better to make the actual mesh of the object a child of the main object
/// because it makes a lot of stuff easier once the object get more complicated
/// </summary>
public class ShipVisuals : ShipBase
{
	/// <summary>
	/// The visual parent is a child object of the main gameObject this component is attached to
	/// so we need to know which of the child objects it is
	/// </summary>
	public Transform VisualParent;

	/// <summary>
	/// How strongly should the ship roll when making a turn
	/// </summary>
	public float MaximumRollAngle;

	/// <summary>
	/// How fast should the ship roll from once side to the next
	/// </summary>
	public float RollAcceleration;

	/// <summary>
	/// When turning the ship right-side up again after a half-looping, how fast should it turn?
	/// </summary>
	public float UpsideDownRollAcceleration;

	/// <summary>
	/// When the ship turns, the visual representation turns even further. This gives the player a bigger range where he can shoot
	/// </summary>
	public float MaximumTurnAngle;

	/// <summary>
	/// How fast should the visual object turn
	/// </summary>
	public float TurnAcceleration;

	float m_Roll;
	float m_UpsideDownRoll;
	float m_UpsideDownTurnModifier;
	float m_VisualTurn;

	float m_NetworkedRoll;

	ParticleSystem m_SmokeFx;

	void Start()
	{
		m_SmokeFx = VisualParent.Find( "FX_Smoke" ).GetComponent<ParticleSystem>();
	}

	void Update()
	{
		if( PhotonView.isMine == true )
		{
			UpdateRoll();
		}
		else
		{
			UpdateNetworkedRoll();
		}

		UpdateUpsideDownTurnModifier();
		UpdateVisualTurn();
		UpdateUpsideDownRoll();
		UpdateVisualRotation();
	}

	void UpdateNetworkedRoll()
	{
		m_Roll = Mathf.MoveTowards( m_Roll, m_NetworkedRoll, Time.deltaTime * RollAcceleration );
	}

	void UpdateUpsideDownRoll()
	{
		float targetUpsideDownRoll = 0f;

		if( ShipMovement.IsMovementUpsideDown() == true )
		{
			targetUpsideDownRoll = 180f;
		}

		//When dealing with movement, I always try to use Lerp rather than MoveTowards, it creates a more organic feel 
		//whereas MoveTowards feels more robotic, since it is strictly linear
		m_UpsideDownRoll = Mathf.Lerp( m_UpsideDownRoll, targetUpsideDownRoll, UpsideDownRollAcceleration * Time.deltaTime );
	}

	void UpdateVisualTurn()
	{
		m_VisualTurn = Mathf.Lerp( m_VisualTurn, ShipMovement.GetTurnDelta(), TurnAcceleration * Time.deltaTime );
	}

	void UpdateRoll()
	{
		m_Roll = Mathf.Lerp( m_Roll, ShipMovement.GetTiltedTurnDelta(), RollAcceleration * Time.deltaTime );
	}

	void UpdateUpsideDownTurnModifier()
	{
		float target = 1f;

		if( ShipMovement.IsUpsideDown() == true )
		{
			target = -1f;
		}

		m_UpsideDownTurnModifier = Mathf.Lerp( m_UpsideDownTurnModifier, target, Time.deltaTime );
	}

	void UpdateVisualRotation()
	{
		//Since the VisualParent is just a child of the main object, we can rotate it however we want without affecting its actual movement
		//This is just eye candy rotation to make the flying feel better

		VisualParent.transform.localRotation = Quaternion.identity;
		
		//The order how we rotate the ship around the different axis is important here.
		//Try to change it to get a feel for what is going wrong when you do
		VisualParent.transform.Rotate( Vector3.up, m_VisualTurn * MaximumTurnAngle * m_UpsideDownTurnModifier );
		VisualParent.transform.Rotate( Vector3.forward, -m_Roll * MaximumRollAngle + m_UpsideDownRoll );
	}

	public float GetRoll()
	{
		return m_Roll;
	}

	public void SetTeamColors( Team team )
	{
		Debug.Log( "Set Team: " + team );
		//When setting the ships team, we want to change its colors too to represent it
		//The colors have to be changed not only on the main mesh, but also on the exhaust trail

		if( team == Team.Blue )
		{
			VisualParent.Find( "Ship_Player_07" ).GetComponent<Renderer>().material.color = Color.blue;
			VisualParent.Find( "Ship_Player_07" ).GetComponent<Renderer>().material.SetColor( "_Color1", new Color( 0.5f, 0.5f, 1f ) );
			VisualParent.Find( "FX_Exaust" ).GetComponent<ParticleSystem>().startColor = Color.blue;
			VisualParent.Find( "FX_Exaust" ).Find( "Particle System Glow" ).GetComponent<ParticleSystem>().startColor = Color.blue;
			VisualParent.Find( "FX_Exaust" ).Find( "Trail" ).GetComponent<Renderer>().material.SetColor( "_ColorBottom", Color.blue );
			VisualParent.Find( "FX_Exaust" ).Find( "Trail" ).GetComponent<Renderer>().material.SetColor( "_ColorTop", new Color( 0.5f, 0.5f, 1f ) );
		}
		else if( team == Team.Red )
		{
			VisualParent.Find( "Ship_Player_07" ).GetComponent<Renderer>().material.color = Color.red;
			VisualParent.Find( "Ship_Player_07" ).GetComponent<Renderer>().material.SetColor( "_Color1", new Color( 1f, 0.5f, 0.5f ) );
			VisualParent.Find( "FX_Exaust" ).GetComponent<ParticleSystem>().startColor = Color.red;
			VisualParent.Find( "FX_Exaust" ).Find( "Particle System Glow" ).GetComponent<ParticleSystem>().startColor = Color.red;
			VisualParent.Find( "FX_Exaust" ).Find( "Trail" ).GetComponent<Renderer>().material.SetColor( "_ColorBottom", Color.red );
			VisualParent.Find( "FX_Exaust" ).Find( "Trail" ).GetComponent<Renderer>().material.SetColor( "_ColorTop", new Color( 1f, 0.5f, 0.5f ) );
		}
		else
		{
			VisualParent.Find( "Ship_Player_07" ).GetComponent<Renderer>().material.color = new Color( 0.05f, 0.05f, 0.05f );
			VisualParent.Find( "Ship_Player_07" ).GetComponent<Renderer>().material.SetColor( "_Color1", new Color( 0.5f, 0.5f, 0.5f ) );
			VisualParent.Find( "FX_Exaust" ).GetComponent<ParticleSystem>().startColor = Color.red;
			VisualParent.Find( "FX_Exaust" ).Find( "Particle System Glow" ).GetComponent<ParticleSystem>().startColor = Color.yellow;
			VisualParent.Find( "FX_Exaust" ).Find( "Trail" ).GetComponent<Renderer>().material.SetColor( "_ColorBottom", Color.yellow );
			VisualParent.Find( "FX_Exaust" ).Find( "Trail" ).GetComponent<Renderer>().material.SetColor( "_ColorTop", new Color( 1f, 1f, 0.8f ) );
		}
	}

	public void OnRespawn()
	{
		m_Roll = 0;
		m_UpsideDownRoll = 0;
		m_UpsideDownTurnModifier = 1;
		m_VisualTurn = 0;
		m_SmokeFx.emissionRate = 0f;
	}

	public void OnHealthChanged( float health )
	{
		//If the players health is below a certain threshold, we want to create a smoke effect 
		//that gets stronger when the player takes more damage
		if( health <= 11f )
		{
			m_SmokeFx.emissionRate = 30f;
		}
		else if( health <= 21f )
		{
			m_SmokeFx.emissionRate = 10f;
		}
		else
		{
			m_SmokeFx.emissionRate = 0f;
		}
	}

	public void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{
		if( stream.isWriting == true )
		{
			stream.SendNext( m_Roll );
		}
		else
		{
			m_NetworkedRoll = (float)stream.ReceiveNext();
		}
	}
}