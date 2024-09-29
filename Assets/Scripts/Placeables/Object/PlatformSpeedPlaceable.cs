using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformSpeedPlaceable : Placeable
{
    [SerializeField] private float speedMultiplier;
    private MovingPlatform _movingPlatform;
    private float _originalSpeed;

    protected override void ActivateObjectItem(Placeable owner)
    {
        base.ActivateObjectItem(owner);
        Debug.Log(targetPlaceable.name);
        _movingPlatform = owner.GetComponent<MovingPlatform>();
        _originalSpeed = _movingPlatform.Speed;
        _movingPlatform.Speed *= speedMultiplier;
    }

    protected override void DeactivateObjectItem()
    {
        Debug.Log("adasdasd");
        _movingPlatform.Speed = _originalSpeed;
        _movingPlatform = null;
        base.DeactivateObjectItem();
    }
}
