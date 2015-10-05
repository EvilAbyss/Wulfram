using UnityEngine;
using System.Collections;

/// <summary>
/// We use a projector to simulate a shadow of the ship
/// This class rotates and squashes and stretches the shadow as needed
/// </summary>
public class ShipShadow : MonoBehaviour
{
	ShipVisuals m_ShipVisuals;
	Projector m_Projector;

	void Start()
	{
		m_ShipVisuals = transform.parent.GetComponent<ShipVisuals>();
		m_Projector = GetComponent<Projector>();
	}

	void Update()
	{
		UpdateShadowRotation();
		UpdateShadowScale();
	}

	void UpdateShadowRotation()
	{
		transform.rotation = Quaternion.Euler( 90, transform.parent.rotation.eulerAngles.y, 0 );
	}

	void UpdateShadowScale()
	{
		//If the ship is rolled on it's side, the shadow should be squashed a bit. Its not a 100% correct shadow,
		//but a close enough approximation for our cases
		m_Projector.aspectRatio = Mathf.Lerp( 1f, 0.2f, Mathf.Abs( m_ShipVisuals.GetRoll() ) * 0.5f );
	}
}