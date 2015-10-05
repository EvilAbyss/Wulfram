using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public delegate void TodoListStringListAddCallback();
public delegate void TodoListStringListDeleteCallback( string item );
public delegate void TodoListStringListChangedCallback( string oldItem, string newItem );

public static class TodoListStringList
{
	static bool MoveCursorToEnd = false;
	static Texture2D DeleteTaskTexture;

	public static void Display( ref Vector2 ScrollView, List<string> stringList, TodoListStringListAddCallback onAddCallback, TodoListStringListDeleteCallback onDeleteCallback, TodoListStringListChangedCallback onChangedCallback )
	{
		if( DeleteTaskTexture == null )
		{
			DeleteTaskTexture = (Texture2D)UnityEngine.Resources.Load( TodoList.GetImageFolder( GUI.skin.name ) + "Cross", typeof( Texture2D ) );
		}

		GUIStyle buttonStyle = new GUIStyle( "button" );
		buttonStyle.fixedWidth = 19;
		buttonStyle.fixedHeight = 15;
		buttonStyle.margin.left = 2;
		buttonStyle.margin.right = 4;

		if( MoveCursorToEnd && Event.current != null && Event.current.type == EventType.repaint )
		{
			TextEditor te = (TextEditor)GUIUtility.GetStateObject( typeof( TextEditor ), GUIUtility.keyboardControl );
			if( te != null )
			{
				te.MoveCursorToPosition( new Vector2( 5555, 5555 ) );
			}

			MoveCursorToEnd = false;
		}

		GUIStyle scrollViewStyle = new GUIStyle( "ScrollView" );
		scrollViewStyle.fixedHeight = 100;

		ScrollView = EditorGUILayout.BeginScrollView( ScrollView, GUILayout.Height( Screen.height - 93 ) );
		{
			for( int i = 0; i < stringList.Count; ++i )
			{
				string newString = "";

				EditorGUILayout.BeginHorizontal();
				{
					newString = EditorGUILayout.TextField( stringList[ i ] );

					if( GUILayout.Button( new GUIContent( DeleteTaskTexture ), buttonStyle ) )
					{
						EditorApplication.Beep();
						if( EditorUtility.DisplayDialog( "Please Confirm", "Do you really want to delete the item '" + stringList[ i ] + "'?", "Yes", "No" ) )
						{
							onDeleteCallback( stringList[ i ] );
							stringList.RemoveAt( i );
						}
					}
				}
				EditorGUILayout.EndHorizontal();

				if( GUI.changed )
				{
					if( newString != stringList[ i ] )
					{
						if( stringList.Exists( item => item == newString ) )
						{
							if( Event.current.keyCode == KeyCode.Backspace )
							{
								newString = newString.Substring( 0, newString.Length - 1 );
							}
							else
							{
								newString += "/";
								MoveCursorToEnd = true;
							}
						}

						onChangedCallback( stringList[ i ], newString );
						stringList[ i ] = newString;
					}
				}
			}
		}
		EditorGUILayout.EndScrollView();

		if( GUILayout.Button( "Add" ) )
		{
			onAddCallback();
			ScrollView = new Vector2( 0, Mathf.Infinity );
			stringList.Add( "" );
		}
	}
}