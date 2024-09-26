using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalPlaceable : Placeable
{
    protected override void OnTrigger(Collider2D other)
    {
        if (other.gameObject != Player.Instance.gameObject) return;
        Debug.Log("Goal");
        GameManager.Instance.Win();
    }
}
