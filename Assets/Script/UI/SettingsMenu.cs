using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public static SettingsMenu Instance;
    public GameObject settingMenu;
    private bool MenuOpen;
    private void Awake() 
    {
    
    }
    private void Start()
    {
       // settingMenu=GameObject.Find("SettingsPanel");

      if(Instance==null)
    {
        Instance=this;
        DontDestroyOnLoad(gameObject);//keep savedCanvas Settingsmenu
        settingMenu.SetActive(false);
    }
        else{
        Destroy(gameObject);
    }
    }

    /// <summary>
    /// Closes the UI window.
    /// </summary>
    public void CloseWindow()
    {
        MenuOpen = false;
        settingMenu.SetActive(MenuOpen);
    }
    /*
    /// <summary>
    /// Opens the UI window.
    /// </summary>
    public void OpenWindow()
    {
        MenuOpen = true;
        settingMenu.SetActive(MenuOpen);
    }
    /// <summary>
    /// Toggles the state of the UI window open or closed
    /// </summary>
    public void ToggleWindow()
    {
        if (MenuOpen){
            CloseWindow();
        }
        else{
            OpenWindow();
        }
    }*/
}
