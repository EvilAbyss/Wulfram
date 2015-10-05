using System;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum TodoListTimeWindowTypes
{
	AllTime,
	Sprint,
	Month,
	Week,
}

public class TodoListStatistics
{
	TodoListWindow Window;

	Color GraphBackgroundLines = Color.gray;
	Color GraphTotalTasks = new Color( 0, 0, 0.7f, 0.6f );
	Color GraphCreatedTasks = new Color( 0.8f, 0, 0, 0.6f );
	Color GraphCompletedTasks = new Color( 0, 0.6f, 0, 0.6f );

	int TotalNumberOfDays = -1;
	DateTime FirstDay;
	DateTime LastDay;

	int[] CompletedTasks;
	int[] CreatedTasks;
	int[] TotalTasks;
	int MaximumNumberOfTasks;

	TodoListSprint ViewingSprint;

	int GraphXMin = 42;
	int GraphYMin = 50;
	int GraphHeight = Screen.height - 110;
	int GraphWidth = Screen.width - 49;

	Dictionary<int, int> EffortCompletedPerSprint = new Dictionary<int, int>();
	float AverageVelocity = 10.57f;

	public TodoListStatistics( TodoListWindow window )
	{
		Window = window;
	}

	public void UpdateStatistics()
	{
		if( Window.GetCurrentList().GetTaskList().Count == 0 )
		{
			return;
		}

		int earliestTimeCreated = int.MaxValue;
		int lastTimeCompleted = 0;
		EffortCompletedPerSprint.Clear();

		foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
		{
			earliestTimeCreated = Mathf.Min( earliestTimeCreated, task.TimeCreated );
			lastTimeCompleted = Mathf.Max( lastTimeCompleted, task.TimeCompleted );
			lastTimeCompleted = Mathf.Max( lastTimeCompleted, task.TimeCreated );
		}

		DateTime earliestTime = TodoList.FromUnixTimestamp( earliestTimeCreated );
		DateTime lastTime = TodoList.FromUnixTimestamp( lastTimeCompleted );
		lastTime = lastTime.AddDays( 2 );
		FirstDay = new DateTime( earliestTime.Year, earliestTime.Month, earliestTime.Day );
		LastDay = new DateTime( lastTime.Year, lastTime.Month, lastTime.Day );
		
		TimeSpan timeSpan = LastDay - FirstDay;
		TotalNumberOfDays = (int)timeSpan.TotalDays;

		CompletedTasks = new int[ TotalNumberOfDays + 1 ];
		CreatedTasks = new int[ TotalNumberOfDays + 1 ];
		TotalTasks = new int[ TotalNumberOfDays + 1 ];

		foreach( TodoListTask task in Window.GetCurrentList().GetTaskList() )
		{
			timeSpan = TodoList.FromUnixTimestamp( task.TimeCreated ) - FirstDay;

			int index = (int)timeSpan.TotalDays;

			CreatedTasks[ index ] += task.Effort;

			if( EffortCompletedPerSprint.ContainsKey( task.SprintIndex ) == false )
			{
				EffortCompletedPerSprint.Add( task.SprintIndex, 0 );
			}

			if( task.IsDone() == true )
			{
				EffortCompletedPerSprint[ task.SprintIndex ] += task.Effort;

				timeSpan = TodoList.FromUnixTimestamp( task.TimeCompleted ) - FirstDay;

				index = (int)timeSpan.TotalDays;

				CompletedTasks[ index ] += task.Effort;
			}
		}

		AverageVelocity = 0;
		foreach( KeyValuePair<int, int> sprintEffort in EffortCompletedPerSprint )
		{
			AverageVelocity += sprintEffort.Value;
		}
		AverageVelocity /= EffortCompletedPerSprint.Count;

		for( int i = 0; i <= TotalNumberOfDays; ++i )
		{
			if( i != 0 )
			{
				TotalTasks[ i ] = TotalTasks[ i - 1 ];
			}

			TotalTasks[ i ] += CreatedTasks[ i ];
			TotalTasks[ i ] -= CompletedTasks[ i ];
		}

		UpdateViewingSpanIndices();
	}

