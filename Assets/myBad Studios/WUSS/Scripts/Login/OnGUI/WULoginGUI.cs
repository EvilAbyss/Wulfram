using UnityEngine;
using System.Collections;

namespace MBS {
	public class WULoginGUI : WULogin {

		static WULoginGUI _instance;
		static public WULoginGUI Instance
		{
			get
			{
				if (null == _instance)
				{
					WULoginGUI[] objs = GameObject.FindObjectsOfType<WULoginGUI>();
					if (null != objs && objs.Length > 0)
					{
						_instance = objs[0];
						for (int i = 1; i < objs.Length; i++)
							Destroy (objs[i].gameObject);
					} else
					{
						GameObject newobject = new GameObject("WULoginGUI");
						_instance = newobject.AddComponent<WULoginGUI>();
					}
				}
				return _instance;
			}
		}

		public WULStates LoginState { get { return loginState.CurrentState; } set {loginState.SetState(value); } }
		
		mbsStateMachine<WULStates>
			loginState;

		public GUISkin
			the_skin;

		public mbsSlider
			displayArea;
		
		[SerializeField]
		public TabbedServerDataCapture
			account_info;

		public TabbedServerDataCapturePage
			login_challenge,
			registration_challenge,
			pass_reset_challenge,
			password_change_challenge;

		public bool
			destroy_prefab_on_login = false,
			attempt_auto_login,
			destroy_on_scene_change = false,
			fix_resolution = false;
		
		public float
			fixed_screen_width = 2048f,
			fixed_screen_height = 1536f;

		public Texture2D
			watermark;

		public Rect
			remember_me_area,
			watermark_area;

		void Start () 
		{
			if (this == Instance)
			{
				if (!destroy_on_scene_change)
					DontDestroyOnLoad(gameObject);

				InitWULoginGUI();
			}
		}

		virtual protected void InitWULoginGUI()
		{
			InitLoginSystem();

			WUCookie.LoadStoredCookie();
			if (PlayerPrefs.HasKey("Remember Me"))
			{
				attempt_auto_login = PlayerPrefs.GetInt("Remember Me",0) > 0;
				login_challenge.Entries[0].value = PlayerPrefs.GetString("username");
			}

			if (fix_resolution)
				GUIX.SetScreenSize(fixed_screen_width, fixed_screen_height);

			//the server's state can always be only one of two: Idling or awaiting a response
			//set this up and start in the idle state.
			WPServer.ServerState = new mbsStateMachine<WULServerState>();
			WPServer.ServerState.AddState(WULServerState.None);
			WPServer.ServerState.AddState(WULServerState.Contacting, ShowPleaseWait);
			WPServer.ServerState.SetState (WULServerState.None);

			//setup the various states our login kit could be in...
			loginState = new mbsStateMachine<WULStates>();
			loginState.AddState(WULStates.Dummy);
			loginState.AddState(WULStates.ValidateLoginStatus  );
			loginState.AddState(WULStates.Logout			   );
			loginState.AddState(WULStates.FetchAccountDetails  );
			loginState.AddState(WULStates.UpdateAccountDetails );
			loginState.AddState(WULStates.LoginMenu				, ShowLoginMenu);
			loginState.AddState(WULStates.AccountMenu			, ShowAccountMenu);
			loginState.AddState(WULStates.AccountInfo			, account_info.Draw);
			loginState.AddState(WULStates.LoginChallenge		, ShowLoginChallenge);
			loginState.AddState(WULStates.PasswordReset			, pass_reset_challenge.Draw);
			loginState.AddState(WULStates.RegisterAccount		, registration_challenge.Draw);
			loginState.AddState(WULStates.PasswordChange		, password_change_challenge.Draw);

			//if this script is loaded while already logged in, go to the account 
			//management menu or else show the login menu
			if (WULogin.logged_in)
				loginState.SetState(WULStates.AccountMenu);
			else
				loginState.SetState(WULStates.LoginMenu);

			//setup all the actions that will take place when buttons are clicked
			//in the OnGUI prefab...
			SetupResponders();

			//and finally, setup the window we will be displaying the stuff in and activate it
			displayArea.Init();
			displayArea.ForceState(eSlideState.Closed);
			displayArea.Activate();

			//if "Remember me" was selected during the last login, try to log in automatically...
			if (attempt_auto_login && loginState.CompareState(WULStates.LoginMenu) )
				AttemptAutoLogin();
		}

