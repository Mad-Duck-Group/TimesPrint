using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformReversePlaceable : Placeable
{
    private MovingPlatform _movingPlatform;

    protected override void ActivateObjectItem(Placeable owner)
    {
        base.ActivateObjectItem(owner);
        _movingPlatform = owner.GetComponent<MovingPlatform>();
        _movingPlatform.ReverseDirection();
    }

    protected override void DeactivateObjectItem()
    {
        _movingPlatform.ReverseDirection();
        _movingPlatform = null;
        base.DeactivateObjectItem();
    }
}
