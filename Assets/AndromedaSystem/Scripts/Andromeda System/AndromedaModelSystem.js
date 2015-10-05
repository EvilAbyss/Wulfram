//Andromeda Model System JavaScript Version 1.0 @ Copyright
//Black Horizon Studios

var floatUpSpeed : float = .01; 
var floatDownSpeed : float = .01; 
var shakeIntensity : float = .1;

var hologramModel : GameObject;

var rotateSpeed : float = 0;
private var floatup;

private var flickerSpeed : float;
var minFlicker : float = 0;
var maxFlicker : float = 1;

var offsetX : float; 
var offsetY : float; 
var xSpeed : float = 1; 
var ySpeed : float = 1; 

//Added support for flickering light, light flickering intensity based off static/flicker intensity
var hologramLight : Light;

//Added support for flickering sound
var hologramSound : AudioClip;

var useLight : boolean = false;
var useSound : boolean = false;

function Awake()
{
    GetComponent.<AudioSource>().clip = hologramSound;
    GetComponent.<AudioSource>().enabled = false;
}

function Start()
{
    floatup = false;
    
    if (useSound)
    {
    	GetComponent.<AudioSource>().enabled = true;
    }		
	
	if (hologramModel == null)
		Debug.LogError("You need to apply a model to the Hologram Model slot. The model must contain 1st the the Hologram Static Shader then the Hologram Solid Shader. Refer to the demo scene for an example if needed.");

}

function Update(){

	transform.Rotate(Vector3.right * rotateSpeed);
    transform.Rotate(Vector3.up * rotateSpeed, Space.World);
    
    transform.rotation.eulerAngles.x = 0;
   	transform.rotation.eulerAngles.z = 0;
   	
    offsetX = Time.time * xSpeed; 
    offsetY = Time.time * ySpeed; 

    if(floatup)
        floatingup();
    else if(!floatup)
        floatingdown();
        
    flickerSpeed = Random.Range(minFlicker,maxFlicker);
    
    if (useLight)
    {
		//Added support for flickering light, light flickering intensity based off static/flicker intensity
    	hologramLight.intensity = flickerSpeed;
    }
    
    if (useSound)
    {
	    //Added support for flickering sound, sound volume intensity based off static/flicker intensity
	    if (GetComponent.<AudioSource>().clip == hologramSound)
	    {
	    	if (flickerSpeed > 0.25)
	    	{
	    		GetComponent.<AudioSource>().enabled = true;
	    		GetComponent.<AudioSource>().volume = flickerSpeed + 0.3;
	    		
	    		Delay();
	    	}
	    	
	    	if (flickerSpeed < 0.25)
	    	{
	    		GetComponent.<AudioSource>().enabled = false;
	    	}
	    }
    }

	hologramModel.GetComponent.<Renderer>().material.color.a = flickerSpeed;
	
	if (maxFlicker > 2)
	{
		Debug.LogError("Max flicker amount should not exceed 2");
	}	
	
	if (minFlicker < -1)
	{
		Debug.LogError("Min flicker amount should not go below -1");
	}
	
	hologramModel.GetComponent.<Renderer>().material.mainTextureOffset = Vector2 (offsetX,offsetY); 
        
}
function floatingup(){
    transform.position.y += shakeIntensity * Time.deltaTime;
    yield WaitForSeconds(floatUpSpeed);
    floatup = false;
}

function floatingdown(){
    transform.position.y -= shakeIntensity * Time.deltaTime;;
    yield WaitForSeconds(floatDownSpeed);
    floatup = true;
}

function Delay ()
{
	yield WaitForSeconds(0.05);
	GetComponent.<AudioSource>().enabled = false;
}