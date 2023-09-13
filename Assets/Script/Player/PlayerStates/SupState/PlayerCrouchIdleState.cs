using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCrouchIdleState : PlayerGroundedState
{
    public PlayerCrouchIdleState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
        player.CrouchIdleMove();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(xInput != 0)
            player.CheckDirectionToFace(xInput > 0);

        if(!isExitingState)
        {
            if(yInput != -1)
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}
