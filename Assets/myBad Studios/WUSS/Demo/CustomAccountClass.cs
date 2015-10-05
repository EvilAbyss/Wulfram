using UnityEngine;
using System.Collections;
using MBS;

//This script demonstrates that you need never modify the LoginGUI class as it is merely a front end
//The underlaying WUServer sends out events that you can plug into and that way link your code with
//the login kit...

public class CustomAccountClass : MonoBehaviour {
	
	void Start()
	{
		WULogin.onLoggedIn += OnLoggedIn;
		WULogin.onLoggedOut += OnLoggedOut;
		WULogin.onLoginFailed += OnLoginFail;
	}

	void OnLoginFail(cmlData response)
	{
		Debug.Log("Error message from failed login: "+ response.String("message"));
	}

	void OnLoggedIn(object data)
	{
		Debug.Log ("Yeah! Logged in! Now I can load my level!");
	}

	void OnLoggedOut(object data)
	{
		Debug.Log("Oh, no, yo! Like in game over, yo. Time to load the main menu scene again");
	}
}
