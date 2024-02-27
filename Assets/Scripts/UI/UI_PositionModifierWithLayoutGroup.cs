using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_PositionModifierWithLayoutGroup : MonoBehaviour
{
    private Vector2 initialPosition;
    [SerializeField] private Vector2 initialParentSize;
    private RectTransform rectTransform;
    private RectTransform parentRectSize;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialPosition = rectTransform.anchoredPosition;
        parentRectSize = transform.parent.GetComponent<RectTransform>();
        initialParentSize = parentRectSize.sizeDelta;
    }

    private void Start()
    {
        Vector2 parentSize = parentRectSize.sizeDelta;
        //根据父物体的大小和初始位置，计算出当前物体的位置
        Vector2 sizeDifference = (parentSize - initialParentSize);
        print(sizeDifference);
        print(parentRectSize.sizeDelta);
        //当前物体的位置 = 初始位置 + 父物体大小差值/2
        Vector2 currentPosition = initialPosition + sizeDifference / 2;
        //如果初始位置的x值大于0但是当前位置的x值小于0，说明父物体的宽度不够，需要将当前位置的x值设为0
        if (initialPosition.x > 0 && currentPosition.x < 0)
        {
            currentPosition.x = 0;
        }
        //如果初始位置的y值大于0但是当前位置的y值小于0，说明父物体的高度不够，需要将当前位置的y值设为0
        if (initialPosition.y > 0 && currentPosition.y < 0)
        {
            currentPosition.y = 0;
        }
        rectTransform.anchoredPosition = currentPosition;

    }

    private void Update()
    {
        print(parentRectSize.sizeDelta);
    }

    private void SetPosition()
    {
        /*
         * 1. 在Awake中获取到初始位置和父物体的初始大小
         * 2. 在Start中获取到父物体的当前大小
         * 3. 根据父物体的大小和初始位置，计算出当前物体的位置
         * 4. 如果初始位置的x值大于0但是当前位置的x值小于0，说明父物体的宽度不够，需要将当前位置的x值设为0
         * 5. 如果初始位置的y值大于0但是当前位置的y值小于0，说明父物体的高度不够，需要将当前位置的y值设为0
         * 6. 将当前位置赋值给当前物体
         */
        
        




    }
}
