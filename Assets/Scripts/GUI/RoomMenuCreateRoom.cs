using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RoomMenuCreateRoom : RoomMenuBase 
{
	string m_ServerName = "";
	Gamemode m_SelectedMode = Gamemode.CaptureTheFlag;
	string m_SelectedMap = "TronV2";

	List<MapQueueEntry> m_MapQueue = new List<MapQueueEntry>();

	void Start()
	{
		if( ChatHandler.Instance != null )
		{
			m_ServerName = ChatHandler.Instance.ChatUsername + "'s Server";
		}

		AddTestMapsToQueue();
	}

	void AddTestMapsToQueue()
	{
		AddMapToQueue( "TronV2", Gamemode.CaptureTheFlag );
		AddMapToQueue( "BoldanPass", Gamemode.TeamDeathmatch );
		AddMapToQueue( "TronV2", Gamemode.Deathmatch );
		AddMapToQueue( "BoldanPass", Gamemode.CaptureTheFlag );
		AddMapToQueue( "TronV2", Gamemode.TeamDeathmatch );
		AddMapToQueue( "BoldanPass", Gamemode.Deathmatch );
	}

	public void Draw()
	{
		DrawBackground();

		DrawServerNameTextField();
		DrawMapQueue();
		DrawMapInput();
		DrawCreateRoomButton();
	}

	void DrawServerNameTextField()
	{
		GUI.Label( new Rect( 40, 100, 500, 40 ), "Server Name", Styles.LabelSmall );

		m_ServerName = GUI.TextField( new Rect( 200, 100, 720, 40 ), m_ServerName, Styles.TextField );
	}

	void DrawMapQueue()
	{
		GUI.Box( new Rect( 490, 160, 430, Screen.height - 260 ), "", Styles.Box );

		GUI.Label( new Rect( 510, 180, 500, 40 ), "Map Queue", Styles.Header );

		DrawMapQueueList();
	}

	string GetGamemodeShortform( Gamemode mode )
	{
		switch( mode )
		{
		case Gamemode.Deathmatch:
			return "DM";
		case Gamemode.CaptureTheFlag:
			return "CTF";
		case Gamemode.TeamDeathmatch:
			return "TDM";
		}

		return "N/A";
	}

	void DrawMapQueueList()
	{
		GUILayout.BeginArea( new Rect( 510, 220, 390, Screen.height - 340 ) );
		{
			for( int i = 0; i < m_MapQueue.Count; ++i )
			{
				MapQueueEntry entry = m_MapQueue[ i ];

				GUILayout.BeginHorizontal();
				{
					GUILayout.Label( entry.Name + " [" + GetGamemodeShortform( entry.Mode ) + "]", Styles.LabelSmall );
					GUILayout.FlexibleSpace();

					if( GUILayout.Button( "X", Styles.DarkButtonActive, GUILayout.Width( 60 ) ) == true )
					{
						m_MapQueue.RemoveAt( i );
					}
				}
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();
	}

	void DrawMapInput()
	{
		GUI.Box( new Rect( 40, 160, 430, Screen.height - 260 ), "", Styles.Box );

		GUILayout.BeginArea( new Rect( 60, 180, 390, Screen.height - 300 ) );
		{
			GUILayout.Label( "Add map to queue", Styles.Header );
			GUILayout.Space( 5 );

			DrawSelectMapArea();
			DrawSelectModeArea();

			GUILayout.FlexibleSpace();

			if( GUILayout.Button( "Add map to queue", Styles.DarkButton, GUILayout.Height( 40 ) ) == true )
			{
				AddMapToQueue( m_SelectedMap, m_SelectedMode );
			}
		}
		GUILayout.EndArea();
	}

	void AddMapToQueue( string map, Gamemode mode )
	{
		MapQueueEntry newEntry = new MapQueueEntry
		{
			Name = map,
			Mode = mode,
		};

		m_MapQueue.Add( newEntry );
	}

	void DrawSelectMapArea()
	{
		GUILayout.Label( "Selected Map: " + m_SelectedMap, Styles.LabelSmall );

		for( int i = 0; i < ServerOptions.AvailableMaps.Length; ++i )
		{
			bool isSelected = m_SelectedMap == ServerOptions.AvailableMaps[ i ];
			bool newValue = GUILayout.Toggle( isSelected, ServerOptions.AvailableMaps[ i ], Styles.Toggle );

			if( newValue != isSelected )
			{
				m_SelectedMap = ServerOptions.AvailableMaps[ i ];
			}
		}
	}

	void DrawSelectModeArea()
	{
		GUILayout.Space( 5 );
		GUILayout.Label( "Selected Gamemode: " + m_SelectedMode, Styles.LabelSmall );

		for( int i = 0; i < ServerOptions.AvailableModes.Length; ++i )
		{
			bool isSelected = m_SelectedMode == ServerOptions.AvailableModes[ i ];
			bool newValue = GUILayout.Toggle( isSelected, ServerOptions.AvailableModes[ i ].ToString(), Styles.Toggle );

			if( newValue != isSelected )
			{
				m_SelectedMode = ServerOptions.AvailableModes[ i ];
			}
		}
	}

	void DrawCreateRoomButton()
	{
		if( GUI.Button( new Rect( 40, Screen.height - 80, Screen.width - 80, 40 ), "Create Room", Styles.DarkButton ) )
		{
			if( m_ServerName == "" )
			{
				RoomMenu.ShowPopupMessage( "Please enter a name" );
			}
			else if( m_MapQueue.Count == 0 )
			{
				RoomMenu.ShowPopupMessage( "Please add a map to the map queue" );
			}
			else
			{
				ServerOptions.CreateRoom( m_ServerName, MapQueue.ListToString( m_MapQueue ) );
			}
		}
	}
}
