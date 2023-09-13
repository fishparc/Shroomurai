using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class MovingSpike : MonoBehaviour
{
    public Transform leftPoint, rightPoint;
    public float speed=1;
    private float leftX, rightX;
    private bool ToLeft = true;
    private bool IsCoroutine = false;
    // Start is called before the first frame update
    void Start()
    {
        transform.DetachChildren();
        leftX = leftPoint.position.x;
        rightX = rightPoint.position.x;
        Destroy(leftPoint.gameObject);
        Destroy(rightPoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        //Movement();
        if (!IsCoroutine)
        {
            if (ToLeft)
            {
                IsCoroutine=true;
                StartCoroutine(IEPatrolLeft());
            }
            else
            {
                IsCoroutine=true;
                StartCoroutine(IEPatrolRight());
            }
        }
    }

    /*void Movement()
    {
        if (ToLeft)
        {
            rb.velocity = new Vector2(-speed,rb.velocity.y);
            if (transform.position.x < leftX)
            {
                transform.localScale = new Vector2(-1,1);
                ToLeft = false;
            }
        }
        else
        {
            rb.velocity = new Vector2(speed,rb.velocity.y);
            if (transform.position.x > rightX)
            {
                transform.localScale = new Vector2(1,1);
                ToLeft = true;
            }
        }
    }*/

    /*private IEnumerator IEPatrolMechanic()
    {
        if (ToLeft)
        {
            while (transform.position.x > leftX)
            {
                Vector2 pos = transform.localPosition;
                //後面的transform.position.x放錯了 要跟前面的pos.x一模一樣才對
                pos.x = Mathf.MoveTowards(transform.position.x, leftX, -speed/1000);
                transform.localPosition = pos;
                yield return null;
            }
        }
        else 
        {
            while (transform.position.x < rightX)
            {
                Vector2 pos = transform.localPosition;
                pos.x = Mathf.MoveTowards(transform.position.x, rightX, speed/1000);
                transform.localPosition = pos;
                yield return null;
            }
        }
    }*/
    
    private IEnumerator IEPatrolLeft()
    {
        while (transform.position.x > leftX)
        {
            Vector2 pos = transform.localPosition;
            pos.x = Mathf.MoveTowards(pos.x, leftX, -speed/10);
            transform.localPosition = pos;
            yield return null;
        }
        ToLeft=false;
        IsCoroutine=false;
    }

    private IEnumerator IEPatrolRight()
    {
        while (transform.position.x < rightX)
        {
            Vector2 pos = transform.localPosition;
            pos.x = Mathf.MoveTowards(pos.x, rightX, speed/10);
            transform.localPosition = pos;
            yield return null;
        }
        ToLeft=true;
        IsCoroutine=false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.gameObject.tag=="Player")
        {
            PlayerDeath playerdeath = collision.gameObject.GetComponent<PlayerDeath>();
            playerdeath.Death();
        }
    }
}
