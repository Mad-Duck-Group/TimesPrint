using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipBlock : MonoBehaviour
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
            if (Player.Instance.isFlipped == false)
            {
                Player.Instance.isFlipped = true;
                Debug.Log("Flip");
            }
            else if (Player.Instance.isFlipped == true)
            {
                Player.Instance.isFlipped = false;
                Debug.Log("Normal");
            }
        }
    }
}
