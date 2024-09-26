using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.EventSystems;

public enum PlaceableTypes
{
    Platform,
    MovingPlatform,
    PlayerJump,
    PlayerFlip,
    PlayerStop,
    PlatformStop,
    PlatformSpeed,
    PlatformReverse
}
public abstract class Placeable : MonoBehaviour, IPointerClickHandler
{
    [SerializeField][OnValueChanged(nameof(CheckIfObjectItem))] 
    private PlaceableTypes placeableType;
    
    [SerializeField][HideIf(nameof(_isObjectItem))] 
    private LayerMask obstacleLayer;
    
    [SerializeField][ShowIf(nameof(_isObjectItem))] 
    private LayerMask targetLayer;
    
    [ReadOnly][SerializeField][HideIf(nameof(_isObjectItem))] private Placeable objectItem;
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;
    private Collider2D _collider;
    private bool _placeByPlayer;
    private bool _isObjectItem;
    protected Placeable targetPlaceable;
    public PlaceableTypes PlaceableType => placeableType;
    public Placeable ObjectItem => objectItem;
    
    private void CheckIfObjectItem()
    {
        _isObjectItem = placeableType is PlaceableTypes.PlatformStop or PlaceableTypes.PlatformSpeed
            or PlaceableTypes.PlatformReverse;
    }
    
    private void OnEnable()
    {
        GameManager.Instance.restartDelegate += Restart;
    }
    
    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.restartDelegate -= Restart;
    }
    
    protected virtual void Restart()
    {
        if (objectItem)
            objectItem.ReactivateObjectItem(this);
    }

    public virtual void Initialize()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
        _originalColor = _spriteRenderer.color;
        CheckIfObjectItem();
    }

    public virtual void Place()
    {
        _placeByPlayer = true;
        _spriteRenderer.color = _originalColor;
        if (!_isObjectItem || !targetPlaceable) return;
        targetPlaceable.AssignObjectItem(this);
        _spriteRenderer.color = Color.clear;
    }

    public virtual void AssignObjectItem(Placeable item)
    {
        objectItem = item;
        objectItem.ActivateObjectItem(this);
    }
    
    protected virtual void ReactivateObjectItem(Placeable owner)
    {
        
    }
    
    protected virtual void ActivateObjectItem(Placeable owner)
    {
        
    }
    
    protected virtual void DeactivateObjectItem()
    {
        
    }

    protected virtual void Update()
    {
        if (!objectItem || GameManager.Instance.IsPaused) return;
        objectItem.UpdateObjectItem();
    }
    
    protected virtual void UpdateObjectItem()
    {
        
    }
    
    public virtual void RemoveObjectItem()
    {
        objectItem.DeactivateObjectItem();
        objectItem = null;
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
    
    public virtual void SetSpriteOrder(int order)
    {
        _spriteRenderer.sortingOrder = order;
    }

    public virtual void Remove()
    {
        if (!objectItem)
        {
            ItemManager.Instance.ChangeItemAmount(placeableType);
            _placeByPlayer = false;
            Destroy(gameObject);
        }
        else
        {
            ItemManager.Instance.ChangeItemAmount(objectItem.PlaceableType);
            RemoveObjectItem();
        }
    }
    
    protected virtual bool CanPlaceBoxCollider()
    {
        Vector2 size = _collider.bounds.size;
        Vector2 position = _collider.bounds.center;
        Collider2D[] result = new Collider2D[10];
        if (!_isObjectItem)
        {
            var collide = Physics2D.OverlapBoxNonAlloc(position, size, 0, result, obstacleLayer);
            if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
            return collide == 0;
        }
        else
        {
            var collide = Physics2D.OverlapBoxNonAlloc(position, size, 0, result, targetLayer);
            if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
            if (collide == 0) return false;
            targetPlaceable = result[0].GetComponent<Placeable>();
            return !targetPlaceable.objectItem;
        }
    }
    
    protected virtual bool CanPlaceCircleCollider()
    {
        float radius = _collider.bounds.extents.x;
        Vector2 position = _collider.bounds.center;
        Collider2D[] result = new Collider2D[10];
        if (!_isObjectItem)
        {
            var collide = Physics2D.OverlapCircleNonAlloc(position, radius, result, obstacleLayer);
            if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
            return collide == 0;
        }
        else
        {
            var collide = Physics2D.OverlapCircleNonAlloc(position, radius, result, targetLayer);
            if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
            if (collide == 0) return false;
            targetPlaceable = result[0].GetComponent<Placeable>();
            return !targetPlaceable.objectItem;
        }
    }
    
    protected virtual bool CanPlaceCapsuleCollider()
    {
        Vector2 size = _collider.bounds.size;
        Vector2 position = _collider.bounds.center;
        Collider2D[] result = new Collider2D[10];
        if (!_isObjectItem)
        {
            var collide = Physics2D.OverlapCapsuleNonAlloc(position, size, CapsuleDirection2D.Vertical, 0, result, obstacleLayer);
            if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
            return collide == 0;
        }
        else
        {
            var collide = Physics2D.OverlapCapsuleNonAlloc(position, size, CapsuleDirection2D.Vertical, 0, result, targetLayer);
            if (collide == 1 && result[0].gameObject == gameObject) collide = 0;
            if (collide == 0) return false;
            targetPlaceable = result[0].GetComponent<Placeable>();
            return !targetPlaceable.objectItem;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!GameManager.Instance.BeforePlay) return;
        if (eventData.button != PointerEventData.InputButton.Right) return;
        if (_placeByPlayer)
        {
            Remove();
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
