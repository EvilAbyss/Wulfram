using UnityEngine;
using System.Collections;

/// <summary>
/// Handles all behavior of the crosshair
/// </summary>
public class ShipCrosshair : MonoBehaviour
{
	Ship m_Ship;

	/// <summary>
	/// The ships visual parent is used to rotate the crosshairs
	/// </summary>
	Transform m_VisualParent;
	Transform m_Crosshair1;
	Transform m_Crosshair2;

	bool m_MouseTargetVisible = true;
	Transform m_MouseTarget;
	Transform m_MouseTargetArrows;
	Transform m_MouseTargetParent;

	Renderer[] m_MouseTargetArrowRenderers;

	public float Crosshair1TurnModifier;
	public float Crosshair2TurnModifier;

	void Start()
	{
		m_Ship = transform.parent.GetComponent<Ship>();

		m_VisualParent = m_Ship.transform.Find( "Visual Parent" );

		m_Crosshair1 = transform.Find( "Crosshair1" );
		m_Crosshair2 = transform.Find( "Crosshair2" );

		m_MouseTargetParent = transform.Find( "MouseTarget" );
		m_MouseTargetArrows = m_MouseTargetParent.Find( "Arrows" );
		m_MouseTarget = m_MouseTargetParent.Find( "Target" );
		m_MouseTargetArrowRenderers = m_MouseTargetArrows.GetComponentsInChildren<Renderer>();
	}

	void Update()
	{
		if( m_Ship == null )
		{
			Destroy( gameObject );
			return;
		}

		if( m_VisualParent == null )
		{
			return;
		}

		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			return;
		}

		transform.position = m_VisualParent.position;
		transform.LookAt( m_VisualParent.position + m_VisualParent.forward, Vector3.up );

		UpdateCrosshairRotation();
	}

	void UpdateCrosshairRotation()
	{
		//Making sure the crosshairs rotate nicely with the ship. This is just eye candy
		m_Crosshair1.localRotation = Quaternion.Euler( 0, m_Ship.ShipMovement.GetTiltedTurnDelta() * Crosshair1TurnModifier, 0 );
		m_Crosshair2.localRotation = Quaternion.Euler( 0, m_Ship.ShipMovement.GetTiltedTurnDelta() * Crosshair2TurnModifier, 0 );
	}

	void SetVisibility( bool visible, Transform parent )
	{
		Renderer[] renderers = parent.GetComponentsInChildren<Renderer>();

		foreach( Renderer renderer in renderers )
		{
			renderer.enabled = visible;
		}
	}

	public void SetMouseTargetVisibility( bool visible )
	{
		if( m_MouseTargetVisible == visible )
		{
			return;
		}

		m_MouseTargetVisible = visible;
		SetVisibility( visible, m_MouseTargetParent );
	}

	public void SetMouseTargetPosition( Vector2 mouseTarget )
	{
		//The mouse target will be received in [-1;1] coordinates
		//Depending on that, we rotate the arrows and show more or less of them

		if( m_MouseTargetVisible == false )
		{
			SetVisibility( false, m_MouseTargetParent );
			return;
		}

		if( m_Ship == null || m_Ship.Health <= 0f )
		{
			return;
		}

		//The y axis we get from ShipInput is inverted, but we want to show the direction where we go, so we have to invert it back again
		//However, we only want to do that if we are not in looping mode and upside down.
		//IsUpsideDown() tells us if we are visually upside down and
		//IsMovementUpsideDown() only changes its state once we leave looping mode. So we only want to invert it when we are right side up
		if( m_Ship.ShipMovement.IsUpsideDown() == m_Ship.ShipMovement.IsMovementUpsideDown() )
		{
			mouseTarget.y *= -1f;
		}

		//Just making the MouseTarget movement a little bit bigger
		m_MouseTarget.localPosition = new Vector3( mouseTarget.x * 1.5f, mouseTarget.y * 1.5f, 0f );

		float angle = Vector2.Angle( mouseTarget, Vector2.up );

		//Thee Angle we get from Vector2.Angle is always positive, so we have to modify it manually to get the whole 360°
		//The Cross between two vectors results in a third vector that is perpendicular to both vectors.
		Vector3 cross = Vector3.Cross( mouseTarget, Vector2.up );

		//If the resulting vector of the cross is facing forward, we change the angle to be on the [360;180] side of the circle
		if( cross.z > 0 )
		{
			angle = 360 - angle;
		}

		m_MouseTargetArrows.localRotation = Quaternion.Euler( 0, 0, angle );

		//The magnitude of the mouse target tells us how strongly the player wants to turn
		//and the stronger the turn, the more arrows we want to display
		//To be honest, the values here are magic numbers, I just tried what looks best. (*CHEAT*) ;)
		float magnitude = mouseTarget.magnitude;

		m_MouseTargetArrowRenderers[ 0 ].enabled = magnitude > 0.5f;
		m_MouseTargetArrowRenderers[ 1 ].enabled = magnitude > 0.7f;
		m_MouseTargetArrowRenderers[ 2 ].enabled = magnitude > 0.9f;
		m_MouseTargetArrowRenderers[ 3 ].enabled = magnitude > 1.1f;
		m_MouseTargetArrowRenderers[ 4 ].enabled = magnitude > 1.3f;
	}
}