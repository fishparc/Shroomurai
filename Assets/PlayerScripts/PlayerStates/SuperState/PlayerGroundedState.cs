using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGroundedState : PlayerState
{
    protected float xInput;
    protected float yInput;
    protected bool jumpInput;
    protected bool fireballInput;
    protected bool airPushInput;
    protected bool meleeInput;
    protected bool DashInput;
    protected bool isGrounded;

    public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {

    }

    public override void DoChecks()
    {
        base.DoChecks();

        player.OnGroundCheck();
        isGrounded = player.CheckIfGrounded();
    }

    public override void Enter()
    {
        base.Enter();

        player.FireballState.ResetCanFireball();
        player.AirPushState.ResetCanAirPush();
        player.DashState.ResetDashesLeft();
    }

    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.inputHandler.XInput;
        yInput = player.inputHandler.YInput;
        jumpInput = player.inputHandler.JumpInput();
        fireballInput = player.inputHandler.FireballInput();
        airPushInput = player.inputHandler.AirPushInput();
        meleeInput = player.inputHandler.MeleeInput();
        DashInput =player.inputHandler.DashInput();
        if (jumpInput && isGrounded)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if (!isGrounded)
        {
            stateMachine.ChangeState(player.InAirState);
        }
        else if (fireballInput && player.FireballState.CheckIfCanFireball())
        {
            stateMachine.ChangeState(player.FireballState);
        }
        else if (airPushInput && player.AirPushState.CheckIfCanAirPush())
        {
            stateMachine.ChangeState(player.AirPushState);
        }
        else if (meleeInput)
        {
            if (yInput != -1)// on Ground and no Crouch
            stateMachine.ChangeState(player.SlashState);
        }
        else if(DashInput&&yInput!=-1&&player.DashState.CheckIfCanDash())
        {
            stateMachine.ChangeState(player.DashState);
        }
      
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
