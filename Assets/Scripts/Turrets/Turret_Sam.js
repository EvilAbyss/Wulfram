#pragma strict

var myProjectile : GameObject;
var reloadTime : float = 1f;
var turnSpeed : float = 5f;
var firePauseTime : float = .25f;
var errorAmount : float = .001;
var myTarget : Transform;
var muzzlePositions : Transform[];
var pivot_Tilt : Transform;
var pivot_Pan : Transform;
var aim_Pan : Transform;
var aim_Tilt : Transform;

private var nextFireTime : float;

function Start () 
{

}

function Update () 
{
	if(myTarget)
	{
		aim_Pan.LookAt(myTarget);
		aim_Pan.eulerAngles = Vector3(0, aim_Pan.eulerAngles.y, 0);
		aim_Tilt.LookAt(myTarget);
		
		pivot_Pan.rotation = Quaternion.Lerp(pivot_Pan.rotation, aim_Pan.rotation, Time.deltaTime*turnSpeed);
		pivot_Tilt.rotation = Quaternion.Lerp(pivot_Tilt.rotation, aim_Tilt.rotation, Time.deltaTime*turnSpeed);
		
		if(Time.time >= nextFireTime)
		{
			FireProjectile();
		}
	}
}

function OnTriggerEnter(other : Collider)
{
	if(other.gameObject.tag == "Ship")
	{
		nextFireTime = Time.time+(reloadTime*.5);
		myTarget = other.gameObject.transform;
	}
}

function OnTriggerExit(other : Collider)
{
	if(other.gameObject.transform == myTarget)
	{
		myTarget = null;
	}
}

function FireProjectile()
{
	GetComponent.<AudioSource>().Play();
	nextFireTime = Time.time+reloadTime;
	
	var m : int = Random.Range(0,6);
	var newMissile = Instantiate(myProjectile, muzzlePositions[m].position, muzzlePositions[m].rotation);
	newMissile.GetComponent(Projectile_Missile).myTarget = myTarget;
}