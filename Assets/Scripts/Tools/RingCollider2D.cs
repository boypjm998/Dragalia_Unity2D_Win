using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingCollider2D : MonoBehaviour
{
    PolygonCollider2D collider2D;
    //[SerializeField] private LayerMask layerMask;
    [SerializeField] private float innerRadius;
    [SerializeField] private float outerRadius;
    [SerializeField] private bool isTrigger;
    [SerializeField] private bool isEnabled;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            Debug.Log("Triggered");
    }

    private void Awake()
    {
        collider2D = gameObject.AddComponent<PolygonCollider2D>();
        collider2D.pathCount = 2;
        collider2D.isTrigger = isTrigger;
        collider2D.enabled = isEnabled;
        collider2D.pathCount = 2;
        collider2D.SetPath(1, GeneratePoints(innerRadius));
        collider2D.SetPath(0, GeneratePoints(outerRadius));

    }

    private void Update()
    {
        collider2D.isTrigger = isTrigger;
    }

    private List<Vector2> GeneratePoints(float radius)
    {
        int pointCount = 16;
        if (radius >= 8)
        {
            pointCount = 24;
        }else if (radius >= 15)
        {
            pointCount = 32;
        }else if (radius >= 20)
        {
            pointCount = 40;
        }
        else if(radius >= 30)
        {
            pointCount = 64;
        }

        List<Vector2> list = new();
        
        for (int i = 0; i < pointCount; i++)
        {
            float angle = i * Mathf.PI * 2 / pointCount;
            Vector2 pos = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
            list.Add(pos);
        }
        
        return list;

    }






}
