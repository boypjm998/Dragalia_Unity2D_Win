using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D))]
public class TrailCollider : MonoBehaviour
{
    [SerializeField]
    [Tooltip("为了让碰撞效果和实际效果分开的同时提高性能," + 
             "可以将这个碰撞器设置为透明的，并减少其顶点数。")]
    private TrailRenderer trail;
    
    [SerializeField][Range(0,5f)] private float edgeRadius = 0.1f;
    
    [SerializeField][Range(0.1f,1f)]
    [Tooltip("这个值表示从轨迹的起点开始，到轨迹的百分之多少的位置，这个碰撞器才会开始生效。")]
    
    private float trailPercent = 0.5f;
    

    public float EdgeRadius
    {
        get => _edgeCollider2D.edgeRadius;
        set => _edgeCollider2D.edgeRadius = value;
    }

    private EdgeCollider2D _edgeCollider2D;

    private void Awake()
    {
        _edgeCollider2D = GetComponent<EdgeCollider2D>();
        _edgeCollider2D.edgeRadius = edgeRadius;
    }

    private void Update()
    {
        SetColliderPoints();
    }

    private void SetColliderPoints()
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

        for (int i = trail.positionCount - 1; i >= trail.positionCount - pointCount; i--)
        {
            points.Add(trail.GetPosition(i) - trail.transform.position);
        }
        print($"points: {points.Count}");

        _edgeCollider2D.SetPoints(points);

    }
}
