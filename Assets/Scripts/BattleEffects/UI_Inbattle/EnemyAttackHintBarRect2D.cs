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
    [SerializeField] private bool doScale = false;
    
    private enum RectFillType
    {
        X,
        Y
    }

    [SerializeField] private RectFillType fillType;
    
    public void SetFillAxis(int axis)
    {
        fillType = (RectFillType) axis;
    }

    private void Awake()
    {
        if (doScale)
        {
            if (fillType == RectFillType.X)
            {
                transform.localScale = new Vector3(transform.localScale.x * 0.1f, transform.localScale.y, transform.localScale.z);
            }else if (fillType == RectFillType.Y)
            {
                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.1f, transform.localScale.z);
            }
        }
    }

    public GameObject SetDoScale(int axis)
    {
        doScale = true;
        if (axis == 0)
        {
            transform.localScale = new Vector3(transform.localScale.x * 0.1f, transform.localScale.y, transform.localScale.z);
        }else if (axis == 1)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 0.1f, transform.localScale.z);
        }

        return gameObject;
    }

    protected override IEnumerator Start()
    {
        yield return base.Start();
        
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
            maxFillSize, warningTime).SetEase(Ease.Linear);

        if (doScale)
        {
            transform.DOScale(1,Mathf.Max(warningTime / 10f,0.1f));
        }

        
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
