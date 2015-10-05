using UnityEngine;
using System.Collections;

namespace MBS {
	public enum eFieldType		{ Normal, Required,	Email,	RequiredEmail,	Verify,	ReadOnly }
	
	[System.Serializable]
	public class ServerDataCapture 
	{
		public string label;
		public string serverfield;
		public string value;
		public eFieldType fieldType = eFieldType.Normal;
		public int verify = -1;
		public string passwordChar;
		public int length = 15;
		public Rect label_area, value_area;

		public bool IsValidEmailFormat(string s)
		{
			string[] invalids = new string[8]{" ", "?", "|", "&", "%", "!", "<", ">"};
			
			s = s.Trim();
			int atIndex	= s.IndexOf("@");
			int lastAt	= s.LastIndexOf("@");
			int dotCom	= s.LastIndexOf(".");
			
			bool result = true;
			foreach(string str in invalids)
				if (s.IndexOf(str) >= 0)
					result = false;
			
			if (result) result = (atIndex > 0);
			if (result) result = (atIndex == lastAt);
			if (result)	result = (dotCom > atIndex + 1);
			
			return result;
		}

		public void UpdateWWWForm(ref WWWForm data)
		{
			if (fieldType == eFieldType.Verify)
				return;

			data.AddField(serverfield, value);
		}

		public bool ValidInput (ServerDataCapture compare_with = null)
		{
			switch (fieldType)
			{
			case eFieldType.Required:
				if (value == string.Empty)
				{
					StatusMessage.Message = "Field " + label + " is required...";
					return false;
				}
				break;
						
			case eFieldType.RequiredEmail:
				if (value == string.Empty)
				{
					StatusMessage.Message = "Field " + label + " is required...";
					return false;
				}

				if (!IsValidEmailFormat(value))
				{
					StatusMessage.Message = "Invalid email address for field " + label;  
					return false;
				}
				break;
						
			case eFieldType.Email:
				if (value != string.Empty)
					if (!IsValidEmailFormat(value))
				{
					StatusMessage.Message = "Invalid email address for field " + label;  
					return false;
				}
				break;
						
			case eFieldType.Verify:
				if (verify >= 0)
				{
					if (value != compare_with.value)
					{
						StatusMessage.Message = label + " does not match field " + compare_with.label;
						return false;
					}
				}
				break;		
			}
			return true;
		}

	}
	
}
