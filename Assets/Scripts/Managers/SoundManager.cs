using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public enum SoundFXTypes
{
    MousePointButton,
    MouseClickButton,
    ResetButton,
    TimeControlButton,
    MousePoint,
    MouseClick,
    MouseDeleteObject,
    MouseDeleteObjectItem,
    MousePlaceObject,
    MousePlaceObjectItem,
    MousePlaceFail,
    SceneTransition,
    Start,
    HitObject,
    CoinCollect,
    Win,
    Lose,
    Popup,
    CoinCheck
}

public enum PlayerFXTypes
{
    Walk,
    Death
}

public enum BGMTypes
{
    MainMenu,
    GamePlan,
    GamePlay
}
public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public static SoundManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Sound Manager is null");
            }
            return _instance;
        }
    }

    [Header("Sound Settings")]
    [SerializeField] private int poolSize = 10;
    [SerializeField] AudioMixerGroup audioMixerGroup;
    
    [Header("General FX")]
    [SerializeField] private AudioClip mousePointButton;
    [SerializeField] private AudioClip mouseClickButton;
    [SerializeField] private AudioClip sceneTransition;

    [Header("Game FX")]
    [SerializeField] private AudioClip resetButton;
    [SerializeField] private AudioClip timeControlButton;
    [SerializeField] private AudioClip mousePoint;
    [SerializeField] private AudioClip mouseClick;
    [SerializeField] private AudioClip mouseDeleteObject;
    [SerializeField] private AudioClip mouseDeleteObjectItem;
    [SerializeField] private AudioClip mousePlaceObject;
    [SerializeField] private AudioClip mousePlaceObjectItem;
    [SerializeField] private AudioClip mousePlaceFail;
    [SerializeField] private AudioClip start;
    [SerializeField] private AudioClip hitObject;
    [SerializeField] private AudioClip coinCollect;
    [SerializeField] private AudioClip win;
    [SerializeField] private AudioClip lose;
    [SerializeField] private AudioClip popup;
    [SerializeField] private AudioClip[] coinCheck;
    
    [Header("Player FX")]
    [SerializeField] private AudioClip walk;
    [SerializeField] private AudioClip death;

    [Header("BGM")]
    [SerializeField] private AudioClip mainMenuBGM;
    [SerializeField] private AudioClip gamePlanBGM;
    [SerializeField] private AudioClip gamePlayBGM;
    
    private List<AudioSource> _audioSources = new List<AudioSource>();
    private float _masterVolume = 1f;
    private AudioSource _volumeSliderAudioSource;
    
    public float MasterVolume => _masterVolume;

    private void Awake()
    {
        List<SoundManager> soundManagers = FindObjectsOfType<SoundManager>().ToList();
        if (soundManagers.Count > 1)
        {
            foreach (SoundManager soundManager in soundManagers)
            {
                if (soundManager != _instance)
                {
                    Destroy(soundManager.gameObject);
                }
            }
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < poolSize; i++)
        {
            CreateAudioSource();
        }
    }
    
    private void CreateAudioSource()
    {
        GameObject soundGameObject = new GameObject($"AudioSource{_audioSources.Count}");
        soundGameObject.transform.SetParent(transform);
        AudioSource audioSource = soundGameObject.AddComponent<AudioSource>();
        audioSource.outputAudioMixerGroup = audioMixerGroup;
        _audioSources.Add(audioSource);
        audioSource.playOnAwake = false;
    }
    
    private bool FindFreeAudioSource(out AudioSource audioSource)
    {
        audioSource = _audioSources.Find(source => !source.isPlaying && !source.loop);
        return audioSource;
    }
    
    public void PlaySound(AudioClip clip, out AudioSource audioSource, bool loop, AudioSource preset = null)
    {
        audioSource = null;
        if (!clip) return;
        if (!FindFreeAudioSource(out AudioSource source))
        {
            Debug.LogWarning("No free audio source found, creating a new one, consider increasing the pool size");
            CreateAudioSource();
            source = _audioSources.Last();
        }
        source.loop = loop;
        source.clip = clip;
        if (preset) ApplyPreset(source, preset);
        source.Play();
        audioSource = source;
    }
    
    public void PauseSound(AudioSource audioSource)
    {
        if (!audioSource) return;
        audioSource.Pause();
    }
    
    public void ResumeSound(AudioSource audioSource)
    {
        if (!audioSource) return;
        audioSource.UnPause();
    }
    
    public void StopSound(AudioSource audioSource)
    {
        if (!audioSource) return;
        audioSource.Stop();
    }
    
    public void ResumeAllSounds()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            PauseSound(audioSource);
        }
    }
    
    public void PauseAllSounds()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            ResumeSound(audioSource);
        }
    }

    public void StopAllSounds()
    {
        foreach (AudioSource audioSource in _audioSources)
        {
            StopSound(audioSource);
        }
    }
    
    private void ApplyPreset(AudioSource audioSource, AudioSource preset)
    {
        audioSource.volume = preset.volume;
        audioSource.pitch = preset.pitch;
        audioSource.loop = preset.loop;
        audioSource.outputAudioMixerGroup = preset.outputAudioMixerGroup;
        audioSource.spatialBlend = preset.spatialBlend;
        audioSource.panStereo = preset.panStereo;
        audioSource.reverbZoneMix = preset.reverbZoneMix;
        audioSource.bypassEffects = preset.bypassEffects;
        audioSource.bypassListenerEffects = preset.bypassListenerEffects;
        audioSource.bypassReverbZones = preset.bypassReverbZones;
        audioSource.dopplerLevel = preset.dopplerLevel;
        audioSource.spread = preset.spread;
        audioSource.rolloffMode = preset.rolloffMode;
        audioSource.minDistance = preset.minDistance;
        audioSource.maxDistance = preset.maxDistance;
        audioSource.ignoreListenerVolume = preset.ignoreListenerVolume;
        audioSource.ignoreListenerPause = preset.ignoreListenerPause;
        audioSource.priority = preset.priority;
        audioSource.mute = preset.mute;
    }
    
    public void PlaySoundFX(SoundFXTypes soundFXType, out AudioSource audioSource, bool loop = false, AudioSource preset = null)
    {
        AudioClip clip = null;
        switch (soundFXType)
        {
            
        }
        PlaySound(clip, out AudioSource source, loop, preset);
        audioSource = source;
    }

    public void PlayBGM(BGMTypes bgmType, out AudioSource audioSource, bool loop = true, AudioSource preset = null)
    {
        AudioClip clip = null;
        switch (bgmType)
        {
            
        }
        PlaySound(clip, out AudioSource source, loop, preset);
        audioSource = source;
    }
    
    public void ChangeMixerVolume(float volume)
    {
        //translate the volume from 0-1 to -80-0
        _masterVolume = volume;
        volume = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20;
        audioMixerGroup.audioMixer.SetFloat("MasterVolume", volume);
        if (_volumeSliderAudioSource && _volumeSliderAudioSource.isPlaying) return;
        //PlaySoundFX(SoundFXTypes.VolumeSlider, out _volumeSliderAudioSource);
    }
}
