using UnityEngine;
using System.Collections;

public class Styles 
{
	static Font m_BoldFont;
	public static Font BoldFont
	{
		get
		{
			if( m_BoldFont == null )
			{
				m_BoldFont = Resources.Load<Font>( "Fonts/Orbitron Black" );
			}

			return m_BoldFont;
		}
	}

	static Font m_RegularFont;
	public static Font RegularFont
	{
		get
		{
			if( m_RegularFont == null )
			{
				m_RegularFont = Resources.Load<Font>( "Fonts/Jura-Medium" );
			}

			return m_RegularFont;
		}
	}

	static Texture2D m_DarkButtonBackgroundTexture;
	public static Texture2D DarkButtonBackgroundTexture
	{
		get
		{
			if( m_DarkButtonBackgroundTexture == null )
			{
				m_DarkButtonBackgroundTexture = Resources.Load<Texture2D>( "GUI/DarkButtonBackground" );
			}

			return m_DarkButtonBackgroundTexture;
		}
	}

	static Texture2D m_ButtonBackgroundTexture;
	public static Texture2D ButtonBackgroundTexture
	{
		get
		{
			if( m_ButtonBackgroundTexture == null )
			{
				m_ButtonBackgroundTexture = Resources.Load<Texture2D>( "GUI/ButtonBackground" );
			}

			return m_ButtonBackgroundTexture;
		}
	}

	static Texture2D m_ReverseButtonBackgroundTexture;
	public static Texture2D ReverseButtonBackgroundTexture
	{
		get
		{
			if( m_ReverseButtonBackgroundTexture == null )
			{
				m_ReverseButtonBackgroundTexture = Resources.Load<Texture2D>( "GUI/ReverseButtonBackground" );
			}

			return m_ReverseButtonBackgroundTexture;
		}
	}

	static GUIStyle m_BoxStyle;
	public static GUIStyle Box
	{
		get
		{
			if( m_BoxStyle == null )
			{
				m_BoxStyle = new GUIStyle( GUI.skin.box );
				m_BoxStyle.normal.background = ButtonBackgroundTexture;
			}

			return m_BoxStyle;
		}
	}

	static GUIStyle m_DarkBoxStyle;
	public static GUIStyle DarkBox
	{
		get
		{
			if( m_DarkBoxStyle == null )
			{
				m_DarkBoxStyle = new GUIStyle( GUI.skin.box );
				m_DarkBoxStyle.normal.background = DarkButtonBackgroundTexture;
			}

			return m_DarkBoxStyle;
		}
	}

	public static GUIStyle GetSelectableButtonStyle( bool isActive )
	{
		if( isActive == true )
		{
			return ButtonActive;
		}

		return Button;
	}

	public static GUIStyle GetSelectableDarkButtonStyle( bool isActive )
	{
		if( isActive == true )
		{
			return DarkButtonActive;
		}

		return DarkButton;
	}

	static GUIStyle m_ButtonStyle;
	public static GUIStyle Button
	{
		get
		{
			if( m_ButtonStyle == null )
			{
				m_ButtonStyle = new GUIStyle( GUI.skin.button );
				m_ButtonStyle.font = BoldFont;
				m_ButtonStyle.fontSize = 20;
				m_ButtonStyle.alignment = TextAnchor.MiddleCenter;

				m_ButtonStyle.normal.background = ButtonBackgroundTexture;
				m_ButtonStyle.normal.textColor = Color.grey;

				m_ButtonStyle.hover.background = ButtonBackgroundTexture;
				m_ButtonStyle.hover.textColor = Color.white;

				m_ButtonStyle.active.background = ButtonBackgroundTexture;
				m_ButtonStyle.active.textColor = Color.white;
			}

			return m_ButtonStyle;
		}
	}

	static GUIStyle m_DarkButtonStyle;
	public static GUIStyle DarkButton
	{
		get
		{
			if( m_DarkButtonStyle == null )
			{
				m_DarkButtonStyle = new GUIStyle( Button );

				m_DarkButtonStyle.normal.background = DarkButtonBackgroundTexture;
				m_DarkButtonStyle.hover.background = DarkButtonBackgroundTexture;
				m_DarkButtonStyle.active.background = DarkButtonBackgroundTexture;
			}

			return m_DarkButtonStyle;
		}
	}

