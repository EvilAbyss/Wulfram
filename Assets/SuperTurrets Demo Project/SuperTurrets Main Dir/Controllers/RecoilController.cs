using UnityEngine;
using System.Collections;

/// <summary>
/// Do a recoil animation when turret shots.
/// </summary>
[AddComponentMenu("SuperTurrets/Recoil Controller")]
public class RecoilController : MonoBehaviour {
	
	private enum RecoilStates{
		goBack,
		goForward,
	}
	
	// Configure each parameter by each kind of turret.
	
	/// <summary>
	/// Max distance for recoil path.
	/// </summary>
	public float 			maxDistance;
	/// <summary>
	/// Backward velocity (when shot).
	/// </summary>
	public float 			backwardVelocity;
	/// <summary>
	/// The forward velocity when recoil recovery.
	/// </summary>
	public float 			forwardVelocity;
	
	private float 			actualDistance 		= 0;
	private float 			interpolationCut 	= 0.01f;
	private Transform 		myTransform;
	private Vector3			originalLocalPosition;
	private RecoilStates 	state;
	
	void Awake()
	{
		myTransform = gameObject.transform;
		originalLocalPosition = myTransform.localPosition;
		enabled=false;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 recoilDirection 		= Vector3.back;
		Vector3 actualPosition 			= myTransform.localPosition;		
		float 	realBackwardVelocity	= backwardVelocity*Time.deltaTime;
		float 	realForwardVelocity 	= forwardVelocity *Time.deltaTime;
		Vector3 newPosition 			= Vector3.zero;
		
		if(state == RecoilController.RecoilStates.goBack) // GO BACK (ANIMATION AFTER SHOT)
		{
			newPosition =  actualPosition+(recoilDirection*actualDistance);
			if(actualDistance>interpolationCut)
			{	
				myTransform.localPosition	= Vector3.Lerp(actualPosition,newPosition,realBackwardVelocity);
				actualDistance 			= Vector3.Distance(myTransform.localPosition,newPosition);
			}else{
				state = RecoilController.RecoilStates.goForward;	
				actualDistance=maxDistance-actualDistance;
			}
		}else if(state == RecoilController.RecoilStates.goForward) //GO FORWARD, (ANIMATION RELOADING)
		{
			newPosition =  actualPosition-(recoilDirection*actualDistance);
			if(actualDistance>interpolationCut)
			{	
				myTransform.localPosition	= Vector3.Lerp(actualPosition,newPosition,realForwardVelocity);
				actualDistance 			= Vector3.Distance(myTransform.localPosition,newPosition);
			}else{
				state = RecoilController.RecoilStates.goBack;	
				actualDistance = maxDistance-actualDistance;
				enabled=false;
			}
		}
	}
	
	public void Enable()
	{
		state = RecoilController.RecoilStates.goBack;
		actualDistance = maxDistance;
		myTransform.localPosition = originalLocalPosition;
		enabled = true;
	}
}
