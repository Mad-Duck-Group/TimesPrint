using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StopPlaceable : Placeable
{
    protected override void OnTrigger(Collider2D other)
    {
        if (other.gameObject != Player.Instance.gameObject) return;
        if (Player.Instance.isStop) return;
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.HitObject, out _);
        Player.Instance.isStop = true;
        Debug.Log("Stop");
    }
}
