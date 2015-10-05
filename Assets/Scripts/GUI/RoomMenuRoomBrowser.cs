using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class draws the room list while the player is in the lobby
/// </summary>
public class RoomMenuRoomBrowser : RoomMenuBase 
{
	/// <summary>
	/// This enum represents the different ways the room list can be sorted
	/// </summary>
	enum SortRoomList
	{
		None,
		NameAsc,
		NameDesc,
		PlayersAsc,
		PlayersDesc,
		ModeAsc,
		ModeDesc,
		MapAsc,
		MapDesc,
	}

	Texture2D m_AscArrowTexture;
	/// <summary>
	/// Load the ascending arrow for the sort buttons from the resources folder
	/// </summary>
	/// <value>
	/// The ascending arrow texture.
	/// </value>
	Texture2D AscArrowTexture
	{
		get
		{
			if( m_AscArrowTexture == null )
			{
				m_AscArrowTexture = Resources.Load<Texture2D>( "GUI/SortArrowAsc" );
			}

			return m_AscArrowTexture;
		}
	}

	Texture2D m_DescArrowTexture;
	/// <summary>
	/// Load the descending arrow for the sort buttons from the resources folder
	/// </summary>
	/// <value>
	/// The descending arrow texture.
	/// </value>
	Texture2D DescArrowTexture
	{
		get
		{
			if( m_DescArrowTexture == null )
			{
				m_DescArrowTexture = Resources.Load<Texture2D>( "GUI/SortArrowDesc" );
			}

			return m_DescArrowTexture;
		}
	}

	Texture2D m_NoArrowTexture;
	/// <summary>
	/// Load an empty texture that is as big as the arrow textures for the sort buttons from the resources folder
	/// This is used by the buttons that don't have an arrow so the width is the same with or without an arrow
	/// and the text doesn't jump around when an arrow appears or disappears
	/// </summary>
	/// <value>
	/// The empty texture
	/// </value>
	Texture2D NoArrowTexture
	{
		get
		{
			if( m_NoArrowTexture == null )
			{
				m_NoArrowTexture = Resources.Load<Texture2D>( "GUI/SortArrowNone" );
			}

			return m_NoArrowTexture;
		}
	}

	/// <summary>
	/// Are we currently joining a room?
	/// </summary>
	bool m_IsJoiningRoom = false;

	/// <summary>
	/// The name of the room we are joining
	/// </summary>
	string m_JoinRoomName;

	/// <summary>
	/// The currently selected way to sort the room list
	/// </summary>
	SortRoomList m_SortRoomList = SortRoomList.None;

	/// <summary>
	/// This is just a dummy list of rooms I create to test the room browser while being offline
	/// I just used it to make working on the room browser faster and it is not needed for the final game
	/// I left it in for demonstration purposes
	/// </summary>
	RoomInfo[] m_TestDummyRoomInfos;

	/// <summary>
	/// The width of the name column
	/// </summary>
	const float RoomNameWidth = 300;

	/// <summary>
	/// The width of the player count column
	/// </summary>
	const float RoomPlayerWidth = 140;

	/// <summary>
	/// The width of the gamemode column
	/// </summary>
	const float RoomModeWidth = 200;

	/// <summary>
	/// The width of the map name column
	/// </summary>
	const float RoomMapWidth = 200;

	void Start()
	{
		//If we are not connected we want to create a dummy list of rooms
		//This happens when you press play in the editor while you are in the RoomBrowser scene
		//It just helps to speed up development
		//if( PhotonNetwork.connected == false )
		{
			CreateTestDummyRoomInfos();
		}
	}

	/// <summary>
	/// This function creates 8 dummy rooms with random properties
	/// </summary>
	void CreateTestDummyRoomInfos()
	{
		m_TestDummyRoomInfos = new RoomInfo[ 4 ];
		string[] serverNames = new string[] { "Team Tactical Server", "24/7 Deathmatch", "Wulfram Developers", "Capture The Flag" };

		for( int i = 0; i < m_TestDummyRoomInfos.Length; ++i )
		{
			int mapIndex = Random.Range( 0, 4 );
			string mapQueueString = "BoldanPass#0~TronV2#1~BoldanPass#2~TronV2#";
			
			MapQueueEntry currentMap = MapQueue.GetSingleEntryInMapQueue( mapQueueString, mapIndex );

			ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
			properties.Add( GameProperties.MaxPlayers, (byte)8 );
			properties.Add( GameProperties.PlayerCount, (byte)Random.Range( 0, 8 ) );
			properties.Add( RoomProperty.MapIndex, mapIndex );
			properties.Add( RoomProperty.MapQueue, mapQueueString );
			properties.Add( RoomProperty.Map, currentMap.Name );
			properties.Add( RoomProperty.Mode, (int)currentMap.Mode );

			m_TestDummyRoomInfos[ i ] = new RoomInfo( serverNames[ i ], properties );
		}
	}

