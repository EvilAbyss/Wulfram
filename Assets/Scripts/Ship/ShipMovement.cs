using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class handles all the position and rotation updates for local and remote ships
/// It keeps track of how fast the ship is going, where it is turning, and if its bouncing off of an obstacle
/// </summary>
public class ShipMovement : ShipBase
{
	/// <summary>
	/// The normal speed of the ship
	/// </summary>
	public float Speed;

	/// <summary>
	/// The speed while the player is applying boost
	/// </summary>
	public float BoostSpeed;

	/// <summary>
	/// The speed while breaking
	/// </summary>
	public float BreakSpeed;

	/// <summary>
	/// How fast should the ship go from slow to fast
	/// </summary>
	public float Acceleration;

	/// <summary>
	/// How fast should the ship go from fast to slow
	/// </summary>
	public float Deceleration;

	/// <summary>
	/// How fast should the pitch change after the player sends the input
	/// </summary>
	public float PitchAcceleration;

	/// <summary>
	/// What is the maximum pitch angle of the ship while not in looping mode
	/// </summary>
	public float PitchMaximumAngle;

	/// <summary>
	/// How fast should the ship change its direction in looping mode (the higher this number, the shorter the radius of the looping)
	/// </summary>
	public float PitchLoopingAcceleration;

	/// <summary>
	/// How fast should the ship turn after the player sends the input
	/// </summary>
	public float TurnAcceleration;

	/// <summary>
	/// How fast should the ship tilt after the player sends the input
	/// </summary>
	public float TiltAcceleration;

	/// <summary>
	/// How fast is the ship pitching
	/// </summary>
	public float PitchSpeed;

	/// <summary>
	/// How fast is the ship turning
	/// </summary>
	public float TurnSpeed;

	/// <summary>
	/// How fast is the ship turning when tilted on its side
	/// </summary>
	public float TiltTurnSpeed;

	/// <summary>
	/// How strong should the ship bounce back when hitting an obstacle
	/// </summary>
	public float ImpactBounceStrength;

	/// <summary>
	/// How fast should the ship recover from an impact
	/// </summary>
	public float ImpactRecoverSpeed;

	/// <summary>
	/// Some people just want to see the world burn... ehhh their Y-Axis not inverted
	/// </summary>
	public bool DontInvertPitch;

	/// <summary>
	/// This is set by the input class to tell where the player wants to go
	/// </summary>
	/// <value>
	/// The target turn.
	/// </value>
	public float TargetTurn
	{
		get;
		set;
	}

	/// <summary>
	/// This is set by the input class to tell where the player wants to go
	/// </summary>
	/// <value>
	/// The target pitch.
	/// </value>
	public float TargetPitch
	{
		get;
		set;
	}

	/// <summary>
	/// This is set by the input class to tell how fast the player wants to go
	/// </summary>
	/// <value>
	/// The target boost power.
	/// </value>
	public float TargetBoost
	{
		get;
		set;
	}

	/// <summary>
	/// This is set by the input class to tell how strongly the player wants to tilt
	/// </summary>
	/// <value>
	/// The target tilt.
	/// </value>
	public float TargetTilt
	{
		get;
		set;
	}


	/// <summary>
	/// This value smoothly moves towards the turn that is send from the users input
	/// </summary>
	float m_TurnDelta;

	/// <summary>
	/// This value smoothly moves towards the pitch that is send from the users input
	/// </summary>
	float m_PitchDelta;

	/// <summary>
	/// This value smoothly moves towards the tilt that is send from the users input
	/// </summary>
	float m_TiltDelta;

	/// <summary>
	/// This is how fast the ship is currently turning and in what direction
	/// </summary>
	float m_TurnAngle;

	/// <summary>
	/// This is how fast the ship is currently pitching and in what direction
	/// </summary>
	float m_PitchAngle;

	/// <summary>
	/// This value smoothly moves towards the speed that is send from the users input
	/// </summary>
	float m_Speed;

	/// <summary>
	/// Looping mode is activated via right click, or pressing the left thumbstick down
	/// It allows the player to rotate freely and make a looping or 180° turns
	/// I've decided that you have to manually activate it because its easier for new players to
	/// get used to the regular flying first where you can't accidentally end up upside down ;)
	/// </summary>
	bool m_IsInLoopingMode;
	public bool IsInLoopingMode
	{
		get
		{
			return m_IsInLoopingMode;
		}
	}

