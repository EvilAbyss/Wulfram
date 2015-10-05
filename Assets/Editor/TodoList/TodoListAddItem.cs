using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TodoListAddItem
{
	string AddItemDesc = "";
	int AddItemEffort = 1;
	float AddItemProgress = 0;
	int AddItemSprint = -1;
	int StatusIndex = 0;
	int PriorityIndex = 0;
	int CategoryIndex = 0;
	int DeveloperIndex = 0;
	bool DueDateToggle;
	bool DueTimeToggle;

	string DueDateDay = System.DateTime.Now.Day.ToString( "00" );
	string DueDateMonth = System.DateTime.Now.Month.ToString( "00" );
	string DueDateYear = System.DateTime.Now.Year.ToString( "0000" );
	string DueTimeHour = System.DateTime.Now.Hour.ToString( "00" );
	string DueTimeMin = System.DateTime.Now.Minute.ToString( "00" );

	bool MaxlengthOfTextFieldReached = false;

	bool AddCurrentTaskViaKeyDown = false;

	TodoListWindow Window;

	public TodoListAddItem( TodoListWindow window )
	{
		Window = window;

		DueDateToggle = true;
		DueTimeToggle = true;
	}

	public void Display()
	{
		HandleEvents();

		EditorGUILayout.BeginHorizontal();
		{
			bool isInitialStatusListEmpty = GetInitialStatusList().Count == 0;
			GUI.enabled = !isInitialStatusListEmpty;

			DisplayAddNewItemDescription();

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowProgress ) )
			{
				DisplayAddNewItemProgress();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowEffort ) )
			{
				DisplayAddNewItemEffort();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowStatus ) )
			{
				DisplayAddNewItemStatus();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowPriority ) )
			{
				DisplayAddNewItemPriority();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowSprint ) )
			{
				DisplayAddNewItemSprint();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowCategories ) )
			{
				DisplayAddNewItemCategory();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDevelopers ) )
			{
				DisplayAddNewItemDeveloper();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDueDate ) )
			{
				DisplayAddNewItemDueDate();
			}

			if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDueTime ) )
			{
				DisplayAddNewItemDueTime();
			}

			DisplayAddNewItemButton();

			GUI.enabled = true;

			if( isInitialStatusListEmpty )
			{
				DisplayNoInitialStatusError();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayAddNewItemDescription()
	{
		EditorGUILayout.BeginVertical();
		{
			GUILayout.Label( "Description", EditorStyles.boldLabel );
			GUI.SetNextControlName( "AddItemDesc" );
			AddItemDesc = EditorGUILayout.TextField( AddItemDesc );
		}
		EditorGUILayout.EndVertical();
	}

	List<string> GetInitialStatusList()
	{
		List<string> initialStatus = new List<string>();
		List<string> initialNonDefaultStatus = new List<string>();

		foreach( TodoListWorkflow workflow in Window.GetCurrentList().GetWorkflowList() )
		{
			if( workflow.Type == TodoListWorkflowType.DefaultStartingStatus )
			{
				initialStatus.Add( workflow.Description );
			}
			else if( workflow.Type == TodoListWorkflowType.StartingStatus )
			{
				initialNonDefaultStatus.Add( workflow.Description );
			}
		}

		foreach( string status in initialNonDefaultStatus )
		{
			initialStatus.Add( status );
		}

		return initialStatus;
	}

	void DisplayAddNewItemProgress()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.fixedWidth = TodoList.ProgressFieldWidth;
		vertical.margin.top = 0;

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.ProgressFieldWidth ) );
		{
			GUILayout.Label( "Progress", EditorStyles.boldLabel );

			EditorGUILayout.BeginHorizontal( GUILayout.Width( TodoList.ProgressFieldWidth ) );
			{
				AddItemProgress = GUILayout.HorizontalSlider( AddItemProgress, 0f, 1f, GUILayout.Width( TodoList.ProgressFieldWidth - 37 ) );
				GUILayout.Label( Mathf.FloorToInt( AddItemProgress * 100 ) + "%", GUILayout.Width( 35 ) );
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemEffort()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = 0;

		int numOptions = Window.GetCurrentList().GetNumberOfBacklogItemEffortOptions();
		GUIContent[] displayedOptions = new GUIContent[ numOptions ];
		int[] valueOptions = new int[ numOptions ];

		for( int i = 0; i < numOptions; ++i )
		{
			displayedOptions[ i ] = new GUIContent( ( i + 1 ).ToString() );
			valueOptions[ i ] = ( i + 1 );
		}

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.EffortFieldWidth ) );
		{
			GUILayout.Label( "Effort", EditorStyles.boldLabel );
			AddItemEffort = EditorGUILayout.IntPopup( AddItemEffort, displayedOptions, valueOptions, GUILayout.Width( TodoList.EffortFieldWidth - 6 ) );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemStatus()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = 0;

		List<string> initialStatus = GetInitialStatusList();

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.StatusFieldWidth ) );
		{
			GUILayout.Label( "Status", EditorStyles.boldLabel );
			StatusIndex = EditorGUILayout.Popup( StatusIndex, initialStatus.ToArray() );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemSprint()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = 0;

		int numOptions = Window.GetCurrentList().GetSprintList().Count + 1;
		GUIContent[] displayedOptions = new GUIContent[ numOptions ];
		int[] valueOptions = new int[ numOptions ];

		displayedOptions[ 0 ] = new GUIContent( "Unassigned" );
		valueOptions[ 0 ] = -1;

		for( int i = 0; i < Window.GetCurrentList().GetSprintList().Count; ++i )
		{
			displayedOptions[ i + 1 ] = new GUIContent( Window.GetCurrentList().GetSprintList()[ i ].Name );
			valueOptions[ i + 1 ] =  Window.GetCurrentList().GetSprintList()[ i ].SprintIndex;
		}

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.SprintFieldWidth ) );
		{
			GUILayout.Label( "Sprint", EditorStyles.boldLabel );
			AddItemSprint = EditorGUILayout.IntPopup( AddItemSprint, displayedOptions, valueOptions );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemPriority()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = 0;

		string[] options = Window.GetCurrentList().GetPriorityStringList();

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.PriorityFieldWidth ) );
		{
			GUILayout.Label( "Priority", EditorStyles.boldLabel );
			GUI.SetNextControlName( "AddItemPriorityIndex" );
			PriorityIndex = EditorGUILayout.Popup( PriorityIndex, options );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemCategory()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = 0;
		
		string[] options = Window.GetCurrentList().GetCategoriesStringList();

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.CategoryFieldWidth ) );
		{
			GUILayout.Label( "Category", EditorStyles.boldLabel );
			GUI.SetNextControlName( "AddItemListIndex" );
			CategoryIndex = EditorGUILayout.Popup( CategoryIndex, options );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemDeveloper()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = 0;

		string[] options = Window.GetCurrentList().GetDevelopersStringList();

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( TodoList.DeveloperFieldWidth ) );
		{
			GUILayout.Label( "Developer", EditorStyles.boldLabel );
			GUI.SetNextControlName( "AddItemDeveloperIndex" );
			DeveloperIndex = EditorGUILayout.Popup( DeveloperIndex, options );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemDueDate()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = -3;

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( 1 ) );
		{
			DueDateToggle = EditorGUILayout.BeginToggleGroup( "Due Date", DueDateToggle );
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Space( 5 );

					GUI.SetNextControlName( "AddItemDueDateDay-numeric" );
					DueDateDay	= GUILayout.TextField( DueDateDay, 2, GUILayout.Width( 25 ) );

					GUI.SetNextControlName( "AddItemDueDateMonth-numeric" );
					DueDateMonth = GUILayout.TextField( DueDateMonth, 2, GUILayout.Width( 25 ) );

					GUI.SetNextControlName( "AddItemDueDateYear-numeric" );
					DueDateYear = GUILayout.TextField( DueDateYear, 4, GUILayout.Width( 40 ) );
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();
		}
		EditorGUILayout.EndVertical();
	}

	void DisplayAddNewItemDueTime()
	{
		GUIStyle vertical = new GUIStyle();
		vertical.margin.top = -3;

		EditorGUILayout.BeginVertical( vertical, GUILayout.Width( 1 ) );
		{
			DueTimeToggle = EditorGUILayout.BeginToggleGroup( "Due Time", DueTimeToggle );
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Space( 5 );

					GUI.SetNextControlName( "AddItemDueDateHour-numeric" );
					DueTimeHour	= GUILayout.TextField( DueTimeHour, 2, GUILayout.Width( 25 ) );

					GUI.SetNextControlName( "AddItemDueDateMin-numeric" );
					DueTimeMin = GUILayout.TextField( DueTimeMin, 2, GUILayout.Width( 25 ) );
				}
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.EndToggleGroup();
		}
		EditorGUILayout.EndVertical();

		GUILayout.Space( 8 );
	}

	void DisplayAddNewItemButton()
	{
		GUIStyle buttonStyle = new GUIStyle( "button" );
		buttonStyle.fixedHeight = 36;
		buttonStyle.fixedWidth = 40;

		if( GUILayout.Button( "Add", buttonStyle ) )
		{
			AddNewTodoListItem();
		}

		/*if( GUILayout.Button( "Test", buttonStyle ) )
		{
			AddTestData();
		}*/

		GUILayout.Space( 14 );
	}

	void AddTestData()
	{
		for( int i = 0; i < 50; ++i )
		{
			TodoListTask task = new TodoListTask();

			task.Description = "Test " + i;
			task.TimeCreated = TodoList.ToUnixTimestamp( 2012, Random.Range( 1, 5 ), Random.Range( 1, 30 ), Random.Range( 1, 24 ), Random.Range( 0, 60 ) );
			task.Status = "Task not started";

			task.SetCompleted( true );
			task.TimeCompleted = task.TimeCreated + 60 * 60 * 24 * Random.Range( 50, 90 );
			task.Status = "Completed";

			task.Developer = "Unassigned";
			task.Category = "Unassigned";

			task.IsMatchingSearchString = true;
			task.Order = Window.GetCurrentList().GetHighestOrder() + 1;

			Window.GetCurrentList().AddTask( task );
		}
	}

	void DisplayNoInitialStatusError()
	{
		GUI.enabled = true;

		Texture2D warningIcon = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Warning", typeof( Texture2D ) );
		GUIContent content = new GUIContent( "You cannot add a new To-Do List item if no Workflow Item is of type StartingStatus or DefaultStartingStatus.", warningIcon );

		GUI.Box( new Rect( 3, 30, Screen.width - 21, 20 ), content, TodoList.GetWarningBoxStyle() );
	}

	void SetMaxlengthOfTextFieldReached( string controlName, string input, int maxlength )
	{
		if( GUI.GetNameOfFocusedControl() == controlName )
		{
			if( input.Length >= maxlength )
			{
				MaxlengthOfTextFieldReached = true;
			}
		}
	}

	string ProcessDateString( string newString, string oldString, int maxLength )
	{
		if( newString.Length > maxLength )
		{
			newString = newString.Substring( 0, maxLength );
		}

		return newString;
	}

	void RestrictKeyboardInputToNumeric()
	{
		if( Event.current.type != EventType.keyDown )
		{
			return;
		}

		char[] allowedChars = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

		bool currentCharIsAllowed = false;

		foreach( char c in allowedChars )
		{
			if( Event.current.character == c )
			{
				currentCharIsAllowed = true;
			}
		}

		if( currentCharIsAllowed == false )
		{
			if( Event.current.character != '\0' )
			{
				EditorApplication.Beep();
				Event.current.character = '\0';
			}
		}
	}

	void DisallowCurrentKeyboardInput()
	{
		Event.current.character = '\0';
	}

	bool IsFocusedControlNumericOnly()
	{
		string name = GUI.GetNameOfFocusedControl();

		if( name == "" || name.IndexOf( "-" ) < 0 )
		{
			return false;
		}

		string[] segments = name.Split( '-' );

		foreach( string segment in segments )
		{
			if( segment == "numeric" )
			{
				return true;
			}
		}

		return false;
	}

	void AddNewTodoListItem()
	{
		if( AddItemDesc == "" )
		{
			return;
		}

		TodoListTask task = new TodoListTask();
		string[] initialStatusList = GetInitialStatusList().ToArray();

		task.Description = AddItemDesc;
		task.Status = initialStatusList[ StatusIndex ];
		task.Effort = AddItemEffort;
		task.SprintIndex = AddItemSprint;

		if( Window.GetCurrentList().GetCategoriesList().Count > 0 )
		{
			string cat = "Unassigned";

			if( CategoryIndex > 0 && CategoryIndex < Window.GetCurrentList().GetCategoriesList().Count + 1 )
			{
				cat = Window.GetCurrentList().GetCategoriesList()[ CategoryIndex - 1 ];
			}

			task.Category = cat;
		}

		if( Window.GetCurrentList().GetDevelopersList().Count > 0 )
		{
			string dev = "Unassigned";

			if( DeveloperIndex > 0 && DeveloperIndex < Window.GetCurrentList().GetDevelopersList().Count + 1 )
			{
				dev = Window.GetCurrentList().GetDevelopersList()[ DeveloperIndex - 1 ];
			}

			task.Developer = dev;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDueDate ) )
		{
			if( DueDateToggle )
			{
				int day = System.Convert.ToInt32( DueDateDay );
				int month = System.Convert.ToInt32( DueDateMonth );
				int year = System.Convert.ToInt32( DueDateYear );

				task.DueDate  = TodoList.ToUnixTimestamp( year, month, day );
			}
			else
			{
				task.DueDate = 0;
			}
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDueTime ) )
		{
			if( DueTimeToggle )
			{
				int hour = System.Convert.ToInt32( DueTimeHour );
				int min = System.Convert.ToInt32( DueTimeMin );

				task.DueTime  = TodoList.ToUnixTimestamp( 1970, 1, 1, hour, min );
			}
			else
			{
				task.DueTime = 0;
			}
		}

		task.Tags = "";
		task.Order = Window.GetCurrentList().GetHighestOrder() + 1;

		task.TimeCreated = TodoList.GetUnixTimestampNow();
		task.TimeCompleted = 0;

		task.IsMatchingSearchString = true;
		Window.GetCurrentList().AddTask( task );

		AddItemDesc = "";
		AddItemEffort = 1;
		AddItemSprint = -1;

		GUIUtility.keyboardControl = 0;
		Window.Repaint();
	}

	void HandleEvents()
	{
		if( AddCurrentTaskViaKeyDown )
		{
			AddNewTodoListItem();
			AddCurrentTaskViaKeyDown = false;
		}

		if( Event.current == null )
		{
			return;
		}

		//Debug.Log( Event.current.type );

		switch( Event.current.type )
		{
		case EventType.keyDown:
			OnKeyDown();
			break;
		}
	}

	void OnKeyDown()
	{
		if( IsFocusedControlNumericOnly() )
		{
			RestrictKeyboardInputToNumeric();
		}

		if( MaxlengthOfTextFieldReached )
		{
			DisallowCurrentKeyboardInput();
		}

		MaxlengthOfTextFieldReached = false;

		switch( Event.current.keyCode )
		{
		case KeyCode.Return:
		case KeyCode.KeypadEnter:
			AddCurrentTaskViaKeyDown = true;
			break;
		}
	}
}
