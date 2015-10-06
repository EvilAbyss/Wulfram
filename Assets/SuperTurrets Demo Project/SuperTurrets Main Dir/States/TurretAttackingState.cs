using UnityEngine;
using System.Collections;

/// <summary>
/// Turret is ready to shot and shot if it can.
/// </summary>
public class TurretAttackingState : FSMState {
	
	private SuperTurret 	turretActor = null;
	
	public TurretAttackingState(GameObject npc):base(npc)
	{
		stateID = StateID.Attacking;
		turretActor = npc.GetComponent<SuperTurret>();
	}
	
	public override void DoBeforeEntering ()
	{
		turretActor.bodyController.enabled=true;
		
		foreach (CannonController cannon in turretActor.cannonControllers)
		{
			if(cannon != null)
				cannon.enabled=true;
		}

		base.DoBeforeEntering ();
	}

	public override void Reason(GameObject target)
	{
		if(target==null)	
		{
			turretActor.GetMachineState().PerformTransition(Transition.LostTarget);
		}
	}
	
	public override void Act(GameObject newTarget)
	{
		if(newTarget != null)
		{
			turretActor.StartCoroutine(ShotCoroutine(newTarget,false));
		}
	}
	
	public void ForceShot(GameObject newTarget)
	{
		turretActor.StartCoroutine(ShotCoroutine(newTarget,true));
	}
	
	/// <summary>
	/// Shots all trurrets cannons. newTarget can be null, only is useful for bullets like missiles, that needs a target.
	/// </summary>
	/// <param name='forceShot'>
	/// If true, it will ignore visibility tests and turret will shot.
	/// </param>
	private IEnumerator ShotCoroutine(GameObject newTarget, bool forceShot)
	{
		if(forceShot || (turretActor.IsTargetReady(newTarget) && turretActor.ReadyToShoot))
		{
			if(!forceShot && turretActor.visibilityLevel == SuperTurret.VisibilityPrecissionLevel.Simple)
			{
				// Simple visibility test from center of all cannons to target.
				Vector3 origin 	= GetCenterOfShotPoints();
				Vector3 dir		= turretActor.cannonControllers[0].transform.forward;
				
				if(!turretActor.VisibilityTest(origin,dir,turretActor.attackArea.Radius,newTarget))
					yield break;
			}
				
			foreach (CannonController cannon in turretActor.cannonControllers)
			{
				if(cannon == null)
					yield break;
				
				if(!forceShot && cannon.cannonWeapon != null && !cannon.cannonWeapon.WeaponReady)
					continue;
					
				// Do a detailed visibility test for each cannon if needed.
				// Only cannons with a target in sight will shot 
				if(!forceShot && turretActor.visibilityLevel == SuperTurret.VisibilityPrecissionLevel.Detailed)
				{
					Vector3 origin 	= cannon.shotPoint.position;
					Vector3 dir		= cannon.transform.forward;
					
					if(!turretActor.VisibilityTest(origin,dir,turretActor.attackArea.Radius,newTarget))
						continue;
				}
				
				// In case we avoid raycasting, we have to check cannon angle to avoid cannons shotting to nothing
				if(!forceShot && turretActor.visibilityLevel == SuperTurret.VisibilityPrecissionLevel.None)
				{
					if(!cannon.IsCannonPointingTarget())
						continue;
				}
				
				// Fire at will !!!!!!!!
				cannon.cannonWeapon.Shot(cannon.shotPoint.position,newTarget,cannon.myTransform);
				
				// Recoil
				if(cannon.recoilController != null)
					cannon.recoilController.Enable();
					
				yield return new WaitForSeconds(cannon.shotDelay);
			}
		}
	}
	
	/// <summary>
	/// Gets the center of all shot points.
	/// </summary>
	private Vector3 GetCenterOfShotPoints()
	{
		float x=0,y=0,z=0;
		foreach (CannonController cannon in turretActor.cannonControllers)
		{
			x += cannon.myTransform.position.x;
			y += cannon.myTransform.position.y;
			z += cannon.myTransform.position.z;
		}
		
		float total = turretActor.cannonControllers.Length;
		return new Vector3(x/total,y/total,z/total);
	}
}
