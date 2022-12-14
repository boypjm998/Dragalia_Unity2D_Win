using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class EnemyAttackHintBarRect2D : EnemyAttackHintBar
{
    private Vector2 maxFillSize;
    private SpriteRenderer fillRenderer;
    private enum RectFillType
    {
        X,
        Y
    }

    [SerializeField] private RectFillType fillType;
    
    void Start()
    {
        Fill = transform.Find("Fill").gameObject;
        MaxFill = transform.Find("Back").gameObject;
        fillRenderer = Fill.GetComponent<SpriteRenderer>();
        if (fillType == RectFillType.X)
        {
            maxFillSize = MaxFill.GetComponent<SpriteRenderer>().size;
            fillRenderer.size = new Vector2(fillRenderer.size.x, maxFillSize.y);
        }
        else
        {
            maxFillSize = MaxFill.GetComponent<SpriteRenderer>().size;
            fillRenderer.size = new Vector2(maxFillSize.x, fillRenderer.size.y);
        }

        DOTween.To(() => fillRenderer.size,
            x => fillRenderer.size = x,
            maxFillSize, warningTime);

        
    }

    // Update is called once per frame
    protected virtual void OnTweenCompleted()
    {
        print("Completed");
    }

    private void OnDestroy()
    {
        DOTween.KillAll();
    }
}
