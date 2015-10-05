using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace MBS
{
	public class mbsSlideTest : MonoBehaviour
	{
		public mbsSlider panel;

		void Start()
		{
			//prepare the underlaying state machine...
			panel.Init ();
		}

		void Update()
		{
			//this will calculate the appropriate position to draw content at
			panel.Update ();
		}

		void OnGUI()
		{
			//normally you would have your normal gui code here but in this
			//example I am drawing one of three GUI functions based on the
			//slide state, just to demonstrate the use of the underlaying
			//state machine
			switch (panel.slideState.CurrentState)
			{
			case eSlideState.Closing:
			case eSlideState.Opening:
				DrawBasic();
				break;

			case eSlideState.Opened: 
				DrawCloseButton();
				break;

			case eSlideState.Closed:
				DrawRestartButton();
				break;
			}
		}

		//draw my actual content
		void DrawWindow(string text)
		{
			panel.FadeGUI();
			GUI.Box(panel.Pos, text);
			panel.FadeGUI(false);
		}

		//draw only the box while sliding
		void DrawBasic()
		{
			DrawWindow("This box is sliding");
		}

		//while the slide operation is done, display the content
		//as well as a button to start the slide out animation
		void DrawCloseButton()
		{
			DrawWindow("This box is displaying");
			if (GUI.Button(new Rect(0,0,200,25), "Close this window"))
			{
				//start sliding out...
				panel.Deactivate();
			}
		}

		//while the rect is completely invisible, show a button to slide it back into view...
		void DrawRestartButton()
		{
			if (GUI.Button(new Rect(0,0,200,25), "Slide window in"))
			{
				//slide the rect back in again...
				panel.Activate();
			}
		}

	}
}