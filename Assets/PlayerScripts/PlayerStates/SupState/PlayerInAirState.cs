using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInAirState : PlayerState
{
    private float xInput;
    private bool jumpCutInput;
    private bool jumpInput;
    private bool fireballInput;
    private bool airPushInput;
    private bool meleeInput;
    private bool dashInput;
    private bool isGrounded;
    private bool isOnWall;
    public bool IsJumping { get; private set; }
    public bool IsJumpCut { get; private set; }
    private bool canGrab;
    public PlayerInAirState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        player.OnGroundCheck();
        isGrounded = player.CheckIfGrounded();

        player.OnWallCheck();
        isOnWall = player.CheckIfOnWall();

        //Debug.Log(isGrounded);
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();
        //Local Assign vvv remember
        xInput = player.inputHandler.XInput;
        jumpCutInput = player.inputHandler.JumpCutInput;
        jumpInput = player.inputHandler.JumpInput();
        fireballInput = player.inputHandler.FireballInput();
        airPushInput = player.inputHandler.AirPushInput();
        meleeInput = player.inputHandler.MeleeInput();
        dashInput = player.inputHandler.DashInput();

        canGrab = player.CanGrab();

        if (xInput != 0)
            player.CheckDirectionToFace(xInput > 0);

        if (isGrounded && player.CurrentVelocity.y < 0.01f)
        {
            IsJumpCut = false;
            stateMachine.ChangeState(player.LandState);
        }
        else if (isOnWall && jumpInput)
        {
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if (CanWallGrab())
        {
            stateMachine.ChangeState(player.WallGrabState);
        }
        else if (CanWallSlide())
        {
            stateMachine.ChangeState(player.WallSlideState);
        }
        else if (fireballInput && player.FireballState.CheckIfCanFireball())
        {
            stateMachine.ChangeState(player.FireballState);
        }
        else if (airPushInput && player.AirPushState.CheckIfCanAirPush())
        {
            stateMachine.ChangeState(player.AirPushState);
        }
        else if (meleeInput && player.SlashState.CheckIfCanSlash())
        {
            stateMachine.ChangeState(player.SlashState);
        }
        else if (dashInput && player.DashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }



        CheckJumping();
        CheckJumpCut();
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.InAirMove(1, xInput, playerData.runMaxSpeed, playerData.runAccelAmount * playerData.accelInAir, playerData.runDeccelAmount * playerData.deccelInAir, playerData.jumpHangTimeThreshold, playerData.jumpHangAccelerationMult, playerData.jumpHangMaxSpeedMult, playerData.doConserveMomentum, IsJumping || player.WallJumpState.IsWallJumping);
    }

    private void CheckJumping()
    {
        if (IsJumping && player.CurrentVelocity.y < 0)
            IsJumping = false;
    }
    private void CheckJumpCut()
    {
        if (jumpCutInput && CanJumpCut())
        {
            IsJumpCut = true;
        }
    }

    //public bool CanWallJumpCut() => player.WallJumpState.IsWallJumping && player.CurrentVelocity.y > 0;
    public bool CanJumpCut() => IsJumping && player.CurrentVelocity.y > 0;

    public void SetJumping(bool setting) => IsJumping = setting;
    public void SetJumpCut(bool setting) => IsJumpCut = setting;
    public bool CanWallSlide() => !IsJumping && isOnWall && XInputAtWall() && player.CurrentVelocity.y < 0;
    public bool CanWallGrab() => !IsJumping && isOnWall && XInputAtWall() && canGrab;
    public bool XInputAtWall() => (player.LastOnWallLeftTime > 0 && xInput < 0) || (player.LastOnWallRightTime > 0 && xInput > 0);


}
