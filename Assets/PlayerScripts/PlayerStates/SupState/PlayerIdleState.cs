using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerGroundedState
{
    public PlayerIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
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
            if(xInput != 0)
            {
                stateMachine.ChangeState(player.MoveState);
            }
            else if(yInput == -1)
            {
                stateMachine.ChangeState(player.CrouchIdleState);
            }
        }

        
    }

    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
        player.GroundMove(1, 0, 0, playerData.runAccelAmount, playerData.runDeccelAmount);
    }
}
