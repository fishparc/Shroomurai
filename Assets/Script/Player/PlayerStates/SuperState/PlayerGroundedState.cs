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

        if(jumpInput && isGrounded)
        {
            stateMachine.ChangeState(player.JumpState);
        }
        else if(!isGrounded)
        {
            stateMachine.ChangeState(player.InAirState);
        }
        else if(fireballInput && player.FireballState.CheckIfCanFireball())
        {
            stateMachine.ChangeState(player.FireballState);
        }
        else if(airPushInput && player.AirPushState.CheckIfCanAirPush())
        {
            stateMachine.ChangeState(player.AirPushState);
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
