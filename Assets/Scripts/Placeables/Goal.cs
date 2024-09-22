using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : Placeable
{
    protected override void Trigger(Collider2D other)
    {
        if (other.gameObject != Player.Instance.gameObject) return;
        Debug.Log("Goal");
        GameManager.Instance.Win();
    }
}
