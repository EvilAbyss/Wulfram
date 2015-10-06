using UnityEngine;
using System.Collections;

public class Playernaming : Photon.MonoBehaviour {
	//public TextMesh text;
	void OnGUI()
	{
		if (photonView.isMine == false){
		Vector3 offset = new Vector3(0, 0, 0); // height above the target position
		
		Vector3 point = Camera.main.WorldToScreenPoint(transform.position + offset);
		point.y = Screen.height - point.y;

		//text.text = photonView.owner.name;
		GUI.TextArea(new Rect (point.x - 35, point.y - 20, 150, 20), GetComponent<PhotonView> ().owner.name);

		         
	}else 
		{
			Vector3 offset = new Vector3(0, 0, 0); // height above the target position
			Camera myCamera = GameObject.FindWithTag("NameTagCamera").GetComponent<Camera>();
			Vector3 point = myCamera.WorldToScreenPoint(transform.position + offset);
			point.y = Screen.height - point.y;
			GUI.TextArea(new Rect (point.x - 35, point.y - 20, 150, 20), GetComponent<PhotonView> ().owner.name);

			Debug.Log ("nothing");	//nothing
}
	}
}