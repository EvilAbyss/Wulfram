using UnityEngine;
using System.Collections;

/// <summary>
/// Every ship component inherits from this one
/// Since they reference each other all the time, I was lazy and setup all the references once
/// </summary>
public class ShipBase : MonoBehaviour
{
	Ship m_Ship;
	public Ship Ship
	{
		get
		{
			return Helper.GetCachedComponent<Ship>( gameObject, ref m_Ship );
		}
	}

	ShipMovement m_Movement;
	public ShipMovement ShipMovement
	{
		get
		{
			return Helper.GetCachedComponent<ShipMovement>( gameObject, ref m_Movement );
		}
	}

	ShipCollision m_Collision;
	public ShipCollision ShipCollision
	{
		get
		{
			return Helper.GetCachedComponent<ShipCollision>( gameObject, ref m_Collision );
		}
	}




	ShipShooting m_Shooting;
	public ShipShooting ShipShooting
	{
		get
		{
			return Helper.GetCachedComponent<ShipShooting>( gameObject, ref m_Shooting );
		}
	}

	ShipShooting2 m_Shooting2;
	public ShipShooting2 ShipShooting2
	{
		get
		{
			return Helper.GetCachedComponent<ShipShooting2>( gameObject, ref m_Shooting2 );
		}
	}



	ShipVisuals m_Visuals;
	public ShipVisuals ShipVisuals
	{
		get
		{
			return Helper.GetCachedComponent<ShipVisuals>( gameObject, ref m_Visuals );
		}
	}

	ShipInput m_Input;
	public ShipInput ShipInput
	{
		get
		{
			return Helper.GetCachedComponent<ShipInput>( gameObject, ref m_Input );
		}
	}

	PhotonView m_View;
	public PhotonView PhotonView
	{
		get
		{
			return Helper.GetCachedComponent<PhotonView>( gameObject, ref m_View );
		}
	}

}