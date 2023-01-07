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
    private Tweener _tweener;
    private enum RectFillType
    {
        X,
        Y
    }

    [SerializeField] private RectFillType fillType;
    
    protected override void Start()
    {
        base.Start();
        
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

        _tweener = DOTween.To(() => fillRenderer.size,
            x => fillRenderer.size = x,
            maxFillSize, warningTime);

        
    }

    // Update is called once per frame
    protected virtual void OnTweenCompleted()
    {
        print("Completed");
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _tweener.Kill();
    }
}
