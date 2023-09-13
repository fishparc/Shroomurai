using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionEffectPS : MonoBehaviour
{

    public GameObject disassemblePS;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.I)){
            DisassemblePS(transform);
        }
    }


    public void DisassemblePS(Transform bomb)
    {
        gameObject.SetActive(false);
        float angle = AngleCal(bomb);
        Debug.Log(angle);
        var disassembleParticle = Instantiate(disassemblePS , transform.position , Quaternion.Euler(0 , 0 , angle));
        disassembleParticle.SetActive(true);
        Destroy(disassembleParticle , UnityEngine.Random.Range(3.5f , 4.5f));
    }

    public float AngleCal(Transform bomb)
    {
        Vector2 distacneVector = this.transform.position - bomb.position;
        return Mathf.Atan(distacneVector.y/distacneVector.x) * Mathf.Rad2Deg;
    }
}
