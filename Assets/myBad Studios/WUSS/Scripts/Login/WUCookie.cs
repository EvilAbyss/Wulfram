using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MBS {

	public class WUCookie {

		static readonly string cookie_key = "wordpress_logged_in";


		#if UNITY_4_3 || UNITY_4_2 || UNITY_4_1 || UNITY_4_0_1 || UNITY_4_0 || UNITY_3_5 || UNITY_3_4 || UNITY_3_3 || UNITY_3_2
		static Hashtable			_cookie;
		static public Hashtable		Cookie
		{ 
			get
			{
				if (null == _cookie)
					_cookie = new Hashtable();
				return _cookie;
			}
			
			set
			{
				_cookie = value;
			} 
		}

		static public string CookieVal 
		{
			get
			{
				Hashtable h = Cookie;
				return (h.ContainsKey("Cookie"))
					? h["Cookie"] as string
						: string.Empty;
			}
			
			set
			{
				Cookie["Cookie"] = value;
			}
		}

		#else
		static Dictionary<string,string>	
			_cookie;

		static public Dictionary<string,string>
			Cookie { get { if (null == _cookie) _cookie = new Dictionary<string,string>(); return _cookie; } set { _cookie = value; } }

		static public string 
			CookieVal { get { return (Cookie.ContainsKey("Cookie")) ? Cookie["Cookie"] : string.Empty; } set { Cookie["Cookie"] = value; } }
		#endif

		static public void StoreCookie()
		{
			string value = CookieVal;
			if(string.Empty == value)
				PlayerPrefs.DeleteKey("Cookie");
			else
				PlayerPrefs.SetString("Cookie", value);
		}

		static public void LoadStoredCookie()
		{
			string s = PlayerPrefs.GetString("Cookie");
			CookieVal = s;
		}

		static public void ClearCookie()
		{
			_cookie = null;
		}
		
		static public bool 
			CookieIsSet	{ get { return null != CookieVal; } }

		static public void ExtractCookie(Dictionary<string,string> t,	 bool store = true)
		{
			if (t.ContainsKey("SET-COOKIE"))
			{
				string source = t["SET-COOKIE"] as string;
				string[] v = source.Split(';');
				foreach (string s in v)
				{
					if (string.IsNullOrEmpty(s)) continue;
					if (s.IndexOf(cookie_key) == 0)
					{
						CookieVal = s;
						if (store)
							StoreCookie();
						return;
					}
				}
			}
		}

	}
}