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
    private int currentIndex = 0;

    private void Awake()
    {
        htpImages.sprite = htpSprites[0];
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
    
    // ฟังก์ชันเปลี่ยนเป็นรูปถัดไป
    private void NextImage()
    {
        if (htpSprites.Length == 0) return;

        currentIndex++;

        // เช็คว่าถึงรูปสุดท้ายแล้วหรือยัง
        if (currentIndex >= htpSprites.Length)
        {
            // ถ้าคลิกถัดไปหลังจากรูปสุดท้าย ให้ซ่อนรูปภาพ
            htpPanel.SetActive(false); // ซ่อนรูป
        }
        else
        {
            // ถ้ายังไม่ถึงรูปสุดท้าย ให้แสดงรูปถัดไป
            htpImages.sprite = htpSprites[currentIndex];
        }
    }

    // ฟังก์ชันเปลี่ยนเป็นรูปก่อนหน้า
    private void PreviousImage()
    {
        if (htpSprites.Length == 0) return;

        currentIndex--;

        // เช็คว่าถอยหลังจนถึงจุดเริ่มต้นแล้วหรือยัง
        if (currentIndex < 0)
        {
            currentIndex = 0;  // ป้องกันไม่ให้ถอยเกินรูปแรก
        }
        else
        {
            htpImages.sprite = htpSprites[currentIndex];  // เปลี่ยนรูปใน Image
            htpImages.enabled = true;  // ถ้ารูปถูกซ่อนไว้ให้แสดงรูปใหม่เมื่อคลิกย้อนกลับ
        }
    }
}
