using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon.Chat;

/// <summary>
/// This class implements everything related to the Photon Chat API. 
/// It connects to the chat server, subscribes to the global channel and the room channels 
/// as soon as the player joins a room, handles the friend list and so on.
/// There is a full video dedicated to this implementation here: https://www.youtube.com/watch?v=pRmvHJJdPjI
/// </summary>
public class ChatHandler : bl_Login, IChatClientListener
{
    #region Properties
    static ChatHandler m_Instance;
    /// <summary>
    /// This static variable provides a quick way to access the chat functionality from everywhere in the project
    /// </summary>
    /// <value>
    /// Returns the single instance of this component
    /// </value>
    public static ChatHandler Instance
    {
        get
        {
            //If there is no instance yet, create one
            if( m_Instance == null )
            {
                GameObject newObject = new GameObject( "Chat" );
                m_Instance = newObject.AddComponent<ChatHandler>();
                newObject.AddComponent<ChatGUI>();
            }

            return m_Instance;
        }
    }

    /// <summary>
    /// Gets the name of the chat channel that is available to everybody in the same room
    /// </summary>
    public string RoomChannelName
    {
        get
        {
            return m_RoomChannelName;
        }
    }

    /// <summary>
    /// Gets the name of the team channel, if the current gamemode has teams
    /// </summary>
    public string TeamChannelName
    {
        get
        {
            return m_TeamChannelName;
        }
    }

    /// <summary>
    /// Gets the ChatClient object that is the core of the chat API
    /// </summary>
    public ChatClient Client
    {
        get
        {
            return m_ChatClient;
        }
    }

    /// <summary>
    /// The chat App Id that belongs to this game. It has to be created in the chat section on exitgames.com
    /// Note that this is different to the Photon App Id. Chat and Photon are seperate to allow for more flexibility
    /// </summary>
    public string ChatAppId;

    /// <summary>
    /// The chat username
    /// </summary>
    public string ChatUsername;

    #endregion

    #region Members
    /// <summary>
    /// The ChatClient object. This is the core of the chat API
    /// </summary>
    ChatClient m_ChatClient;

    /// <summary>
    /// Are we connected to the chat server?
    /// </summary>
    bool m_Connected;

    /// <summary>
    /// The channel name that belongs to the room we are currently playing in
    /// </summary>
    string m_RoomChannelName = null;

    /// <summary>
    /// The channel name that belongs to our current team
    /// </summary>
    string m_TeamChannelName = null;

    /// <summary>
    /// Are we already subscribed to the room channels?
    /// </summary>
    bool m_SubscribedToRoom = false;

    /// <summary>
    /// A list of chat messages that we received.
    /// </summary>
    Queue<string> m_Messages = new Queue<string>();

    /// <summary>
    /// A list of our ChatFriends. This stores their online status once it is received from the chat server
    /// </summary>
    List<ChatFriend> m_FriendList = new List<ChatFriend>();

    /// <summary>
    /// The last person who sent us a private message so we can quickly reply to them by pressing 'R'
    /// </summary>
    string m_LastPrivateMessageSender = "";
    
    ChatGUI m_ChatGui;

    /// <summary>
    /// Gets the chat GUI component that draws everything chat related
    /// </summary>
    ChatGUI ChatGui
    {
        get
        {
            return Helper.GetCachedComponent<ChatGUI>( gameObject, ref m_ChatGui );
        }
    }
    #endregion

    void Awake() 
    {
        //Make sure only one ChatHandler object exists
        if( m_Instance != null && m_Instance != this )
        {
            Destroy( gameObject );
            return;
        }

        //The chat client object needs an IChatListener as parameter. The IChatListener interface defines 
        //several callback methods that the Chat API calls when certain events happened. We made this
        //ChatHandler class into an IChatListener so we can pass it as a parameter here
        m_ChatClient = new ChatClient( this );

        m_Instance = this;

        //This GameObject should not be destroyed when a new level is loaded
        DontDestroyOnLoad( gameObject );

        // this must run in background or it will drop connection if not focused.
        Application.runInBackground = true; 
    }

