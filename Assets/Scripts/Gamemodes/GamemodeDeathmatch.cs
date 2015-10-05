using UnityEngine;
using System.Collections;

public class GamemodeDeathmatch : GamemodeBase 
{
	/// <summary>
	/// How long is one match?
	/// </summary>
	public const float TotalRoundTime = 5 * 60;

	/// <summary>
	/// The spawn points
	/// </summary>
	public Transform[] SpawnPoints;

	/// <summary>
	/// Called at the beginning of a map when this mode is active
	/// </summary>
	public override void OnSetup()
	{
		//The master client stores its round start time, so that all clients can calculate
		//themselves when the current round ends
		if( PhotonNetwork.isMasterClient == true )
		{
			SetRoundStartTime();
		}
	}

	/// <summary>
	/// We don't need to clean up anything when deathmatch is not being played
	/// </summary>
	public override void OnTearDown()
	{

	}

	/// <summary>
	/// What game mode are we?
	/// </summary>
	public override Gamemode GetGamemodeType()
	{
		return Gamemode.Deathmatch;
	}

	/// <summary>
	/// Determines whether the current round is finished
	/// Deathmatch only finishes after the time is over
	/// </summary>
	/// <returns>Is the round finished?</returns>
	public override bool IsRoundFinished()
	{
		double timePassed = Time.timeSinceLevelLoad;

		if( PhotonNetwork.room != null )
		{
			if( PhotonNetwork.room.customProperties.ContainsKey( RoomProperty.StartTime ) == true )
			{
				//PhotonNetwork.time is synchronized between all players, so we can be sure that each client
				//gets the same result here
				timePassed = PhotonNetwork.time
						   - (double)PhotonNetwork.room.customProperties[ RoomProperty.StartTime ];
			}
		}

		return timePassed >= TotalRoundTime;
	}

	/// <summary>
	/// Deathmatch is not using teams
	/// </summary>
	public override bool IsUsingTeams()
	{
		return false;
	}

	/// <summary>
	/// Gets a random spawnpoint on the map
	/// The list of available spawnpoints is set in editor mode
	/// </summary>
	/// <param name="team">The team is irrelevant here since deathmatch is not using team. We simply ignore it</param>
	/// <returns></returns>
	public override Transform GetSpawnPoint( Team team )
	{
		return SpawnPoints[ Random.Range( 0, SpawnPoints.Length ) ];
	}
}
