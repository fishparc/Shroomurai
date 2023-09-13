using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerCrouchSlideState : PlayerGroundedState
{
    public PlayerCrouchSlideState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(yInput != -1)
        {
            stateMachine.ChangeState(player.MoveState);
        }
        else if(Mathf.Abs(player.CurrentVelocity.x) < 0.01f)
        {
            stateMachine.ChangeState(player.CrouchIdleState);
        }
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.CrouchSlideMove(playerData.crouchSlideLerp);

    }
}
