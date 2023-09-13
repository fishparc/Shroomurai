using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurrentFiring : MonoBehaviour
{
    public TurrentData turrentData;
    [Header("ReadfromData")]
    public float range;
    public float fireRate;
    public float force;
    public Transform targetedPerson;
    [Header("ManualAccessVVV")]
    public SpriteRenderer spriteRenderer;
    public GameObject gun;
    public GameObject bullet;
    public Transform shootpoint;
    Vector2 Direction;
    bool Detected = false;
    float nextTimeToFire = 0;
    // Start is called before the first frame update
    void Start()
    {
        range = turrentData.range;
        fireRate = turrentData.fireRate;
        force = turrentData.pulse;
        spriteRenderer.sprite = turrentData.artillery_Base;
        if (targetedPerson == null)
        {
            targetedPerson = GameObject.FindGameObjectWithTag(turrentData.targetTagName).GetComponent<Transform>();
        }
        StartCoroutine(Detection());
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 targetPos = targetedPerson.transform.position;
        Direction = targetPos - (Vector2)transform.position;
    }
    void shoot()
    {
        GameObject BulletIns = Instantiate(bullet, shootpoint.position, gun.transform.rotation);
        BulletIns.GetComponent<Rigidbody2D>().velocity=Direction * force*0.1f;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, range);
    }
    IEnumerator Detection()
    {
        while (true)
        {
            yield return new WaitForSeconds(2);
            //Debug.Log("2isOn");
            RaycastHit2D rayInfo = Physics2D.Raycast(transform.position, Direction, range);
            if (rayInfo)
            {
                if (rayInfo.collider.gameObject.tag == turrentData.targetTagName)
                {
                    if (Detected == false)
                    {
                        Detected = true;
                        Debug.Log("trueonein");
                    }
                }
                else
                {

                    if (Detected == true)
                    {
                        Detected = false;
                    }
                }
                if (Detected)
                {
                    gun.transform.right = Direction;
                    shoot();
                }
            }
        }
    }
}
