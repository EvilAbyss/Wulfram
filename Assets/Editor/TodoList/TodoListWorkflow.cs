using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TodoListTransition
{
	public TodoListTransition( string to )
	{
		To = to;
	}

	public string To;
}

[System.Serializable]
public enum TodoListWorkflowType
{
	Normal,
	DefaultStartingStatus,
	StartingStatus,
	DefaultFinishedStatus,
	FinishedStatus,
}

[System.Serializable]
public class TodoListWorkflow
{
	public TodoListWorkflow( string description )
	{
		Description = description;
		Transitions = new List<TodoListTransition>();
		Type = TodoListWorkflowType.Normal;
	}

	public string Description;
	public TodoListWorkflowType Type;
	public List<TodoListTransition> Transitions;
}