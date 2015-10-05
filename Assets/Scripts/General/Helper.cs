using UnityEngine;
using System.Collections;

public class Helper 
{
	public static T GetCachedComponent<T>( GameObject gameObject, ref T cachedComponent ) where T : MonoBehaviour
	{
		if( cachedComponent == null )
		{
			cachedComponent = gameObject.GetComponent<T>();
		}

		return cachedComponent;
	}

	public static T GetCustomProperty<T>( PhotonView view, string property, T offlineValue, T defaultValue )
	{
		//If in offline mode, return the value from the local variable
		if( PhotonNetwork.offlineMode == true )
		{
			return offlineValue;
		}
		//In online mode, use the players custom properties. This enables
		//other players to see this stat as well
		else
		{
			//Check if the KillCount property already exist
			if( view != null && 
				view.owner != null && 
				view.owner.customProperties.ContainsKey( property ) == true )
			{
				return (T)view.owner.customProperties[ property ];
			}

			//If not, no kills have been registered yet, return 0
			return defaultValue;
		}
	}

	public static void SetCustomProperty<T>( PhotonView view, string property, ref T offlineVariable, T value )
	{
		//If in offline mode, store the value in a local variable
		if( PhotonNetwork.offlineMode == true )
		{
			offlineVariable = value;
		}
		else
		{
			//Photon has it's own Hashtable class in order to ensure that the data
			//can be synchronized between all platforms
			ExitGames.Client.Photon.Hashtable properties = new ExitGames.Client.Photon.Hashtable();
			properties.Add( property, value );

			//Use the SetCustomProperties function to set new values and update existing ones
			//This function saves the data locally and sends synchronize operations so that every
			//client receives the update as well. 
			//Don't set PhotonView.owner.customProperties directly as it wouldn't be synchronized
			view.owner.SetCustomProperties( properties );
		}
	}
}
