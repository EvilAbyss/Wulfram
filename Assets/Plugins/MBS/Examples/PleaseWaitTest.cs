using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MBS;

public class PleaseWaitTest : MonoBehaviour {

	void OnGUI()
	{
		PleaseWait.Draw();
		//That's it. No setup required, no prefab required, no nothing. 
		//Just make sure you have an image caleld "Spinner" inside a "Resources" folder
	}
	
}