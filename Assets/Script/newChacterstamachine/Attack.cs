using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int damage;
    public Vector2 knockbackDirection = new Vector2(1, 0);
    public float knockbackStrengh;
    public float attackRange;
    public float attackRate;
    private void OnTriggerStay2D(Collider2D other)
    {
        other.GetComponent<PlayerStats>()?.TakeDamage(this);
    }
}
