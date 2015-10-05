using UnityEngine;
using System.Collections;

/// <summary>
/// This script displays the scoring HUD at the top edge of the screen and also the win messages at the end of a match
/// </summary>
public class GamemodeGUITeamDeathmatch : GamemodeGUIBase
{
	/// <summary>
	/// Scoring HUD Background
	/// </summary>
	public Texture2D GuiTexture;
	/// <summary>
	/// Scoring HUD Team Wings Background
	/// </summary>
	public Texture2D GuiTextureTeamWings;

	GamemodeTeamDeathmatch m_Gamemode;

	void Start()
	{
		m_Gamemode = GamemodeManager.Instance.GetGamemode( Gamemode.TeamDeathmatch ) as GamemodeTeamDeathmatch;
	}

	void Update()
	{
		UpdateLeaderboardsFadeIn();
	}

	void OnGUI()
	{
		if( GamemodeManager.CurrentGamemode.GetGamemodeType() != Gamemode.TeamDeathmatch )
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
		int blueScore = m_Gamemode.GetTeamScore( Team.Blue );
		int redScore = m_Gamemode.GetTeamScore( Team.Red );
		double timePassed = Time.timeSinceLevelLoad;

		GUI.color = Color.white;
		GUI.DrawTexture( new Rect( ( Screen.width - GuiTexture.width ) * 0.5f, 0, GuiTexture.width, GuiTexture.height ), GuiTexture );
		GUI.DrawTexture( new Rect( ( Screen.width - GuiTextureTeamWings.width ) * 0.5f, 0, GuiTextureTeamWings.width, GuiTextureTeamWings.height ), GuiTextureTeamWings );
		GUI.Label( new Rect( Screen.width * 0.5f - 53, 3, 100, 40 ), GetTimeLeftString( timePassed ), LabelStyleCentered );
		GUI.Label( new Rect( Screen.width * 0.5f - 129, 3, 50, 40 ), blueScore.ToString(), LabelStyleCentered );
		GUI.Label( new Rect( Screen.width * 0.5f + 75, 3, 50, 40 ), redScore.ToString(), LabelStyleCentered );
	}

	
}