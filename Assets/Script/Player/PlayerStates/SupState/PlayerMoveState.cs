using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMoveState : PlayerGroundedState
{
    public PlayerMoveState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void DoChecks()
    {
        base.DoChecks();
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

        if(xInput != 0)
            player.CheckDirectionToFace(xInput > 0);

        if(!isExitingState)
        {
            if(xInput == 0)
            {
                stateMachine.ChangeState(player.IdleState);
            }
            else if(yInput == -1 && Mathf.Abs(player.CurrentVelocity.x) >= playerData.runMaxSpeed * 0.9f)
            {
                stateMachine.ChangeState(player.CrouchSlideState);
            }
            else if(yInput == -1 && Mathf.Abs(player.CurrentVelocity.x) < playerData.runMaxSpeed * 0.9f)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
        }
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.GroundMove(1, xInput, playerData.runMaxSpeed, playerData.runAccelAmount, playerData.runDeccelAmount);
    }
}
