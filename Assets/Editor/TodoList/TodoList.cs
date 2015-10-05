using UnityEngine;
using UnityEditor;
using System.Collections;

public static class TodoList
{
	public static string Server = "";
	public static string Username = "";
	public static string Password = "";
	public static string ProjectName = "";
	public static bool UseServerToggle;

	public static float Version = 1.3f;
	public static string StringVersion = "1.3";

	public const int MaxMinWidth = 1280;

	public const int NumberAreaWidth = 26;
	public const int ButtonAreaWidth = 56;

	public const int ProgressFieldWidth = 120;
	public const int EffortFieldWidth = 45;
	public const int StatusFieldWidth = 150;
	public const int PriorityFieldWidth = 120;
	public const int SprintFieldWidth = 120;
	public const int CategoryFieldWidth = 120;
	public const int DeveloperFieldWidth = 120;
	public const int DueDateFieldWidth = 107;
	public const int DueTimeFieldWidth = 94;

	public const string PersonalSettingsPath = "Assets/Editor/TodoList/PersonalSettings.asset";

	static Texture2D[] BackgroundTextures;
	static Texture2D[] CompletedTaskBackgroundTextures;
	static Texture2D[] DueBackgroundTextures;

	public static GUIStyle GetHeadlineStyle()
	{
		GUIStyle boxStyle = new GUIStyle( "box" );

		boxStyle.stretchWidth = true;
		boxStyle.alignment = TextAnchor.UpperLeft;
		boxStyle.font = EditorStyles.boldFont;
		boxStyle.normal.textColor = EditorStyles.label.normal.textColor;

		return boxStyle;
	}

	public static GUIStyle GetWarningBoxStyle()
	{
		GUIStyle box = new GUIStyle( "box" );

		box.normal.textColor = EditorStyles.miniLabel.normal.textColor;
		box.imagePosition = ImagePosition.ImageLeft;
		box.stretchWidth = true;
		box.alignment = TextAnchor.UpperLeft;

		return box;
	}

	public static void WarningBox( string text, string tooltip = "" )
	{
		GUIStyle box = GetWarningBoxStyle();

		Texture2D warningIcon = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Warning", typeof( Texture2D ) );
		GUIContent content = new GUIContent( text, warningIcon, tooltip );
		GUILayout.Label( content, box );
	}

#if UNITY_3_5
	public static bool IsUsingProSkin()
	{
		return EditorGUIUtility.isProSkin;
	}

	public static string GetImageFolder( string skinName )
	{
		if( EditorGUIUtility.isProSkin == true )
		{
			return "TodoList/Pro/";
		}
		else
		{
			return "TodoList/Indie/";
		}
	}
#else
	public static bool IsUsingProSkin()
	{
		return GUI.skin.name == "SceneGUISkin";
	}

	public static string GetImageFolder( string skinName )
	{
		if( skinName == "SceneGUISkin" )
		{
			return "TodoList/Pro/";
		}
		else
		{
			return "TodoList/Indie/";
		}
	}
#endif

	public static Color GetSkinColor( Color indie, Color pro )
	{
		if( IsUsingProSkin() == true )
		{
			return pro;
		}
		else
		{
			return indie;
		}
	}

	public static string GetAuthString( string username = "", string password = "" )
	{
		if( username == "" )
		{
			username = Username;
		}

		if( password == "" )
		{
			password = Password;
		}

		return Md5Sum( username + password + "mySaltToAvoidCollisions" );
	}

	public static string Md5Sum( string strToEncrypt )
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding();
		byte[] bytes = ue.GetBytes( strToEncrypt );

		// encrypt bytes
		System.Security.Cryptography.MD5CryptoServiceProvider md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
		byte[] hashBytes = md5.ComputeHash( bytes );

		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";

		for( int i = 0; i < hashBytes.Length; i++ )
		{
			hashString += System.Convert.ToString( hashBytes[ i ], 16 ).PadLeft( 2, '0' );
		}

