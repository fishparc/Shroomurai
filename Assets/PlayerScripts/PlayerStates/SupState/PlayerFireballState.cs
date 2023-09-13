using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerFireballState : PlayerAbilityState
{
    public bool CanFireball { get; private set; }

    private bool isHolding;
    private bool fireballUsed;

    private Vector2 fireballDirection;
    
    private bool fireballStopInput;
    private Vector2 fireballDirectionInput;
    //private float xInput;

    private float lastFireballTime;
    public PlayerFireballState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animBoolName) : base(player, stateMachine, playerData, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();

        CanFireball = false;
        player.inputHandler.UseFireballInput();

        isHolding = true;
        fireballUsed = false;

        player.AimPivot.gameObject.SetActive(true);
        Time.timeScale = playerData.fireballHoldtimeScale;
        player.RB.drag = playerData.fireballDrag;

        startTime = Time.unscaledTime;
    }
    public override void Exit()
    {
        base.Exit();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if(!isExitingState)
        {
            if(isHolding)
            {
                player.Anim.speed = 0;
                fireballStopInput = player.inputHandler.FireballStopInput;
                fireballDirectionInput = player.inputHandler.PointerDirectionInput;
                xInput = player.inputHandler.XInput;

                player.RotateAimPivot();

                if(fireballDirectionInput != Vector2.zero)
                {
                    fireballDirection = fireballDirectionInput;
                }

                AimFacingDirection();

                if(fireballStopInput || Time.unscaledTime >= startTime + playerData.maxHoldTime)
                {
                    //按住(瞄準時間結束)
                    player.Anim.speed = 1;
                    isHolding = false;
                    Time.timeScale = 1f;
                    startTime = Time.time;

                    player.AimPivot.gameObject.SetActive(false);
                    //trun

                }

                float angle = Mathf.Atan2(fireballDirection.y , fireballDirection.x) * Mathf.Rad2Deg;

                if(player.RB.transform.localScale.x == -1)
			        angle -= 90;
                
		        Quaternion rotation = Quaternion.AngleAxis(angle , Vector3.forward);
		        player.AimPivot.rotation = rotation;
            }
            else
            {
                if(!fireballUsed)
                {
                    player.FireProjectile();
                    fireballUsed = true;
                }
                //執行丟火球
                //丟火球時間
                if(Time.time >= startTime + playerData.fireballDuration)
                {
                    isAbilityDone = true;
                    lastFireballTime = Time.time;
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

    public bool CheckIfCanFireball()
    {
        return CanFireball && Time.time >= lastFireballTime + playerData.fireballCooldown;
    }

    public void ResetCanFireball() => CanFireball = true;

    void AimFacingDirection()
    {
        player.CheckDirectionToFace(fireballDirection.x > 0 && fireballDirection.x != 0);
    }
}