	/// <summary>
	/// This gets called by the RoomMenu when the room browser tab is active
	/// </summary>
	public void Draw()
	{
		DrawBackground();
		DrawRoomListHeaderButtons();
		DrawRoomListButtons();
		DrawConnectionState();
	}

	/// <summary>
	/// Draws a feedback popup to tell the player that we are currently joining the room
	/// Depending on the players ping, the connection process takes a little bit, so it is always
	/// helpful to give some kind of feedback. Otherwise the room browser would just do nothing when
	/// the player clicks on a room button and he probably will be confused that nothing is happening
	/// until the game finally connects successfully
	/// </summary>
	void DrawConnectionState()
	{
		if( m_IsJoiningRoom == false )
		{
			return;
		}

		GUI.Label( GetBackgroundRect(), "Joining " + m_JoinRoomName + "...\n" + PhotonNetwork.connectionStateDetailed, Styles.LabelCentered );
	}

	/// <summary>
	/// Draws the room list header buttons.
	/// These are all clickable in order to sort the room list in different ways
	/// </summary>
	void DrawRoomListHeaderButtons()
	{
		float x = 40;
		DrawRoomListHeaderButton( x, RoomNameWidth, "Room name", SortRoomList.NameAsc, SortRoomList.NameDesc );

		x += RoomNameWidth + 10;
		DrawRoomListHeaderButton( x, RoomPlayerWidth, "Players", SortRoomList.PlayersAsc, SortRoomList.PlayersDesc );

		x += RoomPlayerWidth + 10;
		DrawRoomListHeaderButton( x, RoomModeWidth, "Gamemode", SortRoomList.ModeAsc, SortRoomList.ModeDesc );

		x += RoomModeWidth + 10;
		DrawRoomListHeaderButton( x, RoomMapWidth + 10, "Map", SortRoomList.MapAsc, SortRoomList.MapDesc );
	}

	/// <summary>
	/// Draw a header button that toggles specific sort types
	/// </summary>
	void DrawRoomListHeaderButton( float x, float width, string text, SortRoomList ascType, SortRoomList descType )
	{
		if( GUI.Button( new Rect( x, 94, width, 40 ),
						new GUIContent( text, GetArrowTexture( ascType, descType ) ),
						Styles.ButtonServerHeader ) )
		{
			ToggleSortRoomListType( ascType, descType );
		}
	}

	/// <summary>
	/// Toggles the type of the sort room list between ascType and descType
	/// </summary>
	void ToggleSortRoomListType( SortRoomList ascType, SortRoomList descType )
	{
		if( m_SortRoomList == ascType )
		{
			m_SortRoomList = descType;
		}
		else
		{
			m_SortRoomList = ascType;
		}
	}

	/// <summary>
	/// Gets the appropriate arrow texture for the sorting types, depending on which one is active
	/// </summary>
	Texture2D GetArrowTexture( SortRoomList ascType, SortRoomList descType )
	{
		if( m_SortRoomList == ascType )
		{
			return AscArrowTexture;
		}

		if( m_SortRoomList == descType )
		{
			return DescArrowTexture;
		}

		return NoArrowTexture;
	}

