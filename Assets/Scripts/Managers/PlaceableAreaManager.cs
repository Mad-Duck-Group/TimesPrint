using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class PlaceableAreaManager : MonoBehaviour
{
    private static PlaceableAreaManager _instance;

    public static PlaceableAreaManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("PlaceableAreaManager is null");
            }

            return _instance;
        }
    }

    [SerializeField] private bool adjustByBounds = true;

    [SerializeField] [ShowIf(nameof(adjustByBounds))]
    private Transform leftBound;

    [SerializeField] [ShowIf(nameof(adjustByBounds))]
    private Transform rightBound;

    [SerializeField] [ShowIf(nameof(adjustByBounds))]
    private Transform topBound;

    [SerializeField] [ShowIf(nameof(adjustByBounds))]
    private Transform bottomBound;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        AdjustSize();
    }

    [Button("Adjust Size")]
    private void AdjustSize()
    {
        if (!adjustByBounds) return;
        float leftBoundEdge = leftBound.position.x + leftBound.localScale.x / 2;
        float rightBoundEdge = rightBound.position.x - rightBound.localScale.x / 2;
        float width = rightBoundEdge - leftBoundEdge;
        float topBoundEdge = topBound.position.y - topBound.localScale.y / 2;
        float bottomBoundEdge = bottomBound.position.y + bottomBound.localScale.y / 2;
        float height = topBoundEdge - bottomBoundEdge;
        transform.localScale = new Vector3(width, height, 1);
        transform.position = new Vector3(leftBoundEdge + width / 2, bottomBoundEdge + height / 2, 0);
    }

    public bool WithinBound(Transform placeableTransform)
    {
        var placeablePosition = placeableTransform.position;
        var placeableScale = placeableTransform.localScale;
        Vector3 placeableTop = placeablePosition + new Vector3(0, placeableScale.y / 2, 0);
        Vector3 placeableBottom = placeablePosition - new Vector3(0, placeableScale.y / 2, 0);
        Vector3 placeableLeft = placeablePosition - new Vector3(placeableScale.x / 2, 0, 0);
        Vector3 placeableRight = placeablePosition + new Vector3(placeableScale.x / 2, 0, 0);
        var areaScale = transform.localScale;
        var areaPosition = transform.position;
        Vector3 top = areaPosition + new Vector3(0, areaScale.y / 2, 0);
        Vector3 bottom = areaPosition - new Vector3(0, areaScale.y / 2, 0);
        Vector3 left = areaPosition - new Vector3(areaScale.x / 2, 0, 0);
        Vector3 right = areaPosition + new Vector3(areaScale.x / 2, 0, 0);
        return placeableTop.y < top.y && placeableBottom.y > bottom.y &&
               placeableLeft.x > left.x && placeableRight.x < right.x;
    }
}