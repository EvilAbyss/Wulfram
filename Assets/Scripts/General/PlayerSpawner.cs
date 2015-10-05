using UnityEngine;
using System.Collections;

/// <summary>
/// Creates the synchronized ship objects
/// </summary>
public class PlayerSpawner : MonoBehaviour
{
	void Start()
	{
		//if we are not connected, than we probably pressed play in a level in editor mode.
		//In this case go back to the main menu to connect to the server first
		if( PhotonNetwork.connected == false )
		{
			Application.LoadLevel( "MainMenu" );
			return;
		}
	}

	public void CreateLocalPlayer( Team team )
	{
		object[] instantiationData = new object[] { (int)team } ;

		//Notice the differences from PhotonNetwork.Instantiate to Unitys GameObject.Instantiate
		GameObject newShipObject = PhotonNetwork.Instantiate( 
			"Ship", 
			Vector3.zero, 
			Quaternion.identity, 
			0,
			instantiationData
		);

		Transform spawnPoint = GamemodeManager.CurrentGamemode.GetSpawnPoint( team );
		newShipObject.transform.position = spawnPoint.transform.position;
		newShipObject.transform.rotation = spawnPoint.transform.rotation;

		Ship newShip = newShipObject.GetComponent<Ship>();
		newShip.SetTeam( team );

		//Since this function is called on every machine to create it's one and only local player, the new ship is always the camera target
		Camera.main.GetComponent<CameraShipFollow>().SetTarget( newShip );
	}

}