using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TodoListLocal : TodoListInterface
{
	TodoListObject ListObject;

	public const string TodoListAssetPath = "Assets/Editor/TodoList/TodoList.asset";

	Vector2 DeveloperScrollView;
	Vector2 CategoriesScrollView;
	public TodoListLocal()
	{
		FetchListObject();
	}

	public override void OnEnable()
	{
		//FetchListObject();
	}

	void FetchListObject()
	{
		if( ListObject != null )
		{
			return;
		}

		ListObject = (TodoListObject)AssetDatabase.LoadAssetAtPath( TodoListAssetPath, typeof( TodoListObject ) );
		bool creatingNewList = false;

		if( ListObject == null )
		{
			ListObject = ScriptableObject.CreateInstance<TodoListObject>();

			AssetDatabase.CreateAsset( ListObject, TodoListAssetPath );

			creatingNewList = true;
		}

		ListObject.Init( creatingNewList );

		EditorUtility.SetDirty( ListObject );
	}

	public override void AddTask( TodoListTask newTask )
	{
		ListObject.AddTask( newTask );
		OnTaskAdded();
	}

	public override void Clear()
	{
		ListObject.Clear();
	}

	public override void Refresh()
	{

	}

	public override bool IsLoaded()
	{
		return true;
	}

	public override bool IsValid()
	{
		TodoListObject checkObject = (TodoListObject)AssetDatabase.LoadAssetAtPath( TodoListAssetPath, typeof( TodoListObject ) );

		return checkObject != null;
	}

	public override List<TodoListTask> GetTaskList()
	{
		return ListObject.GetTaskList();
	}

	public override TodoListTask GetTask( int index )
	{
		return ListObject.GetTask( index );
	}

	public override int GetHighestOrder()
	{
		int highest = 0;

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Order > highest )
			{
				highest = task.Order;
			}
		}

		return highest;
	}

	public override bool HasSettingsWindow()
	{
		return true;
	}

	public override void OnDeveloperAdded()
	{
		Undo.RegisterUndo( ListObject, "Add Developer" );

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnDeveloperDeleted( string developerName )
	{
		Undo.RegisterUndo( ListObject, "Delete Developer" );

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Developer == developerName )
			{
				task.Developer = "Unassigned";
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnDeveloperChanged( string oldName, string newName )
	{
		Undo.RegisterUndo( ListObject, "Edit Developer" );

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Developer == oldName )
			{
				task.Developer = newName;
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnTaskDeleted( TodoListTask deletedTask )
	{
		base.OnTaskDeleted( deletedTask );

		Undo.RegisterUndo( ListObject, "Delete Task" );
		EditorUtility.SetDirty( ListObject );
	}

	public override void OnTaskChanged( TodoListTask changedTask, string undoDescription = "Edit Task" )
	{
		base.OnTaskChanged( changedTask );

		Undo.RegisterUndo( ListObject, undoDescription );
		EditorUtility.SetDirty( ListObject );
	}

	public override void OnTaskAdded()
	{
		base.OnTaskAdded();

		Undo.RegisterUndo( ListObject, "Add Task" );
		EditorUtility.SetDirty( ListObject );
	}

	public override void OnCategoryAdded()
	{
		Undo.RegisterUndo( ListObject, "Add Category" );

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnCategoryDeleted( string categoryName )
	{
		Undo.RegisterUndo( ListObject, "Delete Category" );

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Category == categoryName )
			{
				task.Category = "Unassigned";
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnCategoryChanged( string oldName, string newName )
	{
		Undo.RegisterUndo( ListObject, "Edit Category" );

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Category == oldName )
			{
				task.Category = newName;
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnWorkflowItemAdded()
	{
		RegisterUndo( "Add Workflow Item" );

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnWorkflowItemEdited( string oldDesc, string newDesc )
	{
		RegisterUndo( "Edit Workflow Item" );

		if( oldDesc != newDesc )
		{
			foreach( TodoListTask task in GetTaskList() )
			{
				if( task.Status == oldDesc )
				{
					task.Status = newDesc;
				}
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnWorkflowItemDeleted()
	{
		RegisterUndo( "Delete Workflow Item" );

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnPriorityAdded()
	{
		Undo.RegisterUndo( ListObject, "Add Priority" );

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnPriorityDeleted( string priorityName )
	{
		Undo.RegisterUndo( ListObject, "Delete Priority" );

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Priority == priorityName )
			{
				task.Priority = "Unassigned";
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnPriorityChanged( string oldDesc, string newDesc )
	{
		Undo.RegisterUndo( ListObject, "Edit Priority" );

		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.Priority == oldDesc )
			{
				task.Priority = newDesc;
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override int GetNumberOfBacklogItemEffortOptions()
	{
		return ListObject.GetBacklogItemEffortOptions();
	}

	public override void SetNumberOfBacklogItemEffortOptions( int newValue )
	{
		ListObject.SetBacklogItemEffortOptions( newValue );
	}

	public override void RegisterUndo( string description )
	{
		Undo.RegisterUndo( ListObject, description );
	}

	public override TodoListType GetListType()
	{
		return TodoListType.Local;
	}

	public override List<string> GetDevelopersList()
	{
		return ListObject.GetDevelopersList();
	}

	public override List<string> GetCategoriesList()
	{
		return ListObject.GetCategoriesList();
	}

	public override List<TodoListWorkflow> GetWorkflowList()
	{
		return ListObject.GetWorkflowList();
	}

	public override List<string> GetPriorityList()
	{
		return ListObject.GetPriorityList();
	}

	public override List<TodoListSprint> GetSprintList()
	{
		return ListObject.GetSprintList();
	}

	public override void OnSprintAdded()
	{
		ListObject.GetSprintList().Sort( delegate( TodoListSprint s1, TodoListSprint s2 )
		{
			return s1.StartDate.CompareTo( s2.StartDate );
		} );

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnSprintDeleted( int sprintIndex )
	{
		foreach( TodoListTask task in ListObject.GetTaskList() )
		{
			if( task.SprintIndex == sprintIndex )
			{
				task.SprintIndex = -1;
			}
		}

		EditorUtility.SetDirty( ListObject );
	}

	public override void OnSprintChanged()
	{
		EditorUtility.SetDirty( ListObject );
	}
}