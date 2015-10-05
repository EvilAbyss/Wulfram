using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MBS;

public class GUIXTest : MonoBehaviour {

	void Start()
	{
		GUIX.SetScreenSize(960,640);
	}

	void OnGUI()
	{
		//these blocks will always be the same size
		//and will position themselves relative to the screen
		GUI.Box(new Rect(0,0,200,200), "Top left");
		GUI.Box(new Rect(Screen.width - 200f, 0f,200f,200f), "Top right");

		//these blocks will always keep the same relative size to the screen
		//but will be distorted to fit the aspect ratio of the screen size you set
		//meaning the GUI elements will fit perfectly on an iPhone 3 or on an iPad Retina or anything inbetween...
		GUIX.FixScreenSize();
		GUI.Box(new Rect(0, GUIX.screenHeight - 200f, 200f, 200f), "Bottom left");
		GUI.Box(new Rect(GUIX.screenWidth - 200f, GUIX.screenHeight - 200f,200f,200f), "Bottom right");

		//or you could hardcode the values according to 960x640
		GUI.Box(new Rect(0, 220, 200f, 200f), "Middle left");
		GUI.Box(new Rect(760, 220,200f,200f), "Middle right");

		//the next two boxes use the exact same coordinates but are drawn at different positions!

		//this will not do what you expect...
		//(because you are using the Screen's coordinates even though you've fixed the GUI to a hard coded resolution)
		GUI.Box(new Rect((Screen.width /2f)-50f, (Screen.height /2f)-50f,100,100), "Wrong");

		//..but this will...
		//(because you reset the GUI to use the Screen's actual resolution)
		GUIX.ResetDisplay();
		GUI.Box(new Rect((Screen.width /2f)-50f, (Screen.height /2f)-50f,100,100), "Right");
	}
	
}