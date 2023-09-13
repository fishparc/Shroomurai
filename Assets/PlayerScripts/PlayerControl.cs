/*
	Created by @DawnosaurDev at youtube.com/c/DawnosaurStudios
	Thanks so much for checking this out and I hope you find it helpful! 
	If you have any further queries, questions or feedback feel free to reach out on my twitter or leave a comment on youtube :D

	Feel free to use this in your own games, and I'd love to see anything you make!
 */

using System;
using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
public class PlayerControl : MonoBehaviour
{
	//Scriptable object which holds all the player's movement parameters. If you don't want to use it
	//just paste in all the parameters, though you will need to manuly change all references in this script
	public PlayerData Data;
	public FireballScript fireball;
	public PushWind pushWind;
	
	#region COMPONENTS
    public Rigidbody2D RB { get; private set; }
	public Animator Anim { get; private set; }
	#endregion

	#region STATE PARAMETERS
	//Variables control the various actions the player can perform at any time.
	//These are fields which can are public allowing for other sctipts to read them
	//but can only be privately written to.
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool IsWallJumping { get; private set; }
	public bool IsDashing { get; private set; }
	public bool IsSliding { get; private set; }
	public bool IsCrouching { get; private set; }

	//Timers (also all fields, could be private and a method returning a bool could be used)
	public float LastOnGroundTime { get; private set; }
	public float LastOnWallTime { get; private set; }
	public float LastOnWallRightTime { get; private set; }
	public float LastOnWallLeftTime { get; private set; }
	public float LastOnMudTime { get; private set; }
	public float LastOnWindTime { get; private set; }
	public float LastPressedJumpTime { get; private set; }
	public float LastPressedDashTime { get; private set; }
	public float LastPressedPushTime { get; private set; }
	//Jump
	private bool _isJumpCut;
	private bool _isJumpFalling;

	//Wall Jump
	private float _wallJumpStartTime;
	private int _lastWallJumpDir;

	//Dash
	private int _dashesLeft;
	private bool _dashRefilling;
	private Vector2 _lastDashDir;
	private bool _isDashAttacking;

	//Slide
	private bool frontWallMud;
	private bool backWallMud;

	//Push
	private float _pushesLeft;
	private bool _pushRefilling;
	//private Collider2D[] airMovableObj;

	//Direction
	private bool FacingUp;
	private bool FacingDown;

	//Effects
	private bool inHorizontalWind;

	#endregion

	#region INPUT PARAMETERS
	public Vector2 _moveInput;
	private bool leftKeyPressed;
	private bool rightKeyPressed;
	private bool downKeyPressed;
	#endregion

	#region CHECK PARAMETERS
	//Set all of these up in the inspector
	[Header("Checks")] 
	[SerializeField] private Transform _groundCheckPoint;
	//Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	[SerializeField] private Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);
	[Space(5)]
	[SerializeField] private Transform _frontWallCheckPoint;
	[SerializeField] private Transform _backWallCheckPoint;
	[SerializeField] private Vector2 _wallCheckSize = new Vector2(0.5f, 1f);
	[Space(5)]
	[SerializeField] private Transform _barrelCheckPoint;
	[SerializeField] private Vector2 _pushCheckSize = new Vector2(1f, 0.5f);
    #endregion

    #region LAYERS & TAGS
    [Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	[SerializeField] private LayerMask _airMovable;
	//[SerializeField] private LayerMask _mudLayer;
	#endregion

	#region OBJECTS & EFFECTS
	[Header("Objects & Effects")]
	public GameObject center;
	public Transform aimPivot;
	public GameObject standCollider;
	public GameObject crouchCollider;
	public ParticleSystem jumpDust;
	public GameObject dashWindVFX;
	public GameObject dashWindVFXSecond;
	#endregion

	#region MOVEMENT
	[Header("Movement")]
	public bool isOnSlope;
	public LayerMask slopeMask;
	public float slopeRaycastDistance = 1;
	public RaycastHit2D slopeHit;
	#endregion

	#region  FOR DEBUG
	[Header("Debug")]
	public float movement;
	public bool beingExplode;
	public bool EnableDash;
	#endregion
	//public ParticleSystem dashParticle;
	//public Transform dashParticlePivot;
	//public bool airPushing;
	//public GameObject windTrail;
    private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		Anim = GetComponent<Animator>();
	}

