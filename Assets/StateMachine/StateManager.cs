using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StateManager<EState> : MonoBehaviour where EState : Enum
{
    protected Dictionary<EState,BaseState<EState>>States =new Dictionary<EState, BaseState<EState>>();
    protected BaseState<EState>CurrentState;
 void Start() 
 {
    
 }
 void Update()
 {

 }
 void OnTriggerEnter(Collider other)
 {

 }
 void OnTriggerStay(Collider other)
 {
    
 }
  void OnTriggerExit(Collider other)
 {
    
 }
}
