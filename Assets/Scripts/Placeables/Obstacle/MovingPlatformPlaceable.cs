using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovingPlatform))]
[RequireComponent(typeof(LineRenderer))]
public class MovingPlatformPlaceable : Placeable
{
   private MovingPlatform _movingPlatform;
   private LineRenderer _lineRenderer;

   private void Awake()
   {
      _movingPlatform = GetComponent<MovingPlatform>();
      _lineRenderer = GetComponent<LineRenderer>();
      Vector3[] positions = _movingPlatform.GetDrawLinePositions();
      _lineRenderer.positionCount = positions.Length;
      _lineRenderer.SetPositions(positions);
   }
   
   protected override void PlayOrPause(bool isPaused, bool beforePlay)
   {
        _lineRenderer.enabled = isPaused;
   }

   public override bool CanPlace()
   {
      bool canPlace = false;
      switch (placeableCollider)
      {
         case BoxCollider2D:
            canPlace = CanPlaceBoxCollider();
            break;
         case CircleCollider2D:
            canPlace = CanPlaceCircleCollider();
            break;
         case CapsuleCollider2D:
            canPlace = CanPlaceCapsuleCollider();
            break;
      }
      canPlace = PlaceableAreaManager.Instance.WithinBound(transform) && canPlace;
      spriteRenderer.color = canPlace ? Color.green : Color.red;
      Vector3[] positions = _movingPlatform.GetDrawLinePositions();
      _lineRenderer.positionCount = positions.Length;
      _lineRenderer.SetPositions(positions);
      return canPlace;
   }
}
