using UnityEngine;
using System.Collections;

public class ServerOptions 
{
	public static string[] AvailableMaps = new string[] 
	{ 
		"TronV2", 
		"BoldanPass" 
	};

	public static Gamemode[] AvailableModes = new Gamemode[] 
	{ 
		Gamemode.CaptureTheFlag, 
		Gamemode.Deathmatch,
		Gamemode.TeamDeathmatch
	};

	public static void CreateRoom( string name, string mapQueueString, int skillLevel = 5 )
	{
		MapQueueEntry firstMap = MapQueue.GetSingleEntryInMapQueue( mapQueueString, 0 );

		RoomOptions roomOptions = new RoomOptions();
		roomOptions.maxPlayers = 8;

		roomOptions.customRoomProperties = new ExitGames.Client.Photon.Hashtable();
		roomOptions.customRoomProperties.Add( RoomProperty.MapQueue, mapQueueString );
		roomOptions.customRoomProperties.Add( RoomProperty.MapIndex, 0 );
		roomOptions.customRoomProperties.Add( RoomProperty.RedScore, 0 );
		roomOptions.customRoomProperties.Add( RoomProperty.BlueScore, 0 );
		roomOptions.customRoomProperties.Add( RoomProperty.Map, firstMap.Name );
		roomOptions.customRoomProperties.Add( RoomProperty.Mode, (int)firstMap.Mode );
		roomOptions.customRoomProperties.Add( RoomProperty.SkillLevel, skillLevel );

		roomOptions.customRoomPropertiesForLobby = new string[] {
			RoomProperty.Map,
			RoomProperty.Mode,
			RoomProperty.SkillLevel,
		};

		PhotonNetwork.JoinOrCreateRoom( name, roomOptions, MultiplayerConnector.Lobby );
	}
}
