using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RingCollider2D : MonoBehaviour
{
    CustomCollider2D collider;
    public float innerRadius = 2;
    public float outerRadius = 1;
    public bool Enabled = false;

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
            Debug.Log("Triggered");
    }

    private void Awake()
    {
        //定义一个环形碰撞器,碰撞区域为内半径为1,外半径为2的环形区域

        collider = gameObject.AddComponent<CustomCollider2D>();
        collider.enabled = Enabled;
        collider.isTrigger = true;
        var shapeGroup = new PhysicsShapeGroup2D();

        // 创建外圆
        shapeGroup.AddCircle(Vector2.zero, outerRadius);

        // 创建内圆
        shapeGroup.AddCircle(Vector2.zero, innerRadius);

        // 设置自定义形状
        collider.SetCustomShapes(shapeGroup);

        collider.SetCustomShapes(shapeGroup);

        
    }

        

    
}
