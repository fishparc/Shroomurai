using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrushableWall : MonoBehaviour
{
    public GameObject crashableWall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D other) 
    {
    if(other.gameObject.tag=="Player")
    {
        //動畫RUN2F
      Destroy(crashableWall,2f);
    }

    }
}
