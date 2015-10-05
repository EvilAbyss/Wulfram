using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using MBS;

public class EncoderTest : MonoBehaviour {

	string	field = "",
			label = "";

	void OnGUI()
	{
		field = GUI.TextField(new Rect(50,10,250,25), field);
		if ( GUI.Button(new Rect(320, 10, 70, 25)  , "Submit") )
		{
			string based = Encoder.Base64Encode(field);
			label = "Source: " + field + "\nMD5: " + Encoder.MD5(field) + "\nBase64: " + based + "\nDecoded is: " + Encoder.Base64Decode(based);
			field = string.Empty;
		}
		GUI.TextArea(new Rect(50,50,340, 90), label);
	}
	
}