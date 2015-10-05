using UnityEngine;
using System.Collections;
using System;

namespace MBS {

	[System.Serializable]
	public class TabbedServerDataCapturePage
	{
		public Action<string, string>
			onButtonClicked;

		public string 
			tabname;

		public string[]
			buttons;
		
		public ServerDataCapture[] 
			Entries;

		public bool 
			horizontal_button_layout;

		public Rect
			buttons_area;
	
		static mbsStateMachine<WULServerState>
			serverState;

		public void OnButtonClicked(string tab, string which)
		{
			if (null != onButtonClicked)
				onButtonClicked(tab, which);
		}

		public void AssignServerStateVariable(mbsStateMachine<WULServerState> state)
		{
			serverState = state;
		}

		public bool ValidateChallengeData()
		{
			return ValidateChallengeData (ref Entries);
		}

		public bool ValidateChallengeData(ref ServerDataCapture[] fields)
		{
			bool result = true;
			foreach(ServerDataCapture field in fields)
			{
				if (field.fieldType != eFieldType.Verify)
					result = field.ValidInput();
				else
					result = field.ValidInput( fields[ field.verify ] );
				
				if (!result)
					return false;
			}
			return true;
		}

		public void ShowChallengeFields()
		{
			ShowChallengeFields(ref Entries);
		}

		public void ShowChallengeFields(ref ServerDataCapture[] challenge_text)
		{
			for ( int i = 0; i < challenge_text.Length; i++)
			{
				GUI.Label(challenge_text[i].label_area, challenge_text[i].label);
				string val = "";
				if ( challenge_text[i].passwordChar == string.Empty )
				    val = GUI.TextField (challenge_text[i].value_area, challenge_text[i].value);
				else
					val = GUI.PasswordField (challenge_text[i].value_area, challenge_text[i].value, challenge_text[i].passwordChar[0]);
				if (serverState.CompareState(WULServerState.None))
					challenge_text[i].value = val;
			}
		}

		public string DrawButtons()
		{
			if (null == buttons || buttons.Length == 0)
				return string.Empty;

			float
				width_gap = (buttons_area.width / buttons.Length) * 0.02f,
				height_gap = (buttons_area.height / buttons.Length) * 0.02f,

				button_width
				= horizontal_button_layout
				? (buttons_area.width / buttons.Length) - width_gap
				: buttons_area.width,

				button_height
				= horizontal_button_layout
				? buttons_area.height
				: (buttons_area.height / buttons.Length) - height_gap;

			string result = string.Empty;
			int i =0;
			GUI.BeginGroup(buttons_area);
			foreach(string s in buttons)
			{
				bool temp_result = false;
				if ( horizontal_button_layout )
					temp_result = GUI.Button(new Rect(i * (button_width + width_gap),
					                                  0,
					                                  button_width,
					                                  button_height), s);
				else
					temp_result = GUI.Button(new Rect(0,
					                                  i * (button_height + height_gap),
					                                  button_width,
					                                  button_height), s);

				//only accept button presses if not waiting for a server response
				if (temp_result && serverState.CompareState(WULServerState.None) )
				{
					result = s;
					OnButtonClicked(tabname, result);
				}
				i++;
			}
			GUI.EndGroup();
			return result;
		}

		public void Draw()
		{
			ShowChallengeFields();
			DrawButtons();
		}

		public cmlData ChallengeToCMLData()
		{
			return ChallengeToCMLData(ref Entries, true);
		}
		
		public cmlData ChallengeToCMLData(bool allow_empty)
		{
			return ChallengeToCMLData(ref Entries, allow_empty);
		}
		
		public cmlData ChallengeToCMLData(ref ServerDataCapture[] challenge_text, bool allow_empty)
		{
			cmlData fields = new cmlData();
			foreach (ServerDataCapture field in challenge_text)
				if (field.fieldType != eFieldType.Verify)
					if (field.value != "" || allow_empty)
						fields.Set( field.serverfield, field.value );

			return fields;
		}
	}
	
	[System.Serializable]
	public class TabbedServerDataCapture
	{
		public TabbedServerDataCapturePage[]
			pages;
		
		public int 
			active_page = 0;

		public Rect
			view_area,
			tabs_area;

		public string[] TabNames { get { if (null == tab_names) RefreshTabNames (); return tab_names; } }

		string[] tab_names;


		public void RefreshTabNames()
		{
			tab_names = new string[pages.Length];
			for(int i = 0; i < tab_names.Length; i++)
				tab_names[i] = pages[i].tabname;
		}
		
		public void Draw()
		{
			active_page = GUI.Toolbar(tabs_area, active_page, TabNames);

			GUI.BeginGroup(view_area);
			if (null != pages
			    && active_page < pages.Length
			    && null != pages[active_page])
			{
				pages[active_page].ShowChallengeFields();
				pages[active_page].DrawButtons();
			}
			GUI.EndGroup();
		}

		public bool AllFieldsAreValid()
		{
			bool result = true;
			foreach (TabbedServerDataCapturePage page in pages)
				if (!page.ValidateChallengeData())
					result = false;

			return result;
		}

		public cmlData ChallengeToCMLData()
		{
			return ChallengeToCMLData(true);
		}
		
		public cmlData ChallengeToCMLData(bool allow_empty)
		{
			cmlData fields = new cmlData();
			foreach (TabbedServerDataCapturePage page in pages)
				foreach (ServerDataCapture field in page.Entries)
					if (field.fieldType != eFieldType.Verify)
						if (field.value != "" || allow_empty)
							fields.Set( field.serverfield, field.value );
			
			return fields;
		}

	}

}
