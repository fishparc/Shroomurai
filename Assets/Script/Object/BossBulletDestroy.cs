using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletDestroy : MonoBehaviour
{
    Player playerScript;
    private void Start()
    {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        Destroy(gameObject, 5f);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerScript.TakeDamage(50);
            Destroy(gameObject);
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
        //Debug.Log(other.gameObject.name);
    }
}
