using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace MBS {
	public enum WULServerState		{	None, Contacting }
	public enum WULStates			{	Dummy, LoginChallenge, AccountMenu, RegisterAccount, ValidateLoginStatus, LoginMenu, Logout, PasswordReset, PasswordChange, FetchAccountDetails, UpdateAccountDetails, AccountInfo, Count }
	public enum WULActions			{	DoLogin, SubmitRegistration, VerifyLogin, Logout, PasswordReset, PasswordChange, FetchAccountDetails, UpdateAccountDetails, FetchUserEmail }
	public enum WULGravatarTypes	{	MysteryMan, Identicon, Monsterid, Wavatar, Retro, Blank }

	public interface IWULogin
	{
		void RegisterAccount(cmlData fields);
		void ResetPassword(cmlData fields);
		void ChangePassword(cmlData fields);
		void AttemptAutoLogin();
		void LogOut();
		void AttemptToLogin(cmlData fields);
		void FetchPersonalInfo();
		void UpdatePersonalInfo(cmlData fields);
	}

	#region info
	/*
	  AVAILABLE RESPONSE DELEGATES
	  ----------------------------
		onRegistered
		onReset
		onPasswordChanged
		onLoggedIn
		onLoggedOut
		onAccountInfoReceived
		onInfoUpdated
	*/
	#endregion

	public class WULogin : MonoBehaviour, IWULogin {

		public bool
			fetch_account_id,
			fetch_username,
			fetch_display_name,
			fetch_email, 
			fetch_url, 
			fetch_registration,
			fetch_roles;

		public WULGravatarTypes
			gravatar_type = WULGravatarTypes.Wavatar;

		public string[]
			fetch_meta_info = new string[]{"nickname"};

		string FieldsToFetch
		{
			get
			{
				string result = "";
				foreach(string meta in fetch_meta_info)
					if (meta.Trim() != string.Empty)
						result += "," + meta;

				if (fetch_account_id)	result += ",user_id";
				if (fetch_username)		result += ",user_login";
				if (fetch_display_name) result += ",display_name";
				if (fetch_email)		result += ",user_email";
				if (fetch_url)			result += ",user_url";
				if (fetch_registration) result += ",user_registered";
				if (fetch_roles)		result += ",roles";
				if (result[0] == ',')	result = result.Substring(1);
				return result;
			}
		}

		static public int	 UID				{ get { return (null == fetched_info) ? 0  : fetched_info.Int("uid"); 				 } set {if (null == fetched_info) return; fetched_info.Seti("uid", value);} } 
		static public string display_name		{ get { return (null == fetched_info) ? "" : fetched_info.String("display_name");	 } set {if (null == fetched_info) return; fetched_info.Set("display_name", value);} } 
		static public string nickname			{ get { return (null == fetched_info) ? "" : fetched_info.String("nickname"); 		 } set {if (null == fetched_info) return; fetched_info.Set("nickname", value);}  } 
		static public string username			{ get { return (null == fetched_info) ? "" : fetched_info.String("user_login"); 	 } set {if (null == fetched_info) return; fetched_info.Set("user_login", value);}  } 
		static public string email				{ get { return (null == fetched_info) ? "" : fetched_info.String("user_email"); 	 } set {if (null == fetched_info) return; fetched_info.Set("user_email", value);}  } 
		static public string website			{ get { return (null == fetched_info) ? "" : fetched_info.String("user_url"); 		 } set {if (null == fetched_info) return; fetched_info.Set("user_url", value);}  } 
		static public string registration_date	{ get { return (null == fetched_info) ? "" : fetched_info.String("user_registered"); } set {if (null == fetched_info) return; fetched_info.Set("user_registered", value);}  } 
		static public string roles				{ get { return (null == fetched_info) ? "" : fetched_info.String("roles"); 			 } set {if (null == fetched_info) return; fetched_info.Set("roles", value);}  } 
		static public bool	 logged_in = false;

		static public cmlData fetched_info = null;

		static public Action<CML>
			onRegistered,
			onReset,
			onLoggedIn,
			onLoggedOut,
			onAccountInfoReceived,
			onInfoUpdated,
			onPasswordChanged;

		static public Action<cmlData>
			onLoginFailed,
			onLogoutFailed,
			onRegistrationFailed,
			onResetFailed,
			onAccountInfoFetchFailed,
			onInfoUpdateFail,
			onPasswordChangeFail;

		public System.Action<Texture2D>
			onProfileImageReceived;

		static public Texture2D
			user_gravatar;

		protected mbsSlider
			activePanel,
			nextPanel;

		static readonly string login_filepath = "wuss_login/unity_functions.php";
		static readonly string LOGINConstant = "LOGIN";

		public void RegisterAccount(cmlData fields)
		{
			WPServer.ContactServer(WULActions.SubmitRegistration.ToString(),login_filepath, LOGINConstant, fields, onRegistered, onRegistrationFailed);
		}
		
		public void ResetPassword(cmlData fields)
		{
			WPServer.ContactServer(WULActions.PasswordReset.ToString(),login_filepath, LOGINConstant, fields, onReset, onResetFailed);
		}

		public void ChangePassword(cmlData fields)
		{
			WPServer.ContactServer(WULActions.PasswordChange.ToString(),login_filepath, LOGINConstant, fields, onPasswordChanged, onPasswordChangeFail);
		}
		
		public void AttemptAutoLogin()
		{
			cmlData data = new cmlData();
			data.Set("wul_fields", FieldsToFetch);
			WPServer.ContactServer(WULActions.VerifyLogin.ToString(),login_filepath, LOGINConstant, data, onLoggedIn);
		}
		
		public void LogOut()
		{
			WPServer.ContactServer(WULActions.Logout.ToString(),login_filepath, LOGINConstant, null, onLoggedOut, onLogoutFailed);
		}
		
		public void AttemptToLogin(cmlData fields)
		{
			WUCookie.ClearCookie();
			WUCookie.StoreCookie();
			fields.Set("wul_fields", FieldsToFetch);
			WPServer.ContactServer(WULActions.DoLogin.ToString(),login_filepath, LOGINConstant, fields, onLoggedIn, onLoginFailed);
		}

		public void FetchProfileImage(System.Action<Texture2D> response)
		{
			FetchProfileImage(response, WULGravatarTypes.Identicon);
		}

		public void FetchProfileImage(System.Action<Texture2D> response, WULGravatarTypes gravatar_type)
		{
			onProfileImageReceived = response;

			if (null == onProfileImageReceived)
				return;

			cmlData data = new cmlData();
			data.Set("gravatar_type", gravatar_type.ToString());
			WPServer.ContactServer(WULActions.FetchUserEmail.ToString(),login_filepath, LOGINConstant, data, onProfileImageFetched);
		}

		public void FetchPersonalInfo()
		{
			WPServer.ContactServer(WULActions.FetchAccountDetails.ToString(),login_filepath, LOGINConstant, null, onAccountInfoReceived, onAccountInfoFetchFailed);
		}
		
		public void UpdatePersonalInfo(cmlData fields)
		{
			if (fields.String("descr") != string.Empty) 
				fields.Set ("descr", Encoder.Base64Encode(fields.String ("descr") ) );
			WPServer.ContactServer(WULActions.UpdateAccountDetails.ToString(),login_filepath, LOGINConstant, fields, onInfoUpdated, onInfoUpdateFail);
		}
		
		public void InitLoginSystem()
		{
			onLoggedIn += onLoginSuccess;
			onLoggedOut += onLogOutSuccess;
		}

		void onLoginSuccess(CML data)
		{
			fetched_info = data.GetFirstNodeOfType(LOGINConstant).Copy();
			fetched_info.Remove("success");
			logged_in = true;
			user_gravatar = null;

			if (email != string.Empty)
				FetchProfileImage(__SetProfileImage, gravatar_type);
		}

		void __SetProfileImage(Texture2D image)
		{
			user_gravatar = image;
		}

		virtual public void onLogOutSuccess(CML data)
		{
			user_gravatar = null;
			logged_in = false;
			fetched_info = null;
		}

		virtual public void onProfileImageFetched(CML data)
		{
			cmlData response = data[0];

			string gravatar_type_name = response.String("gravatar_type");
			WULGravatarTypes type = WULGravatarTypes.Blank;
			for (WULGravatarTypes t = WULGravatarTypes.MysteryMan; t < WULGravatarTypes.Blank; t++)
			{
				if (t.ToString().ToLower() == gravatar_type_name.ToLower())
					type = t;
			}
			StartCoroutine (ContactGravatar(response.String("gravatar"), type));
		}

		public IEnumerator ContactGravatar(string gravatar)
		{
			yield return ContactGravatar(gravatar, WULGravatarTypes.Identicon);
		}

		public IEnumerator ContactGravatar(string gravatar, WULGravatarTypes gravatar_type = WULGravatarTypes.Identicon)
		{

			string URL = "http://www.gravatar.com/avatar/"+gravatar+"?s=128&d="+ gravatar_type.ToString().ToLower();
			WWW w = new WWW(URL);
			yield return w;
			Texture2D avatar = null;
			if (w.error != null)
			{
				avatar = new Texture2D(1,1);
				avatar.SetPixel(0,0, Color.white);
				avatar.Apply();
			} else
			{
				avatar= w.texture;
			}
			onProfileImageReceived(avatar);
		}

	}

}
