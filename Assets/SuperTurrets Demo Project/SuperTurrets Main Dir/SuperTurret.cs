using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

/// <summary>
/// Main turret actor. We define here the turret machine state.
/// </summary>
[AddComponentMenu("SuperTurrets/SuperTurret Main Script")]
public class SuperTurret : MonoBehaviour
{
	/// <summary>
	/// This enum specifies the level of precission for visibility tests.
	/// None: 		No visibility test is done, if enemy is inside turret area, turret will attack it
	/// Simple: 	One ray is used to determine if turret can see enemy,
	/// Detailed: 	Each cannon launchs a ray to do a visibility test with the enemy.
	/// </summary>
	public enum VisibilityPrecissionLevel
	{
		None,
		Simple,
		Detailed
	}
	
	#region PUBLICS
	
	/// <summary>
	/// Use to show debug info in editor window
	/// </summary>
	public bool 					debugMode;
	
	/// <summary>
	/// GameObjects with tags that this turret will attack.
	/// </summary>
	public string[] 				targetTags = new string[0];
	
	/// <summary>
	/// Turret base moving velocity to target an enemy.
	/// </summary>
	public float 					bodyTargetingVelocity;
	
	/// <summary>
	/// Turret cannons moving velocity to target an enemy.
	/// </summary>
	public float 					cannonTargetingVelocity;
	
	/// <summary>
	/// The minimum distance at wich turret will stop targeting an enemy because is too close.
	/// </summary>
	public float 					minimumDistance;
	
	/// <summary>
	/// Max angle that cannon can handle to target an enemy. From 0 to 360.
	/// Use less of 90Âº if you want turrent do not target enemies below it. Use 360Âº if you want a turret that can attack enemies in spherical space
	/// </summary>
	public float 					cannonMaxAngle	= 120f;
	
	/// <summary>
	/// If this var is assigned, turret will ignore current targets and the detected new ones, and will attack to the assigned game object.
	/// </summary>
	public GameObject				manualTarget;
	
	/// <summary>
	/// Priority used by turret to choose between targets.
	/// </summary>
	public AbstractTargetPriority	targetPriority;
	
	/// <summary>
	/// If false, turret will change current target when another target is availible if this new target has a higher priority according to target priority algorithm.
	/// If true, turret only will change target when the current one dies.
	/// </summary>
	public bool 					waitUntilCurrentTargetDies = false;
	
	/// <summary>
	/// Skip targets that are in turret range but are not visible due to obstacles between turret and target
	/// </summary>
	public bool						skipNotVisibleTargets	   = true;
	
	/// <summary>
	/// How many time the turret will maintain current target if is not visible and there is others availible targets
	/// </summary>
	public float					waitingTimeIfNotVisible    = 5f;
	
	/// <summary>
	/// Turret AI will shot at target if all conditions are OK, buy maybe, you want a turret wich shots are controlled by user.
	/// </summary>
	public bool						autoShot				   = true;
	
	/// <summary>
	/// Aux controller to know current status of deploy animation.
	/// </summary>
	public AnimationController 		animationController;
	 
	/// <summary>
	/// Enemies inside attacking area can be attacked.
	/// </summary>
	public Area 					attackArea;
	
	/// <summary>
	/// Enemies inside targeting area are targeted, but until they reach attacking area, turret can not shot.
	/// </summary>
	public Area 					targetingArea;
	
	/// <summary>
	/// Precision to do visibility tests
	/// </summary>
	public VisibilityPrecissionLevel visibilityLevel = VisibilityPrecissionLevel.None;
	
	// Controllers of this actor 
	
	/// <summary>
	/// Array with all cannon info.
	/// </summary>
	public CannonController[] 	cannonControllers = new CannonController[0];
	
	/// <summary>
	/// Controller with base behavior. Base movements is different to cannon movemenet.
	/// </summary>
	public BodyController 		bodyController;
	
	/// <summary>
	/// Store the best availbile target, sometimes, we are shotting to a target because the best availible one is not ready (is hide, out of angle, etc)
	/// </summary>
	public GameObject			bestCandidateTarget;
	
	/// <summary>
	/// Cached transform for optimizations.
	/// </summary>
	public  Transform			myTransform;
	
	// Public vars needed for custom editor
	public bool hasTargetingArea;
	public bool hasAnimation;
	public int  cannonsNumber = 1;
	public bool cannonsExpanded;
	public int  tagsNumber;
	public bool tagsExpanded;
	public bool customTargetPriority;

