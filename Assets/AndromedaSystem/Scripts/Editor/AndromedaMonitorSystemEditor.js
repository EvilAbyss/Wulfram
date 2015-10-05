//Andromeda Monitor System Editor JavaScript Version 1.0 @ Copyright
//Black Horizon Studios

@CustomEditor (AndromedaMonitorSystem)
class AndromedaMonitorSystemEditor extends Editor {

    function OnInspectorGUI () {
    
        var thirdOfScreen : float = Screen.width/3-10;
		var sizeOfHideButtons : int = 18;
	
    	//Time Number Variables
    	EditorGUILayout.LabelField("Andromeda Monitor System", EditorStyles.boldLabel);
		EditorGUILayout.LabelField("By: Black Horizon Studios", EditorStyles.label);
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Hologram Options", EditorStyles.boldLabel);
		
		EditorGUILayout.Space();
        
        target.useLight = EditorGUILayout.Toggle ("Use Light?",target.useLight);
        
        EditorGUILayout.Space();
        
        if (target.useLight)
        {
        	EditorGUILayout.HelpBox("While 'Use Light' is checked Andromeda will flicker a light based off the filcker speed.", MessageType.Info, true);
	        //Added support for flickering light, light flickering intensity based off static/flicker intensity
	        var hologramLight : boolean = !EditorUtility.IsPersistent (target);
	        target.hologramLight = EditorGUILayout.ObjectField ("Hologram Light", target.hologramLight, Light, hologramLight);
        }
        
        EditorGUILayout.Space();
        
        target.useSound = EditorGUILayout.Toggle ("Use Sound?",target.useSound);
        
        EditorGUILayout.Space();
        
        if (target.useSound)
        {
        	EditorGUILayout.HelpBox("While 'Use Sound' is checked Andromeda will play a flickering sound based off the flicker speed.", MessageType.Info, true);
	        //Added support for flickering sound
	        var hologramSound : boolean = !EditorUtility.IsPersistent (target);
	        target.hologramSound = EditorGUILayout.ObjectField ("Hologram Sound", target.hologramSound, AudioClip, hologramSound);
        }
        
        EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		var hologramModel1 : boolean = !EditorUtility.IsPersistent (target);
        target.hologramModel1 = EditorGUILayout.ObjectField ("Monitor Static", target.hologramModel1, GameObject, hologramModel1);
		var hologramModel2 : boolean = !EditorUtility.IsPersistent (target);
        target.hologramModel2 = EditorGUILayout.ObjectField ("Monitor Texture", target.hologramModel2, GameObject, hologramModel2);
		
		EditorGUILayout.Space();
    	
    	target.shakeIntensity = EditorGUILayout.Slider ("Shake Intensity", target.shakeIntensity, 0.0, 0.6);
    	
    	//target.shakeIntensity = EditorGUILayout.IntField ("Shake Intensity", target.shakeIntensity);
    	EditorGUILayout.Space();
    	
    	target.minFlicker = EditorGUILayout.Slider ("Min Static", target.minFlicker, -0.5, 0.5);
    	target.maxFlicker = EditorGUILayout.Slider ("Max Static", target.maxFlicker, 0.0, 2.0);
    	
    	EditorGUILayout.Space();
    	
    	target.xSpeed = EditorGUILayout.Slider ("Static Speed X", target.xSpeed, 0.1, 5.0);
    	target.ySpeed = EditorGUILayout.Slider ("Static Speed Y", target.ySpeed, 0.0, 5.0);

		
		EditorGUILayout.Space();

        
        GUILayout.BeginHorizontal();
        

        GUILayout.EndHorizontal();
    }

}