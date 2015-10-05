using UnityEngine;
using System.Collections;

public class Command : MonoBehaviour {
	public Camera cam1;

	// Use this for initialization
	void Start () {
	cam1.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		// only be picked by the player!
		if (Input.GetKeyDown (KeyCode.V)) {

			cam1.enabled = !cam1.enabled;
			}
				}
	}


