using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    int speed = 5;
    
    private Vector3 movement;
    
    private static Player _instance;
    public bool isFlipped = false;
    public bool isStop = false;
    
    public float countdownTime = 3f;

    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Game Manager is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void FixedUpdate()
    {
        if (isStop == true)
        {MovementStop();}
        
        if (isFlipped == true)
        {MovementBack();}
        else if (isFlipped == false)
        {MovementNormal();}
    }

    // เดินหน้าอย่างเดียว
    private void MovementNormal()
    {
        movement = new Vector3(1, 0, 0);
        transform.position += movement * speed * Time.deltaTime; 
    }
    
    // เดินถอยหลัง
    private void MovementBack()
    {
        movement = new Vector3(-1, 0, 0);
        transform.position += movement * speed * Time.deltaTime;
    }

    private void MovementStop()
    {
        movement = new Vector3(0, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < countdownTime)
        {
            // รอไปเรื่อยๆ จนกว่าจะครบเวลาที่กำหนด
            elapsedTime += Time.deltaTime;
        }

        if (elapsedTime >= countdownTime)
        {
            isStop = false;
        }
    }
}
