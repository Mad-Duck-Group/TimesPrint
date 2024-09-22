using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPlaceable : Placeable
{
    protected override void Trigger(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            if (Player.Instance.isFlipped == false) //you can !
            {
                Player.Instance.isFlipped = true;
                Debug.Log("Flip");
            }
            else if (Player.Instance.isFlipped == true) //remove check for true
            {
                Player.Instance.isFlipped = false;
                Debug.Log("Normal");
            }
        }
    }
}