	private void Start()
	{
		SetGravityScale(Data.gravityScale);
		IsFacingRight = true;
		crouchCollider.SetActive(false);
	}

	private void Update()
	{
        #region TIMERS
        LastOnGroundTime -= Time.deltaTime;
		LastOnWallTime -= Time.deltaTime;
		LastOnWallRightTime -= Time.deltaTime;
		LastOnWallLeftTime -= Time.deltaTime;
		LastOnMudTime -= Time.deltaTime;
		LastOnWindTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		LastPressedDashTime -= Time.deltaTime;
		LastPressedPushTime -= Time.deltaTime;
		#endregion

		#region INPUT HANDLER
		_moveInput.x = Input.GetAxisRaw("Horizontal");
		_moveInput.y = Input.GetAxisRaw("Vertical");

		if(Input.GetKey(KeyCode.LeftArrow))
			leftKeyPressed = true;
		else
			leftKeyPressed = false;
		
		if(Input.GetKey(KeyCode.RightArrow))
			rightKeyPressed = true;
		else
			rightKeyPressed = false;
		if(Input.GetKey(KeyCode.DownArrow))
			downKeyPressed = true;
		else
			downKeyPressed = false;


		if (_moveInput.x != 0)
			CheckDirectionToFace(_moveInput.x > 0);

		if(Input.GetButtonDown("Jump"))
        {
			OnJumpInput();
        }

		if (Input.GetButtonUp("Jump"))
		{
			OnJumpUpInput();
		}

		if (Input.GetKeyDown(KeyCode.X))
		{
			OnDashInput();
			//OnPushInput();
		}
		if (Input.GetButtonDown("Wind"))
		{
			OnPushInput();
		}
		if(Input.GetButtonDown("Fire"))
		{
			Fire();
		}
		#endregion

		#region COLLISION CHECKS
		slopeHit = Physics2D.Raycast(_groundCheckPoint.position , Vector2.down , slopeRaycastDistance , slopeMask);
		if(slopeHit && Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer))
		{
			isOnSlope = true;
			float slopeAngle = Vector2.Angle(slopeHit.normal , Vector2.up);
		}
		else
			isOnSlope = false;
		if (!IsDashing && !IsJumping)
		{
			//Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) //checks if set box overlaps with ground
			{
				//Debug.Log("ground");
				//if so sets the lastGrounded to coyoteTime
				LastOnGroundTime = Data.coyoteTime;

				if(Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer).gameObject.tag == "Mud")
					LastOnMudTime = Data.coyoteTime;
            }

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)) && !IsWallJumping)
				LastOnWallRightTime = Data.coyoteTime;

			//Right Wall Check
			if (((Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && !IsFacingRight)
					|| (Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) && IsFacingRight)) && !IsWallJumping)
				LastOnWallLeftTime = Data.coyoteTime;

			//Two checks needed for both left and right walls since whenever the play turns the wall checkPoints swap sides
			LastOnWallTime = Mathf.Max(LastOnWallLeftTime, LastOnWallRightTime);

			//Front Mud Check
			if(Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) != null)
				if(Physics2D.OverlapBox(_frontWallCheckPoint.position, _wallCheckSize, 0, _groundLayer).gameObject.tag == "Mud")
					frontWallMud = true;
				else
					frontWallMud = false;
			else
				frontWallMud = false;
			
			//Back Mud Check
			if(Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer) != null)
				if(Physics2D.OverlapBox(_backWallCheckPoint.position, _wallCheckSize, 0, _groundLayer).gameObject.tag == "Mud")
					backWallMud = true;
				else
					backWallMud = false;
			else
				backWallMud = false;

			//Air-movable Object Check
			/*
			if(Physics2D.OverlapBox(_barrelCheckPoint.position, _pushCheckSize, 0, _airMovable) && IsCrouching)
				airMovableObj = Physics2D.OverlapBoxAll(_barrelCheckPoint.position, _pushCheckSize, 0, _airMovable);
			else
				airMovableObj = null;
			*/
			

		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;

			if(!IsWallJumping)
				_isJumpFalling = true;
		}

		if (IsWallJumping && Time.time - _wallJumpStartTime > Data.wallJumpTime)
		{
			IsWallJumping = false;
		}

		if (LastOnGroundTime > 0 && !IsJumping && !IsWallJumping)
        {
			_isJumpCut = false;

			if(!IsJumping)
				_isJumpFalling = false;
		}

		if (!IsDashing)
		{
			//Jump
			if (CanJump() && LastPressedJumpTime > 0)
			{
				IsJumping = true;
				IsWallJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;
				Jump();
			}
			//WALL JUMP
			else if (CanWallJump() && LastPressedJumpTime > 0)
			{
				IsWallJumping = true;
				IsJumping = false;
				_isJumpCut = false;
				_isJumpFalling = false;

				_wallJumpStartTime = Time.time;
				_lastWallJumpDir = (LastOnWallRightTime > 0) ? -1 : 1;

				WallJump(_lastWallJumpDir);
			}
		}
		#endregion

		#region DASH CHECKS
		if (CanDash() && LastPressedDashTime > 0 && !IsCrouching && EnableDash)
		{
			//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
			Sleep(Data.dashSleepTime); 

			//If not direction pressed, dash forward
			if (_moveInput != Vector2.zero && !IsSliding)
			{
				_lastDashDir = _moveInput;
			}
			else
				_lastDashDir = IsFacingRight ? Vector2.right : Vector2.left;

			IsDashing = true;
			IsJumping = false;
			IsWallJumping = false;
			_isJumpCut = false;
			
			/*
			if((_moveInput.x == 0 && _moveInput.y == -1 && isGround))
			{
				Debug.Log("gee");
				StartCoroutine(nameof(StartDash), new Vector2(transform.localScale.x , 0));
			}
			else
			{
				Debug.Log(_lastDashDir);
				StartCoroutine(nameof(StartDash), _lastDashDir);
			}
			*/
			StartCoroutine(nameof(StartDash), _lastDashDir);
		}
		#endregion

		#region SLIDE CHECKS
		if (CanSlide() && ((LastOnWallLeftTime > 0 && _moveInput.x < 0 ) || (LastOnWallRightTime > 0 && _moveInput.x > 0)))
		{
			IsSliding = true;

			Debug.Log("slide");
		}
		else
			IsSliding = false;
		#endregion

		if(!IsDashing)
			beingExplode = false;

		#region GRAVITY
		if (!_isDashAttacking)
		{
			//Higher gravity if we've released the jump input or are falling
			if (IsSliding)
			{
				SetGravityScale(0);
			}
			else if(isOnSlope)
			{
				SetGravityScale(0);
				if(RB.velocity.y > 0)
					RB.AddForce(Vector2.down * 10f , ForceMode2D.Force);
			}
			else if (RB.velocity.y < 0 && _moveInput.y < 0)
			{
				//Much higher gravity if holding down
				SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				//Higher gravity if jump button released
				SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
			{
				SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
			}
			else if (RB.velocity.y < 0)
			{
				//Higher gravity if falling
				SetGravityScale(Data.gravityScale * Data.fallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else
			{
				//Default gravity if standing on a platform or moving upwards
				SetGravityScale(Data.gravityScale);
			}
		}
		else
		{
			//No gravity when dashing (returns to normal once initial dashAttack phase over)
			SetGravityScale(0);
		}
		#endregion

		#region CROUCH CHECK

		if(CanCrouch() && downKeyPressed)
		{
			IsCrouching = true;
			standCollider.SetActive(false);
			crouchCollider.SetActive(true);
			center.transform.localPosition = new Vector3(0 , -0.7f , 0);
			
		}
		else
		{
			IsCrouching = false;
			standCollider.SetActive(true);
			crouchCollider.SetActive(false);
			center.transform.localPosition = new Vector3(0 , -0.435f , 0);
		}

		#endregion
		
		#region PUSH CHECK
		if(CanPush() && LastPressedPushTime > 0)
		{
			Push();
		}
		#endregion

		//CheckAttackDir();
		CheckAimDir();
	}

    private void FixedUpdate()
	{
		//Handle Run
		if (!IsDashing)
		{
			if (IsWallJumping)
				Run(Data.wallJumpRunLerp);
			else
				Run(1);
		}
		else if (_isDashAttacking)
		{
			//Run(Data.dashEndRunLerp);
		}

		//Handle Slide
		if (IsSliding)
			Slide();
			
		SwitchAnim();
    }

    #region INPUT CALLBACKS
	//Methods which whandle input detected in Update()
    public void OnJumpInput()
	{
		LastPressedJumpTime = Data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut() || CanWallJumpCut())
			_isJumpCut = true;
	}

	public void OnDashInput()
	{
		LastPressedDashTime = Data.dashInputBufferTime;
	}
	public void OnPushInput()
	{
		LastPressedPushTime = Data.pushInputBufferTime;
	}
    #endregion

    #region GENERAL METHODS
    public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}

	private void Sleep(float duration)
    {
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(nameof(PerformSleep), duration);
    }

	private IEnumerator PerformSleep(float duration)
    {
		Time.timeScale = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		Time.timeScale = 1;
	}
    #endregion
	//MOVEMENT METHODS
    #region RUN METHODS
    private void Run(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * Data.runMaxSpeed;
		if(IsCrouching)
			targetSpeed = 0f;
		//We can reduce are control using Lerp() this smooths changes to are direction and speed
		if(LastOnWindTime < 0)
		{
			targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);
		}

		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		#endregion

		#region Add Bonus Jump Apex Acceleration
		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((IsJumping || IsWallJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if(Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0 && LastOnWindTime < 0)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0; 
		}
		if(Data.doConserveMomentum && LastOnWindTime > 0 && inHorizontalWind)
			accelRate = 0.2f;
		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		if(isOnSlope)
		{
			RB.AddForce(movement * GetSlopeMoveDirection(), ForceMode2D.Force);
		}
		else
			RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		/*
		 * For those interested here is what AddForce() will do
		 * RB.velocity = new Vector2(RB.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / RB.mass, RB.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
	}

	private void Turn()
	{
		//stores scale and flips the player along the x axis, 
		if(!IsDashing)
		{
			Vector3 scale = transform.localScale; 
			scale.x *= -1;
			transform.localScale = scale;

			IsFacingRight = !IsFacingRight;
		}
	}
    #endregion

    #region JUMP METHODS
    private void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;

		#region Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		float force = Data.jumpForce;

		if(LastOnMudTime > 0)
			force = Data.jumpForce / 2f;
			
		if (RB.velocity.y < 0)
			force -= RB.velocity.y;
		if(RB.velocity.y > 0)
			RB.velocity = new Vector2(RB.velocity.x , 0);

		CreateJumpDust();

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}

	private void WallJump(int dir)
	{
		//Ensures we can't call Wall Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		LastOnWallRightTime = 0;
		LastOnWallLeftTime = 0;

		#region Perform Wall Jump
		Vector2 force = new Vector2(Data.wallJumpForce.x, Data.wallJumpForce.y);
		force.x *= dir; //apply force in opposite direction of wall

		if (Mathf.Sign(RB.velocity.x) != Mathf.Sign(force.x))
			force.x -= RB.velocity.x;

		if (RB.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
			force.y -= RB.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply are force instantly ignoring masss

		CreateJumpDust();

		RB.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region DASH METHODS
	//Dash Coroutine
	private IEnumerator StartDash(Vector2 dir)
	{

		CreateDashWind(dir);
		//CheckDashParticleDir();
		//windTrail.SetActive(true);
		//Overall this method of dashing aims to mimic Celeste, if you're looking for
		// a more physics-based approach try a method similar to that used in the jump

		LastOnGroundTime = 0;
		LastPressedDashTime = 0;

		float startTime = Time.time;

		_dashesLeft--;
		_isDashAttacking = true;
		//bool dashPSUsed = false;
		
		SetGravityScale(0);

		//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
		while (Time.time - startTime <= Data.dashAttackTime)
		{
			if(!beingExplode)
			{
				/*
				if(!dashPSUsed)
				{
					//CreateDashParticle();
				}
				*/
				RB.velocity = dir.normalized * Data.dashSpeed;
				//dashPSUsed = true;
			}
			else
			{
				//dashParticle.Stop();
				break;
			}
			//RB.velocity = dir.normalized * Data.dashSpeed;
			//RB.AddForce(dir.normalized * Data.dashSpeed , ForceMode2D.Force);
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}

		//Invoke("DisableDashVFX" , 0.5f);

		//windTrail.GetComponent<TrailRenderer>().Clear();
		//windTrail.SetActive(false);
		//dashParticle.Stop();
		startTime = Time.time;

		_isDashAttacking = false;

		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
		SetGravityScale(Data.gravityScale);
		if(!beingExplode)
		{
			RB.velocity = Data.dashEndSpeed * dir.normalized;
		}
		//RB.velocity = new Vector2(Mathf.Lerp(RB.velocity.x , movement , 0.001f) , RB.velocity.y);

		while (Time.time - startTime <= Data.dashEndTime)
		{
			yield return null;
		}

		//Dash over
		IsDashing = false;
		beingExplode = false;
		//dashPSUsed = false;
		//Debug.Log("end");
	}

	//Short period before the player is able to dash again
	private IEnumerator RefillDash(int amount)
	{
		//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
		_dashRefilling = true;
		yield return new WaitForSeconds(Data.dashRefillTime);
		_dashRefilling = false;
		_dashesLeft = Mathf.Min(Data.dashAmount, _dashesLeft + amount);
	}
	#endregion

	#region OTHER MOVEMENT METHODS
	private void Slide()
	{
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		//Debug.Log(RB.velocity);
		if(RB.velocity.y >= Data.slideSpeed)
		{
			RB.velocity = new Vector2(RB.velocity.x , RB.velocity.y - 1);
		}
		float speedDif = Data.slideSpeed - RB.velocity.y;
		float movement = Mathf.Abs(speedDif * Data.slideAccel);
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		RB.AddForce(movement * Vector2.down);
	}
    #endregion

    #region CHECK METHODS
    public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

    private bool CanJump()
    {
		return LastOnGroundTime > 0 && !IsJumping;
    }

	private bool CanWallJump()
    {
		return LastPressedJumpTime > 0 && LastOnWallTime > 0 && LastOnGroundTime <= 0 && (!IsWallJumping ||
			 (LastOnWallRightTime > 0 && _lastWallJumpDir == 1) || (LastOnWallLeftTime > 0 && _lastWallJumpDir == -1));
	}

	private bool CanJumpCut()
    {
		return IsJumping && RB.velocity.y > 0;
    }

	private bool CanWallJumpCut()
	{
		return IsWallJumping && RB.velocity.y > 0;
	}

	private bool CanDash()
	{
		if (!IsDashing && _dashesLeft < Data.dashAmount && LastOnGroundTime > 0 && !_dashRefilling)
		{

			StartCoroutine(nameof(RefillDash), 1);

		}

		return _dashesLeft > 0;
	}

	public bool CanSlide()
    {
		if (LastOnWallTime > 0 && !IsJumping && !IsWallJumping && !IsDashing && LastOnGroundTime <= 0)
		{
			return true;
		}
		else
			return false;
	}

	public bool CanCrouch()
	{
		if(!IsJumping && !IsDashing && LastOnGroundTime > 0 && !IsSliding && !IsWallJumping && !_isJumpFalling)
		{
			//Debug.Log("yes");
			return true;
		}
		else
			return false;
	}

	public bool CanPush()
	{
		if(!_pushRefilling)
		{
			StartCoroutine(nameof(RefillPush), 1);
		}
		return _pushesLeft > 0;
	}

	public bool OnWallMud()
	{
		return frontWallMud || backWallMud;
	}
	#endregion

    #region EDITOR METHODS
    private void OnDrawGizmosSelected()
    {
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(_frontWallCheckPoint.position, _wallCheckSize);
		Gizmos.DrawWireCube(_backWallCheckPoint.position, _wallCheckSize);
		//Gizmos.DrawWireCube(_barrelCheckPoint.position, _pushCheckSize);
		Gizmos.DrawRay(_groundCheckPoint.position , Vector2.down * slopeRaycastDistance);
	}
    #endregion

	#region ANIMATION CONTROL
	void SwitchAnim()
    {
        Anim.SetFloat("IsRunning" , Mathf.Abs(RB.velocity.x));

        if(Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping){
            Anim.SetBool("IsFalling", false);
        }
        else if(!Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && IsJumping && RB.velocity.y > 0)
        {
            Anim.SetBool("IsJumping" , true);
        }
        else if (RB.velocity.y < 0)
        {
            Anim.SetBool("IsJumping" , false);
            Anim.SetBool("IsFalling" , true);
        }
		if(IsCrouching)
		{
			Anim.SetBool("IsCrouching" , true);
		}
		else
			Anim.SetBool("IsCrouching" , false);
    }
	#endregion

	
	#region DIRECTION METHODS
	/*
	void CheckAttackDir()
	{
		//bool isLeftWallSliding = false , isRightWallSliding = false;

		if(IsSliding)
		{
			if(OnWallMud() && LastOnWallRightTime > 0 && rightKeyPressed && transform.localScale.x == 1)
			{
				Turn();	
			}
			
			if(OnWallMud() && LastOnWallLeftTime > 0 && leftKeyPressed && transform.localScale.x == -1)
			{
				Turn();
			}
		}

		if(_moveInput.y == 1 && _moveInput.x != 0)
		{
			aimPivot.rotation = Quaternion.Euler(0 , 0 , 45 * transform.localScale.x);
			FacingUp = true;
		}
		else if(_moveInput.y == 1 && _moveInput.x == 0)
		{
			aimPivot.rotation = Quaternion.Euler(0 , 0 , 90 * transform.localScale.x);
			FacingUp = true;
		}
		else if(_moveInput.y == -1 && _moveInput.x != 0 && (IsJumping || _isJumpFalling || LastOnGroundTime < 0))
		{
			aimPivot.rotation = Quaternion.Euler(0 , 0 , -45 * transform.localScale.x);
			FacingDown = true;
		}
		else if(_moveInput.y == -1 && _moveInput.x == 0 && (IsJumping || _isJumpFalling || LastOnGroundTime < 0))
		{
			aimPivot.rotation = Quaternion.Euler(0 , 0 , -90 * transform.localScale.x);
			FacingDown = true;
		}
		else
		{
			aimPivot.rotation = Quaternion.Euler(0 , 0 , 0);
		}
	}
	*/
	void CheckDashWindDir(GameObject dashWind , Vector2 dir)
	{
		VisualEffect dashWindvfx = dashWind.GetComponent<VisualEffect>();
		float inputXAbs = Mathf.Abs(_moveInput.x);

		if(_moveInput.y == 1)
		{
			dashWindvfx.SetVector3("WindRotate" , new Vector3(dir.y , -dir.x*inputXAbs , 0));
			dashWindvfx.SetVector3("WindMovingDirecton" , new Vector3(-dir.x*inputXAbs , -1 , 0));
		}
		else if(_moveInput.y == -1 && (IsJumping || _isJumpFalling || LastOnGroundTime < 0))
		{
			dashWindvfx.SetVector3("WindRotate" , new Vector3(dir.y , -dir.x*inputXAbs , 0));
			dashWindvfx.SetVector3("WindMovingDirecton" , new Vector3(-dir.x*inputXAbs , 1 , 0));
		}
		else
		{
			dashWindvfx.SetVector3("WindRotate" , new Vector3(0 , -dir.x , 0));
			dashWindvfx.SetVector3("WindMovingDirecton" , new Vector3(-dir.x, 0 , 0));
		}
	}

	void CheckAimDir()
	{
		Vector2 dir = Camera.main.ScreenToWorldPoint(Input.mousePosition) - aimPivot.position;
		float angle = Mathf.Atan2(dir.y , dir.x) * Mathf.Rad2Deg;

		if(transform.localScale.x == -1)
			angle += 180;
	
		Quaternion rotation = Quaternion.AngleAxis(angle , Vector3.forward);
		aimPivot.rotation = rotation;
	}
	#endregion

	#region FIRE METHODS
	void Fire() 
	{
		if(!IsCrouching)
		{
			if(FacingUp)
			{
				Anim.Play("FireUp");
				FacingUp = false;
			}
			else if(FacingDown)
			{
				Anim.Play("FireDown");
				FacingDown = false;
			}
			else
			{
				Anim.Play("FireFront");
			}
		}
	}

	//動畫執行時會執行此函式
	void UsingFire()
	{
		fireball.Fireball();
	}
	#endregion

	#region PUSH METHODS
	private void Push()
	{
		LastPressedPushTime = 0;
		_pushesLeft--;

		pushWind.CreatePushWind();
		/*
		if(airMovableObj != null)
		{
			foreach(var obj in airMovableObj)
			{
				Rigidbody2D objRB = obj.GetComponent<Rigidbody2D>();
				objRB.AddForce(new Vector2(transform.localScale.x*Data.pushForce , 0) , ForceMode2D.Impulse);
				RB.AddForce(new Vector2(-transform.localScale.x*Data.pushKnockbackForce , 0) , ForceMode2D.Impulse);
			}
		}
		airMovableObj = null;
		*/
	}
	private IEnumerator RefillPush(int amount)
	{
		//SHoet cooldown, so we can't constantly dash along the ground, again this is the implementation in Celeste, feel free to change it up
		_pushRefilling = true;
		yield return new WaitForSeconds(Data.pushRefillTime);
		_pushRefilling = false;
		_pushesLeft = Mathf.Min(Data.pushAmount, _dashesLeft + amount);
	}
	#endregion

	#region PS AND VFX METHODS
	void CreateJumpDust()
	{
		jumpDust.Play();
	}

	/*
	void CreateDashParticle()
	{
		dashParticle.Play();
	}

	void DisableDashVFX()
	{
		dashWindVFX.SetActive(false);
	}
	*/

	void CreateDashWind(Vector2 dir)
	{
		var dashWind = Instantiate(dashWindVFX , center.transform.position , transform.rotation);
		var dashWindSecond = Instantiate(dashWindVFXSecond , center.transform.position , transform.rotation);

		dashWindSecond.transform.parent = center.transform;
		//dashWind.transform.parent = windCenter.transform;

		dashWind.SetActive(true);
		dashWindSecond.SetActive(true);

		CheckDashWindDir(dashWind , dir);
		CheckDashWindDir(dashWindSecond , dir);

		Destroy(dashWind , 1f);
		Destroy(dashWindSecond , 1f);
	}
	#endregion

	#region TRIGGER METHODS
	private void OnTriggerStay2D(Collider2D trigger) {
		if(trigger.tag == "WindArea")
		{
			LastOnWindTime = Data.coyoteTime;
			WindArea wind = trigger.gameObject.GetComponent<WindArea>();
			if(wind.isHorizontal)
				inHorizontalWind = true;
			else
				inHorizontalWind = false;
		}
	}
	#endregion
	private void OnCollisionEnter2D(Collision2D other) {
		if(other.gameObject.tag == "Explosive")
		{
			beingExplode = true;
		}
		//beingExplode = false;
	}

	private Vector2 GetSlopeMoveDirection()
	{
		float slopeAngle = Vector2.Angle(slopeHit.normal , Vector2.up);
		return new Vector2(Mathf.Cos(slopeAngle * Mathf.Deg2Rad) , Mathf.Sin(slopeAngle * Mathf.Deg2Rad));
	}
}



// created by Dawnosaur :D