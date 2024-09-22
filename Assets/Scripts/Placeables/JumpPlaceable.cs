using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlaceable : Placeable
{
    [SerializeField] private float jumpForce = 5;
   
    protected override void Trigger(Collider2D other)
    {
        if (other.gameObject == Player.Instance.gameObject)
        {
            Player.Instance.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0, jumpForce); //Cache rigidbody in Player get it from there instead
        }
    }
}
