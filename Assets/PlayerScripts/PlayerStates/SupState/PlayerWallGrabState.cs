using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallGrabState : PlayerTouchingWallState
{
    private float yInput;
    private bool canGrab;
    public PlayerWallGrabState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();


        
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        yInput = player.inputHandler.YInput;
        canGrab = player.CanGrab();

        if(!isExitingState)
        {
            if (!canGrab || yInput < 0)//該牆不能抓或按住下-->滑掉
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
        }

        player.Grab();
    }
    public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();
    }
}
