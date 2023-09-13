using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerWallJumpState : PlayerAbilityState
{
    public bool IsWallJumping{ get; private set; }
    private bool wallJumpCutInput;
    private float xInput;
    public PlayerWallJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        player.inputHandler.UseJumpInput();
        player.WallJump();

        IsWallJumping = true;

        player.InAirState.SetJumping(false);
        player.InAirState.SetJumpCut(false);
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        wallJumpCutInput = player.inputHandler.JumpCutInput;
        xInput = player.inputHandler.XInput;

        if(wallJumpCutInput)
        {
            player.InAirState.SetJumpCut(true);
        }

        if(Time.time >= startTime + playerData.wallJumpTime)
        {
            isAbilityDone = true;
            IsWallJumping = false;
        }

    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.InAirMove(playerData.wallJumpRunLerp, xInput, playerData.runMaxSpeed, playerData.runAccelAmount*playerData.accelInAir, playerData.runDeccelAmount*playerData.deccelInAir, playerData.jumpHangTimeThreshold, playerData.jumpHangAccelerationMult, playerData.jumpHangMaxSpeedMult, playerData.doConserveMomentum, IsWallJumping);
    }
}
