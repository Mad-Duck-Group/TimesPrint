using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public enum MovingPlatformStartingDirection
{
    Left,
    Right,
    Up,
    Down
}
public class MovingPlatform : MonoBehaviour
{
    [SerializeField][MinValue(0)] 
    private float speed = 1.0f;

    [SerializeField][MinValue(0)] 
    private float changeDirectionInterval;

    [SerializeField]
    private bool fixedDistance;
    
    [SerializeField][ShowIf(nameof(fixedDistance))][MinValue(0)] 
    private float distance = 5.0f;
    
    [SerializeField] 
    private MovingPlatformStartingDirection startingDirection = MovingPlatformStartingDirection.Right;
    
    [SerializeField][MinValue(0)] 
    private float boxCastCheckLength = 0.1f;
    
    [SerializeField] 
    private LayerMask obstacleLayerMask;
    
    private bool _initialized;
    private bool _isMoving;
    private BoxCollider2D _boxCollider2D;
    private Vector3 _startingPosition;
    private Vector3 _direction;
    private Vector3 _offset;
    private Vector3 _boxCastSize;
    private Coroutine _changeDirectionCoroutine;
    private Vector3 _changeDirectionPosition;
    
    public float Speed { get => speed; set => speed = value; }

    private void OnEnable()
    {
        GameManager.Instance.playOrPauseDelegate += PlayOrPause;
        GameManager.Instance.restartDelegate += Restart;
    }
    
    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.playOrPauseDelegate -= PlayOrPause;
        GameManager.Instance.restartDelegate -= Restart;
    }

    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
    }

    private void PlayOrPause(bool isPaused, bool beforePlay)
    {
        if (!isPaused && !_initialized)
            Initialize();
        _isMoving = !isPaused;
    }

    private void RecheckDirection()
    {
        GetStartingDirection();
        if (_changeDirectionCoroutine != null)
            StopCoroutine(_changeDirectionCoroutine);
        _changeDirectionCoroutine = null;
        _changeDirectionPosition = _boxCollider2D.bounds.center - (_direction * distance);
        GenerateBoxCastData();
    }

    private void Initialize()
    {
        _initialized = true;
        _startingPosition = _boxCollider2D.bounds.center;
        GetStartingDirection();
        _changeDirectionPosition = _boxCollider2D.bounds.center - (_direction * distance);
    }
    
    public void ReverseDirection()
    {
        switch (startingDirection)
        {
            case MovingPlatformStartingDirection.Left:
                startingDirection = MovingPlatformStartingDirection.Right;
                break;
            case MovingPlatformStartingDirection.Right:
                startingDirection = MovingPlatformStartingDirection.Left;
                break;
            case MovingPlatformStartingDirection.Up:
                startingDirection = MovingPlatformStartingDirection.Down;
                break;
            case MovingPlatformStartingDirection.Down:
                startingDirection = MovingPlatformStartingDirection.Up;
                break;
        }
        RecheckDirection();
    }

    private void Restart()
    {
        Vector3 relativePosition = transform.position - _boxCollider2D.bounds.center;
        transform.position = _startingPosition + relativePosition;
        RecheckDirection();
    }
    
    private void GetStartingDirection()
    {
        switch (startingDirection)
        {
            case MovingPlatformStartingDirection.Left:
                _direction = Vector3.left;
                break;
            case MovingPlatformStartingDirection.Right:
                _direction = Vector3.right;
                break;
            case MovingPlatformStartingDirection.Up:
                _direction = Vector3.up;
                break;
            case MovingPlatformStartingDirection.Down:
                _direction = Vector3.down;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        BoxCastCheck();
        Move();
    }

    private void Move()
    {
        if (!_isMoving) return;
        if (_changeDirectionCoroutine != null) return;
        transform.Translate(_direction * (speed * Time.deltaTime));
        if (!fixedDistance) return;
        bool passedThreshold;
        if (_direction == Vector3.right)
        {
            passedThreshold = Vector3.Distance(_boxCollider2D.bounds.center, _changeDirectionPosition) >= 0.1f &&
                              _boxCollider2D.bounds.center.x > _changeDirectionPosition.x;

        }
        else if (_direction == Vector3.up)
        {
            passedThreshold = Vector3.Distance(_boxCollider2D.bounds.center, _changeDirectionPosition) >= 0.1f &&
                              _boxCollider2D.bounds.center.y > _changeDirectionPosition.y;
        }
        else if (_direction == Vector3.left)
        {
            passedThreshold = Vector3.Distance(_boxCollider2D.bounds.center, _changeDirectionPosition) >= 0.1f &&
                              _boxCollider2D.bounds.center.x < _changeDirectionPosition.x;
        }
        else
        {
            passedThreshold = Vector3.Distance(_boxCollider2D.bounds.center, _changeDirectionPosition) >= 0.1f &&
                              _boxCollider2D.bounds.center.y < _changeDirectionPosition.y;
        }
        if (Vector3.Distance(_startingPosition, _boxCollider2D.bounds.center) < distance || !passedThreshold) return;
        _changeDirectionPosition = _startingPosition + (_direction * distance);
        Vector3 relativePosition = transform.position - _boxCollider2D.bounds.center;
        transform.position = _changeDirectionPosition + relativePosition;
        _changeDirectionCoroutine = StartCoroutine(ChangeDirection());
    }

    private void GenerateBoxCastData()
    {
        if (_direction == Vector3.left || _direction == Vector3.right)
        {
            _offset = ((_boxCollider2D.bounds.size.x * 0.5f) + (boxCastCheckLength / 2)) * _direction;
            _boxCastSize = new Vector3(boxCastCheckLength, _boxCollider2D.bounds.size.y, 0);
        }
        else
        {
            _offset = ((_boxCollider2D.bounds.size.y * 0.5f) + (boxCastCheckLength / 2)) * _direction;
            _boxCastSize = new Vector3(_boxCollider2D.bounds.size.x, boxCastCheckLength, 0);
        }
    }
    
    private IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(changeDirectionInterval);
        _direction = -_direction;
        _changeDirectionCoroutine = null;
    }
    
    private void BoxCastCheck()
    {
        if (!_isMoving) return;
        GenerateBoxCastData();
        Vector3 startingPosition = _boxCollider2D.bounds.center + _offset;
        ContactFilter2D contactFilter2D = new ContactFilter2D
        {
            useLayerMask = true
        };
        contactFilter2D.SetLayerMask(obstacleLayerMask);
        Collider2D[] colliders = new Collider2D[10];
        int colliderCount = Physics2D.OverlapBox(startingPosition, _boxCastSize, 0, contactFilter2D, colliders);
        switch (colliderCount)
        {
            case 0:
            case 1 when colliders[0].gameObject == gameObject:
                return;
        }
        _changeDirectionPosition = _boxCollider2D.bounds.center;
        if (_changeDirectionCoroutine != null) return;
        _changeDirectionCoroutine = StartCoroutine(ChangeDirection());
    }

    private void OnDrawGizmosSelected()
    {
        if (!_boxCollider2D) _boxCollider2D = GetComponent<BoxCollider2D>();
        Gizmos.color = Color.red;
        if (!Application.isPlaying)
        {
            GetStartingDirection();
            GenerateBoxCastData();
            _startingPosition = _boxCollider2D.bounds.center;
        }
        if (fixedDistance)
        {
            Vector3[] positions = GetDrawLinePositions();
            Gizmos.DrawLine(positions[0], positions[1]);
        }
        Gizmos.DrawWireCube(_boxCollider2D.bounds.center + _offset, _boxCastSize);
    }
    
    public Vector3[] GetDrawLinePositions()
    {
        if (!_initialized)
        {
            if (!_boxCollider2D) _boxCollider2D = GetComponent<BoxCollider2D>();
            _startingPosition = _boxCollider2D.bounds.center;
            GetStartingDirection();
        }
        Vector3[] positions = new Vector3[2];
        float sizeOffset;
        if (_direction == Vector3.left || _direction == Vector3.right)
        {
            sizeOffset = _boxCollider2D.bounds.size.x / 2;
        }
        else
        {
            sizeOffset = _boxCollider2D.bounds.size.y / 2;
        }
        positions[0] = _startingPosition - (_direction * (distance + sizeOffset));
        positions[1] = _startingPosition + (_direction * (distance + sizeOffset));
        return positions;
    }
}
