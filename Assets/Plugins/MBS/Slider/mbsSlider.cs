using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MBS
{
public enum eSlideDirection	{Up=0, Down, Right, Left}
public enum eSlideState		{Closed=0, Opening, Opened, Closing}

/*	-- FUNCTION LIST -----------------------------------------

	void Init()
	public void Activate()
	public void Deactivate()
	public void Update ()
	public void FadeGUI(bool fade=true)

	-- PROPERTIES -------------------------------------------- 
	-- protected ---
	Rect			curPos;

	-- public ------
	Rect 							Pos	* read only
	float							alpha 
	mbsStateMachine<eSlideState>	slideState
	public Rect 					targetPos
	public bool 					Fade			
	public bool 					Slide			
	public eSlideDirection			slideInDirection
	public eSlideDirection			slideOutDirection
	public float					slideSpeed 		

	-- CALL BACKS -------------------------------------------- 
	public FunctionCall 	OnActivating;
	public FunctionCall 	OnActivated;
	public FunctionCall 	OnDeactivated;
	public FunctionCall 	OnDeactivating;

	-- FUNCTION LIST ----------------------------------------- 
	*/

[System.Serializable]
public class mbsSlider  {

	public Rect targetPos;
	public bool Fade							= true;
	public bool Slide							= true;
	public eSlideDirection	slideInDirection	= eSlideDirection.Right;
	public eSlideDirection	slideOutDirection	= eSlideDirection.Right;
	public float			slideSpeed 			= 300.0f;

	public Action 	OnActivating,
					OnActivated,
					OnDeactivated,
					OnDeactivating;

	protected Rect			curPos;

	public float			alpha {get; set; }
	public mbsStateMachine<eSlideState>	slideState {get; set; }
	
	public Rect Pos { get { return curPos; } }

	public void FadeGUI(bool fade=true) {

		Color temp = GUI.color;
		temp.a = fade ? alpha : 1;
		GUI.color = temp;
	}
	
	public mbsSlider() {
		InitStateMachine();
	}

	void InitStateMachine() {
		if ( null != slideState )
			return;
		slideState = new mbsStateMachine<eSlideState>();
		slideState.AddState(eSlideState.Closed);
		slideState.AddState(eSlideState.Opening, StateOpening);
		slideState.AddState(eSlideState.Opened);
		slideState.AddState(eSlideState.Closing, StateClosing);
		slideState.SetState(eSlideState.Closed);
		Deactivate(true);
	}
	
	// this function is called while the state is Opening
	void StateOpening() {
		bool isDone = false;
		switch (slideInDirection)
		{
			case eSlideDirection.Up :
				curPos.y -= slideSpeed * Time.deltaTime;
				if (curPos.y <= targetPos.y)
					curPos.y = targetPos.y;
				isDone = curPos.y == targetPos.y;
				break;

			case eSlideDirection.Down :
				curPos.y += slideSpeed * Time.deltaTime;
				if (curPos.y >= targetPos.y)
					curPos.y = targetPos.y;
				isDone = curPos.y == targetPos.y;
				break;

			case eSlideDirection.Right :
				curPos.x += slideSpeed * Time.deltaTime;
				if (curPos.x >= targetPos.x)
					curPos.x = targetPos.x;
				isDone = curPos.x == targetPos.x;
				break;

			case eSlideDirection.Left :
				curPos.x -= slideSpeed * Time.deltaTime;
				if (curPos.x <= targetPos.x)
					curPos.x = targetPos.x;
				isDone = curPos.x == targetPos.x;
				break;
		}

		DetermineAlpha();
		if (!isDone)
			isDone = (Fade && alpha == 1);

		if (isDone)
		{
			alpha = 1;
			curPos = targetPos;
			slideState.SetState(eSlideState.Opened);
			if (null != OnActivated)
				OnActivated();
		}
	}

	// this function is called while the state is Closing
	void StateClosing() {
		bool isDone = false;
		switch (slideOutDirection)
		{
			case eSlideDirection.Up :
				curPos.y -= slideSpeed * Time.deltaTime;
				isDone = curPos.y <= targetPos.y - targetPos.height;
				break;

			case eSlideDirection.Down :
				curPos.y += slideSpeed * Time.deltaTime;
				isDone = curPos.y >= targetPos.y + targetPos.height;
				break;

			case eSlideDirection.Right :
				curPos.x += slideSpeed * Time.deltaTime;
				isDone = curPos.x >= targetPos.x + targetPos.width;
				break;

			case eSlideDirection.Left :
				curPos.x -= slideSpeed * Time.deltaTime;
				isDone = curPos.x <= targetPos.x - targetPos.width;
				break;
		}
		DetermineAlpha();
		if (!isDone)
			isDone = (Fade && alpha == 0);

		if (isDone) {
			Init();
			alpha = 0;
			slideState.SetState(eSlideState.Closed);
			if (null != OnDeactivated)
				OnDeactivated();
		}
	}

	//set the menu to start at a proper offset to allow for smooth fading
	// also, prepare the state machine
	public void Init()
	{
		InitStateMachine();
		switch(slideInDirection)
		{
			case eSlideDirection.Up		:
				curPos = new Rect(targetPos.x,targetPos.y + targetPos.height,targetPos.width,targetPos.height);
				break;

			case eSlideDirection.Down	: 
				curPos = new Rect(targetPos.x,targetPos.y - targetPos.height,targetPos.width,targetPos.height);
				break;

			case eSlideDirection.Right	: 
				curPos = new Rect(targetPos.x - targetPos.width,targetPos.y,targetPos.width,targetPos.height);
				break;

			case eSlideDirection.Left	: 
				curPos = new Rect(targetPos.x + targetPos.width,targetPos.y,targetPos.width,targetPos.height);
				break;
		}
	}
	
	//start the process of fading in
	public void Activate(bool force = false)
	{
		//force trigger "OnActivated" but skip "OnActivating"
		if (force) {
			curPos = targetPos;
			slideState.SetState(eSlideState.Opening);
			return;
		}
		
		if (slideState.CompareState(eSlideState.Closed) || slideState.CompareState(eSlideState.Closing) ) {
			slideState.SetState(eSlideState.Opening);
			if (null != OnActivating)
				OnActivating();

			if (!Slide)
				curPos = targetPos;
		}
	}
	
	//start the process of fading out
	public void Deactivate(bool force = false)
	{
		if (force) {
			curPos = targetPos;
			switch(slideOutDirection)
			{
				case eSlideDirection.Up:	curPos.y -= targetPos.height; break;
				case eSlideDirection.Down:	curPos.y += targetPos.height; break;
				case eSlideDirection.Left:	curPos.x -= targetPos.width; break;
				case eSlideDirection.Right:	curPos.x += targetPos.width; break;
			}
			slideState.SetState(eSlideState.Closing);
			return;
		}
		if (slideState.CompareState(eSlideState.Opened) || slideState.CompareState(eSlideState.Opening) ) {
			slideState.SetState(eSlideState.Closing);
			if (null != OnDeactivating)
				OnDeactivating();

			if (!Slide) {
				curPos = targetPos;
					
				switch(slideOutDirection)
				{
					case eSlideDirection.Up:	curPos.y -= targetPos.height; break;
					case eSlideDirection.Down:	curPos.y += targetPos.height; break;
					case eSlideDirection.Left:	curPos.x -= targetPos.width; break;
					case eSlideDirection.Right:	curPos.x += targetPos.width; break;
				}
			}
		}
	}
	
	public void ForceState(eSlideState new_state) {
		slideState.SetState(new_state);
		switch(slideState.CurrentState) {
			case eSlideState.Closed:	Init(); break;
			case eSlideState.Opened:	curPos = targetPos; break;
		}
	}
	
	void DetermineAlpha()
	{
		if (!Fade)
		{
			alpha = 1;
			return;
		}
		
		float dist = 0;
		float perc = 0;

		//determine wether themenu is sliding in or out to determine which axis to use for alpha testing
		eSlideDirection slideDirection = (slideState.CompareState(eSlideState.Opening)) ? slideInDirection : slideOutDirection;
		switch (slideDirection)
		{
			case eSlideDirection.Up		:
			case eSlideDirection.Down	:
					dist = targetPos.y - curPos.y;
					break;
					
			case eSlideDirection.Right	: 
			case eSlideDirection.Left	:
					dist = targetPos.x - curPos.x;
					break;
		}
		
		//speed up fading so the menu fades in half the travel distance
		if (dist < 0) dist *= -1;
		if (dist != 0)
			dist *= 2;
		
		//test the difference between current pos and target location
		//target location being the position the menu should display at
		//or the position right next to it, offset by exactly the size of the menu
		//return the distance as a percentage and set alpha to that percentage
		switch (slideDirection)
		{
			case eSlideDirection.Up:
			case eSlideDirection.Down:
				perc = 1 - (dist / targetPos.height);
				break;

			case eSlideDirection.Right:
			case eSlideDirection.Left:
				perc = 1 - (dist / targetPos.width);
				break;
		}
		
		if (perc > 1) perc = 1;
		if (perc < 0) perc = 0;
		alpha = perc;
	}
	
	public void Update ()
	{
		slideState.PerformAction();
	}
}
}
