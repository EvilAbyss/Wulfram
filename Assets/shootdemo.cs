


using UnityEngine;
using System.Collections;

public class shootdemo : MonoBehaviour
{
	public RaycastHit hit;
	public int WeaponRange;
	public GameObject GunParticle;
	public AudioClip GunShotSound;
	public Transform SpawnLocation;
	PhotonView pv;
	
	
	
	
	// Use this for initialization
	void Start () {
		
		
		pv = this.GetComponent<PhotonView>();
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
		
		if(Input.GetMouseButtonUp(0)&& pv.isMine)
		{
			pv.RPC("Shoot", PhotonTargets.All);
		}
	}

	
	
	
	[RPC]
	void Shoot()
	{
		Vector3 fwd = transform.TransformDirection(Vector3.forward * 10.0f);
		if (Physics.Raycast(SpawnLocation.position, fwd, out hit, WeaponRange))
		{
			GetComponent<AudioSource>().PlayOneShot (GunShotSound);
		Instantiate(GunParticle, SpawnLocation.transform.forward * 100 * Time.deltaTime, Quaternion.identity);
		

		};
	}
}

