using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TodoListEditTaskWindow : EditorWindow
{
	TodoListTask Task;
	TodoListWindow Window;

	bool ShowDefaultTaskData = true;
	bool ShowActivity = true;

	bool IsEnabled;

	int TransitionToIndex = 0;
	int AssignToIndex = 0;

	string PostCommentText = "";
	Vector2 ActivityScrollView = Vector2.zero;

	string NewDay = System.DateTime.Now.Day.ToString( "00" );
	string NewMonth = System.DateTime.Now.Month.ToString( "00" );
	string NewYear = System.DateTime.Now.Year.ToString( "0000" );

	string NewHour = System.DateTime.Now.Hour.ToString( "00" );
	string NewMinute = System.DateTime.Now.Minute.ToString( "00" );

	void OnEnable()
	{
		this.minSize = new Vector2( 400, 500 );
	}

	public void Disable()
	{
		IsEnabled = false;
	}

	public void SetTask( TodoListTask task )
	{
		Task = task;

		if( Task.DueDate != 0 )
		{
			System.DateTime date = TodoList.FromUnixTimestamp( Task.DueDate );

			NewDay = date.Day.ToString( "00" );
			NewMonth = date.Month.ToString( "00" );
			NewYear = date.Year.ToString( "0000" );
		}

		if( Task.DueTime != 0 )
		{
			System.DateTime date = TodoList.FromUnixTimestamp( Task.DueTime );

			NewHour = date.Hour.ToString( "00" );
			NewMinute = date.Minute.ToString( "00" );
		}
	}

	public void SetTodoListWindow( TodoListWindow window )
	{
		Window = window;
		Window.SetEditTaskWindow( this );
		IsEnabled = true;

		AssignToIndex = Window.GetCurrentList().GetDevelopersList().FindIndex( item => item == Task.Developer ) + 1;
	}

	void RepaintOnUndoRedo()
	{
		if( Event.current.type == EventType.ValidateCommand )
		{
			switch( Event.current.commandName )
			{
				case "UndoRedoPerformed":
					Repaint();
					Window.Repaint();
					break;
			}
		}
	}

	void OnGUI()
	{
		RepaintOnUndoRedo();

		GUI.enabled = IsEnabled;

		GUIStyle foldoutStyle = new GUIStyle( EditorStyles.foldout );
		foldoutStyle.fontStyle = FontStyle.Bold;

		if( ShowDefaultTaskData = EditorGUILayout.Foldout( ShowDefaultTaskData, "Task Data", foldoutStyle ) )
		{
			DisplayTaskDesc();
			DisplayTaskProgress();
			DisplayTaskEffort();
			DisplayTaskStatus();
			DisplayTaskPriority();
			DisplayTaskCategory();
			DisplayTaskDeveloper();
			DisplayTaskDueDate();
			DisplayTaskDueTime();
			DisplayTaskTags();

			GUILayout.Box( "", TodoList.GetHeadlineStyle(), GUILayout.Height( 2 ) );
		}

		if( ShowActivity = EditorGUILayout.Foldout( ShowActivity, "Activity", foldoutStyle ) )
		{
			DisplayActivityAdd();
			DisplayActivity();
		}

		GUILayout.Space( 5 );

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Space( 180 );

			/*if( GUILayout.Button( "Mark Completed", EditorStyles.miniButtonLeft ) )
			{
				Window.GetCurrentList().RegisterUndo( "Task Completed" );
				GUIUtility.keyboardControl = 0;
				Task.ToggleCompleded();
				Window.Repaint();
			}*/

			if( GUILayout.Button( "Delete Task", EditorStyles.miniButtonLeft ) )
			{
				GUIUtility.keyboardControl = 0;
				EditorApplication.Beep();
				if( EditorUtility.DisplayDialog( "Please Confirm", "Do you really want to delete the task '" + Task.Description + "'?", "Yes", "No" ) )
				{
					Window.GetCurrentList().RegisterUndo( "Delete Task" );
					Window.GetCurrentList().OnTaskDeleted( Task );
					Window.GetCurrentList().GetTaskList().Remove( Task );
					this.Close();
					return;
				}
			}

			if( GUILayout.Button( "Close", EditorStyles.miniButtonRight ) )
			{
				this.Close();
			}
		}
		EditorGUILayout.EndHorizontal();

		if( GUI.changed )
		{
			Window.Repaint();
		}
	}

	string[] GetCurrentTransitions()
	{
		List<string> transitionList = new List<string>();
		List<TodoListWorkflow> workflow = Window.GetCurrentList().GetWorkflowList();

		int index = -1;

		for( int i = 0; i < workflow.Count; ++i )
		{
			if( workflow[ i ].Description == Task.Status )
			{
				index = i;
				break;
			}
		}

		if( index != -1 )
		{
			transitionList.Add( "Do not transition" );

			foreach( TodoListTransition transition in workflow[ index ].Transitions )
			{
				transitionList.Add( transition.To );
			}
		}

		return transitionList.ToArray();
	}

	void DisplayActivityAdd()
	{
		if( GetCurrentTransitions().Length > 0 )
		{
			TransitionToIndex = EditorGUILayout.Popup( "Transition to", TransitionToIndex, GetCurrentTransitions() );
		}

		if( Window.GetCurrentList().GetDevelopersStringList().Length > 0 )
		{
			string[] developers = Window.GetCurrentList().GetDevelopersStringList();

			for( int i = 0; i < developers.Length; ++i )
			{
				if( developers[ i ] == Task.Developer )
				{
					developers[ i ] = "Do not reassign";
				}
			}

			AssignToIndex = EditorGUILayout.Popup( "Assign to", AssignToIndex, developers );
		}

		PostCommentText = EditorGUILayout.TextArea( PostCommentText );

		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.FlexibleSpace();

			if( GUILayout.Button( "Post Comment" ) )
			{
				OnPostCommentClicked();
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayActivity()
	{
		ActivityScrollView = EditorGUILayout.BeginScrollView( ActivityScrollView, GUILayout.Height( Screen.height - 325 ) );
		{
			GUIStyle style = new GUIStyle( "Label" );
			style.wordWrap = true;
			style.fixedWidth = Screen.width - 40;

			for( int i = 0; i < Task.Posts.Count; ++i )
			{
				TodoListTaskPost post = Task.Posts[ i ];

				GUIStyle postStyle = new GUIStyle( TodoList.GetHeadlineStyle() );
				postStyle.fixedHeight = postStyle.CalcHeight( new GUIContent( post.Text ), Screen.width - 40 ) + 25;

				EditorGUILayout.BeginVertical( postStyle );
				{
					DisplayActivityPostHeader( post );

					if( post.Text != "" )
					{
						style.fixedHeight = style.CalcHeight( new GUIContent( post.Text ), Screen.width - 40 );
						
						EditorGUILayout.SelectableLabel( post.Text, style );
					}
				}
				EditorGUILayout.EndVertical();
			}

			EditorGUILayout.BeginVertical( TodoList.GetHeadlineStyle() );
			{
				DisplayActivityPostHeader( null, true );
			}
			EditorGUILayout.EndVertical();
		}
		EditorGUILayout.EndScrollView();
	}

	void DisplayActivityPostHeader( TodoListTaskPost post, bool specialCreatedHeader = false )
	{
		EditorGUILayout.BeginHorizontal();
		{
			EditorGUILayout.BeginVertical();
			{
				if( specialCreatedHeader )
				{
					GUILayout.Label( "Created", EditorStyles.miniLabel );
				}
				else
				{
					if( post.AssignTo != "" )
					{
						GUILayout.Label( "Assigned to " + post.AssignTo, EditorStyles.miniLabel );
					}

					if( post.TransitionTo != "" )
					{
						GUILayout.Label( "Transitioned to " + post.TransitionTo, EditorStyles.miniLabel );
					}
				}
			}
			EditorGUILayout.EndVertical();

			GUILayout.FlexibleSpace();

			int unixDate = 0;

			if( specialCreatedHeader )
			{
				unixDate = Task.TimeCreated;
			}
			else
			{
				unixDate = post.Date;
			}

			System.DateTime date = TodoList.FromUnixTimestamp( unixDate );

			GUILayout.Label( date.Day + "." + date.Month + "." + date.Year + " " + date.Hour.ToString( "00" ) + ":" + date.Minute.ToString( "00" ), EditorStyles.miniLabel );
			GUILayout.Space( 3 );
		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Space( 5 );
	}

	void DisplayTaskDesc()
	{
		string newDesc = EditorGUILayout.TextField( "Description", Task.Description );

		if( newDesc != Task.Description )
		{
			Window.GetCurrentList().RegisterUndo( "Edit Task Description" );
			Task.Description = newDesc;
			Window.GetCurrentList().OnTaskChanged( Task );
		}
	}

	void DisplayTaskStatus()
	{
		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Space( 6 );
			GUILayout.Label( "Status", GUILayout.Width( 93 ) );
			GUILayout.Label( Task.Status );

			List<string> transitions = Window.GetCurrentList().GetTransitions( Task.Status );
			transitions.Insert( 0, "" );

			GUIStyle popupStyle = new GUIStyle( EditorStyles.popup );
			popupStyle.margin.top = 4;
			popupStyle.margin.right = 8;

			int selectedIndex = 0;

			if( transitions.Count > 1 )
			{
				selectedIndex = EditorGUILayout.Popup( 0, transitions.ToArray(), popupStyle, GUILayout.Width( 16 ) );
			}

			GUILayout.FlexibleSpace();

			if( selectedIndex != 0 )
			{
				Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Status" );

				Task.SetCompleted( Window.GetCurrentList().IsStatusFinished( transitions[ selectedIndex ] ) );

				Task.Status = transitions[ selectedIndex ];

				Task.AddPost( "", transitions[ selectedIndex ], "" );
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayTaskProgress()
	{
		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Space( 6 );
			GUILayout.Label( "Progress", GUILayout.Width( 95 ) );

			float newValue = GUILayout.HorizontalSlider( Task.Progress, 0f, 1f );

			GUILayout.Label( Mathf.FloorToInt( newValue * 100 ) + "%", GUILayout.Width( 35 ) );
			if( newValue != Task.Progress )
			{
				Window.GetCurrentList().RegisterUndo( "Edit Task Progress" );
				Task.Progress = newValue;
				Window.GetCurrentList().OnTaskChanged( Task );
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayTaskEffort()
	{
		int numOptions = Window.GetCurrentList().GetNumberOfBacklogItemEffortOptions();
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
		popupStyle.margin.left = 4;

		int newValue = EditorGUILayout.IntPopup( new GUIContent( "Effort" ), Task.Effort, displayedOptions, valueOptions, popupStyle );

		if( newValue != Task.Effort )
		{
			Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Backlog Item Effort" );

			Task.Effort = newValue;
			Window.GetCurrentList().OnTaskChanged( Task );
		}
	}

	void DisplayTaskPriority()
	{
		string[] options = Window.GetCurrentList().GetPriorityStringList();

		int currentIndex = Window.GetCurrentList().GetPriorityList().IndexOf( Task.Priority ) + 1;

		int newIndex = EditorGUILayout.Popup( "Priority", currentIndex, options );

		if( newIndex != currentIndex )
		{
			Window.GetCurrentList().RegisterUndo( "Edit Task Priority" );

			if( newIndex == 0 )
			{
				Task.Priority = "Unassigned";
			}
			else
			{
				Task.Priority = Window.GetCurrentList().GetPriorityList()[ newIndex - 1 ];
			}

			Window.GetCurrentList().OnTaskChanged( Task );
		}
	}

	void DisplayTaskCategory()
	{
		string[] options = Window.GetCurrentList().GetCategoriesStringList();

		int currentIndex = Window.GetCurrentList().GetCategoriesList().IndexOf( Task.Category ) + 1;

		int newIndex = EditorGUILayout.Popup( "Category", currentIndex, options );

		if( newIndex != currentIndex )
		{
			Window.GetCurrentList().RegisterUndo( "Edit Task Category" );

			if( newIndex == 0 )
			{
				Task.Category = "Unassigned";
			}
			else
			{
				Task.Category = Window.GetCurrentList().GetCategoriesList()[ newIndex - 1 ];
			}

			Window.GetCurrentList().OnTaskChanged( Task );
		}
	}

	void DisplayTaskDeveloper()
	{
		string[] options = Window.GetCurrentList().GetDevelopersStringList();

		int currentIndex = Window.GetCurrentList().GetDevelopersList().IndexOf( Task.Developer ) + 1;

		int newIndex = EditorGUILayout.Popup( "Developer", currentIndex, options );

		if( newIndex != currentIndex )
		{
			Window.GetCurrentList().OnTaskChanged( Task );

			if( newIndex == 0 )
			{
				Task.Developer = "Unassigned";
			}
			else
			{
				Task.Developer = Window.GetCurrentList().GetDevelopersList()[ newIndex - 1 ];
			}

			Task.AddPost( "", "", Task.Developer );
		}
	}

	void DisplayTaskDueDate()
	{
		EditorGUILayout.BeginHorizontal();
		{
			bool isEnabled = Task.DueDate != 0;

			bool newEnabled = GUILayout.Toggle( isEnabled, "", GUILayout.Width( 16 ) );
			GUILayout.Label( "Due Date", GUILayout.Width( 76 ) );

			GUI.enabled = isEnabled && IsEnabled;

			GUI.SetNextControlName( "AddItemDueDateDay-numeric" );
			NewDay = GUILayout.TextField( NewDay, 2, GUILayout.Width( 25 ) );

			GUI.SetNextControlName( "AddItemDueDateMonth-numeric" );
			NewMonth = GUILayout.TextField( NewMonth, 2, GUILayout.Width( 25 ) );

			GUI.SetNextControlName( "AddItemDueDateYear-numeric" );
			NewYear = GUILayout.TextField( NewYear, 4, GUILayout.Width( 40 ) );

			GUI.enabled = IsEnabled;

			if( GUI.changed )
			{
				if( newEnabled )
				{
					int day = System.Convert.ToInt32( NewDay );
					int month = System.Convert.ToInt32( NewMonth );
					int year = System.Convert.ToInt32( NewYear );

					if( year > 1970 )
					{
						Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Due Date" );
						Task.DueDate = TodoList.ToUnixTimestamp( year, month, day );
					}
				}
				else
				{
					Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Due Date" );
					Task.DueDate = 0;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayTaskDueTime()
	{
		EditorGUILayout.BeginHorizontal();
		{
			bool isEnabled = Task.DueTime != 0;

			bool newEnabled = GUILayout.Toggle( isEnabled, "", GUILayout.Width( 16 ) );
			GUILayout.Label( "Due Time", GUILayout.Width( 76 ) );

			GUI.enabled = isEnabled && IsEnabled;

			GUI.SetNextControlName( "AddItemDueTimeHour-numeric" );
			NewHour = GUILayout.TextField( NewHour, 2, GUILayout.Width( 25 ) );

			GUI.SetNextControlName( "AddItemDueTimeMinute-numeric" );
			NewMinute = GUILayout.TextField( NewMinute, 2, GUILayout.Width( 25 ) );

			GUI.enabled = IsEnabled;

			if( GUI.changed )
			{
				if( newEnabled )
				{
					int hour = System.Convert.ToInt32( NewHour );
					int minute = System.Convert.ToInt32( NewMinute );

					if( hour >= 0 && hour < 24 && minute >= 0 && minute < 60 )
					{
						Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Due Time" );
						Task.DueTime = TodoList.ToUnixTimestamp( 1970, 1, 1, hour, minute );
					}
				}
				else
				{
					Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Due Time" );
					Task.DueTime = 0;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplayTaskTags()
	{
		string newTags = EditorGUILayout.TextField( "Search Tags", Task.Tags );

		if( newTags != Task.Tags )
		{
			Window.GetCurrentList().OnTaskChanged( Task, "Edit Task Tags" );

			Task.Tags = newTags;
		}
	}

	void OnPostCommentClicked()
	{
		Window.GetCurrentList().OnTaskChanged( Task, "Post Comment" );
		
		string developer = Window.GetCurrentList().GetDevelopersStringList()[ AssignToIndex ];
		if( developer == Task.Developer )
		{
			developer = "";
		}
		else
		{
			Task.Developer = developer;
		}

		string transition = GetCurrentTransitions()[ TransitionToIndex ];
		if( transition == "Do not transition" )
		{
			transition = "";
		}
		else
		{
			Task.SetCompleted( Window.GetCurrentList().IsStatusFinished( transition ) );
			Task.Status = transition;
		}

		Task.AddPost( PostCommentText, transition, developer );

		GUIUtility.keyboardControl = 0;
		PostCommentText = "";

		TransitionToIndex = 0;
		AssignToIndex = 0;

		Window.Repaint();
	}
}