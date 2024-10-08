using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikePlaceable : Placeable
{
    [SerializeField] private bool isWall;
    protected override void OnCollision(Collision2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.Dead(isWall);
        }
    }
}
