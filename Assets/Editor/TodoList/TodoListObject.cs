using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TodoListObject : ScriptableObject
{
	[SerializeField]
	protected List<TodoListTask> TaskList;

	[SerializeField]
	protected List<string> Developers;

	[SerializeField]
	protected List<string> Categories;

	[SerializeField]
	protected List<TodoListWorkflow> Workflow;

	[SerializeField]
	protected float TodoListVersion = 1f;

	[SerializeField]
	protected List<string> Priority;

	[SerializeField]
	protected int BacklogItemEffortOptions = 4;

	[SerializeField]
	protected List<TodoListSprint> Sprints;

	public void Init( bool creatingNewList = false )
	{
		if( TaskList == null )
		{
			TaskList = new List<TodoListTask>();
		}

		if( Developers == null )
		{
			Developers = new List<string>();
		}

		if( Categories == null )
		{
			Categories = new List<string>();
		}

		if( TaskList == null )
		{
			TaskList = new List<TodoListTask>();
		}

		if( Workflow == null )
		{
			Workflow = new List<TodoListWorkflow>();
		}

		if( Priority == null )
		{
			Priority = new List<string>();
		}

		if( Sprints == null )
		{
			Sprints = new List<TodoListSprint>();
		}

		if( creatingNewList )
		{
			AddDefaultDevelopers();
			AddDefaultCategories();
			AddDefaultWorkflow();
			AddDefaultPriorities();

			TodoListVersion = TodoList.Version;
		}

		UpdateOldListObject();
	}

	protected void UpdateOldListObject()
	{
		string updateString = "";

		if( TodoListVersion < 1.2f )
		{
			UpdateTo12();

			updateString = "Your To-Do List Object has been updated to version 1.2";
		}

		if( TodoListVersion == 1.2f )
		{
			UpdateTo13();

			updateString = "Your To-Do List Object has been updated to version 1.3";
		}

		if( updateString != "" )
		{
			Debug.Log( updateString );
			EditorUtility.SetDirty( this );
		}
	}

	protected void UpdateTo12()
	{
		if( Workflow.Count == 0 )
		{
			AddDefaultWorkflow();
		}

		string defaultStartingStatus = GetDefaultStartingStatus();

		foreach( TodoListTask task in TaskList )
		{
			task.Status = defaultStartingStatus;
			task.Tags = "";
			task.TimeCreated = TodoList.GetUnixTimestampNow();
			task.TimeCompleted = 0;
		}

		TodoListVersion = 1.2f;
	}

	void UpdateTo13()
	{
		TodoListVersion = 1.3f;

		if( Priority.Count == 0 )
		{
			AddDefaultPriorities();
		}
	}

	protected string GetDefaultStartingStatus()
	{
		TodoListWorkflow workflow = Workflow.Find( item => item.Type == TodoListWorkflowType.DefaultStartingStatus );

		if( workflow != null )
		{
			return workflow.Description;
		}

		workflow = Workflow.Find( item => item.Type == TodoListWorkflowType.StartingStatus );

		if( workflow != null )
		{
			return workflow.Description;
		}

		return Workflow[ 0 ].Description;
	}

	public void AddDefaultDevelopers()
	{
		Developers.Add( "Oliver" );
	}

	public void AddDefaultCategories()
	{
		Categories.Add( "Features" );
		Categories.Add( "Bugs" );
		Categories.Add( "Wishlist" );
	}

	public void AddDefaultWorkflow()
	{
		AddWorkflowItem( "Task not started", TodoListWorkflowType.DefaultStartingStatus, new[] { "In Production", "Cancelled", "Completed" } );
		AddWorkflowItem( "In Production", TodoListWorkflowType.Normal, new[] { "Waiting for Feedback", "Waiting for Sign of", "Cancelled", "Completed" } );
		AddWorkflowItem( "Waiting for Feedback", TodoListWorkflowType.Normal, new[] { "Rejected", "In Production", "Waiting for Sign of", "Cancelled", "Completed" } );
		AddWorkflowItem( "Waiting for Sign of", TodoListWorkflowType.Normal, new[] { "Rejected", "Completed" } );
		AddWorkflowItem( "Rejected", TodoListWorkflowType.Normal, new[] { "In Production", "Cancelled", "Completed" } );
		AddWorkflowItem( "Cancelled", TodoListWorkflowType.FinishedStatus, new[] { "Task not started", "In Production" } );

		AddWorkflowItem( "Bug claimed", TodoListWorkflowType.StartingStatus, new[] { "Reproducable", "Confirmed", "Claimed fixed", "Known shipable" } );
		AddWorkflowItem( "Reproducable", TodoListWorkflowType.Normal, new[] { "Claimed fixed", "Known shipable" } );
		AddWorkflowItem( "Confirmed", TodoListWorkflowType.Normal, new[] { "Claimed fixed", "Known shipable" } );
		AddWorkflowItem( "Claimed fixed", TodoListWorkflowType.Normal, new[] { "Fix rejected", "Completed" } );
		AddWorkflowItem( "Fix rejected", TodoListWorkflowType.Normal, new[] { "Reproducable", "Confirmed", "Claimed fixed", "Known shipable" } );
		AddWorkflowItem( "Known shipable", TodoListWorkflowType.FinishedStatus, new[] { "Claimed fixed" } );

		AddWorkflowItem( "Completed", TodoListWorkflowType.DefaultFinishedStatus, new string[ 0 ] );
	}

	public void AddDefaultPriorities()
	{
		Priority.Add( "High" );
		Priority.Add( "Normal" );
		Priority.Add( "Low" );
	}

	void AddWorkflowItem( string description, TodoListWorkflowType type, string[] transitions )
	{
		TodoListWorkflow newItem = new TodoListWorkflow( description );
		newItem.Type = type;

		foreach( string transition in transitions )
		{
			newItem.Transitions.Add( new TodoListTransition( transition ) );
		}

		Workflow.Add( newItem );
	}

	public void AddTask( TodoListTask newTask )
	{
		TaskList.Add( newTask );
	}

	public void Clear()
	{
		TaskList.Clear();
	}

	public List<TodoListTask> GetTaskList()
	{
		return TaskList;
	}

	public TodoListTask GetTask( int index )
	{
		return TaskList[ index ];
	}

	public List<string> GetDevelopersList()
	{
		return Developers;
	}

	public List<string> GetCategoriesList()
	{
		return Categories;
	}

	public List<string> GetPriorityList()
	{
		return Priority;
	}

	public List<TodoListSprint> GetSprintList()
	{
		return Sprints;
	}

	public List<TodoListWorkflow> GetWorkflowList()
	{
		return Workflow;
	}

	public void SetBacklogItemEffortOptions( int newValue )
	{
		BacklogItemEffortOptions = newValue;
	}

	public int GetBacklogItemEffortOptions()
	{
		return BacklogItemEffortOptions;
	}

	/*public void WriteToXml( string filename )
	{
		XmlWriterSettings ws = new XmlWriterSettings();
		ws.NewLineHandling = NewLineHandling.Entitize;
		ws.Encoding = Encoding.UTF8;
		ws.Indent = true;

		XmlSerializer xs = new XmlSerializer( typeof( SongData ) );
		XmlWriter xmlTextWriter = XmlWriter.Create( filename, ws );

		xs.Serialize( xmlTextWriter, this );
		xmlTextWriter.Close();
	}

	protected void LoadFromXml( string xmlData )
	{
		StringReader reader = new StringReader( xmlData );
		
		XmlSerializer xdsg = new XmlSerializer( typeof( SongData ) );

		SongData newSong = (SongData)xdsg.Deserialize( reader );
		reader.Close();

		Name = newSong.Name;
		Band = newSong.Band;

		Bpm = newSong.Bpm;
		StartBeatOffset = newSong.StartBeatOffset;

		Notes = newSong.Notes;
	}*/
}