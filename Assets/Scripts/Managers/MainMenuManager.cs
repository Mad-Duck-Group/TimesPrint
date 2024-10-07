using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    
    private AudioSource _bgmAudioSource;
    
    private void Awake()
    {
        _instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.Instance.PlayBGM(BGMTypes.MainMenu, out _bgmAudioSource);
    }

    public void PlayGame()
    {
        if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.Level, LoadSceneMode.Additive, true);
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
