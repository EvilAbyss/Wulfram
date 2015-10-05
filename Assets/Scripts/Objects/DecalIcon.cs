using UnityEngine;
using System.Collections;

/// <summary>
/// This class draws a small GUI icon of an object on screen. The icon is also drawn if the object
/// is not visible on screen. In that case the icon is clamped to the nearest sensible screen edge
/// The icon is not drawn when it is behind the player though.
/// </summary>
public class DecalIcon : MonoBehaviour
{
	/// <summary>
	/// Set the icon that should be drawn here
	/// </summary>
	public Texture2D Icon;


	/// <summary>
	/// Should the icon be drawn at a different position relative to the GameObject. Expected coordinates are in world space
	/// </summary>
	public Vector3 Offset;


	/// <summary>
	/// The icon can be tinted with this property
	/// </summary>
	public Color Color = Color.white;

	void OnGUI()
	{
		Vector3 position = transform.position + Offset;
		Plane plane = new Plane( Camera.main.transform.forward, Camera.main.transform.position );

		//If the object is behind the camera, then don't draw it
		if( plane.GetSide( position ) == false )
		{
			return;
		}

		//Calculate the 2D position of the position where the icon should be drawn
		Vector3 viewportPoint = Camera.main.WorldToViewportPoint( position );

		//The viewportPoint coordinates are between 0 and 1, so we have to convert them into screen space here
		Vector2 drawPosition = new Vector2(	viewportPoint.x * Screen.width, Screen.height * ( 1 - viewportPoint.y ) );

		float clampBorder = 12;

		//Clamp the position to the edge of the screen in case the icon would be drawn outside the screen
		drawPosition.x = Mathf.Clamp( drawPosition.x, clampBorder, Screen.width - clampBorder );
		drawPosition.y = Mathf.Clamp( drawPosition.y, clampBorder, Screen.height - clampBorder );

		GUI.color = Color;
		GUI.DrawTexture( new Rect( drawPosition.x - Icon.width * 0.5f, drawPosition.y - Icon.height * 0.5f, Icon.width, Icon.height ), Icon );
	}
}