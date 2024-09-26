using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (!_instance)
            {
                Debug.LogError("GameManager is null");
            }
            return _instance;
        }
    }
    
    
    [Header("UI Buttons")]
    [FormerlySerializedAs("pauseButtonImage")] 
    [SerializeField] private Button pauseButton;
    
    [FormerlySerializedAs("playButtonImage")] 
    [SerializeField] private Button playButton;
    
    [SerializeField] private Button restartButton;
    
    [SerializeField] private GameObject winPanel;
    
    public delegate void PlayOrPauseDelegate(bool isPaused, bool beforePlay);
    public PlayOrPauseDelegate playOrPauseDelegate;
    
    public delegate void RestartDelegate();
    public RestartDelegate restartDelegate;
    
    
    private bool _isPaused;
    private bool _beforePlay = true;
    public bool IsPaused => _isPaused;
    public bool BeforePlay => _beforePlay;
    

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Time.timeScale = 0;
        winPanel.SetActive(false);
        PlayOrPause();
    }
    
    public void Play()
    {
        Time.timeScale = 1;
        PlayOrPause();
    }
    
    public void Pause()
    {
        Time.timeScale = 0;
        PlayOrPause();
    }
    
    public void Restart()
    {
        Player.Instance.transform.position = Player.Instance.PlayerStartPosition;
        Player.Instance.isFlipped = false;
        _beforePlay = true;
        Pause(); 
        restartDelegate?.Invoke();
    }

    public void Win()
    {
        Time.timeScale = 0;
        winPanel.SetActive(true);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void PlayOrPause()
    {
        if (Time.timeScale == 0)
        {
            pauseButton.interactable = false;
            playButton.interactable = true;
            restartButton.interactable = !_beforePlay;
            _isPaused = true;
            
        }
        else
        {
            pauseButton.interactable = true;
            playButton.interactable = false;
            restartButton.interactable = true;
            _isPaused = false;
            _beforePlay = false;
        }
        playOrPauseDelegate?.Invoke(_isPaused, _beforePlay);
    }
}
