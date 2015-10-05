using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public delegate void TodoListWorkflowListAddCallback();
public delegate void TodoListWorkflowListEditCallback( string oldDesc = "", string newDesc = "" );
public delegate void TodoListWorkflowListDeleteCallback();

public class TodoListWorkflowList
{
	static List<TodoListWorkflow> Workflow;
	static Texture2D DeleteTaskTexture;
	static Texture2D EditTaskTexture;

	static TodoListWorkflowListAddCallback AddCallback;
	static TodoListWorkflowListEditCallback EditCallback;
	static TodoListWorkflowListDeleteCallback DeleteCallback;

	static int EditItemIndex = -1;
	static bool EditItemShowTransitions = true;

	static Vector2 WorkflowItemScollList;

	static Vector2 ScrollView;

	public static void Display( TodoListWindow window, List<TodoListWorkflow> workflow
		, TodoListWorkflowListAddCallback addCallback
		, TodoListWorkflowListEditCallback editCallback
		, TodoListWorkflowListDeleteCallback deleteCallback )
	{
		if( workflow == null )
		{
			Debug.LogError( "Workflow List is null" );
			return;
		}

		if( DeleteTaskTexture == null )
		{
			DeleteTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Cross", typeof( Texture2D ) );
		}

		if( EditTaskTexture == null )
		{
			EditTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Edit", typeof( Texture2D ) );
		}

		Workflow = workflow;
		AddCallback = addCallback;
		EditCallback = editCallback;
		DeleteCallback = deleteCallback;

		EditorGUILayout.BeginVertical( GUILayout.Width( Mathf.Max( 295, Screen.width / 3 ) ) );
		{
			GUILayout.Space( 5 );

			DisplayHeadline();

			if( EditItemIndex == -1 )
			{
				DisplayWorkflowItems();
				DisplayAddButton();
			}
			else
			{
				DisplayWorkflowItemEdit();
			}
		}
		EditorGUILayout.EndVertical();
	}

	static void DisplayHeadline()
	{
		GUIStyle style = TodoList.GetHeadlineStyle();

		EditorGUILayout.BeginHorizontal();
		{
			if( EditItemIndex != -1 )
			{
				GUILayout.Label( "Edit Workflow Item", style );
			}
			else
			{
				GUILayout.Label( "Workflow", style );
			}
		}
		EditorGUILayout.EndHorizontal();
	}

	static void DisplayWorkflowItems()
	{
		GUIStyle buttonStyle = new GUIStyle( "button" );
		buttonStyle.fixedWidth = 19;
		buttonStyle.fixedHeight = 15;
		buttonStyle.margin.left = 0;
		buttonStyle.margin.right = 4;

		ScrollView = EditorGUILayout.BeginScrollView( ScrollView, GUILayout.Height( Screen.height - 93 ) );
		{
			for( int i = 0; i < Workflow.Count; ++i )
			{
				EditorGUILayout.BeginHorizontal();
				{
					GUILayout.Label( Workflow[ i ].Description );

					if( GUILayout.Button( new GUIContent( EditTaskTexture ), buttonStyle ) )
					{
						EditItemIndex = i;
						WorkflowItemScollList = Vector2.zero;
					}

					if( GUILayout.Button( new GUIContent( DeleteTaskTexture ), buttonStyle ) )
					{
						EditorApplication.Beep();
						if( EditorUtility.DisplayDialog( "Please Confirm", "Do you really want to delete the item '" + Workflow[ i ].Description + "'?", "Yes", "No" ) )
						{
							DeleteCallback();
							Workflow.Remove( Workflow[ i ] );
						}
					}
				}
				EditorGUILayout.EndHorizontal();
			}
		}
		EditorGUILayout.EndScrollView();
	}

	static void DisplayAddButton()
	{
		if( GUILayout.Button( "Add" ) )
		{
			AddCallback();
			Workflow.Add( new TodoListWorkflow( "" ) );
			EditItemIndex = Workflow.Count - 1;
		}
	}

	public static string[] GetWorkflowNames( List<TodoListWorkflow> list = null )
	{
		if( list == null )
		{
			list = Workflow;
		}

		List<string> transitionNames = new List<string>();

		for( int i = 0; i < list.Count; ++i )
		{
			transitionNames.Add( list[ i ].Description );
		}

		return transitionNames.ToArray();
	}

	public static int GetSelectedWorkflowIndex( string selection, List<TodoListWorkflow> list = null )
	{
		if( list == null )
		{
			list = Workflow;
		}

		return list.FindIndex( item => item.Description == selection );
	}

