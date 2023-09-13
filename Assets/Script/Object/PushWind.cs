using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PushWind : MonoBehaviour
{
    public GameObject wind;
    public float windSpeed;
    public float windTime;

    public void CreatePushWind()
    {
        var pushWind = Instantiate(wind , transform.position , transform.rotation);
        
        //pushWind.GetComponent<Rigidbody2D>().AddForce(transform.up * windSpeed);
        //StartCoroutine(nameof(MoveWind), pushWind);
        pushWind.GetComponentInChildren<Rigidbody2D>().AddForce(transform.up * windSpeed);
        Destroy(pushWind , windTime);
    }
    /*
    public IEnumerator MoveWind(GameObject pushWind)
    {
        Vector2 pos = pushWind.transform.localPosition;
        while (pos.x != targetPos) {
            pos.x = Mathf.MoveTowards(pos.x, -300f, windSpeed * Time.deltaTime);
            pushWind.transform.localPosition = pos;
            yield return null;
        }
    }
    */
}
