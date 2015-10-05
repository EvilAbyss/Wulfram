using UnityEngine;
using System.Collections;

/// <summary>
/// This class is used in offline mode for the second dummy ship that always flies in circles
/// The ship was created for to be able to test hit collisions with lasers
/// </summary>
public class ShipInputDummy : ShipBase
{
	void Start()
	{
		Ship.SetTeam( Team.Blue );
	}

	void Update()
	{
		UpdateTargetPitch();
		UpdateTargetTurn();
		UpdateTargetBoost();
		UpdateTargetTilt();
		UpdateIsShooting();
		UpdateIsShooting2();
	}

	void UpdateIsShooting()
	{
		ShipShooting.IsShooting = true;
	}
	void UpdateIsShooting2()
	{
		ShipShooting2.IsShooting = true;
	}
	void UpdateTargetTurn()
	{
		ShipMovement.TargetTurn = 0.5f;
	}

	void UpdateTargetPitch()
	{
		ShipMovement.TargetPitch = 0;
	}

	void UpdateTargetBoost()
	{
		ShipMovement.TargetBoost = 0;
	}

	void UpdateTargetTilt()
	{
		ShipMovement.TargetTilt = 0;
	}
}