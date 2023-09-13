using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D coll;
    private Animator anim;
    public float speed , jumpForce;
    [SerializeField] private int jumpNumber;
    public Transform groundCheck;
    public LayerMask ground;
    public bool isGround , isJump;
    bool jumpPressed;
    int jumpCount;

    void Start() 
    {
        rb = GetComponent<Rigidbody2D>();
        coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;

        }  
    }
    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position , 0.1f , ground);
        GroundMovement();
        Jump();
        SwitchAnim();
    }

    void GroundMovement()
    {
        float HorizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(HorizontalMove * speed , rb.velocity.y);

        if(HorizontalMove != 0)
        {
            transform.localScale = new Vector3(HorizontalMove , 1 , 1);
        }
    }

    void Jump()
    {
        if(isGround)
        {
            jumpCount = jumpNumber;
            isJump = false;
        }
        if(jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x , jumpForce);
            jumpCount --;
            jumpPressed = false;
        }
        else if(jumpPressed && jumpCount > 0 && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x,jumpForce);
            jumpCount--;
            jumpPressed=false;
        }
    }

    void SwitchAnim()
    {
        anim.SetFloat("IsRunning" , Mathf.Abs(rb.velocity.x));

        if(isGround){
            anim.SetBool("IsFalling", false);
        }
        else if(!isGround && rb.velocity.y > 0)
        {
            anim.SetBool("IsJumping" , true);
        }
        else if (rb.velocity.y < 0)
        {
            anim.SetBool("IsJumping" , false);
            anim.SetBool("IsFalling" , true);
        }
    }
}
