using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopBlock : MonoBehaviour
{
    [SerializeField] private GameObject player;
    
    void Start()
    {
        player = GameObject.Find("Player");
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject == player)
        {
            if (Player.Instance.isStop == false)
            {
                Player.Instance.isStop = true;
                Debug.Log("Stop");
            }
        }
    }
}
