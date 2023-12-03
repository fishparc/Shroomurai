using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallDestroy : MonoBehaviour
{
    BossAttackChecks boss;
    private void Start()
    {
        boss=GameObject.FindGameObjectWithTag("Boss").GetComponent<BossAttackChecks>();
        Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Boss")
        {
            boss.TakeDamage(50);
        }
        else if (other.gameObject.tag == "Explosive")
        {
            Explosion explode = other.gameObject.GetComponent<Explosion>();
            explode.OnCollisionExplode();
        }
        else if (other.gameObject.tag == "WoodPillar")
        {
            WoodPillar woodPillar = other.gameObject.GetComponent<WoodPillar>();
            woodPillar.Collapse();
        }
        else if (other.gameObject.tag != "Player")
        {
            //Destroy(gameObject);
        }
        //Destroy(gameObject);
        Debug.Log(other.gameObject.name);
    }

}
