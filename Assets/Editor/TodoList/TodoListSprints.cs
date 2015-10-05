using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TodoListSprint
{
	public int SprintIndex;
	public string Name;
	public int StartDate;
	public int FinishDate;
	public int TotalEffort;
	public int TotalCompletedEffort;
}

public class TodoListSprints
{
	TodoListWindow Window;
	Vector2 ScrollViewPosition;

	int TotalBacklogEffort;
	int TotalBacklogCompletedEffort;

	public TodoListSprints( TodoListWindow window )
	{
		Window = window;
	}

	public void Display()
	{
		ScrollViewPosition = EditorGUILayout.BeginScrollView( ScrollViewPosition );
		{
			DisplayButtons();

			UpdateDisplayTaskSettings();

			for( int i = 0; i < Window.GetCurrentList().GetSprintList().Count; ++i )
			{
				DisplaySprint( Window.GetCurrentList().GetSprintList().Count - 1 - i );
			}

			DisplayBacklog();
		}
		EditorGUILayout.EndScrollView();
	}

	void UpdateDisplayTaskSettings()
	{
		if( GetSettings().ShowSprintTasks.Count != Window.GetCurrentList().GetSprintList().Count )
		{
			int difference = Window.GetCurrentList().GetSprintList().Count - GetSettings().ShowSprintTasks.Count;

			while( difference > 0 )
			{
				GetSettings().ShowSprintTasks.Add( true );
				difference--;
			}

			while( difference < 0 )
			{
				GetSettings().ShowSprintTasks.RemoveAt( GetSettings().ShowSprintTasks.Count - 1 );
				difference++;
			}
		}
	}

	void DisplaySprint( int index )
	{
		TodoListSprint sprint = Window.GetCurrentList().GetSprintList()[ index ];

		EditorGUILayout.BeginVertical( TodoList.GetHeadlineStyle() );
		{
			DisplaySprintHeader( sprint, index );
			DisplaySprintData( sprint );
			DisplaySprintTasks( sprint, index );
		}
		EditorGUILayout.EndVertical();
	}

