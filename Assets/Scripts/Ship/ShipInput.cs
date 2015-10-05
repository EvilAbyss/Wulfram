using UnityEngine;
using System.Collections;

/// <summary>
/// This class checks the user input and sends the data on to the components that need it
/// </summary>
public class ShipInput : ShipBase
{
	public float MouseSensitivity;

	Vector2 m_MousePosition;
	ShipCrosshair m_Crosshair;
	bool m_MouseControlsEnabled = true;
	bool m_InputEnabled = true;

	void Start()
	{
		if( PhotonView.isMine == true )
		{
			//lockCursor makes sure that the cursor always stays in the middle of the screen
			//This is useful if players are playing with mouse input in windowed mode so that
			//the mouse will never leave the screen area
			Screen.lockCursor = true;
		}
	}

	void Update()
	{
		if( m_InputEnabled == false )
		{
			return;
		}

		if( GamemodeManager.CurrentGamemode.IsRoundFinished() == true )
		{
			return;
		}

		//We only want to check the input if this is the local ship
		//For remote ships, we get the necessary movement data over the network
		if( PhotonView.isMine == true )
		{
			UpdateCursorLock();

			UpdateTargetPitch();
			UpdateTargetTurn();
			UpdateTargetBoost();
			UpdateTargetTilt();
			UpdateIsShooting();

			UpdateLoopingMode();

			UpdateMousePosition();
			UpdateKeyboardBoost();
			UpdateKeyboardTilt();

			UpdateIsShooting2();


		}
	}

	void UpdateCursorLock()
	{
		if( Screen.lockCursor == false )
		{
			if( Input.GetMouseButtonDown( 0 ) == true )
			{
				Screen.lockCursor = true;
			}
		}
	}

	void UpdateMousePosition()
	{
		//Clamp the mouse input to [-1; 1]
		m_MousePosition.x = Mathf.Clamp( Input.GetAxisRaw( "Mouse X" ) * MouseSensitivity + m_MousePosition.x, -1f, 1f );
		m_MousePosition.y = Mathf.Clamp( Input.GetAxisRaw( "Mouse Y" ) * MouseSensitivity + m_MousePosition.y, -1f, 1f );

		//If there is mouse movement, switch from gamepad controls to mouse controls
		if( Mathf.Abs( Input.GetAxisRaw( "Mouse X" ) ) > 0.1f || Mathf.Abs( Input.GetAxisRaw( "Mouse Y" ) ) > 0.1f )
		{
			SetMouseControlsEnabled( true );
		}

		if( m_MouseControlsEnabled == true )
		{
			ShipMovement.TargetTurn = m_MousePosition.x;
			ShipMovement.TargetPitch = m_MousePosition.y;

			if( m_Crosshair != null )
			{
				m_Crosshair.SetMouseTargetPosition( m_MousePosition );
			}
		}
	}

	void UpdateKeyboardBoost()
	{
		float targetBoost = Input.GetAxisRaw( "VerticalKeyboard" );

		//again, making sure to switch the control scheme if any input is detected from the keyboard
		if( Mathf.Abs( targetBoost ) > 0.1f )
		{
			SetMouseControlsEnabled( true );
		}

		if( m_MouseControlsEnabled == true )
		{
			ShipMovement.TargetBoost = targetBoost;
		}
	}

	void UpdateKeyboardTilt()
	{
		float targetTilt = Input.GetAxisRaw( "HorizontalKeyboard" );

		//again, making sure to switch the control scheme if any input is detected from the keyboard
		if( Mathf.Abs( targetTilt ) > 0.1f )
		{
			SetMouseControlsEnabled( true );
		}

		if( m_MouseControlsEnabled == true )
		{
			ShipMovement.TargetTilt = targetTilt;
		}
	}

