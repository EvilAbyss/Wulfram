#pragma strict
#pragma implicit
#pragma downcast

class SoldierDamageControl extends MonoBehaviour
{
	public var life : float;
	public var Respawn : Vector3[];
	public var Parent : GameObject;
	public var hitTexture : Texture2D;
	public var blackTexture : Texture2D;
	public var Skin : GUISkin;
	public var Local : boolean;
	
	public var Soldier : GameObject;
	public var Gun : GameObject;
	
	public var KilledStyle : GUIStyle;
	public var ActorNr : int;
		public var ActorName : String;
	public var DisplayKillYou : boolean;
	
	private var hitAlpha : float;
	private var blackAlpha : float;
	private var AtackerName : String;
	private var actorText : GameObject;
	private var actorTextOffset : Vector3 = new Vector3(0,1.0f,0);
	
	private var recoverTime : float;
	
	public var hitSounds : AudioClip[];
	public var dyingSound : AudioClip;
	
	function SetActorNr(DataActor : Hashtable)
	{
	    ActorNr = DataActor["actornr"];
	    ActorName = DataActor["actorname"];
	    if(actorText == null && gameObject.layer != 13)
	    {
            this.actorText = Instantiate(Resources.Load("ActorName"));
		    this.actorText.name = ActorNr+"";
            actorText.transform.localScale = new Vector3(1.4 / 8f, 1.4 / 8f, 1.4 / 8f);
        }
        
	}
	
    function Start()
	{
		SoldierController.dead = false;
		hitAlpha = 0.0;
		blackAlpha = 0.0;
		life = 1.0;
		KilledStyle = Skin.GetStyle("Killed");
	}
	
	function HitSoldierGranade(hit : String)
	{

		if(GameManager.receiveDamage)
		{
            if(!GetComponent.<AudioSource>().isPlaying)
			{
				if(life < 0.5 && (Random.Range(0, 100) < 30))
				{
					GetComponent.<AudioSource>().clip = dyingSound;
				}
				else
				{
					GetComponent.<AudioSource>().clip = hitSounds[Random.Range(0, hitSounds.length)];
				}
				
				GetComponent.<AudioSource>().Play();
			}
		 if(gameObject.layer != 13)
		 { 
		 
                var _usePhoton : GameObject;
                _usePhoton = GameObject.Find("_GameManager/UsePhoton");                
                _usePhoton.SendMessage("DownLifeGranade", ActorNr);
                
		    return;
		 }
		 
			life -= 1.0;

			recoverTime = (1.0 - life) * 10.0;
			
			if(hit == "Dummy")
			{
				TrainingStatistics.dummiesHit++;
			}
			else if(hit == "Turret")
			{
				TrainingStatistics.turretsHit++;
			}
			
			if(life <= 0.0)
			{
			    AtackerName = hit;
				SoldierController.dead = true;
				DisplayKillYou = true;
				
			}
			
		}
	}
	
	
	
	
	function HitSoldier(hit : String)
	{
		if(GameManager.receiveDamage)
		{
		    //To do Enviar reduccion de life
            if(!GetComponent.<AudioSource>().isPlaying)
			{
				if(life < 0.5 && (Random.Range(0, 100) < 30))
				{
					GetComponent.<AudioSource>().clip = dyingSound;
				}
				else
				{
					GetComponent.<AudioSource>().clip = hitSounds[Random.Range(0, hitSounds.length)];
				}
				
				GetComponent.<AudioSource>().Play();
			}
		
		 if(gameObject.layer != 13)
		 { 
 
                var _usePhoton : GameObject;
                _usePhoton = GameObject.Find("_GameManager/UsePhoton");                
                _usePhoton.SendMessage("DownLife",ActorNr);
                
		        return;
		 }
			life -= 0.05;
    		

			
			recoverTime = (1.0 - life) * 10.0;
			
			if(hit == "Dummy")
			{
				TrainingStatistics.dummiesHit++;
			}
			else if(hit == "Turret")
			{
				TrainingStatistics.turretsHit++;
			}			
			if(life <= 0.0)
			{
			    AtackerName = hit;
			    DisplayKillYou = true;
				SoldierController.dead = true;
 	
			}
			
		}
	}
	
	function DisplayCountDown(time : int)
	{
	    yield WaitForSeconds (time);
        DisplayKillYou = false;
	}
	
	function Update()
	{
	
	    if(actorText != null)
	    {
	    
            var textMesh : TextMesh =  actorText.GetComponent(TextMesh);
            textMesh.text = ActorName;
		    // text looking into oposite direction of camera
            this.actorText.transform.position = transform.position + this.actorTextOffset;
            var camDiff : Vector3 = actorText.transform.position - Camera.main.transform.position;
            var lookAt : Vector3 = actorText.transform.position + camDiff;
		    this.actorText.transform.LookAt(lookAt);
	    }
       
		recoverTime -= Time.deltaTime;
		
		if(recoverTime <= 0.0)
		{
			life += life * Time.deltaTime;
			
			life = Mathf.Clamp(life, 0.0, 1.0);
			
			hitAlpha = 0.0;
		}
		else
		{
			hitAlpha = recoverTime / ((1.0 - life) * 10.0);
		}
	
		if(!SoldierController.dead)
		{ 
		    blackAlpha = 0.0;
 
		    return;
		}
		
		
		blackAlpha += Time.deltaTime;
		
		if(blackAlpha >= 1.0)
		{
		    life = 1.0;
		    hitAlpha = 0.0;
		    blackAlpha = 0.0;
		    recoverTime = 0;
            if(SoldierController.dead)
            {
                    var _DeadUsePhoton : GameObject;
                    _DeadUsePhoton = GameObject.Find("_GameManager/UsePhoton");

				   // _DeadUsePhoton.SendMessage("Dead", new Vector3(15,30,-200));		

				    _DeadUsePhoton.SendMessage("Dead", Respawn[Random.Range(0, 2)]);		

            }	
		    SoldierController.dead = false;
		    
		    
		    //Application.LoadLevel(0);
			//Application.LoadLevel(1);
		}
		

		
        if(DisplayKillYou)
        {
            DisplayCountDown(7);
        }
	}
	
	function OnGUI()
	{
        if(DisplayKillYou)
		{
		    if(AtackerName != "Player"){
		        GUI.Label(Rect (Screen.width - 200,60,200,50),AtackerName+" killed you",KilledStyle);
		    }else
		    {
		        GUI.Label(Rect (Screen.width - 200,60,200,50),"killed yourself",KilledStyle);
		    }
		}
		if(!GameManager.receiveDamage) return;
		if(gameObject.layer != 13){ 
		    return;
		}

		var oldColor : Color;
		var auxColor : Color;
		oldColor = auxColor = GUI.color;
		
		auxColor.a = hitAlpha;
		GUI.color = auxColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), hitTexture);
		
		auxColor.a = blackAlpha;
		GUI.color = auxColor;
		GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), blackTexture);
		

		
		GUI.color = oldColor;
	}	
}