using System.Threading;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class LaunchieLogic : MonoBehaviour
{
	// location of patches
	public string url;
	
	// current version of game
	public string version;
	
	// enable crafty_error.txt
	public bool debug;
	
	// force restarting game with
	// administator privilages ( Windows only )
	public bool forceAdmin;

	//external error handler
	protected ErrorHandler eh;
	protected bool eh_exists;

	protected Launchie.Launchie _l;
	protected double _progress;
	protected LaunchieGUI lgui;
	
	static List<Func<int>> ExecuteOnMainThread = new List<Func<int>>();
	
	void Update()
	{
		lock(ExecuteOnMainThread)
		{
			if(ExecuteOnMainThread.Count > 0)
			{
				ExecuteOnMainThread[0].Invoke();
				ExecuteOnMainThread.RemoveAt(0);
			}
		}
	}
	
	void Start()
	{
		if( Application.isEditor )
		{
			Debug.LogWarning( "Launchie shouldn't run from editor! It will be explained soon." );
			return;
		}
		
		if( url == null || url == "" || version == null || version == "" )
		{
			Debug.LogWarning( "Launchie `url` and `version` cannot be empty!" );
			return;
		}
		
		lgui = (LaunchieGUI)GetComponent( "LaunchieGUI" );
		eh = (ErrorHandler)GetComponent( "ErrorHandler" );
		eh_exists = ( eh != null );
		
		new Thread( waitTime ).Start();
	}

	void waitTime()
	{
		lgui.setState( 0 );
		lgui.setText( "Please wait a while." );
		
		float _realtimeSinceStartup = 0;
		int _range = 0;
		
		do
		{
			lock(ExecuteOnMainThread)
			{
				ExecuteOnMainThread.Add( () =>
				{
					_realtimeSinceStartup = Time.realtimeSinceStartup;
					_range = UnityEngine.Random.Range(100, 500);
					return 0;
				});
			}

			lgui.setProgress( Math.Round(Math.Min( 3, _realtimeSinceStartup ) * 100 / 3 ));
			Thread.Sleep( _range );
		}
		while(_realtimeSinceStartup < 3 );
		
		new Thread( Asyncwork ).Start();
	}

	protected string FormatSize( double size )
	{
		string unit = "B";
		if( size > 921 ) // 1024 * 90%
		{
			unit = "KB";
			size /= 1024;
		}
		if( size > 921 ) // 1024 * 1024 * 90%
		{
			unit = "MB";
			size /= 1024;
		}
		if( size > 921 ) // 1024 * 1024 * 1024 * 90%
		{
			unit = "GB";
			size /= 1024;
		}
		
		return Math.Round( size, 2 ) + unit;
	}

	
	protected void OnError( System.Exception ex )
	{
		if( eh_exists )
		{
			eh.Add( ex );
		}
	}

	virtual protected void Asyncwork(){}
	virtual public void DownloadPatch(){}
}
