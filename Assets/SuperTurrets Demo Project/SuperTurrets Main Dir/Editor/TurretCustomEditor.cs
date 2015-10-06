using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SuperTurret))]
public class TurretCustomEditor : Editor {

	public override void OnInspectorGUI()
	{
		SuperTurret turret = target as SuperTurret;
		
		if (turret.bodyController == null)
			EditorGUILayout.HelpBox("You must assign a BodyController",MessageType.Error);
		
		// Assign required components
		turret.bodyController = EditorGUILayout.ObjectField("Body controller",turret.bodyController,typeof(BodyController),true) as BodyController;
		
		if (turret.cannonControllers != null && turret.cannonControllers.Length == 0)
			EditorGUILayout.HelpBox("You must assign a CannonController at least",MessageType.Error);
		
		if(turret.cannonControllers != null && turret.cannonControllers.Length >= 1 )
		{
			bool bad = true;
			foreach (CannonController controller in turret.cannonControllers)
			{
				if (controller != null)
				{
					bad = false;
					break;
				}
			}
			
			if(bad)
				EditorGUILayout.HelpBox("You must assign a CannonController at least",MessageType.Error);
		}
		
		InspectorCannons(turret);
		
		EditorGUILayout.Separator();
		
		// Has animation ? 
		turret.hasAnimation = EditorGUILayout.Toggle("Deploy animation ?",turret.hasAnimation);
		if(turret.hasAnimation)
		{
			if (turret.animationController == null)
				EditorGUILayout.HelpBox("You must assign an AnimationController",MessageType.Error);
			turret.animationController = EditorGUILayout.ObjectField("Animation controller: ",turret.animationController,typeof(AnimationController),true) as AnimationController;
		}
		else
			turret.animationController = null;
			
		
		EditorGUILayout.Separator();
		// Targeting area
		
		EditorGUILayout.HelpBox("If you want your turret start targeting enemies before it can shot them, enable targeting area.",MessageType.Info);
		turret.hasTargetingArea = EditorGUILayout.Toggle("Targeting area ?",turret.hasTargetingArea);
		
		InspectorTags(turret);
		 
		// Velocities
		turret.bodyTargetingVelocity = EditorGUILayout.Slider("Base point velocity",turret.bodyTargetingVelocity,10f,1500f);
		turret.cannonTargetingVelocity = EditorGUILayout.Slider("Cannon point velocity",turret.cannonTargetingVelocity,10f,1500f);
		
		// Cannon max angle
		turret.cannonMaxAngle = EditorGUILayout.Slider("Cannon max angle",turret.cannonMaxAngle,15f,360f);
		
		// Targeting priority
		EditorGUILayout.LabelField("Custom target selection priority ?");
		turret.customTargetPriority = EditorGUILayout.Toggle("Custom selection strategy",turret.customTargetPriority);
		if (turret.customTargetPriority)
		{
			EditorGUILayout.Separator();
			EditorGUILayout.HelpBox("You can implement different selection strategies. By default, turret attack the first targeted object.",MessageType.Info);
			turret.targetPriority = EditorGUILayout.ObjectField("Target selection strategy",turret.targetPriority,typeof(AbstractTargetPriority),true) as AbstractTargetPriority;
			
			// Change when a new target is availible
			EditorGUILayout.HelpBox("If not checked, turret will change current target when another target is availible if this new target has a higher priority according to target selection priority.If checked, turret only will change target when the current one dies.",MessageType.Info);
			turret.waitUntilCurrentTargetDies = EditorGUILayout.Toggle("Wait ?",turret.waitUntilCurrentTargetDies);
			
		}
		
		turret.visibilityLevel = (SuperTurret.VisibilityPrecissionLevel)EditorGUILayout.EnumPopup("Visibility accuaracy: ",turret.visibilityLevel);
		
		if(turret.visibilityLevel != SuperTurret.VisibilityPrecissionLevel.None)
		{
			// Wait time if not visible
			EditorGUILayout.HelpBox("If current target becomes not visible, how many time turret will be targeting it before change to another target",MessageType.Info);
			turret.waitingTimeIfNotVisible = EditorGUILayout.FloatField("Wait time if not visible",turret.waitingTimeIfNotVisible);
			
			// Skip not visible targets
			turret.skipNotVisibleTargets = EditorGUILayout.Toggle("Skip not visible targets",turret.skipNotVisibleTargets);
		}
		
		// Manual target
		turret.manualTarget= EditorGUILayout.ObjectField("Manual target",turret.manualTarget,typeof( GameObject),true) as GameObject;
		
		// Debug info
		turret.debugMode = EditorGUILayout.Toggle("Show debug info",turret.debugMode);
		
		if (GUI.changed)
			EditorUtility.SetDirty(turret);
	}
	
