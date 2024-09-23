using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceableSpike : Placeable
{
    protected override void OnCollision(Collision2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            GameManager.Instance.Restart();
        }
    }
}
