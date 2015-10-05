using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace MBS {
	public class Encoder : MonoBehaviour {

		static public string MD5 (string str)
		{
		
			UTF8Encoding encoding = new UTF8Encoding ();
			byte[] bytes = encoding.GetBytes (str);
		
			// encrypt bytes
			MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider ();
			byte[] hashBytes = md5.ComputeHash (bytes);
		
			// Convert the encrypted bytes back to a string (base 16)
			string hashString = "";
		
			for (int i = 0; i < hashBytes.Length; i++)
				hashString += System.Convert.ToString (hashBytes [i], 16).PadLeft (2, "0" [0]);
		
			return hashString.PadLeft (32, "0" [0]);
		}		

		static public string Base64Encode(string source) {
			byte[] bytesToEncode = Encoding.UTF8.GetBytes (source);
			return System.Convert.ToBase64String (bytesToEncode);
		}

		static public string Base64Decode(string source) {
		
			byte[] decodedBytes = System.Convert.FromBase64String (source);
			return Encoding.UTF8.GetString (decodedBytes);
			Debug.Log(source);
		}

	}
}