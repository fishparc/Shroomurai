using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicCheck : MonoBehaviour
{
    [Header("Status")]
    public bool onGround;
    public bool canJump;//InCoyoteTime
    public bool isWallTouched;
    public bool ishangableWall;
    [Header("Assist")]
    public float coyoteTime;
    public float groundHourglass;
    [Header("Checksbox")]
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public Vector2 groundCheckPointOffset;
    public Vector2 groundCheckSize = new Vector2(1, 0.5f);
    public LayerMask wallLayer;
    public Vector2 wallCheckPointOffset;
    public Vector2 wallCheckSize = new Vector2(1, 0.5f);
    void Update()
    {
        Check();
        Hourglasses();
    }
    public void Check()
    {
        onGround = Physics2D.OverlapBox((Vector2)groundCheckPoint.position + groundCheckPointOffset, groundCheckSize, 0, groundLayer);
        if (onGround)
        {
            groundHourglass = coyoteTime;
        }
        canJump = groundHourglass > 0;

    }
    public void Hourglasses()
    {
        groundHourglass = Mathf.Max(groundHourglass - Time.deltaTime, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube((Vector2)groundCheckPoint.position + groundCheckPointOffset, groundCheckSize);
    }
}


