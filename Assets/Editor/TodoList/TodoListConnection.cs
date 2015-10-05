using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TodoListConnection
{
	WWW ShowProjectsRequest;
	WWW ConnectToServerRequest;

	bool ShowProjects = false;
	Dictionary<int, string> Projects;

	Vector2 ProjectsScrollView;
	int ServerControlId = -1;
	int ProjectNameControlId = -1;

	Texture2D LoadingTexture;

	TodoListWindow Window;

	public TodoListConnection( TodoListWindow window )
	{
		Window = window;

		Projects = new Dictionary<int, string>();

		TodoList.LoadPreferences();
	}

	public void Display()
	{
		if( LoadingTexture == null )
		{
			LoadingTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Loading", typeof( Texture2D ) );
		}

		HandleEvents();
		HandleWebRequests();

		if( IsInputDisabled() )
		{
			GUI.enabled = false;
		}

		DisplayLocalConfig();
		DisplayRemoteConfig();		

		if( IsInputDisabled() )
		{
			GUI.enabled = true;
		}
	}

	bool IsInputDisabled()
	{
		return ConnectToServerRequest != null;
	}

	void DisplayLocalConfig()
	{
		//bool localToggle = EditorGUILayout.BeginToggleGroup( "Store To-Do List locally", !TodoList.UseServerToggle );
		bool localToggle = EditorGUILayout.BeginToggleGroup( "Store To-Do List in a local File", true );
		{
			if( Window.IsConnectedToList() && Window.GetCurrentList().GetListType() == TodoListType.Local )
			{
				GUI.enabled = false;
				GUILayout.Button( "You already created a local To-Do List", GUILayout.Width( 300 ) );

				if( IsInputDisabled() == false )
				{
					GUI.enabled = true;
				}
			}
			else
			{
				if( GUILayout.Button( "Create To-Do List File", GUILayout.Width( 150 ) ) )
				{
					OnCreateLocalTodoListClicked();
				}
			}
		}
		EditorGUILayout.EndToggleGroup();
		TodoList.UseServerToggle = !localToggle;
	}

	void DisplayRemoteConfig()
	{
		//TodoList.UseServerToggle = EditorGUILayout.BeginToggleGroup( "Store To-Do List on a remote Server", TodoList.UseServerToggle );
		EditorGUILayout.BeginToggleGroup( "Store To-Do List on a remote PHP/MySQL Server", false );
		{
			EditorGUILayout.BeginHorizontal();
			{
				//DisplayConnection();
				//DisplayProjectsOnServer();
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndToggleGroup();

		TodoList.WarningBox( "This feature is planned and coming soon. If you are interested in a remote solution, send me an e-mail to todo@olivereberlei.com so I'll know to hurry up :-)" );
		TodoList.WarningBox( "I know it has been a while. But with version 1.3 I implemented all the features I wanted to have working before I create the server. Not much longer, I promise" );
		/*if( newServerToggle )
		{
			EditorApplication.Beep();
			EditorUtility.DisplayDialog( "Sorry, Not done yet", "The remote solution is currently under development.\n\nIt is going to be a PHP/MySQL powered application which will allow you to have multiple people connecting to the same list.\n\nIf you are interessted, send me a mail to todo@olivereberlei.com so I'll know to hurry up :-)", "kthxbye" );
		}*/
	}

	void DisplayConnection()
	{
		EditorGUILayout.BeginVertical();
		{
			GUILayout.Label( "Connection", TodoList.GetHeadlineStyle() );
			GUILayout.Space( 4 );
			
			ServerControlId = GUIUtility.GetControlID( FocusType.Keyboard ) + 1;
			
			TodoList.Server = AddSlash( EditorGUILayout.TextField( "Server", TodoList.Server ) );
			GUILayout.Space( 2 );

			TodoList.Username = EditorGUILayout.TextField( "User Name", TodoList.Username );
			GUILayout.Space( 2 );

			TodoList.Password = EditorGUILayout.PasswordField( "Password", TodoList.Password );
			GUILayout.Space( 1 );

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if( ShowProjectsRequest != null )
				{
					GUI.enabled = false;

					float windowWidth = GUILayoutUtility.GetLastRect().width;

					float angle = Time.realtimeSinceStartup * 300 % 360;
					Vector2 offset = new Vector2( windowWidth - 16, 151 + 16 );

					GUIUtility.RotateAroundPivot( angle, offset );
					GUI.DrawTexture( new Rect( offset.x - 8, offset.y - 8, 16, 16 ), LoadingTexture );
					GUIUtility.RotateAroundPivot( -angle, offset );
				}

				if( GUILayout.Button( "Show Projects", GUILayout.Width( 140 ) ) )
				{
					OnShowProjectsClicked( true );
				}

				if( GUILayout.Button( "Server Administration", GUILayout.Width( 140 ) ) )
				{
					OnServerAdministrationClicked();
				}

				if( GUILayout.Button( "Clear Configuration", GUILayout.Width( 140 ) ) )
				{
					OnClearConfigurationClicked();
				}

				if( ShowProjectsRequest != null )
				{
					GUI.enabled = true;
				}
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space( 1 );

			ProjectNameControlId = GUIUtility.GetControlID( FocusType.Keyboard ) + 1;
			TodoList.ProjectName = EditorGUILayout.TextField( "Project Name", TodoList.ProjectName );
			GUILayout.Space( 1 );

			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.FlexibleSpace();

				if( ConnectToServerRequest != null )
				{
					float windowWidth = GUILayoutUtility.GetLastRect().width;

					float angle = Time.realtimeSinceStartup * 300 % 360;
					Vector2 offset = new Vector2( windowWidth - 16, 210 );

					GUIUtility.RotateAroundPivot( angle, offset );
					GUI.DrawTexture( new Rect( offset.x - 8, offset.y - 8, 16, 16 ), LoadingTexture );
					GUIUtility.RotateAroundPivot( -angle, offset );
				}

				if( GUILayout.Button( "Connect", GUILayout.Width( 140 ) ) )
				{
					OnConnectClicked();
				}
			}
			EditorGUILayout.EndHorizontal();

			if( GUI.changed )
			{
				TodoList.SavePreferences();
			}
		}
		EditorGUILayout.EndVertical();

		if( ShouldRepaint() )
		{
			Window.Repaint();
		}
	}

	bool ShouldRepaint()
	{
		return ShowProjectsRequest != null || ConnectToServerRequest != null;
	}

	string AddSlash( string text )
	{
		if( text.Length < 2 )
		{
			return text;
		}

		if( text.Substring( text.Length - 1 ) != "/" )
		{
			text += "/";
		}

		return text;
	}

	void DisplayProjectsOnServer()
	{
		GUIStyle style = new GUIStyle();

		EditorGUILayout.BeginVertical( style, GUILayout.Width( 200 ) );
		{
			GUILayout.Label( "Projects on Server", TodoList.GetHeadlineStyle() );

			if( ShowProjects )
			{
				if( Projects.Count == 0 )
				{
					TodoList.WarningBox( "There are no projects assigned to you" );
				}
				else
				{
					ProjectsScrollView = EditorGUILayout.BeginScrollView( ProjectsScrollView );
					{
						foreach( var project in Projects )
						{
							if( GUILayout.Button( project.Value ) )
							{
								TodoList.ProjectName = project.Value;
							}
						}
					}
					EditorGUILayout.EndScrollView();
				}
			}
		}
		EditorGUILayout.EndVertical();
	}

	void HandleEvents()
	{
		if( Event.current.type == EventType.keyDown )
		{
			if( Event.current.keyCode == KeyCode.Return )
			{
				int pressedId = EditorGUIUtility.keyboardControl;

				if( pressedId >= ServerControlId && pressedId < ServerControlId + 3 )
				{
					OnShowProjectsClicked( false );
				}
			}
		}
	}

	delegate void WebRequestCallback( string webResult );

	void HandleWebRequests()
	{
		HandleWebRequest( ref ShowProjectsRequest, new WebRequestCallback( OnShowProjectsReceived ) );
		HandleWebRequest( ref ConnectToServerRequest, new WebRequestCallback( OnConnectToServerReceived ) );
	}

	void HandleWebRequest( ref WWW request, WebRequestCallback requestCallback )
	{
		if( request != null )
		{
			if( request.error != null )
			{
				EditorUtility.DisplayDialog( "Error", request.error, "Ok" );
				request = null;
			}
			else
			{
				if( request.isDone )
				{
					if( request.text.Substring( 0, 14 ) == "unityTodoWorks" )
					{
						string text = request.text.Substring( 14 );

						if( IsWebError( text ) == false )
						{
							requestCallback( text );
						}
						request = null;
					}
					else
					{
						EditorUtility.DisplayDialog( "Error", "Server URL is invalid", "Ok" );
						request = null;
					}
				}
			}
		}
	}

	bool IsWebError( string text )
	{
		if( text.Length >= 7 && text.Substring( 0, 7 ) == "Error: " )
		{
			string errorMsg = text.Substring( 7 );

			EditorApplication.Beep();
			EditorUtility.DisplayDialog( "Error", errorMsg, "Ok" );

			return true;
		}
		else if( text.Length >= 6 && text.Substring( 0, 6 ) == "<br />" )
		{
			string errorMsg = text.Substring( 6 );
			errorMsg = errorMsg.Replace( "<b>", "" );
			errorMsg = errorMsg.Replace( "</b>", "" );
			errorMsg = errorMsg.Replace( "<br />", "" );

			EditorApplication.Beep();
			EditorUtility.DisplayDialog( "Fatal Error", errorMsg, "Ok" );

			return true;
		}
		else
		{
			return false;
		}
	}

	void OnConnectToServerReceived( string text )
	{
		if( text == "true" )
		{
			TodoList.SavePreferences();

			TodoListRemote todoList = new TodoListRemote( TodoList.Server, TodoList.Username, TodoList.Password, TodoList.ProjectName );

			Window.SetTodoList( todoList );
		}
		else
		{
			TodoList.ErrorDialog( "You don't have access to this project" );
		}
	}

	void OnShowProjectsReceived( string text )
	{
		Projects.Clear();
		ShowProjects = true;

		string[] ProjectStrings = text.Split( '\n' );

		foreach( string projectString in ProjectStrings )
		{
			if( projectString == "" )
			{
				continue;
			}

			string[] temp = projectString.Split( '\t' );

			
			Projects.Add( System.Convert.ToInt32( temp[ 0 ] ), temp[ 1 ] );
		}
	}

	void OnShowProjectsClicked( bool showProjectsButtonClicked )
	{
		EditorGUIUtility.keyboardControl = 0;

		if( AreTextfieldsEmpty( true, false, showProjectsButtonClicked ) )
		{
			return;
		}

		WWWForm form = new WWWForm();
		form.AddField( "action", "showProjects" );
		form.AddField( "auth", TodoList.GetAuthString() );

		ShowProjectsRequest = new WWW( TodoList.Server + "index.php", form );
	}

	void OnServerAdministrationClicked()
	{
		
	}

	void OnCreateLocalTodoListClicked()
	{
		TodoList.ProjectName = "";
		TodoList.SavePreferences();

		TodoListLocal todoList = new TodoListLocal();

		Window.SetTodoList( todoList );
	}

	void OnClearConfigurationClicked()
	{
		EditorApplication.Beep();
		if( EditorUtility.DisplayDialog( "Please confirm", "Do you really want to clear the server configuration?", "Yes", "No" ) )
		{
			TodoList.Server = "";
			TodoList.Username = "";
			TodoList.Password = "";
			TodoList.ProjectName = "";

			//GUI.DrawTexture(

			ShowProjects = false;
			Projects.Clear();

			EditorGUIUtility.keyboardControl = 0;

			TodoList.SavePreferences();
		}
	}

	bool AreTextfieldsEmpty( bool serverData, bool projectName, bool throwError = true )
	{
		if( serverData )
		{
			if( TodoList.Server == "" )
			{
				if( throwError )
				{
					TodoList.ErrorDialog( "Please enter a server address" );
					EditorGUIUtility.keyboardControl = ServerControlId;
					Window.Focus();
				}
				return true;
			}

			if( TodoList.Username == "" )
			{
				if( throwError )
				{
					TodoList.ErrorDialog( "Please enter a username" );
					EditorGUIUtility.keyboardControl = ServerControlId + 1;
					Window.Focus();
				}
				return true;
			}

			if( TodoList.Password == "" )
			{
				if( throwError )
				{
					TodoList.ErrorDialog( "Please enter a password" );
					EditorGUIUtility.keyboardControl = ServerControlId + 2;
					Window.Focus();
				}
				return true;
			}
		}

		if( projectName )
		{
			if( TodoList.ProjectName == "" )
			{
				if( throwError )
				{
					TodoList.ErrorDialog( "Please enter a project name" );
					EditorGUIUtility.keyboardControl = ProjectNameControlId;
					Window.Focus();
				}
				return true;
			}
		}

		return false;
	}

	void OnConnectClicked()
	{
		if( AreTextfieldsEmpty( true, true ) )
		{
			return;
		}

		WWWForm form = new WWWForm();
		form.AddField( "action", "hasAccessToProject" );
		form.AddField( "auth", TodoList.GetAuthString() );
		form.AddField( "project", TodoList.ProjectName );

		ConnectToServerRequest = new WWW( TodoList.Server + "index.php", form );
	}
}