using UnityEngine;
using System.Collections;
using ExitGames.Client.Photon;

public class WulfNetworking : Photon.MonoBehaviour {

	//SoldierController wulfSolderController;
	CharacterController wulfCharacterController;
	//CharacterMotor wulfCharacterMotor;

	void Awake () {
		//wulfSolderController = GetComponent<SoldierController>();
		wulfCharacterController = GetComponent<CharacterController>();
		//wulfCharacterMotor = GetComponent<CharacterMotor>();

		if (photonView.isMine)
		{
			//MINE: local player, simply enable the local scripts
			//cameraScript.enabled = true;
			//controllerScript.enabled = true;
		}
		else
		{           
			Object.Destroy (GetComponent("SoldierController"));
			//wulfCharacterController.enabled = false;
			//Object.Destroy (GetComponent("CharacterMotor"));
			Object.Destroy (transform.FindChild("bluetank/Soldier Camera").gameObject);
			Object.Destroy (transform.FindChild("bluetank/Soldier Camera").gameObject);

		}
		
		gameObject.name = gameObject.name + photonView.viewID;

	}


	// Use this for initialization
	void Start () {
	
	}
	
	void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
	{
		if (stream.isWriting)
		{
			//We own this player: send the others our data
			//stream.SendNext((int)controllerScript._characterState);
			stream.SendNext(transform.position);
			stream.SendNext(transform.rotation); 
		}
		else
		{
			//Network player, receive data
			//controllerScript._characterState = (CharacterState)(int)stream.ReceiveNext();
			correctPlayerPos = (Vector3)stream.ReceiveNext();
			correctPlayerRot = (Quaternion)stream.ReceiveNext();
		}
	}
	
	private Vector3 correctPlayerPos = Vector3.zero; //We lerp towards this
	private Quaternion correctPlayerRot = Quaternion.identity; //We lerp towards this
	
	void Update()
	{
		if (!photonView.isMine)
		{
			//Update remote player (smooth this, this looks good, at the cost of some accuracy)
			transform.position = Vector3.Lerp(transform.position, correctPlayerPos, Time.deltaTime * 5);
			transform.rotation = Quaternion.Lerp(transform.rotation, correctPlayerRot, Time.deltaTime * 5);
		}
	}

}
