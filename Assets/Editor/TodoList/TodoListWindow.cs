// C# example:
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public enum TodoListModes
{
	Overview,
	Connection,
	Settings,
	Statistics,
	Sprints,
	About,
}

public enum TodoListDisplayOptions
{
	ShowProgress,
	ShowEffort,
	ShowStatus,
	ShowPriority,
	ShowSprint,
	ShowCategories,
	ShowDevelopers,
	ShowDueDate,
	ShowDueTime,
	ShowCompletedTasks,
	Count,
}

public class TodoListWindow : EditorWindow
{
	static bool IsOpen;

	TodoListAddItem AddItemGui;
	TodoListConnection ConnectionGui;
	TodoListView ViewGui;
	TodoListStatistics StatisticsGui;
	TodoListAbout AboutGui;
	TodoListSettings SettingsGui;
	TodoListSprints SprintsGui;

	TodoListInterface CurrentList;

	TodoListEditTaskWindow EditTaskWindow;

	Texture2D StatisticsTexture;
	Texture2D SettingsTexture;
	Texture2D SprintsTexture;

	[MenuItem( "Window/To-Do List %t" )]
	public static void Init()
	{
		TodoListWindow window = (TodoListWindow)EditorWindow.GetWindow( typeof( TodoListWindow ), false, "To-Do List" );

		window.UpdateMinSize();

		if( IsOpen )
		{
			window.Close();
			IsOpen = false;
		}
	}

	void OnEnable()
	{
		wantsMouseMove = true;

		CreateGuiSectionObjects();

		ConnectToExistingLists();

		if( EditTaskWindow != null )
		{
			EditTaskWindow = (TodoListEditTaskWindow)EditorWindow.GetWindow( typeof( TodoListEditTaskWindow ), false, "Edit Task" );
			EditTaskWindow.Close();
			EditTaskWindow = null;
		}

		UpdateMinSize();
	}

	void OnDisable()
	{
		IsOpen = false;

		if( EditTaskWindow != null )
		{
			EditTaskWindow.Close();
		}
	}

	public bool GetDisplayOption( TodoListDisplayOptions option )
	{
		if( option == TodoListDisplayOptions.Count )
		{
			return false;
		}

		string prefName = "TodoList" + option.ToString();

		return PlayerPrefs.GetInt( prefName, 0 ) == 1;
	}

	public void ToggleDisplayOption( TodoListDisplayOptions option )
	{
		SetDisplayOption( option, !GetDisplayOption( option ) );
	}

	public void SetDisplayOption( TodoListDisplayOptions option, bool newValue )
	{
		if( option == TodoListDisplayOptions.Count )
		{
			return;
		}

		string prefName = "TodoList" + option.ToString();

		if( newValue )
		{
			PlayerPrefs.SetInt( prefName, 1 );
		}
		else
		{
			PlayerPrefs.SetInt( prefName, 0 );
		}
	}

	void CreateGuiSectionObjects()
	{
		if( AddItemGui == null )
		{
			AddItemGui = new TodoListAddItem( this );
		}

		if( ConnectionGui == null )
		{
			ConnectionGui = new TodoListConnection( this );
		}

		if( ViewGui == null )
		{
			ViewGui = new TodoListView( this );
		}

		if( StatisticsGui == null )
		{
			StatisticsGui = new TodoListStatistics( this );
		}

		if( AboutGui == null )
		{
			AboutGui = new TodoListAbout( this );
		}

		if( SettingsGui == null )
		{
			SettingsGui = new TodoListSettings( this );
		}

		if( SprintsGui == null )
		{
			SprintsGui = new TodoListSprints( this );
		}
	}

	public bool IsConnectedToList()
	{
		return CurrentList != null;
	}

	public TodoListInterface GetCurrentList()
	{
		return CurrentList;
	}

	void ConnectToExistingLists()
	{
		//if( PlayerPrefs.GetInt( "TodoListRemote", 0 ) == 0 )
		{
			TodoListObject todoListObject = (TodoListObject)AssetDatabase.LoadAssetAtPath( TodoListLocal.TodoListAssetPath, typeof( TodoListObject ) );

			if( todoListObject != null )
			{
				TodoListLocal todoList = new TodoListLocal();

				SetTodoList( todoList );
			}
		}
		//else
		{
			
		}
	}

