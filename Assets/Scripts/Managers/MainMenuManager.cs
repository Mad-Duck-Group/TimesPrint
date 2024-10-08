using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    private static MainMenuManager _instance;
    public static MainMenuManager Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogError("MainMenuManager is null");
            }
            return _instance;
        }
    }
    
    [SerializeField] private Slider volumeSlider;
    private AudioSource _bgmAudioSource;
    
    private void Awake()
    {
        _instance = this;
        volumeSlider.value = SoundManager.Instance.MasterVolume;
        volumeSlider.gameObject.SetActive(false);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(BGMTypes.MainMenu, out _bgmAudioSource);
    }
    
    public void ToggleVolumeSlider()
    {
        volumeSlider.gameObject.SetActive(!volumeSlider.gameObject.activeSelf);
    }
    
    public void ChangeVolume()
    {
        SoundManager.Instance.ChangeMixerVolume(volumeSlider.value);
    }

    public void PlayGame()
    {
        if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.LevelSelect, LoadSceneMode.Single, false);
    }
    
    public void QuitGame()
    {
        if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        Application.Quit();
    }
}
