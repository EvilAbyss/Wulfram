#pragma strict
#pragma implicit
#pragma downcast

enum FireType
{
	RAYCAST,
	PHYSIC_PROJECTILE,
}

enum FireMode
{
	SEMI_AUTO,
	FULL_AUTO,
	BURST
}

class Gun extends UnityEngine.MonoBehaviour
{
    public var Local : boolean;
	public var gunName : String;
	public var bulletMark : GameObject;
	public var projectilePrefab : GameObject;
	public var Point : Vector3;
	public var OutputPoint : Vector3;
	public var weaponTransformReference : Transform;
	
	public var hitLayer : LayerMask;
	
	public var woodParticle : GameObject;
	public var metalParticle : GameObject;
	public var concreteParticle : GameObject;
	public var sandParticle : GameObject;
    public var bloodParticle : GameObject;
	public var waterParticle : GameObject;

	//How many shots the gun can take in one second
	public var fireRate : float;
	public var useGravity : boolean;
	private var fireType : FireType;
	public var fireMode : FireMode;
	
	//Number of shoots to fire when on burst mode
	public var burstRate : int;
	
	//Range of fire in meters
	public var fireRange : float;
	
	//Speed of the projectile in m/s
	public var projectileSpeed : float;
	
	public var clipSize : int;
	public var totalClips : int;
	
	//Time to reload the weapon in seconds
	public var reloadTime : float;
	public var autoReload : boolean;
	public var currentRounds : int;
	
	public var shootVolume : float = 0.4;
	public var shootSound : AudioClip;
	private var shootSoundSource : AudioSource;
	
	public var reloadSound : AudioClip;
	private var reloadSoundSource : AudioSource;
	
	public var outOfAmmoSound : AudioClip;
	private var outOfAmmoSoundSource : AudioSource;
	
	private var reloadTimer : float;
	
	@HideInInspector
	public var freeToShoot : boolean;
	
	@HideInInspector
	public var reloading : boolean;
	public var oldreloading : boolean;
	private var lastShootTime : float;
	private var shootDelay : float;
	private var cBurst : int;
	
	//@HideInInspector
	public var fire : boolean;
	public var fireRemote : boolean;
	public var hitParticles : GameObject;
	
	public var shotingEmitter : GunParticles;
	private var shottingParticles : Transform;
	
	public var capsuleEmitter : ParticleEmitter[];
	
	public var shotLight : ShotLight;
	
	public var unlimited : boolean = true;
	
	private var timerToCreateDecal : float;
	
	public var pushPower : float = 3.0;
	
	public var soldierCamera : SoldierCamera;
	public var Soldier: GameObject;
	public var cam : Camera;
	
	function SetPoint(_point : Vector3)
	{
	    Point  = _point;
	}
	
	
	function OnDisable()
	{
		if(shotingEmitter != null)
		{
			shotingEmitter.ChangeState(false);
		}
		
		if(capsuleEmitter != null)
		{
			for(var i : int = 0; i < capsuleEmitter.Length; i++)
			{
				if (capsuleEmitter[i] != null)
                    capsuleEmitter[i].emit = false;
			}
		}
		
		if(shotLight != null)
		{
			shotLight.enabled = false;
		}
	}
	
	function OnEnable()
	{
		if(Local)cam = soldierCamera.GetComponent.<Camera>();
		
		reloadTimer = 0.0;
		reloading = false;
		freeToShoot = true;
		shootDelay = 1.0 / fireRate;
		
		cBurst = burstRate;
		
		totalClips--;
		currentRounds = clipSize;
		
		if(projectilePrefab != null)
		{
			fireType = FireType.PHYSIC_PROJECTILE;
		}
		
		if(shotLight != null)
		{
			shotLight.enabled = false;
		}
		
		shottingParticles = null;
		if(shotingEmitter != null)
		{
			for(var i : int = 0; i < shotingEmitter.transform.childCount; i++)
			{
				if(shotingEmitter.transform.GetChild(i).name == "bullet_trace")
				{
					shottingParticles = shotingEmitter.transform.GetChild(i);
					break;
				}
			}
		}
	}
	
