using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;
    [SerializeField] private AudioMixer myMixer;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider effectSlider;
    [SerializeField] private AudioSource _musicSource, _effectSource;
    //保留Soundmanager不讓BGM重來,音量已被PlayerPref保存了
    void Awake()
    {
        if (GameObject.Find("MusicVolume Slider") == null)
        {
            Debug.Log("No Slider");
        }
        else
        {
            getSliders();
        }

        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            

        }
        else
        {
            Destroy(gameObject);
        }
        CheckthenLoad();
    }
    private void Start()
    {

        CheckthenLoad();
    }
    public void PlaySound(AudioClip clip)
    {
        _effectSource.PlayOneShot(clip);
    }
    /// <summary>
    /// audioMixer setvalue and save on 
    /// </summary>
    public void ChangeMusicVolume()
    {
        float volume = musicSlider.value;
        myMixer.SetFloat("Music", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("musicVolume", volume);

    }
    public void ChangeSoundVolume()
    {
        float volume = effectSlider.value;
        myMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        PlayerPrefs.SetFloat("SFXVolume", volume);

    }
    public void LoadVolume()
    {
        musicSlider.value = PlayerPrefs.GetFloat("musicVolume");
        effectSlider.value = PlayerPrefs.GetFloat("SFXVolume");
        ChangeMusicVolume();
        ChangeSoundVolume();
    }
    public void CheckthenLoad()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            LoadVolume();
        }
        else
        {
            ChangeMusicVolume();
            ChangeSoundVolume();
        }
    }

    public void getSliders()
    {

        musicSlider = GameObject.Find("MusicVolume Slider").GetComponent<Slider>();
        musicSlider.onValueChanged.AddListener(delegate { ChangeMusicVolume(); });
        effectSlider = GameObject.Find("EffectVolume Slider").GetComponent<Slider>();
        effectSlider.onValueChanged.AddListener(delegate { ChangeSoundVolume(); });
    }
    // Update is called once per frame
    void Update()
    {

    }
}
