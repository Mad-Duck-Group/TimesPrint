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
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button playButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button pausePanelButton;
    [SerializeField] private GameObject pausedPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private Image[] stars;
    [SerializeField] private Slider volumeSlider;
    
    public delegate void PlayOrPauseDelegate(bool isPaused, bool beforePlay);
    public PlayOrPauseDelegate playOrPauseDelegate;
    
    public delegate void RestartDelegate();
    public RestartDelegate restartDelegate;
    
    public delegate void FinishLoadingDelegate();
    public FinishLoadingDelegate finishLoadingDelegate;

    private bool _finishLoading;
    private bool _isPaused;
    private bool _beforePlay = true;
    private List<Placeable> _placeableList = new List<Placeable>();
    public bool IsPaused => _isPaused;
    public bool FinishLoading => _finishLoading;
    public bool BeforePlay => _beforePlay;
    
    private AudioSource _bgmAudioSource;
    private BGMTypes _currentBGM = BGMTypes.GamePlay;


    private void OnEnable()
    {
        SceneManager.activeSceneChanged += OnActiveSceneChange;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.activeSceneChanged -= OnActiveSceneChange;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnActiveSceneChange(Scene previousScene, Scene scene)
    {
        if (scene.name != SceneManager.GetActiveScene().name) return;
        Initialize();
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != SceneManager.GetActiveScene().name) return;
        Initialize();
    }

    private void Initialize()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.Start, out _);
        _finishLoading = true;
        finishLoadingDelegate?.Invoke();
        clearButton.interactable = true;
        pausePanelButton.interactable = true;
        PlayOrPause();
    }

    private void Awake()
    {
        _instance = this;
        Time.timeScale = 0;
        pausedPanel.SetActive(false);
        winPanel.SetActive(false);
        playButton.interactable = false;
        restartButton.interactable = false;
        pauseButton.interactable = false;
        clearButton.interactable = false;
        pausePanelButton.interactable = false;
    }

    private void Start()
    {
        volumeSlider.value = SoundManager.Instance.MasterVolume;
        volumeSlider.gameObject.SetActive(false);
    }

    public void Play()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.TimeControlButton, out _);
        Time.timeScale = 1;
        PlayOrPause();
    }
    
    public void Pause()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.TimeControlButton, out _);
        Time.timeScale = 0;
        PlayOrPause();
    }
    
    public void Restart()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.TimeControlButton, out _);
        Player.Instance.transform.position = Player.Instance.PlayerStartPosition;
        Player.Instance.isFlipped = false;
        _beforePlay = true;
        Time.timeScale = 0;
        PlayOrPause();
        Player.Instance.ResetSpeed();
        restartDelegate?.Invoke();
        StarManager.Instance.ResetStars();
    }

    public void Win()
    {
        Time.timeScale = 0;
        winPanel.SetActive(true);
        int levelIndex = SceneManagerPersistent.Instance.GetLevelIndex(SceneManager.GetActiveScene().name);
        SaveManager.SaveStars(levelIndex, StarManager.Instance.StarsCollected);
        StartCoroutine(ShowStar());
    }

    private IEnumerator ShowStar()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            stars[i].sprite = i < StarManager.Instance.StarsCollected ? StarManager.Instance.StarFull : StarManager.Instance.StarEmpty;
            if (i >= StarManager.Instance.StarsCollected) continue;
            SoundManager.Instance.PlayCoinCheck(out _, i);
            yield return new WaitForSecondsRealtime(1f);
        }
        SoundManager.Instance.PlaySoundFX(StarManager.Instance.StarsCollected > 1 ? SoundFXTypes.Win : SoundFXTypes.Lose, out _);
    }

    public void RestartLevel()
    {
        if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    private void PlayOrPause()
    {
        if (Time.timeScale == 0)
        {
            pauseButton.interactable = false;
            playButton.interactable = true;
            restartButton.interactable = !_beforePlay;
            clearButton.interactable = _beforePlay;
            _isPaused = true;
        }
        else
        {
            pauseButton.interactable = true;
            playButton.interactable = false;
            restartButton.interactable = true;
            clearButton.interactable = false;
            _isPaused = false;
            _beforePlay = false;
        }
        switch (_beforePlay)
        {
            case true when _currentBGM == BGMTypes.GamePlay:
                if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
                SoundManager.Instance.PlayBGM(BGMTypes.GamePlan, out _bgmAudioSource);
                _currentBGM = BGMTypes.GamePlan;
                break;
            case false when _currentBGM == BGMTypes.GamePlan:
                if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
                SoundManager.Instance.PlayBGM(BGMTypes.GamePlay, out _bgmAudioSource);
                _currentBGM = BGMTypes.GamePlay;
                break;
        }
        playOrPauseDelegate?.Invoke(_isPaused, _beforePlay);
    }
    
    public void DisplayPausedPanel(bool show)
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.Popup, out _);
        pausedPanel.SetActive(show);
        if (_beforePlay || _isPaused) return;
        Time.timeScale = show ? 0 : 1;
    }
    
    public void ToMainMenu()
    {
        if (_bgmAudioSource) SoundManager.Instance.StopSound(_bgmAudioSource);
        SceneManagerPersistent.Instance.LoadNextScene(SceneTypes.MainMenu, LoadSceneMode.Single, false);
    }

    public void ToggleVolumeSlider()
    {
        volumeSlider.gameObject.SetActive(!volumeSlider.gameObject.activeSelf);
    }
    
    public void ChangeVolume()
    {
        SoundManager.Instance.ChangeMixerVolume(volumeSlider.value);
    }

    public void AddPlaceable(Placeable placeable)
    {
        _placeableList.Add(placeable);
    }
    
    public void RemovePlaceable(Placeable placeable)
    {
        _placeableList.Remove(placeable);
    }

    public void ClearPlaceable()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.ResetButton, out _);
        foreach (var placeable in _placeableList)
        {
            placeable.Remove();
        }
        _placeableList.Clear();
    }
}
