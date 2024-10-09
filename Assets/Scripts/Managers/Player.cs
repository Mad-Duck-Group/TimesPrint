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
    [SerializeField] float speed = 5;
    [SerializeField] LayerMask groundLayer; // เพิ่มการตรวจสอบพื้น
    [SerializeField] private float boxCastDistance = 0.1f;
    private Vector3 _movement;

    private static Player _instance;
    [ReadOnly] public bool isWalking;
    [ReadOnly] public bool isFlipped;
    [ReadOnly] public bool isStop;
    [ReadOnly] public bool isDead;

    [SerializeField] private float countdownTime = 3f;
    private float _countdownTimer;
    private float _originalSpeed;

    private Vector2 _playerStartPosition;
    public Vector2 PlayerStartPosition => _playerStartPosition;

    private Rigidbody2D _playerRb;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _spriteRenderer;
    private Coroutine _deadCoroutine;

    [Header("Animation")]
    private Animator _anim;

    // เพิ่มตัวแปรสำหรับจัดการสถานะ
    public UnitState state;

    private static readonly int Player_Idle = Animator.StringToHash("Player_Idle");
    private static readonly int Player_Walk = Animator.StringToHash("Player_Walk");
    private static readonly int Player_JumpUp = Animator.StringToHash("Player_JumpUp");
    private static readonly int Player_JumpDown = Animator.StringToHash("Player_JumpDown");
    private static readonly int Player_Dead = Animator.StringToHash("Player_Dead");
    private static readonly int Player_Dead_Wall = Animator.StringToHash("Player_Dead_Wall");


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

    private void OnEnable()
    {
        GameManager.Instance.playOrPauseDelegate += PlayOrPause;
        GameManager.Instance.restartDelegate += Restart;
        GameManager.Instance.winDelegate += Win;
    }
    
    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.playOrPauseDelegate -= PlayOrPause;
        GameManager.Instance.restartDelegate -= Restart;
        GameManager.Instance.winDelegate -= Win;
    }
    
    private void Win()
    {
        isWalking = false;
    }

    private void PlayOrPause(bool ispaused, bool beforeplay)
    {
        isWalking = !ispaused;
    }
    
    private void Restart()
    {
        if (_deadCoroutine != null) StopCoroutine(_deadCoroutine);
        _spriteRenderer.flipX = false;
        isDead = false;
        _countdownTimer = 0;
        _deadCoroutine = null;
    }

    private void Awake()
    {
        _anim = GetComponent<Animator>();
        _boxCollider = GetComponent<BoxCollider2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _instance = this;
        _playerRb = GetComponent<Rigidbody2D>();
        _originalSpeed = speed;
    }

    private void Start()
    {
        _playerStartPosition = transform.position;
        state = UnitState.Idle;  // กำหนดสถานะเริ่มต้นเป็น Idle
    }

    void Update()
    {
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
        if (!isWalking && !isDead)
        {
            state = UnitState.Idle;
            return;
        }
        if (isDead) return;
        if (!isFlipped)
        {
            _movement = Vector3.right;
            transform.position += _movement * (speed * Time.deltaTime);
            _spriteRenderer.flipX = false;
        }
        else
        {
            _movement = Vector3.left;
            transform.position += _movement * (speed * Time.deltaTime);
            _spriteRenderer.flipX = true;
        }
        
        // เปลี่ยนสถานะเป็น Move เมื่อเคลื่อนไหว
        if (isWalking)
            state = UnitState.Move;
    }

    public void Jump(float jumpForce)
    {
        _playerRb.velocity = new Vector2(0, jumpForce);

        // เปลี่ยนสถานะเป็น Jump เมื่อกระโดด
        state = UnitState.Jump;
    }

    public void Dead(bool wall)
    {
        if (_deadCoroutine != null) return;
        _anim.SetTrigger(wall ? Player_Dead_Wall : Player_Dead);
        isDead = true;
        _countdownTimer = 0;
        state = UnitState.Move;
        SoundManager.Instance.PlayPlayerFX(PlayerFXTypes.Death, out _);
        isWalking = false;
        _deadCoroutine = StartCoroutine(DeadCoroutine());
    }
    
    private IEnumerator DeadCoroutine()
    {
        yield return null;
        while (_anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.Restart();
        ResetSpeed();
        _spriteRenderer.flipX = false;
        isDead = false;
        _deadCoroutine = null;
    }
    
    public void ResetSpeed(bool walk = false)
    {
        isWalking = walk;
        state = UnitState.Idle;
        speed = _originalSpeed;
        isStop = false;
    }

    private void MovementStop()
    {
        _movement = Vector3.zero;
        speed = 0;

        _countdownTimer += Time.deltaTime;
        if (_countdownTimer >= countdownTime)
        {
            ResetSpeed(true);
            _countdownTimer = 0;
        }
        
        // เปลี่ยนสถานะเป็น Idle เมื่อหยุดเคลื่อนที่
        if (!isDead) state = UnitState.Idle;
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
            _anim.SetBool(Player_Walk, false);
            _anim.SetBool(Player_JumpDown, false);
            _anim.SetBool(Player_JumpUp, false);
        }
        else if (state == UnitState.Move)
        {
            _anim.SetBool(Player_Idle, false);
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
            if (IsGrounded() && !isDead)
            {
                state = isWalking ? UnitState.Move : UnitState.Idle; // เมื่อแตะพื้น กลับไปอนิเมชันเดิน
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