    void Update() 
    {
        if( m_ChatClient == null )
        {
            return;
        }

        //Check every frame if we are subscribed to the room channels yet and, if not
        //wait until the player has joined a room and a team to join the room channels
        if( m_SubscribedToRoom == false &&
            Ship.LocalPlayer != null &&
            Ship.LocalPlayer.Team != Team.None )
        {
            JoinRoomChannels();
        }

        //The Service function of ChatClient has to be called as often as possible.
        //This is where all the functionality of the Chat API is implemented. It looks
        //for incoming messages and events and calls the necessary callback functions
        m_ChatClient.Service();
    }

    /// <summary>
    /// Connect to the chat server using the app id and username the player chose
    /// </summary>
    public void Connect()
    {
		m_ChatClient.Connect( ChatAppId, "1.0", ChatUsername, null );
	
    }

    /// <summary>
    /// Once the player has connected to a room, we want to subscribe to specific
    /// room channels that only players in that room are subscribed to so they can
    /// talk to each other. We also join a team room so the team can talk to each
    /// other without the other team listening in
    /// </summary>
    void JoinRoomChannels()
    {
        m_SubscribedToRoom = true;

        //PhotonNetwork.room.name is the name of the currently joined room. This is the
        //same for all players in that room so we can use it to construct the room channel name
        m_RoomChannelName = "room" + PhotonNetwork.room.name.GetHashCode();

        //For the team channel, we simply use the same room channel name and add the team color at the end
        m_TeamChannelName = m_RoomChannelName + Ship.LocalPlayer.Team;

        //By passing an array of strings we tell the Chat API to connect to both these rooms.
        m_ChatClient.Subscribe( new string[] { m_RoomChannelName, m_TeamChannelName } );
    }

    /// <summary>
    /// If the player leaves the room, we want to leave the previously joined room channels too.
    /// </summary>
    void LeaveRoomChannels()
    {
        if( m_TeamChannelName != null )
        {
            m_ChatClient.Unsubscribe( new string[] { m_TeamChannelName } );
        }

        if( m_RoomChannelName != null )
        {
            m_ChatClient.Unsubscribe( new string[] { m_RoomChannelName } );
        }

        m_SubscribedToRoom = false;
    }

    /// <summary>
    /// SetOnlineStatus updates the players online status, which gets send to all players who are
    /// friends with this player. You can use these values however you want
    /// </summary>
    /// <param name="status">An integer representing the current status. 
    /// There are a few predefined values in ExitGames.Client.Photon.Chat.ChatUserStatus, 
    /// but you can use whatever value you like since you are also handling how the client deals with them in OnStatusUpdate</param>
    /// <param name="message">This is an optional message you can send. 
    /// I use this to send the name of the room the player is currently playing in so friends can find each other easily</param>
    public void SetOnlineStatus( int status, string message )
    {
        m_ChatClient.SetOnlineStatus( status, message );
    }

    /// <summary>
    /// Are we connected to the chat server?
    /// </summary>
    public bool IsConnected()
    {
        return m_Connected;
    }

    /// <summary>
    /// Send a public or a private chat message
    /// </summary>
    /// <param name="text">The chat message.</param>
    /// <param name="channel">The channel to which this message is sent. If it's a private message this contains the name of the receiving player</param>
    /// <param name="isPrivateMessage">if set to <c>true</c>, the message is sent as a private message.</param>
    public void SendText( string text, string channel, bool isPrivateMessage )
    {
        if( text == "" )
        {
            return;
        }

        if( isPrivateMessage == true )
        {
            //in this case, channel is filled with the name of the user you want to chat with
            m_ChatClient.SendPrivateMessage( channel, text );
        }
        else
        {
            //Sending chat messages is as simple as this.
            m_ChatClient.PublishMessage( channel, text );
        }
    }

    /// <summary>
    /// Gets a list of all the recent chat messages
    /// </summary>
    /// <returns></returns>
    public Queue<string> GetMessages()
    {
        return m_Messages;
    }

