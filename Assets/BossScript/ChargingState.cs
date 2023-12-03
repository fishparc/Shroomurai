using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ChargingState : StateMachineBehaviour
{
    Transform player;
    Rigidbody2D Rb;
    BossAttackChecks boss;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player=GameObject.FindGameObjectWithTag("Player").transform;
        Rb=animator.GetComponent<Rigidbody2D>();
        Vector2 dir = new Vector2(player.transform.position.x-Rb.position.x,0);
        boss=animator.GetComponent<BossAttackChecks>();
        boss.ToggleWarningBox();
        boss.DoDashStep(0.1f);
    }
    
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.BossLaido();
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        boss.ToggleWarningBox();
    }
    
  
}