	#endregion
	
	#region PROPERTIES
	
	/// <summary>
	/// Turret target. Each turret can have only one target active.
	/// </summary>
	/// <value>
	/// The target.
	/// </value>
	public GameObject Target 
	{
		get 
		{
			return this.target;
		} 
		private set{
			if(value == null)
			{
				this.TargetTransform 	= null;
				ReadyToShoot 			= false;
			}
			else
			{
				this.TargetTransform 	= value.transform;
				
				this.tempEnemyComponent = value.GetComponent<AbstractEnemy>();
				
				if(tempEnemyComponent == null)
					Debug.LogWarning("Enemies must have attached an script implementing AbstractEnemy so turrets can know if enemy is dead");
			}
			
			this.target = value;
		}
	}

	/// <summary>
	/// Save Target transform for optimization purposes. Controllers use this property.
	/// </summary>
	public Transform TargetTransform  {get; private set;}
	
	/// <summary>
	/// True when target is inside attacking area so cannons can start to shoot
	/// </summary>
	public bool ReadyToShoot {get; private set;}
	
	#endregion
	
	#region PRIVATES
	
	private GameObject 			targetActor;
	private FSMSystem			machineState;
	private GameObject 			lastTarget;
	private GameObject 			target;
	private float				timeTargetNotReady = 0f;
	private float				targetSleepValue;
	
	/// <summary>
	/// The targets in attack area.
	/// </summary>
	private List<GameObject>	targetsInAttackArea 	= new List<GameObject>(); 
	
	/// <summary>
	/// The targets in targeting area.
	/// </summary>
	private List<GameObject>	targetsInTargetingArea 	= new List<GameObject>(); 
	
	/// <summary>
	/// Stores current AbstractEnemy component to avoid expensive GetComponent call
	/// </summary>
	private AbstractEnemy		tempEnemyComponent;
	
	#endregion
	
	#region MONOBEHAVIOR METHODS
	
	void Awake()
	{
		if (this.cannonControllers.Length > 0)
		{
			foreach (CannonController cannon in this.cannonControllers)
			{
				if(cannon != null)
					cannon.setTurretActor(this);
			}
		}
		else
			Debug.LogError("Cannon Controllers must have assigned one cannon at least");	
		

		if (this.bodyController != null)
			bodyController.setTurretActor(this);
		else
			Debug.LogError("Body Controller can not be null in TurretActor");
		
		if (this.attackArea == null)
			Debug.LogError("Attack Area can not be null in TurretActor");
		else
		{
			this.attackArea.SetImportantTags(targetTags);
		}
		
		if(this.targetingArea != null)
		{
			this.targetingArea.SetImportantTags(targetTags);
		}
		
		if (this.bodyTargetingVelocity == 0)
			Debug.LogError("Set a value diferent to 0 to Body Targeting Velocity");
		
		if (this.cannonTargetingVelocity == 0)
			Debug.LogError("Set a value diferent to 0 to Cannon Targeting Velocity");
				
		this.myTransform = this.transform;
		
		// Machine state
		CreateMachineState();
	}
	
	void OnEnable()
	{
		// Suscribe to Areas
		if (this.attackArea != null)
		{
			this.attackArea.TargetEnterEvent += this.TargetEnterHandler;
			this.attackArea.TargetExitEvent  +=	this.TargetExitHandler; 
		}
		
		if (this.targetingArea != null)
		{
			this.targetingArea.TargetEnterEvent += this.TargetEnterHandler;
			this.targetingArea.TargetExitEvent  += this.TargetExitHandler;
		}	
	}
	
	void OnDisable()
	{
		// Suscribe to Areas
		if (this.attackArea != null)
		{
			this.attackArea.TargetEnterEvent -= this.TargetEnterHandler;
			this.attackArea.TargetExitEvent  -=	this.TargetExitHandler; 
		}
		
		if (this.targetingArea != null)
		{
			this.targetingArea.TargetEnterEvent -= this.TargetEnterHandler;
			this.targetingArea.TargetExitEvent  -= this.TargetExitHandler;
		}		
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Update machine state
		this.machineState.CurrentState.Reason(this.Target);
		this.machineState.CurrentState.Act(this.Target);
			
		if(Target != null)
		{
			if((tempEnemyComponent!= null && tempEnemyComponent.IsDead()))
			{
				Target = null;
			}
			
			// Increment always, it will be setted to each frame target is ready
			timeTargetNotReady += Time.deltaTime;
			
			if((!ReadyToShoot) || (manualTarget != null && manualTarget != Target) || (bestCandidateTarget != null && bestCandidateTarget != Target))
			{
				// 1- We search new targets each frame if we are idle ReadyToShoot = false
				// 2- We search new targets each frame if we have a Target, but we are not ready for shot, try to choose a new target each frame
				// 3- We try to get a best target each frame if we have a best candidate but we are engaging a different one
				ChooseNewTarget();	
			}
		}
	}
	