	/// <summary>
	/// Sort the room info list with the selected sorting type
	/// </summary>
	void SortRoomInfoList( ref List<RoomInfo> list, SortRoomList sortType )
	{
		switch( sortType )
		{
		case SortRoomList.NameAsc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				return room1.name.CompareTo( room2.name );
			} );
			break;
		case SortRoomList.NameDesc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				return room2.name.CompareTo( room1.name );
			} );
			break;

		case SortRoomList.PlayersAsc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				if( room1.playerCount == room2.playerCount )
				{
					return room1.maxPlayers.CompareTo( room2.maxPlayers );
				}

				return room1.playerCount.CompareTo( room2.playerCount );
			} );
			break;
		case SortRoomList.PlayersDesc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				if( room1.playerCount == room2.playerCount )
				{
					return room2.maxPlayers.CompareTo( room1.maxPlayers );
				}

				return room2.playerCount.CompareTo( room1.playerCount );
			} );
			break;

		case SortRoomList.MapAsc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				string map1 = (string)room1.customProperties[ RoomProperty.Map ];
				string map2 = (string)room2.customProperties[ RoomProperty.Map ];

				return map1.CompareTo( map2 );
			} );
			break;
		case SortRoomList.MapDesc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				string map1 = (string)room1.customProperties[ RoomProperty.Map ];
				string map2 = (string)room2.customProperties[ RoomProperty.Map ];

				return map2.CompareTo( map1 );
			} );
			break;

		case SortRoomList.ModeAsc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				int mode1 = (int)room1.customProperties[ RoomProperty.Mode ];
				int mode2 = (int)room2.customProperties[ RoomProperty.Mode ];

				return mode1.CompareTo( mode2 );
			} );
			break;
		case SortRoomList.ModeDesc:
			list.Sort( delegate( RoomInfo room1, RoomInfo room2 )
			{
				int mode1 = (int)room1.customProperties[ RoomProperty.Mode ];
				int mode2 = (int)room2.customProperties[ RoomProperty.Mode ];

				return mode2.CompareTo( mode1 );
			} );
			break;
		}
	}

	/// <summary>
	/// Draws the big list of buttons for each of the available rooms
	/// </summary>
	void DrawRoomListButtons()
	{
		if( m_IsJoiningRoom == true )
		{
			return;
		}

		List<RoomInfo> roomInfoList = new List<RoomInfo>();

		//Depending on whether we are online or offline, either draw the room list
		//we receive from Photon, or draw our own dummy list
		//if( PhotonNetwork.connected == true )
		{
			//roomInfoList = new List<RoomInfo>( PhotonNetwork.GetRoomList() );
		}
		//else
		{
			roomInfoList = new List<RoomInfo>( m_TestDummyRoomInfos );
		}

		//Sort the list first
		SortRoomInfoList( ref roomInfoList, m_SortRoomList );

		//And then draw all the buttons
		for( int i = 0; i < roomInfoList.Count; ++i )
		{
			DrawRoomListButton( 94 + i * 50 + 50, roomInfoList[ i ] );
		}
	}

	/// <summary>
	/// Draws the room list button.
	/// </summary>
	/// <param name="top">The distance to the top border of the screen</param>
	/// <param name="roomInfo">The room information.</param>
	void DrawRoomListButton( float top, RoomInfo roomInfo )
	{
		//Receive the map and mode data from the rooms custom properties
		string map = (string)roomInfo.customProperties[ RoomProperty.Map ];
		Gamemode mode = (Gamemode)( (int)roomInfo.customProperties[ RoomProperty.Mode ] );

		//Create the rect where the button should be drawn
		Rect buttonRect = new Rect( 40, top, Screen.width - 80, 40 );

		//Is the user hovering the button?
		bool isMouseOver = buttonRect.Contains( Event.current.mousePosition );

		//I first draw an empty button here, because it's contents should be nicely formated
		//If I'd draw the content in this button, I could only type one long text string and
		//it wouldn't look very nice
		//But this is still the button that responds to the player click and joins the room if this happens
		if( GUI.Button( buttonRect, "", Styles.DarkButton ) )
		{
			m_IsJoiningRoom = true;
			m_JoinRoomName = name;

			PhotonNetwork.JoinRoom( name );
		}

		//Now we fake a mouse over color change for our button content
		//since the content isn't actually part of the Unity button, we have to do this ourselves
		if( isMouseOver == true )
		{
			GUI.color = new Color( 1f, 0.8f, 0f );
		}
		else
		{
			GUI.color = Color.white;
		}

		//Create a GUILayout area at the same position we drew the button
		GUILayout.BeginArea( buttonRect );
		{
			//And draw all the content nicely spaced out so that multiple buttons will have the 
			//different texts starting at the same x-position. That'll look way nicer in a big list
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space( 20 );
				GUILayout.Label( roomInfo.name, Styles.LabelSmall, GUILayout.Width( RoomNameWidth ), GUILayout.Height( buttonRect.height ) );
				GUILayout.Label( roomInfo.playerCount + "/" + roomInfo.maxPlayers, Styles.LabelSmall, GUILayout.Width( RoomPlayerWidth ), GUILayout.Height( buttonRect.height ) );
				GUILayout.Label( mode.ToString(), Styles.LabelSmall, GUILayout.Width( RoomModeWidth ), GUILayout.Height( buttonRect.height ) );
				GUILayout.Label( map, Styles.LabelSmall, GUILayout.Width( RoomMapWidth ), GUILayout.Height( buttonRect.height ) );
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();

		GUI.color = Color.white;
	}
}
