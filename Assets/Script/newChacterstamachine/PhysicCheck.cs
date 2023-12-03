using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using UnityEngine;

public class PhysicCheck : MonoBehaviour
{
    [Header("Status")]
    public bool onGround;
    public bool onWall;

    public int canHangWhichWall;

    [Header("Assist")]
    public float coyoteTime;
    public float groundHourglass;
    public float wallHourglass;
    [Header("Checksbox")]
    public Transform groundCheckPoint;
    public LayerMask groundLayer;
    public Vector2 groundCheckPointOffset;
    public Vector2 groundCheckSize = new Vector2(1, 0.5f);
    public LayerMask wallLayer;
    public Vector2 wallCheckFrontPointOffset;
    public Vector2 wallCheckBackPointOffset;
    public Vector2 wallCheckSize = new Vector2(1, 0.5f);
    void Update()
    {
        Check();
        CheckHangableWall();
        Hourglasses();
    }
    public void Check()
    {
        onGround = Physics2D.OverlapBox((Vector2)groundCheckPoint.position + groundCheckPointOffset, groundCheckSize, 0, groundLayer);
        if (onGround)
        {
            groundHourglass = coyoteTime;
        }
        onWall =Physics2D.OverlapBox((Vector2)groundCheckPoint.position + wallCheckFrontPointOffset, wallCheckSize, 0, groundLayer)||Physics2D.OverlapBox((Vector2)groundCheckPoint.position + wallCheckBackPointOffset, wallCheckSize, 0, groundLayer);
       if(onWall)
       {
        wallHourglass=coyoteTime;
       }

    }
    public void Hourglasses()
    {
        groundHourglass = Mathf.Max(groundHourglass - Time.deltaTime, 0);
        wallHourglass = Mathf.Max(wallHourglass - Time.deltaTime, 0);
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube((Vector2)groundCheckPoint.position + groundCheckPointOffset, groundCheckSize);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube((Vector2)groundCheckPoint.position + wallCheckFrontPointOffset, wallCheckSize);

        Gizmos.DrawWireCube((Vector2)groundCheckPoint.position + wallCheckBackPointOffset, wallCheckSize);
    }
    public void CheckHangableWall()
    {
        if (Physics2D.OverlapBox((Vector2)groundCheckPoint.position + wallCheckFrontPointOffset, wallCheckSize, 0, groundLayer) != null)
        {
            if (Physics2D.OverlapBox((Vector2)groundCheckPoint.position + wallCheckFrontPointOffset, wallCheckSize, 0, groundLayer).tag == "HangAble")
                canHangWhichWall = 1;
            else
                canHangWhichWall = -1;
        }
        else if (Physics2D.OverlapBox((Vector2)groundCheckPoint.position + wallCheckBackPointOffset, wallCheckSize, 0, groundLayer) != null)
        {
            if (Physics2D.OverlapBox((Vector2)groundCheckPoint.position + wallCheckBackPointOffset, wallCheckSize, 0, groundLayer).tag == "HangAble")
                canHangWhichWall = 0;//boolean false
            else
                canHangWhichWall = -2;
        }
        else
            canHangWhichWall = -3;
    }
}
