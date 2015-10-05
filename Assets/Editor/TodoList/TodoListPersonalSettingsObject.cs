using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TodoListPersonalSettingsObject : ScriptableObject 
{
	[SerializeField]
	public bool[] SelectedStatus;

	[SerializeField]
	public bool[] SelectedCategories;

	[SerializeField]
	public bool[] SelectedDevelopers;

	[SerializeField]
	public bool[] SelectedPriority;

	[SerializeField]
	public bool[] SelectedSprints;

	[SerializeField]
	public int[] FieldSizes;

	[SerializeField]
	public TodoListModes DisplayMode = TodoListModes.Connection;

	[SerializeField]
	public bool ShowBacklogTasks;

	[SerializeField]
	public List<bool> ShowSprintTasks = new List<bool>();


	[NonSerialized]
	public TodoListTimeWindowTypes DisplayTimeWindow = TodoListTimeWindowTypes.AllTime;

	[NonSerialized]
	public DateTime ShowFirstDay = new DateTime( 0 );

	[NonSerialized]
	public DateTime ShowLastDay = new DateTime( 9999, 1, 1 );

	[NonSerialized]
	public int ShowFirstIndex = -1;

	[NonSerialized]
	public int ShowLastIndex = -1;
}
