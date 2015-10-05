using UnityEngine;
using System.Collections;

/// <summary>
/// This component is used in the Level1Offline scene to setup a local game
/// </summary>
public class OfflineMode : MonoBehaviour
{
	void Awake()
	{
		//When photon is in offline mode, several behaviours are changed to simulate the server environment
		//For example: PhotonNetwork.time now returns the Unity time. This is helpful so we don't have to
		//create branching code for online and offline games wherever we are using Photon
		PhotonNetwork.offlineMode = true;

		//The player ship is already placed in the scene so we are setting up the connection to the camera here
		GetComponent<CameraShipFollow>().SetTarget( GameObject.Find( "Ship" ).GetComponent<Ship>() );
	}
}