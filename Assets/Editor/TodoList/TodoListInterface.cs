using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public enum TodoListType
{
	Local,
	Remote,
}

public enum TodoListSortType
{
	DescriptionAsc,
	DescriptionDesc,
	DueDateAsc,
	DueDateDesc,
	DueTimeAsc,
	DueTimeDesc,
	OrderAsc,
	OrderDesc,
	EffortAsc,
	EffortDesc,
	PriorityAsc,
	PriorityDesc,
	CategoryAsc,
	CategoryDesc,
	DeveloperAsc,
	DeveloperDesc,
	StatusAsc,
	StatusDesc,
	SprintAsc,
	SprintDesc,
	SameAsBefore,
}

public abstract class TodoListInterface
{
	public abstract void OnEnable();

	public abstract void AddTask( TodoListTask newTask );
	public abstract void Clear();
	public abstract void Refresh();

	public abstract bool IsLoaded();
	public abstract bool IsValid();

	public abstract List<TodoListTask> GetTaskList();
	public abstract TodoListTask GetTask( int index );
	public abstract int GetHighestOrder();

	public abstract bool HasSettingsWindow();

	public abstract List<string> GetDevelopersList();
	public abstract List<string> GetCategoriesList();
	public abstract List<TodoListWorkflow> GetWorkflowList();
	public abstract List<string> GetPriorityList();
	public abstract List<TodoListSprint> GetSprintList();

	protected TodoListPersonalSettingsObject SettingsObject;

	public string[] GetDevelopersStringList()
	{
		string[] options = new string[ GetDevelopersList().Count + 1 ];

		options[ 0 ] = "Unassigned";
		for( int i = 0; i < GetDevelopersList().Count; ++i )
		{
			options[ i + 1 ] = GetDevelopersList()[ i ];
		}

		return options;
	}

	public string[] GetCategoriesStringList()
	{
		string[] options = new string[ GetCategoriesList().Count + 1 ];

		options[ 0 ] = "Unassigned";
		for( int i = 0; i < GetCategoriesList().Count; ++i )
		{
			options[ i + 1 ] = GetCategoriesList()[ i ];
		}

		return options;
	}

	public string[] GetWorkflowStringList()
	{
		List<string> stringList = new List<string>();

		foreach( TodoListWorkflow workflow in GetWorkflowList() )
		{
			stringList.Add( workflow.Description );
		}

		return stringList.ToArray();
	}

	public string[] GetPriorityStringList()
	{
		string[] options = new string[ GetPriorityList().Count + 1 ];

		options[ 0 ] = "Unassigned";
		for( int i = 0; i < GetPriorityList().Count; ++i )
		{
			options[ i + 1 ] = GetPriorityList()[ i ];
		}

		return options;
	}

	public string[] GetSprintsStringList()
	{
		string[] options = new string[ GetSprintList().Count + 1 ];

		options[ 0 ] = "Unassigned";

		for( int i = 0; i < GetSprintList().Count; ++i )
		{
			options[ i + 1 ] = GetSprintList()[ i ].Name;
		}

		return options;
	}

	public List<string> GetTransitions( string workflow )
	{
		List<string> transitions = new List<string>();

		foreach( TodoListWorkflow item in GetWorkflowList() )
		{
			if( item.Description == workflow )
			{
				foreach( TodoListTransition transition in item.Transitions )
				{
					transitions.Add( transition.To );
				}

				break;
			}
		}

		return transitions;
	}

	public abstract int GetNumberOfBacklogItemEffortOptions();
	public abstract void SetNumberOfBacklogItemEffortOptions( int newValue );

	public abstract void RegisterUndo( string description );

	public abstract TodoListType GetListType();

	TodoListSortType SortType = TodoListSortType.OrderAsc;

	public TodoListSortType GetSortType()
	{
		return SortType;
	}

	public TodoListPersonalSettingsObject GetPersonalSettings()
	{
		if( SettingsObject == null )
		{
			SettingsObject = (TodoListPersonalSettingsObject)AssetDatabase.LoadAssetAtPath( TodoList.PersonalSettingsPath, typeof( TodoListPersonalSettingsObject ) );

			if( SettingsObject == null )
			{
				SettingsObject = ScriptableObject.CreateInstance<TodoListPersonalSettingsObject>();

				AssetDatabase.CreateAsset( SettingsObject, TodoList.PersonalSettingsPath );
			}
		}

		return SettingsObject;
	}