    /// <summary>
    /// This function converts the received channel name to a readable word
    /// </summary>
    /// <param name="channel">The channel name which may look like "room416226Red" and converts it to a single word.</param>
    /// <returns>"Team", "Room" or "Global"</returns>
    public string GetReadableChannelName( string channel )
    {
        if( channel == TeamChannelName )
        {
            return "Team";
        }

        if( channel == RoomChannelName )
        {
            return "Room";
        }

        return "Global";
    }

    /// <summary>
    /// Get the name of the last player who sent a private message to us
    /// </summary>
    public string GetLastPrivateMessageSender()
    {
        return m_LastPrivateMessageSender;
    }

    #region Friend List
    /// <summary>
    /// Load a list of names from the PlayerPrefs and create the friend list from it
    /// </summary>
    void LoadFriendList()
    {
        //The friend list is stored like this "friend1;friend2;friend3"
        string friendListString = PlayerPrefs.GetString( "FriendList", "" );
        m_FriendList.Clear();

        //By splitting the string, we receive an array of all the names that are on our friend list
        string[] friends = friendListString.Split( ';' );

        //And then we add all of those names to our list
        for( int i = 0; i < friends.Length; ++i )
        {
            AddFriend( friends[ i ] );
        }
    }

    /// <summary>
    /// Create a ChatFriend object for the given friend name if it doesn't already exist
    /// </summary>
    /// <param name="friendName">Name of the friend.</param>
    public void AddFriend( string friendName )
    {
        if( string.IsNullOrEmpty( friendName ) == true )
        {
            return;
        }

        //Don't add it if this friend is already on our list
        if( m_FriendList.Find( item => item.Name == friendName ) != null )
        {
            return;
        }

        //Tell the Chat API that we've added a new friend for whom we want to receive status updates
        m_ChatClient.AddFriends( new string[] { friendName } );
        m_FriendList.Add( new ChatFriend
        {
            Name = friendName,
            Status = 0,
            StatusMessage = "",
        } ); 

        //Store the new friend list in the player prefs
        PlayerPrefs.SetString( "FriendList", string.Join( ";", GetFriendListNameArray() ) );
    }

    /// <summary>
    /// Removes a friend from our friend list
    /// </summary>
    /// <param name="friendName">Name of the friend.</param>
    public void RemoveFriend( string friendName )
    {
        //Tell the chat API that we no longer want to receive updates for that friend
        m_ChatClient.RemoveFriends( new string[] { friendName } );

        //Remove the ChatFriend object from our list
        m_FriendList.RemoveAll( item => item.Name == friendName );

        //Store the new friend list in the player prefs
        PlayerPrefs.SetString( "FriendList", string.Join( ";", GetFriendListNameArray() ) );
    }

    /// <summary>
    /// Gets the friend list.
    /// </summary>
    public List<ChatFriend> GetFriendList()
    {
        return m_FriendList;
    }

    /// <summary>
    /// Gets the friend list as a simple array of strings
    /// </summary>
    /// <returns>An array of strings with the names of our friends</returns>
    string[] GetFriendListNameArray()
    {
        string[] friends = new string[ m_FriendList.Count ];

        for( int i = 0; i < friends.Length; ++i )
        {
            friends[ i ] = m_FriendList[ i ].Name;
        }

        return friends;
    }
    #endregion

    /// <summary>
    /// Called by Unity when the game is closed.
    /// </summary>
    public void OnApplicationQuit()
    {
        if( m_ChatClient == null )
        {
            return;
        }

        //Tell the chat api that we are done
        m_ChatClient.Disconnect();
    }

    /// <summary>
    /// Called by the chat API when we disconnected from the chat server
    /// </summary>
    public void OnDisconnected()
    {
        Debug.Log( "Disconnected: " + m_ChatClient.DisconnectedCause );
    }

    /// <summary>
    /// Called by the chat API when we connected to the chat server
    /// </summary>
    public void OnConnected()
    {
        m_Connected = true;

        //Automatically subscribe to the global channel. Since everyone is subscribed to this channel, everybody can
        //talk to everyone else in the game. The usefulness is debatable but it's the simplest demonstration how you
        //subscribe to a chat channel
        m_ChatClient.Subscribe( new string[] { "global" } );

        //Tell our friends that we're online
        m_ChatClient.SetOnlineStatus( ChatUserStatus.Online );

        LoadFriendList();
    }

