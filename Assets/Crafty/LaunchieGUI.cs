using System.Threading;
using UnityEngine;
using System;

public class LaunchieGUI : MonoBehaviour
{
	protected string _text;
	protected int _guistate = 0;
	protected double _progress = 0;
	
	public Font font;
	public int width = 300;
	public int height = 200;
	
	protected LaunchieLogic llogic;

	public void setState( int guistate )
	{
		_guistate = guistate;
	}
	
	public int getState()
	{
		return _guistate;
	}
	
	public void setText( string text )
	{
		_text = text;
	}
	
	public void setProgress( double progress )
	{
		_progress = progress;
	}
	
	void Start()
	{
		llogic = (LaunchieLogic)GetComponent( "LaunchieLogic" );
		_text = "Checking for updates...";
	}
	
	void OnGUI()
	{
		GUI.skin.font = font;
	}
}