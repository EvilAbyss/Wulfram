using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace MBS {
	public class WUUGLoginGUI : WULogin {

		public enum eWULUGUIState {Inactive, Active, ContactingServer}

		static WUUGLoginGUI _instance;
		static public WUUGLoginGUI Instance
		{
			get
			{
				if (null == _instance)
				{
					WUUGLoginGUI[] objs = GameObject.FindObjectsOfType<WUUGLoginGUI>();
					if (null != objs && objs.Length > 0)
					{
						_instance = objs[0];
						for (int i = 1; i < objs.Length; i++)
							Destroy (objs[i].gameObject);
					} else
					{
						GameObject newobject = new GameObject("WUUGLoginGUI");
						_instance = newobject.AddComponent<WUUGLoginGUI>();
					}
				}
				return _instance;
			}
		}

		public bool
			attempt_auto_login = true;

		public string 
			error_invalid_email = "Please check email address: Invalid email format detected",
			error_all_fields_required = "All fields are required...",
			error_failed_verification = "Password mismatch",
			error_email_required = "Email is a required field",
			error_provide_current_password = "Please provide your current password",
			error_provide_new_password = "Please provide a new password",
			error_need_email_or_username = "Please enter either your username or your email to continue";

		
		public GameObject
			login_menu,
			login_screen,
			register_screen,
			password_reset_screen,
			password_change_screen,
			post_login_menu_screen,
			personal_info_screen;
		
		public InputField
			login_username,
			login_password,
			register_username,
			register_password,
			register_verify,
			register_email,
			pass_reset_username,
			pass_reset_email,
			pass_change_old,
			pass_change_new,
			pass_change_verify,
			personal_name,
			personal_surname,
			personal_display_name,
			personal_nickname,
			personal_aol,
			personal_yim,
			personal_jabber,
			personal_email,
			personal_url,
			personal_bio;
			
		[System.NonSerialized]
		public eWULUGUIState
			active_state;

		GameObject
			active_screen = null;

		void DisplayScreen(GameObject screen)
		{
			active_state = eWULUGUIState.Active;

			if (null != active_screen && screen != active_screen)
				active_screen.SetActive(false);
			active_screen = screen;
			active_screen.SetActive(true);
		}

		public void ShowPreLoginMenu()
		{
			DisplayScreen(login_menu);
		}

		public void ShowPostLoginMenu()
		{
			DisplayScreen(post_login_menu_screen);
		}

		void Start () 
		{
			if (this == Instance)
			{
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
				login_username.text = PlayerPrefs.GetString("username");
			}

			//if this script is loaded while already logged in, go to the account management menu or else show the login menu
			DisplayScreen( WULogin.logged_in ? post_login_menu_screen : login_menu);

			//setup all the actions that will take place when buttons are clicked
			SetupResponders();

			//if "Remember me" was selected during the last login, try to log in automatically...
			if (attempt_auto_login && !WULogin.logged_in )
				AttemptAutoLogin();
		}

		void SetupResponders()
		{
			onRegistered			+= OnRegistered;
			onLoggedIn				+= OnLoggedIn;
			onLoggedOut				+= OnLoggedOut;
			onReset					+= OnReset;
			onAccountInfoReceived 	+= OnAccountInfoReceived;
			onInfoUpdated			+= OnAccountInfoUpdated;
			onPasswordChanged		+= OnPasswordChanged;

			onAccountInfoFetchFailed += DisplayErrors;
			onInfoUpdateFail		+= DisplayErrors;
			onLoginFailed			+= DisplayErrors;
			onLogoutFailed			+= DisplayErrors;
			onPasswordChangeFail	+= DisplayErrors;
			onRegistrationFailed	+= DisplayErrors;
			onResetFailed			+= DisplayErrors;
		}

		void OnDestroy()
		{
			onRegistered			-= OnRegistered;
			onLoggedIn				-= OnLoggedIn;
			onLoggedOut				-= OnLoggedOut;
			onReset					-= OnReset;
			onAccountInfoReceived 	-= OnAccountInfoReceived;
			onInfoUpdated			-= OnAccountInfoUpdated;
			onPasswordChanged		-= OnPasswordChanged;
			
			onAccountInfoFetchFailed -= DisplayErrors;
			onInfoUpdateFail 		-= DisplayErrors;
			onLoginFailed			-= DisplayErrors;
			onLogoutFailed			-= DisplayErrors;
			onPasswordChangeFail	-= DisplayErrors;
			onRegistrationFailed	-= DisplayErrors;
			onResetFailed			-= DisplayErrors;
		}

		void DisplayErrors(cmlData error)
		{
			StatusMessage.Message = error.String("message");
		}

		public bool IsValidEmailFormat(string s)
		{
			string[] invalids = new string[8]{" ", "?", "|", "&", "%", "!", "<", ">"};
			
			s = s.Trim();
			int atIndex	= s.IndexOf("@");
			int lastAt	= s.LastIndexOf("@");
			int dotCom	= s.LastIndexOf(".");
			
			bool result = true;
			foreach(string str in invalids)
				if (s.IndexOf(str) >= 0)
					result = false;
			
			if (result) result = (atIndex > 0);
			if (result) result = (atIndex == lastAt);
			if (result)	result = (dotCom > atIndex + 1);
			
			return result;
		}

		public void OnToggleMeClicked(bool value)
		{
			attempt_auto_login = value;
		}


		#region Server contact
		public void DoLogin()
		{
			cmlData data = new cmlData();
			data.Set("username", login_username.text.Trim());
			data.Set("password", login_password.text.Trim());
			AttemptToLogin(data);
			DisplayScreen(login_menu);
		}

		public void DoResumeGame()
		{
			active_state = eWULUGUIState.Inactive;
			active_screen.SetActive(false);
		}

		public void DoRegistration()
		{
			if (register_email.text.Trim() == string.Empty || register_password.text.Trim() == string.Empty || register_username.text.Trim() == string.Empty)
			{
				StatusMessage.Message = error_all_fields_required;
				return;
			}
			if (register_verify.text.Trim() != register_password.text.Trim())
			{
				StatusMessage.Message = error_failed_verification;
				return;
			}
			if (!IsValidEmailFormat(register_email.text.Trim()))
		    {
				StatusMessage.Message = error_invalid_email;
				return;
			}

			cmlData data = new cmlData();
			data.Set("username", register_username.text.Trim());
			data.Set("email", register_email.text.Trim ());
			data.Set("password", register_password.text.Trim());
			RegisterAccount(data);
			DisplayScreen(login_menu);
		}

		public void DoFetchAccountInfo()
		{
			FetchPersonalInfo();
		}

		public void DoInfoUpdates()
		{
			cmlData data = new cmlData();

			if (personal_email.text != string.Empty)
			{
				if (!IsValidEmailFormat(personal_email.text.Trim()))
				{
					StatusMessage.Message = error_invalid_email;
					return;
				}
				data.Set("email", personal_email.text.Trim());
			}
			else
			{
				StatusMessage.Message = error_email_required;
				return;
			}
			data.Set("website", personal_url.text.Trim());
			data.Set("descr", personal_bio.text.Trim());
			data.Set("yim",personal_yim.text.Trim());
			data.Set("jabber",personal_jabber.text.Trim());
			data.Set("aim", personal_aol.text.Trim());
			data.Set("fname", personal_name.text.Trim());
			data.Set("lname", personal_surname.text.Trim());
			data.Set("nname", personal_nickname.text.Trim());
			data.Set("dname", personal_display_name.text.Trim());
			UpdatePersonalInfo(data);
			DisplayScreen(post_login_menu_screen);
		}
		
		public void DoPasswordChange()
		{
			if (pass_change_old.text.Trim() == string.Empty)
			{
				StatusMessage.Message = error_provide_current_password;
				return;
			}
			if (pass_change_new.text.Trim() == string.Empty)
			{
				StatusMessage.Message = error_provide_new_password;
				return;
			}
			if (pass_change_new.text.Trim() != pass_change_verify.text.Trim())
			{
				StatusMessage.Message = error_failed_verification;
				return;
			}

			cmlData data = new cmlData();
			data.Set ("password", pass_change_old.text.Trim());
			data.Set ("passnew", pass_change_new.text.Trim());
			ChangePassword(data);
			DisplayScreen(post_login_menu_screen);
		}
				
		public void DoPasswordReset()
		{
			pass_reset_email.text = pass_reset_email.text.Trim();
			if (pass_reset_email.text == string.Empty && pass_reset_username.text.Trim() == string.Empty)
			{
				StatusMessage.Message = error_need_email_or_username;
				return;
			}
			string login = pass_reset_email.text == string.Empty ? pass_reset_username.text.Trim(): pass_reset_email.text;
			if (pass_reset_email.text != string.Empty && !IsValidEmailFormat(pass_reset_email.text))
			{
				StatusMessage.Message = error_invalid_email;
				return;
			}
			cmlData data = new cmlData();
			data.Set("login", login);
			ResetPassword( data );
		}
		#endregion

		#region Server response handlers
		//upon successful login, the fields you requested to be returned are stored in cmlData fetched_info
		//and are left available to you until logout.
		virtual public void OnLoggedIn(CML _data)
		{
			//remember the "Remember me" choice...
			PlayerPrefs.SetInt("Remember Me", attempt_auto_login ? 1 : 0);
			if (attempt_auto_login)
				PlayerPrefs.SetString("username",login_username.text);

			//remove the password from the textfield
			login_password.text = "";

			//return to main menu and set it out of view...
			DisplayScreen(post_login_menu_screen);
			DoResumeGame();
		}

		virtual public void OnLoggedOut(CML data)
		{
			StatusMessage.Message = display_name + " logged out successfully";
			logged_in = false;
			nickname = display_name = string.Empty;
			DisplayScreen(login_menu);
		}

		virtual public void OnReset(CML data)
		{
			StatusMessage.Message = "Password reset emailed to your registered email address";
			DisplayScreen(login_menu);
			pass_reset_email.text = pass_reset_username.text = string.Empty;
		}

		virtual public void OnAccountInfoReceived(CML data)
		{
			personal_aol.text = data[0].String("aim");
			personal_bio.text = data[0].String("descr");
			personal_display_name.text = data[0].String("dname");
			personal_email.text = data[0].String("email");
			personal_jabber.text = data[0].String("jabber");
			personal_name.text = data[0].String("fname");
			personal_nickname.text = data[0].String("nname");
			personal_surname.text = data[0].String("lname");
			personal_url.text = data[0].String("website");
			personal_yim.text = data[0].String("yim");
			ShowAccountDetailsScreen();
		}

		virtual public void OnPasswordChanged(CML data)
		{
			pass_change_old.text = pass_change_new.text = pass_change_verify.text = string.Empty;
			OnLoggedOut(data);
			StatusMessage.Message = "Password successfully changed";
		}

		virtual public void OnRegistered(CML data)
		{
			StatusMessage.Message = "Registration successful...";
			DisplayScreen(login_screen);
		}

		virtual public void OnAccountInfoUpdated(CML data)
		{
			nickname = personal_nickname.text.Trim();
			display_name = personal_display_name.text.Trim();
			email = personal_email.text.Trim();
			website = personal_url.text.Trim();

			DisplayScreen(post_login_menu_screen);
		}
		#endregion

		#region ugui accessors
		public void ShowLoginScreen()
		{
			DisplayScreen(login_screen);
		}
		
		public void ShowLoginMenuScreen()
		{
			DisplayScreen(login_menu);
		}
		
		public void ShowRegisterScreen()
		{
			DisplayScreen(register_screen);
		}
		
		public void ShowPostLoginScreen()
		{
			DisplayScreen(post_login_menu_screen);
		}
		
		public void ShowPasswordResetScreen()
		{
			DisplayScreen(password_reset_screen);
		}
		
		public void ShowPasswordChangeScreen()
		{
			DisplayScreen(password_change_screen);
		}

		public void ShowAccountDetailsScreen()
		{
			DisplayScreen(personal_info_screen);
		}


		#endregion
	}
}
