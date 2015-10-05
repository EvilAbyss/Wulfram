using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// This class draws everything that is related to the chat.
/// </summary>
public class ChatGUI : ShipBase
{
    /// <summary>
    /// Is the chat input open?
    /// </summary>
    bool m_IsOpen;

    /// <summary>
    /// Is the friend list visible
    /// </summary>
    bool m_ShowFriendList;

    /// <summary>
    /// Are we currently sending a private message?
    /// </summary>
    bool m_SendPrivateMessage;

    /// <summary>
    /// The text that the player is currently typing
    /// </summary>
    string m_TextInput = "";

    /// <summary>
    /// The target channel where this text should be sent
    /// </summary>
    string m_SendToChannel;

    /// <summary>
    /// Used by the friendlist, this is the name that is currently typed in the "Add friend" field
    /// </summary>
    string m_AddFriendName = "";

    ChatHandler m_ChatHandler;
    /// <summary>
    /// A reference to the ChatHandler component
    /// </summary>
    ChatHandler ChatHandler
    {
        get
        {
            return Helper.GetCachedComponent<ChatHandler>( gameObject, ref m_ChatHandler );
        }
    }

    void Update()
    {
	
		//uplink code maybe? -Justin
        //Whenever menus are visible, we want to show the cursor
		if (m_TextInput.StartsWith ("/")) { //user is using a command
			
			if (m_TextInput.Contains ("kill")) { //user is using kill command }
				if( PhotonView.isMine == true )
				{
					Invoke( "SendRespawn", 2f );
				}
			}
		}

        bool showCuror = m_IsOpen == true || m_ShowFriendList == true || Ship.LocalPlayer == null;

        Cursor.visible = showCuror;
        Screen.lockCursor = !showCuror;
    }

    /// <summary>
    /// This function prepares and starts the typing process to channels or users
    /// </summary>
    /// <param name="sendToChannel">The channel/user this message should go to.</param>
    /// <param name="sendPrivateMessage">if set to <c>true</c> the message is sent privately to a user.</param>
    void Open( string sendToChannel, bool sendPrivateMessage )
    {
        if( m_ShowFriendList == true )
        {
            CloseFriendList();
        }

        m_IsOpen = true;
        m_SendToChannel = sendToChannel;
        m_SendPrivateMessage = sendPrivateMessage;
        m_TextInput = "";

        //While we are texting, ship input should be disabled
		//Could be used in Uplink -Justin
        Ship.LocalPlayer.ShipInput.SetInputEnabled( false );
    }

    /// <summary>
    /// Closes the chat text input and reenables control for the user
    /// </summary>
    void Close()
    {
        m_IsOpen = false;
        m_TextInput = "";
        Ship.LocalPlayer.ShipInput.SetInputEnabled( true );
    }

    /// <summary>
    /// Opens the friend list.
    /// </summary>
    void OpenFriendList()
    {
        m_AddFriendName = "";
        m_ShowFriendList = true;
        Ship.LocalPlayer.ShipInput.SetInputEnabled( false );
    }

    /// <summary>
    /// Closes the friend list.
    /// </summary>
    void CloseFriendList()
    {
        m_ShowFriendList = false;
        Ship.LocalPlayer.ShipInput.SetInputEnabled( true );
    }

    /// <summary>
    /// Sends the text that was previously typed in by the user
    /// </summary>
    void SendText()
    {
        m_ChatHandler.SendText( m_TextInput, m_SendToChannel, m_SendPrivateMessage );
        Close();
    }

    /// <summary>
    /// Everything is drawn here
    /// </summary>
    void OnGUI()
    {
        //Don't draw anything if we are not connected
        if( ChatHandler.IsConnected() == false )
        {
            return;
        }

        //Don't draw anything if the user hasn't created his ship yet
        if( Ship.LocalPlayer == null )
        {
            return;
        }

        //Handle all input events. Whenever an input event was handled we want to
        //return here because some GUI elements may have changed in the process
        if( GrabInputEvents() == true )
        {
            return;
        }

        if( m_IsOpen == true )
        {
            DrawChatInputWindow();
        }
        else if( m_ShowFriendList == true )
        {
            DrawFriendList();
        }
        else
        {
            DrawChatInfoLabel();
        }

        //Draw the received chat messages
        DrawChatMessages();
    }

