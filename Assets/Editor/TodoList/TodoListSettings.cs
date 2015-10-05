using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TodoListSettings
{
	enum DisplayType
	{
		Categories,
		Developers,
		Workflow,
		Priority,
		Effort,
	}

	Vector2 DeveloperScrollView;
	Vector2 CategoriesScrollView;
	Vector2 PriorityScrollView;

	TodoListWindow Window;

	DisplayType DisplayMode = DisplayType.Categories;

	Texture2D SettingsBackground;
	Texture2D SettingsBackgroundHover;

	public TodoListSettings( TodoListWindow window )
	{
		Window = window;
	}

	void SetupBackgroundTextures()
	{
		if( SettingsBackground == null )
		{
			SettingsBackground = new Texture2D( 1, 1 );

			SettingsBackground.SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 0.5f, 0.5f, 0.5f ), new Color( 0.27f, 0.27f, 0.27f ) ) );
			SettingsBackground.Apply();
			SettingsBackground.hideFlags = HideFlags.HideAndDontSave;
		}

		if( SettingsBackgroundHover == null )
		{
			SettingsBackgroundHover = new Texture2D( 1, 1 );

			SettingsBackgroundHover.SetPixel( 0, 0, TodoList.GetSkinColor( new Color( 0.6f, 0.6f, 0.6f ), new Color( 0.35f, 0.35f, 0.35f ) ) );
			SettingsBackgroundHover.Apply();
			SettingsBackgroundHover.hideFlags = HideFlags.HideAndDontSave;
		}
	}

	public void Display()
	{
		SetupBackgroundTextures();

		EditorGUILayout.BeginHorizontal();
		{
			DisplayNavigation();

			GUILayout.Space( 5 );

			switch( DisplayMode )
			{
			case DisplayType.Categories:
				DisplayCategoriesSettings();
				break;
			case DisplayType.Developers:
				DisplayDevelopersSettings();
				break;
			case DisplayType.Workflow:
				DisplayWorkflowSettings();
				break;
			case DisplayType.Priority:
				DisplayPrioritySettings();
				break;
			case DisplayType.Effort:
				DisplayEffortSettings();
				break;
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void SetDisplayMode( DisplayType mode )
	{
		DisplayMode = mode;
		Window.Repaint();
	}

	void DisplayNavigation()
	{
		GUIStyle buttonStyle = new GUIStyle( "Label" );
		buttonStyle.fixedHeight = 30;
		buttonStyle.alignment = TextAnchor.MiddleCenter;
		buttonStyle.normal.background = SettingsBackground;
		buttonStyle.hover.background = SettingsBackgroundHover;
		buttonStyle.margin = new RectOffset( 0, 0, 0, 0 );
		buttonStyle.fontStyle = FontStyle.Bold;

		GUIStyle backgroundStyle = new GUIStyle();
		backgroundStyle.normal.background = SettingsBackground;
		backgroundStyle.fixedHeight = Screen.height;
		backgroundStyle.margin = new RectOffset( 0, 0, 0, 0 );

		EditorGUILayout.BeginVertical( backgroundStyle, GUILayout.Width( 266 ) );
		{
			GUILayout.Space( 5 );

			GUILayout.Label( "Settings", buttonStyle );

			GUILayout.Space( 15 );

			if( GUILayout.Button( "Categories", buttonStyle ) )
			{
				SetDisplayMode( DisplayType.Categories );
			}

			if( GUILayout.Button( "Developers", buttonStyle ) )
			{
				SetDisplayMode( DisplayType.Developers );
			}

			if( GUILayout.Button( "Workflow", buttonStyle ) )
			{
				SetDisplayMode( DisplayType.Workflow );
			}

			if( GUILayout.Button( "Priority", buttonStyle ) )
			{
				SetDisplayMode( DisplayType.Priority );
			}

			if( GUILayout.Button( "Backlog Item Effort", buttonStyle ) )
			{
				SetDisplayMode( DisplayType.Effort );
			}
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayCategoriesSettings()
	{
		EditorGUILayout.BeginVertical( GUILayout.Width( Mathf.Max( 295, Screen.width / 3 ) ) );
		{
			GUILayout.Space( 5 );

			GUILayout.Label( "Categories", TodoList.GetHeadlineStyle() );

			TodoListStringList.Display( ref CategoriesScrollView, Window.GetCurrentList().GetCategoriesList()
										, new TodoListStringListAddCallback( Window.GetCurrentList().OnCategoryAdded )
										, new TodoListStringListDeleteCallback( Window.GetCurrentList().OnCategoryDeleted )
										, new TodoListStringListChangedCallback( Window.GetCurrentList().OnCategoryChanged ) );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayDevelopersSettings()
	{
		EditorGUILayout.BeginVertical( GUILayout.Width( Mathf.Max( 295, Screen.width / 3 ) ) );
		{
			GUILayout.Space( 5 );

			GUILayout.Label( "Developers", TodoList.GetHeadlineStyle() );


			TodoListStringList.Display( ref DeveloperScrollView, Window.GetCurrentList().GetDevelopersList()
										, new TodoListStringListAddCallback( Window.GetCurrentList().OnDeveloperAdded )
										, new TodoListStringListDeleteCallback( Window.GetCurrentList().OnDeveloperDeleted )
										, new TodoListStringListChangedCallback( Window.GetCurrentList().OnDeveloperChanged ) );

		}
		EditorGUILayout.EndVertical();
	}

	void DisplayWorkflowSettings()
	{
		TodoListWorkflowList.Display( Window, Window.GetCurrentList().GetWorkflowList()
			, new TodoListWorkflowListAddCallback( Window.GetCurrentList().OnWorkflowItemAdded )
			, new TodoListWorkflowListEditCallback( Window.GetCurrentList().OnWorkflowItemEdited )
			, new TodoListWorkflowListDeleteCallback( Window.GetCurrentList().OnWorkflowItemDeleted ) );
	}

	void DisplayPrioritySettings()
	{
		EditorGUILayout.BeginVertical( GUILayout.Width( Mathf.Max( 295, Screen.width / 3 ) ) );
		{
			GUILayout.Space( 5 );

			GUILayout.Label( "Priority", TodoList.GetHeadlineStyle() );


			TodoListStringList.Display( ref PriorityScrollView, Window.GetCurrentList().GetPriorityList()
										, new TodoListStringListAddCallback( Window.GetCurrentList().OnPriorityAdded )
										, new TodoListStringListDeleteCallback( Window.GetCurrentList().OnPriorityDeleted )
										, new TodoListStringListChangedCallback( Window.GetCurrentList().OnPriorityChanged ) );

		}
		EditorGUILayout.EndVertical();
	}

	void DisplayEffortSettings()
	{
		EditorGUILayout.BeginVertical( GUILayout.Width( Mathf.Max( 288, Screen.width / 3 ) ) );
		{
			GUILayout.Space( 5 );

			GUILayout.Label( "Backlog Item Effort", TodoList.GetHeadlineStyle() );

			GUIStyle style = new GUIStyle( "Label" );
			style.wordWrap = true;
			GUILayout.Label( "Some Scrum practitioners estimate the effort of product backlog items in ideal engineering days, but many people prefer less concrete-sounding backlog effort estimation units.\nAlternative units might include story points, function points, or \"t-shirt sizes\" (1 for small, 2 for medium, etc.).\n\nThe advantage of vaguer units is they're explicit about the distinction that product backlog item effort estimates are not estimates of duration.  Also, estimates at this level are rough guesses that should never be confused with actual working hours.", style );

			style.fontStyle = FontStyle.Italic;

			GUILayout.Label( "Description by Victor Szalvay, http://scrumalliance.org", style );

			style.fontStyle = FontStyle.Normal;
			GUILayout.Label( "\nThis option sets the number of available units you can select in your effort estimation. The more options you have, the finer you can distinguish between different efforts but it's also very easy to waste time thinking about a tasks effort.\n\nIn my experience, four options is enough for a first estimate.", style );
			int numOptions = 9;
			GUIContent[] displayedOptions = new GUIContent[ numOptions ];
			int[] valueOptions = new int[ numOptions ];

			for( int i = 0; i < numOptions; ++i )
			{
				displayedOptions[ i ] = new GUIContent( ( i + 1 ).ToString() );
				valueOptions[ i ] = ( i + 1 );
			}

			int newValue = EditorGUILayout.IntPopup( new GUIContent( "Total options" ), Window.GetCurrentList().GetNumberOfBacklogItemEffortOptions(), displayedOptions, valueOptions );

			if( newValue != Window.GetCurrentList().GetNumberOfBacklogItemEffortOptions() )
			{
				Window.GetCurrentList().RegisterUndo( "Edit Backlog Item Effort Options" );

				Window.GetCurrentList().SetNumberOfBacklogItemEffortOptions( newValue );
			}
		}
		EditorGUILayout.EndVertical();
	}
}