	void UpdateLoopingMode()
	{
		//This is the gamepad control and looping mode is initialized by pressing down the left thumbstick
		//You can easily leave looping mode with the gamepad by letting go of the thumbstick for a split second
		//thats why we are only initiating looping mode here, instead of toggling it for keyboard and mouse input
		if( Input.GetButton( "LoopingMode" ) == true )
		{
			ShipMovement.InitiateLoopingMode();
		}

		//The way the mouse input is constructed, the ship will continue to turn in the same direction it did before
		//even if you don't move the mouse. This means it's not easy to just return the mouse into a zero position to
		//leave looping mode. So we give the player the option to toggle it on and off by repeatedly pressing the looping button
		if( Input.GetButtonDown( "LoopingModeKeyboard" ) )
		{
			if( ShipMovement.IsInLoopingMode == true )
			{
				m_MousePosition = Vector2.zero;
			}

			ShipMovement.ToggleLoopingMode();
		}
	}

	void UpdateIsShooting()
	{
		ShipShooting.IsShooting = Input.GetButton( "FireKeyboard" ) || Input.GetAxisRaw( "FireTrigger" ) > 0.1f;


	}

	void UpdateIsShooting2()
	{
		ShipShooting2.IsShooting = Input.GetButton( "Fire2" );
	
		
	}


	void UpdateTargetTurn()
	{
		float targetTurn = Input.GetAxisRaw( "Horizontal" );

		//again, making sure to switch the control scheme if any input is detected from the keyboard
		if( Mathf.Abs( targetTurn ) > 0.1f )
		{
			SetMouseControlsEnabled( false );
		}

		if( m_MouseControlsEnabled == false )
		{
			ShipMovement.TargetTurn = targetTurn;
		}
	}

	void UpdateTargetPitch()
	{
		float targetPitch = Input.GetAxisRaw( "Vertical" );

		//we are in gamepad territory now, so if we detect input here, we disable mouse controls
		if( Mathf.Abs( targetPitch ) > 0.1f )
		{
			SetMouseControlsEnabled( false );
		}

		if( m_MouseControlsEnabled == false )
		{
			ShipMovement.TargetPitch = targetPitch;
		}
	}

	void UpdateTargetBoost()
	{
		float targetBoost = Input.GetAxisRaw( "Vertical2" );

		//we are in gamepad territory now, so if we detect input here, we disable mouse controls
		if( Mathf.Abs( targetBoost ) > 0.1f )
		{
			SetMouseControlsEnabled( false );
		}

		if( m_MouseControlsEnabled == false )
		{
			ShipMovement.TargetBoost = targetBoost;
		}
	}

	void UpdateTargetTilt()
	{
		float targetTilt = Input.GetAxisRaw( "Horizontal2" );

		//we are in gamepad territory now, so if we detect input here, we disable mouse controls
		if( Mathf.Abs( targetTilt ) > 0.1f )
		{
			SetMouseControlsEnabled( false );
		}

		if( m_MouseControlsEnabled == false )
		{
			ShipMovement.TargetTilt = targetTilt;
		}
	}

	void OnCrosshairCreated()
	{
		m_Crosshair = transform.Find( "Crosshair" ).GetComponent<ShipCrosshair>();
	}

	public bool IsMouseControlsEnabled()
	{
		return m_MouseControlsEnabled;
	}

	void SetMouseControlsEnabled( bool enabled )
	{
		//It is necessary to enable and disable mouse controls as needed, because there are additional arrow indicators visible during mouse mode
		//and we want to hide them if the player switches to gamepad control on the fly

		m_MouseControlsEnabled = enabled;

		if( m_Crosshair != null )
		{
			m_Crosshair.SetMouseTargetVisibility( enabled );
		}

		if( m_MouseControlsEnabled == false )
		{
			m_MousePosition = Vector2.zero;
		}
	}

	public void SetInputEnabled( bool inputEnabled )
	{
		m_InputEnabled = inputEnabled;
	}

	public void OnRespawn()
	{
		m_MousePosition = Vector2.zero;

		if( m_Crosshair != null )
		{
			m_Crosshair.SetMouseTargetPosition( m_MousePosition );
		}
	}
}