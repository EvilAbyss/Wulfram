using UnityEngine;
using System.Collections;
using MBS;

//This script demonstrates how you can pop the login prefab back into view again from your own code
//and make it show any menu you like...
public class DemoObject : MonoBehaviour {
	
	void OnGUI()
	{
		if (WULoginGUI.Instance.LoginState != WULStates.Dummy)
			return;

		if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 30), WULogin.nickname))
		{
			//select the "I am logged in, show me my account info" menu to show
			WULoginGUI.Instance.LoginState = WULStates.AccountMenu;

			//then slide the window into view...
			WULoginGUI.Instance.displayArea.Activate();

			//Done!
		}
	}

}
