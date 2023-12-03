using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Xml.Serialization;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityHFSM;

public class PlayerStateManager : MonoBehaviour
{
	[Header("SetUps")]
	[SerializeField] private PlayerData playerData;
	public PlayerInputControl inputControl;
	private PhysicCheck physicCheck;
	private UnityHFSM.StateMachine fsm;
	private Animator animator;
	private Rigidbody2D rb;
	private PlayerStats playerStats;
	private Text stateDisplayText;
	[Header("Input")]
	public Vector2 inputDirection;
	public Vector2 mousePosition;
	public Vector2 mousePositionOnScreen;
	[Header("Throw")]
	public GameObject aimPivot;//change to getCoponentl8
	public Transform barrel;
	[Space(5)][Range(0, 50)] public float ProjectileSpeed = 20f;
	public GameObject bulletPrefab;
	private bool isCrouching;
	public bool isAttacking;

	public bool isWalling;//candel

	private void Awake()
	{
		inputControl = new PlayerInputControl();
		inputControl.Gameplay.Jump.started += Jump; //使JUMP輸入註冊(+=)jump
		inputControl.Gameplay.Jump.canceled += JumpCut;
		inputControl.Gameplay.Dash.started += Dash;
		inputControl.Gameplay.Throw.started += Throw;
		inputControl.Gameplay.Throw.canceled += ThrowCut;
		inputControl.Gameplay.Heal.started += HealInput;
		inputControl.Gameplay.Heal.canceled += HealCut;
		inputControl.Gameplay.Katana.started += katanaInput;
		inputControl.Gameplay.Katana.canceled += katanaInputCut;
	}
	void Start()
	{

		physicCheck = GetComponent<PhysicCheck>();
		animator = GetComponent<Animator>();
		stateDisplayText = GetComponentInChildren<Text>();
		rb = GetComponent<Rigidbody2D>();
		playerStats = GetComponent<PlayerStats>();
		SetGravityScale(playerData.gravityScale);


		fsm = new UnityHFSM.StateMachine();
		var groundFsm = new HybridStateMachine(

		);
		groundFsm.AddState("Idle",
			onEnter: state => { animator.Play("Idle"); RefillAbility(); },
				onLogic: state =>
				{
					GroundMove(1, 0, 0, playerData.runAccelAmount, playerData.runDeccelAmount);
				});

		groundFsm.AddState("Walk", onEnter: state => animator.Play("Walk"),
		onLogic: state =>
		{
			CheckDirectionToFace();
			GroundMove(1, inputDirection.x, playerData.runMaxSpeed, playerData.runAccelAmount, playerData.runDeccelAmount);
		});
		groundFsm.AddTransition("Idle", "Walk", t => Mathf.Abs(inputDirection.x) >= Mathf.Epsilon);
		groundFsm.AddTransition("Walk", "Idle", t => inputDirection.x == 0 && inputDirection.y == 0);
		groundFsm.AddState("Crouch", onEnter: state => { animator.Play("Crouch"); CrouchIdleMove(); isCrouching = true; }, onLogic => CheckDirectionToFace(), onExit => isCrouching = false);
		groundFsm.AddTransition("Idle", "Crouch", t => inputDirection.y == -1);
		groundFsm.AddTransition("Crouch", "Idle", t => inputDirection.y >= 0);
		groundFsm.AddState("CrouchSlide", onEnter: state => animator.Play("CrouchSlide"), onLogic: state => CrouchSlideMove(0.02f));
		groundFsm.AddTransition("Walk", "CrouchSlide", t => inputDirection.y < 0 && Mathf.Abs(rb.velocity.x) >= playerData.runMaxSpeed * 0.9f);
		groundFsm.AddTransition("CrouchSlide", "Crouch", t => Mathf.Abs(rb.velocity.x) < playerData.runMaxSpeed * 0.1f);
		groundFsm.AddState("Heal", onEnter => { animator.Play("Heal"); StartCoroutine(Heal()); }, onLogic =>
		{
			CheckDirectionToFace();
			GroundMove(1, inputDirection.x, playerData.runMaxSpeed * 0.4f, playerData.runAccelAmount, playerData.runDeccelAmount);
		}, onExit => StopCoroutine(Heal()), canExit: state => !isHealing, needsExitTime: true);
		groundFsm.AddTransitionFromAny("Heal", t => healInputHourglass > 0);
		groundFsm.AddTransition("Heal", "Idle");



		//Attack FSM
		var attackFsm = new HybridStateMachine(afterOnEnter: state => isAttacking = true,
		beforeOnExit: state => isAttacking = false,
			needsExitTime: true
		);

		attackFsm.AddState("Attack1", onEnter: state => animator.Play("Attack1"), onLogic: state =>
		{
			CheckDirectionToFace();
			GroundMove(1, inputDirection.x, playerData.runMaxSpeed * 0.4f, playerData.runAccelAmount, playerData.runDeccelAmount);
		},
		 canExit: state => !AnimatorIsPlaying("Attack1"), needsExitTime: true);
		attackFsm.AddTransition("Attack1", "Attack2", t => katanaInputHourglass > 0, afterTransition: state => katanaInputHourglass = 0f);

		attackFsm.AddState("Attack2", onEnter: state => animator.Play("Attack2"), onLogic: state =>
		{
			CheckDirectionToFace();
			GroundMove(1, inputDirection.x, playerData.runMaxSpeed * 0.4f, playerData.runAccelAmount, playerData.runDeccelAmount);
		},
		canExit: state => !AnimatorIsPlaying("Attack2"), needsExitTime: true);
		attackFsm.AddTransition("Attack2", "Attack3", t => katanaInputHourglass > 0, afterTransition: state => katanaInputHourglass = 0f);


		attackFsm.AddState("Attack3", onEnter: state => { animator.Play("Attack3"); }, onLogic: state =>
		{
			CheckDirectionToFace();
			GroundMove(1, inputDirection.x, playerData.runMaxSpeed * 0.4f, playerData.runAccelAmount, playerData.runDeccelAmount);
		},
		canExit: state => !AnimatorIsPlaying("Attack3"), needsExitTime: true);
		attackFsm.AddTransition("Attack3", "Attack1", t => katanaInputHourglass > 0, afterTransition: state => katanaInputHourglass = 0f);

		attackFsm.AddExitTransitionFromAny(t => !AnyAnimatorIsPlaying() && katanaInputHourglass < 0, forceInstantly: false);
		attackFsm.SetStartState("Attack1");
		groundFsm.AddState("Attack", attackFsm);
		groundFsm.AddTransition("Crouch", "Attack", t => katanaInputHourglass > 0, afterTransition => katanaInputHourglass = 0);
		groundFsm.AddTransition("Attack", "Crouch");
		groundFsm.AddTransition("Attack", "Idle", t => inputDirection.y != -1, forceInstantly: true);

		//Wall FSM
		var wallFsm = new HybridStateMachine();
		//if inputdir.x>0==physick.CheckWallHang
		wallFsm.AddState("Wallslide", onEnter: state => { animator.Play("Wallslide"); CrouchIdleMove(); },
		 onLogic: state =>
		 {
			 WallSlide(); GroundMove(0.9f, inputDirection.x, playerData.runMaxSpeed * 0.2f,
		  playerData.runAccelAmount * playerData.accelInAir, playerData.runDeccelAmount * playerData.deccelInAir);
		 });

		wallFsm.AddState("Wallhang", onEnter: state => { animator.Play("Wallhang"); SetGravityScale(0); },
		onLogic: state => rb.velocity = Vector2.zero, onExit => SetGravityScale(playerData.gravityScale),
		canExit => IsInputSameWallSide() < 0 || inputDirection.y < 0, needsExitTime: true);

		wallFsm.AddTransition("Wallslide", "Wallhang", t => inputDirection.y >= 0 && IsInputSameWallSide() > 0);
		wallFsm.AddTransition("Wallhang", "Wallslide", t => inputDirection.y < 0);


		wallFsm.SetStartState("Wallslide");



		//Root FSM
		fsm.AddState("Ground", groundFsm);

		fsm.SetStartState("Ground");
		fsm.AddState("Jump", onEnter: state => { animator.Play("Jump"); zerolizeJumpHourglass(); Jump(); },
		onLogic =>
		{
			CheckDirectionToFace();
			InAirMove(1, inputDirection.x, playerData.runMaxSpeed, playerData.runAccelAmount * playerData.accelInAir
		, playerData.runDeccelAmount * playerData.deccelInAir,
		playerData.jumpHangTimeThreshold, playerData.jumpHangAccelerationMult,
		playerData.jumpHangMaxSpeedMult, playerData.doConserveMomentum,
		rb.velocity.y > 0);
			if (jumpCutInput)
			{
				SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMult);
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -playerData.maxFallSpeed));
			};
		},
		onExit => { zerolizeJumpHourglass(); SetGravityScale(playerData.gravityScale); },
		canExit => rb.velocity.y < 0, needsExitTime: true);

		fsm.AddTransitionFromAny("Ground", t => physicCheck.onGround && rb.velocity.y < 0.01f);
		fsm.AddTransition("Ground", "Jump", t => jumpInputHourglass > 0 && physicCheck.groundHourglass > 0);

		fsm.AddState("Fall", onEnter: state => { animator.Play("Fall"); },
		onLogic =>
		{
			CheckDirectionToFace();

			if (rb.velocity.y < 0f && !physicCheck.onGround)
			{
				//Higher gravity if falling
				SetGravityScale(playerData.gravityScale * playerData.fallGravityMult);
				//Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -playerData.maxFallSpeed));
			}
			if (!jumpCutInput)
			{
				SetGravityScale(playerData.gravityScale * playerData.jumpHangGravityMult);
			}
		},
		onExit => SetGravityScale(playerData.gravityScale));


		fsm.AddState("Dash", new CoState(this, Dash, loop: false, canExit: state => !isDashing, needsExitTime: true));
		fsm.AddTransitionFromAny("Dash", t => dashesLeft > 0 && DashInputHourglass > 0);
		fsm.AddTransitionFromAny("Fall", t => !physicCheck.onGround && rb.velocity.y < 0 && physicCheck.canHangWhichWall == -3);
		fsm.AddTransitionFromAny("Ground", t => physicCheck.onGround && rb.velocity.y < 0.01f);
		fsm.AddState("Throw", new CoState(this, AimThrow, loop: false, canExit: state => !isThrowing, needsExitTime: true));
		fsm.AddTransitionFromAny("Throw", t => throwleft > 0 && throwInputHourglass > 0 && lastThrowTime > playerData.fireballCooldown);
		fsm.AddState("Block", new CoState(this, Block, loop: false, canExit: state => !isBlocking, needsExitTime: true));
		fsm.AddTransitionFromAny("Block", t => !isCrouching && katanaInputHourglass > 0 && !isAttacking);

		fsm.AddState("Wall", wallFsm);
		fsm.AddState("Walljump", onEnter: state => { animator.Play("Walljump"); zerolizeJumpHourglass(); WallJump(); },
		onLogic =>
		{
			CheckDirectionToFace();

			if (jumpCutInput)
			{
				SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMult);
				rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -playerData.maxFallSpeed));
			};
		}, onExit => { SetGravityScale(playerData.gravityScale); zerolizeJumpHourglass(); },
		canExit => rb.velocity.y < 0, needsExitTime: true);

		fsm.AddTransitionFromAny("Walljump", t => jumpInputHourglass > 0 && physicCheck.wallHourglass > 0 && !physicCheck.onGround);
		fsm.AddTransition("Jump", "Wall", t => IsInputSameWallSide() >= -1 && !physicCheck.onGround);
		fsm.AddTransition("Fall", "Wall", t => IsInputSameWallSide() >= -1 && !physicCheck.onGround);
		fsm.AddTransition("Dash", "Wall", t => IsInputSameWallSide() >= -1 && !physicCheck.onGround);

		fsm.AddState("Hurt", onEnter:state=>{animator.Play("Hurt");},
		onLogic:state => {GroundMove(0.1f, 0, 0, playerData.runAccelAmount, playerData.runDeccelAmount);},
		onExit=>playerStats.gotHurt=false,
		canExit:state=>!AnimatorIsPlaying("Hurt"), needsExitTime: true);
		fsm.AddTransitionFromAny("Hurt",t=>playerStats.gotHurt,forceInstantly:true);


		fsm.Init();//開機
		IsFacingRight = true;
		FacingDirection = 1;
		RefillAbility();
	}
	void Update()
	{
		fsm.OnLogic();
		stateDisplayText.text = fsm.GetActiveHierarchyPath();
		inputDirection = inputControl.Gameplay.Movement.ReadValue<Vector2>();
		getMouseInWorldPosition();
		Hourglass();

		animator.SetFloat("VelocityY", rb.velocity.y);
		FacingDirection = (int)transform.localScale.x;
	}
	private void Hourglass()
	{
		jumpInputHourglass -= Time.deltaTime;
		DashInputHourglass -= Time.deltaTime;
		throwInputHourglass -= Time.deltaTime;
		healInputHourglass -= Time.deltaTime;
		katanaInputHourglass -= Time.deltaTime;
		lastThrowTime += Time.deltaTime;
	}
	public void getMouseInWorldPosition()
	{
		mousePosition = inputControl.Gameplay.Aim.ReadValue<Vector2>();
		mousePositionOnScreen = Camera.main.ScreenToWorldPoint(mousePosition);
	}
	private bool IsFacingRight;
	private int FacingDirection;
	public void CheckDirectionToFace()
	{
		if (Mathf.Abs(inputDirection.x) > Mathf.Epsilon)
		{
			if (inputDirection.x > 0 != IsFacingRight)
				Turn();
		}
	}
	public void Turn()
	{
		//stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
	public void RefillAbility()
	{
		dashesLeft = playerData.dashAmount;
		throwleft = 2;
	}
	public void GetKnockback(Transform attacker)//呼叫自己擊退功能
    {
        Vector2 dir = attacker.GetComponent<Attack>().knockbackDirection.normalized * (transform.position.x - attacker.transform.position.x > 0 ? 1 : -1);
        rb.AddForce(dir * attacker.GetComponent<Attack>().knockbackStrengh, ForceMode2D.Impulse);
	}
	public void PlayInvunerbleTwinkle()
	{
		animator.SetBool("Hurt",true);
		Invoke("TwinkleOff",playerStats.invunerableDuartion);
	}
	private void TwinkleOff()
	{
		animator.SetBool("Hurt",false);
	}
	private void OnEnable()
	{
		inputControl.Enable();
	}
	private void OnDisable()
	{
		inputControl.Disable();
	}
	public float DashInputHourglass;
	private void Dash(InputAction.CallbackContext obj)
	{
		DashInputHourglass = playerData.dashInputBufferTime;
	}
	public float jumpInputHourglass;
	public bool jumpCutInput;
	private void Jump(InputAction.CallbackContext obj)
	{
		jumpInputHourglass = playerData.jumpInputBufferTime;
		jumpCutInput = false;
	}

	private void JumpCut(InputAction.CallbackContext obj)
	{
		jumpCutInput = true;
	}
	public float throwInputHourglass;
	public bool throwInputCut;
	private void Throw(InputAction.CallbackContext obj)
	{
		throwInputHourglass = playerData.fireballInputBufferTime;
		throwInputCut = false;
	}
	private void ThrowCut(InputAction.CallbackContext obj)
	{
		throwInputCut = true;
	}
	private float healInputHourglass;
	private bool healInputCut;
	public float healInputBufferTime;//add to data;

	private void HealInput(InputAction.CallbackContext obj)
	{
		healInputHourglass = healInputBufferTime;
		healInputCut = false;

	}
	private void HealCut(InputAction.CallbackContext obj)
	{
		healInputCut = true;
	}
	public float katanaInputHourglass;
	public float katanaInputBufferTime;//add to data
	private bool katanaInputStop;
	private void katanaInput(InputAction.CallbackContext obj)
	{
		katanaInputHourglass = katanaInputBufferTime;
		katanaInputStop = false;
	}
	private void katanaInputCut(InputAction.CallbackContext obj)
	{
		katanaInputStop = true;
	}

	public void Jump()
	{
		//Ensures we can't call Jump multiple times from one press
		//physicCheck.groundHourglass = 0;
		#region Perform Jump
		//We increase the force applied if we are falling
		//This means we'll always feel like we jump the same amount 
		//(setting the player's Y velocity to 0 beforehand will likely work the same, but I find this more elegant :D)
		float force = playerData.jumpForce;


		if (rb.velocity.y < 0)
			force -= rb.velocity.y;
		if (rb.velocity.y > 0)
			rb.velocity = new Vector2(rb.velocity.x, 0);

		rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}
	public void GroundMove(float lerpAmount, float xInput, float maxSpeed, float accel, float deccel)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = xInput * maxSpeed;

		targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate

		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel : deccel;

		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - rb.velocity.x;

		//Calculate force along x-axis to apply to thr player
		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

	}
	public void InAirMove(float lerpAmount, float xInput, float maxSpeed, float accel, float deccel, float jumpHangTimeThreshold, float jumpHangAccelerationMult, float jumpHangMaxSpeedMult, bool doConserveMomentum, bool isJumpingUp)
	{
		//Calculate the direction we want to move in and our desired velocity
		float targetSpeed = xInput * maxSpeed;

		targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate

		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accel : deccel;

		#endregion

		#region Add Bonus Jump Apex Acceleration

		//Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if (isJumpingUp && Mathf.Abs(rb.velocity.y) < jumpHangTimeThreshold)
		{
			accelRate *= jumpHangAccelerationMult;
			targetSpeed *= jumpHangMaxSpeedMult;
			SetGravityScale(playerData.gravityScale * playerData.jumpHangGravityMult);
		}
		#endregion

		#region Conserve Momentum
		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (doConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0;
		}
		#endregion

		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - rb.velocity.x;

		//Calculate force along x-axis to apply to thr player
		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		rb.AddForce(movement * Vector2.right, ForceMode2D.Force);

	}
	public void SetGravityScale(float scale)
	{
		rb.gravityScale = scale;
	}
	private bool isHealing;
	private IEnumerator Heal()//require mana enough
	{
		isHealing = true;
		int numberOfEnergyConsumed = 0;
		if (playerStats.currentHealth < playerStats.maxHealth)
		{
			for (int i = 0; i < 5; i++)
			{
				if (!healInputCut && playerStats.currentEnergy > playerStats.energyConsumption)
				{
					yield return new WaitForSeconds(0.5f);
					playerStats.EnergyConsume();
					numberOfEnergyConsumed++;
				}
				else
				{
					break;
				}
			}
			if (numberOfEnergyConsumed >= 5)
			{
				playerStats.HealthRegen();
			}
		}
		isHealing = false;
	}
	public void CrouchIdleMove()
	{
		rb.velocity = new Vector2(0, rb.velocity.y);
	}
	public void CrouchSlideMove(float lerpAmount)
	{
		//Calculate the direction we want to move in and our desired velocity

		//在斜坡上會改變
		float targetSpeed = 0;

		targetSpeed = Mathf.Lerp(rb.velocity.x, targetSpeed, lerpAmount);


		#region Calculate AccelRate
		float accelRate;

		//Gets an acceleration value based on if we are accelerating (includes turning) 
		//or trying to decelerate (stop). As well as applying a multiplier if we're air borne.

		//if在斜坡上
		if (!physicCheck.onGround)
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.crouchSlideAccelAmount : playerData.crouchSlideDeccelAmount;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? playerData.crouchSlideAccelAmount * playerData.accelOnGround : playerData.crouchSlideDeccelAmount * playerData.deccelOnGround;

		#endregion

		//斜坡上保留速度
		/*
		#region Conserve Momentum

		//We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if(playerData.crouchSlideDoConserveMomentum && Mathf.Abs(rb.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(rb.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f)
		{
			//Prevent any deceleration from happening, or in other words conserve are current momentum
			//You could experiment with allowing for the player to slightly increae their speed whilst in this "state"
			accelRate = 0;
		}
		*/
		/*
		if(playerData.doConserveMomentum && LastOnWindTime > 0 && inHorizontalWind)
			accelRate = 0.2f;
		
		#endregion
		*/


		//Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - rb.velocity.x;
		//Calculate force along x-axis to apply to thr player

		float movement = speedDif * accelRate;

		//Convert this to a vector and apply to rigidbody
		/*
		 * For those interested here is what AddForce() will do
		 * rb.velocity = new Vector2(rb.velocity.x + (Time.fixedDeltaTime  * speedDif * accelRate) / rb.mass, rb.velocity.y);
		 * Time.fixedDeltaTime is by default in Unity 0.02 seconds equal to 50 FixedUpdate() calls per second
		*/
		rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
	}
	public void Sleep(float duration)
	{
		//Method used so we don't need to call StartCoroutine everywhere
		//nameof() notation means we don't need to input a string directly.
		//Removes chance of spelling mistakes and will improve error messages if any
		StartCoroutine(nameof(PerformSleep), duration);
	}
	public int dashesLeft;
	bool isDashing;
	private IEnumerator Dash()
	{
		DashInputHourglass = 0; dashesLeft--; isDashing = true;
		Vector2 Dir;
		animator.Play("Dash");
		//Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
		Sleep(playerData.dashSleepTime);
		//If not direction pressed, dash forward
		if (inputDirection != Vector2.zero)
		{
			Dir = inputDirection.normalized;
		}
		else
			Dir = new Vector2(FacingDirection, 0);

		//Overall this method of dashing aims to mimic Celeste, if you're looking for
		// a more physics-based approach try a method similar to that used in the jump
		float startTime = Time.time;

		SetGravityScale(0);

		//We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
		while (Time.time - startTime <= playerData.dashAttackTime)
		{
			rb.velocity = Dir * playerData.dashSpeed;
			//Pauses the loop until the next frame, creating something of a Update loop. 
			//This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
			yield return null;
		}
		startTime = Time.time;
		//Begins the "end" of our dash where we return some control to the player but still limit run acceleration (see Update() and Run())
		SetGravityScale(playerData.gravityScale);
		//Debug.Log(rb.gravityScale);
		rb.velocity = playerData.dashEndSpeed * Dir.normalized;
		while (Time.time - startTime <= playerData.dashEndTime)
		{
			isDashing = false;
			yield return null;
		}
	}
	private IEnumerator PerformSleep(float duration)
	{
		Time.timeScale = 0;
		animator.speed = 0;
		yield return new WaitForSecondsRealtime(duration); //Must be Realtime since timeScale with be 0 
		animator.speed = 1;
		Time.timeScale = 1;
	}

	public void RotateAimPivot()
	{
		Vector2 dir = mousePositionOnScreen - (Vector2)aimPivot.transform.position;

		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

		if (!IsFacingRight)
			angle -= 180;

		Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
		aimPivot.transform.rotation = rotation;
	}
	private void FaceToAimingDirection()
	{
		if (mousePositionOnScreen.x > transform.position.x == !IsFacingRight)
		{
			Turn();
		}
	}
	public float lastThrowTime;
	public int throwleft;
	bool isThrowing;
	private IEnumerator AimThrow()
	{
		isThrowing = true;
		aimPivot.gameObject.SetActive(true);
		Time.timeScale = playerData.fireballHoldtimeScale;
		rb.drag = playerData.fireballDrag;
		float startTime = Time.unscaledTime;
		animator.Play("Throw");
		animator.speed = 0;
		while (!throwInputCut && Time.unscaledDeltaTime < startTime + playerData.maxHoldTime)
		{
			FaceToAimingDirection();
			RotateAimPivot();
			yield return null;
		}
		animator.speed = 1;
		Time.timeScale = 1f;
		FireProjectile();//change it to animation 
		aimPivot.gameObject.SetActive(false);
		throwleft--;
		startTime = Time.time;
		while (Time.time <= startTime + playerData.fireballDuration)
		{
			yield return null;
		}
		lastThrowTime = 0;
		rb.drag = 0f;
		isThrowing = false;
		Debug.Log("Throw ends");
		yield break;

	}

	public void FireProjectile()
	{
		GameObject BulletIns = Instantiate(bulletPrefab, barrel.position, aimPivot.transform.rotation);
		BulletIns.GetComponent<Rigidbody2D>().velocity = aimPivot.transform.right * ProjectileSpeed * FacingDirection;
		//BulletIns.transform.localScale *= FacingDirection;
	}
	bool AnyAnimatorIsPlaying()
	{
		return animator.GetCurrentAnimatorStateInfo(0).length >
			   animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
	}
	bool AnimatorIsPlaying(string stateName)
	{
		return AnyAnimatorIsPlaying() && animator.GetCurrentAnimatorStateInfo(0).IsName(stateName);
	}

	public bool isBlocking;
	private IEnumerator Block()
	{
		isBlocking = true;
		aimPivot.gameObject.SetActive(true);
		Time.timeScale = playerData.slashHoldtimeScale;
		rb.drag = playerData.SlashDrag;
		float startTime = Time.unscaledTime;
		animator.Play("Block");
		animator.speed = 0;
		while (!katanaInputStop && Time.unscaledDeltaTime < startTime + playerData.maxHoldTime)
		{
			FaceToAimingDirection();
			RotateAimPivot();
			yield return null;
		}
		animator.speed = 1;
		Time.timeScale = 1f;
		Debug.Log("block!");
		//FireProjectile();//change it to animation 
		aimPivot.gameObject.SetActive(false);
		startTime = Time.time;
		while (Time.time <= startTime + playerData.SlashDuration)
		{
			yield return null;
		}
		rb.drag = 0f;
		isBlocking = false;
		Debug.Log("block ends");
		yield break;

	}
	private int IsInputSameWallSide()
	{
		if (inputDirection.x > 0 && physicCheck.canHangWhichWall == -1 || inputDirection.x < 0 && physicCheck.canHangWhichWall == -2)//nohangableR L
			return -1;
		else if (inputDirection.x > 0 && physicCheck.canHangWhichWall == 1)//hangR
			return 1;
		else if (inputDirection.x < 0 && physicCheck.canHangWhichWall == 0)//L
			return 0;
		else
			return -2;//wronginputDirection

	}
	public void WallSlide()
	{
		//Works the same as the Run but only in the y-axis
		//THis seems to work fine, buit maybe you'll find a better way to implement a slide into this system
		//Debug.Log(rb.velocity);
		if (rb.velocity.y >= playerData.slideSpeed)
		{
			rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y - 0.5f);
		}
		float speedDif = playerData.slideSpeed - rb.velocity.y;
		float movement = Mathf.Abs(speedDif * playerData.slideAccel);
		//So, we clamp the movement here to prevent any over corrections (these aren't noticeable in the Run)
		//The force applied can't be greater than the (negative) speedDifference * by how many times a second FixedUpdate() is called. For more info research how force are applied to rigidbodies.
		movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));

		rb.AddForce(movement * Vector2.down);
	}
	//Ensures we can't call Wall Jump multiple times from one press
	private void zerolizeJumpHourglass()
	{
		physicCheck.groundHourglass = 0;
		physicCheck.wallHourglass = 0;
	}
	public void WallJump()
	{

		float WallJumpDir = -1;
		if (physicCheck.canHangWhichWall != -3)
		{
			if (physicCheck.canHangWhichWall == 1 || physicCheck.canHangWhichWall == -1)//R
			{
				WallJumpDir = -1;//goL
			}
			else
			{
				WallJumpDir = 1;//-3 nocase
			}
		}

		#region Perform Wall Jump

		Vector2 force = new(playerData.wallJumpForce.x, playerData.wallJumpForce.y);
		force.x *= WallJumpDir; //apply force in opposite direction of wall

		if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= rb.velocity.x;

		if (rb.velocity.y < 0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
			force.y -= rb.velocity.y;

		//Unlike in the run we want to use the Impulse mode.
		//The default mode will apply are force instantly ignoring masss


		rb.AddForce(force, ForceMode2D.Impulse);
		#endregion
	}

}