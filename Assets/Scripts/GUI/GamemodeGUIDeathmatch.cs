using UnityEngine;
using System.Collections;

/// <summary>
/// This script displays the scoring HUD at the top edge of the screen and also the win messages at the end of a match
/// </summary>
public class GamemodeGUIDeathmatch : GamemodeGUIBase
{
	/// <summary>
	/// Scoring HUD Background
	/// </summary>
	public Texture2D GuiTexture;

	GamemodeDeathmatch m_Gamemode;

	void Start()
	{
		m_Gamemode = GamemodeManager.Instance.GetGamemode( Gamemode.Deathmatch ) as GamemodeDeathmatch;
	}

	void Update()
	{
		UpdateLeaderboardsFadeIn();
	}

	void OnGUI()
	{
		if( GamemodeManager.CurrentGamemode.GetGamemodeType() != Gamemode.Deathmatch )
		{
			return;
		}

		DrawLeaderboards();

		double timePassed = Time.timeSinceLevelLoad;

		if( PhotonNetwork.room != null )
		{
			if( PhotonNetwork.room.customProperties.ContainsKey( RoomProperty.StartTime ) == true )
			{
				timePassed = PhotonNetwork.time - (double)PhotonNetwork.room.customProperties[ RoomProperty.StartTime ];
			}
		}

		if( m_Gamemode.IsRoundFinished() == true )
		{
			DrawEndOfMatchText();
		}
		else
		{
			DrawScoreHUD( timePassed );
		}
	}

	void DrawEndOfMatchText()
	{
		if( PhotonNetwork.isMasterClient == true && m_Gamemode.GetEndRoundTime() > 2f )
		{
			GUI.color = Color.white;
			GUI.Label( new Rect( 0, 0, Screen.width, Screen.height ), "Press any key to restart round.", Styles.LabelSmallCentered );
		}
	}

	void DrawScoreHUD( double timePassed )
	{
		GUI.color = Color.white;
		GUI.DrawTexture( new Rect( ( Screen.width - GuiTexture.width ) * 0.5f, 0, GuiTexture.width, GuiTexture.height ), GuiTexture );
		GUI.Label( new Rect( Screen.width * 0.5f - 53, 3, 100, 40 ), GetTimeLeftString( timePassed ), LabelStyleCentered );
	}
}