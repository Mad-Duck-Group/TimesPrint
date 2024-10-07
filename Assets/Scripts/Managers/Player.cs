using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Unity.VisualScripting;
using UnityEngine;

// สถานะของตัวละคร
public enum UnitState
{
    Idle,
    Move,
    Jump
}

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
    public PointManager pointManager;

    [Header("Animation")]
    private Animator anim;

    // เพิ่มตัวแปรสำหรับจัดการสถานะ
    public UnitState state;

    private static readonly int Player_Idle = Animator.StringToHash("Player_Idle");
    private static readonly int Player_Walk = Animator.StringToHash("Player_Walk");
    private static readonly int Player_Jump = Animator.StringToHash("Player_Jump");

    public static Player Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("Player is null");
            }
            return _instance;
        }
    }

    private void Awake()
    {
        anim = GetComponent<Animator>();

        _instance = this;
        _playerRb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        _playerStartPosition = transform.position;
        state = UnitState.Idle;  // กำหนดสถานะเริ่มต้นเป็น Idle
    }

    void Update()
    {
        // อัปเดตอนิเมชันตามการเคลื่อนไหว
        ChooseAnimation();

        if (isStop)
        {
            MovementStop();
        }
        else
        {
            Movement();
        }
    }

    // การเคลื่อนไหว
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

        // เปลี่ยนสถานะเป็น Move เมื่อเคลื่อนไหว
        state = UnitState.Move;
    }

    public void Jump(float jumpForce)
    {
        _playerRb.velocity = new Vector2(0, jumpForce);

        // เปลี่ยนสถานะเป็น Jump เมื่อกระโดด
        state = UnitState.Jump;
    }

    private void MovementStop()
    {
        _movement = Vector3.zero;
        speed = 0;

        _countdownTimer += Time.deltaTime;
        if (_countdownTimer >= countdownTime)
        {
            speed = 5;
            _countdownTimer = 0;
            isStop = false;
        }

        // เปลี่ยนสถานะเป็น Idle เมื่อหยุดเคลื่อนไหว
        state = UnitState.Idle;
    }

    // ฟังก์ชันสำหรับเลือกอนิเมชันตามสถานะของตัวละคร
    private void ChooseAnimation()
    {
        anim.SetBool(Player_Idle, false);
        anim.SetBool(Player_Walk, false);
        anim.SetBool(Player_Jump, false);

        // ตรวจสอบสถานะของตัวละครและเล่นอนิเมชันที่เหมาะสม
        if (state == UnitState.Idle)
        {
            anim.SetBool(Player_Idle, true);
        }
        else if (state == UnitState.Move)
        {
            anim.SetBool(Player_Walk, true);
        }
        else if (state == UnitState.Jump)
        {
            anim.SetBool(Player_Jump, true);
        }
    }

    // Collect star สำหรับเพิ่มคะแนน
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Star"))
        {
            pointManager.CollectStar();  // เรียกฟังก์ชัน CollectStar เมื่อชนกับไอเท็มดาว
            other.gameObject.SetActive(false);
        }
    }
}
