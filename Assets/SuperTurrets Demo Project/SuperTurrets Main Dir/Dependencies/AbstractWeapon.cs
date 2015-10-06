using UnityEngine;
using System.Collections;

public abstract class AbstractWeapon : MonoBehaviour
{
	/// <summary>
	/// True when weapon is ready for shot. When Shot is called, weapon will shot.
	/// WeaponReady is a flag that will change depending on weapon realoding time, ammo, fire rate, etc.
	/// Each frame this var is getted to check if turret can shot.
	/// </summary>
	public bool WeaponReady {get; protected set; }
	
	/// <summary>
	///	Your weapon must create a new bullet and shot it to the target
	/// </summary>
	public abstract void Shot(Vector3 shotOrigin, GameObject target, Transform cannonTransform);
}

