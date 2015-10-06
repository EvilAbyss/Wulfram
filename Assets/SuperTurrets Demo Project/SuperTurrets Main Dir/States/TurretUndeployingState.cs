using UnityEngine;
using System.Collections;

/// <summary>
/// Turret undeploying state. We control that all components have finished before start undeploying animation.
/// If there is not undeploying animation, turret will go to it's original orientation.
/// </summary>
public class TurretUndeployingState : FSMState 
{
	private SuperTurret turretActor;
	private CannonController[] cannonControllers;
	private BodyController baseController;
	private AnimationController animationController;
	bool 	deployingAnimationStarted = false;
	
	public TurretUndeployingState(GameObject npc, CannonController[] cannonControllers, BodyController baseController, AnimationController animationController): base(npc)
	{
		stateID 					= StateID.UnDeploying;
		turretActor	 				= npc.GetComponent<SuperTurret>();
		this.cannonControllers 		= cannonControllers;
		this.baseController 		= baseController;
		this.animationController 	= animationController;
	}
	
	public override void DoBeforeEntering ()
	{
		base.DoBeforeEntering ();
		
		// See if we found new targets before undeploy turret
		//turretActor.SeekNewTargets();
	}

	public override void Reason(GameObject player)
	{	
		if(turretActor.Target == null && !deployingAnimationStarted)
			// Seek for new targets while turret is returning to its original orientation before undeploying animation starts
			turretActor.ChooseNewTarget();	
		
		if(turretActor.Target != null && !deployingAnimationStarted)
		{
			// While undeploying, turret get a new target, go to attacking state
			turretActor.GetMachineState().PerformTransition(Transition.TargetInRange);
		}else if((deployingAnimationStarted && !animationController.GetComponent<Animation>().isPlaying) || animationController == null)
		{
			// Turret undeployed, go to idle state
			turretActor.GetMachineState().PerformTransition(Transition.DeployingEnd);
		}
	}
		
	public override void Act(GameObject player)
	{
		// All cannons are in their original position ?
		bool allCannonsInPosition = true;
		foreach (CannonController cannon in cannonControllers) {
			if(cannon.enabled == true)
			{
				allCannonsInPosition = false;
				break;
			}
		}
		
		//We only play undeploy animation when baseController and cannonControllers have returned to his original orientation
		//this controllers  auto disable when they reach their original orientation
		if(animationController != null && !animationController.GetComponent<Animation>().isPlaying && baseController.enabled==false && allCannonsInPosition)
		{
			animationController.PlayAnimationBackward();
			
			deployingAnimationStarted=true;
		}
	}
	
	public override void DoBeforeLeaving ()
	{
		deployingAnimationStarted=false;
		base.DoBeforeLeaving ();
	}
}
