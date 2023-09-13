using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowReflectionDestroy : MonoBehaviour
{
    [SerializeField] private int bounceableLefted = 3;
    // Start is called before the first frame update
    void Start()
    {
        _rb = this.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        bounceableLefted--;
        if (bounceableLefted != 0)
        {
            this.transform.right=Vector2.Reflect(this.transform.right,other.contacts[0].normal);
            switch (other.gameObject.tag)
            {
                case "Explosive":
                   /* Explosion explode = other.gameObject.GetComponent<Explosion>();
                    explode.OnCollisionExplode();
                    Destroy(gameObject, 2);*/
                    break;

                case "WoodPillar":
                    WoodPillar woodPillar = other.gameObject.GetComponent<WoodPillar>();
                    woodPillar.Collapse();
                    //ReflectProjectile(_rb, other.contacts[0].normal);
                    break;

                case "Player":
                    Debug.Log("HitP");
                    //Destroy(gameObject);

                    //not now maybe future call DoDamage or something idk
                    break;

                default:
                    //bounce;
                    Debug.Log("Noramally Bouncing!");
                    //ReflectProjectile(_rb, other.contacts[0].normal);
                    break;

            }
        }
        else
        {
            Destroy(gameObject);
        }
        Destroy(gameObject,5);
        Debug.Log(other.gameObject.name);
    }
    private Rigidbody2D _rb;
    private Vector2 _velocity;
    private void ReflectProjectile(Rigidbody2D rb, Vector2 reflectVector)
    {
        Debug.Log("Reflecting!");
        _velocity = Vector2.Reflect(_velocity, reflectVector);
        _rb.velocity = _velocity;
    }
}