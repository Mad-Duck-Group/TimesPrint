using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlaceable : Placeable
{
    protected override void Trigger(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            if (!Player.Instance.isStop)
            {
                Player.Instance.isStop = true;
                Debug.Log("Stop");
            }
        }
    }
}
