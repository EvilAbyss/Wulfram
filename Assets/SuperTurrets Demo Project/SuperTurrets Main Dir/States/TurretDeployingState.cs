using UnityEngine;
using System.Collections;

/// <summary>
/// State for turret deploying animation
/// </summary>
public class TurretDeployingState : FSMState {
	
	private AnimationController animationController;
	private SuperTurret turretActor;
	bool deploying=false;
	
	public TurretDeployingState(GameObject npc,AnimationController animationController):base(npc)
	{
		stateID = StateID.Deploying;
		turretActor = npc.GetComponent<SuperTurret>();
		this.animationController = animationController;
	}
	
	public override void Reason (GameObject player)
	{
		// Is deploying and animation has finished ? Or turret has not animation ?
 		if( (deploying && !animationController.GetComponent<Animation>().isPlaying) || animationController == null)
		{
			turretActor.GetMachineState().PerformTransition(Transition.DeployingEnd);
		}	
	}
	
	public override void Act (GameObject player)
	{
		// Start deploying animation if not started yet
		if(animationController != null && !animationController.GetComponent<Animation>().isPlaying)
		{
			animationController.PlayAnimationForward();
			
			deploying=true;
		}
	}
	
	public override void DoBeforeLeaving ()
	{
		//If deploying animation ends...
		deploying=false;
		base.DoBeforeLeaving();
	}
}
