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
    [SerializeField] LayerMask groundLayer; // เพิ่มการตรวจสอบพื้น
    [SerializeField] private float boxCastDistance = 0.1f;
    private Vector3 _movement;

    private static Player _instance;
    [ReadOnly] public bool isFlipped;
    [ReadOnly] public bool isStop;

    [SerializeField] private float countdownTime = 3f;
    private float _countdownTimer;

    private Vector2 _playerStartPosition;
    public Vector2 PlayerStartPosition => _playerStartPosition;

    private Rigidbody2D _playerRb;
    private BoxCollider2D _boxCollider;

    [Header("Animation")]
    private Animator _anim;

    // เพิ่มตัวแปรสำหรับจัดการสถานะ
    public UnitState state;

    private static readonly int Player_Idle = Animator.StringToHash("Player_Idle");
    private static readonly int Player_Walk = Animator.StringToHash("Player_Walk");
    private static readonly int Player_JumpUp = Animator.StringToHash("Player_JumpUp");
    private static readonly int Player_JumpDown = Animator.StringToHash("Player_JumpDown");


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
        _anim = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
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
        Debug.Log(_playerRb.velocity.y);
        // อัปเดตอนิเมชันตามการเคลื่อนไหว

        if (isStop)
        {
            MovementStop();
        }
        else
        {
            Movement();
        }
        
        if (!IsGrounded())
        {
            if (_playerRb.velocity.y < -0.1)
            {
                _anim.SetBool(Player_JumpDown, true);  // ถ้าไม่สัมผัสพื้นและตกลง ให้เล่น JumpDown
            }
            state = UnitState.Jump;  // เปลี่ยนสถานะเป็น Jump เมื่อกำลังตก
        }
        
        ChooseAnimation();
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
    
    private bool IsGrounded()
    {
        Vector2 boxCastPosition = new Vector3(_boxCollider.bounds.center.x,
            _boxCollider.bounds.center.y - _boxCollider.size.y * transform.localScale.y / 2);
        Vector2 size = new Vector2(_boxCollider.size.x, boxCastDistance) * transform.localScale;
        // ทำการ BoxCast ใต้ตัวละครเพื่อตรวจสอบการสัมผัสพื้น
        bool onGround = Physics2D.BoxCast(boxCastPosition, size, 0, Vector2.down, 0.1f, groundLayer);
        return onGround;  // ถ้ามีการชนกับพื้น ให้คืนค่า true
    }

    // ฟังก์ชันสำหรับเลือกอนิเมชันตามสถานะของตัวละคร
    private void ChooseAnimation()
    {
        _anim.SetBool(Player_Idle, false);
        _anim.SetBool(Player_Walk, false);

        // ตรวจสอบสถานะของตัวละครและเล่นอนิเมชันที่เหมาะสม
        if (state == UnitState.Idle)
        {
            _anim.SetBool(Player_Idle, true);
            _anim.SetBool(Player_JumpDown, false);
            _anim.SetBool(Player_JumpUp, false);
        }
        else if (state == UnitState.Move)
        {
            _anim.SetBool(Player_Walk, true);
            _anim.SetBool(Player_JumpDown, false);
            _anim.SetBool(Player_JumpUp, false);
        }
        else if (state == UnitState.Jump)
        {
            // ตรวจสอบความเร็วแกน Y เพื่อเลือกอนิเมชันกระโดดขึ้นหรือลง
            if (_playerRb.velocity.y > 0.1 && !IsGrounded())
            {
                _anim.SetBool(Player_JumpUp, true); // กำลังขึ้น
                _anim.SetBool(Player_JumpDown, false);
            }
            else if (_playerRb.velocity.y < -0.1 && !IsGrounded())
            {
                _anim.SetBool(Player_JumpUp, false);
                _anim.SetBool(Player_JumpDown, true);  // กำลังลง
            }

            // เมื่อถึงพื้นให้กลับไปเป็นสถานะ Move หรือ Idle
            if (IsGrounded())
            {
                state = UnitState.Move;  // เมื่อแตะพื้น กลับไปอนิเมชันเดิน
            }
            else
            {
                // หากไม่แตะพื้น และกำลังเคลื่อนที่ลงให้เล่นอนิเมชัน JumpDown จนกว่าจะถึงพื้น
                if (_playerRb.velocity.y < 0)
                {
                    _anim.SetBool(Player_JumpDown, true);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!_boxCollider) _boxCollider = GetComponent<BoxCollider2D>();
        Vector2 boxCastPosition = new Vector3(_boxCollider.bounds.center.x,
            _boxCollider.bounds.center.y - _boxCollider.size.y * transform.localScale.y / 2);
        Vector2 size = new Vector2(_boxCollider.size.x, boxCastDistance) * transform.localScale;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(boxCastPosition, size);
    }
}
