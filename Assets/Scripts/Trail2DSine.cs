using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(TrailRenderer))]
public class Trail2DSine : MonoBehaviour
{
    TrailRenderer trail;
    public float amplitude = 1f;
    public float speed = 8;
    private float currentTime = 0;
    [Range(0,1)]public float startOffsetX = 0;
    public float startOffsetY = 0;

    private float offsetX;
    //public float startOffsetY = 0;
    
    void Start()
    {
        trail = GetComponent<TrailRenderer>();
        //把x映射到0-2π
        offsetX = startOffsetX * Mathf.PI * 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //物体按照正弦波曲线移动，x为时间，y为正弦函数
        Vector3 pos = transform.localPosition;
        pos.y = amplitude * Mathf.Sin((currentTime + offsetX) * speed);
        transform.localPosition = pos;
        transform.localRotation = transform.parent.rotation;
        currentTime += Time.fixedDeltaTime;

    }
}
