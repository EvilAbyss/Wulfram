using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum MatchmakingType
{
	Random,
	RoomProperties,
	Sql,
}

public class Matchmaking : MonoBehaviour 
{
	public MatchmakingType SelectedMatchmakingType = MatchmakingType.Sql;

	Dictionary<string, bool> m_MapSelection = new Dictionary<string, bool>();
	Dictionary<Gamemode, bool> m_ModeSelection = new Dictionary<Gamemode, bool>();

	bool m_IsMatchmakingStarted = false;
	int m_PlayerSkill = 5;
	int m_JoinAttempt = 0;

	List<MapQueueEntry> m_MatchmakingMapQueue = new List<MapQueueEntry>();

	const int MaximumSkillDeviationInSqlMatchmaking = 3;

	void Awake()
	{
		for( int i = 0; i < ServerOptions.AvailableMaps.Length; ++i )
		{
			m_MapSelection.Add( ServerOptions.AvailableMaps[ i ], true );
		}

		for( int i = 0; i < ServerOptions.AvailableModes.Length; ++i )
		{
			m_ModeSelection.Add( ServerOptions.AvailableModes[ i ], true );
		}
	}

	public void SetPlayerSkill( int newSkill )
	{
		m_PlayerSkill = Mathf.Clamp( newSkill, 1, 10 );
	}

	public int GetPlayerSkill()
	{
		return m_PlayerSkill;
	}

	public bool IsMapSelected( string map )
	{
		if( m_MapSelection.ContainsKey( map ) == false )
		{
			return false;
		}

		return m_MapSelection[ map ];
	}

	public bool IsModeSelected( Gamemode mode )
	{
		if( m_ModeSelection.ContainsKey( mode ) == false )
		{
			return false;
		}

		return m_ModeSelection[ mode ];
	}

	public void SetMapSelection( string map, bool value )
	{
		if( m_MapSelection.ContainsKey( map ) == false )
		{
			return;
		}

		m_MapSelection[ map ] = value;
	}

	public void SetModeSelection( Gamemode mode, bool value )
	{
		if( m_ModeSelection.ContainsKey( mode ) == false )
		{
			return;
		}

		m_ModeSelection[ mode ] = value;
	}

	public bool IsMatchmakingStarted()
	{
		return m_IsMatchmakingStarted;
	}

	public string GetMatchmakingInfo()
	{
		switch( SelectedMatchmakingType )
		{
		case MatchmakingType.Random:
			return "";
		case MatchmakingType.RoomProperties:
			return "Join attempt " + m_JoinAttempt + "/" + m_MatchmakingMapQueue.Count;
		case MatchmakingType.Sql:
			return "Join attempt " + m_JoinAttempt;
		}

		return "";
	}

	public void CancelMatchmaking()
	{
		m_IsMatchmakingStarted = false;
		StopAllCoroutines();
	}

	void OnPhotonRandomJoinFailed()
	{
		if( SelectedMatchmakingType == MatchmakingType.Random )
		{
			CreateRandomMatchmakingServer();
		}
		else if( SelectedMatchmakingType == MatchmakingType.RoomProperties )
		{
			Invoke( "MakeRoomPropertiesMatchmakingJoinAttempt", 1f );
		}
		else if( SelectedMatchmakingType == MatchmakingType.Sql )
		{
			Invoke( "MakeSqlMatchmakingJoinAttempt", 1f );
		}
	}

	public void StartMatchmaking()
	{
		if( IsMatchmakingStarted() == true )
		{
			return;
		}

		m_IsMatchmakingStarted = true;
		m_JoinAttempt = 0;
		m_MatchmakingMapQueue = CreateRoomPropertiesMapQueue();

		switch( SelectedMatchmakingType )
		{
		case MatchmakingType.Random:
			DoRandomMatchmaking();
			break;
		case MatchmakingType.RoomProperties:
			MakeRoomPropertiesMatchmakingJoinAttempt();
			break;
		case MatchmakingType.Sql:
			MakeSqlMatchmakingJoinAttempt();
			break;
		}
	}

	#region Random Matchmaking
	void DoRandomMatchmaking()
	{
		PhotonNetwork.JoinRandomRoom();
	}

	void CreateRandomMatchmakingServer()
	{
		string serverName = ChatHandler.Instance.ChatUsername + "'s Server";
		string mapQueueString = "BoldanPass#0~TronV2#1~BoldanPass#2~TronV2#0~BoldanPass#1~TronV2#2";

		ServerOptions.CreateRoom( serverName, mapQueueString );
	}
	#endregion

	#region Room Properties Matchmaking
	void MakeRoomPropertiesMatchmakingJoinAttempt()
	{
		if( m_JoinAttempt < m_MatchmakingMapQueue.Count )
		{
			MapQueueEntry searchForMap = m_MatchmakingMapQueue[ m_JoinAttempt ];

			ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable();
			expectedProperties.Add( RoomProperty.Map, searchForMap.Name );
			expectedProperties.Add( RoomProperty.Mode, searchForMap.Mode );

			PhotonNetwork.JoinRandomRoom( expectedProperties, 0 );
			 
			m_JoinAttempt++;
		}
		else
		{
			CreateRoomPropertiesMatchmakingServer();
		}
	}