	void OnDrawGizmos()
	{
		float height = 2f;
		Gizmos.DrawIcon(transform.position + Vector3.up * height,"SuperTurretGizmo.png",true);	
		
		if(debugMode)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawWireSphere(transform.position,attackArea.GetComponent<SphereCollider>().radius);
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(transform.position,minimumDistance);
			
			if(targetingArea != null)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawWireSphere(transform.position,targetingArea.GetComponent<SphereCollider>().radius);
			}
		}
	}
	
	/*void OnGUI()
	{
		GUI.color = Color.red;
		GUI.Label(new Rect(Screen.width/2f - 75f,Screen.height/2f - 20f,150,40),"SUPER TURRETS DEMO VERSION");	
	}*/
	
	#endregion
	
	
	#region TARGET MANAGEMENT
	
	/// <summary>
	/// A new target has entered in an Area, we add it to the propper list.
	/// </summary>
	public void TargetEnterHandler (GameObject newTarget, Area area)
	{
		AbstractEnemy enemy = newTarget.GetComponent<AbstractEnemy> ();
		
		if(enemy != null && enemy.IsDead())
			// Avoid dead enemies (Example: an enemy that already exist but is in a diying animation)
			return;
		
		if(area == targetingArea)
		{
			AddTarget(targetsInTargetingArea,targetingArea,newTarget);
		}else if(area == attackArea)
		{
			if(AddTarget(targetsInAttackArea,attackArea,newTarget))
				// We have to remove enemies from targeting area when enters in attack area
				RemoveTarget(targetsInTargetingArea,targetingArea,newTarget);
			
			if(Target != null && newTarget == Target)
				// Current target moves from targeting area to attack area
				// If we have a target in attack area, turret is ready for shoot
				ReadyToShoot = true;
			
			if(Target != null && newTarget != target && ReadyToShoot == false)
				// We are targeting an enemy, but we are not shooting to it
				// choose the new one
				ChooseNewTarget();
		}
		
		// If we have a target priority algorithm, each time a new target enters, we have to choose
		// the wich one with the highest priority
		if(!waitUntilCurrentTargetDies && targetPriority != null)
			ChooseNewTarget();
	}
	
	/// <summary>
	/// Targets exit from a turret Area. Can be the targeting area or attack area.
	/// </summary>
	public void TargetExitHandler (GameObject newTarget, Area area)
	{
		if(GameObjectInsideArea(newTarget,area))
			// IMPORTANT NOTE: Due to a bug in unity related with rigidbodies that fall asleep, sometimes the method OnTriggerExit is called.
			// so we have to be sure that the object is really inside the area before do a real TargetExitHandler execution.
			return;
		
		if(targetingArea != null && area == targetingArea)
		{
			RemoveTarget(targetsInTargetingArea,targetingArea,newTarget);
			
			if(newTarget == Target)
				// If the current target exit from  target area, we loose it
				Target = null;
		}else if(area == attackArea)
		{
			if(RemoveTarget(targetsInAttackArea,attackArea,newTarget))
			{
				// Enemies exiting from attack area enters in targeting area
				AddTarget(targetsInTargetingArea,targetingArea,newTarget);
				
				if(Target != null && newTarget == Target)
					// Current target moves from attack area to targeting area
					ReadyToShoot = false;
				
				if((newTarget == Target && targetingArea == null) || (targetPriority != null && targetingArea != null))
					// If the current target exit from  attack area there is not attack area, we loose it
					// Or we have a targetPriority, so we want to choose a new one with highest priorty
					Target = null;
				
				if(newTarget == bestCandidateTarget)
					// We only are interested in best candidate targets that are in shotting range
					bestCandidateTarget = null;
				
			}else
				Debug.LogError("An enemy is exiting from attacking area but it was not in attackingarea targets lists");
		}
	}
	
	/// <summary>
	/// Adds the target to the desired list.
	/// </summary>
	public bool AddTarget(List<GameObject> targetList, Area targetArea, GameObject newEnemy)
	{
		bool res = false;
		
		if(targetArea != null && !targetList.Contains(newEnemy))
		{
			targetList.Add(newEnemy);
			res = true;
		}
			
		return res;
	}
	
	/// <summary>
	/// Removes the target from the desired list.
	/// </summary>
	public bool RemoveTarget(List<GameObject> targetList, Area targetArea, GameObject newEnemy)
	{
		bool res = false;
		
		if(targetArea != null && targetList.Contains(newEnemy))
		{
			targetList.Remove(newEnemy);
			res = true;
		}
			
		return res;
	}
	
	/// <summary>
	/// Choose a propper target from all availible ones, the Target propertie will be setted.
	/// </summary>
	public void ChooseNewTarget()
	{	
		// Special case where we have a best candidate than the current one
		// see if now best candidate is ready to replace the current Target
		if(Target != null && bestCandidateTarget != null && bestCandidateTarget != Target)
		{
			if(IsTargetReady(bestCandidateTarget))
			{
				Target = bestCandidateTarget;
				ReadyToShoot = true;
				return;
			}
		}
		
		// Get a target from attack area first
		if(targetsInAttackArea.Count > 0)
		{
			// We have targets in attack area, choose one !
			GameObject candidate = GetTheBestTarget(targetsInAttackArea);
		
			if(candidate != null)
			{
				Target = candidate;
				// If we have a target in attack area, turret is ready for shot
				ReadyToShoot = true;
				
				return;
			}
		}
		
		// Get a target from targeting area if there is not a Target assigned yet
		if(targetingArea != null && targetsInTargetingArea.Count > 0 && (Target == null || (manualTarget != null && Target != manualTarget)))
		{
			// Choose a target from atrgeting area if there is no availible one in attack area
			GameObject candidateTarget = GetTheBestTarget(targetsInTargetingArea);
			
			if(candidateTarget != null)
			{
				ReadyToShoot = false;
				Target = candidateTarget;
			}
		}
	}
	
	/// <summary>
	/// From all availible targets this method will select he best one. The result will depends if manual target
	/// is assigned, if we have a priority algortihm, etc
	/// </summary>
	private GameObject GetTheBestTarget(List<GameObject> targets)
	{
		GameObject res = null;
		
		// Clean destroyed game objects from list before selecting a new one
		CleanList(targets);
		
		// ManualTarget has always preference
		// && targets.Contains(manualTarget)
		if(manualTarget != null && targets.Contains(manualTarget) && IsTargetReady(manualTarget))
			return manualTarget;
		
		if(targetPriority != null)
			// Get the best target based on target priority algorithm
			res = targetPriority.GetTarget(targets,IsTargetReady, ref bestCandidateTarget);
		else if(targetPriority == null)
		{
			for(int i = 0; i<targets.Count; i++)
			{
				GameObject candidate = targets[i];
				
				if(IsTargetReady(candidate))
				{
					// Get the first availible target if there is not priority algorithm assigned
					res = candidate;
					break;
				}
			}
		}
		
		return res;
	}
	
	/// <summary>
	/// Aux method to clean the list of destroyed GameObjects
	/// </summary>
	private void CleanList(List<GameObject> targets)
	{
		for(int i = 0; i<targets.Count; i++)
		{
			GameObject target = targets[i];
			
			if(target == null)
			{
				// Deleted or destroyed GameObject
				targets.RemoveAt(i);
				i--;
			}
		}
	}
	
	/// <summary>
	/// Target is ready when it meets with all conditions like distance, angle, visibility tests, etc.
	/// If checked target is the same as the current Target, Target will be setted to null
	/// </summary>
	public bool	IsTargetReady(GameObject newTarget)
	{
		bool targetReady = true;
		
		AbstractEnemy enemy = newTarget.GetComponent<AbstractEnemy> ();
		
		// 1- If it is an enemy, is dead ? We dont want aim to dead things ! (Maybe Zombies...)
		bool condition1 = ((enemy != null && !enemy.IsDead()) || enemy == null);
		// 2- Is inside shotting angle ?
		bool condition2 = condition1 && IsTargetInShottingAngle (newTarget);
		// 3- Is too close to the turret or too far?
		bool condition3 = condition2 && IsTargetInPropperDistance (newTarget);
		// 4 - There is an obstacle between target and turret ? 
		bool condition4 = condition3 && IsTargetVisible(newTarget);
		
		// All conditions must be true to get a valid target
		targetReady = condition4;
		
		if(skipNotVisibleTargets && !targetReady && newTarget == Target && timeTargetNotReady >= waitingTimeIfNotVisible)
		{
			timeTargetNotReady = 0f;
			// If target is not ready and  is the same as current Target, we null current one so turret will change state if there is
			// not more availible targets or it will change to another that is ready.
			Target = null;
		}else if(targetReady)
			timeTargetNotReady = 0f;
		
		return targetReady;
	}
	
	#endregion
	
	#region AUX METHODS
	
	/// <summary>
	/// Aux method that creates a machine state (FSM) to manage turret AI
	/// </summary>
	private void CreateMachineState ()
	{
		machineState = new FSMSystem(this.gameObject);
		
		TurretIdleState idleState = new TurretIdleState(this.gameObject);
		idleState.AddTransition(Transition.TargetInRange,StateID.Deploying);
		idleState.AddTransition(Transition.NoLife,StateID.Dead);
		
		TurretDeployingState deployingState = new TurretDeployingState(this.gameObject,this.animationController);
		deployingState.AddTransition(Transition.DeployingEnd,StateID.Attacking);
		deployingState.AddTransition(Transition.NoLife,StateID.Dead);
				
		TurretAttackingState attackingState = new TurretAttackingState(this.gameObject);
		attackingState.AddTransition(Transition.LostTarget,StateID.UnDeploying);
		attackingState.AddTransition(Transition.NoLife,StateID.Dead);
		
		TurretUndeployingState undeployingState= new TurretUndeployingState(this.gameObject,this.cannonControllers,this.bodyController,this.animationController);
		undeployingState.AddTransition(Transition.DeployingEnd,StateID.Idle);
		undeployingState.AddTransition(Transition.TargetInRange,StateID.Attacking);
		undeployingState.AddTransition(Transition.NoLife,StateID.Dead);
		
		TurretDeadState deadState = new TurretDeadState(this.gameObject);
		
		machineState.AddState(idleState);
		machineState.AddState(deployingState);
		machineState.AddState(attackingState);
		machineState.AddState(undeployingState);
		machineState.AddState(deadState);
	}
	
	/// <summary>
	/// This method is called when turret lost its current target and in the Start method. We use a distance test, to see of we have an enemy inside our area.
	/// </summary>
	public void SeekNewTargets()
	{
		// Clear current availible targets we are to search all the availible ones
		targetsInAttackArea.Clear();
		
		float targetingRadius 	= float.MinValue;
		
		if (targetingArea != null)
			targetingRadius 	= targetingArea.Radius;
		
		float attackRadius 		= attackArea.Radius;
		
		foreach (string availibleTag in targetTags)
		{
			foreach (GameObject candidate in GameObject.FindGameObjectsWithTag(availibleTag))
			{
				float sqrDistanceToTurret 	= Vector3.SqrMagnitude(candidate.transform.position-myTransform.position);
				
				float sqrAttackRadius		= (attackRadius*attackRadius);
				
				if(sqrDistanceToTurret <= sqrAttackRadius)
				{
					//Debug.Log("CANDIDATE: "+candidate+" inside attack area");
					TargetEnterHandler(candidate,attackArea);
				}
				else if ( sqrDistanceToTurret > sqrAttackRadius && sqrDistanceToTurret <= (targetingRadius*targetingRadius))
				{
					//Debug.Log("CANDIDATE: "+candidate+" inside targeting area");
					TargetEnterHandler(candidate,targetingArea);
				}
			}
		}
	}
	
	
	/// <summary>
	/// Is target inside the limits of points angle ?
	/// Usually, turrets can not shot below them.
	/// </summary>
	/// <returns>
	/// True if target can be pointed because it's inside angle points limits.
	/// </returns>
	private bool IsTargetInShottingAngle(GameObject target)
	{	
		bool res=true;

		Vector3 turretUp = this.myTransform.up;
		Vector3 targetDirection = target.transform.position-this.myTransform.position;
				
		float angle=Vector3.Angle(turretUp,targetDirection);
		
		if(angle > this.cannonMaxAngle)
		{
			res=false;
		}
		
		return res;
	}
	
	/// <summary>
	/// Is target so close to the turret that turret can aim it ? Is target too far away that we loose target?
	/// </summary>
	/// <returns>
	/// True if target can be shotted because it's in a propper distance.
	/// </returns>
	private bool IsTargetInPropperDistance(GameObject target)
	{
		// NOTE: All calculous made in SQR to gain performance
		
		float targetDistance 	= Vector3.SqrMagnitude(this.myTransform.position-target.transform.position);
		float minDistance		= this.minimumDistance*this.minimumDistance;
		float maxDistance		= GetMaxDistance();
		maxDistance 			*= maxDistance;
		
		return (targetDistance > minDistance) && targetDistance <= maxDistance;
	}
	
	/// <summary>
	/// Determines if the new target has a tag contained inside turret valid tags list.
	/// </summary>
	/// <returns>
	/// <c>true</c> If target has a valid tag, otherwise, <c>false</c>.
	/// </returns>
	/// <param name='newTarget'>
	/// Target with tag.
	/// </param>
	private bool IsValidTag(GameObject newTarget)
	{
		foreach (string tag in targetTags) {
			if(newTarget.CompareTag(tag))
			{
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Check if cannon has a clear visibility of target. Different test are made depending on current visibilityLevel and fromTurretCenter.
	/// Note: I have decided that if there is any kind of enemy between turret an its actual target, why not shot ? ;)
	/// </summary>
	/// <returns>
	/// <c>true</c> if target visible; otherwise, <c>false</c>.
	/// </returns>
	private bool IsTargetVisible(GameObject newTarget)
	{
		bool res = true;
		
		if (visibilityLevel == VisibilityPrecissionLevel.None)
			res = true;
		else
			// Simple test to see if there is obstacles between turret and enemy, usually used in idle state
			res = VisibilityTest(myTransform.position,newTarget.transform.position-myTransform.position,GetMaxDistance(),newTarget);
		
		return res;
	}
	
	/// <summary>
	/// Raycasting that returns true if the target is visible or the obstacle between turret and target is another enemie.
	/// </summary>
	public bool VisibilityTest(Vector3 origin, Vector3 direction, float maxDistance, GameObject target)
	{
		bool res = false;
		
		Ray targetingRay = new Ray(origin,direction);
			
		if (debugMode)
			Debug.DrawRay(targetingRay.origin,targetingRay.direction*maxDistance);
		
		RaycastHit hit = new RaycastHit();
		
		// Check raycast
		if (Physics.Raycast(targetingRay,out hit,maxDistance))
		{
			Transform auxTransform = hit.transform;
			if (auxTransform != target.transform)
			{
				//TODO: Asegurarme que se ignoran areas de otras torretas poniendo las areas en el layer ignore raycast
				
				// The obstacle between turret and enemy, is another enemy ?
				if (IsValidTag(auxTransform.gameObject))
					res = true;
			}else if(auxTransform == target.transform)
				res = true;
		}
		
		return res;
	}
	
	/// <summary>
	/// Gets the max distance and it depends if turret has targeting area or not and turret current state.
	/// </summary>
	private float GetMaxDistance()
	{
		float maxDistance = 0f;
		
		// Note: ReadyToShoot = true implies that there is an enemy in attack area, so we reduce distance
		// to attack distance for performance purposes
		
		if (targetingArea != null && !ReadyToShoot)
			maxDistance = targetingArea.Radius;
		else
			maxDistance = attackArea.Radius;
				
		return maxDistance;
	}
	
	/// <summary>
	/// Test if an object is inside an area. Needed because there is a bug in unity with rigidbodies when fall as sleep
	/// the method OnTriggerExit is called.
	/// </summary>
	private bool GameObjectInsideArea(GameObject target,Area area)
	{
		bool res = false;
		
		int layerMask = 1 << target.layer;
		
		foreach (Collider coll in Physics.OverlapSphere(area.transform.position,area.Radius,layerMask))
		{
			if (coll == target.GetComponent<Collider>())
			{
				res = true;
				break;
			}
		}
		
		return res;
	}
	
	#endregion
	
	
	# region PUBLIC METHODS
	
	/// <summary>
	/// Forces turret shot. Useful if you want turret shot by user action ignoring AI autoShot.
	/// </summary>
	public void ForceShot()
	{
		if (machineState.CurrentStateID == StateID.Attacking)
		{
			(machineState.CurrentState as TurretAttackingState).ForceShot(null);
		}
	}
	
	/// <summary>
	/// Gets actor state machine
	/// </summary>
	public FSMSystem GetMachineState()
	{
		return machineState;
	}
	
	#endregion
}