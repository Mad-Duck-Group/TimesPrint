using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ButtonInGame : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    [SerializeField] private Image pauseButton;
    [SerializeField] private Image playButton;
    [SerializeField] private GameObject restartButton;
    
    private bool isPaused = false;
    void Start()
    {
        Time.timeScale = 0;
        pauseButton = pauseButton.GetComponent<Image>();
        playButton = playButton.GetComponent<Image>();
    }
    
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
       player.transform.position = new Vector3(-5.4f, -1.26f, 0);
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