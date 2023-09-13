using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallDestroy : MonoBehaviour
{  
    private void Start() {
        Destroy(gameObject , 5f);
    }
   private void OnCollisionEnter2D(Collision2D other) {
    
        if(other.gameObject.tag == "Explosive")
        {
            Explosion explode = other.gameObject.GetComponent<Explosion>();
            explode.OnCollisionExplode();
        }
        else if(other.gameObject.tag == "WoodPillar")
        {
            WoodPillar woodPillar = other.gameObject.GetComponent<WoodPillar>();
            woodPillar.Collapse();
        }
        else if(other.gameObject.tag != "Player")
        {
            Destroy(gameObject);
        }
        Destroy(gameObject);
        Debug.Log(other.gameObject.name);
    }

}
