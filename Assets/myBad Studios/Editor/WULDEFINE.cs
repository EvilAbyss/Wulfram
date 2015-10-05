using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

/* This script only creates a DEFINE to let other kits know this kit exists
 * It is not included in your final product so you need not concern yourself with it
 * unless you are a plugin developer who needs to test for the existence of this kit
 * This functionality is only available in Unity 4.x
 */
#if !(UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0	|| UNITY_3_0_0 || UNITY_3_1	|| UNITY_3_2 || UNITY_3_3 || UNITY_3_4 || UNITY_3_5)

[InitializeOnLoad]
public class WULDEFINE
{
	static WULDEFINE()
	{
		// run through all the build targets and add the define symbol if needed
		foreach (BuildTargetGroup btg in System.Enum.GetValues(typeof(BuildTargetGroup)))
		{
			// extract existing defines to check if the ones to be added
			// does not allready exist before adding the new defines
			if (btg == BuildTargetGroup.Unknown) continue;
			string defines_field = PlayerSettings.GetScriptingDefineSymbolsForGroup(btg);
			List<string> defines = new List<string>(defines_field.Split(';'));
			if (!defines.Contains("WUL"))
			{
				defines.Add("WUL");
				PlayerSettings.SetScriptingDefineSymbolsForGroup(btg, string.Join(";", defines.ToArray()));
			}
		}	
	}
}
#endif
