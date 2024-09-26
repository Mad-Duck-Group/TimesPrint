using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformStopPlaceable : Placeable
{
    [SerializeField] private float duration = 3f;
    private MovingPlatform _movingPlatform;
    private float _originalSpeed;
    private float _timer;
    private bool _isActivated;
    
    protected override void ReactivateObjectItem(Placeable owner)
    {
        base.ReactivateObjectItem(owner);
        _isActivated = false;
        _timer = 0;
        _movingPlatform.Speed = 0;
    }
    protected override void ActivateObjectItem(Placeable owner)
    {
        base.ActivateObjectItem(owner);
        _movingPlatform = owner.GetComponent<MovingPlatform>();
        _originalSpeed = _movingPlatform.Speed;
        _movingPlatform.Speed = 0;
    }

    protected override void UpdateObjectItem()
    {
        if (_isActivated) return;
        _timer += Time.deltaTime;
        if (_timer < duration) return;
        _movingPlatform.Speed = _originalSpeed;
        _timer = 0;
        _isActivated = true;
    }

    protected override void DeactivateObjectItem()
    {
        _movingPlatform.Speed = _originalSpeed;
        _movingPlatform = null;
        base.DeactivateObjectItem();
    }
}
