using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    private static ItemManager _instance;
    public static ItemManager Instance
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogError("ItemManager is null");
            }
            return _instance;
        }
    }
    
    [SerializeField] private List<Item> items = new List<Item>();

    private void Awake()
    {
        _instance = this;
    }

    public void ChangeItemAmount(PlaceableTypes placeableType, int amount = 1)
    {
        Item item = items.Find(x => x.PlaceableType == placeableType);
        if (item) item.ChangeAmount(amount);
    }
}
