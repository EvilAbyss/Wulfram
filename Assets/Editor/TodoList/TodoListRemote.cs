using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TodoListRemote : TodoListInterface
{
	protected List<TodoListTask> TaskList;

	protected string Server = "";
	protected string Username = "";
	protected string Password = "";
	protected string ProjectName = "";

	protected bool isLoaded;

	public TodoListRemote( string server, string username, string password, string projectName )
	{
		Server = server;
		Username = username;
		Password = password;
		ProjectName = projectName;

		isLoaded = false;

		TaskList = new List<TodoListTask>();

		Refresh();
	}

	public override void OnEnable()
	{

	}

	public override void AddTask( TodoListTask newTask )
	{

	}

	public override void Clear()
	{
		TaskList.Clear();
	}

	public override void Refresh()
	{

	}

	public override bool IsLoaded()
	{
		return isLoaded;
	}

	public override bool IsValid()
	{
		return true;
	}

	public override List<TodoListTask> GetTaskList()
	{
		return TaskList;
	}

	public override TodoListTask GetTask( int index )
	{
		return TaskList[ index ];
	}

	public override int GetHighestOrder()
	{
		return 1;
	}

	public override bool HasSettingsWindow()
	{
		return false;
	}

	public override List<string> GetDevelopersList()
	{
		return new List<string>();
	}

	public override List<string> GetCategoriesList()
	{
		return new List<string>();
	}

	public override List<TodoListWorkflow> GetWorkflowList()
	{
		return null;
	}

	public override List<string> GetPriorityList()
	{
		return new List<string>();
	}

	public override List<TodoListSprint> GetSprintList()
	{
		return new List<TodoListSprint>();
	}

	public override int GetNumberOfBacklogItemEffortOptions()
	{
		return 1;
	}

	public override void SetNumberOfBacklogItemEffortOptions( int newValue )
	{
		
	}

	public override void RegisterUndo( string description )
	{
		
	}

	public override TodoListType GetListType()
	{
		return TodoListType.Remote;
	}

	public override void OnTaskDeleted( TodoListTask deletedTask )
	{

	}

	public override void OnTaskChanged( TodoListTask changedTask, string undoDescription = "Edit Task" )
	{

	}

	public override void OnTaskAdded()
	{

	}
}