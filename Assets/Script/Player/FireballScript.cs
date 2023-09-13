using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballScript : MonoBehaviour
{
    public Rigidbody2D bullet;
    public float fireSpeed = 500f;

    // Update is called once per frame

    public void Fireball()
    {
        var firedBullet = Instantiate(bullet , transform.position , transform.rotation);

        firedBullet.AddForce(transform.up * fireSpeed);
    }
}
