using UnityEngine;
using System.Collections;

namespace MBS {
	public class GUIX : MonoBehaviour {

		static public float screenWidth		= 2048f,
							screenHeight	= 1536f;


		static public void SetScreenSize(float width = 2048f, float height = 1536f)
		{
			screenWidth = width;
			screenHeight = height;
		}

		static public void FixScreenSize()
		{
			GUI.matrix = Matrix4x4.TRS( Vector3.zero, Quaternion.identity, new Vector3(Screen.width / screenWidth, Screen.height / screenHeight, 1f));
		}

		static public void ResetDisplay()
		{
			GUI.matrix = Matrix4x4.identity;
		}

	}
}
