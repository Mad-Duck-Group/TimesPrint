using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlaceableTypes
{
    Platform,
    MovingPlatform,
    PlayerJump,
    PlayerFlip,
    PlayerStop,
    PlatformSpeed,
    PlatformReverse
}
public abstract class Placeable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private PlaceableTypes placeableType;
    [SerializeField] private LayerMask obstacleLayer;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Collider2D _collider;
    private bool _placeByPlayer;
    public PlaceableTypes PlaceableType => placeableType;

    public virtual void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _originalColor = _spriteRenderer.color;
    }

    public virtual void Place()
    {
        _placeByPlayer = true;
        _spriteRenderer.color = _originalColor;
    }
    
    public virtual bool CanPlace()
    {
        bool canPlace = false;
        switch (_collider)
        {
            case BoxCollider2D:
                canPlace = CanPlaceBoxCollider();
                break;
            case CircleCollider2D:
                canPlace = CanPlaceCircleCollider();
                break;
            case CapsuleCollider2D:
                canPlace = CanPlaceCapsuleCollider();
                break;
        }
        canPlace = PlaceableAreaManager.Instance.WithinBound(transform) && canPlace;
        _spriteRenderer.color = canPlace ? Color.green : Color.red;
        return canPlace;
    }

    public virtual void Delete()
    {
        _placeByPlayer = false;
        ItemManager.Instance.ChangeItemAmount(placeableType);
        Destroy(gameObject);
    }
    
    protected virtual bool CanPlaceBoxCollider()
    {
        Vector2 size = _collider.bounds.size;
        Vector2 position = _collider.bounds.center;
        Collider2D[] result = new Collider2D[10];
        var collide = Physics2D.OverlapBoxNonAlloc(position, size, 0, result, obstacleLayer);
        if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
        return collide == 0;
    }
    
    protected virtual bool CanPlaceCircleCollider()
    {
        float radius = _collider.bounds.extents.x;
        Vector2 position = _collider.bounds.center;
        Collider2D[] result = new Collider2D[10];
        var collide = Physics2D.OverlapCircleNonAlloc(position, radius, result, obstacleLayer);
        if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
        return collide == 0;
    }
    
    protected virtual bool CanPlaceCapsuleCollider()
    {
        Vector2 size = _collider.bounds.size;
        Vector2 position = _collider.bounds.center;
        Collider2D[] result = new Collider2D[10];
        var collide = Physics2D.OverlapCapsuleNonAlloc(position, size, CapsuleDirection2D.Vertical, 0, result, obstacleLayer);
        if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
        return collide == 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.Instance.IsPaused) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (_placeByPlayer)
        {
            Delete();
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTrigger(other);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        OnCollision(other);
    }


    protected virtual void OnTrigger(Collider2D other)
    {
        
    }
    
    protected virtual void OnCollision(Collision2D other)
    {
        
    }
}