	public void SetTodoList( TodoListInterface newList )
	{
		if( CurrentList != null )
		{
			CurrentList.Clear();
		}

		CurrentList = newList;
	}

	public TodoListInterface GetTodoList()
	{
		return CurrentList;
	}

	public void SetDisplayMode( TodoListModes newMode )
	{
		GUIUtility.keyboardControl = 0;
		CurrentList.GetPersonalSettings().DisplayMode = newMode;

		EditorUtility.SetDirty( CurrentList.GetPersonalSettings() );

		if( newMode == TodoListModes.Statistics )
		{
			StatisticsGui.UpdateStatistics();
		}
	}

	TodoListModes GetDisplayMode()
	{
		if( CurrentList == null )
		{
			return TodoListModes.Connection;
		}

		return CurrentList.GetPersonalSettings().DisplayMode;
	}

	void Update()
	{
		if( IsOpen == false )
		{
			IsOpen = true;
		}

		if( ViewGui != null && GetDisplayMode() == TodoListModes.Overview )
		{
			ViewGui.Update();
		}
	}

	void OnGUI()
	{
		if( Event.current != null && Event.current.type == EventType.mouseMove )
		{
			if( GetDisplayMode() == TodoListModes.Overview && Event.current.mousePosition.x < 29 || GetDisplayMode() == TodoListModes.Settings || GetDisplayMode() == TodoListModes.About )
			{
				Repaint();
			}
			else
			{
				return;
			}
		}

		if( EditTaskWindow != null )
		{
			EditTaskWindow.Repaint();
		}

		if( StatisticsTexture == null )
		{
			StatisticsTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Statistics", typeof( Texture2D ) );
		}

		if( SettingsTexture == null )
		{
			SettingsTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Settings", typeof( Texture2D ) );
		}

		if( SprintsTexture == null )
		{
			SprintsTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Sprints", typeof( Texture2D ) );
		}

		if( ( GetDisplayMode() != TodoListModes.Connection && GetDisplayMode() != TodoListModes.About ) && ( CurrentList == null || CurrentList.IsValid() == false ) )
		{
			CurrentList = null;
			SetDisplayMode( TodoListModes.Connection );
		}

		RepaintOnUndoRedo();

		DisplayHeadingToolbar();

		switch( GetDisplayMode() )
		{
		case TodoListModes.Overview:
			AddItemGui.Display();
			ViewGui.Display();
			break;
		case TodoListModes.Connection:
			ConnectionGui.Display();
			break;
		case TodoListModes.Settings:
			SettingsGui.Display();
			break;
		case TodoListModes.Statistics:
			StatisticsGui.Display();
			break;
		case TodoListModes.About:
			AboutGui.Display();
			break;
		case TodoListModes.Sprints:
			SprintsGui.Display();
			break;
		}
	}

	void RepaintOnUndoRedo()
	{
		if( Event.current.type == EventType.ValidateCommand )
		{
			switch( Event.current.commandName )
			{
			case "UndoRedoPerformed":
				Repaint();
				break;
			}
		}
	}

	private void ToggleDisplayOptionsDelegate( object userData, string[] options, int selected )
	{
		if( selected == (int)TodoListDisplayOptions.Count )
		{
			selected--;
		}

		ToggleDisplayOption( (TodoListDisplayOptions)selected );

		UpdateMinSize();
	}

	void DisplayDisplayOptionsButton()
	{
		if( GUILayout.Button( "Display Options", EditorStyles.toolbarPopup ) )
		{
			List<string> items = new List<string>();
			List<int> selected = new List<int>();

			for( int i = 0; i < (int)TodoListDisplayOptions.Count; ++i )
			{
				if( i == (int)TodoListDisplayOptions.Count - 1 )
				{
					items.Add( "" );
				}

				items.Add( TodoList.InsertSpaceForCamelCase( ( (TodoListDisplayOptions)i ).ToString() ) );

				if( GetDisplayOption( (TodoListDisplayOptions)i ) )
				{
					if( i == (int)TodoListDisplayOptions.Count - 1 )
					{
						i++;
					}

					selected.Add( i );
				}
			}

			object[] param = new object[ 5 ];
			param[ 0 ] = new Rect( 100, 0, 1, 1 );
			param[ 1 ] = items.ToArray();
			param[ 2 ] = selected.ToArray();
			param[ 3 ] = new EditorUtility.SelectMenuItemFunction( ToggleDisplayOptionsDelegate );
			param[ 4 ] = null;

			typeof( EditorUtility ).InvokeMember( "DisplayCustomMenu", BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, typeof( EditorUtility ), param );
		}
	}