	static GUIStyle m_ButtonActiveStyle;
	public static GUIStyle ButtonActive
	{
		get
		{
			if( m_ButtonActiveStyle == null )
			{
				m_ButtonActiveStyle = new GUIStyle( Button );

				m_ButtonActiveStyle.normal.textColor = Color.white;
				m_ButtonActiveStyle.normal.background = ReverseButtonBackgroundTexture;
				m_ButtonActiveStyle.hover.background = ReverseButtonBackgroundTexture;
				m_ButtonActiveStyle.active.background = ReverseButtonBackgroundTexture;
			}

			return m_ButtonActiveStyle;
		}
	}

	static GUIStyle m_DarkButtonActiveStyle;
	public static GUIStyle DarkButtonActive
	{
		get
		{
			if( m_DarkButtonActiveStyle == null )
			{
				m_DarkButtonActiveStyle = new GUIStyle( ButtonActive );

				m_DarkButtonActiveStyle.normal.background = DarkButtonBackgroundTexture;
				m_DarkButtonActiveStyle.hover.background = DarkButtonBackgroundTexture;
				m_DarkButtonActiveStyle.active.background = DarkButtonBackgroundTexture;
			}

			return m_DarkButtonActiveStyle;
		}
	}

	static GUIStyle m_ButtonServerHeaderStyle;
	public static GUIStyle ButtonServerHeader
	{
		get
		{
			if( m_ButtonServerHeaderStyle == null )
			{
				m_ButtonServerHeaderStyle = new GUIStyle( ButtonActive );

				m_ButtonServerHeaderStyle.normal.textColor = Color.white;
				m_ButtonServerHeaderStyle.hover.textColor = new Color( 1f, 0.8f, 0f );
				m_ButtonServerHeaderStyle.active.textColor = Color.grey;
				m_ButtonServerHeaderStyle.imagePosition = ImagePosition.ImageLeft;
				m_ButtonServerHeaderStyle.contentOffset = new Vector2( -9, 1 );

				m_ButtonServerHeaderStyle.normal.background = DarkButtonBackgroundTexture;
				m_ButtonServerHeaderStyle.hover.background = DarkButtonBackgroundTexture;
				m_ButtonServerHeaderStyle.active.background = DarkButtonBackgroundTexture;
			}

			return m_ButtonServerHeaderStyle;
		}
	}

	static GUIStyle m_LabelStyle;
	public static GUIStyle Label
	{
		get
		{
			if( m_LabelStyle == null )
			{
				m_LabelStyle = new GUIStyle( GUI.skin.label );
				m_LabelStyle.font = RegularFont;
				m_LabelStyle.fontSize = 40;
			}

			return m_LabelStyle;
		}
	}

	static GUIStyle m_LabelStyleCentered;
	public static GUIStyle LabelCentered
	{
		get
		{
			if( m_LabelStyleCentered == null )
			{
				m_LabelStyleCentered = new GUIStyle( Label );
				m_LabelStyleCentered.alignment = TextAnchor.MiddleCenter;
			}

			return m_LabelStyleCentered;
		}
	}

	static GUIStyle m_HeaderStyle;
	public static GUIStyle Header
	{
		get
		{
			if( m_HeaderStyle == null )
			{
				m_HeaderStyle = new GUIStyle( Label );
				m_HeaderStyle.font = BoldFont;
				m_HeaderStyle.fontSize = 30;
				m_HeaderStyle.alignment = TextAnchor.MiddleLeft;
			}

			return m_HeaderStyle;
		}
	}

	static GUIStyle m_EndLabelStyle;
	public static GUIStyle EndMatchLabel
	{
		get
		{
			if( m_EndLabelStyle == null )
			{
				m_EndLabelStyle = new GUIStyle( GUI.skin.label );
				m_EndLabelStyle.font = BoldFont;
				m_EndLabelStyle.fontSize = 60;
				m_EndLabelStyle.alignment = TextAnchor.MiddleCenter;
			}

			return m_EndLabelStyle;
		}
	}

	static GUIStyle m_LabelStyleSmall;
	public static GUIStyle LabelSmall
	{
		get
		{
			if( m_LabelStyleSmall == null )
			{
				m_LabelStyleSmall = new GUIStyle( Label );
				m_LabelStyleSmall.fontSize = 20;
				m_LabelStyleSmall.alignment = TextAnchor.MiddleLeft;
			}

			return m_LabelStyleSmall;
		}
	}

