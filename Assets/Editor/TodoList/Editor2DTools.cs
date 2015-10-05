using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public static class Editor2DTools
{
	struct Circles
	{
		public int Radius;
		public int Radius2;
		public Color Color;
		public Texture2D Texture;
	}

	private static Dictionary<Color,Texture2D> CachedColors;
	private static List<Circles> CachedCircles;
	
	static Texture2D GetColoredTexture( Color color )
	{
		if( CachedColors == null )
		{
			CachedColors = new Dictionary<Color, Texture2D>();
		}

		if( CachedColors.ContainsKey( color ) == true && CachedColors[ color ] == null )
		{
			CachedColors.Remove( color );
		}

		if( CachedColors.ContainsKey( color ) == false )
		{
			Texture2D newTexture = new Texture2D( 1, 1 );

			newTexture.wrapMode = TextureWrapMode.Repeat;
			newTexture.SetPixel( 0, 0, color );
			newTexture.Apply();

			CachedColors.Add( color, newTexture );
		}

		return CachedColors[ color ];
	}

	public static void DrawLine( Vector2 lineStart, Vector2 lineEnd, Color color, int thickness = 1 )
	{
		Texture2D texture = GetColoredTexture( color );
		Vector2 lineVector = lineEnd - lineStart;

		// The center of the line will always be at the center
		// regardless of the thickness.
		int thicknessOffset = (int)Mathf.Ceil( thickness / 2 );

		if( lineVector.x == 0 )
		{
			GUI.DrawTexture( new Rect( lineStart.x - thicknessOffset, lineStart.y, thickness, lineVector.magnitude ), texture );
			return;
		}

		if( lineVector.y == 0 )
		{
			GUI.DrawTexture( new Rect( lineStart.x, lineStart.y - thicknessOffset, lineVector.magnitude, thickness ), texture );
			return;
		}

		float angle = Mathf.Rad2Deg * Mathf.Atan( lineVector.y / lineVector.x );

		if( lineVector.x < 0 )
		{
			angle += 180;
		}

		if( thickness < 1 )
		{
			thickness = 1;
		}

		GUIUtility.RotateAroundPivot( angle, lineStart );

		GUI.DrawTexture( new Rect( lineStart.x,
								 lineStart.y - thicknessOffset,
								 lineVector.magnitude,
								 thickness ), texture );

		GUIUtility.RotateAroundPivot( -angle, lineStart );
	}

	public static void DrawRect( Rect rect, Color color )
	{
		Texture2D texture = GetColoredTexture( color );

		GUI.DrawTexture( rect, texture );
	}

	public static void DrawRect( Rect rect, Color color, int stroke, Color strokeColor )
	{
		if( stroke <= 0 )
		{
			DrawRect( rect, color );
			return;
		}

		DrawRect( rect, strokeColor );
		DrawRect( new Rect( rect.xMin + stroke, rect.yMin + stroke, rect.width - stroke * 2, rect.height - stroke * 2 ), color );
	}

	public static void DrawCircle( Vector2 center, int radius, Color color, int radius2 = 0 )
	{
		if( CachedCircles == null )
		{
			CachedCircles = new List<Circles>();
		}

		if( radius2 >= radius )
		{
			return;
		}

		Texture2D texture = null;

		for( int i = 0; i < CachedCircles.Count; ++i )
		{
			if( CachedCircles[ i ].Radius == radius
				&& CachedCircles[ i ].Radius2 == radius2
				&& CachedCircles[ i ].Color == color )
			{
				if( CachedCircles[ i ].Texture == null )
				{
					CachedCircles.RemoveAt( i );
					i--;
				}
				else
				{
					texture = CachedCircles[ i ].Texture;
				}
			}
		}

		if( texture == null )
		{
			int width = radius * 2;
			int height = radius * 2;

			texture = new Texture2D( width, height );

			// x*x + y*y = h*h

			Color[] colors = new Color[ width * height ];
			int i = 0;
			float radiusSquare = ( radius - 1 ) * ( radius - 1 );
			float radiusOuterSquare = radius * radius;

			float radius2Square = radius2 * radius2;
			float radius2OuterSquare = ( radius2 + 1 ) * ( radius2 + 1 ); ;

			if( radius2Square == 0 )
			{
				radius2OuterSquare = 0;
			}

			for( int x = 0; x < width; ++x )
			{
				for( int y = 0; y < width; ++y )
				{
					float xDistance = x - radius;
					float yDistance = y - radius;
					float distance = xDistance * xDistance + yDistance * yDistance;
					colors[ i ].r = color.r;
					colors[ i ].g = color.g;
					colors[ i ].b = color.b;
					
					if( distance > radiusSquare )
					{
						if( distance > radiusOuterSquare )
						{
							colors[ i ].a = 0f;
						}
						else
						{
							colors[ i ].a = 1 - ( distance - radiusSquare ) / ( radiusOuterSquare - radiusSquare );
						}
					}
					else
					{
						if( radius2Square > 0 && distance < radius2OuterSquare )
						{
							if( distance < radius2Square )
							{
								colors[ i ].a = 0f;
							}
							else
							{
								colors[ i ].a = ( distance - radius2Square ) / ( radius2OuterSquare - radius2Square );
							}
						}
						else
						{
							colors[ i ].a = 1f;
						}
					}

					++i;
				}
			}

			texture.SetPixels( colors );
			texture.Apply();

			Circles circle;
			circle.Radius = radius;
			circle.Radius2 = radius2;
			circle.Color = color;
			circle.Texture = texture;

			CachedCircles.Add( circle );
		}

		GUI.DrawTexture( new Rect( center.x - radius
								 , center.y - radius
								 , radius * 2
								 , radius * 2 ), texture );
	}
}