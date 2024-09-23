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
            if (_instance == null)
            {
                Debug.LogError("GameManager is null");
            }
            return _instance;
        }
    }
    
    [Header("UI Buttons")]
    [FormerlySerializedAs("pauseButton")] 
    [SerializeField] private Image pauseButtonImage;
    
    [FormerlySerializedAs("playButton")] 
    [SerializeField] private Image playButtonImage;
    
    [SerializeField] private GameObject winPanel;
    
    private bool _isPaused;

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Time.timeScale = 0;
        
        pauseButtonImage = pauseButtonImage.GetComponent<Image>();
        playButtonImage = playButtonImage.GetComponent<Image>();
        
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
        Pause();
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
            pauseButtonImage.color = Color.gray;
            playButtonImage.color = Color.white;
            _isPaused = true;
        }
        else
        {
            pauseButtonImage.color = Color.white;
            playButtonImage.color = Color.gray;
            _isPaused = false;
        }
    }
}
