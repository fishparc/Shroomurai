using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D trigger) {
        if(trigger.tag == "WindArea")
        {
        }
    }
}