		void SetupResponders()
		{
			for (int i = 0; i < account_info.pages.Length; i++)
			{
				account_info.pages[i].onButtonClicked += AccountInfoButtonResponse;
				account_info.pages[i].AssignServerStateVariable(WPServer.ServerState);
			}

			pass_reset_challenge.onButtonClicked		+= PasswordResetButtonResponse;
			password_change_challenge.onButtonClicked	+= PasswordChangeButtonResponse;
			registration_challenge.onButtonClicked		+= RegistrationButtonResponse;
			login_challenge.onButtonClicked 			+= LoginButtonResponse;

			onRegistered			+= OnRegistered;
			onLoggedIn				+= OnLoggedIn;
			onLoggedOut				+= OnLoggedOut;
			onReset					+= OnReset;
			onAccountInfoReceived 	+= OnAccountInfoReceived;
			onInfoUpdated			+= OnAccountInfoUpdated;
			onPasswordChanged		+= OnPasswordChanged;

			displayArea.OnDeactivated = SetToDummy;
		}

		void OnDestroy()
		{
			for (int i = 0; i < account_info.pages.Length; i++)
				account_info.pages[i].onButtonClicked -= AccountInfoButtonResponse;

			pass_reset_challenge.onButtonClicked		-= PasswordResetButtonResponse;
			password_change_challenge.onButtonClicked	-= PasswordChangeButtonResponse;
			registration_challenge.onButtonClicked		-= RegistrationButtonResponse;
			login_challenge.onButtonClicked				-= LoginButtonResponse;

			onRegistered = 
			onLoggedIn =
			onLoggedOut =
			onAccountInfoReceived = 
			onInfoUpdated =
			null;
		}

		#region generic display code
		public void Update()
		{
			displayArea.Update();
		}

		void OnGUI()
		{
			if ( displayArea.slideState.CompareState(eSlideState.Closed) )
				return;

			GUI.skin = the_skin;

			//if currently waiting for a server response, show the please wait icon and quit this function...
			WPServer.ServerState.PerformAction();
			if (WPServer.ServerState.CompareState(WULServerState.Contacting))
				return;

			if (fix_resolution)
				GUIX.FixScreenSize();

			displayArea.FadeGUI();
			GUI.ModalWindow(0, displayArea.Pos, DrawWindow, "");
			displayArea.FadeGUI(false);

			if (fix_resolution)
				GUIX.ResetDisplay();
		}
		
		void DrawWindow(int id)
		{
			if (watermark)
				GUI.DrawTexture(watermark_area, watermark);
			loginState.PerformAction();			
		}

		virtual protected void ShowPleaseWait()
		{
			PleaseWait.Draw();
		}

		void SetToDummy()
		{
			LoginState = WULStates.Dummy;
		}

		#endregion

		virtual protected void ShowLoginChallenge()
		{
			login_challenge.Draw();
			attempt_auto_login = GUI.Toggle(remember_me_area, attempt_auto_login, " Remember Me");			
		}

		virtual protected void ShowLoginMenu()
		{
			if ( GUI.Button(new Rect(50,080, 300, 40), "Login") )
				loginState.SetState(WULStates.LoginChallenge);
			
			if ( GUI.Button(new Rect(50,130, 300, 40), "Register") )
				loginState.SetState(WULStates.RegisterAccount);
			
			if ( GUI.Button(new Rect(50,180, 300, 40), "Reset password") )
				loginState.SetState(WULStates.PasswordReset);
		}

		virtual protected void ShowAccountMenu()
		{
			if (GUI.Button(new Rect(50,050, 300, 40), "Resume..."))
				displayArea.Deactivate();
			
			if (GUI.Button(new Rect(50,100, 300, 40), "My Account"))
			{
				loginState.SetState(WULStates.AccountInfo);
				FetchPersonalInfo();
			}

			if (GUI.Button(new Rect(50,150, 300, 40), "Change password"))
				loginState.SetState(WULStates.PasswordChange);
			
			if (GUI.Button(new Rect(50,200, 300, 40), "Log out") && WPServer.ServerState.CompareState(WULServerState.None))
				LogOut();
		}

		virtual protected void LoginButtonResponse(string tab, string button)
		{
			//do not accept any input while waiting for a server response...
			if (WPServer.ServerState.CompareState(WULServerState.Contacting) )
				return;
			
			switch(button)
			{
			case "Cancel":
				loginState.SetState(WULStates.LoginMenu);
				break;
				
			case "Login":
				AttemptToLogin( login_challenge.ChallengeToCMLData( ) );
				break;
				
				
			}
		}
		
