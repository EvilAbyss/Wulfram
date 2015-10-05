using UnityEngine;
using System.Collections;

/// <summary>
/// This script displays the scoring HUD at the top edge of the screen and also the win messages at the end of a match
/// </summary>
public class GamemodeGUICaptureTheFlag : GamemodeGUIBase
{
	/// <summary>
	/// Scoring HUD Background
	/// </summary>
	public Texture2D GuiTexture;
	/// <summary>
	/// Scoring HUD Team Wings Background
	/// </summary>
	public Texture2D GuiTextureTeamWings;
	/// <summary>
	/// Icon of the flag
	/// </summary>
	public Texture2D FlagIcon;

	GamemodeCaptureTheFlag m_Gamemode;

	void Start()
	{
		m_Gamemode = GamemodeManager.Instance.GetGamemode( Gamemode.CaptureTheFlag ) as GamemodeCaptureTheFlag;
	}

	void Update()
	{
		UpdateLeaderboardsFadeIn();
	}

	void OnGUI()
	{
		if( GamemodeManager.CurrentGamemode.GetGamemodeType() != Gamemode.CaptureTheFlag )
		{
			return;
		}

		DrawLeaderboards();

		if( m_Gamemode.IsRoundFinished() == true )
		{
			DrawEndOfMatchText();
		}
		else
		{
			DrawScoreHUD();
		}
	}

	void DrawEndOfMatchText()
	{
		Team winningTeam = m_Gamemode.GetWinningTeam();

		if( winningTeam == Team.Blue )
		{
			GUI.color = Color.blue;
			GUI.Label( new Rect( 0, 0, Screen.width, 200 ), "Blue Team won!", Styles.EndMatchLabel );
		}
		else if( winningTeam == Team.Red )
		{
			GUI.color = Color.red;
			GUI.Label( new Rect( 0, 0, Screen.width, 200 ), "Red Team won!", Styles.EndMatchLabel );
		}
		else
		{
			GUI.color = Color.white;
			GUI.Label( new Rect( 0, 0, Screen.width, 200 ), "Both Teams are tied!", Styles.EndMatchLabel );
		}

		GUI.color = Color.white;
		GUI.Label( new Rect( 0, Screen.height * 0.5f + 140, Screen.width, 50 ), "Next Map: " + MapQueue.GetNextMap(), Styles.LabelSmallCentered );

		if( PhotonNetwork.isMasterClient == true && m_Gamemode.GetEndRoundTime() > 2f )
		{
			float blinkAlpha = (int)( m_Gamemode.GetEndRoundTime() * 2 ) % 2;

			GUI.color = new Color( 1f, 1f, 1f, blinkAlpha );
			GUI.Label( new Rect( 0, 0, Screen.width, Screen.height ), "Press any key to load the next map.", LabelStyleCentered );
		}
	}

	void DrawScoreHUD()
	{
		int blueScore = 0;
		int redScore = 0;
		double timePassed = Time.timeSinceLevelLoad;

		if( PhotonNetwork.room != null )
		{
			//Receive the room properties which store all the data about the current match state

			if( PhotonNetwork.room.customProperties.ContainsKey( RoomProperty.BlueScore ) == true )
			{
				blueScore = (int)PhotonNetwork.room.customProperties[ RoomProperty.BlueScore ];
			}

			if( PhotonNetwork.room.customProperties.ContainsKey( RoomProperty.RedScore ) == true )
			{
				redScore = (int)PhotonNetwork.room.customProperties[ RoomProperty.RedScore ];
			}

			if( PhotonNetwork.room.customProperties.ContainsKey( RoomProperty.StartTime ) == true )
			{
				timePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[ RoomProperty.StartTime ];
			}
		}

		GUI.color = Color.white;
		GUI.DrawTexture( new Rect( ( Screen.width - GuiTexture.width ) * 0.5f, 0, GuiTexture.width, GuiTexture.height ), GuiTexture );
		GUI.DrawTexture( new Rect( ( Screen.width - GuiTextureTeamWings.width ) * 0.5f, 0, GuiTextureTeamWings.width, GuiTextureTeamWings.height ), GuiTextureTeamWings );
		GUI.Label( new Rect( Screen.width * 0.5f - 53, 3, 100, 40 ), GetTimeLeftString( timePassed ), LabelStyleCentered );
		GUI.Label( new Rect( Screen.width * 0.5f - 129, 3, 50, 40 ), blueScore.ToString(), LabelStyleCentered );
		GUI.Label( new Rect( Screen.width * 0.5f + 75, 3, 50, 40 ), redScore.ToString(), LabelStyleCentered );

		if( PickupFlag.BlueFlag != null && PickupFlag.BlueFlag.IsHome() == true )
		{
			GUI.color = Color.blue;
			GUI.DrawTexture( new Rect( Screen.width * 0.5f - 78, 4, FlagIcon.width, FlagIcon.height ), FlagIcon );
		}

		if( PickupFlag.RedFlag != null && PickupFlag.RedFlag.IsHome() == true )
		{
			GUI.color = Color.red;
			GUI.DrawTexture( new Rect( Screen.width * 0.5f + 43, 4, FlagIcon.width, FlagIcon.height ), FlagIcon );
		}
	}
}