	void DisplayLegend()
	{
		float x = Screen.width - 400;

		GUI.Label( new Rect( x + 30, 20, 100, 20 ), "Created Effort" );
		GUI.Label( new Rect( x + 150, 20, 100, 20 ), "Completed Effort" );
		GUI.Label( new Rect( x + 284, 20, 100, 20 ), "Total Effort left" );

		Editor2DTools.DrawLine( new Vector2( x + 110 + 10, 29 ), new Vector2( x + 128 + 10, 29 ), GraphCreatedTasks, 18 );
		Editor2DTools.DrawLine( new Vector2( x + 115 + 140, 29 ), new Vector2( x + 133 + 140, 29 ), GraphCompletedTasks, 18 );
		Editor2DTools.DrawLine( new Vector2( x + 114 + 263, 29 ), new Vector2( x + 132 + 263, 29 ), GraphTotalTasks, 18 );
	}

	void UpdateViewingSpanIndices()
	{
		TimeSpan timespan = GetSettings().ShowLastDay - GetSettings().ShowFirstDay;

		if( timespan > LastDay - FirstDay )
		{
			timespan = LastDay - FirstDay;
		}

		if( GetSettings().ShowFirstDay < FirstDay )
		{
			GetSettings().ShowFirstDay = FirstDay;
			GetSettings().ShowLastDay = GetSettings().ShowFirstDay + timespan;
		}

		if( GetSettings().ShowFirstDay > LastDay )
		{
			GetSettings().ShowFirstDay = LastDay - timespan;
		}

		if( GetSettings().ShowLastDay < FirstDay )
		{
			GetSettings().ShowLastDay = FirstDay + timespan;
		}

		if( GetSettings().ShowLastDay > LastDay )
		{
			GetSettings().ShowLastDay = LastDay;
			GetSettings().ShowFirstDay = GetSettings().ShowLastDay - timespan;
		}

		if( GetSettings().ShowLastDay <= GetSettings().ShowFirstDay )
		{
			GetSettings().ShowLastDay = GetSettings().ShowFirstDay.AddDays( 1 );
		}

		timespan = GetSettings().ShowFirstDay - FirstDay;

		GetSettings().ShowFirstIndex = Mathf.FloorToInt( (float)timespan.TotalDays );

		timespan = GetSettings().ShowLastDay - GetSettings().ShowFirstDay;

		GetSettings().ShowLastIndex = GetSettings().ShowFirstIndex + Mathf.FloorToInt( (float)timespan.TotalDays );

		TotalNumberOfDays = GetSettings().ShowLastIndex - GetSettings().ShowFirstIndex;

		MaximumNumberOfTasks = 1;

		for( int i = Mathf.Max( GetSettings().ShowFirstIndex - 1, 0 ); i < GetSettings().ShowLastIndex; ++i )
		{
			if( i >= TotalTasks.Length )
			{
				break;
			}

			MaximumNumberOfTasks = Mathf.Max( MaximumNumberOfTasks, TotalTasks[ i ] );
			MaximumNumberOfTasks = Mathf.Max( MaximumNumberOfTasks, CompletedTasks[ i ] );
			MaximumNumberOfTasks = Mathf.Max( MaximumNumberOfTasks, CreatedTasks[ i ] );
		}
	}

	public void Display()
	{
		GraphHeight = Screen.height - 130;
		GraphWidth = Screen.width - 49;

		if( TotalNumberOfDays == -1 || GetSettings().ShowFirstIndex == -1 )
		{
			UpdateStatistics();
		}

		DisplayTimeWindowSettings();
		DisplayLegend();
		DisplayGraph();
	}

