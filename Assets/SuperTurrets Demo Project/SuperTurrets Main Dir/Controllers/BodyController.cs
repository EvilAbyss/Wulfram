using UnityEngine;
using System.Collections;

/// <summary>
/// Controll turret body. Turret body has a limited rotation.
/// </summary>
[AddComponentMenu("SuperTurrets/Body Controller")]
public class BodyController : MonoBehaviour 
{
	// Actor that manage this controller
	private SuperTurret turretActor;
	// Save original rotation so turret can return when idle
	private Quaternion 	originalRotation;
	// Save transform for optimization
	private Transform	myTransform;
	
	// Use this for initialization
	void Awake ()
	{
		myTransform = transform;
		originalRotation = myTransform.localRotation;
		enabled=false;
	}
	
	/// <summary>
	/// Main actor will call this method.
	/// </summary>
	/// <param name="actor">
	/// Turret actor where controller gets info,
	/// </param>
	public void setTurretActor(SuperTurret actor)
	{
		turretActor=actor;
	}
	
	// Update is called once per frame
	void Update () {
		Transform targetTransform = turretActor.TargetTransform;
		if(targetTransform != null)
		{
			// Put targeting direction into local space
			Vector3 localTarget = myTransform.InverseTransformDirection(targetTransform.position - myTransform.position);
			// Calculate body targeting angle in the plane X,Z. Turret base rotates around Y axis.
    		float targetAngle 	= Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
			// Angle interpolation
    		float adjustedAngle = Mathf.MoveTowardsAngle(0f, targetAngle, Time.deltaTime * turretActor.bodyTargetingVelocity);
   			transform.Rotate(Vector3.up, adjustedAngle);
		}else{
			myTransform.localRotation=Quaternion.Slerp(myTransform.localRotation,originalRotation,0.07f);	
			float angle = Quaternion.Angle(myTransform.localRotation,originalRotation);
			
			if(angle<1)
			{
				myTransform.localRotation = originalRotation;
				enabled=false;
			}
		}
	}
}
