using System.Threading;
using UnityEngine;
using System;

public class LaunchieGUI1 : LaunchieGUI
{
	void OnGUI()
	{
		if( Application.isEditor )
		{
			return;
		}
		
		int x = ( Screen.width - width ) / 2;
		int y = ( Screen.height - height ) / 2;

		GUI.skin.box.wordWrap = true;
		GUI.Box( new Rect( x, y, width, height ), _text );
		if( _guistate == 0 )
		{
			GUI.Box( new Rect( x + 50, y + 50, ( float )Math.Max( 12, ( 2 * _progress ) ), 30 ), "" );
			GUI.Box( new Rect( x + 50, y + 50, width - 100, 30 ), Math.Round( _progress, 2 ) + "%" );
		}
		else if( _guistate == 1 )
		{
			bool clicked = GUI.Button( new Rect( x + 50, y + height - 50, width - 100, 30 ), "Download" );
			if( clicked )
			{
				new Thread (llogic.DownloadPatch).Start(); 
			}
		}
		else if( _guistate == 2 )
		{
			bool clicked = GUI.Button( new Rect( x + 50, y + height - 50, width - 100, 30 ), "Close" );
			if( clicked )
			{
				Destroy(gameObject);
			}
		}
		else if( _guistate == 3 || _guistate == 4 )
		{
			GUI.Box( new Rect( x + 50, y + height - 50, width - 100, 30 ), Math.Round( _progress, 2 ) + "%" );
			GUI.Box( new Rect( x + 50, y + height - 50, ( float )Math.Max( 12, ( 2 * _progress ) ), 30 ), "" );
		}
		else if( _guistate == 5 )
		{
			Application.Quit();
		}
	}
}