	static GUIStyle m_LabelStyleSmallBottomLeft;
	public static GUIStyle LabelSmallBottomLeft
	{
		get
		{
			if( m_LabelStyleSmallBottomLeft == null )
			{
				m_LabelStyleSmallBottomLeft = new GUIStyle( LabelSmall );
				m_LabelStyleSmallBottomLeft.alignment = TextAnchor.LowerLeft;
			}

			return m_LabelStyleSmallBottomLeft;
		}
	}

	static GUIStyle m_LabelStyleSmallCentered;
	public static GUIStyle LabelSmallCentered
	{
		get
		{
			if( m_LabelStyleSmallCentered == null )
			{
				m_LabelStyleSmallCentered = new GUIStyle( LabelSmall );
				m_LabelStyleSmallCentered.alignment = TextAnchor.MiddleCenter;
			}

			return m_LabelStyleSmallCentered;
		}
	}

	static GUIStyle m_TextFieldStyle;
	public static GUIStyle TextField
	{
		get
		{
			if( m_TextFieldStyle == null )
			{
				m_TextFieldStyle = new GUIStyle( GUI.skin.textField );

				m_TextFieldStyle.normal.background = Resources.Load<Texture2D>( "GUI/TextFieldBackground" );
				m_TextFieldStyle.normal.textColor = Color.white;

				m_TextFieldStyle.hover.background = Resources.Load<Texture2D>( "GUI/TextFieldBackground" );
				m_TextFieldStyle.hover.textColor = Color.white;

				m_TextFieldStyle.active.background = Resources.Load<Texture2D>( "GUI/TextFieldBackground" );
				m_TextFieldStyle.active.textColor = Color.white;

				m_TextFieldStyle.focused.background = Resources.Load<Texture2D>( "GUI/TextFieldBackgroundActive" );
				m_TextFieldStyle.focused.textColor = Color.white;

				m_TextFieldStyle.border = new RectOffset( 4, 4, 4, 4 );
				m_TextFieldStyle.padding = new RectOffset( 6, 3, 3, 3 );
				m_TextFieldStyle.font = RegularFont;
				m_TextFieldStyle.fontSize = 20;
				m_TextFieldStyle.alignment = TextAnchor.MiddleLeft;
			}

			return m_TextFieldStyle;
		}
	}

	static GUIStyle m_Toggle;
	public static GUIStyle Toggle
	{
		get
		{
			if( m_Toggle == null )
			{
				m_Toggle = new GUIStyle( GUI.skin.toggle );

				m_Toggle.font = RegularFont;
				m_Toggle.fontSize = 20;

				m_Toggle.normal.background = Resources.Load<Texture2D>( "GUI/Toggle" );
				m_Toggle.normal.textColor = Color.white;

				m_Toggle.hover.background = Resources.Load<Texture2D>( "GUI/Toggle" );
				m_Toggle.hover.textColor = new Color( 1f, 0.8f, 0f );

				m_Toggle.active.background = Resources.Load<Texture2D>( "GUI/Toggle" );
				m_Toggle.active.textColor = Color.grey;

				m_Toggle.focused.background = Resources.Load<Texture2D>( "GUI/Toggle" );
				m_Toggle.focused.textColor = Color.white;

				m_Toggle.onNormal.background = Resources.Load<Texture2D>( "GUI/ToggleOn" );
				m_Toggle.onNormal.textColor = Color.white;

				m_Toggle.onHover.background = Resources.Load<Texture2D>( "GUI/ToggleOn" );
				m_Toggle.onHover.textColor = new Color( 1f, 0.8f, 0f );

				m_Toggle.onActive.background = Resources.Load<Texture2D>( "GUI/ToggleOn" );
				m_Toggle.onActive.textColor = Color.grey;

				m_Toggle.onFocused.background = Resources.Load<Texture2D>( "GUI/ToggleOn" );
				m_Toggle.onFocused.textColor = Color.white;

				m_Toggle.border = new RectOffset( 20, 0, 20, 0 );
				m_Toggle.overflow = new RectOffset( 0, 0, 0, 0 );
				m_Toggle.padding = new RectOffset( 24, 0, 0, 0 );

				m_Toggle.fixedHeight = 22;
			}

			return m_Toggle;
		}
	}
}
