using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] int speed = 5;
    
    private Vector3 _movement;
    
    private static Player _instance;
    [ReadOnly] public bool isFlipped;
    [ReadOnly] public bool isStop;
    
    [SerializeField] private float countdownTime = 3f;
    private float _countdownTimer;
    
    private Vector2 _playerStartPosition;
    public Vector2 PlayerStartPosition => _playerStartPosition;
    
    private Rigidbody2D _playerRb;
    
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
        
        _playerRb = GetComponent<Rigidbody2D>();
    }
    
    private void Start()
    {
        _playerStartPosition = transform.position;
    }
    
    void Update()
    {
        if (isStop)
        { MovementStop(); }
        else
        { Movement(); }
    }

    
    // Change to rigidbody movement**
    private void Movement()
    {
        if (!isFlipped)
        {
            _movement = Vector3.right;
            transform.position += _movement * (speed * Time.deltaTime); 
        }
        else
        {
            _movement = Vector3.left;
            transform.position += _movement * (speed * Time.deltaTime);
        }
    }
    
    public void Jump(float jumpForce)
    {
        _playerRb.velocity = new Vector2(0, jumpForce);
    }
    
    private void MovementStop()
    {
        _movement = new Vector3(0, 0, 0);
        speed = 0;
        
        _countdownTimer += Time.deltaTime;
        if (_countdownTimer >= countdownTime)
        {
            speed = 5;
            _countdownTimer = 0;
            isStop = false;
        }
    }
}