	public void SortTaskList( TodoListSortType sortType = TodoListSortType.SameAsBefore )
	{
		if( sortType != TodoListSortType.SameAsBefore )
		{
			SortType = sortType;
		}

		switch( SortType )
		{
		case TodoListSortType.DescriptionAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDescriptionAsc ) );
			break;
		case TodoListSortType.DescriptionDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDescriptionDesc ) );
			break;
		case TodoListSortType.DueDateAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDueDateAsc ) );
			break;
		case TodoListSortType.DueDateDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDueDateDesc ) );
			break;
		case TodoListSortType.DueTimeAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDueTimeAsc ) );
			break;
		case TodoListSortType.DueTimeDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDueTimeDesc ) );
			break;
		case TodoListSortType.OrderAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByOrderAsc ) );
			break;
		case TodoListSortType.OrderDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByOrderDesc ) );
			break;
		case TodoListSortType.EffortAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByEffortAsc ) );
			break;
		case TodoListSortType.EffortDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByEffortDesc ) );
			break;
		case TodoListSortType.PriorityAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByPriorityAsc ) );
			break;
		case TodoListSortType.PriorityDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByPriorityDesc ) );
			break;
		case TodoListSortType.CategoryAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByCategoryAsc ) );
			break;
		case TodoListSortType.CategoryDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByCategoryDesc ) );
			break;
		case TodoListSortType.DeveloperAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDeveloperAsc ) );
			break;
		case TodoListSortType.DeveloperDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByDeveloperDesc ) );
			break;
		case TodoListSortType.StatusAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByStatusAsc ) );
			break;
		case TodoListSortType.StatusDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortByStatusDesc ) );
			break;
		case TodoListSortType.SprintAsc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortBySprintAsc ) );
			break;
		case TodoListSortType.SprintDesc:
			GetTaskList().Sort( new System.Comparison<TodoListTask>( TaskListSortBySprintDesc ) );
			break;
		}
	}

	int TaskListSortByDescriptionAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj1.Description.CompareTo( obj2.Description );
	}

	int TaskListSortByDescriptionDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj2.Description.CompareTo( obj1.Description );
	}

	int TaskListSortByDueDateAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj1.DueDate.CompareTo( obj2.DueDate );
	}

	int TaskListSortByDueDateDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj2.DueDate.CompareTo( obj1.DueDate );
	}

	int TaskListSortByDueTimeAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj1.DueTime.CompareTo( obj2.DueTime );
	}

	int TaskListSortByDueTimeDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj2.DueTime.CompareTo( obj1.DueTime );
	}

	int TaskListSortByOrderAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj1.Order.CompareTo( obj2.Order );
	}

	int TaskListSortByOrderDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj2.Order.CompareTo( obj1.Order );
	}

	int TaskListSortByStatusAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj1.Status.CompareTo( obj2.Status );
	}

	int TaskListSortByStatusDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj2.Status.CompareTo( obj1.Status );
	}

	int TaskListSortByEffortAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj1.Effort.CompareTo( obj2.Effort );
	}

	int TaskListSortByEffortDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		return obj2.Effort.CompareTo( obj1.Effort );
	}

	int TaskListSortByPriorityAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.Priority, obj2.Priority, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		List<string> priorityList = new List<string>( GetPriorityList() );

		return priorityList.FindIndex( item => item == obj1.Priority ).CompareTo( priorityList.FindIndex( item => item == obj2.Priority ) );
	}

	int TaskListSortByPriorityDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.Priority, obj2.Priority, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		List<string> priorityList = new List<string>( GetPriorityList() );

		return priorityList.FindIndex( item => item == obj2.Priority ).CompareTo( priorityList.FindIndex( item => item == obj1.Priority ) );
	}

	int TaskListSortBySprintAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.SprintIndex, obj2.SprintIndex, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		return obj1.SprintIndex.CompareTo( obj2.SprintIndex );
	}

	int TaskListSortBySprintDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.SprintIndex, obj2.SprintIndex, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		return obj2.SprintIndex.CompareTo( obj1.SprintIndex );
	}

	int TaskListSortByCategoryAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.Category, obj2.Category, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		return obj1.Category.CompareTo( obj2.Category );
	}

	int TaskListSortByCategoryDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.Category, obj2.Category, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		return obj2.Category.CompareTo( obj1.Category );
	}

	int TaskListSortByDeveloperAsc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.Developer, obj2.Developer, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		return obj1.Developer.CompareTo( obj2.Developer );
	}

	int TaskListSortByDeveloperDesc( TodoListTask obj1, TodoListTask obj2 )
	{
		int returnUnassignedValue = 0;
		if( ReturnUnassignedSortingValues( obj1.Developer, obj2.Developer, out returnUnassignedValue ) )
		{
			return returnUnassignedValue;
		}

		return obj2.Developer.CompareTo( obj1.Developer );
	}

	bool ReturnUnassignedSortingValues( string obj1, string obj2, out int returnUnassignedValue )
	{
		returnUnassignedValue = 0;

		if( ( obj2 == "Unassigned" || obj2 == "" ) && ( obj1 == "Unassigned" || obj1 == "" ) )
		{
			returnUnassignedValue = 0;
			return true;
		}

		if( obj2 == "Unassigned" || obj2 == "" )
		{
			returnUnassignedValue = -1;
			return true;
		}

		if( obj1 == "Unassigned" || obj1 == "" )
		{
			returnUnassignedValue = 1;
			return true;
		}

		return false;
	}

	bool ReturnUnassignedSortingValues( int obj1, int obj2, out int returnUnassignedValue )
	{
		returnUnassignedValue = 0;

		if( obj2 == -1 && obj1 == -1 )
		{
			returnUnassignedValue = 0;
			return true;
		}

		if( obj2 == -1 )
		{
			returnUnassignedValue = -1;
			return true;
		}

		if( obj1 == -1 )
		{
			returnUnassignedValue = 1;
			return true;
		}

		return false;
	}

	public bool IsStatusFinished( string status )
	{
		foreach( TodoListWorkflow workflow in GetWorkflowList() )
		{
			if( workflow.Description == status )
			{
				if( workflow.Type == TodoListWorkflowType.FinishedStatus || workflow.Type == TodoListWorkflowType.DefaultFinishedStatus )
				{
					return true;
				}
			}
		}

		return false;
	}

	public virtual void OnTaskDeleted( TodoListTask deletedTask )
	{
		foreach( TodoListTask task in GetTaskList() )
		{
			if( task.Order > deletedTask.Order )
			{
				task.Order--;
			}
		}
	}

	public virtual void OnTaskAdded()
	{
		SortTaskList( SortType );
	}

	public virtual void OnTaskChanged( TodoListTask changedTask, string undoDescription = "Edit Task" )
	{
		
	}

	public virtual void OnCategoryAdded()
	{

	}

	public virtual void OnCategoryDeleted( string categoryName )
	{

	}

	public virtual void OnCategoryChanged( string oldName, string newName )
	{

	}

	public virtual void OnDeveloperAdded()
	{

	}

	public virtual void OnDeveloperDeleted( string developerName )
	{

	}

	public virtual void OnDeveloperChanged( string oldName, string newName )
	{

	}

	public virtual void OnWorkflowItemAdded()
	{

	}

	public virtual void OnWorkflowItemEdited( string oldDesc, string newDesc )
	{

	}

	public virtual void OnWorkflowItemDeleted()
	{

	}

	public virtual void OnPriorityAdded()
	{

	}

	public virtual void OnPriorityDeleted( string priorityName )
	{

	}

	public virtual void OnPriorityChanged( string oldDesc, string newDesc )
	{

	}

	public virtual void OnSprintAdded()
	{
		
	}

	public virtual void OnSprintDeleted( int sprintIndex )
	{

	}

	public virtual void OnSprintChanged()
	{

	}
}