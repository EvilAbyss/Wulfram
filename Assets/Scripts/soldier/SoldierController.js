#pragma strict
#pragma implicit
#pragma downcast

class SoldierController extends MonoBehaviour
{
	// Public variables shown in inspector
 
	public var runSpeed : float = 4.6;
	public var runStrafeSpeed : float = 3.07;
	public var walkSpeed : float = 4.6;
	public var walkStrafeSpeed : float = 3.07;
	public var crouchRunSpeed : float = 4.6;
	public var crouchRunStrafeSpeed : float = 5;
	public var crouchWalkSpeed : float = 4.6;
	public var crouchWalkStrafeSpeed : float = 3.07;

    public var radarObject : GameObject;
	
	public var maxRotationSpeed : float = 540;
	
	public var weaponSystem : GunManager;
	public var minCarDistance : float;
	
	static public var dead : boolean;
	
	// Public variables hidden in inspector
	
	//@HideInInspector
	public var walk : boolean;
	public var oldwalk : boolean;
	//@HideInInspector
	public var crouch : boolean;
	
	///@HideInInspector
	public var inAir : boolean;
	public var oldinAir : boolean;
	//@HideInInspector
	public var fire : boolean;
	
	//@HideInInspector
	public var aim : boolean;
	public var oldaim : boolean;
	//@HideInInspector
	public var reloading : boolean;
	
	//@HideInInspector
	public var currentWeaponName : String;
	
	///@HideInInspector
	public var currentWeapon : int;
	
	///@HideInInspector
	public var grounded : boolean;
	
	///@HideInInspector
	public var targetYRotation : float;
	
	// Private variables
	
	private var soldierTransform : Transform;
	private var controller : CharacterController;
	//private var headLookController : HeadLookController;
	private var motor : CharacterMotor;
	
	private var firing : boolean;
	private var oldfiring : boolean;
	private var firingTimer : float;
	public var idleTimer : float;
	
	public var enemiesRef : Transform;
	public var enemiesShootRef : Transform;
	
	static public var enemiesReference : Transform;
	static public var enemiesShootReference : Transform;
	
	//@HideInInspector
	public var moveDir : Vector3;
    
    private var _useIK : boolean;

	function Awake()
	{   
		if(enemiesRef != null) enemiesReference = enemiesRef;
		if(enemiesShootRef != null) enemiesShootReference = enemiesShootRef;
	}
	
	function Start()
	{
		// Setup _GameManger references and activate _GameManager
		//GameObject.Find("/_GameManager").SetActive(true);		
		//GameObject.Find("/_GameManager/SoldierTargetHUD").GetComponent.<SoldierTarget>().soldierController = this;
		
		idleTimer = 0.0;

		soldierTransform = transform;

		walk = true;
		aim = false;
		reloading = false;
	
		controller = gameObject.GetComponent("CharacterController");
		motor = gameObject.GetComponent("CharacterMotor");
	}
	
	function OnEnable()
	{
        if(radarObject != null)
        { 
           	#if UNITY_3_5
           		radarObject.SetActiveRecursively(true);  	
           	#endif
           	#if UNITY_4
           		radarObject.SetActive(true);
           	#endif  	
        }

        moveDir = Vector3.zero;
		//headLookController = gameObject.GetComponent("HeadLookController");
		//headLookController.enabled = true;
        walk = true;
		aim = false;
		reloading = false;
	}
	
	function OnDisable()
	{
        if(radarObject != null)
        { 
            #if UNITY_3_5
           		radarObject.SetActiveRecursively(false);  	
           	#endif
           	#if UNITY_4
           		radarObject.SetActive(false);
           	#endif
        }

        moveDir = Vector3.zero;
		//headLookController.enabled = false;
        walk = true;
		aim = false;
		reloading = false;
	}
	
	function Update()
	{
		if(GameManager.showMenu || GameManager.scores || GameManager.chat)
		{
			moveDir = Vector3.zero;
			weaponSystem.currentGun.fire = false;
			fire = false;
			motor.canControl = false;
		}
		else
		{
			GetUserInputs();
			
			if(!motor.canControl)
			{
				motor.canControl = true;
			}
			
			if(!dead)
			{
				moveDir = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
			}
			else
			{
				moveDir = Vector3.zero;
				motor.canControl = false;
			}
		}
		
		//Check the soldier move direction
		if (moveDir.sqrMagnitude > 1)
			moveDir = moveDir.normalized;
		
		motor.inputMoveDirection = transform.TransformDirection(moveDir);
		motor.inputJump = Input.GetButton("Jump");
		
		motor.movement.maxForwardSpeed = ((walk) ? ((crouch) ? crouchWalkSpeed : walkSpeed) : ((crouch) ? crouchRunSpeed : runSpeed));
	
		if(walk != oldwalk)
		{    
            SendMessage("SetWalk",walk, SendMessageOptions.DontRequireReceiver);
		    oldwalk = walk;
		}
		motor.movement.maxBackwardsSpeed = motor.movement.maxForwardSpeed;
		motor.movement.maxSidewaysSpeed = ((walk) ? ((crouch) ? crouchWalkStrafeSpeed : walkStrafeSpeed) : ((crouch) ? crouchRunStrafeSpeed : runStrafeSpeed));
		
		if(moveDir != Vector3.zero)
        {
			idleTimer = 0.0;
        }
		
		inAir = !motor.grounded;
		if(oldinAir != inAir)
		{
		    SendMessage("SetInAir",inAir, SendMessageOptions.DontRequireReceiver);
		    oldinAir = inAir;
		}
		var currentAngle = soldierTransform.localRotation.eulerAngles.y;
		var delta = Mathf.Repeat ((targetYRotation - currentAngle), 360);
		if (delta > 180)
			delta -= 360;
		soldierTransform.localRotation.eulerAngles.y = Mathf.MoveTowards(currentAngle, currentAngle + delta, Time.deltaTime * maxRotationSpeed); 
	}
	
	function GetUserInputs()
	{
		//Check if the user if firing the weapon
		fire = Input.GetButton("Fire1") && weaponSystem.currentGun.freeToShoot && !dead;
	
		//Check if the user is aiming the weapon
		aim = Input.GetButton("Fire2") && !dead;
		if(oldaim != aim)
		{
		    SendMessage("SetAim",aim, SendMessageOptions.DontRequireReceiver);
		    oldaim = aim;
		}
		
		idleTimer += Time.deltaTime;
		
		if(aim || fire)
		{
			firingTimer -= Time.deltaTime;
			idleTimer = 0.0;
		}
		else
		{
		    firingTimer = 0.3;
		}
		
		firing = (firingTimer <= 0.0 && fire);
		
		if(weaponSystem.currentGun != null)
		{
			weaponSystem.currentGun.fire = firing;
			reloading = weaponSystem.currentGun.reloading;
			currentWeaponName = weaponSystem.currentGun.gunName;
			currentWeapon = weaponSystem.currentWeapon;
		}
		
		//Check if the user wants the soldier to crouch
		if(Input.GetKeyDown(KeyCode.LeftControl))
		{
		    crouch = !crouch;
			SendMessage("SetCrouch",crouch, SendMessageOptions.DontRequireReceiver);
			idleTimer = 0.0;
		}

		//crouch |= dead;
		crouch = crouch || dead;
        if(dead)
		{
		    SendMessage("SetCrouch",crouch, SendMessageOptions.DontRequireReceiver);
		}
		//Check if the user wants the soldier to walk
		walk = (!Input.GetKey(KeyCode.LeftShift) && !dead) || moveDir == Vector3.zero || crouch;
	}
}