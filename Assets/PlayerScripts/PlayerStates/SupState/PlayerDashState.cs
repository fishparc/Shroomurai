using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerAbilityState
{
    private bool DashUsed;
    private int DashesLeft=0;
    private Vector2 _lastDashDir;
    public PlayerDashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        DashUsed = false;
        player.inputHandler.UseDashInput();

    }
        public override void Exit()
    {
        base.Exit();
    }
    public override void LogicUpdate()
    {
        base.LogicUpdate();
        if (!isExitingState)
        {
            if (!DashUsed&&DashesLeft>0)
                //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
                player.Sleep(playerData.dashSleepTime);
            //If not direction pressed, dash forward
            if ((player.inputHandler.XInput, player.inputHandler.YInput) != (0, 0))
            {
                _lastDashDir = new Vector2(player.inputHandler.XInput, player.inputHandler.YInput).normalized;
            }
            else
                _lastDashDir =new Vector2(player.FacingDirection,0);
            
            player.GoDash(_lastDashDir);
            DashesLeft--;
            Debug.Log("Dashleft:"+DashesLeft);
            DashUsed = true;
            isAbilityDone = true;
        }
    }
        public bool CheckIfCanDash()
    {
        return DashesLeft>0;
    }
    public void ResetDashesLeft() => DashesLeft=playerData.dashAmount;
}
