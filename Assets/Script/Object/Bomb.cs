using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Explosion
{
    private bool hasCollided = false;
    private Animator anim;

    public bool killdisable = false;

    private void Start() {
        anim = GetComponent<Animator>();
    }

    private void OnCollisionEnter2D(Collision2D other) {

        //Debug.Log(other.gameObject.name);
        hasCollided = true;
        if(hasCollided)
        {
            if(!killdisable)
            {
                anim.SetBool("IsExplode" , true);
            }
            OnCollisionExplode();
            
        }
        hasCollided = false;
    }
    private void Kill()
    {
        Destroy(gameObject);
    }
    
    
}
