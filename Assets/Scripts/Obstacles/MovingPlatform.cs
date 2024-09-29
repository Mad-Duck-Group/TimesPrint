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
    
    private bool _isMoving;
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

    private void PlayOrPause(bool isPaused, bool beforePlay)
    {
        if (!isPaused) Initialize();
        _isMoving = !isPaused;
    }

    private void Initialize()
    {
        _startingPosition = transform.position;
        GetStartingDirection();
        _changeDirectionPosition = transform.position - (_direction * distance);
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
    }

    private void Restart()
    {
        transform.position = _startingPosition;
        GetStartingDirection();
        if (_changeDirectionCoroutine != null)
            StopCoroutine(_changeDirectionCoroutine);
        _changeDirectionCoroutine = null;
        GenerateBoxCastData();
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
            passedThreshold = Vector3.Distance(transform.position, _changeDirectionPosition) >= 0.1f &&
                              transform.position.x > _changeDirectionPosition.x;

        }
        else if (_direction == Vector3.up)
        {
            passedThreshold = Vector3.Distance(transform.position, _changeDirectionPosition) >= 0.1f &&
                              transform.position.y > _changeDirectionPosition.y;
        }
        else if (_direction == Vector3.left)
        {
            passedThreshold = Vector3.Distance(transform.position, _changeDirectionPosition) >= 0.1f &&
                              transform.position.x < _changeDirectionPosition.x;
        }
        else
        {
            passedThreshold = Vector3.Distance(transform.position, _changeDirectionPosition) >= 0.1f &&
                              transform.position.y < _changeDirectionPosition.y;
        }
        if (Vector3.Distance(_startingPosition, transform.position) < distance || !passedThreshold) return;
        _changeDirectionPosition = _startingPosition + (_direction * distance);
        transform.position = _changeDirectionPosition;
        _changeDirectionCoroutine = StartCoroutine(ChangeDirection());
    }

    private void GenerateBoxCastData()
    {
        if (_direction == Vector3.left || _direction == Vector3.right)
        {
            _offset = ((transform.localScale.x * 0.5f) + (boxCastCheckLength / 2)) * _direction;
            _boxCastSize = new Vector3(boxCastCheckLength, transform.localScale.y, 0);
        }
        else
        {
            _offset = ((transform.localScale.y * 0.5f) + (boxCastCheckLength / 2)) * _direction;
            _boxCastSize = new Vector3(transform.localScale.x, boxCastCheckLength, 0);
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
        Vector3 startingPosition = transform.position + _offset;
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
        _changeDirectionPosition = transform.position;
        if (_changeDirectionCoroutine != null) return;
        _changeDirectionCoroutine = StartCoroutine(ChangeDirection());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        if (!Application.isPlaying)
        {
            GetStartingDirection();
            GenerateBoxCastData();
            _startingPosition = transform.position;
        }
        if (fixedDistance)
        {
            float sizeOffset;
            if (_direction == Vector3.left || _direction == Vector3.right)
            {
                sizeOffset = transform.localScale.x / 2;
            }
            else
            {
                sizeOffset = transform.localScale.y / 2;
            }
            Gizmos.DrawLine(_startingPosition - (_direction * (distance + sizeOffset)), _startingPosition + (_direction * (distance + sizeOffset)));
        }
        Gizmos.DrawWireCube(transform.position + _offset, _boxCastSize);
    }
}
