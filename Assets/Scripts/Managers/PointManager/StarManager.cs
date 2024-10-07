using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarManager : MonoBehaviour
{
    private static StarManager _instance;
    
    [SerializeField] private Image[] stars;  // Array สำหรับเก็บ Image ของดาวแต่ละดวง
    [SerializeField] private Sprite starEmpty;  // ภาพดาวเปล่า
    [SerializeField] private Sprite starFull;   // ภาพดาวเต็ม
    [SerializeField] private GameObject[] starObj;

    private int _starsCollected; // จำนวนดาวที่เก็บได้
    
    public int StarsCollected => _starsCollected;
    public Sprite StarEmpty => starEmpty;
    public Sprite StarFull => starFull;

    public static StarManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("StarManager is null");
            }
            return _instance;
        }
    }
    
    private void Awake()
    {
        _instance = this;
    }
    
    // ฟังก์ชันสำหรับอัพเดตภาพดาว
    public void CollectStar()
    {
        if (_starsCollected < stars.Length)
        {
            stars[_starsCollected].sprite = starFull;  // เปลี่ยนภาพดาวเปล่าเป็นดาวเต็ม
            _starsCollected++;
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
        _starsCollected = 0;
    }
}
