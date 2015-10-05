using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class ErrorHandler : MonoBehaviour
{
	
	// enable crafty_error.txt
	public bool showErrors;
	
	Queue<System.Exception> eq;
	System.Exception currentEx;
	
	public void Add( System.Exception ex )
	{
		lock( eq )
		{
			eq.Enqueue( ex );
		}
	}
	
	void Start ()
	{
		eq = new Queue<System.Exception>();
	}
	
	void Update ()
	{
		System.Exception ex = null;
		lock( eq )
		{
			if( eq.Count > 0 )
			{
				ex = eq.Dequeue();
			}
		}
		if
		(
			ex != null && showErrors
		)
		{
			var mb = gameObject.GetComponents<MonoBehaviour>();
			foreach( var s in mb )
			{
				try
				{
					if( !(s is ErrorHandler ) )
						UnityEngine.Object.Destroy( s );
				}
				catch{}
			}
			Clean();
			currentEx = ex;
		}
	}
	
	void Clean()
	{
		UnityEngine.Object[] objs = GameObject.FindObjectsOfType( typeof( GameObject ) );
			foreach( GameObject obj in objs )
			{
				try
				{
					if( !( gameObject.Equals(obj) || obj.GetComponent( typeof(Camera) ) ) )
						GameObject.DestroyImmediate( obj );
				}
				catch{};
			}
			
			objs = UnityEngine.Camera.main.GetComponents( typeof( Component ) );
			foreach( Component obj in objs )
			{
				try
				{
					if( !( obj is UnityEngine.GUILayer ) && !( obj is UnityEngine.Camera ) && !( obj is UnityEngine.Transform ) )
						GameObject.DestroyImmediate( obj );
				}
				catch{};
			}
	}
	
	void OnGUI ()
	{
		if( currentEx != null )
		{
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.contentColor = Color.red;
			GUI.backgroundColor = Color.green;
			GUI.color = Color.white;
				
			var err_string = currentEx.ToString();
			if( err_string.Length == 0 )
				err_string = currentEx.GetType().ToString();
			
			GUI.Box( new Rect( (float)(0.1 * Screen.width), (float)(0.1 * Screen.height), (float)(0.8f * Screen.width), (float)(0.8f * Screen.height) ), err_string );
		}
	}
}
