using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class HowToPlay : MonoBehaviour, IPointerClickHandler
{   
    [SerializeField] private Image htpImages;
    [SerializeField] private Sprite[] htpSprites;
    [SerializeField] private GameObject htpPanel;
    [SerializeField] private Image leftClick;
    [SerializeField] private Image rightClick;
    private int _currentIndex = 0;

    private void Awake()
    {
        htpImages.sprite = htpSprites[0];
        ToggleClickImage();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
            PreviousImage();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            NextImage();
        }
    }

    private void ToggleClickImage()
    {
        int previousIndex = _currentIndex - 1;
        int nextIndex = _currentIndex + 1;
        rightClick.enabled = nextIndex <= htpSprites.Length;
        leftClick.enabled = previousIndex >= 0;
    }
    
    // ฟังก์ชันเปลี่ยนเป็นรูปถัดไป
    private void NextImage()
    {
        if (htpSprites.Length == 0) return;
        
        _currentIndex++;
        ToggleClickImage();
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.MousePointButton, out _);

        // เช็คว่าถึงรูปสุดท้ายแล้วหรือยัง
        if (_currentIndex >= htpSprites.Length)
        {
            _currentIndex = 0;
            htpImages.sprite = htpSprites[0];
            ToggleClickImage();
            // ถ้าคลิกถัดไปหลังจากรูปสุดท้าย ให้ซ่อนรูปภาพ
            htpPanel.SetActive(false); // ซ่อนรูป
            
        }
        else
        {
            // ถ้ายังไม่ถึงรูปสุดท้าย ให้แสดงรูปถัดไป
            htpImages.sprite = htpSprites[_currentIndex];
        }
    }

    // ฟังก์ชันเปลี่ยนเป็นรูปก่อนหน้า
    private void PreviousImage()
    {
        if (htpSprites.Length == 0) return;
        
        _currentIndex--;
        ToggleClickImage();

        // เช็คว่าถอยหลังจนถึงจุดเริ่มต้นแล้วหรือยัง
        if (_currentIndex < 0)
        {
            _currentIndex = 0;  // ป้องกันไม่ให้ถอยเกินรูปแรก
        }
        else
        {
            SoundManager.Instance.PlaySoundFX(SoundFXTypes.MousePointButton, out _);
            htpImages.sprite = htpSprites[_currentIndex];  // เปลี่ยนรูปใน Image
            htpImages.enabled = true;  // ถ้ารูปถูกซ่อนไว้ให้แสดงรูปใหม่เมื่อคลิกย้อนกลับ
        }
    }
}
