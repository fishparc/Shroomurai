using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollapsePlatform : MonoBehaviour
{
    public float collapseTimer;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision) {

        if (collision.gameObject.tag == "Player")
        {
            StartCoroutine(CollapseCoroutine());
        }
    }
    IEnumerator CollapseCoroutine()
    {
        // 直接執行這裡...

        // 停兩秒
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
        Destroy(gameObject);

        // 兩秒之後才執行這裡
    }
}