	void DisplayHeadingToolbar()
	{
		GUIStyle normalButton = new GUIStyle( EditorStyles.toolbarButton );
		normalButton.fixedWidth = 85;

		GUIStyle iconButton = new GUIStyle( EditorStyles.toolbarButton );
		iconButton.fixedWidth = 25;

		EditorGUILayout.BeginHorizontal( EditorStyles.toolbar );
		{
			if( IsConnectedToList() )
			{
				if( GUILayout.Toggle( GetDisplayMode() == TodoListModes.Overview, "To-Do List", normalButton ) )
				{
					if( GetDisplayMode() != TodoListModes.Overview )
					{
						CurrentList.GetDevelopersList().RemoveAll( item => item == "" );
						CurrentList.GetCategoriesList().RemoveAll( item => item == "" );
						CurrentList.GetWorkflowList().RemoveAll( item => item.Description == "" );

						ViewGui.OnEnable();

						SetDisplayMode( TodoListModes.Overview );
					}
				}

				DisplayDisplayOptionsButton();

				if( GUILayout.Toggle( GetDisplayMode() == TodoListModes.Sprints, new GUIContent( SprintsTexture, "Sprints" ), iconButton ) )
				{
					if( GetDisplayMode() != TodoListModes.Sprints )
					{
						SetDisplayMode( TodoListModes.Sprints );
					}
				}

				if( GUILayout.Toggle( GetDisplayMode() == TodoListModes.Statistics, new GUIContent( StatisticsTexture, "Statistics" ), iconButton ) )
				{
					if( GetDisplayMode() != TodoListModes.Statistics )
					{
						SetDisplayMode( TodoListModes.Statistics );
					}
				}

				if( CurrentList.HasSettingsWindow() )
				{
					if( GUILayout.Toggle( GetDisplayMode() == TodoListModes.Settings, new GUIContent( SettingsTexture, "Settings" ), iconButton ) )
					{
						if( GetDisplayMode() != TodoListModes.Settings )
						{
							SetDisplayMode( TodoListModes.Settings );
						}
					}
				}
			}

			GUILayout.FlexibleSpace();

			if( GUILayout.Toggle( GetDisplayMode() == TodoListModes.About, "About", normalButton ) )
			{
				if( GetDisplayMode() != TodoListModes.About )
				{
					SetDisplayMode( TodoListModes.About );
				}
			}

			if( GUILayout.Toggle( GetDisplayMode() == TodoListModes.Connection, "Connection", normalButton ) )
			{
				if( GetDisplayMode() != TodoListModes.Connection )
				{
					SetDisplayMode( TodoListModes.Connection );
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	public void UpdateMinSize()
	{
		int width = TodoList.MaxMinWidth;

		if( CurrentList != null )
		{
			if( GetDisplayOption( TodoListDisplayOptions.ShowProgress ) == false )
			{
				width -= TodoList.ProgressFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowEffort ) == false )
			{
				width -= TodoList.EffortFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowStatus ) == false )
			{
				width -= TodoList.StatusFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowPriority ) == false )
			{
				width -= TodoList.PriorityFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowSprint ) == false )
			{
				width -= TodoList.SprintFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowCategories ) == false )
			{
				width -= TodoList.CategoryFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowDevelopers ) == false )
			{
				width -= TodoList.DeveloperFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowDueDate ) == false )
			{
				width -= TodoList.DueDateFieldWidth;
			}

			if( GetDisplayOption( TodoListDisplayOptions.ShowDueTime ) == false )
			{
				width -= TodoList.DueTimeFieldWidth;
			}
		}

		this.minSize = new Vector2( Mathf.Max( width, 570 ), 310 );
		Repaint();
	}

	public void SetEditTaskWindow( TodoListEditTaskWindow window )
	{
		EditTaskWindow = window;
	}
}