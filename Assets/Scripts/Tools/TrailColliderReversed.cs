using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailColliderReversed : TrailCollider
{
    protected override void SetColliderPoints()
    {
        List<Vector2> points = new();

        int pointCount = Mathf.CeilToInt(trail.positionCount * trailPercent);

        if (pointCount < 2)
        {
            points.Clear();
            points.Add(Vector2.zero);
            
            _edgeCollider2D.enabled = false;
            return;
        }

        _edgeCollider2D.enabled = true;

        for (int i = 0; i < pointCount; i++)
        {
            var trailPos = trail.GetPosition(i);
            //trailPos = new Vector3(trailPos.x / transform.lossyScale.x, trailPos.y / transform.lossyScale.y);

            //var worldPos = trail.transform.position;
            var localPos = trail.transform.InverseTransformPoint(trailPos);
            
            
            points.Add(localPos);
        }
        print($"points: {points.Count}");

        _edgeCollider2D.SetPoints(points);
    }
}
