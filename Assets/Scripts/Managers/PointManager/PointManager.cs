using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointManager : MonoBehaviour
{
    public Image[] stars;  // Array สำหรับเก็บ Image ของดาวแต่ละดวง
    public Sprite starEmpty;  // ภาพดาวเปล่า
    public Sprite starFull;   // ภาพดาวเต็ม
    [SerializeField] private GameObject[] starObj;
    private int starInArray = 0;
    
    private int starsCollected = 0;  // จำนวนดาวที่เก็บได้

    // ฟังก์ชันสำหรับอัพเดตภาพดาว
    public void CollectStar()
    {
        if (starsCollected < stars.Length)
        {
            stars[starsCollected].sprite = starFull;  // เปลี่ยนภาพดาวเปล่าเป็นดาวเต็ม
            starsCollected++;
        }
    }

    // ฟังก์ชันสำหรับรีเซ็ตดาว (ถ้าต้องการใช้งานในอนาคต)
    public void ResetStars()
    {
        for (int i = 0; i < stars.Length; i++)
        {
            starObj[i].SetActive(true);
            stars[i].sprite = starEmpty;  // เปลี่ยนกลับเป็นดาวเปล่า
        }
        starsCollected = 0;
    }
}
