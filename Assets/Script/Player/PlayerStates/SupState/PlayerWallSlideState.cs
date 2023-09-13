using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallSlideState : PlayerTouchingWallState
{

    private bool canGrab;
    private float yInput;
    public PlayerWallSlideState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        canGrab = player.CanGrab();
        yInput = player.inputHandler.YInput;

        if(!isExitingState)
        {
            if (canGrab && yInput == 0)
            {
                stateMachine.ChangeState(player.WallGrabState);
            }
        }

    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        player.WallSlide();
    }
}