	/// <summary>
	/// This methods takes the users map and mode selections and builds
	/// a list which each possible map/mode pair, this can then be used
	/// to start matchmaking searches for the different combinations
	/// </summary>
	/// <returns></returns>
	List<MapQueueEntry> CreateRoomPropertiesMapQueue()
	{
		List<MapQueueEntry> mapQueue = new List<MapQueueEntry>();

		//Iterate over all available maps
		foreach( KeyValuePair<string, bool> mapPair in m_MapSelection )
		{
			//and iterate over all available gamemodes
			foreach( KeyValuePair<Gamemode, bool> modePair in m_ModeSelection )
			{
				//If both the map and the mode are selected to be included in matchmaking
				if( mapPair.Value == true &&
					modePair.Value == true )
				{
					//add this pair to the list
					mapQueue.Add( new MapQueueEntry
					{
						Name = mapPair.Key,
						Mode = modePair.Key,
					} );
				}
			}
		}

		return mapQueue;
	}

	/// <summary>
	/// If no suitable room was found through room properties matchmaking, we just create a new one
	/// </summary>
	void CreateRoomPropertiesMatchmakingServer()
	{
		string serverName = ChatHandler.Instance.ChatUsername + "'s Server";

		ServerOptions.CreateRoom( serverName, MapQueue.ListToString( m_MatchmakingMapQueue ) );
	}
	#endregion


	#region Sql Matchmaking Demonstration
	void SqlSearchStringDemonstration()
	{
		TypedLobby newLobby = new TypedLobby( "SkyArenaLobby", LobbyType.SqlLobby );



		string possibleSqlSearch = "( C5 = \"Map2\" OR C5 = \"Map5\" ) AND C2 < 20 AND C2 > 10";



		PhotonNetwork.JoinRandomRoom(
			null,
			0,
			MatchmakingMode.FillRoom,
			newLobby,
			possibleSqlSearch
		);
	}











	void SetSqlRoomPropertiesDemonstration()
	{
		ExitGames.Client.Photon.Hashtable newSqlProperties = new ExitGames.Client.Photon.Hashtable();

		//We use C0 as the current map name
		newSqlProperties.Add( "C0", "BoldanPass" );

		//I use RoomProperty.Map for this, but this is also defined as C0
		newSqlProperties.Add( RoomProperty.Map, "BoldanPass" ); //same meaning as the above line

		//C1 is used for the currently played gamemode
		//RoomProperty.Mode = "C1"
		newSqlProperties.Add( RoomProperty.Mode, (int)Gamemode.Deathmatch );

		//C2 is used for an arbitrarily selected skill level the player has
		//RoomProperty.SkillLevel = "C2"
		newSqlProperties.Add( RoomProperty.SkillLevel, 5 );

		string[] lobbyProperties = new string[] { "C0", "C1", "C2" };

		PhotonNetwork.CreateRoom( "My Sql Room", true, true, 8, newSqlProperties, lobbyProperties );
	}




	void Bla()
	{










		//Possible variables to help you determine what matchmaking algorithm fits best

		int countOfPlayers = PhotonNetwork.countOfPlayers;

		int countOfPlayersInRooms = PhotonNetwork.countOfPlayersInRooms;

		int countOfPlayersOnMaster = PhotonNetwork.countOfPlayersOnMaster;

		int countOfRooms = PhotonNetwork.countOfRooms;
























	}
	#endregion




	#region Sql Matchmaking

	string CreateSqlSearchString()
	{
		string possibleSqlSearchResult = "( C0 = \"TronV2\" OR C0 = \"BoldanPass\" ) AND "
									   + "( C1 = 0 OR C1 = 1 OR C1 = 2 ) AND "
									   + "C2 > 3 AND C2 < 7";

		//Create all the search segments used to look for maps
		List<string> possibleMaps = new List<string>();
		foreach( KeyValuePair<string, bool> mapPair in m_MapSelection )
			if( mapPair.Value == true )
				possibleMaps.Add( RoomProperty.Map + " = \"" + mapPair.Key + "\"" );

		//Create all the search segments used to look for modes
		List<string> possibleModes = new List<string>();
		foreach( KeyValuePair<Gamemode, bool> modePair in m_ModeSelection )
			if( modePair.Value == true )
				possibleModes.Add( RoomProperty.Mode + " = " + (int)modePair.Key );

		int skillDeviation = m_JoinAttempt + 1;

		return
			"( " + string.Join( " OR ", possibleMaps.ToArray() ) + " ) AND "
			+ "( " + string.Join( " OR ", possibleModes.ToArray() ) + " ) AND "
			+ RoomProperty.SkillLevel + " > " + ( m_PlayerSkill - skillDeviation ) + " AND "
			+ RoomProperty.SkillLevel + " < " + ( m_PlayerSkill + skillDeviation );
	}

	void MakeSqlMatchmakingJoinAttempt()
	{
		if( m_JoinAttempt < MaximumSkillDeviationInSqlMatchmaking )
		{
			string sqlLobbyFilter = CreateSqlSearchString();

			PhotonNetwork.JoinRandomRoom( 
				null, 
				8, 
				MatchmakingMode.FillRoom, 
				MultiplayerConnector.Lobby,
				sqlLobbyFilter );

			m_JoinAttempt++;
		}
		else
		{
			CreateSqlMatchmakingServer();
		}
	}

	void CreateSqlMatchmakingServer()
	{
		string serverName = ChatHandler.Instance.ChatUsername + "'s Server";

		ServerOptions.CreateRoom( serverName, MapQueue.ListToString( m_MatchmakingMapQueue ), m_PlayerSkill );
	}
	#endregion
}
