using UnityEngine;
using System.Collections;
using MBS;

//This script demonstrates how you can pop the login prefab back into view again from your own code
//and make it show any menu you like...
public class DemoObjectUGUI : MonoBehaviour {

	public WUUGLoginGUI login;

	void Start()
	{
		WULogin.onLoggedIn += FindLogin;
		FindLogin(null);
	}

	void FindLogin(CML data)
	{
		login = FindObjectOfType<WUUGLoginGUI>();
	}

	void OnGUI()
	{
		if (null == login || !WULogin.logged_in || login.active_state != WUUGLoginGUI.eWULUGUIState.Inactive)
			return;

		if (GUI.Button(new Rect(Screen.width - 100, 0, 100, 30), WULogin.nickname))
		{
			login.ShowPostLoginMenu();
			//Done!
		}
	}

}
