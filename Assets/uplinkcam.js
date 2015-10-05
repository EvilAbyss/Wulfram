#pragma strict

var cam1 : Camera;

 
function Start() {
    cam1.enabled = false;
   
}
 
function Update() {
 
    if (Input.GetKeyDown(KeyCode.V)) {
        cam1.enabled = !cam1.enabled;
        
    }
 
}