    bool GrabInputEvents()
    {
        if( Event.current.type != EventType.keyUp )
        {
            return false;
        }

        if( m_IsOpen == true )
        {
            switch( Event.current.keyCode )
            {
            case KeyCode.Return:
            case KeyCode.KeypadEnter:
                SendText();
                return true;
            }
        }
        else if( m_ShowFriendList == true )
        {
            
        }
        else
        {
            switch( Event.current.keyCode )
            {
            case KeyCode.Return:
            case KeyCode.KeypadEnter:
                Open( ChatHandler.RoomChannelName, false );
                Event.current.Use();
                return true;
            case KeyCode.T:
                Open( ChatHandler.TeamChannelName, false );
                Event.current.Use();
                return true;
            case KeyCode.G:
                Open( "global", false );
                Event.current.Use();
                return true;
            case KeyCode.F:
                OpenFriendList();
                Event.current.Use();
                return true;
            case KeyCode.R:
                if( ChatHandler.GetLastPrivateMessageSender() != "" )
                {
                    Open( ChatHandler.GetLastPrivateMessageSender(), true );
                    Event.current.Use();
                    return true;
                }
                break;
            }
        }

        return false;
    }

    void DrawChatMessages()
    {
        int showLines = 3;

        if( m_IsOpen == true )
        {
            showLines = 10;
        }

        showLines = Mathf.Min( showLines, ChatHandler.GetMessages().Count );
        string finalMessage = "";
        string[] messages = ChatHandler.GetMessages().ToArray();

        for( int i = messages.Length - showLines; i < messages.Length; ++i )
        {
            finalMessage += "\n" + messages[ i ];
        }

        GUI.Label( new Rect( 5, 10, Screen.width, Screen.height - 40 ), finalMessage, Styles.LabelSmallBottomLeft );
    }

    void DrawChatInputWindow()
    {
        float width = 500;

        string channelName = m_SendToChannel;

        if( m_SendPrivateMessage == false )
        {
            channelName = ChatHandler.GetReadableChannelName( m_SendToChannel );
        }

        GUI.FocusControl( "InputText" );
        GUI.SetNextControlName( "InputText" );
        GUI.Label( new Rect( 5, Screen.height - 32, 75, 30 ), "[" + channelName + "]", Styles.LabelSmall );
        m_TextInput = GUI.TextField( new Rect( 80, Screen.height - 32, width, 30 ), m_TextInput, Styles.TextField );
    }

    void DrawChatInfoLabel()
    {
        GUI.Label( new Rect( 5, Screen.height - 25, Screen.width, 25 ), "Press Enter to chat, T to chat to your team, G to chat with all rooms or F to open the friend list", Styles.LabelSmall );
    }

    void DrawFriendList()
    {
        float width = 300;
        float height = 500;

        GUILayout.BeginArea( new Rect( ( Screen.width - width ) * 0.5f, ( Screen.height - height ) * 0.5f, width, height ), Styles.Box );
        {
            for( int i = 0; i < ChatHandler.GetFriendList().Count; ++i )
            {
                if( ChatHandler.GetFriendList()[ i ].Name == "" )
                {
                    continue;
                }

                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label( ChatHandler.GetFriendList()[ i ].Name, Styles.LabelSmall );

                    if( GUILayout.Button( "Remove", Styles.Button ) )
                    {
                        ChatHandler.RemoveFriend( ChatHandler.GetFriendList()[ i ].Name );
                    }

                    if( GUILayout.Button( "Chat", Styles.Button ) )
                    {
                        CloseFriendList();
                        Open( ChatHandler.GetFriendList()[ i ].Name, true );
                    }
                }
                GUILayout.EndHorizontal();

                string status = GetReadableFriendStatus( ChatHandler.GetFriendList()[ i ].Status );

                if( ChatHandler.GetFriendList()[ i ].StatusMessage != "" )
                {
                    status += " (" + ChatHandler.GetFriendList()[ i ].StatusMessage.ToString() + ")";
                }

                GUILayout.Label( status, Styles.LabelSmall );
            }

            GUILayout.FlexibleSpace();

            GUILayout.Label( "Enter your friends name:", Styles.LabelSmall );
            m_AddFriendName = GUILayout.TextField( m_AddFriendName, Styles.TextField );

            if( GUILayout.Button( "Add Friend", Styles.Button ) )
            {
                if( m_AddFriendName != "" )
                {
                    ChatHandler.AddFriend( m_AddFriendName );
                    m_AddFriendName = "";
                }
            }

            if( GUILayout.Button( "Close", Styles.Button ) )
            {
                CloseFriendList();
            }
        }
        GUILayout.EndArea();
    }

    /// <summary>
    /// Since the online status is only sent as an integer, this function helps us to get a
    /// readable string from those integers
    /// </summary>
    /// <param name="status">The integer status that is sent by the chat API.</param>
    /// <returns></returns>
    string GetReadableFriendStatus( int status )
    {
        switch( status )
        {
        case 0:
            return "Offline";
        case 1:
            return "Invisible";
        case 2:
            return "Online";
        case 3:
            return "Away";
        case 4:
            return "DND";
        case 5:
            return "LFG";
        case 6:
            return "Playing";
        }

        return "";
    }
}
