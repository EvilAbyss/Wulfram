using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public enum TodoListFieldTypes
{
	Description,
	Progress,
	Effort,
	Status,
	Priority,
	Sprint,
	Category,
	Developer,
	DueDate,
	DueTime,
	Count
}

public class TodoListView
{
	TodoListWindow Window;

	Vector2 TaskListScrollPosition;
	float TaskListScrollPositionYScroll = 0;

	Texture2D DragLineColorTexture;
	Texture2D DragTaskBackgroundTexture;

	Texture2D ArrowUpTexture;
	Texture2D ArrowDownTexture;
	Texture2D ArrowEmptyTexture;

	Texture2D ArrowUpActiveTexture;
	Texture2D ArrowDownActiveTexture;
	Texture2D ArrowEmptyActiveTexture;

	Texture2D MoveTaskTexture;

	Texture2D CompleteTaskTexture;
	Texture2D EditTaskTexture;

	Texture2D SearchBackgroundTexture;
	Texture2D SearchLeftTexture;
	Texture2D SearchRightTexture;
	Texture2D SearchRightTextTexture;

	Texture2D InfoTexture;

	int LastTime;

	bool IsDragging;
	int DragIndex;
	int TopBorderOfTasksArea = 85;
	TodoListTask DraggingTask;

	bool ResortList;

	string ActiveTimeInput;
	int CurrentlyEditingDateTimeKeyboardControl;

	string SearchString = "";
	bool ClearSearchStringPressed = false;

	public TodoListView( TodoListWindow window )
	{
		Window = window;
	}

	public void OnEnable()
	{
		if( Window.GetCurrentList() == null )
		{
			return;
		}

		Window.GetCurrentList().GetPersonalSettings().FieldSizes = new int[ (int)TodoListFieldTypes.Count ];

		int numStatuses = Window.GetCurrentList().GetWorkflowList().Count;
		int numCategories = Window.GetCurrentList().GetCategoriesList().Count + 1;
		int numDevelopers = Window.GetCurrentList().GetDevelopersList().Count + 1;
		int numPriorities = Window.GetCurrentList().GetPriorityList().Count + 1;
		int numSprints = Window.GetCurrentList().GetSprintList().Count + 1;

		if( Window.GetCurrentList().GetPersonalSettings().SelectedStatus == null || Window.GetCurrentList().GetPersonalSettings().SelectedStatus.Length != numStatuses )
		{
			Window.GetCurrentList().GetPersonalSettings().SelectedStatus = new bool[ numStatuses ];

			for( int i = 0; i < numStatuses; ++i )
			{
				Window.GetCurrentList().GetPersonalSettings().SelectedStatus[ i ] = true;
			}

			EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

			OnSearchChanged();
		}

		if( Window.GetCurrentList().GetPersonalSettings().SelectedCategories == null || Window.GetCurrentList().GetPersonalSettings().SelectedCategories.Length != numCategories )
		{
			Window.GetCurrentList().GetPersonalSettings().SelectedCategories = new bool[ numCategories ];

			for( int i = 0; i < numCategories; ++i )
			{
				Window.GetCurrentList().GetPersonalSettings().SelectedCategories[ i ] = true;
			}

			EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

			OnSearchChanged();
		}

		if( Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers == null || Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers.Length != numDevelopers )
		{
			Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers = new bool[ numDevelopers ];

			for( int i = 0; i < numDevelopers; ++i )
			{
				Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers[ i ] = true;
			}

			EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

			OnSearchChanged();
		}

		if( Window.GetCurrentList().GetPersonalSettings().SelectedPriority == null || Window.GetCurrentList().GetPersonalSettings().SelectedPriority.Length != numPriorities )
		{
			Window.GetCurrentList().GetPersonalSettings().SelectedPriority = new bool[ numPriorities ];

			for( int i = 0; i < numPriorities; ++i )
			{
				Window.GetCurrentList().GetPersonalSettings().SelectedPriority[ i ] = true;
			}

			EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

			OnSearchChanged();
		}

		if( Window.GetCurrentList().GetPersonalSettings().SelectedSprints == null || Window.GetCurrentList().GetPersonalSettings().SelectedSprints.Length != numSprints )
		{
			Window.GetCurrentList().GetPersonalSettings().SelectedSprints = new bool[ numSprints ];

			for( int i = 0; i < numSprints; ++i )
			{
				Window.GetCurrentList().GetPersonalSettings().SelectedSprints[ i ] = true;
			}

			EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

			OnSearchChanged();
		}

		UpdateTaskVisibility();
	}

	public void Update()
	{
		TaskListScrollPosition.y += TaskListScrollPositionYScroll;

		int nowUnix = TodoList.GetUnixTimestampNow();

		if( LastTime < nowUnix )
		{
			Window.Repaint();
			LastTime = nowUnix;
		}
	}

	void LoadTextures()
	{
		DragTaskBackgroundTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "DragTaskBackground", typeof( Texture2D ) );

		ArrowUpTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "ArrowUp", typeof( Texture2D ) );
		ArrowDownTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "ArrowDown", typeof( Texture2D ) );
		ArrowEmptyTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "ArrowEmpty", typeof( Texture2D ) );

		ArrowUpActiveTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "ArrowUpActive", typeof( Texture2D ) );
		ArrowDownActiveTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "ArrowDownActive", typeof( Texture2D ) );
		ArrowEmptyActiveTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "ArrowEmptyActive", typeof( Texture2D ) );

		MoveTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "MoveTask", typeof( Texture2D ) );

		CompleteTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Check", typeof( Texture2D ) );
		//DeleteTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Cross", typeof( Texture2D ) );
		EditTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Edit", typeof( Texture2D ) );

		InfoTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Info", typeof( Texture2D ) );

		SearchBackgroundTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "SearchBackground", typeof( Texture2D ) );
		SearchLeftTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "SearchLeft", typeof( Texture2D ) );
		//SearchLeftSelectedTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "SearchLeftSelected", typeof( Texture2D ) );
		SearchRightTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "SearchRight", typeof( Texture2D ) );
		SearchRightTextTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "SearchRightText", typeof( Texture2D ) );

		DragLineColorTexture = new Texture2D( 1, 1 );
		DragLineColorTexture.SetPixel( 0, 0, new Color( 0f, 0f, 0f ) );
		DragLineColorTexture.Apply();
		DragLineColorTexture.hideFlags = HideFlags.HideAndDontSave;
	}

	int GetFieldSize( TodoListFieldTypes type )
	{
		return Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)type ];
	}

	public void Display()
	{
		if( DragTaskBackgroundTexture == null )
		{
			LoadTextures();
		}

		OnEnable();

		UpdateFieldSizes();

		if( ResortList == true )
		{
			ResortList = false;
			Window.GetCurrentList().SortTaskList();
		}

		if( ClearSearchStringPressed == true )
		{
			ClearSearchStringPressed = false;
			SearchString = "";
			OnSearchChanged();
		}

		DisplayHeadingToolbar();

		DisplayTaskList();

		if( IsDragging )
		{
			GUIStyle floatingLabelStyle = new GUIStyle( EditorStyles.toolbarDropDown );
			floatingLabelStyle.normal.background = DragTaskBackgroundTexture;
			floatingLabelStyle.padding.left = 5;
			floatingLabelStyle.padding.right = 6;
			int dragY = DragIndex * 24 + 10 - (int)TaskListScrollPosition.y;

			GUI.DrawTexture( new Rect( 0, dragY, Screen.width - 15, 4 ), DragLineColorTexture, ScaleMode.StretchToFill );

			int length = Mathf.CeilToInt( floatingLabelStyle.CalcSize( new GUIContent( DraggingTask.Description ) ).x );
			GUI.Label( new Rect( TodoList.NumberAreaWidth + 5, dragY - 8, length, 18 ), DraggingTask.Description, floatingLabelStyle );
			Window.Repaint();
		}
	}

	Texture2D GetArrowTextureForHeader( TodoListSortType ascSortType, TodoListSortType descSortType, bool active )
	{
		if( Window.GetCurrentList().GetSortType() == ascSortType )
		{
			if( active )
			{
				return ArrowUpActiveTexture;
			}
			else
			{
				return ArrowUpTexture;
			}
		}
		else if( Window.GetCurrentList().GetSortType() == descSortType )
		{
			if( active )
			{
				return ArrowDownActiveTexture;
			}
			else
			{
				return ArrowDownTexture;
			}
		}
		else
		{
			if( active )
			{
				return ArrowEmptyActiveTexture;
			}
			else
			{
				return ArrowEmptyTexture;
			}
		}
	}

	void ChangeSortType( TodoListSortType ascType, TodoListSortType descType )
	{
		GUIUtility.keyboardControl = 0;

		if( Window.GetCurrentList().GetSortType() == ascType )
		{
			Window.GetCurrentList().SortTaskList( descType );
		}
		else
		{
			Window.GetCurrentList().SortTaskList( ascType );
		}
	}

	void DisplayHeadingToolbar()
	{
		GUILayout.Space( 5 );

		GUIStyle toolbarStyle = new GUIStyle( EditorStyles.toolbar );
		toolbarStyle.padding.left = 0;

		EditorGUILayout.BeginHorizontal( toolbarStyle );
		{
			GUIStyle labelStyle;

			if( Window.GetCurrentList().GetSortType() == TodoListSortType.OrderAsc || Window.GetCurrentList().GetSortType() == TodoListSortType.OrderDesc )
			{
				labelStyle = new GUIStyle( EditorStyles.toolbarDropDown );

				labelStyle.padding.left = 6;
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.OrderAsc, TodoListSortType.OrderDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.OrderAsc, TodoListSortType.OrderDesc, true );
			}
			else
			{
				labelStyle = new GUIStyle( EditorStyles.toolbarButton );

				labelStyle.padding.left = 10;
			}

			labelStyle.fixedWidth = TodoList.NumberAreaWidth;
			labelStyle.alignment = TextAnchor.MiddleLeft;

			if( GUILayout.Button( "#", labelStyle ) )
			{
				ChangeSortType( TodoListSortType.OrderAsc, TodoListSortType.OrderDesc );
			}

			labelStyle = new GUIStyle( EditorStyles.toolbarDropDown );
			labelStyle.alignment = TextAnchor.MiddleLeft;
			labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.DescriptionAsc, TodoListSortType.DescriptionDesc, false );
			labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.DescriptionAsc, TodoListSortType.DescriptionDesc, true );

			GUIStyle dropdownStyle = new GUIStyle( EditorStyles.toolbarPopup );
			dropdownStyle.alignment = TextAnchor.MiddleLeft;

			DisplayHeadingToolbarDescription( labelStyle );

			if( GetFieldSize( TodoListFieldTypes.Progress ) > 0 )
			{
				GUIStyle progressLabelStyle = new GUIStyle( EditorStyles.toolbarButton );
				progressLabelStyle.alignment = TextAnchor.MiddleLeft;

				GUILayout.Label( "Progress", progressLabelStyle, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Progress ) ) );
			}

			if( GetFieldSize( TodoListFieldTypes.Effort ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.EffortAsc, TodoListSortType.EffortDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.EffortAsc, TodoListSortType.EffortDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.Effort );

				if( GUILayout.Button( "Effort", labelStyle ) )
				{
					ChangeSortType( TodoListSortType.EffortAsc, TodoListSortType.EffortDesc );
				}
			}

			if( GetFieldSize( TodoListFieldTypes.Status ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.StatusAsc, TodoListSortType.StatusDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.StatusAsc, TodoListSortType.StatusDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.Status ) - 12;

				if( GUILayout.Button( "Status", labelStyle ) )
				{
					ChangeSortType( TodoListSortType.StatusAsc, TodoListSortType.StatusDesc );
				}

				MultipleChoicePopup( "Status", Window.GetCurrentList().GetWorkflowStringList(), ref Window.GetCurrentList().GetPersonalSettings().SelectedStatus, 12, false );
			}

			if( GetFieldSize( TodoListFieldTypes.Priority ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.PriorityAsc, TodoListSortType.PriorityDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.PriorityAsc, TodoListSortType.PriorityDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.Priority ) - 12;

				if( GUILayout.Button( "Priority", labelStyle ) )
				{
					ChangeSortType( TodoListSortType.PriorityAsc, TodoListSortType.PriorityDesc );
				}

				MultipleChoicePopup( "Priority", Window.GetCurrentList().GetPriorityStringList(), ref Window.GetCurrentList().GetPersonalSettings().SelectedPriority, 12 );
			}

			if( GetFieldSize( TodoListFieldTypes.Sprint ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.SprintAsc, TodoListSortType.SprintDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.SprintAsc, TodoListSortType.SprintDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.Sprint ) - 12;

				if( GUILayout.Button( "Sprint", labelStyle ) )
				{
					ChangeSortType( TodoListSortType.SprintAsc, TodoListSortType.SprintDesc );
				}

				MultipleChoicePopup( "Sprint", Window.GetCurrentList().GetSprintsStringList(), ref Window.GetCurrentList().GetPersonalSettings().SelectedSprints, 12 );
			}

			if( GetFieldSize( TodoListFieldTypes.Category ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.CategoryAsc, TodoListSortType.CategoryDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.CategoryAsc, TodoListSortType.CategoryDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.Category ) - 12;

				if( GUILayout.Button( "Category", labelStyle ) )
				{
					ChangeSortType( TodoListSortType.CategoryAsc, TodoListSortType.CategoryDesc );
				}

				MultipleChoicePopup( "Category", Window.GetCurrentList().GetCategoriesStringList(), ref Window.GetCurrentList().GetPersonalSettings().SelectedCategories, 12 );
			}

			if( GetFieldSize( TodoListFieldTypes.Developer ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.DeveloperAsc, TodoListSortType.DeveloperDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.DeveloperAsc, TodoListSortType.DeveloperDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.Developer ) - 12;

				if( GUILayout.Button( "Developer", labelStyle ) )
				{
					ChangeSortType( TodoListSortType.DeveloperAsc, TodoListSortType.DeveloperDesc );
				}

				MultipleChoicePopup( "Developer", Window.GetCurrentList().GetDevelopersStringList(), ref Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers, 12 );
			}

			if( GetFieldSize( TodoListFieldTypes.DueDate ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.DueDateAsc, TodoListSortType.DueDateDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.DueDateAsc, TodoListSortType.DueDateDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.DueDate );

				if( GUILayout.Button( TodoListFieldTypes.DueDate.ToString(), labelStyle ) )
				{
					ChangeSortType( TodoListSortType.DueDateAsc, TodoListSortType.DueDateDesc );
				}
			}

			if( GetFieldSize( TodoListFieldTypes.DueTime ) > 0 )
			{
				labelStyle.normal.background = GetArrowTextureForHeader( TodoListSortType.DueTimeAsc, TodoListSortType.DueTimeDesc, false );
				labelStyle.active.background = GetArrowTextureForHeader( TodoListSortType.DueTimeAsc, TodoListSortType.DueTimeDesc, true );
				labelStyle.fixedWidth = GetFieldSize( TodoListFieldTypes.DueTime );

				if( GUILayout.Button( TodoListFieldTypes.DueTime.ToString(), labelStyle ) )
				{
					ChangeSortType( TodoListSortType.DueTimeAsc, TodoListSortType.DueTimeDesc );
				}
			}

			GUILayout.FlexibleSpace();
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayHeadingToolbarDescription( GUIStyle labelStyle )
	{
		labelStyle.fixedWidth = 80;

		if( GUILayout.Button( TodoListFieldTypes.Description.ToString(), labelStyle ) )
		{
			ChangeSortType( TodoListSortType.DescriptionAsc, TodoListSortType.DescriptionDesc );
		}

		int drawTextureY = 67;

		if( GetFieldSize( TodoListFieldTypes.DueDate ) == 0 && GetFieldSize( TodoListFieldTypes.DueTime ) == 0 && GetFieldSize(	TodoListFieldTypes.Progress ) == 0 )
		{
			drawTextureY -= 3;

			if( TodoList.IsUsingProSkin() )
			{
				drawTextureY -= 1;

				if( GetFieldSize( TodoListFieldTypes.Status ) == 0 )
				{
					if( GetFieldSize( TodoListFieldTypes.Category ) == 0 && GetFieldSize( TodoListFieldTypes.Developer ) == 0 )
					{
						drawTextureY += 1;
					}
				}
			}
		}

		GUI.DrawTexture( new Rect( 104, drawTextureY, EditorStyles.toolbar.normal.background.width, EditorStyles.toolbar.normal.background.height ), EditorStyles.toolbar.normal.background );

		GUIStyle style = EditorStyles.toolbarTextField;
		style.fixedWidth = GetFieldSize( TodoListFieldTypes.Description ) - 1 - 88 - SearchLeftTexture.width - SearchRightTexture.width;
		style.normal.background = SearchBackgroundTexture;

		GUI.DrawTexture( new Rect( 109, drawTextureY + 2, SearchLeftTexture.width, SearchLeftTexture.height ), SearchLeftTexture );
		GUILayout.Space( SearchLeftTexture.width );

		//string newSearchString = GUILayout.TextField( SearchString, EditorStyles.toolbarTextField );
		string newSearchString = EditorGUILayout.TextField( SearchString, EditorStyles.toolbarTextField, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Description ) - 113 ) );

		if( GUI.changed && newSearchString != SearchString )
		{
			SearchString = newSearchString;
			OnSearchChanged();
		}

		EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Text );

		Texture2D rightTexture = SearchRightTexture;

		if( SearchString != "" )
		{
			rightTexture = SearchRightTextTexture;
		}

		Rect rightTextureDrawRect = new Rect( GetFieldSize( TodoListFieldTypes.Description ) + 7, drawTextureY + 2, SearchRightTexture.width, SearchRightTexture.height );
		GUI.DrawTexture( rightTextureDrawRect, rightTexture );

		GUILayout.Space( SearchRightTexture.width );

		if( Event.current.type == EventType.MouseDown )
		{
			if( rightTextureDrawRect.Contains( Event.current.mousePosition ) )
			{
				ClearSearchStringPressed = true;
			}
		}
	}

	void UpdateTaskVisibility()
	{
		foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
		{
			task.IsMatchingVisibilityOptions = true;

			if( task.IsDone() == true && Window.GetDisplayOption( TodoListDisplayOptions.ShowCompletedTasks ) == false )
			{
				task.IsMatchingVisibilityOptions = false;
				continue;
			}

			int statusIndex = Window.GetCurrentList().GetWorkflowList().FindIndex( item => item.Description == task.Status );
			if( statusIndex == -1 || Window.GetCurrentList().GetPersonalSettings().SelectedStatus[ statusIndex ] == false )
			{
				task.IsMatchingVisibilityOptions = false;
				continue;
			}

			int categoryIndex = Window.GetCurrentList().GetCategoriesList().IndexOf( task.Category ) + 1;
			if( categoryIndex == -1 || Window.GetCurrentList().GetPersonalSettings().SelectedCategories[ categoryIndex ] == false )
			{
				task.IsMatchingVisibilityOptions = false;
				continue;
			}

			int priorityIndex = Window.GetCurrentList().GetPriorityList().IndexOf( task.Priority ) + 1;
			if( priorityIndex == -1 || Window.GetCurrentList().GetPersonalSettings().SelectedPriority[ priorityIndex ] == false )
			{
				task.IsMatchingVisibilityOptions = false;
				continue;
			}

			int developerIndex = Window.GetCurrentList().GetDevelopersList().IndexOf( task.Developer ) + 1;
			if( developerIndex == -1 || Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers[ developerIndex ] == false )
			{
				task.IsMatchingVisibilityOptions = false;
				continue;
			}

			int sprintIndex = Window.GetCurrentList().GetSprintList().FindIndex( item => item.SprintIndex == task.SprintIndex ) + 1;
			if( sprintIndex == -1 || Window.GetCurrentList().GetPersonalSettings().SelectedSprints[ sprintIndex ] == false )
			{
				task.IsMatchingVisibilityOptions = false;
				continue;
			}
		}
	}

	void DisplayTaskList()
	{
		if( Event.current.type == EventType.mouseUp || Event.current.type == EventType.ignore )
		{
			if( IsDragging )
			{
				OnStopDrag();
			}
		}

		TaskListScrollPosition = EditorGUILayout.BeginScrollView( TaskListScrollPosition );
		{
			int taskNr = 1;

			GUIStyle taskNrStyle = new GUIStyle( EditorStyles.label );
			taskNrStyle.alignment = TextAnchor.MiddleCenter;
			taskNrStyle.padding.bottom = 2;
			taskNrStyle.padding.left = 0;
			taskNrStyle.padding.right = 5;
			taskNrStyle.margin.right = 5;

			if( IsDragging == false )
			{
				taskNrStyle.hover.background = MoveTaskTexture;
				taskNrStyle.hover.textColor = Color.red;
			}

			taskNrStyle.fixedHeight = 20;
			taskNrStyle.fixedWidth = TodoList.NumberAreaWidth;

			GUIStyle labelStyle = new GUIStyle( EditorStyles.label );
			labelStyle.alignment = TextAnchor.MiddleLeft;
			labelStyle.padding.top = 3;

			GUIStyle descStyle = new GUIStyle( labelStyle );

			GUIStyle horizontalRow = new GUIStyle();
			horizontalRow.padding.bottom = 0;
			horizontalRow.padding.top = 0;

			GUIStyle buttonStyle = new GUIStyle( "button" );
			buttonStyle.fixedWidth = 19;
			buttonStyle.fixedHeight = 15;
			buttonStyle.margin.left = 2;
			buttonStyle.margin.right = 2;
			buttonStyle.margin.top = 4;

			int occludedTasks = 0;

			if( TaskListScrollPosition.y < 0 )
			{
				TaskListScrollPosition.y = 0;
			}

			foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
			{
				if( task.IsMatchingVisibilityOptions == false )
				{
					continue;
				}

				if( task.IsMatchingSearchString == false )
				{
					continue;
				}

				horizontalRow.normal.background = TodoList.GetTaskBackground( task, taskNr );

				EditorGUILayout.BeginHorizontal( horizontalRow );
				{
					GUILayout.Box( task.Order.ToString(), taskNrStyle );

					EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Link );

					if( Event.current.type == EventType.mouseDown )
					{
						if( GUILayoutUtility.GetLastRect().Contains( Event.current.mousePosition ) )
						{
							Event.current.Use();
							OnStartDrag( task );
							break;
						}
					}

					int taskHeight = taskNr * 24;


					if( taskHeight < TaskListScrollPosition.y || taskHeight > Screen.height - 108 + TaskListScrollPosition.y + 24 )
					{
						occludedTasks++;
					}
					else
					{
						DisplayTaskDesc( task, descStyle );

						if( GetFieldSize( TodoListFieldTypes.Progress ) > 0 )
						{
							DisplayTaskProgress( task );
						}

						if( GetFieldSize( TodoListFieldTypes.Effort ) > 0 )
						{
							DisplayTaskEffort( task );
						}

						if( GetFieldSize( TodoListFieldTypes.Status ) > 0 )
						{
							DisplayTaskStatus( task );
						}

						if( GetFieldSize( TodoListFieldTypes.Priority ) > 0 )
						{
							DisplayTaskPriority( task );
						}

						if( GetFieldSize( TodoListFieldTypes.Sprint ) > 0 )
						{
							DisplayTaskSprint( task );
						}

						if( GetFieldSize( TodoListFieldTypes.Category ) > 0 )
						{
							DisplayTaskCategory( task );
						}

						if( GetFieldSize( TodoListFieldTypes.Developer ) > 0 )
						{
							DisplayTaskDeveloper( task );
						}

						if( GetFieldSize( TodoListFieldTypes.DueDate ) > 0 )
						{
							DisplayDueDate( task );
						}

						if( GetFieldSize( TodoListFieldTypes.DueTime ) > 0 )
						{
							DisplayDueTime( task );
						}

						TodoListWorkflow defaultCompleted = Window.GetCurrentList().GetWorkflowList().Find( item => item.Type == TodoListWorkflowType.DefaultFinishedStatus );

						bool preGuiStatus = GUI.enabled;
						if( defaultCompleted == null )
						{
							GUI.enabled = false;
						}

						if( GUILayout.Button( new GUIContent( CompleteTaskTexture ), buttonStyle ) )
						{
							Window.GetCurrentList().OnTaskChanged( task, "Task Completed" );
							GUIUtility.keyboardControl = 0;

							if( task.Status != defaultCompleted.Description )
							{
								task.SetCompleted( true );
								task.Status = defaultCompleted.Description;
								task.AddPost( "", task.Status );
							}
							else
							{
								task.SetCompleted( false );
								task.AddPost( "", task.Status );
							}
						}

						if( defaultCompleted == null )
						{
							GUI.enabled = preGuiStatus;
						}

						if( GUILayout.Button( new GUIContent( EditTaskTexture ), buttonStyle ) )
						{
							GUIUtility.keyboardControl = 0;
							TodoListEditTaskWindow window = (TodoListEditTaskWindow)EditorWindow.GetWindow( typeof( TodoListEditTaskWindow ), false, "Edit Task" );
							window.SetTask( task );
							window.SetTodoListWindow( Window );
						}
					}
					/*if( GUILayout.Button( new GUIContent( DeleteTaskTexture ), buttonStyle ) )
					{
						GUIUtility.keyboardControl = 0;
						EditorApplication.Beep();
						if( EditorUtility.DisplayDialog( "Please Confirm", "Do you really want to delete the task '" + task.Description + "'?", "Yes", "No" ) )
						{
							Window.GetCurrentList().RegisterUndo( "Delete Task" );
							Window.GetCurrentList().OnTaskDeleted( task );
							Window.GetCurrentList().GetTaskList().Remove( task );
							Window.Repaint();
							return;
						}
					}*/
				}
				EditorGUILayout.EndHorizontal();

				taskNr++;
			}

		}
		DisplayDidYouKnow();

		EditorGUILayout.EndScrollView();

		if( Event.current != null )
		{
			if( Event.current.type == EventType.MouseDown )
			{
				GUIUtility.keyboardControl = 0;
				Window.Repaint();
			}
		}

		if( Event.current.type == EventType.mouseDrag )
		{
			if( IsDragging )
			{
				OnDrag();
			}
		}
	}

	void DisplayDidYouKnow()
	{
		float didYouKnowVersion = EditorPrefs.GetFloat( "TodoListDidYouKnow", 0 );

		if( didYouKnowVersion == 0 )
		{
			GUILayout.FlexibleSpace();

			GUIStyle style = TodoList.GetWarningBoxStyle();
			EditorGUILayout.BeginHorizontal( style );
			{
				GUILayout.Box( new GUIContent( "Did you know? You can toggle this window with CTRL + T on Windows or CMD + T on MacOS", InfoTexture ), EditorStyles.label );
				GUILayout.FlexibleSpace();

				if( GUILayout.Button( "Thanks", GUILayout.Width( 100 ) ) )
				{
					EditorPrefs.SetFloat( "TodoListDidYouKnow", TodoList.Version );
				}
			}
		}
	}

	void DisplayTaskDesc( TodoListTask task, GUIStyle style )
	{
		int id = GUIUtility.GetControlID( FocusType.Keyboard );

		if( GUIUtility.keyboardControl == id + 1 )
		{
			style = EditorStyles.textField;

			if( Event.current.type == EventType.keyDown )
			{
				if( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter )
				{
					GUIUtility.keyboardControl = 0;
					Window.Repaint();
				}
			}
		}

		int widthModifier = 6;

		if( GetFieldSize( TodoListFieldTypes.Progress ) == 0 && GetFieldSize( TodoListFieldTypes.Effort ) > 0 )
		{
			widthModifier = 8;
		}
		string newDesc = GUILayout.TextField( task.Description, style, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Description ) - widthModifier ) );

		if( IsDragging == false )
		{
			EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Text );
		}

		if( newDesc != task.Description )
		{
			Window.GetCurrentList().RegisterUndo( "Edit Task Description" );
			task.Description = newDesc;
			Window.GetCurrentList().OnTaskChanged( task );
			ResortList = true;
		}
	}

	void DisplayTaskProgress( TodoListTask task )
	{
		GUIStyle style = new GUIStyle();
		style.fixedWidth = GetFieldSize( TodoListFieldTypes.Progress ) - 4;
		style.fixedHeight = 24;

		EditorGUILayout.BeginHorizontal( style, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Progress ) - 4 ) );
		{
			float newValue = GUILayout.HorizontalSlider( task.Progress, 0f, 1f, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Progress ) - 8 - 35 ) );

			GUILayout.Label( Mathf.FloorToInt( newValue * 100 ) + "%", GUILayout.Width( 35 ) );
			if( newValue != task.Progress )
			{
				Window.GetCurrentList().RegisterUndo( "Edit Task Progress" );
				task.Progress = newValue;
				Window.GetCurrentList().OnTaskChanged( task );
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayTaskEffort( TodoListTask task )
	{
		if( GetFieldSize( TodoListFieldTypes.Progress ) > 0 )
		{
			GUILayout.Space( 2 );
		}

		TodoList.DisplayTaskEffort( task, GetFieldSize( TodoListFieldTypes.Effort ) - 6, Window );

		GUILayout.Space( 2 );
	}

	void DisplayTaskStatus( TodoListTask task )
	{
		GUILayout.Label( task.Status, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Status ) - 28 ) );

		List<string> transitions = Window.GetCurrentList().GetTransitions( task.Status );
		transitions.Insert( 0, "" );

		GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
		popupStyle.margin.top = 4;
		popupStyle.margin.right = 8;

		int selectedIndex = 0;

		if( transitions.Count > 1 )
		{
			selectedIndex = EditorGUILayout.Popup( 0, transitions.ToArray(), popupStyle, GUILayout.Width( 16 ) );
		}
		else
		{
			GUILayout.Space( 24 );
		}
		
		if( selectedIndex != 0 )
		{
			Window.GetCurrentList().OnTaskChanged( task, "Edit Task Status" );

			task.SetCompleted( Window.GetCurrentList().IsStatusFinished( transitions[ selectedIndex ] ) );

			task.Status = transitions[ selectedIndex ];

			task.AddPost( "", transitions[ selectedIndex ], "" );
		}
	}

	void DisplayTaskPriority( TodoListTask task )
	{
		string[] options = Window.GetCurrentList().GetPriorityStringList();

		int currentIndex = Window.GetCurrentList().GetPriorityList().IndexOf( task.Priority ) + 1;

		GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
		popupStyle.margin.top = 4;

		int newIndex = EditorGUILayout.Popup( currentIndex, options, popupStyle, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Priority ) - 8 ) );
		GUILayout.Space( 4 );
		if( newIndex != currentIndex )
		{
			Window.GetCurrentList().OnTaskChanged( task, "Edit Task Priority" );

			if( newIndex == 0 )
			{
				task.Priority = "Unassigned";
			}
			else
			{
				task.Priority = Window.GetCurrentList().GetPriorityList()[ newIndex - 1 ];
			}
		}
	}

	void DisplayTaskSprint( TodoListTask task )
	{
		TodoList.DisplayTaskSprint( task, TodoList.SprintFieldWidth - 6, Window );
	}

	void DisplayTaskCategory( TodoListTask task )
	{
		string[] options = Window.GetCurrentList().GetCategoriesStringList();

		int currentIndex = Window.GetCurrentList().GetCategoriesList().IndexOf( task.Category ) + 1;

		GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
		popupStyle.margin.top = 4;

		int newIndex = EditorGUILayout.Popup( currentIndex, options, popupStyle, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Category ) - 8 ) );
		GUILayout.Space( 4 );
		if( newIndex != currentIndex )
		{
			Window.GetCurrentList().OnTaskChanged( task, "Edit Task Category" );
			
			if( newIndex == 0 )
			{
				task.Category = "Unassigned";
			}
			else
			{
				task.Category = Window.GetCurrentList().GetCategoriesList()[ newIndex - 1 ];
			}
		}
	}

	void DisplayTaskDeveloper( TodoListTask task )
	{
		string[] options = Window.GetCurrentList().GetDevelopersStringList();

		int currentIndex = Window.GetCurrentList().GetDevelopersList().IndexOf( task.Developer ) + 1;

		GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
		popupStyle.margin.top = 4;

		int newIndex = EditorGUILayout.Popup( currentIndex, options, popupStyle, GUILayout.Width( GetFieldSize( TodoListFieldTypes.Developer ) - 8 ) );

		GUILayout.Space( 4 );
		if( newIndex != currentIndex )
		{
			Window.GetCurrentList().OnTaskChanged( task, "Edit Task Developer" );

			if( newIndex == 0 )
			{
				task.Developer = "Unassigned";
			}
			else
			{
				task.Developer = Window.GetCurrentList().GetDevelopersList()[ newIndex - 1 ];
			}

			task.AddPost( "", "", task.Developer );
		}
	}

	bool IsCharAllowed( char inputChar, char[] allowedChars )
	{
		foreach( char c in allowedChars )
		{
			if( inputChar == c )
			{
				return true;
			}
		}

		return false;
	}

	void DisplayDueDate( TodoListTask task )
	{
		DisplayDueDateTime( task, true );
	}

	void DisplayDueTime( TodoListTask task )
	{
		DisplayDueDateTime( task, false );
	}

	void DisplayDueDateTime( TodoListTask task, bool displayDate )
	{
		int id = GUIUtility.GetControlID( FocusType.Keyboard );

		if( CurrentlyEditingDateTimeKeyboardControl == id + 1 && GUIUtility.keyboardControl != id + 1 )
		{
			CurrentlyEditingDateTimeKeyboardControl = 0;
		}

		GUIStyle style = new GUIStyle( EditorStyles.label );
		style.margin.top = 4;

		if( GUIUtility.keyboardControl == id + 1 )
		{
			style = EditorStyles.textField;

			if( Event.current.type == EventType.keyDown )
			{
				if( Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter )
				{
					GUIUtility.keyboardControl = 0;
					style = EditorStyles.label;
					Window.Repaint();
				}
				else
				{
					bool notAllowed = false;

					char[] allowedChars;

					if( displayDate )
					{
						allowedChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };
					}
					else
					{
						allowedChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', ':' };
					}

					if( Event.current.character != '\0' && IsCharAllowed( Event.current.character, allowedChars ) == false )
					{
						notAllowed = true;
					}

					if( displayDate )
					{
						if( ActiveTimeInput.Length > 10 )
						{
							notAllowed = true;
						}
					}
					else
					{
						if( ActiveTimeInput.Length > 5 )
						{
							notAllowed = true;
						}
					}

					if( notAllowed )
					{
						EditorApplication.Beep();
						Event.current.character = '\0';
					}
				}
			}
		}

		string display = "";

		if( displayDate == true )
		{
			display = task.DueDate.ToString();

			if( display == "0" )
			{
				display = "";
			}
			else
			{
				System.DateTime date = TodoList.FromUnixTimestamp( task.DueDate );
				display = date.Day.ToString( "00" ) + "." + date.Month.ToString( "00" ) + "." + date.Year.ToString( "0000" );
			}
		}
		else
		{
			display = task.DueTime.ToString();

			if( display == "0" )
			{
				display = "";
			}
			else
			{
				System.DateTime date = TodoList.FromUnixTimestamp( task.DueTime );
				display = date.Hour.ToString( "00" ) + ":" + date.Minute.ToString( "00" );
			}
		}

		int width = GetFieldSize( TodoListFieldTypes.DueDate );

		if( displayDate == false )
		{
			width = GetFieldSize( TodoListFieldTypes.DueTime );
		}

		string myDisplay = display;

		if( GUIUtility.keyboardControl == id + 1 )
		{
			if( CurrentlyEditingDateTimeKeyboardControl != GUIUtility.keyboardControl )
			{
				CurrentlyEditingDateTimeKeyboardControl = GUIUtility.keyboardControl;

				ActiveTimeInput = display;
			}

			if( ActiveTimeInput == null )
			{
				ActiveTimeInput = "";
			}

			myDisplay = ActiveTimeInput;
		}

		myDisplay = GUILayout.TextField( myDisplay, style, GUILayout.Width( width - 4 ) );

		if( GUI.changed && GUIUtility.keyboardControl == id + 1 && id > -1 )
		{
			ActiveTimeInput = myDisplay;

			if( displayDate )
			{
				if( ActiveTimeInput == "" )
				{
					task.DueDate = 0;
				}
				else
				{
					System.DateTime date = System.DateTime.Now;
					bool error = false;
					try
					{
						System.IFormatProvider culture = new System.Globalization.CultureInfo( "de-DE", true );
						date = System.DateTime.Parse( ActiveTimeInput, culture );
					}
					catch
					{
						error = true;
					}

					if( error == false )
					{
						Window.GetCurrentList().RegisterUndo( "Edit Due Date" );

						task.DueDate = TodoList.ToUnixTimestamp( date.Year, date.Month, date.Day );
						
						ResortList = true;
					}
				}
			}
			else
			{
				if( ActiveTimeInput == "" )
				{
					task.DueTime = 0;
				}
				else
				{
					System.DateTime date = System.DateTime.Now;
					bool error = false;
					try
					{
						string time = ActiveTimeInput;

						if( time.Length == 2 )
						{
							time += ":00";
						}
						else if( time.Length == 3 && time.Contains( ":" ) == false )
						{
							time = time.Substring( 0, 2 ) + ":" + time.Substring( 2, 1 ) + "0";
						}
						else if( time.Length == 4 && time.Contains( ":" ) == false )
						{
							time = time.Substring( 0, 2 ) + ":" + time.Substring( 2, 2 );
						}

						time = "1.1.1970 " + time;

						System.IFormatProvider culture = new System.Globalization.CultureInfo( "de-DE", true );
						date = System.DateTime.Parse( time, culture );
					}
					catch
					{
						error = true;
					}

					if( error == false )
					{
						Window.GetCurrentList().RegisterUndo( "Edit Due Time" );
						task.DueTime = TodoList.ToUnixTimestamp( 1970, 1, 1, date.Hour, date.Minute );
						ResortList = true;
					}
				}
			}
		}

		if( IsDragging == false )
		{
			EditorGUIUtility.AddCursorRect( GUILayoutUtility.GetLastRect(), MouseCursor.Text );
		}
	}

	void UpdateFieldSizes()
	{
		if( Window.GetCurrentList() == null )
		{
			return;
		}

		int totalSize = Screen.width;

		for( int i = 0; i < (int)TodoListFieldTypes.Count; ++i )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ i ] = 0;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowProgress ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Progress ] = TodoList.ProgressFieldWidth;
			totalSize -= TodoList.ProgressFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowEffort ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Effort ] = TodoList.EffortFieldWidth;
			totalSize -= TodoList.EffortFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowStatus ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Status ] = TodoList.StatusFieldWidth;
			totalSize -= TodoList.StatusFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowPriority ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Priority ] = TodoList.PriorityFieldWidth;
			totalSize -= TodoList.PriorityFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowSprint ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Sprint ] = TodoList.SprintFieldWidth;
			totalSize -= TodoList.SprintFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowCategories ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Category ] = TodoList.CategoryFieldWidth;
			totalSize -= TodoList.CategoryFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDevelopers ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Developer ] = TodoList.DeveloperFieldWidth;
			totalSize -= TodoList.DeveloperFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDueDate ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.DueDate ] = TodoList.DueDateFieldWidth;
			totalSize -= TodoList.DueDateFieldWidth;
		}

		if( Window.GetDisplayOption( TodoListDisplayOptions.ShowDueTime ) )
		{
			Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.DueTime ] = TodoList.DueTimeFieldWidth;
			totalSize -= TodoList.DueTimeFieldWidth;
		}

		totalSize -= TodoList.NumberAreaWidth;
		totalSize -= TodoList.ButtonAreaWidth;

		Window.GetCurrentList().GetPersonalSettings().FieldSizes[ (int)TodoListFieldTypes.Description ] = totalSize - 8;
	}

	private void ToggleDevelopersDelegate( object userData, string[] options, int selected )
	{
		if( selected > 1 )
		{
			selected--;
		}

		Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers[ selected ] = !Window.GetCurrentList().GetPersonalSettings().SelectedDevelopers[ selected ];

		EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

		UpdateTaskVisibility();
	}

	private void TogglePriorityDelegate( object userData, string[] options, int selected )
	{
		if( selected > 1 )
		{
			selected--;
		}

		Window.GetCurrentList().GetPersonalSettings().SelectedPriority[ selected ] = !Window.GetCurrentList().GetPersonalSettings().SelectedPriority[ selected ];

		EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

		UpdateTaskVisibility();
	}

	private void ToggleSprintDelegate( object userData, string[] options, int selected )
	{
		if( selected > 1 )
		{
			selected--;
		}

		Window.GetCurrentList().GetPersonalSettings().SelectedSprints[ selected ] = !Window.GetCurrentList().GetPersonalSettings().SelectedSprints[ selected ];

		EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

		UpdateTaskVisibility();
	}

	private void ToggleCategoriesDelegate( object userData, string[] options, int selected )
	{
		if( selected > 1 )
		{
			selected--;
		}

		Window.GetCurrentList().GetPersonalSettings().SelectedCategories[ selected ] = !Window.GetCurrentList().GetPersonalSettings().SelectedCategories[ selected ];

		EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

		UpdateTaskVisibility();
	}

	private void ToggleStatusDelegate( object userData, string[] options, int selected )
	{
		Window.GetCurrentList().GetPersonalSettings().SelectedStatus[ selected ] = !Window.GetCurrentList().GetPersonalSettings().SelectedStatus[ selected ];

		EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

		UpdateTaskVisibility();
	}

	public void MultipleChoicePopup( string desc, string[] NameValues, ref bool[] SelectedValues, int width, bool addHr = true )
	{
		List<string> layers = new List<string>();
		List<int> selectedIndices = new List<int>();

		/*if( Event.current.type != EventType.MouseDown && Event.current.type != EventType.ExecuteCommand )
		{
			layers.Add( desc );
		}*/

		for( int i = 0; i < NameValues.Length; ++i )
		{
			//string prefix = "[  ] ";
			if( SelectedValues[ i ] == true )
			{
				//prefix = "[X] ";

				int a = i;

				if( addHr )
				{
					if( i >= 1 )
					{
						a++;
					}
				}

				selectedIndices.Add( a );
			}

			layers.Add( /*prefix + */NameValues[ i ] );
		}

		if( addHr )
		{
			layers.Insert( 1, "" );
		}

		bool preChange = GUI.changed;

		GUI.changed = false;

		GUIStyle dropdownStyle = new GUIStyle( EditorStyles.toolbarPopup );
		dropdownStyle.alignment = TextAnchor.MiddleLeft;

		//newSelected = EditorGUILayout.Popup( newSelected, layers.ToArray(), dropdownStyle, GUILayout.Width( width ) );
		if( GUILayout.Button( desc, dropdownStyle, GUILayout.Width( width ) ) )
		{
			int left = Screen.width - 185 - GetFieldSize( TodoListFieldTypes.DueDate ) - GetFieldSize( TodoListFieldTypes.DueTime );

			if( desc == "Developer" )
			{
				left += 50;
			}
			else if( desc == "Category" )
			{
				left -= GetFieldSize( TodoListFieldTypes.Developer );
				left += 50;
			}
			else if( desc == "Sprint" )
			{
				left -= GetFieldSize( TodoListFieldTypes.Category );
				left -= GetFieldSize( TodoListFieldTypes.Developer );
				left += 50;
			}

			else if( desc == "Priority" )
			{
				left -= GetFieldSize( TodoListFieldTypes.Category );
				left -= GetFieldSize( TodoListFieldTypes.Developer );
				left -= GetFieldSize( TodoListFieldTypes.Sprint );
				left += 50;
			}
			else if( desc == "Status" )
			{
				left -= GetFieldSize( TodoListFieldTypes.Category );
				left -= GetFieldSize( TodoListFieldTypes.Developer );
				left -= GetFieldSize( TodoListFieldTypes.Priority );
				left -= GetFieldSize( TodoListFieldTypes.Sprint );
			}

			object[] param = new object[ 5 ];
			param[ 0 ] = new Rect( left, 64, 1, 1 );
			param[ 1 ] = layers.ToArray();
			param[ 2 ] = selectedIndices.ToArray();

			if( desc == "Category" )
			{
				param[ 3 ] = new EditorUtility.SelectMenuItemFunction( ToggleCategoriesDelegate );
			}
			else if( desc == "Developer" )
			{
				param[ 3 ] = new EditorUtility.SelectMenuItemFunction( ToggleDevelopersDelegate );
			}
			else if( desc == "Priority" )
			{
				param[ 3 ] = new EditorUtility.SelectMenuItemFunction( TogglePriorityDelegate );
			}
			else if( desc == "Sprint" )
			{
				param[ 3 ] = new EditorUtility.SelectMenuItemFunction( ToggleSprintDelegate );
			}
			else
			{
				param[ 3 ] = new EditorUtility.SelectMenuItemFunction( ToggleStatusDelegate );
			}

			param[ 4 ] = null;

			typeof( EditorUtility ).InvokeMember( "DisplayCustomMenu", BindingFlags.InvokeMethod | BindingFlags.NonPublic, null, typeof( EditorUtility ), param );
		}

		GUI.changed = preChange;
	}

	void OnStartDrag( TodoListTask task )
	{
		IsDragging = true;
		DraggingTask = task;
		GUIUtility.keyboardControl = 0;

		Event.current.mousePosition = new Vector2( Event.current.mousePosition.x, Event.current.mousePosition.y + TopBorderOfTasksArea - TaskListScrollPosition.y );
		OnDrag();
	}

	void OnStopDrag()
	{
		Window.GetCurrentList().OnTaskChanged( DraggingTask, "Change Task Order" );

		if( DragIndex >= 0 )
		{
			int targetOrder = DragIndex - 2;

			if( DraggingTask.Order < targetOrder )
			{
				targetOrder--;
			}

			foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
			{
				if( DraggingTask.Order <= targetOrder )
				{
					if( task.Order <= targetOrder && task.Order > DraggingTask.Order )
					{
						task.Order--;
					}
				}
				else
				{
					if( task.Order >= targetOrder && task.Order < DraggingTask.Order )
					{
						task.Order++;
					}
				}
			}

			DraggingTask.Order = targetOrder;

			Window.GetCurrentList().SortTaskList();
		}

		IsDragging = false;
		DraggingTask = null;
		TaskListScrollPositionYScroll = 0;
	}

	void OnDrag()
	{
		if( IsCorrectSortTypeForDrag() == false )
		{
			IsDragging = false;
			return;
		}

		if( Event.current.mousePosition.y < TopBorderOfTasksArea - 30 )
		{
			TaskListScrollPositionYScroll = -10;
		}
		else if( Event.current.mousePosition.y < TopBorderOfTasksArea )
		{
			TaskListScrollPositionYScroll = -3;
		}
		else if( Event.current.mousePosition.y < TopBorderOfTasksArea + 10 )
		{
			TaskListScrollPositionYScroll = -1;
		}
		else if( Event.current.mousePosition.y - Screen.height > 0 )
		{
			TaskListScrollPositionYScroll = 10;
		}
		else if( Event.current.mousePosition.y - Screen.height > -20 )
		{
			TaskListScrollPositionYScroll = 3;
		}
		else if( Event.current.mousePosition.y - Screen.height > -30 )
		{
			TaskListScrollPositionYScroll = 1;
		}
		else
		{
			TaskListScrollPositionYScroll = 0;
		}

		DragIndex = (int)Event.current.mousePosition.y;

		if( DragIndex < TopBorderOfTasksArea )
		{
			DragIndex = -100;
		}

		DragIndex += (int)TaskListScrollPosition.y;
		DragIndex = Mathf.CeilToInt( DragIndex / 24 );

		if( DragIndex > Window.GetCurrentList().GetTaskList().Count + 3 )
		{
			DragIndex = Window.GetCurrentList().GetTaskList().Count + 3;
		}
	}

	void OnSearchChanged()
	{
		foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
		{
			if( task == null )
			{
				continue;
			}

			string[] strings = SearchString.Split( ' ' );
			bool foundAllStrings = true;

			foreach( string searchItem in strings )
			{
				if( task.Description.ToLower().Contains( searchItem.ToLower() ) == false 
							&& task.Category.ToLower().Contains( searchItem.ToLower() ) == false 
							&& task.Developer.ToLower().Contains( searchItem.ToLower() ) == false
							&& task.Tags.ToLower().Contains( searchItem.ToLower() ) == false )
				{
					foundAllStrings = false;
					break;
				}
			}

			task.IsMatchingSearchString = foundAllStrings;
		}
	}

	bool IsCorrectSortTypeForDrag()
	{
		if( Window.GetCurrentList().GetSortType() != TodoListSortType.OrderAsc )
		{
			EditorApplication.Beep();
			if( EditorUtility.DisplayDialog( "Note", "To be able to change the order of tasks you have to sort them ascending by number.\n\nDo you want to change the sorting order now?", "Yes", "No" ) )
			{
				Window.GetCurrentList().SortTaskList( TodoListSortType.OrderAsc );
				Window.Repaint();
			}

			return false;
		}

		return true;
	}
}