using UnityEngine;
using System.Collections;

/// <summary>
/// This class draws the team selection menu from the beginning of a match
/// </summary>
public class PickTeamGUI : MonoBehaviour
{
	public Font ButtonFont;
	public Texture2D ButtonBackground;

	GUIStyle m_PickButtonStyle;

	void Start()
	{
		if( GamemodeManager.CurrentGamemode.IsUsingTeams() == false )
		{
			ChooseTeam( Team.None );
		}
	}

	void Update()
	{
		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			return;
		}

		if( Input.GetButtonDown( "BlueTeamButton" ) == true )
		{
			ChooseTeam( Team.Blue );
		}

		if( Input.GetButtonDown( "RedTeamButton" ) == true )
		{
			ChooseTeam( Team.Red );
		}

		if( Input.GetButtonDown( "AnyTeamButton" ) == true )
		{
			ChooseAnyTeam();
		}
	}

	void ChooseAnyTeam()
	{
		int numberOfRedTeamPlayers = 0;
		int numberOfBlueTeamPlayer = 0;

		GameObject[] shipObjects = GameObject.FindGameObjectsWithTag( "Ship" );

		for( int i = 0; i < shipObjects.Length; ++i )
		{
			Ship ship = shipObjects[ i ].GetComponent<Ship>();

			if( ship != null )
			{
				if( ship.Team == Team.Red )
				{
					numberOfRedTeamPlayers++;
				}
				else if( ship.Team == Team.Blue )
				{
					numberOfBlueTeamPlayer++;
				}
			}
		}

		if( numberOfRedTeamPlayers > numberOfBlueTeamPlayer )
		{
			ChooseTeam( Team.Blue );
		}
		else
		{
			ChooseTeam( Team.Red );
		}
	}

	void ChooseTeam( Team team )
	{
		GetComponent<PlayerSpawner>().CreateLocalPlayer( team );
		enabled = false;
	}

	void OnGUI()
	{
		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			return;
		}

		LoadStyles();

		GUILayout.BeginArea( new Rect( 10, 10, Screen.width - 20, Screen.height - 20 ) );
		{
			GUILayout.BeginHorizontal();
			{
				if( GUILayout.Button( GetButtonLabel( Team.Blue ), m_PickButtonStyle, GUILayout.Width( Screen.width * 0.5f - 20 ), GUILayout.Height( Screen.height - 140 ) ) )
				{
					ChooseTeam( Team.Blue );
				}

				GUILayout.FlexibleSpace();

				if( GUILayout.Button( GetButtonLabel( Team.Red ), m_PickButtonStyle, GUILayout.Width( Screen.width * 0.5f - 20 ), GUILayout.Height( Screen.height - 140 ) ) )
				{
					ChooseTeam( Team.Red );
				}
			}
			GUILayout.EndHorizontal();

			GUILayout.Space( 20 );

			if( GUILayout.Button( "Choose random team", m_PickButtonStyle, GUILayout.Width( Screen.width - 40 ), GUILayout.Height( 100 ) ) )
			{
				ChooseAnyTeam();
			}
		}
		GUILayout.EndArea();
	}

	string GetButtonLabel( Team team )
	{
		GameObject[] shipObjects = GameObject.FindGameObjectsWithTag( "Ship" );
		int playerCount = 0;

		for( int i = 0; i < shipObjects.Length; ++i )
		{
			if( shipObjects[ i ].GetComponent<Ship>() != null && shipObjects[ i ].GetComponent<Ship>().Team == team )
			{
				playerCount++;
			}
		}

		string label = team.ToString() + " Team\n";
		label += playerCount.ToString();

		if( playerCount == 1 )
		{
			label += " Player";
		}
		else
		{
			label += " Players";
		}

		return label;
	}

	void LoadStyles()
	{
		if( m_PickButtonStyle == null )
		{
			m_PickButtonStyle = new GUIStyle( Styles.Button );
			m_PickButtonStyle.fontSize = 60;
		}
	}
}