#pragma strict
var jump : AudioClip;
function Start () {

}

function Update () {
     if(Input.GetButtonDown("Jump")){
        
 
            GetComponent.<AudioSource>().Play();
 
        }   
    }  
