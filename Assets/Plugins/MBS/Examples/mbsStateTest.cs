using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MBS;

public class mbsStateTest : MonoBehaviour {
	public enum smTest { Opening, Idle, Closing, Closed }
	
	//in this example I want to use a state in Update and OnGUI so I create two variables
	mbsStateMachine<smTest> state;
	mbsStateMachine<smTest> guistate;
	
	//just some variables to make this example do something
	float offset = -500f;
	public Rect area;
	
	void Start()
	{
		//first I define the states for the Update function's variable
		//make sure to define every state that exists in the enum
		//if you don't want the state to do anything, just leave the function field empty
		state = new mbsStateMachine<smTest>();
		state.AddState(smTest.Opening, Opening);
		state.AddState(smTest.Idle);
		state.AddState(smTest.Closing, Closing);
		state.AddState(smTest.Closed);
		
		//now define the states for the OnGUI function's variable
		guistate = new mbsStateMachine<smTest>();
		guistate.AddState(smTest.Opening, DrawBasic);
		guistate.AddState(smTest.Idle	, DrawCloseButton);
		guistate.AddState(smTest.Closing, DrawBasic);
		guistate.AddState(smTest.Closed	, DrawRestartButton);
		
		//mbsStateMachine will always default to the first non-null state when you first
		//define the states so in this case state.Currentstate will be set to Opening
	}
	
	//this is all you need to do to make use of the state machine
	void Update()
	{
		state.PerformAction();
	}
	
	//this is all you need to do to make use of the state machine
	void OnGUI()
	{
		guistate.PerformAction();
	}
	
	//this function simply increases offset so it becomes visible.
	//once it reaches 0, it changes it's state to Idle
	void Opening()
	{
		offset += Time.deltaTime * 150f;
		if (offset >= 0)
		{
			offset = 0;
			state.SetState(smTest.Idle);
		}
	}
	
	//this function simply decreases offset so it becomes hidden.
	//once it reaches -500, it changes it's state to Closed
	void Closing()
	{
		offset -= Time.deltaTime * 150f;
		if (offset <= -500)
		{
			offset = -500;
			state.SetState(smTest.Closed);
			//you could simply set guistate here but for this example
			//I want to demonstrate CompareState so I will do it in DrawBasic instead
		}
	}
	
	//draws the rect on screen so you can see the results of this script
	void DrawWindow(string text)
	{
		GUI.Box(area, text);
	}
	
	//this will draw when state is Opening or Closing
	//when it reaches the limits of it's slide areas, it will change the guistate
	//and in doing so it will no longer draw the window
	void DrawBasic()
	{
		area.x = offset;
		DrawWindow("This box is sliding");
		
		if (state.CompareState(smTest.Idle))
			guistate.SetState(smTest.Idle);
		
		if (state.CompareState(smTest.Closed))
			guistate.SetState(smTest.Closed);
	}
	
	//Only while the window is fully open will this be displayed.
	//It will draw the window and a button to press to start the closing animation
	void DrawCloseButton()
	{
		DrawWindow("This box is displaying");
		if (GUI.Button(new Rect(0,0,200,25), "Close this window"))
		{
			state.SetState(smTest.Closing);
			guistate.SetState(smTest.Closing);
		}
	}
	
	
	void DrawRestartButton()
	{
		if (GUI.Button(new Rect(0,0,200,25), "Slide window in"))
		{
			state.SetState(smTest.Opening);
			guistate.SetState(smTest.Opening);
		}
	}
	
}