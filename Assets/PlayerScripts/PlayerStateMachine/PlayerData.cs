﻿using UnityEngine;

[CreateAssetMenu(menuName = "Player Data")] //Create a new playerData object by right clicking in the Project Menu then Create/Player/Player Data and drag onto the player
public class PlayerData : ScriptableObject
{
	[Header("Gravity")]
	[HideInInspector] public float gravityStrength; //Downwards force (gravity) needed for the desired jumpHeight and jumpTimeToApex.
	[HideInInspector] public float gravityScale; //Strength of the player's gravity as a multiplier of gravity (set in ProjectSettings/Physics2D).
										  //Also the value the player's rigidbody2D.gravityScale is set to.
	[Space(5)]
	public float fallGravityMult; //Multiplier to the player's gravityScale when falling.
	public float maxFallSpeed; //Maximum fall speed (terminal velocity) of the player when falling.
	[Space(5)]
	public float fastFallGravityMult; //Larger multiplier to the player's gravityScale when they are falling and a downwards input is pressed.
									  //Seen in games such as Celeste, lets the player fall extra fast if they wish.
	public float maxFastFallSpeed; //Maximum fall speed(terminal velocity) of the player when performing a faster fall.
	
	[Space(20)]

	[Header("Run")]
	public float runMaxSpeed; //Target speed we want the player to reach.
	public float runAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	[HideInInspector] public float runAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float runDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	[HideInInspector] public float runDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(5)]
	[Range(0f, 1)] public float accelInAir; //Multipliers applied to acceleration rate when airborne.
	[Range(0f, 1)] public float deccelInAir;
	[Space(5)]
	public bool doConserveMomentum = true;

	[Space(20)]

	[Header("Jump")]
	public float jumpHeight; //Height of the player's jump
	public float jumpTimeToApex; //Time between applying the jump force and reaching the desired jump height. These values also control the player's gravity and jump force.
	[HideInInspector] public float jumpForce; //The actual force applied (upwards) to the player when they jump.

	[Header("Both Jumps")]
	public float jumpCutGravityMult; //Multiplier to increase gravity if the player releases thje jump button while still jumping
	[Range(0f, 1)] public float jumpHangGravityMult; //Reduces gravity while close to the apex (desired max height) of the jump
	public float jumpHangTimeThreshold; //Speeds (close to 0) where the player will experience extra "jump hang". The player's velocity.y is closest to 0 at the jump's apex (think of the gradient of a parabola or quadratic function)
	[Space(0.5f)]
	public float jumpHangAccelerationMult; 
	public float jumpHangMaxSpeedMult; 				

	[Header("Wall Jump")]
	public Vector2 wallJumpForce; //The actual force (this time set by us) applied to the player when wall jumping.
	[Space(5)]
	[Range(0f, 1f)] public float wallJumpRunLerp; //Reduces the effect of player's movement while wall jumping.
	[Range(0f, 1.5f)] public float wallJumpTime; //Time after wall jumping the player's movement is slowed for.
	public bool doTurnOnWallJump; //Player will rotate to face wall jumping direction

	[Space(20)]

	[Header("Slide")]
	public float slideSpeed;
	public float slideAccel;

	[Space(20)]

	[Header("CrouchSlide")]
	public float crouchSlideLerp;
	public float crouchSlideSlopeLerp;

	[Space(5)]
	public float slopeSlideMaxSpeed; //Target speed we want the player to reach.
	public float crouchSlideAcceleration; //The speed at which our player accelerates to max speed, can be set to runMaxSpeed for instant acceleration down to 0 for none at all
	[HideInInspector] public float  crouchSlideAccelAmount; //The actual force (multiplied with speedDiff) applied to the player.
	public float  crouchSlideDecceleration; //The speed at which our player decelerates from their current speed, can be set to runMaxSpeed for instant deceleration down to 0 for none at all
	[HideInInspector] public float  crouchSlideDeccelAmount; //Actual force (multiplied with speedDiff) applied to the player .
	[Space(5)]
	[Range(0f, 1)] public float accelOnGround; //Multipliers applied to acceleration rate when airborne.
	[Range(0f, 1)] public float deccelOnGround;
	[Space(5)]
	public bool crouchSlideDoConserveMomentum = true;

	[Space(20)]

    [Header("Assists")]
	[Range(0.01f, 0.5f)] public float coyoteTime; //Grace period after falling off a platform, where you can still jump
	[Range(0.01f, 0.5f)] public float jumpInputBufferTime; //Grace period after pressing jump where a jump will be automatically performed once the requirements (eg. being grounded) are met.

	[Space(20)]
	
	[Header("Dash")]
	public int dashAmount;
	public float dashSpeed;
	public float dashSleepTime; //Duration for which the game freezes when we press dash but before we read directional input and apply a force
	[Space(5)]
	public float dashAttackTime;
	[Space(5)]
	public float dashEndTime; //Time after you finish the inital drag phase, smoothing the transition back to idle (or any standard state)
	public Vector2 dashEndSpeed; //Slows down player, makes dash feel more responsive (used in Celeste)
	[Range(0f, 1f)] public float dashEndRunLerp; //Unused,Slows the affect of player movement while dashing
	[Space(5)]
	public float dashRefillTime;
	[Space(5)]
	[Range(0.01f, 0.5f)] public float dashInputBufferTime;

	[Space(20)]

	[Header("Push")]
	public int pushAmount;
	public int pushForce;
	public int pushKnockbackForce;
	[Space(5)]
	public float pushRefillTime;
	[Range(0.01f, 0.1f)] public float pushInputBufferTime;

	[Space(20)]

	[Header("Fireball State")]
	[Range(0.01f, 0.1f)] public float fireballInputBufferTime;
	public float fireballCooldown;
	public float maxHoldTime;
	public float fireballHoldtimeScale;
	public float fireballDuration;
	public float fireballDrag;

	[Space(20)]

	[Header("Air Push State")]
	[Range(0.01f, 0.1f)] public float airPushInputBufferTime;
	public float airPushCooldown;
	public float airPushTime;
	public float airPushDuration;
	public float airPushDrag;
	
	[Space(20)]

	[Header("Melee")]
	[Range(0.01f, 0.1f)] public float meleeInputBufferTime;

	[Header("Sneak Strike State")]
	/// <summary>
	/// CD:
	/// </summary>
	public float sneakStrikeCooldown;
	public float sneakStrikeTime;
	public float sneakStrikeDuration;
	public float sneakStrikeDrag;
	[Space(20)]
	
	[Header("Slash State")]
	public float SlashCooldown;
	public float maxSlashHoldTime;
	public float slashHoldtimeScale;
	public float SlashDuration;
	public float SlashDrag;
	
	

	//Unity Callback, called when the inspector updates
    private void OnValidate()
    {
		//Calculate gravity strength using the formula (gravity = 2 * jumpHeight / timeToJumpApex^2) 
		gravityStrength = -(2 * jumpHeight) / (jumpTimeToApex * jumpTimeToApex);
		
		//Calculate the rigidbody's gravity scale (ie: gravity strength relative to unity's gravity value, see project settings/Physics2D)
		gravityScale = gravityStrength / Physics2D.gravity.y;

		//Calculate are run acceleration & deceleration forces using formula: amount = ((1 / Time.fixedDeltaTime) * acceleration) / runMaxSpeed
		runAccelAmount = (50 * runAcceleration) / runMaxSpeed;
		runDeccelAmount = (50 * runDecceleration) / runMaxSpeed;

		crouchSlideAccelAmount = (50 * crouchSlideAcceleration) / slopeSlideMaxSpeed;
		crouchSlideDeccelAmount = (50 * crouchSlideDecceleration) / slopeSlideMaxSpeed;

		//Calculate jumpForce using the formula (initialJumpVelocity = gravity * timeToJumpApex)
		jumpForce = Mathf.Abs(gravityStrength) * jumpTimeToApex;

		#region Variable Ranges
		runAcceleration = Mathf.Clamp(runAcceleration, 0.01f, runMaxSpeed);
		runDecceleration = Mathf.Clamp(runDecceleration, 0.01f, runMaxSpeed);

		crouchSlideAcceleration = Mathf.Clamp(crouchSlideAcceleration, 0.01f, slopeSlideMaxSpeed);
		crouchSlideDecceleration = Mathf.Clamp(crouchSlideDecceleration, 0.01f, slopeSlideMaxSpeed);
		#endregion
	}
}