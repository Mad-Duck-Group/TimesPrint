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
    private Placeable _itemInstance;
    private Vector3 _startPlaceablePosition;
    private bool _canPlace;
    
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
    }
    
    private void OnDisable()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.playOrPauseDelegate -= PlayOrPause;
    }

    private void Awake()
    {
        _itemUIImage = GetComponent<Image>();
    }

    void Start()
    {
        amountText.text = $"x{amount}";
        InstantiatePlaceable();
    }
    
    protected virtual void InstantiatePlaceable()
    {
        Vector3 position = Camera.main.ScreenToWorldPoint(transform.position);
        position.z = 0;
        _startPlaceablePosition = position;
        _itemInstance = Instantiate(itemPrefab, position, Quaternion.identity);
        _itemInstance.Initialize();
        _itemInstance.gameObject.SetActive(false);
    }

    private void PlayOrPause(bool isPaused)
    {
        if (isPaused)
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
            _itemUIImage.raycastTarget = true;
        }
        else
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 0.5f);
            _itemUIImage.raycastTarget = false;
        }
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
        }
        else
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 0.5f);
            _itemUIImage.raycastTarget = false;
        }
        
    }

    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        //if (!GameManager.Instance.IsPaused) return;
        if (_mouseHoverTween.IsActive()) _mouseHoverTween.Kill();
       _mouseHoverTween = transform.DOScale(1.2f, 0.2f).SetUpdate(true);
    }

    public virtual void OnPointerExit(PointerEventData eventData)
    {
        //if (!GameManager.Instance.IsPaused) return;
        if (_mouseHoverTween.IsActive()) _mouseHoverTween.Kill();
        _mouseHoverTween = transform.DOScale(1f, 0.2f).SetUpdate(true);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        //if (!GameManager.Instance.IsPaused) return;
        _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 0f);
        _itemInstance.gameObject.SetActive(true);
    }
    
    public virtual void OnDrag(PointerEventData eventData)
    {
        //if (!GameManager.Instance.IsPaused) return;
        _itemInstance.transform.position = MousePositionInWorld;
        _canPlace = _itemInstance.CanPlace();
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
       // if (!GameManager.Instance.IsPaused) return;
        if (_canPlace)
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
            _itemInstance.Place();
            _itemInstance.transform.position = MousePositionInWorld;
            _canPlace = false;
            ChangeAmount(-1);
            if (amount > 0)
                InstantiatePlaceable();
        }
        else
        {
            _itemUIImage.color = new Color(_itemUIImage.color.r, _itemUIImage.color.g, _itemUIImage.color.b, 1f);
            _itemInstance.gameObject.SetActive(false);
            _itemInstance.gameObject.transform.position = _startPlaceablePosition;
            _canPlace = false;
        }
    }
}
