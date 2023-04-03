using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyAttackHintBarCircle : EnemyAttackHintBar
{
    private Vector2 maxFillSize;
    private SpriteRenderer fillRenderer;
    
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        Fill = transform.Find("Fill").gameObject;
        MaxFill = transform.Find("Back").gameObject;
        fillRenderer = Fill.GetComponent<SpriteRenderer>();
        maxFillSize = MaxFill.GetComponent<SpriteRenderer>().size;
        fillRenderer.size = new Vector2(fillRenderer.size.x, fillRenderer.size.y);
        
        _tweener = DOTween.To(() => fillRenderer.size,
            x => fillRenderer.size = x,
            maxFillSize, warningTime);
        
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _tweener.Kill();
    }
}
