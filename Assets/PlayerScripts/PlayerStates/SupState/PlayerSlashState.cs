using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlashState : PlayerAbilityState
{
     //public bool CanSlash { get; private set; }
     private bool isHolding;
     
    private bool slashUsed;
    private Vector2 SlashDirection;
    private bool SlashStopInput;
    private Vector2 SlashDirectionInput;
    private float lastSlashTime;
    public PlayerSlashState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        //CanSlash = true;
        player.inputHandler.UseMeleeInput();

        isHolding = true;
        slashUsed = false;

        player.AimPivot.gameObject.SetActive(true);
        Time.timeScale = playerData.slashHoldtimeScale;
        player.RB.drag = playerData.SlashDrag;

        startTime = Time.unscaledTime;
    }
    
    public override void LogicUpdate()
    {
        base.LogicUpdate();
         if(!isExitingState)
        {
            if(isHolding)
            {
                player.Anim.speed = 0;
                SlashStopInput = player.inputHandler.SlashAimStopInput;
                SlashDirectionInput = player.inputHandler.PointerDirectionInput;
                xInput = player.inputHandler.XInput;

                player.RotateAimPivot();

                if(SlashDirectionInput != Vector2.zero)
                {
                    SlashDirection = SlashDirectionInput;
                }

                AimFacingDirection();

                if(SlashStopInput || Time.unscaledTime >= startTime + playerData.maxHoldTime)
                {
                    //按住(瞄準時間結束)
                    player.Anim.speed = 1;
                    isHolding = false;
                    Time.timeScale = 1f;
                    startTime = Time.time;

                    player.AimPivot.gameObject.SetActive(false);
                    //trun

                }

                float angle = Mathf.Atan2(SlashDirection.y , SlashDirection.x) * Mathf.Rad2Deg;

                if(player.RB.transform.localScale.x == -1)
			        angle -= 90;
                
		        Quaternion rotation = Quaternion.AngleAxis(angle , Vector3.forward);
		        player.AimPivot.rotation = rotation;
            }
            else
            {
                if(!slashUsed)
                {
                    //Debug.Log("Slashed!boom");
                    player.AirSlash();
                    slashUsed = true;
                }
                //執行Slash
                //Slash時間
                if(Time.time >= startTime + playerData.fireballDuration)
                {
                    isAbilityDone = true;
                    lastSlashTime = Time.time;
                    player.RB.drag = 0f;
                }
            }
        }
    }
       public override void PhysicsUpdate()
    {
        base.PhysicsUpdate();

        if(isGrounded)
        {
            player.GroundMove(1, xInput, playerData.runMaxSpeed, playerData.runAccelAmount, playerData.runDeccelAmount);
        }
        else
            player.InAirMove(1, xInput, playerData.runMaxSpeed, playerData.runAccelAmount*playerData.accelInAir, playerData.runDeccelAmount*playerData.deccelInAir, playerData.jumpHangTimeThreshold, playerData.jumpHangAccelerationMult, playerData.jumpHangMaxSpeedMult, playerData.doConserveMomentum, false);
    }
     public bool CheckIfCanSlash()
    {
       return Time.time >= lastSlashTime + playerData.SlashCooldown;
    }
    void AimFacingDirection()
    {
        player.CheckDirectionToFace(SlashDirection.x > 0 && SlashDirection.x != 0);
    }

}
