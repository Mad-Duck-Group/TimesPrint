using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonMouseHandler : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private bool playPointSound = true;
    [SerializeField] private bool playClickSound = true;
    
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!playPointSound || !_button.interactable) return;
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.MousePointButton, out _);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!playClickSound || !_button.interactable) return;
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.MouseClickButton, out _);
    }
}

