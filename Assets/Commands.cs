using UnityEngine;
using System.Collections;

public class Commands : Photon.MonoBehaviour {
	private float i = 0.0F;
	private string name;
	GameObject player;

	// Use this for initialization
	public void Command(string com){
		Debug.Log (com);
		string[] splitstring = com.Split (',');
		if (splitstring.Length > 0){
			Debug.Log (splitstring [1]);
		}
		i = float.Parse(splitstring [1]);
		if (splitstring[0] == "/speed" && i > 0 || i < 1000 ) {
			Debug.Log(name + "" + splitstring[0] + " " + i);
			//controllerScript = player.GetComponent<RigidbodyFPSController>();
			//controllerScript.speed = i;
		}
		if (splitstring[0] == "/test") {
			Debug.Log(name + "");
			PhotonNetwork.Instantiate("bluegt", new Vector3 (transform.position.x, transform.position.y + 1.4f, transform.position.z), Quaternion.identity, 0);
		}
	}
	public void Player(string PlayerUserName){
		name = PlayerUserName;
		//player = PhotonView.Find (PlayerUserName);
	}
}

