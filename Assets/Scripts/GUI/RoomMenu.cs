using UnityEngine;
using System.Collections;

public class RoomMenu : MonoBehaviour 
{
	enum TabCategories
	{
		Matchmaking,
		RoomBrowser,
		CreateRoom,
	}

	TabCategories m_ActiveTab = TabCategories.CreateRoom;

	RoomMenuCreateRoom m_CreateRoomMenu;
	RoomMenuCreateRoom CreateRoomMenu
	{
		get
		{
			return Helper.GetCachedComponent<RoomMenuCreateRoom>( gameObject, ref m_CreateRoomMenu );
		}
	}

	RoomMenuMatchmaking m_MatchmakingMenu;
	RoomMenuMatchmaking MatchmakingMenu
	{
		get
		{
			return Helper.GetCachedComponent<RoomMenuMatchmaking>( gameObject, ref m_MatchmakingMenu );
		}
	}

	RoomMenuRoomBrowser m_RoomBrowserMenu;
	RoomMenuRoomBrowser RoomBrowserMenu
	{
		get
		{
			return Helper.GetCachedComponent<RoomMenuRoomBrowser>( gameObject, ref m_RoomBrowserMenu );
		}
	}

	bool m_IsPopupVisible;
	string m_PopupMessage;

	void OnGUI()
	{
		GUI.enabled = m_IsPopupVisible == false;

		DrawTabButtons();
		DrawActiveTab();

		GUI.enabled = true;

		DrawPopupMessage();
	}

	public void ShowPopupMessage( string message )
	{
		m_IsPopupVisible = true;
		m_PopupMessage = message;
	}

	void DrawPopupMessage()
	{
		if( m_IsPopupVisible == false )
		{
			return;
		}

		float popupWidth = 400;
		float popupHeight = 200;

		GUILayout.BeginArea(
			new Rect( ( Screen.width - popupWidth ) * 0.5f,
					  ( Screen.height - popupHeight ) * 0.5f,
					  popupWidth,
					  popupHeight ),
			"", Styles.Box );
		{
			GUILayout.Space( 20 );

			GUILayout.FlexibleSpace();

			GUILayout.Label( m_PopupMessage, Styles.LabelSmallCentered );

			GUILayout.FlexibleSpace();

			if( GUILayout.Button( "Ok", Styles.Button ) == true )
			{
				m_IsPopupVisible = false;
			}

			GUILayout.Space( 20 );
		}
		GUILayout.EndArea();
	}

	void DrawActiveTab()
	{
		switch( m_ActiveTab )
		{
		case TabCategories.Matchmaking:
			MatchmakingMenu.Draw();
			break;
		case TabCategories.RoomBrowser:
			RoomBrowserMenu.Draw();
			break;
		case TabCategories.CreateRoom:
			CreateRoomMenu.Draw();
			break;
		}
	}

	void DrawTabButtons()
	{
		if( GUI.Button( new Rect( 20, 20, 215, 55 ), 
						"Matchmaking", 
						Styles.GetSelectableButtonStyle( m_ActiveTab == TabCategories.Matchmaking ) ) )
		{
			m_ActiveTab = TabCategories.Matchmaking;
		}

		if( GUI.Button( new Rect( 255, 20, 215, 55 ), 
						"Room Browser", 
						Styles.GetSelectableButtonStyle( m_ActiveTab == TabCategories.RoomBrowser ) ) )
		{
			m_ActiveTab = TabCategories.RoomBrowser;
		}

		if( GUI.Button( new Rect( 490, 20, 215, 55 ), 
						"Create Room", 
						Styles.GetSelectableButtonStyle( m_ActiveTab == TabCategories.CreateRoom ) ) )
		{
			m_ActiveTab = TabCategories.CreateRoom;
		}

		if( GUI.Button( new Rect( 725, 20, 215, 55 ), 
						"Logout", 
						Styles.Button ) )
		{
			ChatHandler.Instance.Client.Disconnect();
			MultiplayerConnector.Instance.Disconnect();
			Application.LoadLevel( "MainMenu" );
		}
	}
}