	public void OnSceneGUI()
	{
		SuperTurret turret = target as SuperTurret;
	
		// Set/unset targeting area to the turret
		TargetingArea(turret);
		AttackingArea(turret);
		// Minimum attack distance
		MinimumAttackDistance(turret);
		
		// Draw cannon angle
		if(turret.bodyController != null)
		{
			Handles.color = new Color(Color.white.r,Color.white.b,Color.white.b,0.2f);
			Handles.DrawSolidArc(turret.transform.position,turret.bodyController.transform.right*1,turret.bodyController.transform.up*5,turret.cannonMaxAngle,5);
		}
		
		// Manual Target
		/*Handles.BeginGUI();
			GUIContent content = new GUIContent("Manual target","If you set a manual target, turret will ignore all targetings criteria and will point to the manually asigned target");
			GUILayout.BeginArea(new Rect(Screen.width-320f,Screen.height-70f,300f,50f),content);
       		turret.manualTarget= EditorGUILayout.ObjectField("Manual target",turret.manualTarget,typeof( GameObject),true) as GameObject;
			GUILayout.EndArea();
        Handles.EndGUI();*/
		
		if (GUI.changed)
			EditorUtility.SetDirty(turret);
	}
	
	
	void InspectorCannons(SuperTurret turret)
	{
		turret.cannonsExpanded = EditorGUILayout.Foldout( turret.cannonsExpanded,"Cannons");

		if(turret.cannonsExpanded)
		{
			turret.cannonsNumber = EditorGUILayout.IntField("Number of cannons",turret.cannonsNumber);
			
			if(turret.cannonsNumber == 0)
				// 1 canon minimum
				turret.cannonsNumber = 1;
			
			if(turret.cannonControllers.Length != turret.cannonsNumber)
			{
				CannonController[] cannons  = new CannonController[turret.cannonsNumber];
				
				for (int x = 0; x< turret.cannonsNumber; x++)
				{
					if (turret.cannonControllers.Length > x)
						cannons[x] = turret.cannonControllers[x];
				}
				
				turret.cannonControllers = cannons;
			}
			
			for (int x =0; x < turret.cannonControllers.Length; x++)
			{
				turret.cannonControllers[x] = EditorGUILayout.ObjectField("Cannon "+x, turret.cannonControllers[x], typeof(CannonController),true) as CannonController;
			}
		}	
	}
	
	void InspectorTags(SuperTurret turret)
	{
		turret.tagsExpanded = EditorGUILayout.Foldout( turret.tagsExpanded,"Tags");

		if(turret.tagsExpanded)
		{
			EditorGUILayout.HelpBox("The turret will attack all enemies with a tag contained in your tag list. Wich target to attack first depends on your target selection strategy.",MessageType.Info);
			
			turret.tagsNumber = EditorGUILayout.IntField("Number of tags",turret.tagsNumber);
			
			if(turret.targetTags.Length != turret.tagsNumber)
			{
				string[] tags 		= new string[turret.tagsNumber];
				
				for (int x = 0; x< turret.tagsNumber; x++)
				{
					if (turret.targetTags.Length > x)
						tags[x] = turret.targetTags[x];
				}
				
				turret.targetTags = tags;
			}
			
			for (int x =0; x < turret.targetTags.Length; x++)
			{
				turret.targetTags[x] = EditorGUILayout.TagField("Enemy Tag "+x,turret.targetTags[x]);
			}
		}	
	}
	
	/// <summary>
	/// Control the minimum distance of turret to attack.
	/// </summary>
	/// <param name='turret'>
	/// Turret.
	/// </param>
	void MinimumAttackDistance(SuperTurret turret)
	{
		Handles.color = new Color(Color.blue.r,Color.blue.g,Color.blue.b,1f);
		
		if (turret.minimumDistance == 0)
		{
			if (turret.bodyController != null)
			{
				float width = turret.attackArea.GetComponent<SphereCollider>().radius / 4f;
				
				turret.minimumDistance = width;
			}
		}
		
		 turret.minimumDistance =   Handles.ScaleValueHandle(turret.minimumDistance,
                    turret.transform.position+(Vector3.right*turret.minimumDistance),
                    turret.transform.rotation,
                    2,
                    Handles.SphereCap,
                    2);
		
		 turret.minimumDistance =   Handles.ScaleValueHandle(turret.minimumDistance,
                    turret.transform.position+(Vector3.left*turret.minimumDistance),
                    turret.transform.rotation,
                    2,
                    Handles.SphereCap,
                    2);
		
		turret.minimumDistance = Mathf.Clamp(turret.minimumDistance,2,turret.attackArea.GetComponent<SphereCollider>().radius);
		
		//turret.minimumDistance = Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,turret.minimumDistance,false);
		Handles.color = new Color(Color.blue.r,Color.blue.g,Color.blue.b,0.05f);
		Handles.DrawSolidDisc(turret.transform.position,turret.transform.up,turret.minimumDistance);
		Handles.color = new Color(Color.blue.r,Color.blue.g,Color.blue.b,1f);
		
		Handles.Label(turret.transform.position+(Vector3.left*turret.minimumDistance),"Minimum distance");
		Handles.Label(turret.transform.position+(Vector3.right*turret.minimumDistance),"Minimum distance");
	}
	
