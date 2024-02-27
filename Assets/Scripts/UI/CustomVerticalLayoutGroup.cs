using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(RectTransform))]
public class CustomVerticalLayoutGroup : MonoBehaviour
{
    public float space = 100f; //间隔高度
    public List<GameObject> ignoreList; //忽略列表

    private RectTransform rectTransform; //自身的rectTransform组件
    private List<RectTransform> children; //子对象的rectTransform组件列表

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        children = new List<RectTransform>();
    }

    private void Start()
    {
        //对所有子对象进行遍历，筛选出需要布局的对象，加入children列表
        foreach (Transform child in transform)
        {
            if (!ignoreList.Contains(child.gameObject) && child.gameObject.activeSelf)
            {
                RectTransform childRect = child.GetComponent<RectTransform>();
                if (childRect != null)
                {
                    children.Add(childRect);
                }
            }
        }

        //对children列表进行排序，按照y坐标从大到小排列
        children.Sort((a, b) => b.anchoredPosition.y.CompareTo(a.anchoredPosition.y));

        //计算自身的高度，等于（子对象高度和+（子对象数量-1）*间隔高度）
        float totalHeight = 0f;
        foreach (RectTransform child in children)
        {
            totalHeight += child.rect.height;
        }
        totalHeight += (children.Count - 1) * space;

        //设置自身的高度
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, totalHeight);

        //调整子对象的坐标，使其间隔相等
        float currentY = totalHeight / 2f; //当前的y坐标，从上往下递减
        foreach (RectTransform child in children)
        {
            //设置子对象的锚点为中心
            child.anchorMin = new Vector2(0.5f, 0.5f);
            child.anchorMax = new Vector2(0.5f, 0.5f);

            //设置子对象的y坐标为当前的y坐标减去子对象高度的一半
            child.anchoredPosition = new Vector2(child.anchoredPosition.x, currentY - child.rect.height / 2f);

            //更新当前的y坐标，减去子对象的高度和间隔高度
            currentY -= child.rect.height + space;
        }
    }
}