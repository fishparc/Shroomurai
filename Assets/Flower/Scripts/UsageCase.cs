using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Flower;
using UnityEngine.UI;

public class UsageCase : MonoBehaviour
{
    public simplemove simplemove;
    FlowerSystem flowerSys;
    private bool isTalking;
    private string myName;
    private int progress = 0;
    private bool pickedUpTheKey = false;
    private bool isGameEnd = false;
    private bool isLocked = false;
    private bool isReady = false;

    void Start()
    {
        var audioDemoFile = Resources.Load<AudioClip>("bgm") as AudioClip;
        if(!audioDemoFile){
            Debug.LogWarning("The audio file : 'bgm' is necessary for the demonstration. Please add to the Resources folder.");
        }

        flowerSys = FlowerManager.Instance.CreateFlowerSystem("FlowerSample",false);
        flowerSys.SetupDialog();

        // Setup Variables.
        myName = "Rempty (｢･ω･)｢";
        flowerSys.SetVariable("MyName", myName);

        // Define your customized commands.
        flowerSys.RegisterCommand("UsageCase", CustomizedFunction);
        // Define your customized effects.
        flowerSys.RegisterEffect("customizedRotation", EffectCustomizedRotation);
    }

    void Update()
    {
        // ----- Integration DEMO -----
        // Your own logic control.
        if(Input.GetKeyDown(KeyCode.F))
        {
            if (simplemove.isReady)
            {
                isTalking = true;
                flowerSys.ReadTextFromResource("Talk");
            }
        }

        //if (!isTalking)
        //{
        //    flowerSys.Stop();
        //    flowerSys.RemoveDialog();
        //}
        //else
        //{
        //    flowerSys.Resume();
        //    flowerSys.SetupDialog();
        //}


        if (!isGameEnd)
        {
            if(Input.GetKeyDown(KeyCode.Space)){
                // Continue the messages, stoping by [w] or [lr] keywords.
                flowerSys.Next();
            }
            if(Input.GetKeyDown(KeyCode.R)){
                // Resume the system that stopped by [stop] or Stop().
                flowerSys.Resume();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.name == "Frog")
        {
            Debug.Log("OK");
            isReady = true;
        }
        else
        {
            isReady = false;
        }
    }

    private void CustomizedFunction(List<string> _params)
    {
        var resultValue = int.Parse(_params[0]) + int.Parse(_params[1]);
        Debug.Log($"Hi! This is called by CustomizedFunction with the result of parameters : {resultValue}");
    }
    
    IEnumerator CustomizedRotationTask(string key, GameObject obj, float endTime){
        Vector3 startRotation = obj.transform.eulerAngles;
        Vector3 endRotation = obj.transform.eulerAngles + new Vector3(0,180,0);
        // Apply default timer Task.
        yield return flowerSys.EffectTimerTask(key, endTime, (percent)=>{
            // Update function.
            obj.transform.eulerAngles = Vector3.Lerp(startRotation, endRotation, percent);
        });
    }

    private void EffectCustomizedRotation(string key, List<string> _params){
        try{
            // Parse parameters.
            float endTime;
            try{
                endTime = float.Parse(_params[0])/1000;
            }catch(Exception e){
                throw new Exception($"Invalid effect parameters.\n{e}");
            }
            // Extract targets.
            GameObject sceneObj = flowerSys.GetSceneObject(key);
            // Apply tasks.
            StartCoroutine(CustomizedRotationTask($"CustomizedRotation-{key}", sceneObj, endTime));
        }catch(Exception){
            Debug.LogError($"Effect - SpriteAlphaFadeIn @ [{key}] failed.");
        }
    }
}
