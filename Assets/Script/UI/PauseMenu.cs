using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static PauseMenu Instance;
    public static bool GameIsPaused = false;
    public bool AchievementOpen;
    public bool settingOpening;

    public GameObject pauseMenuUI;
    public GameObject settingMenu;
    private void Start()
    {
        //SoundManager.Instance.getSliders();

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            pauseMenuUI.SetActive(false);
            settingMenu.SetActive(false);

        }
        else
        {
            Destroy(gameObject);
        }

    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                if (AchievementOpen != true)//成就開不能用ESC關暫停
                {
                    if (settingOpening != true)
                    {
                        Resume();
                    }
                    else
                    {
                        CloseSetting();
                    }
                }
                else
                {
                    AchievementOpen = false;
                }
            }
            else
            {
                if (SceneManager.GetActiveScene().name != "StartMenu")
                    Pause();
            }
            AchievementListIngame.Instance.GetAchieveSysButton();
        }

    }
    public void TurnOffPauseMenuOnBack()
    {
        //pauseMenuUI.SetActive(false);
        AchievementOpen = true;
    }
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }
    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }
    public void OpenSetting()
    {
        settingMenu.SetActive(true);
        settingOpening = true;
        SoundManager.Instance.getSliders();
    }
    public void CloseSetting()
    {
        settingMenu.SetActive(false);
        settingOpening = false;
    }
    public void BackToTitle()
    {
        Resume();
        CloseSetting();
        SceneManager.LoadScene("StartMenu");
        Debug.Log("scenesLoaded");

    }
}
