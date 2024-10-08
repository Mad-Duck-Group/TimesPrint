using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private int amount = 2;
    [SerializeField] private Placeable itemPrefab;
    [SerializeField] private TMP_Text amountText;
    private Image _itemUIImage;
    private Tween _mouseHoverTween;
    private Placeable _placeableInstance;
    private Vector3 _startPlaceablePosition;
    private bool _canPlace;
    private bool _interactable;
    private bool _dragging;
    
    public PlaceableTypes PlaceableType => itemPrefab.PlaceableType;
    private Vector3 MousePositionInWorld
    {
        get
        {
            Vector3 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            position.z = 0;
            return position;
        }
    }

    private void OnEnable()
    {
        GameManager.Instance.playOrPauseDelegate += PlayOrPause;
        GameManager.Instance.finishLoadingDelegate += Initialize;
    }
    
    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.playOrPauseDelegate -= PlayOrPause;
        GameManager.Instance.finishLoadingDelegate -= Initialize;
    }

    private void Awake()
    {
        _itemUIImage = GetComponent<Image>();
        amountText.text = $"x{amount}";
        DeactivateItem();
    }

    void Start()
    {
        
    }

    private void Initialize()
    {
        InstantiatePlaceable();
    }
    
    protected virtual void InstantiatePlaceable()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(transform.position);
        position.z = 0;
        _startPlaceablePosition = position;
        _placeableInstance = Instantiate(itemPrefab, position, Quaternion.identity);
        _placeableInstance.Initialize();
        _placeableInstance.gameObject.SetActive(false);
    }

    private void PlayOrPause(bool isPaused, bool beforePlay)
    {
        if (beforePlay)
        {
            ActivateItem();
        }
        else
        {
            DeactivateItem();
        }
    }
    
    private void ActivateItem()
    {
        if (amount <= 0) return;
        _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
        _itemUIImage.raycastTarget = true;
        _interactable = true;
    }
    
    private void DeactivateItem()
    {
        _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 0.5f);
        _itemUIImage.raycastTarget = false;
        _interactable = false;
    }

    public virtual void ChangeAmount(int value)
    {
        if (amount == 0 && value > 0)
            InstantiatePlaceable();
        amount += value;
        amountText.text = $"x{amount}";
        if (amount > 0)
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
            _itemUIImage.raycastTarget = true;
            _interactable = true;
        }
        else
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 0.5f);
            _itemUIImage.raycastTarget = false;
            _interactable = false;
        }
        
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (!_interactable) return;
        if (_dragging) return;
        if (_mouseHoverTween.IsActive()) _mouseHoverTween.Kill();
       _mouseHoverTween = transform.DOScale(1.2f, 0.2f).SetUpdate(true);
       SoundManager.Instance.PlaySoundFX(SoundFXTypes.MousePoint, out _);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        if (!_interactable) return;
        if (_mouseHoverTween.IsActive()) _mouseHoverTween.Kill();
        _mouseHoverTween = transform.DOScale(1f, 0.2f).SetUpdate(true);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (!_interactable) return;
        _dragging = true;
        _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 0f);
        _placeableInstance.gameObject.SetActive(true);
        _placeableInstance.SetSpriteOrder(2);
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.MouseClick, out _);
    }
    
    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!_interactable) return;
        _placeableInstance.transform.position = MousePositionInWorld;
        _canPlace = _placeableInstance.CanPlace();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!_interactable) return;
        if (_canPlace)
        {
            _placeableInstance.SetSpriteOrder(1);
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
            _placeableInstance.Place();
            _placeableInstance.transform.position = MousePositionInWorld;
            _canPlace = false;
            ChangeAmount(-1);
            if (amount > 0)
                InstantiatePlaceable();
        }
        else
        {
            SoundManager.Instance.PlaySoundFX(SoundFXTypes.MousePlaceFail, out _);
            _placeableInstance.SetSpriteOrder(1);
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
            _placeableInstance.gameObject.SetActive(false);
            _placeableInstance.gameObject.transform.position = _startPlaceablePosition;
            _canPlace = false;
        }
        _dragging = false;
    }
}