	/// <summary>
	/// Create an attacking area the first time and manage attacking area radius.
	/// </summary>
	/// <param name='turret'>
	/// Turret.
	/// </param>
	void AttackingArea(SuperTurret turret)	
	{
		Handles.color = Color.red;
		SphereCollider sphereCollider;
		// Attacking area is needed
		if(turret.attackArea == null)
		{
			GameObject newAttackingArea 			 = new GameObject("AttakingArea");
			newAttackingArea.layer					 = LayerMask.NameToLayer("Ignore Raycast");
			newAttackingArea.transform.parent 		 = turret.transform;
			newAttackingArea.transform.localPosition = Vector3.zero;
			sphereCollider 			 			 	 = newAttackingArea.AddComponent<SphereCollider>();
			sphereCollider.isTrigger				 = true;
			sphereCollider.radius 					 = 10;
			Area scriptArea 						 = newAttackingArea.AddComponent<Area>();
			turret.attackArea 				 		 = scriptArea;
		}else{
			sphereCollider 						 	= turret.attackArea.GetComponent<Collider>() as SphereCollider;
		}
		
		sphereCollider.radius 						 = Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,sphereCollider.radius,true);
		
		// Attack area must be always major than minimum distance and minnor than tagreting area
		if (turret.hasTargetingArea)
			turret.attackArea.GetComponent<SphereCollider>().radius = Mathf.Clamp(turret.attackArea.GetComponent<SphereCollider>().radius,turret.minimumDistance,turret.targetingArea.GetComponent<SphereCollider>().radius);
		else
			turret.attackArea.GetComponent<SphereCollider>().radius = Mathf.Clamp(turret.attackArea.GetComponent<SphereCollider>().radius,turret.minimumDistance,float.MaxValue);
			
			
		
		Handles.color = new Color(Color.red.r,Color.red.g,Color.red.b,0.06f);
		Handles.DrawSolidDisc(turret.transform.position,turret.transform.up,sphereCollider.radius );
	
		Handles.Label(turret.transform.position+(Vector3.up*sphereCollider.radius),"Attacking Area");
		Handles.Label(turret.transform.position+(Vector3.left*sphereCollider.radius),"Attacking Area");
		Handles.Label(turret.transform.position+(Vector3.right*sphereCollider.radius),"Attacking Area");
		Handles.Label(turret.transform.position+(Vector3.down*sphereCollider.radius),"Attacking Area");
	}
	
	/// <summary>
	/// Add or remove targeting area to the turret.
	/// </summary>
	/// <param name='turret'>
	/// Turret.
	/// </param>
	void TargetingArea (SuperTurret turret)
	{
		if(turret.hasTargetingArea)
		{
			Handles.color = Color.yellow;
			SphereCollider sphereCollider;
			if(turret.targetingArea == null)
			{
				GameObject newTargetingArea 		= new GameObject("TargetingArea");
				newTargetingArea.layer				= LayerMask.NameToLayer("Ignore Raycast");
				newTargetingArea.transform.parent 	= turret.transform;
				newTargetingArea.transform.localPosition = Vector3.zero;
				sphereCollider 		= newTargetingArea.AddComponent<SphereCollider>();
				Area scriptArea 					= newTargetingArea.AddComponent<Area>();
				turret.targetingArea 				= scriptArea;
				sphereCollider.radius 				= turret.attackArea.GetComponent<SphereCollider>().radius+10;;
				sphereCollider.isTrigger			= true;
				sphereCollider.radius 				= Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,sphereCollider.radius,true);
			}else{
				sphereCollider 						= turret.targetingArea.GetComponent<Collider>() as SphereCollider;
				sphereCollider.radius 				= Handles.RadiusHandle(turret.transform.rotation,turret.transform.position,sphereCollider.radius,true);
			}
			
			// Targeting area mus be always major than attack area
			turret.targetingArea.GetComponent<SphereCollider>().radius = Mathf.Clamp(turret.targetingArea.GetComponent<SphereCollider>().radius,turret.attackArea.GetComponent<SphereCollider>().radius,float.MaxValue);
			
			Handles.color = new Color(Color.yellow.r,Color.yellow.g,Color.yellow.b,0.05f);
			Handles.DrawSolidDisc(turret.transform.position,turret.transform.up,sphereCollider.radius );
			
			Handles.Label(turret.transform.position+(Vector3.up*sphereCollider.radius),"Targeting Area");
			Handles.Label(turret.transform.position+(Vector3.left*sphereCollider.radius),"Targeting Area");
			Handles.Label(turret.transform.position+(Vector3.right*sphereCollider.radius),"Targeting Area");
			Handles.Label(turret.transform.position+(Vector3.down*sphereCollider.radius),"Targeting Area");
		}else{
			if(turret.targetingArea != null)
			{
				DestroyImmediate(turret.targetingArea.gameObject);
			}
		}
	}
}