	/// <summary>
	/// Note: This doesn't say if the ship is upside down or not, but rather if the controls are upside down
	/// During a looping we don't want to invert the controls as soon as the ship is upside down because that
	/// would be confusing. When you initiate looping mode the movement direction remains the same until you
	/// exit looping mode again. You can test this by entering looping mode, make a half turn, and notice 
	/// how the Y Axis is turned. Its more intuitive this way because the controls only change after the player
	/// consciously exits the looping mode
	/// </summary>
	bool m_IsMovementUpsideDown = false;

	/// <summary>
	/// When a ship hits an obstacle, it should bounce back
	/// This Vector stores the direction and strength of the bounce
	/// </summary>
	Vector3 m_ImpactMovement;

	/// <summary>
	/// This is the position where the network tells us the player is
	/// But since this is only updated 10 times a second, we store it so we can calculate the
	/// real position of a remote ship by applying it's known speed and turn angle
	/// </summary>
	Vector3 m_NetworkPosition;

	/// <summary>
	/// Same for the rotation. The ship should rotate smoothly, so we store the received value
	/// and interpolate towards it slowly to smoothen out any stutter
	/// </summary>
	Quaternion m_NetworkRotation;

	/// <summary>
	/// We need to know how old the last NetworkPosition and Rotation is so we can move the
	/// ship forward more, the older the data is
	/// </summary>
	double m_LastNetworkDataReceivedTime;

