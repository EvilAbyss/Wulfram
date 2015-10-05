using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TodoListTaskPost
{
	public TodoListTaskPost( string text, string transitionTo = "", string assignTo = "", string from = "" )
	{
		Text = text;
		TransitionTo = transitionTo;
		AssignTo = assignTo;
		Date = TodoList.GetUnixTimestampNow();
		From = from;
	}

	public int Date;
	public string TransitionTo;
	public string AssignTo;
	public string Text;
	public string From;
}

[System.Serializable]
public class TodoListTask
{
	public int Order;
	public string Description;
	public string Developer;
	public string Category;
	public int DueDate;
	public int DueTime;
	public bool IsMatchingSearchString;
	public bool IsMatchingVisibilityOptions;
	public int TimeCreated;
	public int TimeCompleted;
	public string Tags;
	public string Status;
	public string Priority;
	public int Effort = 1;
	public int SprintIndex = -1;
	public float Progress = 0f;

	public List<TodoListTaskPost> Posts;

	[SerializeField]
	protected bool isDone;

	[SerializeField]
	protected string PreCompleteStatus;

	public TodoListTask()
	{
		if( Posts == null )
		{
			Posts = new List<TodoListTaskPost>();
		}
	}

	public bool IsDone()
	{
		return isDone;
	}

	public void SetCompleted( bool completed )
	{
		isDone = completed;

		if( isDone )
		{
			TimeCompleted = TodoList.GetUnixTimestampNow();
			PreCompleteStatus = Status;
		}
		else
		{
			Status = PreCompleteStatus;
			TimeCompleted = 0;
		}
	}

	public void AddPost( string text, string transitionTo = "", string assignTo = "" )
	{
		TodoListTaskPost newPost = new TodoListTaskPost( text, transitionTo, assignTo );

		Posts.Insert( 0, newPost );
	}
}