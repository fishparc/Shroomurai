using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{

    public float XInput { get; private set; }
    public float YInput { get; private set; }
    public float LastPressedJumpTime { get; private set; }
    public float LastPressedFireballTime { get; private set; }
    public float LastPressedAirPushTime { get; private set; }
    public float LastPressedMeleeTime { get; private set; }
    public float LastPressedDashTime { get; private set; }
    public bool JumpCutInput { get; private set; }
    public bool FireballStopInput { get; private set; }
    public bool SlashAimStopInput { get; private set; }
    public Vector2 RawPointerDirectionInput { get; private set; }
    public Vector2 PointerDirectionInput { get; private set; }

    [SerializeField] private PlayerData playerData;

    // Update is called once per frame
    void Update()
    {
        LastPressedJumpTime -= Time.deltaTime;
        LastPressedFireballTime -= Time.deltaTime;
        LastPressedAirPushTime -= Time.deltaTime;
        LastPressedMeleeTime -= Time.deltaTime;
        LastPressedDashTime-=Time.deltaTime;
        
        XInput = Input.GetAxisRaw("Horizontal");
        YInput = Input.GetAxisRaw("Vertical");

        if (Input.GetButtonDown("Jump"))
        {
            OnJumpInput();
            JumpCutInput = false;
        }
        if (Input.GetButtonUp("Jump"))
        {
            JumpCutInput = true;
        }
        if (Input.GetButtonDown("Fire"))
        {
            OnFireballInput();
            FireballStopInput = false;
        }
        if (Input.GetButtonUp("Fire"))
            FireballStopInput = true;
        if (Input.GetButtonDown("Air"))
        {
            OnAirPushInput();
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Debug.Log("Qpressed");
            OnMeleeInput();
            SlashAimStopInput = false;
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            SlashAimStopInput = true;
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Debug.Log("Cispressed");
            OnDashInput();
        }
        OnPointerDirectionInput();

        //Debug.Log(JumpCutInput);
    }


    public bool JumpInput() => LastPressedJumpTime > 0;
    public bool FireballInput() => LastPressedFireballTime > 0;
    public bool AirPushInput() => LastPressedAirPushTime > 0;
    public bool MeleeInput() => LastPressedMeleeTime > 0;
    public bool DashInput() => LastPressedDashTime > 0;

    public void OnJumpInput() => LastPressedJumpTime = playerData.jumpInputBufferTime;
    public void OnFireballInput() => LastPressedFireballTime = playerData.fireballInputBufferTime;
    public void OnAirPushInput() => LastPressedAirPushTime = playerData.airPushInputBufferTime;
    public void OnMeleeInput() => LastPressedMeleeTime = playerData.meleeInputBufferTime;
    public void OnDashInput() => LastPressedDashTime = playerData.dashInputBufferTime;
    void OnPointerDirectionInput()
    {
        RawPointerDirectionInput = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        PointerDirectionInput = RawPointerDirectionInput.normalized;
    }

    public void UseJumpInput() => LastPressedJumpTime = 0;
    public void UseFireballInput() => LastPressedFireballTime = 0;
    public void UseAirPushInput() => LastPressedAirPushTime = 0;
    public void UseMeleeInput() => LastPressedMeleeTime = 0;
    public void UseDashInput() => LastPressedDashTime = 0;
}
