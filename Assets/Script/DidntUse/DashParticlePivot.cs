using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashParticlePivot : MonoBehaviour
{
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.X))
        {
            DashParticle();
        }
    }
    public GameObject dashParticle;
    public Transform Owner;

    // Update is called once per frame

    public void DashParticle()
    {
        Debug.Log(transform.rotation);
        var firedBullet = Instantiate(dashParticle , transform.position , transform.rotation);
        firedBullet.transform.parent = Owner.transform;

    }
}
