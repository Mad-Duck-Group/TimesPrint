using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlaceable : Placeable
{
    [SerializeField] private float jumpForce = 5;
   
    
    protected override void OnTrigger(Collider2D other)
    {
        if (other.gameObject != Player.Instance.gameObject) return;
        SoundManager.Instance.PlaySoundFX(SoundFXTypes.HitObject, out _);
        Player.Instance.Jump(jumpForce);
    }
}