		return hashString.PadLeft( 32, '0' );
	}

	public static void SavePreferences()
	{
		PlayerPrefs.SetString( "TodoListServer", Server );
		PlayerPrefs.SetString( "TodoListUsername", Username );
		PlayerPrefs.SetString( "TodoListPassword", Password );
		PlayerPrefs.SetString( "TodoListProject", ProjectName );

		if( UseServerToggle )
		{
			PlayerPrefs.SetInt( "TodoListRemote", 1 );
		}
		else
		{
			PlayerPrefs.SetInt( "TodoListRemote", 0 );
		}
	}

	public static void LoadPreferences()
	{
		Server = PlayerPrefs.GetString( "TodoListServer", "" );
		Username = PlayerPrefs.GetString( "TodoListUsername", "" );
		Password = PlayerPrefs.GetString( "TodoListPassword", "" );
		ProjectName = PlayerPrefs.GetString( "TodoListProject", "" );
		UseServerToggle = PlayerPrefs.GetInt( "TodoListRemote", 0 ) == 1;
	}

	public static void ErrorDialog( string error )
	{
		EditorApplication.Beep();
		EditorUtility.DisplayDialog( "Error", error, "Ok" );
	}

	public static int ToUnixTimestamp( int year, int month, int day, int hour = 0, int minute = 0 )
	{
		if( month > 12 )
		{
			month = 12;
		}

		int daysInMonth = System.DateTime.DaysInMonth( year, month );

		if( day > daysInMonth )
		{
			day = daysInMonth;
		}

		System.TimeSpan t = ( new System.DateTime( year, month, day, hour, minute, 0 ) - new System.DateTime( 1970, 1, 1, 0, 0, 0 ) );
		return (int)t.TotalSeconds;
	}

	public static int ToUnixTimestamp( System.DateTime date )
	{
		System.TimeSpan t = ( date - new System.DateTime( 1970, 1, 1, 0, 0, 0 ) );
		return (int)t.TotalSeconds;
	}

	public static System.DateTime FromUnixTimestamp( int timestamp )
	{
		System.DateTime dt = new System.DateTime( 1970, 1, 1, 0, 0, 0, 0 );
		dt = dt.AddSeconds( timestamp );

		return dt;
	}

	public static int GetUnixTimestampNow()
	{
		System.DateTime now = System.DateTime.Now;
		return TodoList.ToUnixTimestamp( now.Year, now.Month, now.Day, now.Hour, now.Minute );
	}

	public static string InsertSpaceForCamelCase( string camelCase )
	{
		return System.Text.RegularExpressions.Regex.Replace( camelCase, "(\\B[A-Z])", " $1" );
	}

	static void CreateTaskBackgroundTextures()
	{
		if( BackgroundTextures == null )
		{
			BackgroundTextures = new Texture2D[] { new Texture2D( 1, 1 ), new Texture2D( 1, 1 ) };

			BackgroundTextures[ 0 ].SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 0.85f, 0.85f, 0.85f ), new Color( 60f / 255f, 60f / 255f, 60f / 255f ) ) );
			BackgroundTextures[ 0 ].Apply();
			BackgroundTextures[ 0 ].hideFlags = HideFlags.HideAndDontSave;

			BackgroundTextures[ 1 ].SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 0.76f, 0.76f, 0.76f ), new Color( 55f / 255f, 55f / 255f, 55f / 255f ) ) );
			BackgroundTextures[ 1 ].Apply();
			BackgroundTextures[ 1 ].hideFlags = HideFlags.HideAndDontSave;
		}

		if( CompletedTaskBackgroundTextures == null )
		{
			CompletedTaskBackgroundTextures = new Texture2D[] { new Texture2D( 1, 1 ), new Texture2D( 1, 1 ) };

			CompletedTaskBackgroundTextures[ 0 ].SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 179f / 255f, 248f / 255f, 152f / 255f ), new Color( 11f / 255f, 38f / 255f, 10f / 255f ) ) );
			CompletedTaskBackgroundTextures[ 0 ].Apply();
			CompletedTaskBackgroundTextures[ 0 ].hideFlags = HideFlags.HideAndDontSave;

			CompletedTaskBackgroundTextures[ 1 ].SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 152f / 255f, 241f / 255f, 117f / 255f ), new Color( 18f / 255f, 63f / 255f, 10f / 255f ) ) );
			CompletedTaskBackgroundTextures[ 1 ].Apply();
			CompletedTaskBackgroundTextures[ 1 ].hideFlags = HideFlags.HideAndDontSave;
		}

		if( DueBackgroundTextures == null )
		{
			DueBackgroundTextures = new Texture2D[] { new Texture2D( 1, 1 ), new Texture2D( 1, 1 ) };

			DueBackgroundTextures[ 0 ].SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 255f / 255f, 74f / 255f, 74f / 255f ), new Color( 147f / 255f, 0f / 255f, 0f / 255f ) ) );
			DueBackgroundTextures[ 0 ].Apply();
			DueBackgroundTextures[ 0 ].hideFlags = HideFlags.HideAndDontSave;

			DueBackgroundTextures[ 1 ].SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 255f / 255f, 41f / 255f, 41f / 255f ), new Color( 127f / 255f, 0f / 255f, 0f / 255f ) ) );
			DueBackgroundTextures[ 1 ].Apply();
			DueBackgroundTextures[ 1 ].hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public static Texture2D GetTaskBackground( TodoListTask task, int taskNr )
	{
		CreateTaskBackgroundTextures();

		if( task.IsDone() == true )
		{
			return CompletedTaskBackgroundTextures[ taskNr % 2 ];
		}

		if( task.DueDate != 0 && task.DueTime != 0 )
		{
			if( task.DueDate + task.DueTime < TodoList.ToUnixTimestamp( System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day, System.DateTime.Now.Hour, System.DateTime.Now.Minute ) )
			{
				return DueBackgroundTextures[ taskNr % 2 ];
			}
		}
		else if( task.DueDate != 0 )
		{
			if( task.DueDate < TodoList.ToUnixTimestamp( System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day ) )
			{
				return DueBackgroundTextures[ taskNr % 2 ];
			}
		}
		else if( task.DueTime != 0 )
		{
			if( task.DueTime < TodoList.ToUnixTimestamp( 1970, 1, 1, System.DateTime.Now.Hour, System.DateTime.Now.Minute ) )
			{
				return DueBackgroundTextures[ taskNr % 2 ];
			}
		}

		return BackgroundTextures[ taskNr % 2 ];
	}

	public static Texture2D GetAlteringBackground( int nr )
	{
		CreateTaskBackgroundTextures();

		return BackgroundTextures[ nr % 2 ];
	}

	public static void DisplayTaskEffort( TodoListTask task, int width, TodoListWindow window )
	{
		int numOptions = window.GetCurrentList().GetNumberOfBacklogItemEffortOptions();
		GUIContent[] displayedOptions = new GUIContent[ numOptions ];
		int[] valueOptions = new int[ numOptions ];

		for( int i = 0; i < numOptions; ++i )
		{
			displayedOptions[ i ] = new GUIContent( ( i + 1 ).ToString() );
			valueOptions[ i ] = ( i + 1 );
		}

		GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
		popupStyle.margin.top = 4;
		popupStyle.margin.right = 6;
		popupStyle.margin.left = 0;

		int newValue = EditorGUILayout.IntPopup( task.Effort, displayedOptions, valueOptions, popupStyle, GUILayout.Width( width ) );

		if( newValue != task.Effort )
		{
			window.GetCurrentList().RegisterUndo( "Edit Task Backlog Item Effort" );

			task.Effort = newValue;

			window.GetCurrentList().OnTaskChanged( task );
		}
	}

	public static void DisplayTaskSprint( TodoListTask task, int width, TodoListWindow window )
	{
		int numOptions = window.GetCurrentList().GetSprintList().Count + 1;
		GUIContent[] displayedOptions = new GUIContent[ numOptions ];
		int[] valueOptions = new int[ numOptions ];

		displayedOptions[ 0 ] = new GUIContent( "Unassigned" );
		valueOptions[ 0 ] = -1;

		for( int i = 0; i < window.GetCurrentList().GetSprintList().Count; ++i )
		{
			displayedOptions[ i + 1 ] = new GUIContent( window.GetCurrentList().GetSprintList()[ i ].Name );
			valueOptions[ i + 1 ] = window.GetCurrentList().GetSprintList()[ i ].SprintIndex;
		}

		GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
		popupStyle.margin.top = 4;
		popupStyle.margin.right = 6;
		popupStyle.margin.left = 0;

		int newValue = EditorGUILayout.IntPopup( task.SprintIndex, displayedOptions, valueOptions, popupStyle, GUILayout.Width( width ) );

		if( newValue != task.SprintIndex )
		{
			window.GetCurrentList().RegisterUndo( "Edit Task Sprint" );

			task.SprintIndex = newValue;

			window.GetCurrentList().OnTaskChanged( task );
		}
	}
}