		virtual protected void RegistrationButtonResponse(string tab, string button)
		{
			//do not accept any input while waiting for a server response...
			if (WPServer.ServerState.CompareState(WULServerState.Contacting) )
				return;
			
			switch(button)
			{
			case "Cancel":
				loginState.SetState(WULStates.LoginMenu);
				break;
				
			case "Register":
				RegisterAccount( registration_challenge.ChallengeToCMLData( ) );
				break;
				
				
			}
		}
		
		virtual protected void AccountInfoButtonResponse(string tab, string button)
		{
			//do not accept any input while waiting for a server response...
			if (WPServer.ServerState.CompareState(WULServerState.Contacting) )
				return;
			
			switch (button)
			{
			case "Cancel":
				loginState.SetState(WULStates.AccountMenu);
				break;
				
			case "Update":
				if ( account_info.AllFieldsAreValid() )
					UpdatePersonalInfo( account_info.ChallengeToCMLData() );
				break;
			}
		}
		
		virtual protected void PasswordChangeButtonResponse(string tab, string button)
		{
			//do not accept any input while waiting for a server response...
			if (WPServer.ServerState.CompareState(WULServerState.Contacting) )
				return;
			
			switch (button)
			{
			case "Cancel":
				loginState.SetState(WULStates.AccountMenu);
				break;
				
			case "Continue":
				if ( password_change_challenge.ValidateChallengeData() )
					ChangePassword( password_change_challenge.ChallengeToCMLData() );
				break;
			}
		}
		
		virtual protected void PasswordResetButtonResponse(string tab, string button)
		{
			//do not accept any input while waiting for a server response...
			if (WPServer.ServerState.CompareState(WULServerState.Contacting) )
				return;
			
			switch (button)
			{
			case "Cancel":
				loginState.SetState(WULStates.LoginMenu);
				break;
				
			case "Reset":
				if ( pass_reset_challenge.ValidateChallengeData() )
					if (pass_reset_challenge.Entries[0].value == "" && pass_reset_challenge.Entries[1].value == "" )
						StatusMessage.Message = "Please enter either your username or your email to continue";
				else
					if ( pass_reset_challenge.ValidateChallengeData( ) )
						ResetPassword( pass_reset_challenge.ChallengeToCMLData( false ) );
				break;
			}
		}

		//upon successful login, the fields you requested to be returned are stored in cmlData fetched_info
		//and are left available to you until logout.
		virtual public void OnLoggedIn(CML _data)
		{
			//remember the "Remember me" choice...
			PlayerPrefs.SetInt("Remember Me", attempt_auto_login ? 1 : 0);
			if (attempt_auto_login)
				PlayerPrefs.SetString("username",login_challenge.Entries[0].value);

			//remove the password from the textfield
			login_challenge.Entries[1].value = string.Empty;

			//and slide it out of view...
			displayArea.Deactivate();

			//You can actually delete this prefab now if you wanted to since login is complete...
			//I prefer to keep it open so I can easily get back to account management but that is
			//a matter or personal choice and performance so do what you feel is best...
			if (destroy_prefab_on_login)
				Destroy (gameObject);
		}

		virtual public void OnLoggedOut(CML data)
		{
			StatusMessage.Message = display_name + " logged out successfully";
			logged_in = false;
			nickname = display_name = string.Empty;
			loginState.SetState(WULStates.LoginMenu);
		}

		virtual public void OnReset(CML data)
		{
			StatusMessage.Message = "Password reset emailed to your registered email address";
			loginState.SetState(WULStates.LoginMenu);
			pass_reset_challenge.Entries[0].value = pass_reset_challenge.Entries[1].value = string.Empty;
		}

		virtual public void OnAccountInfoReceived(CML data)
		{
			for(int p = 0; p < account_info.pages.Length; p++)
				for(int f = 0; f < account_info.pages[p].Entries.Length; f++)
					account_info.pages[p].Entries[f].value = data[0].String( account_info.pages[p].Entries[f].serverfield );
		}

		virtual public void OnPasswordChanged(CML data)
		{
			OnLoggedOut(data);
			StatusMessage.Message = "Password successfully changed";
		}

		virtual public void OnRegistered(CML data)
		{
			StatusMessage.Message = "Registration successful...";
			loginState.SetState(WULStates.LoginChallenge);
		}

		virtual public void OnAccountInfoUpdated(CML data)
		{
			nickname = account_info.pages[0].Entries[2].value;
			display_name = account_info.pages[0].Entries[3].value;

			loginState.SetState(WULStates.AccountMenu);
		}
	}
}