	function ShotTheTarget()
	{		
	

		if(fire && !reloading)
		{
	    if(!Local)
		{
            switch(fireType)
			{
				case FireType.RAYCAST:
				    
					CheckRaycastHit();
					
					break;
				case FireType.PHYSIC_PROJECTILE:
				
					LaunchProjectile();
					
					break;
			}
		}
		
		
		
		
		
			if(currentRounds > 0)
			{
				if(Time.time > lastShootTime && freeToShoot && cBurst > 0)
				{
					lastShootTime = Time.time + shootDelay;
			
					switch(fireMode)
					{
						case FireMode.SEMI_AUTO:
							freeToShoot = false;
							break;
						case FireMode.BURST:
							cBurst--;
							break;
					}
					
					if(capsuleEmitter != null)
					{
						for(var i : int = 0; i < capsuleEmitter.Length; i++)
						{
							capsuleEmitter[i].Emit();
						}
					}
					
					PlayShootSound();
					
					if(shotingEmitter != null)
					{
						shotingEmitter.ChangeState(true);
						
					}
					
					if(shotLight != null)
					{
						shotLight.enabled = true;
					}
					var h : Hashtable;
					
					switch(fireType)
					{
						case FireType.RAYCAST:
							
							if(Local)
                            {
                                TrainingStatistics.shootsFired++;
                                CheckRaycastHit();
                            
                                h = new Hashtable();
                                h.Add(1,(cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5, Screen.height * 0.5, cam.farClipPlane))));
                                h.Add(2,fire);
								
								Soldier.SendMessage("Fire",h, SendMessageOptions.DontRequireReceiver);
                            }
							break;
						case FireType.PHYSIC_PROJECTILE:
							
                            if(Local)
                            {
                                TrainingStatistics.grenadeFired++;
							    LaunchProjectile();
                                h = new Hashtable();
                                h.Add(1,(cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5, Screen.height * 0.55, 40)) ));
                                h.Add(2,fire);
                                h.Add(3,weaponTransformReference.position);
								
								Soldier.SendMessage("Fire",h, SendMessageOptions.DontRequireReceiver);
                            }
                            fire=false;
							break;
					}
					
