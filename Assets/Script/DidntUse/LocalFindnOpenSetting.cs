using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalFindnOpenSetting : MonoBehaviour
{
    //[SerializeField] private Button settingButton;
    public GameObject SettingMenu;
    private void Awake() 
    {
      SetSetting();
    }
    void Start()
    { 
      SetSetting(); 
    }
   
    // Update is called once per frame
    void Update()
    {
        
    }
    /// <summary>
    /// 在切換後得SCENE找SETTINGprefab讓該關BUTTON可以找到並開啟
    /// </summary>
public void OpenSetting ()
    {
        SettingMenu.SetActive(true);
    }
    /*
public void ExitSetting ()
    {
        SettingMenuUI.SetActive(false);
    }*/
public void existOnot()
{
 if (SettingMenu != null)
        {
            Debug.Log("SavedC preexist");
    }
    else{
        Debug.Log("SaveC MISSING!");
    }
}
private void SetSetting()
{
        
       if(GameObject.Find("SettingsPanel")!=null)
       {
       SettingMenu=GameObject.Find("SettingsPanel");
       }
       else
       {
     existOnot();
       }
}
}
