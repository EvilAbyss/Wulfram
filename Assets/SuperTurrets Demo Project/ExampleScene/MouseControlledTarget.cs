using UnityEngine;
using System.Collections;

public class MouseControlledTarget : MonoBehaviour {
	
	public Transform rotationPivot;
	
	public SuperTurret	targetTurret;
	
	public float	 mouseSensivity = 1f;
	
	private Vector3 oldMousePosition;
	
	private Transform myTransform;
	
	// Use this for initialization
	void Start () {
	
		myTransform = transform;
	}
	
	// Update is called once per frame
	void Update ()
	{
		Vector3 currentPosition = Input.mousePosition;
		
		Vector3 direction = oldMousePosition - currentPosition;
		direction.Normalize();

		if (direction.x != 0)
			myTransform.RotateAround(rotationPivot.position,Vector3.down,mouseSensivity*direction.x);
		
		if (direction.y != 0)
			myTransform.RotateAround(rotationPivot.position,Vector3.forward,mouseSensivity*direction.y);
		
		if (Input.GetMouseButtonDown(0))
			targetTurret.ForceShot();
		
		
		oldMousePosition = currentPosition;
	}
}