					if(Local)
					{
					    currentRounds--;
					}
					if(currentRounds <= 0)
					{
						Reload();
					}
				}
			}
			else if(autoReload && freeToShoot)
			{
				if(shotingEmitter != null)
				{
					shotingEmitter.ChangeState(false);
				}
				
				if(shotLight != null)
				{
					shotLight.enabled = false;
				}
				
				if(!reloading)
				{
					Reload();
				}
			}
		}
		else
		{
			if(shotingEmitter != null)
			{
				shotingEmitter.ChangeState(false);
			}
			
			if(shotLight != null)
			{
				shotLight.enabled = false;
			}
		}
	}
	
	function LaunchProjectile()
	{
		//Get the launch position (weapon related)
		if(Local)var camRay : Ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5, Screen.height * 0.6, 0));
		var dir : Vector3;
		var startPosition : Vector3;
        var origin : Vector3;
				
		if(weaponTransformReference != null)
		{
			if(Local)
			{
			    startPosition = weaponTransformReference.position;
			}
			else
			{
			    startPosition = OutputPoint;
			}
		}
		else
		{		
			if(Local)startPosition = cam.ScreenToWorldPoint(new Vector3 (Screen.width * 0.5, Screen.height * 0.5, 0.5));
		}
		
		var projectile : GameObject = GameObject.Instantiate(projectilePrefab, startPosition, Quaternion.identity);
		
		var grenadeObj : Grenade = projectile.GetComponent("Pulse") as Grenade;
		
		if(Local)
		{
    	    grenadeObj.soldierCamera = soldierCamera;
		}
		
		grenadeObj.Local = Local;
		
	
		
		var projectileRigidbody : Rigidbody = projectile.GetComponent.<Rigidbody>();
		
		if(projectile.GetComponent.<Rigidbody>() == null)
		{
			projectileRigidbody = projectile.AddComponent.<Rigidbody>();	
		}
		projectileRigidbody.useGravity = useGravity;
		

			if(Local)
			{
                Point = (cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5, Screen.height * 0.55, 40)) );

                
			    projectileRigidbody.velocity = (Point - weaponTransformReference.position).normalized * projectileSpeed;
			}
			else
			{
			   
    			projectileRigidbody.velocity = (Point - weaponTransformReference.position).normalized * projectileSpeed;
			}

	}
	
	
	
	
	
	
	function CheckRaycastHit()
	{
		var hit : RaycastHit;
		var glassHit : RaycastHit;
		var camRay : Ray;
		var origin : Vector3;
		var glassOrigin : Vector3;
		var dir : Vector3;
		var glassDir : Vector3;
		
		if(weaponTransformReference == null)
		{
			camRay = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5, Screen.height * 0.5, 0));
			origin = camRay.origin;
			dir = camRay.direction;
			origin += dir * 0.1;
		}
		else
		{
		    if(Local)
		    {
			    camRay = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5, Screen.height * 0.5, 0));
    			  
			    origin = weaponTransformReference.position + (weaponTransformReference.right * 0.2);
    			
			    if(Physics.Raycast(camRay.origin + camRay.direction * 0.1, camRay.direction, hit, fireRange, hitLayer))
			    {
				    dir = (hit.point - origin).normalized;
    				
				    if(hit.collider.tag == "glass")
				    {
					    glassOrigin = hit.point + dir * 0.05;
    					
					    if(Physics.Raycast(glassOrigin, camRay.direction, glassHit, fireRange - hit.distance, hitLayer))
					    {
						    glassDir = glassHit.point - glassOrigin;
					    }
				    }
			    }
			    else
			    {
				    dir = weaponTransformReference.forward;
			    }
			}
			else
			{
			    origin = weaponTransformReference.position + (weaponTransformReference.right * 0.2);
    			dir = weaponTransformReference.forward;
			}
		}
		
		if(shottingParticles != null)
		{
		    if(Local)
		    {
		        Point = (cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5, Screen.height * 0.5, cam.farClipPlane)));
                
			    shottingParticles.rotation = Quaternion.FromToRotation(Vector3.forward, (cam.ScreenToWorldPoint(new Vector3(Screen.width * 0.5, Screen.height * 0.5, cam.farClipPlane)) - weaponTransformReference.position).normalized);
			}
		}
		
		if(Physics.Raycast(origin, dir, hit, fireRange, hitLayer))
		{
		    //hit.collider.gameObject.SendMessage("HitSoldier", "Dummy", SendMessageOptions.DontRequireReceiver);
			hit.collider.gameObject.SendMessage("Hit", hit, SendMessageOptions.DontRequireReceiver);
			if(Local == true)
			{
			    hit.collider.gameObject.SendMessage("HitSoldier","Player", SendMessageOptions.DontRequireReceiver);
			}
			GenerateGraphicStuff(hit);
			
			if(hit.collider.tag == "glass")
			{
				if(Physics.Raycast(glassOrigin, glassDir, glassHit, fireRange - hit.distance, hitLayer))
				{
					glassHit.collider.gameObject.SendMessage("Hit", glassHit, SendMessageOptions.DontRequireReceiver);
					GenerateGraphicStuff(glassHit);
				}
			}
		}
	}
	
	function GenerateGraphicStuff(hit : RaycastHit)
	{
		var hitType : HitType;
		
		var body : Rigidbody = hit.collider.GetComponent.<Rigidbody>();
		if(body == null)
		{
			if(hit.collider.transform.parent != null)
			{
				body = hit.collider.transform.parent.GetComponent.<Rigidbody>();
			}
		}
		
		if(body != null)
		{
			if(body.gameObject.layer != 10 && !body.gameObject.name.ToLower().Contains("door"))
			{
				body.isKinematic = false;
			}
		
			if(!body.isKinematic)
			{
    				var direction : Vector3 = hit.collider.transform.position - weaponTransformReference.position;
				body.AddForceAtPosition(direction.normalized * pushPower, hit.point, ForceMode.Impulse);
			}
		}
		
		var go : GameObject;
		
		var delta : float = -0.02;
		var hitUpDir : Vector3 = hit.normal;
		var hitPoint : Vector3 = hit.point + hit.normal * delta;
		
		switch(hit.collider.tag)
		{
			case "wood":
				hitType = HitType.WOOD;
				go = GameObject.Instantiate(woodParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			case "metal":
				hitType = HitType.METAL;
				go = GameObject.Instantiate(metalParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			case "car":
				hitType = HitType.METAL;
				go = GameObject.Instantiate(metalParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			case "concrete":
				hitType = HitType.CONCRETE;
				go = GameObject.Instantiate(concreteParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			case "dirt":
				hitType = HitType.CONCRETE;
				go = GameObject.Instantiate(sandParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			case "EnemyPlayer":
				hitType = HitType.CONCRETE;
				go = GameObject.Instantiate(bloodParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				
				break;
			case "sand":
				hitType = HitType.CONCRETE;
				go = GameObject.Instantiate(sandParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			case "water":
				go = GameObject.Instantiate(waterParticle, hitPoint, Quaternion.FromToRotation(Vector3.up, hitUpDir)) as GameObject;
				break;
			default:
				return;
		}
		
		go.layer = hit.collider.gameObject.layer;
		
		if(hit.collider.GetComponent.<Renderer>() == null) return;
		
		if(timerToCreateDecal < 0.0 && hit.collider.tag != "water")
		{
			go = GameObject.Instantiate(bulletMark, hit.point, Quaternion.FromToRotation(Vector3.forward, -hit.normal));
			var bm : BulletMarks = go.GetComponent("BulletMarks");
			bm.GenerateDecal(hitType, hit.collider.gameObject);
			timerToCreateDecal = 0.02;
		}
	}
	
	function Update()
	{
	    if(Local)
	    {
		    timerToCreateDecal -= Time.deltaTime;
    		
		    if(Input.GetButtonDown("Fire1") && currentRounds == 0 && !reloading && freeToShoot)
		    {
			    PlayOutOfAmmoSound();
		    }
    		
		    if(Input.GetButtonUp("Fire1"))
		    {
                var h : Hashtable;
                h = new Hashtable();
                h.Add(1,Point);
                h.Add(2,fire);
 
			    Soldier.SendMessage("Fire",h, SendMessageOptions.DontRequireReceiver);
				
			    freeToShoot = true;
			    cBurst = burstRate;
		    }
    		
		    HandleReloading();
    		
		    ShotTheTarget();
		}
		
		else
		{
		        ShotTheTarget();
		}
	}
	
	function HandleReloading()
	{
		if(Input.GetKeyDown(KeyCode.R) && !reloading)
		{
			Reload();
		}
		
		if(reloading)
		{
		    oldreloading = reloading;


                
			reloadTimer -= Time.deltaTime;
			
            if(reloadTimer <= 2.3)
			{                
			    var h : Hashtable;
                h = new Hashtable();
                h.Add("reload",reloading);
                h.Add(1,Point);
                h.Add(2,fire);
				
				Soldier.SendMessage("Fire",h, SendMessageOptions.DontRequireReceiver);
			}
			
			if(reloadTimer <= 0.0)
			{
				reloading = false;
				
				if(!unlimited)
				{
					totalClips--;
				}
				currentRounds = clipSize;
			}
			
		}
		else 
		{
		    if (oldreloading != reloading)
            {
            
                h = new Hashtable();
                h.Add("reload",reloading);
                h.Add(1,Point);
                h.Add(2,fire);
				
				Soldier.SendMessage("Fire",h, SendMessageOptions.DontRequireReceiver);
                oldreloading = reloading;
            }
            
		}
	}
	
	function Reload()
	{
		if(totalClips > 0 && currentRounds < clipSize)
		{
			PlayReloadSound();
			reloading = true;
			reloadTimer = reloadTime;
		}
	}
	
	//---------------AUDIO METHODS--------
	function PlayOutOfAmmoSound()
	{
		GetComponent.<AudioSource>().PlayOneShot(outOfAmmoSound, 1.5);
	}
	
	function PlayReloadSound()
	{
		GetComponent.<AudioSource>().PlayOneShot(reloadSound, 1.5);
	}
	
	function PlayShootSound()
	{
		GetComponent.<AudioSource>().PlayOneShot(shootSound);
	}
}