	void DisplayTimeWindowSettings()
	{
		EditorGUILayout.BeginHorizontal();
		{
			GUILayout.Label( "View time window: ", EditorStyles.boldLabel );

			GUIStyle timeWindowPopup = new GUIStyle( EditorStyles.popup );
			timeWindowPopup.margin = new RectOffset( 4, 4, 4, 3 );

			TodoListTimeWindowTypes newTimeWindow = (TodoListTimeWindowTypes)EditorGUILayout.EnumPopup( Window.GetCurrentList().GetPersonalSettings().DisplayTimeWindow, timeWindowPopup, GUILayout.Width( 70 ) );

			if( newTimeWindow != TodoListTimeWindowTypes.AllTime )
			{
				if( GUILayout.Button( "<" ) )
				{
					switch( newTimeWindow )
					{
					case TodoListTimeWindowTypes.Week:
						SetViewingSpan( GetSettings().ShowFirstDay.AddDays( -7 ), GetSettings().ShowFirstDay );
						break;
					case TodoListTimeWindowTypes.Month:
						SetViewingSpan( GetSettings().ShowFirstDay.AddMonths( -1 ), GetSettings().ShowFirstDay );
						break;
					case TodoListTimeWindowTypes.Sprint:
						TodoListSprint newSprint = Window.GetCurrentList().GetSprintList().Find( item => item.SprintIndex == ViewingSprint.SprintIndex - 1 );

						if( newSprint == null )
						{
							EditorApplication.Beep();
						}
						else
						{
							SetViewingSpan( newSprint );
						}
						break;
					}
				}

				if( GUILayout.Button( ">" ) )
				{
					switch( newTimeWindow )
					{
					case TodoListTimeWindowTypes.Week:
						SetViewingSpan( GetSettings().ShowFirstDay.AddDays( 7 ), GetSettings().ShowFirstDay.AddDays( 14 ) );
						break;
					case TodoListTimeWindowTypes.Month:
						SetViewingSpan( GetSettings().ShowFirstDay.AddMonths( 1 ), GetSettings().ShowFirstDay.AddMonths( 2 ) );
						break;
					case TodoListTimeWindowTypes.Sprint:
						TodoListSprint newSprint = Window.GetCurrentList().GetSprintList().Find( item => item.SprintIndex == ViewingSprint.SprintIndex + 1 );

						if( newSprint == null )
						{
							EditorApplication.Beep();
						}
						else
						{
							SetViewingSpan( newSprint );
						}
						break;
					}
				}

				GUIStyle style = new GUIStyle( "Label" );
				style.margin = new RectOffset( 4, 7, 3, 3 );

				GUILayout.Label( GetCurrentTimeWindowDescription(), style );
			}

			GUILayout.FlexibleSpace();

			if( newTimeWindow != Window.GetCurrentList().GetPersonalSettings().DisplayTimeWindow )
			{
				Window.GetCurrentList().GetPersonalSettings().DisplayTimeWindow = newTimeWindow;

				EditorUtility.SetDirty( Window.GetCurrentList().GetPersonalSettings() );

				switch( newTimeWindow )
				{
				case TodoListTimeWindowTypes.Sprint:
					TodoListSprint sprint = FindNearestSprint();

					if( sprint == null )
					{
						EditorApplication.Beep();
						SetViewingSpan( FirstDay, LastDay );
						break;
					}
					else
					{
						SetViewingSpan( sprint );
						break;
					}
				case TodoListTimeWindowTypes.AllTime:
					SetViewingSpan( FirstDay, LastDay );
					break;
				case TodoListTimeWindowTypes.Month:
					SetViewingSpan( LastDay.AddMonths( -1 ), LastDay );
					break;
				case TodoListTimeWindowTypes.Week:
					SetViewingSpan( LastDay.AddDays( -7 ), LastDay );
					break;
				}
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	TodoListSprint FindNearestSprint()
	{
		TodoListSprint nearestSprint = null;
		System.TimeSpan smallestTimespan = new System.TimeSpan( long.MaxValue );

		foreach( TodoListSprint sprint in Window.GetCurrentList().GetSprintList() )
		{
			System.DateTime startDate = TodoList.FromUnixTimestamp( sprint.StartDate );
			System.DateTime finishDate = TodoList.FromUnixTimestamp( sprint.FinishDate );

			if( startDate >= System.DateTime.Now && finishDate <= System.DateTime.Now )
			{
				return sprint;
			}

			System.TimeSpan timespan = startDate - System.DateTime.Now;
		
			if( finishDate - System.DateTime.Now < timespan )
			{
				timespan = finishDate - System.DateTime.Now;
			}

			if( timespan < smallestTimespan )
			{
				smallestTimespan = timespan;
				nearestSprint = sprint;
			}
		}

		return nearestSprint;
	}

	void SetViewingSpan( TodoListSprint sprint )
	{
		SetViewingSpan( TodoList.FromUnixTimestamp( sprint.StartDate ), TodoList.FromUnixTimestamp( sprint.FinishDate ).AddDays( 1 ) );
		ViewingSprint = sprint;
	}

	void SetViewingSpan( DateTime firstDay, DateTime lastDay )
	{
		GetSettings().ShowFirstDay = firstDay;
		GetSettings().ShowLastDay = lastDay;

		UpdateViewingSpanIndices();
	}

	TodoListPersonalSettingsObject GetSettings()
	{
		return Window.GetCurrentList().GetPersonalSettings();
	}

	string GetCurrentTimeWindowDescription()
	{
		if( Window.GetCurrentList().GetPersonalSettings().DisplayTimeWindow == TodoListTimeWindowTypes.Sprint )
		{
			System.DateTime startDate = TodoList.FromUnixTimestamp( ViewingSprint.StartDate );
			System.DateTime finishDate = TodoList.FromUnixTimestamp( ViewingSprint.FinishDate );

			return ViewingSprint.Name + ": " + startDate.Day + "." + startDate.Month + "." + startDate.Year + " - " + finishDate.Day + "." + finishDate.Month + "." + finishDate.Year;
		}

		return GetSettings().ShowFirstDay.Day + "." + GetSettings().ShowFirstDay.Month + "." + GetSettings().ShowFirstDay.Year + " - " + GetSettings().ShowLastDay.Day + "." + GetSettings().ShowLastDay.Month + "." + GetSettings().ShowLastDay.Year;
	}

	void DisplayGraph()
	{
		DrawSprintBackgrounds();
		DrawGraphBackground();

		if( Window.GetCurrentList().GetTaskList().Count == 0 )
		{
			return;
		}

		DrawYAxisLabels();
		DrawXAxisLabels();
		DrawAverageVelocity();
		DrawGraphData();
		
		//Editor2DTools.DrawCircle( new Vector2( 100, 100 ), 50, new Color( 1, 0, 0 ), 30 );
	}

	void DrawSprintBackgrounds()
	{
		float widthOfADay = GetWidthOfADay();
		System.DateTime startDate;
		System.DateTime finishDate;

		foreach( TodoListSprint sprint in Window.GetCurrentList().GetSprintList() )
		{
			startDate = TodoList.FromUnixTimestamp( sprint.StartDate );
			finishDate = TodoList.FromUnixTimestamp( sprint.FinishDate );

			if( ( startDate >= GetSettings().ShowFirstDay && startDate <= GetSettings().ShowLastDay ) || ( finishDate <= GetSettings().ShowLastDay && finishDate >= GetSettings().ShowFirstDay ) || ( startDate < GetSettings().ShowFirstDay && finishDate > GetSettings().ShowLastDay ) )
			{
				if( GetSettings().ShowLastDay < finishDate )
				{
					finishDate = GetSettings().ShowLastDay;
				}

				if( GetSettings().ShowFirstDay > startDate )
				{
					startDate = GetSettings().ShowFirstDay;
				}

				int days = Mathf.CeilToInt( (float)( finishDate - startDate ).TotalDays );
				int dayFromFirstDay = Mathf.CeilToInt( (float)( startDate - GetSettings().ShowFirstDay ).TotalDays );
				Rect rect = new Rect( GraphXMin + widthOfADay * dayFromFirstDay + widthOfADay, GraphYMin, widthOfADay * days, GraphHeight );

				GUI.DrawTexture( rect, TodoList.GetAlteringBackground( 0 ), ScaleMode.StretchToFill );

				Editor2DTools.DrawLine( new Vector2( GraphXMin + widthOfADay * dayFromFirstDay + widthOfADay, GraphYMin ), new Vector2( GraphXMin + widthOfADay * dayFromFirstDay + widthOfADay, GraphHeight + GraphYMin ), GraphBackgroundLines, 1 );
				Editor2DTools.DrawLine( new Vector2( GraphXMin + widthOfADay * ( dayFromFirstDay + days + 1 ), GraphYMin ), new Vector2( GraphXMin + widthOfADay * ( dayFromFirstDay + days + 1 ), GraphHeight + GraphYMin ), GraphBackgroundLines, 1 );
			}
		}
	}

	float GetWidthOfADay()
	{
		return ( Screen.width - 49 ) * 1f / ( TotalNumberOfDays );
	}

	void DrawGraphBackground()
	{
		Editor2DTools.DrawLine( new Vector2( GraphXMin, GraphYMin ), new Vector2( GraphXMin, GraphHeight + GraphYMin ), GraphBackgroundLines, 2 );
		Editor2DTools.DrawLine( new Vector2( GraphXMin - 1, GraphHeight + GraphYMin ), new Vector2( GraphWidth + GraphXMin, GraphHeight + GraphYMin ), GraphBackgroundLines, 2 );
	}

	void DrawYAxisLabels()
	{
		GUIStyle label = new GUIStyle( "label" );
		label.alignment = TextAnchor.MiddleRight;

		GUI.Label( new Rect( 5, 45, 30, 20 ), MaximumNumberOfTasks.ToString(), label );
		GUI.Label( new Rect( 5, Screen.height - 93, 30, 20 ), "0", label );
	}

	void DrawXAxisLabels()
	{
		GUIStyle label = new GUIStyle( "label" );
		label.alignment = TextAnchor.MiddleCenter;

		float widthOfADay = GetWidthOfADay();

		Vector2 fromPoint = new Vector2( 0, Screen.height - 80 );
		Vector2 toPoint = new Vector2( 0, Screen.height - 75 );

		float labelWidth = 0;
		float tickWidth = 0;

		DateTime LabelDay = GetSettings().ShowFirstDay;

		for( int i = 0; i < TotalNumberOfDays; ++i )
		{
			fromPoint.x = Mathf.RoundToInt( 42 + ( i + 1 ) * widthOfADay );
			toPoint.x = fromPoint.x;

			labelWidth -= widthOfADay;
			tickWidth -= widthOfADay;

			if( labelWidth <= 0 && tickWidth <= 0 )
			{
				GUI.Label( new Rect( Mathf.Min( fromPoint.x - 18, Screen.width - 35 ), Screen.height - 75, 36, 20 ), LabelDay.Day + "." + LabelDay.Month + ".", label );
				labelWidth = 42;
				toPoint.y = Screen.height - 73;
			}
			else
			{
				toPoint.y = Screen.height - 77;
			}

			if( tickWidth <= 0 )
			{
				Editor2DTools.DrawLine( fromPoint, toPoint, GraphBackgroundLines, 1 );

				tickWidth = 3;
			}

			LabelDay = LabelDay.AddDays( 1 );
		}
	}

	void DrawAverageVelocity()
	{
		GUIStyle label = new GUIStyle( "label" );
		label.alignment = TextAnchor.MiddleCenter;

		GUI.Label( new Rect( GraphXMin, Screen.height - 50, Screen.width - GraphXMin - 5, 20 ), "Average velocity: " + ( Mathf.Round( AverageVelocity * 10 ) / 10 ) + " backlog effort/sprint", label );
	}

	int GetDrawingHeightOfData( int value )
	{
		return Mathf.RoundToInt( Screen.height - 82 - ( value * 1f / MaximumNumberOfTasks ) * GraphHeight );
	}

	void DrawGraphData()
	{
		float widthOfADay = GetWidthOfADay();

		Vector2 fromPoint = new Vector2();
		Vector2 toPoint = new Vector2();

		for( int i = 0; i < TotalNumberOfDays; ++i )
		{
			if( GetSettings().ShowFirstIndex + i >= TotalTasks.Length )
			{
				continue;
			}

			fromPoint.x = 42 + i * widthOfADay;
			toPoint.x = 42 + ( i + 1 ) * widthOfADay;

			if( GetSettings().ShowFirstIndex + i == 0 )
			{
				fromPoint.y = Mathf.RoundToInt( Screen.height - 82 );
			}
			else
			{
				fromPoint.y = GetDrawingHeightOfData( TotalTasks[ GetSettings().ShowFirstIndex + i - 1 ] );
			}
			toPoint.y = GetDrawingHeightOfData( TotalTasks[ GetSettings().ShowFirstIndex + i ] );

			Editor2DTools.DrawLine( fromPoint, toPoint, GraphTotalTasks, 1 );

			if( GetSettings().ShowFirstIndex + i == 0 )
			{
				fromPoint.y = Mathf.RoundToInt( Screen.height - 82 );
			}
			else
			{
				fromPoint.y = GetDrawingHeightOfData( CreatedTasks[ GetSettings().ShowFirstIndex + i - 1 ] );
			}
			toPoint.y = GetDrawingHeightOfData( CreatedTasks[ GetSettings().ShowFirstIndex + i ] );

			Editor2DTools.DrawLine( fromPoint, toPoint, GraphCreatedTasks, 1 );

			if( GetSettings().ShowFirstIndex + i == 0 )
			{
				fromPoint.y = Mathf.RoundToInt( Screen.height - 82 );
			}
			else
			{
				fromPoint.y = GetDrawingHeightOfData( CompletedTasks[ GetSettings().ShowFirstIndex + i - 1 ] );
			}
			toPoint.y = GetDrawingHeightOfData( CompletedTasks[ GetSettings().ShowFirstIndex + i ] );

			Editor2DTools.DrawLine( fromPoint, toPoint, GraphCompletedTasks, 1 );
		}
	}
}