	void Start()
	{
		m_Speed = Speed;

		OnRespawn( transform.rotation.eulerAngles.y );
	}

	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			//stream.SendNext((int)controllerScript._characterState);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation); 
		}
		else
		{
			//Network player, receive data
			//controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this

	void Update()
	{
		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			return;
		}

		if( PhotonView.isMine == true )
		{
			if( Ship.Health > 0 )
			{
				//UpdateImpactMovement();
				///UpdateSpeed();
				//UpdateTurn();
				//UpdatePitch();
				//UpdateTilt();
				//UpdateTurnAngle();
				//UpdateRotation();
				//UpdatePosition();
				GetComponent<MouseLook>().enabled = true;
			
			}
		}
		else
		{
			//UpdateNetworkedPosition();
			//UpdateNetworkedRotation();
			GetComponent<MouseLook>().enabled = false;
		}
	}

	void UpdateNetworkedPosition()
	{
		//Here we try to predict where the player actually is depending on the data we received through the network
		//Check out Part 1 Lesson 2 http://youtu.be/7hWuxxm6wsA for more detailed explanations
		float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
		float timeSinceLastUpdate = (float)( PhotonNetwork.time - m_LastNetworkDataReceivedTime );
		float totalTimePassed = pingInSeconds + timeSinceLastUpdate;

		Vector3 exterpolatedTargetPosition = m_NetworkPosition
											+ transform.forward * m_Speed * totalTimePassed;


		Vector3 newPosition = Vector3.MoveTowards( transform.position
													, exterpolatedTargetPosition
													, m_Speed * Time.deltaTime );

		if( Vector3.Distance( transform.position, exterpolatedTargetPosition ) > 2f )
		{
			newPosition = exterpolatedTargetPosition;
		}

		newPosition.y = Mathf.Clamp( newPosition.y, 0.5f, 50f );

		transform.position = newPosition;
	}

	void UpdateNetworkedRotation()
	{
		transform.rotation = Quaternion.RotateTowards(
			transform.rotation,
			m_NetworkRotation, 180f * Time.deltaTime
		);
	}

	bool IsPlayerPitching()
	{
		return Mathf.Abs( m_PitchDelta ) > 0.3f || Mathf.Abs( TargetPitch ) > 0.1f;
	}

	public void AddImpact( Vector3 impactNormal )
	{
		//When a ship collides with the environment, we apply a bounce force to the ship
		//The normal of the impact serves us as the bounce direction because it ensures that
		//we are bouncing away from the obstacle, rather than bouncing into it

		//The impact severity tells us how bad the impact actually is
		//A severity value of 1 means that the ship was hitting the obstacle head on
		float impactSeverity = Vector3.Dot( impactNormal, ShipVisuals.VisualParent.transform.forward );

		//If the player flies straight into an obstacle, we just kill him immediately. Otherwise the
		//ship would bounce backwards, which looks awkward. Also, come on, if you fly head on into a mountain... you're dead :)
		if( Mathf.Abs( impactSeverity ) > 0.8f )
		{
			Ship.DealDamage( 999, null );
		}
		else
		{
			m_ImpactMovement += impactNormal * ImpactBounceStrength;
		}
	}

	void UpdateImpactMovement()
	{
		//Depending on how slow the impact movement is returned to zero, the bounce is longer and bigger
		m_ImpactMovement = Vector3.Lerp( m_ImpactMovement, Vector3.zero, ImpactRecoverSpeed * Time.deltaTime );
	}

	void UpdateSpeed()
	{
		//The player speed input ranges from [-1;1]
		//If its lower than zero, we want to apply the breaks
		//If its bigger than zero, we want to boost

		float targetSpeed = Speed;

		if( TargetBoost > 0 )
		{
			targetSpeed = Mathf.Lerp( Speed, BoostSpeed, TargetBoost );
		}
		else if( TargetBoost < 0 )
		{
			targetSpeed = Mathf.Lerp( Speed, BreakSpeed, Mathf.Abs( TargetBoost ) );
		}

		if( targetSpeed > m_Speed )
		{
			m_Speed = Mathf.Lerp( m_Speed, targetSpeed, Acceleration * Time.deltaTime );
		}
		else
		{
			m_Speed = Mathf.Lerp( m_Speed, targetSpeed, Deceleration * Time.deltaTime );
		}
	}

	void UpdateTilt()
	{
		//The player can manually tilt the ship on its side. By doing this, he can make tighter turns
		m_TiltDelta = Mathf.Lerp( m_TiltDelta, TargetTilt, TiltAcceleration * Time.deltaTime );
	}

	void UpdateTurn()
	{
		m_TurnDelta = Mathf.Lerp( m_TurnDelta, TargetTurn, TurnAcceleration * Time.deltaTime );
	}

	void UpdateTurnAngle()
	{
		float turnSpeed = TurnSpeed;

		if( m_TiltDelta != 0 && Mathf.Sign( m_TiltDelta ) == Mathf.Sign( m_TurnDelta ) )
		{
			turnSpeed = Mathf.Lerp( TurnSpeed, TiltTurnSpeed, Mathf.Abs( m_TiltDelta ) );
		}

		m_TurnAngle += m_TurnDelta * turnSpeed * Time.deltaTime;
	}

	void UpdatePitch()
	{
		if( m_IsInLoopingMode == true )
		{
			UpdatePitchLooping();
		}
		else
		{
			UpdatePitchNormal();
		}
	}

	/// <summary>
	/// This defines the pitch behavior when not in looping mode
	/// You cannot turn upside down in this mode so it is easier to understand at first
	/// We are simply lerping the pitch angle towards its target position
	/// </summary>
	void UpdatePitchNormal()
	{
		float upsideDownAngle = 0f;
		float upsideDownYAxisModifier = 1f;
		
		//Make sure that everything is inverted properly when we are in upside down mode
		if( m_IsMovementUpsideDown == true )
		{
			upsideDownAngle = 180f;
			upsideDownYAxisModifier = -1f;
		}

		float invertMultiplier = 1f;

		if( DontInvertPitch == true )
		{
			invertMultiplier *= -1f;
		}

		m_PitchDelta = TargetPitch;

		//Use Mathf.LerpAngle instead of Lerp here because we are dealing with angles between 0 and 360.
		//LerpAngle takes into consideration that the value can jump from 0° to 360°
		m_PitchAngle = Mathf.LerpAngle( m_PitchAngle, m_PitchDelta * upsideDownYAxisModifier * PitchMaximumAngle * invertMultiplier + upsideDownAngle, PitchAcceleration * Time.deltaTime );
	}

	/// <summary>
	/// This defines the pitch behavior while in looping mode.
	/// In this, we continuously apply the pitch delta to the pitch angle, which allows the ship to make a looping
	/// </summary>
	void UpdatePitchLooping()
	{
		m_PitchDelta = Mathf.Lerp( m_PitchDelta, TargetPitch, PitchLoopingAcceleration * Time.deltaTime );

		float invertMultiplier = 1f;

		if( DontInvertPitch == true )
		{
			invertMultiplier *= -1f;
		}

		if( IsMovementUpsideDown() == true )
		{
			invertMultiplier *= -1f;
		}

		m_PitchAngle += m_PitchDelta * PitchSpeed * invertMultiplier * Time.deltaTime;

		//Make sure the PitchAngle is between 0 and 360 degrees
		if( m_PitchAngle < 0 )
		{
			m_PitchAngle += 360;
		}

		if( m_PitchAngle > 360 )
		{
			m_PitchAngle -= 360;
		}

		//If the player let go of the thumbstick (aka, not pitching anymore) we want to leave looping mode
		//We also want to stop looping mode when he is too close to the ground
		if( IsPlayerPitching() == false || transform.position.y < 1.5f )
		{
			m_IsInLoopingMode = false;
			m_IsMovementUpsideDown = IsUpsideDown();
		}
	}

	void UpdateRotation()
	{
		transform.rotation = Quaternion.identity;

		//The order is important here
		//Just for fun, reverse the two next lines and see what happens.
		transform.Rotate( Vector3.up, m_TurnAngle );
		transform.Rotate( Vector3.right, m_PitchAngle );
	}

	void UpdatePosition()
	{
		//Calculate the new position based on speed and possible impact movement
		Vector3 newPosition = transform.position + GetForwardVector() * m_Speed * Time.deltaTime + m_ImpactMovement * Time.deltaTime;

		//We don't want the player to fly too close to the sun or the ground. It's hurtful
		newPosition.y = Mathf.Clamp( newPosition.y, 0.5f, 50f );
		transform.position = newPosition;

		//Tell the simulation how fast we are going, even though we don't use the physics velocity ourselves, it's important for collisions
		GetComponent<Rigidbody>().velocity = transform.forward;
	}

	public float GetCurrentSpeed()
	{
		return m_Speed;
	}

	public float GetTurnDelta()
	{
		return m_TurnDelta;
	}

	public float GetTiltedTurnDelta()
	{
		//Make the ship turn on its side if the player wants to

		if( m_TiltDelta < 0 )
		{
			return Mathf.Lerp( m_TurnDelta, -2f, Mathf.Abs( m_TiltDelta ) );
		}
		else if( m_TiltDelta > 0 )
		{
			return Mathf.Lerp( m_TurnDelta, 2f, Mathf.Abs( m_TiltDelta ) );
		}

		return m_TurnDelta;
	}

	public bool IsMovementUpsideDown()
	{
		return m_IsMovementUpsideDown;
	}

	public Vector3 GetForwardVector()
	{
		return transform.forward;
	}

	public Vector3 GetHorizontalForwardVector()
	{
		return Vector3.Cross( Vector3.up, -transform.right );
	}

	public bool IsUpsideDown()
	{
		return transform.up.y < 0;
	}

	public void InitiateLoopingMode()
	{
		m_IsInLoopingMode = true;
	}

	public void ToggleLoopingMode()
	{
		m_IsInLoopingMode = !m_IsInLoopingMode;

		if( m_IsInLoopingMode == false )
		{
			m_IsMovementUpsideDown = IsUpsideDown();
		}
	}

	public void OnRespawn( float turnAngle )
	{
		m_ImpactMovement = Vector3.zero;
		m_Speed = Speed;
		m_TurnDelta = 0;
		m_TurnAngle = turnAngle;
		m_PitchDelta = 0;
		m_PitchAngle = 0;
		m_IsMovementUpsideDown = false;

		TargetTilt = 0;
		TargetTurn = 0;
		TargetPitch = 0;
		TargetBoost = 0;
	}

	public void SerializeState( PhotonStream stream, PhotonMessageInfo info )
	{
		//We only need to synchronize a couple of variables to be able to recreate a good
		//approximation of the ships position on each client
		//There is a lot of smoke and mirrors happening here
		//Check out Part 1 Lesson 2 http://youtu.be/7hWuxxm6wsA for more detailed explanations
		if( stream.isWriting == true )
		{
			stream.SendNext( transform.position );
			stream.SendNext( transform.rotation );
			stream.SendNext( m_Speed );
			stream.SendNext( m_IsMovementUpsideDown );
		}
		else
		{
			m_NetworkPosition = (Vector3)stream.ReceiveNext();
			m_NetworkRotation = (Quaternion)stream.ReceiveNext();
			m_Speed = (float)stream.ReceiveNext();
			m_IsMovementUpsideDown = (bool)stream.ReceiveNext();

			m_LastNetworkDataReceivedTime = info.timestamp;
		}
	}

	//Debug Display of test variables
	/*void OnGUI()
	{
		GUILayout.BeginArea( new Rect( 10, 10, Screen.width, Screen.height ) );
		{
			GUILayout.Label( "Turn Delta: " + m_turnDelta );
			GUILayout.Label( "Turn Angle: " + m_turnAngle );
			GUILayout.Label( "Pitch Delta: " + m_pitchDelta );
			GUILayout.Label( "Pitch Angle: " + m_pitchAngle );
			GUILayout.Label( "Speed: " + m_speed );
			GUILayout.Label( "Impact Speed: " + m_impactMovement );
		}
		GUILayout.EndArea();
	}*/
}