	void DisplaySprintHeader( TodoListSprint sprint, int index )
	{
		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label( "Sprint", EditorStyles.boldLabel, GUILayout.Width( 215 ) );
			GUILayout.Label( "Completed Effort: " + sprint.TotalCompletedEffort + "/" + sprint.TotalEffort );
			GUILayout.FlexibleSpace();

			if( GUILayout.Button( "X", GUILayout.Width( 20 ) ) )
			{
				EditorApplication.Beep();
				if( EditorUtility.DisplayDialog( "Please Confirm", "Do you really want to delete the sprint '" + sprint.Name + "'?", "Yes", "No" ) )
				{
					Window.GetCurrentList().GetSprintList().RemoveAt( index );
					Window.GetCurrentList().OnSprintDeleted( sprint.SprintIndex );
					Window.Repaint();
					return;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	void DisplaySprintData( TodoListSprint sprint )
	{
		System.DateTime startDate = TodoList.FromUnixTimestamp( sprint.StartDate );
		System.DateTime finishDate = TodoList.FromUnixTimestamp( sprint.FinishDate );

		EditorGUILayout.BeginHorizontal();
		{
			string newName = EditorGUILayout.TextField( sprint.Name, GUILayout.Width( 200 ) );
			GUILayout.Space( 15 );

			GUILayout.Label( "From" );
			GUILayout.Space( 5 );
			startDate = DisplayDateFields( startDate );
			GUILayout.Space( 10 );

			GUILayout.Label( "To" );
			GUILayout.Space( 5 );
			finishDate = DisplayDateFields( finishDate );

			GUILayout.FlexibleSpace();

			if( newName != sprint.Name || startDate != TodoList.FromUnixTimestamp( sprint.StartDate ) || finishDate != TodoList.FromUnixTimestamp( sprint.FinishDate ) )
			{
				Window.GetCurrentList().RegisterUndo( "Edit Sprint" );

				sprint.Name = newName;
				sprint.StartDate = TodoList.ToUnixTimestamp( startDate );
				sprint.FinishDate = TodoList.ToUnixTimestamp( finishDate );

				Window.GetCurrentList().OnSprintChanged();
			}

			
		}
		EditorGUILayout.EndHorizontal();
	}

	System.DateTime DisplayDateFields( System.DateTime date )
	{
		string newDay = GUILayout.TextField( date.Day.ToString(), 2, GUILayout.Width( 25 ) );
		string newMonth = GUILayout.TextField( date.Month.ToString(), 2, GUILayout.Width( 25 ) );
		string newYear = GUILayout.TextField( date.Year.ToString(), 4, GUILayout.Width( 40 ) );

		if( newDay != date.Day.ToString() || newMonth != date.Month.ToString() || newYear != date.Year.ToString() )
		{
			int newDayInt;
			int newMonthInt;
			int newYearInt;

			if( int.TryParse( newDay, out newDayInt ) && int.TryParse( newMonth, out newMonthInt ) && int.TryParse( newYear, out newYearInt ) )
			{
				date = new System.DateTime( newYearInt, newMonthInt, newDayInt );
			}
			else
			{
				EditorApplication.Beep();
				EditorUtility.DisplayDialog( "Error", "You have to input a number", "Oh well, fine" );
			}
		}

		return date;
	}

	void DisplaySprintTasks( TodoListSprint sprint, int index )
	{
		GetSettings().ShowSprintTasks[ index ] = EditorGUILayout.Foldout( GetSettings().ShowSprintTasks[ index ], "Tasks" );

		if( GetSettings().ShowSprintTasks[ index ] == true )
		{
			ShowTasks( sprint.SprintIndex, ref sprint.TotalEffort, ref sprint.TotalCompletedEffort );
		}
	}

	void DisplayBacklog()
	{
		EditorGUILayout.BeginVertical( TodoList.GetHeadlineStyle() );
		{
			EditorGUILayout.BeginHorizontal();
			{
				GUILayout.Label( "Backlog", EditorStyles.boldLabel, GUILayout.Width( 215 ) );
				GUILayout.Label( "Completed Effort: " + TotalBacklogCompletedEffort + "/" + TotalBacklogEffort );
			}
			EditorGUILayout.EndHorizontal();

			GetSettings().ShowBacklogTasks = EditorGUILayout.Foldout( GetSettings().ShowBacklogTasks, "Tasks" );

			if( GetSettings().ShowBacklogTasks == true )
			{
				ShowTasks( -1, ref TotalBacklogEffort, ref TotalBacklogCompletedEffort );
			}
		}
		EditorGUILayout.EndVertical();
	}

	TodoListPersonalSettingsObject GetSettings()
	{
		return Window.GetCurrentList().GetPersonalSettings();
	}

	void DisplayButtons()
	{
		EditorGUILayout.BeginHorizontal();
		{
			if( GUILayout.Button( "Add new Sprint", GUILayout.Width( 140 ) ) )
			{
				System.DateTime startDate = new System.DateTime( System.DateTime.Now.Year, System.DateTime.Now.Month, System.DateTime.Now.Day );

				if( Window.GetCurrentList().GetSprintList().Count > 0 )
				{
					startDate = TodoList.FromUnixTimestamp( Window.GetCurrentList().GetSprintList()[ Window.GetCurrentList().GetSprintList().Count - 1 ].FinishDate ).AddDays( 1 );
				}

				int newIndex = 0;
				foreach( TodoListSprint sprint in Window.GetCurrentList().GetSprintList() )
				{
					if( sprint.SprintIndex >= newIndex )
					{
						newIndex = sprint.SprintIndex + 1;
					}
				}

				TodoListSprint newSprint = new TodoListSprint();
				newSprint.SprintIndex = newIndex;
				newSprint.Name = "Sprint " + newIndex;
				newSprint.StartDate = TodoList.ToUnixTimestamp( startDate );
				newSprint.FinishDate = newSprint.StartDate + 60 * 60 * 24 * 14;

				Window.GetCurrentList().GetSprintList().Add( newSprint );
				Window.GetCurrentList().OnSprintAdded();

				EditorGUIUtility.keyboardControl = 0;
			}

			if( GUILayout.Button( "Reorder Sprints by starting date" ) )
			{
				Window.GetCurrentList().GetSprintList().Sort( delegate( TodoListSprint s1, TodoListSprint s2 )
				{
					return s1.StartDate.CompareTo( s2.StartDate );
				} );

				EditorGUIUtility.keyboardControl = 0;
			}

			GUILayout.FlexibleSpace();
		}
		EditorGUILayout.EndHorizontal();

	}

	void ShowTasks( int sprintIndex, ref int effort, ref int completedEffort )
	{
		int taskNr = 1;

		GUIStyle horizontalRow = new GUIStyle();
		horizontalRow.margin.left = 15;
		horizontalRow.padding.bottom = 0;
		horizontalRow.padding.top = 0;

		effort = 0;
		completedEffort = 0;

		foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
		{
			if( task.SprintIndex != sprintIndex )
			{
				continue;
			}

			effort += task.Effort;

			if( task.IsDone() == true )
			{
				completedEffort += task.Effort;
			}

			horizontalRow.normal.background = TodoList.GetTaskBackground( task, taskNr );

			EditorGUILayout.BeginHorizontal( horizontalRow );
			{
				GUILayout.Label( task.Description, GUILayout.Width( 200 ) );

				GUILayout.Label( "Effort", GUILayout.Width( 50 ) );
				TodoList.DisplayTaskEffort( task, TodoList.EffortFieldWidth, Window );

				GUILayout.Space( 35 );

				GUILayout.Label( "Sprint", GUILayout.Width( 50 ) );
				TodoList.DisplayTaskSprint( task, TodoList.SprintFieldWidth, Window );
			}
			EditorGUILayout.EndHorizontal();

			taskNr++;
		}
	}
}
