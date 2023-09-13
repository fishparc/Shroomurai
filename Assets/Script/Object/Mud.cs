using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mud : MonoBehaviour
{
    //public float onMudTime { get; private set; }
    [SerializeField] float mudAccel;

    private void Update() {
        //onMudTime -= Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision) 
    {
        PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();

        if(collision.gameObject.tag == "Player" && player.LastOnMudTime > 0)
        {
            //onMudTime = 2f;
        }
    }


    private void OnCollisionStay2D(Collision2D collision) 
    {
        if(collision.gameObject.tag == "Player")
        {
            //Sleep(0.1f);
            PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
            Rigidbody2D RB = collision.gameObject.GetComponent<Rigidbody2D>();

            //Debug.Log(player.isOnMud);
            
            //Debug.Log(onMudTime);

            if(player.LastOnMudTime > 0)
            {
                float speedDif = 0 - RB.velocity.x;
		        float movement = speedDif * mudAccel;
		        movement = Mathf.Clamp(movement, -Mathf.Abs(speedDif)  * (1 / Time.fixedDeltaTime), Mathf.Abs(speedDif) * (1 / Time.fixedDeltaTime));
                RB.AddForce(movement * Vector2.right);
            }
        }
    }
}
