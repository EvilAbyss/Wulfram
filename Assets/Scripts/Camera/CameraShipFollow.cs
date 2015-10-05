using UnityEngine;
using System.Collections;

/// <summary>
/// CameraShipFollow is attached to the main camera and controls its movement.
/// After a target ship has been set with SetTarget(), the camera will follow
/// this target and lean intro curves to provide more visibility in a dogfight
/// </summary>
public class CameraShipFollow : MonoBehaviour
{
	/// <summary>
	/// The cameras offset position from the target ship at normal speed
	/// </summary>
	public Vector3 Offset;

	/// <summary>
	/// The cameras offset position from the target ship while breaking
	/// </summary>
	public Vector3 BreakOffset;

	/// <summary>
	/// The cameras offset position from the target ship at maximum speed
	/// </summary>
	public Vector3 BoostOffset;

	/// <summary>
	/// The position relative to the target ship which the camera looks at
	/// </summary>
	public Vector3 LookAtTarget;

	/// <summary>
	/// The angle the camera should turn when the target ship has the maximum turn angle applied
	/// </summary>
	public float MaximumTurnAngle;

	/// <summary>
	/// The speed at which the camera should turn when the target ship turns.
	/// This is used to smooth out quick back and forth movements by the player
	/// </summary>
	public float TurnAcceleration;

	/// <summary>
	/// This is the speed at which the camera changes positions from the back of the ship to the front of the ship when the ship is upside down
	/// </summary>
	public float UpsideDownLerpSpeed;

	/// <summary>
	/// This is the speed at which the camera moves forward and back when the player changes speed
	/// </summary>
	public float DistanceLerpSpeed;

	/// <summary>
	/// The intensity the camera shakes when hit by a laser
	/// </summary>
	public float ShakeIntensity = 0.1f;
	Ship m_Target;

	Vector3 m_TargetPosition;
	Vector3 m_LookAtPosition;

	float m_UpsideDownLerp;

	Vector2 m_CameraShakeOffset;

	float m_ShakeTime = 0;

	float m_DistanceLerp;

	float m_TurnAngle;

	void LateUpdate()
	{
		if( m_Target == null )
		{
			return;
		}

		UpdateTurnAngle();
		UpdateUpsideDownLerp();
		UpdateDistanceLerp();
		UpdateCameraShake();
		UpdatePosition();
		UpdateRotation();
	}

	/// <summary>
	/// This looks for the ships delta turn value and adjusts the camera turn angle accordingly
	/// </summary>
	void UpdateTurnAngle()
	{
		m_TurnAngle = Mathf.Lerp( m_TurnAngle, m_Target.ShipMovement.GetTurnDelta() * MaximumTurnAngle, TurnAcceleration * Time.deltaTime );
	}   

	/// <summary>
	/// The distance lerp is used to determine at what offset position the camera currently is. Offset, BreakOffset or BoostOffset
	/// </summary>
	void UpdateDistanceLerp()
	{
		m_DistanceLerp = Mathf.Lerp( m_DistanceLerp, m_Target.ShipMovement.TargetBoost, DistanceLerpSpeed * Time.deltaTime );
	}

	/// <summary>
	/// When the player is upside down, we want the camera to change positions so it will always be behind the ship
	/// </summary>
	void UpdateUpsideDownLerp()
	{
		float targetLerp = 0f;

		if( m_Target.ShipMovement.IsUpsideDown() == true )
		{
			targetLerp = 1f;
		}

		m_UpsideDownLerp = Mathf.Lerp( m_UpsideDownLerp, targetLerp, UpsideDownLerpSpeed * Time.deltaTime );
	}

	public Ship GetTarget()
	{
		return m_Target;
	}

	/// <summary>
	/// Sets the target ship which the camera should follow. This is always the active player in the game
	/// </summary>
	/// <param name="target">The target ship which should be followed</param>
	public void SetTarget( Ship target )
	{
		m_Target = target;

		//The DecalIcon of a ship is the small diamond that indicates where the ship is. But we don't want to display it for the active player
		Destroy( m_Target.GetComponent<DecalIcon>() );
	}

	/// <summary>
	/// Starts the camera shake.
	/// </summary>
	/// <param name="length">The length of the shake.</param>
	/// <param name="intensity">The shake intensity.</param>
	public void StartShake( float length, float intensity = 0.1f )
	{
		ShakeIntensity = intensity;
		m_ShakeTime = length;
	}

	/// <summary>
	/// We are adding a couple of sine and cosine waves with different wave-lengths together to get a random looking shake
	/// </summary>
	void UpdateCameraShake()
	{
		m_ShakeTime = Mathf.Clamp01( m_ShakeTime - Time.deltaTime * 0.5f );
		m_CameraShakeOffset.x = Mathf.Sin( 10 + Time.time * 16 ) + Mathf.Cos( Time.time * 25 ) + Mathf.Cos( 5 + Time.time * 12 ) * 0.5f;
		m_CameraShakeOffset.y = Mathf.Sin( Time.time * 34 ) + Mathf.Cos( 15 + Time.time * 21 ) + Mathf.Cos( 3 + Time.time * 50 );
	}

	/// <summary>
	/// Called when the target ship is re-spawning. Move the camera behind the ship in right-side up position again.
	/// </summary>
	public void OnRespawn()
	{
		m_UpsideDownLerp = 0;
		m_DistanceLerp = 1;
	}

	void UpdatePosition()
	{
		Vector3 realOffset = Offset;

		//If the player is applying boost
		if( m_DistanceLerp > 0 )
		{
			realOffset = Vector3.Lerp( Offset, BoostOffset, m_DistanceLerp );
		}
		//if the player is breaking
		else if( m_DistanceLerp < 0 )
		{
			realOffset = Vector3.Lerp( Offset, BreakOffset, Mathf.Abs( m_DistanceLerp ) );
		}

		//Determine if the camera should be in front of, or behind of the ship. Depending if it's upside down or not.
		Vector3 offsetPositionZ = Vector3.Lerp( m_Target.ShipMovement.GetHorizontalForwardVector() * realOffset.z, -m_Target.ShipMovement.GetHorizontalForwardVector() * realOffset.z, m_UpsideDownLerp );
		Vector3 shake = m_CameraShakeOffset * m_ShakeTime * ShakeIntensity;

		m_TargetPosition = m_Target.transform.position
			+ offsetPositionZ
			+ Vector3.up * realOffset.y
			+ transform.right * shake.x
			+ transform.up * shake.y;

		transform.position = m_TargetPosition;
	}

	void UpdateRotation()
	{
		//If the player is upside down, the look at position has to be inverted
		Vector3 offsetPositionZ = Vector3.Lerp( m_Target.ShipMovement.GetHorizontalForwardVector() * LookAtTarget.z, -m_Target.ShipMovement.GetHorizontalForwardVector() * LookAtTarget.z, m_UpsideDownLerp );

		Vector3 lookAtPosition = m_Target.transform.position
			+ m_Target.transform.right * LookAtTarget.x
			+ offsetPositionZ
			+ Vector3.up * LookAtTarget.y;

		transform.rotation = Quaternion.LookRotation( lookAtPosition - transform.position );
		transform.Rotate( Vector3.up, m_TurnAngle );
	}
}