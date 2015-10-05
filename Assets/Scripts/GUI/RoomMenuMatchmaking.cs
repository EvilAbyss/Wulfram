using UnityEngine;
using System.Collections;

public class RoomMenuMatchmaking : RoomMenuBase 
{
	Matchmaking m_Matchmaking;
	Matchmaking Matchmaking
	{
		get
		{
			if( m_Matchmaking == null )
			{
				m_Matchmaking = GetComponent<Matchmaking>();
			}

			return m_Matchmaking;
		}
	}

	public void Draw()
	{
		DrawBackground();

		if( Matchmaking.IsMatchmakingStarted() == true )
		{
			GUI.enabled = false;
		}

		GUILayout.BeginArea( GetPaddedBackgroundRect() );
		{
			DrawMatchmakingTypeSelection();
			DrawMatchmakingTypeOptions();
			DrawMatchmakingButton();
		}
		GUILayout.EndArea();

		if( Matchmaking.IsMatchmakingStarted() == true )
		{
			GUI.enabled = true;
			DrawMatchmakingInProgress();
		}
	}

	void DrawMatchmakingInProgress()
	{
		float boxWidth = 500;
		float boxHeight = 200;

		Rect boxRect = new Rect( ( Screen.width - boxWidth ) * 0.5f, ( Screen.height - boxHeight ) * 0.5f, boxWidth, boxHeight );

		GUI.Box( boxRect, "", Styles.DarkBox );

		Rect textRect = GetPaddedBackgroundRect();
		textRect.yMin -= 50;
		textRect.yMax -= 50;

		GUI.Label( textRect, "Searching for matches" + GetAnimatedDots() + "\n" + Matchmaking.GetMatchmakingInfo(), Styles.LabelCentered );

		float buttonWidth = 300;
		float buttonHeight = 50;

		Rect buttonRect = new Rect( ( Screen.width - buttonWidth ) * 0.5f, Screen.height * 0.5f + 30, buttonWidth, buttonHeight );
		
		if( GUI.Button( buttonRect, "Cancel", Styles.DarkButton ) )
		{
			Matchmaking.CancelMatchmaking();
		}
	}

	string GetAnimatedDots()
	{
		int numberOfDots = (int)( Time.realtimeSinceStartup * 2 ) % 4;

		string dots = "";

		for( int i = 0; i < numberOfDots; ++i )
		{
			dots += ".";
		}

		return dots;
	}

	void DrawMatchmakingTypeSelection()
	{
		GUILayout.Label( "Matchmaking Type", Styles.Header );

		GUILayout.BeginHorizontal();
		{
			if( GUILayout.Button( "Random",
								  Styles.GetSelectableDarkButtonStyle( Matchmaking.SelectedMatchmakingType == MatchmakingType.Random ),
								  GUILayout.Height( 40 ) ) )
			{
				Matchmaking.SelectedMatchmakingType = MatchmakingType.Random;
			}

			GUILayout.Space( 10 );

			if( GUILayout.Button( "Room Properties Based",
								  Styles.GetSelectableDarkButtonStyle( Matchmaking.SelectedMatchmakingType == MatchmakingType.RoomProperties ),
								  GUILayout.Height( 40 ) ) )
			{
				Matchmaking.SelectedMatchmakingType = MatchmakingType.RoomProperties;
			}

			GUILayout.Space( 10 );

			if( GUILayout.Button( "SQL Based",
								  Styles.GetSelectableDarkButtonStyle( Matchmaking.SelectedMatchmakingType == MatchmakingType.Sql ),
								  GUILayout.Height( 40 ) ) )
			{
				Matchmaking.SelectedMatchmakingType = MatchmakingType.Sql;
			}
		}
		GUILayout.EndHorizontal();

		GUILayout.Space( 20 );
	}

	void DrawMatchmakingTypeOptions()
	{
		switch( Matchmaking.SelectedMatchmakingType )
		{
		case MatchmakingType.RoomProperties:
			DrawRoomPropertiesMatchmakingOptions();
			break;
		case MatchmakingType.Sql:
			DrawSqlMatchmakingOptions();
			break;
		case MatchmakingType.Random:
			DrawRandomMatchmakingOptions();
			break;
		}
	}

	void DrawRandomMatchmakingOptions()
	{
		GUILayout.FlexibleSpace();
	}

	void DrawRoomPropertiesMatchmakingOptions()
	{
		GUILayout.Label( "Matchmaking Options", Styles.Header );

		GUILayout.BeginHorizontal();
		{
			GUILayout.BeginVertical( Styles.DarkBox, GUILayout.Width( 430 ) );
			{
				DrawMapOptions();
			}
			GUILayout.EndVertical();

			GUILayout.Space( 10 );

			GUILayout.BeginVertical( Styles.DarkBox, GUILayout.Width( 430 ) );
			{
				DrawModeOptions();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();

		GUILayout.FlexibleSpace();
	}

	void DrawSqlMatchmakingOptions()
	{
		GUILayout.Label( "Matchmaking Options", Styles.Header );

		GUILayout.BeginHorizontal();
		{
			GUILayout.BeginVertical( Styles.DarkBox, GUILayout.Width( 430 ) );
			{
				DrawMapOptions();
			}
			GUILayout.EndVertical();

			GUILayout.Space( 10 );

			GUILayout.BeginVertical( Styles.DarkBox, GUILayout.Width( 430 ) );
			{
				DrawModeOptions();
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();

		DrawPlayerSkillOptions();

		GUILayout.FlexibleSpace();
	}

	void DrawPlayerSkillOptions()
	{
		GUILayout.Space( 10 );

		GUILayout.BeginHorizontal();
		{
			GUILayout.Label( "Player Skill Level: " + Matchmaking.GetPlayerSkill(), Styles.LabelSmall, GUILayout.Width( 200 ) );

			if( GUILayout.Button( "+", Styles.DarkButtonActive, GUILayout.Width( 30 ) ) )
			{
				Matchmaking.SetPlayerSkill( Matchmaking.GetPlayerSkill() + 1 );
			}

			if( GUILayout.Button( "-", Styles.DarkButtonActive, GUILayout.Width( 30 ) ) )
			{
				Matchmaking.SetPlayerSkill( Matchmaking.GetPlayerSkill() - 1 );
			}
		}
		GUILayout.EndHorizontal();
	}

	void DrawMapOptions()
	{
		GUILayout.Label( "Maps", Styles.LabelSmall );

		for( int i = 0; i < ServerOptions.AvailableMaps.Length; ++i )
		{
			bool value = GUILayout.Toggle( 
				Matchmaking.IsMapSelected( ServerOptions.AvailableMaps[ i ] ), 
				ServerOptions.AvailableMaps[ i ],
				Styles.Toggle );

			Matchmaking.SetMapSelection( ServerOptions.AvailableMaps[ i ], value );
		}
	}

	void DrawModeOptions()
	{
		GUILayout.Label( "Modes", Styles.LabelSmall );

		for( int i = 0; i < ServerOptions.AvailableModes.Length; ++i )
		{
			bool value = GUILayout.Toggle(
				Matchmaking.IsModeSelected( ServerOptions.AvailableModes[ i ] ),
				ServerOptions.AvailableModes[ i ].ToString(),
				Styles.Toggle );

			Matchmaking.SetModeSelection( ServerOptions.AvailableModes[ i ], value );
		}
	}

	void DrawMatchmakingButton()
	{
		if( GUILayout.Button( "Start Matchmaking", Styles.DarkButton, GUILayout.Height( 90 ) ) == true )
		{
			Matchmaking.StartMatchmaking();
		}
	}
}
