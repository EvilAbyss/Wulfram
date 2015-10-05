using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MBS;

public class StatusMessageTest : MonoBehaviour {

	string test = "";
	void OnGUI()
	{
		test = GUI.TextField(new Rect(50,10,250,25), test);
		if ( GUI.Button(new Rect(320, 10, 70, 25)  , "Submit") )
		{
			StatusMessage.Message = test;
			test = string.Empty;
		}
	}
	
}