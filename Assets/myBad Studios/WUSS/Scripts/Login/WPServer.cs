using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using MBS;

public class WPServer : MonoBehaviour {
	
	static WPServer _instance;
	static public WPServer Instance
	{
		get
		{
			if (null == _instance)
				_instance = GameObject.FindObjectOfType<WPServer>();
			if (null == _instance)
			{
				GameObject go = new GameObject("WPServer");
				_instance = go.AddComponent<WPServer>();
			}
			return _instance;
		}
	}
	
	static mbsStateMachine<WULServerState> serverState;
	static public mbsStateMachine<WULServerState> ServerState { get { if (null == serverState) InitServerState(); return serverState;}  set{ serverState = value;} }
	static public int GameID { get { return Instance.game_id; } }
	
	public string
		online_url = "http://www.mysite.com",
		offline_url = "http://localhost";
	
	public int 
		game_id = 1;
	
	public bool 
		online = false,
		print_response_headers = false;
	
	static public void ContactServer(
		string action, 
		string filepath,
		string wuss_kit,
		cmlData data = null,
		Action<CML> response = null,
		Action<cmlData> failedresponse=null
		)
	{
		Instance.StartCoroutine( CallServer(action, filepath, wuss_kit, data, response, failedresponse) );
	}
	
	static public IEnumerator CallServer(string action, string filepath, string wuss_kit, cmlData data, Action<CML> response, Action<cmlData> failedresponse)
	{
		if (null == data)
			data = new cmlData();
		
		if (data.String("gid") == string.Empty || data.Int("gid") < 0)
			data.Seti("gid", GameID);
		
		data.Seti("unity",	1);
		data.Set ("action", action);
		data.Set ("wuss",	wuss_kit);
		
		
		string get = string.Empty;
		WWWForm f = new WWWForm();
		foreach(string s in data.Keys)
		{
			f.AddField(s, data.String(s) );
			get += "&"+s+"="+data.String(s);
		}
		get = "?" + get.Substring(1);
		
		if (null == serverState)
			InitServerState();
		
		serverState.SetState(WULServerState.Contacting);

		WWW w = newWWW(Instance.URL(filepath)+get, f);
		yield return w;
		serverState.SetState(WULServerState.None);
		
		if (w.error != null)
		{
			StatusMessage.Message = "Connection error: " + w.error;
			if (null != failedresponse)
			{
				cmlData error = new cmlData();
				error.Set("message", w.error);
				failedresponse(error);
			}
		} else 
		{
			string result_string = w.text;
			
			int warning_index = result_string.IndexOf( "<br />" );
			if ( warning_index > 0)
			{
				string server_error = result_string.Substring(warning_index + 6);
				StatusMessage.Message = server_error;
				Debug.Log (server_error);
				
				result_string = result_string.Substring(0, warning_index );
			}
			result_string = Encoder.Base64Decode(result_string);
			
			CML results = new CML();
			results.ParseFile(result_string);
			
			if (Instance.print_response_headers)
				foreach(var x in w.responseHeaders)
					Debug.Log(x.Key + " = " + x.Value + " : " + x.GetType() ) ;
			
			if (action == WULActions.DoLogin.ToString() || action == WULActions.VerifyLogin.ToString())
			{
				WUCookie.ExtractCookie( w.responseHeaders );
			}
			else				
				if (action == WULActions.Logout.ToString())
			{
				WUCookie.ClearCookie();
				WUCookie.StoreCookie();
			}
			
			if (results.Count == 0)
			{
				StatusMessage.Message = "No results returned";
				if (null != failedresponse)
				{
					cmlData error = new cmlData();
					error.Set("message", "No results returned");
					failedresponse(error);
				}
			} else
			{
				//should only ever be one but for the sake of demonstration, let's test for multiple...
				List<cmlData> errors = results.NodesWithField("success", "false");
				if (null != errors)
				{
					if (action != WULActions.VerifyLogin.ToString())
					{
						if (null != failedresponse)
						{
							foreach(cmlData error in errors)
							{	
								StatusMessage.Message =  "Error: " + error.String("message");
								cmlData _error = new cmlData();
								_error.Set("message", error.String("message"));
								failedresponse(_error);
							}
						}
					}
				} else 
					//if there were no errors, pass the resuls along to the response delegate, if any
				{
					if (action == WULActions.FetchUserEmail.ToString() )
					{
						int i = results.GetFirstNodeOfTypei("LOGIN");
						results[i].Set("gravatar_type", data.String("gravatar_type"));
					}
					if (null != response)
					{
						response(results);
					}
				}
			}
		}			
	}
	
	public string URL(string filename)
	{
		return (online ? online_url : offline_url) + "/wp-content/plugins/" + filename;
	}
	
	static public WWW newWWW(string URL, WWWForm data, bool logged_in = true)
	{
		WWW w = null;
		switch(Application.platform)
		{
		case RuntimePlatform.WindowsWebPlayer:
		case RuntimePlatform.OSXWebPlayer:
		case RuntimePlatform.IPhonePlayer:
		case RuntimePlatform.Android:
			w = new WWW(URL);
			break;
			
		default:
			if (logged_in)
				w = new WWW(URL, data.data, WUCookie.Cookie);
			else
				w = new WWW(URL, data.data);
			break;
		}
		return w;
	}
	
	static public void InitServerState()
	{
		if (null != serverState) return;
		
		serverState =  new mbsStateMachine<WULServerState>();
		serverState.AddState(WULServerState.None);
		serverState.AddState(WULServerState.Contacting, ShowPleaseWait);
		serverState.SetState (WULServerState.None);
	}
	
	static public void ShowPleaseWait()
	{
		PleaseWait.Draw();
	}
	
}
