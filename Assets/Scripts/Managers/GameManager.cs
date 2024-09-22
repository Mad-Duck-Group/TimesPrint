using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    
    [SerializeField] private Image pauseButton; // Change this to pauseButtonImage
    [SerializeField] private Image playButton;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject winPanel;
    
    private bool isPaused = false; //remove false because it is not needed

    private void Awake()
    {
        _instance = this;
    }

    void Start()
    {
        Time.timeScale = 0;
        pauseButton = pauseButton.GetComponent<Image>();
        playButton = playButton.GetComponent<Image>();
        winPanel.SetActive(false);
    }
    
    // Remove this and put PlayOrPause in Start, Play and Pause instead
    void Update()
    {
        PlayOrPause();
    }
    
    public void Play()
    {
        Time.timeScale = 1;
    }
    
    public void Pause()
    {
        Time.timeScale = 0;
    }
    
    public void Restart()
    {
        Player.Instance.transform.position = new Vector3(-5.4f, -1.26f, 0);
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
    
    public void PlayOrPause()
    {
        if (Time.timeScale == 0)
        {
            pauseButton.color = Color.gray;
            playButton.color = Color.white;
            isPaused = true;
        }
        else
        {
            pauseButton.color = Color.white;
            playButton.color = Color.gray;
            isPaused = false;
        }
    }
}
