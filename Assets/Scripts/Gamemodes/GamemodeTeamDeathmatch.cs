using UnityEngine;
using System.Collections;

public class GamemodeTeamDeathmatch : GamemodeBase 
{
	/// <summary>
	/// How long is one match?
	/// </summary>
	public const float TotalRoundTime = 5 * 60;

	/// <summary>
	/// The spawn point for the red team
	/// </summary>
	public Transform RedSpawnPoint;

	/// <summary>
	/// The spawn point for the blue team
	/// </summary>
	public Transform BlueSpawnPoint;

	public override void OnSetup()
	{
		if( PhotonNetwork.isMasterClient == true )
		{
			SetRoundStartTime();
		}
	}

	public override void OnTearDown()
	{

	}

	public override Gamemode GetGamemodeType()
	{
		return Gamemode.TeamDeathmatch;
	}

	public override bool IsRoundFinished()
	{
		double timePassed = Time.timeSinceLevelLoad;

		if( PhotonNetwork.room != null )
		{
			if( PhotonNetwork.room.customProperties.ContainsKey( RoomProperty.StartTime ) == true )
			{
				timePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[ RoomProperty.StartTime ];
			}
		}

		return timePassed >= TotalRoundTime;
	}

	public int GetTeamScore( Team team )
	{
		GameObject[] shipObjects = GameObject.FindGameObjectsWithTag( "Ship" );
		int totalScore = 0;
		for( int i = 0; i < shipObjects.Length; ++i )
		{
			Ship ship = shipObjects[ i ].GetComponent<Ship>();

			if( ship.Team == team )
			{
				totalScore += ship.KillCount;
			}
		}

		return totalScore;
	}

	public Team GetWinningTeam()
	{
		int blueScore = GetTeamScore( Team.Blue );
		int redScore = GetTeamScore( Team.Red );

		if( blueScore > redScore )
		{
			return Team.Blue;
		}
		else if( redScore > blueScore )
		{
			return Team.Red;
		}

		return Team.None;
	}

	public override bool IsUsingTeams()
	{
		return true;
	}

	public override Transform GetSpawnPoint( Team team )
	{
		if( team == Team.Blue )
		{
			return BlueSpawnPoint;
		}

		return RedSpawnPoint;
	}
}
