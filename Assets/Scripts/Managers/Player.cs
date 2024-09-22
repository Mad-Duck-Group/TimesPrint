using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    int speed = 5; //make this serialized field
    
    private Vector3 movement; //add underscore to this variable, use the word _direction instead of movement
    
    private static Player _instance;
    public bool isFlipped = false; // remove false because it is already false by default, also use [ReadOnly] attribute
    public bool isStop = false; // remove false because it is already false by default, also use [ReadOnly] attribute
    
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
        if (isStop == true) //no need to compare to true
        {MovementStop();} //Remove brackets
        
        if (isFlipped == true)
        {MovementBack();}
        else if (isFlipped == false) //no need to compare to false use ! instead
        {MovementNormal();}
    }

    
    // Combine MovementNormal and MovementBack into one method
    // Change to rigidbody movement
    // เดินหน้าอย่างเดียว
    private void MovementNormal()
    {
        movement = new Vector3(1, 0, 0); //use Vector3.right
        transform.position += movement * (speed * Time.deltaTime); 
    }
    
    // เดินถอยหลัง
    private void MovementBack()
    {
        movement = new Vector3(-1, 0, 0); //use Vector3.left
        transform.position += movement * (speed * Time.deltaTime);
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
