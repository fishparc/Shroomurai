using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletTime : MonoBehaviour
{
   /* public float slowdownFactor =0.05f;
    public float slowdownLength =2F;*/
    
    public float slowMotionTimescale;

    private float startTimescale;
    private float startFixedDeltaTime;
    void Start()
    {
        startTimescale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            StartSlowMotion();
        }

        if (Input.GetKeyUp(KeyCode.F))
        {
            StopSlowMotion();
        }
        /*Time.timeScale +=(1f/slowdownLength)*Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale,0f,1f);*/
    }
    /*public void DoBulletTime()
    {
     Time.timeScale =slowdownFactor;
     Time.fixedDeltaTime=Time.timeScale*.02f;
    }*/
    private void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale;
    }

    private void StopSlowMotion()
    {
        Time.timeScale = startTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }
}

