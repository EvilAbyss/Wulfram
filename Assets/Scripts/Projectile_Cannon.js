#pragma strict
var mySpeed : float = 10;
var myRange : float = 10;

private var myDist : float;

function Update () 
{
	 transform.Translate(Vector3.forward * Time.deltaTime * mySpeed);
	 myDist += Time.deltaTime * mySpeed;
	 if(myDist >= myRange)
		Destroy(gameObject);
}

