using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterControl : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;

    public float jumpForce;
    public Transform groundcheck;
    public static bool isGround ;
    public bool isJump;
    bool jumpPressed;
    int jumpCount;
    public LayerMask ground;
    public float runspeed=20f;
    float h ;
    //bool jump=false;
    
    // Start is called before the first frame update
    void Start()
    {
        rb=GetComponent<Rigidbody2D>();
        coll=GetComponent<Collider2D>();
        anim =GetComponent<Animator>();
       
    }

    // Update is called once per frame
    void Update()
    {
         if(Input.GetKeyDown("space")&&jumpCount>0)
           {
            jumpPressed=true;
           }  

   
        }
    private void FixedUpdate() 
    {
        
        isGround= Physics2D.OverlapCircle(groundcheck.position, 0.05f ,ground);
        Jump();
        GroundMovement();
        SwitchAnim();
    }
void SwitchAnim()
{
if(Mathf.Abs(h)>0.05f)
{
    anim.SetBool("IsRunning",true);
}
else if (Mathf.Abs(h)<=0.05f)
{
    anim.SetBool("IsRunning",false);
}

if(isGround)
{
    anim.SetBool("IsFalling",false);
}
else if(!isGround&&rb.velocity.y>0)
{
    anim.SetBool("IsJumping",true);
}
else if(rb.velocity.y<0)
{
    anim.SetBool("IsJumping",false);
    anim.SetBool("IsFalling",true);
}

}


   void GroundMovement()
   {
      h =Input.GetAxisRaw("Horizontal");
      rb.velocity=new Vector2(h*runspeed,rb.velocity.y);
    
      if(h!=0)
      {
        transform.localScale= new Vector3(h,1,1);
      }

   }
   void Jump()
   {
    if(isGround)
    {
        jumpCount=2;
        isJump=false;
    }
    if(jumpPressed&&isGround)
    {
        isJump =true;
        rb.velocity= new Vector2(rb.velocity.x,jumpForce);
        jumpCount--;
        jumpPressed=false;
    }
    else if(jumpPressed&&jumpCount>0&&isJump)
    {
        rb.velocity = new Vector2(rb.velocity.x,jumpForce);
        jumpCount--;
        jumpPressed=false;
    }
   }
}
