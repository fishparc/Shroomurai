using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reference : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
/*
   if()
    Debug.Log("do the dash?");
            //Freeze game for split second. Adds juiciness and a bit of forgiveness over directional input
            player.Sleep(playerData.dashSleepTime);
            //If not direction pressed, dash forward
            if (_moveInput != Vector2.zero)
            {
                _lastDashDir = _moveInput;
            }
            else
                _lastDashDir = Vector2.right;//MAYBEONLY RIGHT FIX later
            DashesLeft--;
            player.SetGravityScale(0);
            //We keep the player's velocity at the dash speed during the "attack" phase (in celeste the first 0.15s)
        while (Time.time - startTime <= playerData.dashAttackTime)
        {
            player.RB.velocity = _lastDashDir.normalized * playerData.dashSpeed;
            //RB.velocity = dir.normalized * Data.dashSpeed;
            //RB.AddForce(dir.normalized * Data.dashSpeed , ForceMode2D.Force);
            //Pauses the loop until the next frame, creating something of a Update loop.
            //This is a cleaner implementation opposed to multiple timers and this coroutine approach is actually what is used in Celeste :D
            //yield return null;
        }
        startTime = Time.time;
        player.SetGravityScale(playerData.gravityScale);

         while (Time.time - startTime <= playerData.dashEndTime)
        {
          player.RB.velocity = playerData.dashEndSpeed * _lastDashDir.normalized;
        }*/
}