	static void DisplayWorkflowItemEdit()
	{
		GUIStyle buttonStyle = new GUIStyle( "button" );
		buttonStyle.fixedWidth = 19;
		buttonStyle.fixedHeight = 15;
		buttonStyle.margin.left = 0;
		buttonStyle.margin.right = 4;

		string newDesc = EditorGUILayout.TextField( "Description", Workflow[ EditItemIndex ].Description );
		TodoListWorkflowType newType = (TodoListWorkflowType)EditorGUILayout.EnumPopup( "Type", Workflow[ EditItemIndex ].Type );

		GUIStyle foldoutStyle = new GUIStyle( EditorStyles.foldout );
		foldoutStyle.fontStyle = FontStyle.Bold;

		string[] transitionNames = GetWorkflowNames();

		if( EditItemShowTransitions = EditorGUILayout.Foldout( EditItemShowTransitions, "Transitions", foldoutStyle ) )
		{
			WorkflowItemScollList = EditorGUILayout.BeginScrollView( WorkflowItemScollList, GUILayout.Height( Screen.height - 175 ) );
			{
				for( int i = 0; i < Workflow[ EditItemIndex ].Transitions.Count; ++i )
				{
					int selection = GetSelectedWorkflowIndex( Workflow[ EditItemIndex ].Transitions[ i ].To );
					int newSelection = selection;

					EditorGUILayout.BeginHorizontal();
					{
						newSelection = EditorGUILayout.Popup( "Transition To", selection, transitionNames );

						if( GUILayout.Button( new GUIContent( DeleteTaskTexture ), buttonStyle ) )
						{
							EditorApplication.Beep();
							if( EditorUtility.DisplayDialog( "Please confirm", "Do you really want to delete the transition to '" + Workflow[ EditItemIndex ].Transitions[ i ].To + "' ?", "Yes", "No" ) )
							{
								EditCallback();
								Workflow[ EditItemIndex ].Transitions.RemoveAt( i );
								continue;
							}
						}
					}
					EditorGUILayout.EndHorizontal();

					if( GUI.changed )
					{
						if( newSelection != selection )
						{
							EditCallback();

							Workflow[ EditItemIndex ].Transitions[ i ].To = transitionNames[ newSelection ];
						}
					}
				}
			}
			EditorGUILayout.EndScrollView();

			if( GUILayout.Button( "Add Transition" ) )
			{
				TodoListTransition newTransition = new TodoListTransition( Workflow[ EditItemIndex ].Description );

				EditCallback();

				Workflow[ EditItemIndex ].Transitions.Add( newTransition );
			}
		}

		GUILayout.Box( "", TodoList.GetHeadlineStyle(), GUILayout.Height( 2 ) );

		if( GUILayout.Button( "Back to Overview" ) )
		{
			GUIUtility.keyboardControl = 0;
			EditItemIndex = -1;
		}

		GUILayout.Space( 7 );

		if( GUI.changed )
		{
			if( newDesc != Workflow[ EditItemIndex ].Description )
			{
				string oldDesc = Workflow[ EditItemIndex ].Description;
				
				for( int i = 0; i < Workflow.Count; ++i )
				{
					for( int j = 0; j < Workflow[ i ].Transitions.Count; ++j )
					{
						if( Workflow[ i ].Transitions[ j ].To == oldDesc )
						{
							Workflow[ i ].Transitions[ j ].To = newDesc;
						}
					}
				}

				EditCallback( oldDesc, newDesc );
				Workflow[ EditItemIndex ].Description = newDesc;
			}

			if( newType != Workflow[ EditItemIndex ].Type )
			{
				bool cancelledEdit = false;

				if( newType == TodoListWorkflowType.DefaultStartingStatus
				 || newType == TodoListWorkflowType.DefaultFinishedStatus )
				{
					TodoListWorkflow existingItem = Workflow.Find( item => item.Type == newType );

					if( existingItem != null )
					{
						EditorApplication.Beep();
						if( EditorUtility.DisplayDialog( "Default Status already exists",
							"The workflow item '" + existingItem.Description + "' already has the status type '" + newType.ToString() + "'.\nDo you still want to change it and remove the default status type for '" + existingItem.Description + "'?", "Yes", "No" ) )
						{
							if( newType == TodoListWorkflowType.DefaultStartingStatus )
							{
								existingItem.Type = TodoListWorkflowType.StartingStatus;
							}
							else
							{
								existingItem.Type = TodoListWorkflowType.FinishedStatus;
							}
						}
						else
						{
							cancelledEdit = true;
						}
					}
				}

				if( cancelledEdit == false )
				{
					EditCallback();
					Workflow[ EditItemIndex ].Type = newType;
				}
			}
		}
	}
}