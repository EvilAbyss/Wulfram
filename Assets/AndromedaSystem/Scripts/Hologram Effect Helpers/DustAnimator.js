var xSpeed : float = 0; 
var ySpeed : float = 0; 
function Update() { 
var offsetX = Time.time * xSpeed; 
var offsetY = Time.time * ySpeed; 
GetComponent.<Renderer>().material.mainTextureOffset = Vector2 (offsetX,offsetY); 
} 