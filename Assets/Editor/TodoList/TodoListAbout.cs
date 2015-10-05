using UnityEngine;
using UnityEditor;
using System.Collections;

public class TodoListAbout
{
	//TodoListWindow Window;

	Texture2D IconTexture;
	Texture2D EmailTexture;
	Texture2D FacebookTexture;
	Texture2D TwitterTexture;
	Texture2D YouTubeTexture;
	Texture2D WebTexture;

	public TodoListAbout( TodoListWindow window )
	{
		//Window = window;
	}

	public void Display()
	{
		LoadTextures();

		GUI.DrawTexture( new Rect( 10, 30, 64, 64 ), IconTexture );

		GUIStyle headline = new GUIStyle( "label" );
		headline.fontSize = 18;
		headline.fontStyle = FontStyle.Bold;

		GUI.Label( new Rect( 85, 30, 200, 26 ), "To-Do List", headline );
		GUI.Label( new Rect( 85, 50, 200, 18 ), "by Oliver Eberlei" );
		GUI.Label( new Rect( 85, 75, 200, 18 ), "Version " + TodoList.StringVersion );

		GUI.Label( new Rect( 8, 105, 200, 26 ), "Contact me", headline );

		DrawIconAndText( 43, 135, EmailTexture, "todo@olivereberlei.com", "mailto:todo@olivereberlei.com" );
		DrawIconAndText( 280, 135, WebTexture, "olivereberlei.com", "http://www.olivereberlei.com" );

		//DrawIconAndText( 43, 170, FacebookTexture, "facebook.com/olivereberlei", "http://www.facebook.com/olivereberlei" );
		DrawIconAndText( 280, 170, TwitterTexture, "twitter.com/olivereberlei", "http://twitter.com/olivereberlei" );

		DrawIconAndText( 43, 170, YouTubeTexture, "youtube.com/user/olivereberlei", "http://www.youtube.com/user/olivereberlei" );
	}

	void DrawIconAndText( int x, int y, Texture2D texture, string text, string url )
	{
		bool isMouseOver = false;
		Rect clickRect = new Rect( x, y, 225, 32 );

		if( Event.current != null )
		{
			if( clickRect.Contains( Event.current.mousePosition ) )
			{
				isMouseOver = true;
			}
		}

		GUI.DrawTexture( new Rect( x, y, 32, 32 ), texture );

		GUIStyle labelStyle = new GUIStyle( "label" );

		if( isMouseOver )
		{
			labelStyle.normal.textColor = new Color( 5f / 255f, 14f / 255f, 255f / 255f );
		}
		
		GUI.Label( new Rect( x + 42, y + 6, 200, 18 ), text, labelStyle );
		EditorGUIUtility.AddCursorRect( clickRect, MouseCursor.Link );

		if( Event.current != null && Event.current.type == EventType.MouseDown )
		{
			if( isMouseOver )
			{
				Application.OpenURL( url );
			}
		}
	}

	void LoadTextures()
	{
		if( IconTexture == null )
		{
			IconTexture = (Texture2D)UnityEngine.Resources.Load( "TodoList/IconToDoList", typeof( Texture2D ) );
		}

		if( EmailTexture == null )
		{
			EmailTexture = (Texture2D)UnityEngine.Resources.Load( "TodoList/IconEmail", typeof( Texture2D ) );
		}

		if( FacebookTexture == null )
		{
			FacebookTexture = (Texture2D)UnityEngine.Resources.Load( "TodoList/IconFacebook", typeof( Texture2D ) );
		}

		if( TwitterTexture == null )
		{
			TwitterTexture = (Texture2D)UnityEngine.Resources.Load( "TodoList/IconTwitter", typeof( Texture2D ) );
		}

		if( YouTubeTexture == null )
		{
			YouTubeTexture = (Texture2D)UnityEngine.Resources.Load( "TodoList/IconYoutube", typeof( Texture2D ) );
		}

		if( WebTexture == null )
		{
			WebTexture = (Texture2D)UnityEngine.Resources.Load( "TodoList/IconWeb", typeof( Texture2D ) );
		}
	}
}
