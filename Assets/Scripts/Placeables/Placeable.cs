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
    PlatformReverse,
    PlatformSlow
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
    protected SpriteRenderer spriteRenderer;
    private Color _originalColor;
    protected Collider2D placeableCollider;
    private bool _placeByPlayer;
    private bool _isObjectItem;
    protected Placeable targetPlaceable;
    public PlaceableTypes PlaceableType => placeableType;
    public Placeable ObjectItem => objectItem;
    
    private void CheckIfObjectItem()
    {
        _isObjectItem = placeableType is PlaceableTypes.PlatformStop or PlaceableTypes.PlatformSpeed
            or PlaceableTypes.PlatformReverse or PlaceableTypes.PlatformSlow;
    }
    
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
    
    protected virtual void PlayOrPause(bool isPaused, bool beforePlay)
    {
        
    }
    
    protected virtual void Restart()
    {
        if (objectItem)
            objectItem.ReactivateObjectItem(this);
    }

    public virtual void Initialize()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        placeableCollider = GetComponent<Collider2D>();
        _originalColor = spriteRenderer.color;
        CheckIfObjectItem();
    }

    public virtual void Place()
    {
        SoundManager.Instance.PlaySoundFX(_isObjectItem ? SoundFXTypes.MousePlaceObjectItem : SoundFXTypes.MousePlaceObject, out _);
        _placeByPlayer = true;
        spriteRenderer.color = _originalColor;
        if (!_isObjectItem) GameManager.Instance.AddPlaceable(this);
        if (!_isObjectItem || !targetPlaceable) return;
        GameManager.Instance.AddPlaceable(targetPlaceable);
        targetPlaceable.AssignObjectItem(this);
        spriteRenderer.color = Color.clear;
        placeableCollider.enabled = false;
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

    public virtual bool CanPlace()
    {
        bool canPlace = false;
        switch (placeableCollider)
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
        spriteRenderer.color = canPlace ? Color.green : Color.red;
        return canPlace;
    }
    
    public virtual void SetSpriteOrder(int order)
    {
        spriteRenderer.sortingOrder = order;
    }

    public virtual void Remove()
    {
        if (_isObjectItem) return;
        if (!objectItem)
        {
            RemoveObject();
        }
        else
        {
            RemoveObjectItem();
        }
    }

    protected virtual void RemoveFromClick()
    {
        GameManager.Instance.RemovePlaceable(this);
        Remove();
    }

    public virtual void RemoveObject()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.MouseDeleteObject, out _);
        ItemManager.Instance.ChangeItemAmount(placeableType);
        _placeByPlayer = false;
        Destroy(gameObject);
    }
    
    public virtual void RemoveObjectItem()
    {
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.MouseDeleteObjectItem, out _);
        ItemManager.Instance.ChangeItemAmount(objectItem.PlaceableType);
        objectItem.DeactivateObjectItem();
        Destroy(objectItem.gameObject);
        objectItem = null;
    }

    protected virtual bool CanPlaceBoxCollider()
    {
        Vector2 size = placeableCollider.bounds.size;
        Vector2 position = placeableCollider.bounds.center;
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
        float radius = placeableCollider.bounds.extents.x;
        Vector2 position = placeableCollider.bounds.center;
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
        Vector2 size = placeableCollider.bounds.size;
        Vector2 position = placeableCollider.bounds.center;
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
        if (_placeByPlayer || (!_placeByPlayer && objectItem))
        {
            RemoveFromClick();
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
