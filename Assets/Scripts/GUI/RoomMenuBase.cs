using UnityEngine;
using System.Collections;

public class RoomMenuBase : MonoBehaviour 
{
	RoomMenu m_RoomMenu;
	protected RoomMenu RoomMenu
	{
		get
		{
			return Helper.GetCachedComponent<RoomMenu>( gameObject, ref m_RoomMenu );
		}
	}

	protected Rect GetBackgroundRect()
	{
		return new Rect( 20, 75, Screen.width - 40, Screen.height - 95 );
	}

	protected Rect GetPaddedBackgroundRect()
	{
		return new Rect( 40, 95, Screen.width - 80, Screen.height - 135 );
	}

	protected void DrawBackground()
	{
		GUI.Box( GetBackgroundRect(), "", Styles.Box );
	}
}
