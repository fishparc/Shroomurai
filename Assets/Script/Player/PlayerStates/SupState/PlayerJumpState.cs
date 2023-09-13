using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerJumpState : PlayerAbilityState
{
    public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        player.inputHandler.UseJumpInput();
        player.Jump();
        
        isAbilityDone = true;

        player.InAirState.SetJumping(true);
        player.InAirState.SetJumpCut(false);
    }
}
    
