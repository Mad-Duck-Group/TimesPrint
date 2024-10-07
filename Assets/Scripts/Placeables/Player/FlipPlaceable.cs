using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlipPlaceable : Placeable
{
    protected override void OnTrigger(Collider2D other)
    {
        if (other.gameObject != Player.Instance.gameObject) return;
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.HitObject, out _);
        switch (Player.Instance.isFlipped)
        {
            case false:
                Player.Instance.isFlipped = true;
                Debug.Log("Flip");
                break;
            case true:
                Player.Instance.isFlipped = false;
                Debug.Log("Normal");
                break;
        }
    }
}