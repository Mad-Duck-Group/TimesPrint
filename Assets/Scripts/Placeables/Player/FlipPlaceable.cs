using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPlaceable : Placeable
{
    protected override void OnTrigger(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            if (!Player.Instance.isFlipped)
            {
                Player.Instance.isFlipped = true;
                Debug.Log("Flip");
            }
            else if (Player.Instance.isFlipped)
            {
                Player.Instance.isFlipped = false;
                Debug.Log("Normal");
            }
        }
    }
}