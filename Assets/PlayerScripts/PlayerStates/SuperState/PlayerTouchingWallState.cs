using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchingWallState : PlayerState
{
    protected bool isGrounded;
    protected bool isOnWall;
    protected float xInput;
    protected bool jumpInput;
    public bool IsOnWall { get; private set; }
    public PlayerTouchingWallState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();

        player.OnGroundCheck();
        isGrounded = player.CheckIfGrounded();

        player.OnWallCheck();
        isOnWall = player.CheckIfOnWall();
    }

    public override void Enter()
    {
        base.Enter();

        IsOnWall = true;
    }

    public override void Exit()
    {
        base.Exit();

        IsOnWall = false;
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        xInput = player.inputHandler.XInput;
        jumpInput = player.inputHandler.JumpInput();

        if(jumpInput)
        {
            stateMachine.ChangeState(player.WallJumpState);
        }
        else if(isGrounded)
        {
            stateMachine.ChangeState(player.IdleState);
        }
        else if(!OnWall())
        {
            stateMachine.ChangeState(player.InAirState);
        }

        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }

    public bool OnWall() => isOnWall && XInputAtWall();
    public bool XInputAtWall() => (player.LastOnWallLeftTime > 0 && xInput < 0) || (player.LastOnWallRightTime > 0 && xInput > 0);
}