    /// <summary>
    /// The ChatClient's state changed. Usually, OnConnected and OnDisconnected are the callbacks to react to.
    /// </summary>
    /// <param name="state">The new state.</param>
    public void OnChatStateChange( ChatState state )
    {

    }

    /// <summary>
    /// Notifies app that client got new messages from server
    /// Number of senders is equal to number of messages in 'messages'. Sender with number '0' corresponds to message with
    /// number '0', sender with number '1' corresponds to message with number '1' and so on
    /// </summary>
    /// <param name="channelName">channel from where messages came</param>
    /// <param name="senders">list of users who sent messages</param>
    /// <param name="messages">list of messages it self</param>
    public void OnGetMessages( string channelName, string[] senders, object[] messages )
    {
        //For each message that we received
        for( int i = 0; i < senders.Length; ++i )
        {
            //Construct a readable string with information about where the message came from and what it says
            string message = 
                "[" + GetReadableChannelName( channelName ) + "] " 
              + senders[ i ] + ": "
              + messages[ i ];

            //And then add it to our list of messages
            AddMessage( message );
        }
    }

    /// <summary>
    /// Notifies client about private message
    /// </summary>
    /// <param name="sender">user who sent this message</param>
    /// <param name="message">message it self</param>
    /// <param name="channelName">channelName for private messages (messages you sent yourself get added to a channel per target username)</param>
    public void OnPrivateMessage( string sender, object message, string channelName )
    {
        //We want to store the last person who sent us a private message so we can quickly reply to them by pressing 'R'
        m_LastPrivateMessageSender = sender;

        //Add the private message to our list of messages
        AddMessage( "[Private] " + sender + ": " + message );
    }

    /// <summary>
    /// Adds a chat message to our list of messages
    /// </summary>
    /// <param name="message">The message.</param>
    void AddMessage( string message )
    {
        m_Messages.Enqueue( message );

        //We only want to store the last 10 messages
        while( m_Messages.Count > 10 )
        {
            m_Messages.Dequeue();
        }
    }

    /// <summary>
    /// Result of Subscribe operation. Returns per channelname if the channel is now subscribed.
    /// </summary>
    /// <param name="channels">Array of channel names.</param>
    /// <param name="results">Per channel result if subscribed.</param>
    public void OnSubscribed( string[] channels, bool[] results )
    {

    }

    /// <summary>
    /// Result of Unsubscribe operation. Returns per channelname if the channel is now unsubscribed.
    /// </summary>
    /// <param name="channels">Array of channel names that are no longer subscribed.</param>
    public void OnUnsubscribed( string[] channels )
    {

    }

    /// <summary>
    /// New status of another user (you get updates for users set in your friends list).
    /// </summary>
    /// <param name="user">Name of the user.</param>
    /// <param name="status">New status of that user.</param>
    /// <param name="gotMessage">True if the status contains a message you should cache locally. False: This status update does not include a message (keep any you have).</param>
    /// <param name="message">Message that user set.</param>
    public void OnStatusUpdate( string user, int status, bool gotMessage, object message )
    {
        //When a status update arrived
        //search our locally stored friend list
        for( int i = 0; i < m_FriendList.Count; ++i )
        {
            //If we have that user in our friend list
            if( user == m_FriendList[ i ].Name )
            {
                //Update its status
                m_FriendList[ i ].Status = status;

                if( gotMessage == true && message != null )
                {
                    m_FriendList[ i ].StatusMessage = message.ToString();
                }
                else
                {
                    m_FriendList[ i ].StatusMessage = "";
                }
            }
        }
    }

    /// <summary>
    /// Called by Unity when a level finished loading
    /// </summary>
    /// <param name="level">The level.</param>
    void OnLevelWasLoaded( int level )
    {
        //If we are back in the main menu or the room browser
        if( level == 0 || level == 1 )
        {
            LeaveRoomChannels();
        }
    }
}

/// <summary>
/// This class holds all necessary data to store and display your friends status
/// </summary>
public class ChatFriend
{
    public string Name;
    public int Status;
    public string StatusMessage;
}