using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class TodoListFirstRun
{
	static float StartingTime;
	static TodoListFirstRun()
	{
		StartingTime = Time.realtimeSinceStartup;

		EditorApplication.update += RunDelayed;
	}

	static void RunDelayed()
	{
		if( Time.realtimeSinceStartup - StartingTime < 1 )
		{
			return;
		}

		EditorApplication.update -= RunDelayed;

		float savedVersion = EditorPrefs.GetFloat( "TodoListFirstRun", 0 );

		if( savedVersion == 0 )
		{
			EditorPrefs.SetFloat( "TodoListFirstRun", TodoList.Version );

			Debug.Log( "Thank you for installing my To-Do list. To get started, select 'Window' -> 'To-Do List' in the menu at the top. You can also use CTRL + T on Windows and CMD + T on MacOS to toggle the To-Do List window." );
		}
		/*else if( savedVersion < TodoList.Version )
		{
			EditorPrefs.SetFloat( "TodoListFirstRun", TodoList.Version );

			Debug.Log( "You have successfully updated your To-Do List to Version " + TodoList.StringVersion );
		}*/
	}
}