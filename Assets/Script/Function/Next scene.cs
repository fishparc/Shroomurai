using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Nextscene : MonoBehaviour
{
  private void OnTriggerEnter2D(Collider2D other) 
  {
    Debug.Log("penis" + other.gameObject.name);
      SceneManager.LoadScene("UnitedScene 1");  
  }
}
