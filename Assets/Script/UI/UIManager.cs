using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public  GameObject startMenuPanel;
    public  GameObject resumeMenuPanel;
    public  GameObject settingMenuPanel;
    private void Awake()
    {
        SetSettingPanel();
        SetResumePanel();
        SetStartMenuPanel();
    }

    private void Start()
    {

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);//keep UImanager
            //gameObject.SetActive(false);
            SetSettingPanel();
            SetResumePanel();
            SetStartMenuPanel();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void BackToTitle()
    {
        SceneManager.LoadScene("StartMenu");
        resumeMenuPanel.SetActive(false);
        SetStartMenuPanel();
        Debug.Log("scenesLoaded");

    }
    public void StartGame ()//現在是直接進關卡內 之後要進章節選單
    {
       SceneManager.LoadScene("UnitedScene");
       SetResumePanel();
       Debug.Log("playscenesLoaded");
    }
    //在暫停畫面沒開設定:On false
    public bool SettingOpened = false;
    public void ToggleRecentMenuAndSettings()
    {
        if (SettingOpened == false)
        {
            OpenSetting();
        }
        else
        {
            CloseSetting();
        }
    }
    public void OpenSetting()
    {
        Debug.Log("settingOpening");
        //切暫停與主選單
        if (resumeMenuPanel != null)
        {
            resumeMenuPanel.SetActive(false);
        }
        else if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(false);
        }
        settingMenuPanel.SetActive(true);
        SettingOpened = true;
    }
    public void CloseSetting()
    {
        Debug.Log("settingClosing");
        if (resumeMenuPanel != null)
        {
            resumeMenuPanel.SetActive(true);
        }
        else if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(true);
        }
        settingMenuPanel.SetActive(false);
        SettingOpened = false;
    }

    /// <summary>
    /// 找到PANEL並指派 場景中NAME:Setting's'Panel
    /// </summary>
    private void SetSettingPanel()
    {
        if (GameObject.Find("SettingsPanel") != null)
        {
            Debug.Log("Setting exist");
            settingMenuPanel = GameObject.Find("SettingsPanel");
        }
        else
        {
            Debug.Log("Setting missing!");
        }
    }
    private void SetResumePanel()
    {
        if (GameObject.Find("ResumePanel") != null)
        {
            Debug.Log("Resume exist");
            resumeMenuPanel = GameObject.Find("ResumePanel");
        }
        else
        {
            Debug.Log("Resume missing!");
        }
    }
    private void SetStartMenuPanel()
    {
        if (GameObject.Find("StartMenuPanel") != null)
        {
            Debug.Log("StartMenuP exist");
            startMenuPanel = GameObject.Find("StartMenuPanel");
        }
        else
        {
            Debug.Log("StartMenu missing!");
        }
    }
}
