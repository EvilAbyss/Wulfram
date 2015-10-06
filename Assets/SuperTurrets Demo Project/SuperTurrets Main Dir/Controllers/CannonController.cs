using UnityEngine;
using System.Collections;

/// <summary>
/// Controll cannon targeting movement.
/// </summary>
[AddComponentMenu("SuperTurrets/Cannon Controller")]
public class CannonController : MonoBehaviour 
{	
	private SuperTurret turretActor;
	private Quaternion 	originalRotation;
	private Vector3 	position1,position2 = Vector3.zero;
	private float		targetAngle;
	
	[HideInInspector]
	public 	Transform 	myTransform;
	

	/// <summary>
	/// Controller with recoil behavior activated when turret shots.
	/// </summary>
	public RecoilController		recoilController;
	/// <summary>
	/// Cannon delay to shot.
	/// </summary>
	public float 			 	shotDelay;
	/// <summary>
	/// The exact point where bullets will spawn.
	/// </summary>
	public Transform 			shotPoint;
	/// <summary>
	/// Cannon weapon. Can be null
	/// </summary>
	public AbstractWeapon		cannonWeapon;
	
	void Awake()
	{
		myTransform 		= transform;
		originalRotation	= myTransform.localRotation;
		enabled			= false;
		
		if(shotPoint == null)
			Debug.LogError("You must assign a shot point to CannonController in: "+name);
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
	void Update ()
	{	
		Transform targetTransform=turretActor.TargetTransform;
		if(targetTransform!=null)
		{
			Vector3 targetPosition = targetTransform.position;
			
			// Simple prediction based on target movement
			//Previous target position
			position2 = position1;
			//New target position
			position1 = targetTransform.position;
			
			// Actual position + last velocity vector
			targetPosition = targetTransform.position + (position1-position2);
				
			// Put targeting direction into local space
			Vector3 localTarget 	= myTransform.InverseTransformDirection( targetPosition- myTransform.position);
			// Calculate body targeting angle in the plane Y,Z. Cannon rotates around X axis.
	    	targetAngle 			= Mathf.Atan2(localTarget.z, localTarget.y) * Mathf.Rad2Deg;
			
			// Avoid cannon triying to point something behind it. Because base always is triying to point target, when target stay in front of the cannon
			// we start moving cannon towards target
			if (targetAngle > 0)
			{
				// Angle clamping
				float 	targetAngle2 	= targetAngle - 90f;
							
				// Interpolate between angles
				float adjustedAngle = Mathf.MoveTowardsAngle(0, targetAngle2, Time.deltaTime * turretActor.cannonTargetingVelocity);				
				// Rotate cannon
		    	myTransform.Rotate(Vector3.right, adjustedAngle);
			}
		}else{
			myTransform.localRotation=Quaternion.Slerp(myTransform.localRotation,originalRotation,0.07f);
			float angle = Quaternion.Angle(transform.localRotation,originalRotation);
			
			if(angle<1)
			{
				transform.localRotation = originalRotation;
				enabled=false;
			}
		}
	}
	
	/// <summary>
	/// Is this cannon approximately pointing to the target ?
	/// </summary>
	public bool IsCannonPointingTarget()
	{
		return Mathf.Abs(targetAngle-90f) < 5f;
	}
}
