using UnityEngine;
using System.Collections;

public class PlayerName : Photon.MonoBehaviour {
	
	string PlayerNamer;
	
	void OnGUI(){
		PlayerNamer = GUI.TextField(new Rect(Screen.width / 2 - 50, Screen.height / 2, 100, 20), PlayerNamer);
	}
	
	void Update () {
		PhotonNetwork.player.name = PlayerNamer;
	}
}