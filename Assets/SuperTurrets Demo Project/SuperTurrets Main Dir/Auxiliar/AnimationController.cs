using UnityEngine;
using System.Collections;

/// <summary>
/// Helper class to control turrets animations
/// </summary>
[AddComponentMenu("SuperTurrets/Animation Controller")]
public class AnimationController : MonoBehaviour {
	
	public float 		velocity = 1f;
	public Animation 	animationComponent;
	public string 		animationName;
	
	/// <summary>
	/// Usually animations does not start at frame 0. You have your model with a pose an then the first animation start at frame 0.
	/// Put here the first frame of your animation so turret appears on scene in the desired frame.
	/// </summary>
	public int 			firstAnimationFrame;
	
	private AnimationState animationState;
	
	void Awake()
	{
		if (animationComponent == null)
			Debug.Log("You must assign an animation component in: "+name+". If you dont want use animations, remove this component");
		
		animationState = animationComponent[animationName];
		
		// Put model in the first animation frame
		float initialTime = firstAnimationFrame/animationState.clip.frameRate;
		animationState.time = initialTime;
		animationState.clip.SampleAnimation(gameObject,initialTime);
	}
	
	// Use this for initialization
	
	/// <summary>
	/// Animation controller only is enabled if some animation is running.
	/// </summary>
	void Update ()
	{	
		if(!animationComponent.isPlaying)
			enabled=false;
	}
	
	/// <summary>
	/// Play animations of the gameObject forward
	/// </summary>
	public void PlayAnimationForward()
	{
		enabled=true;

    	animationState.speed = velocity;
		animationState.time = firstAnimationFrame/animationState.clip.frameRate;;
    	
		animationComponent.Play(animationState.name);
	}
	
	/// <summary>
	/// Play the animations of the game object backward
	/// </summary>
	public void PlayAnimationBackward()
	{
		enabled=true;

    	animationState.speed = -velocity;
		animationState.time = animationState.length;
    	
		animationComponent.Play(animationState.name);
	}
}
