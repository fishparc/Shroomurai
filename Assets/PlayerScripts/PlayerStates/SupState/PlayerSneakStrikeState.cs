using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSneakStrikeState : PlayerAbilityState
{
    public PlayerSneakStrikeState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public bool CanSneakStrike { get; private set; }
    private bool sneakStrikeUsed;
    private float lastSneakStrikeTime;
    public override void Enter()
    {
        base.Enter();

        CanSneakStrike = false;
        sneakStrikeUsed = false;
        player.inputHandler.UseMeleeInput();

        player.RB.drag = playerData.sneakStrikeDrag;
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (Time.time >= startTime + playerData.sneakStrikeTime && !sneakStrikeUsed)
        {
            //Fapping- funct
            player.SwingSneakStrike();
            sneakStrikeUsed = true;
        }

        if (Time.time >= startTime + playerData.sneakStrikeDuration)
        {
            player.RB.drag = 0;
            isAbilityDone = true;
            lastSneakStrikeTime = Time.time;
        }
    }

    public bool CheckIfCanSneakStrike()
    {
        return CanSneakStrike && Time.time >= lastSneakStrikeTime + playerData.sneakStrikeCooldown;
    }
    public void ResetCanSneakStrike() => CanSneakStrike = true;

}
