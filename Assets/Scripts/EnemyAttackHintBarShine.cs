using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyAttackHintBarShine : EnemyAttackHintBar
{
    // Start is called before the first frame update
    private SpriteRenderer fillRenderer;
    [SerializeField] private float shineTime = 0.2f;
    void Start()
    {
        Fill = transform.Find("Fill").gameObject;
        fillRenderer = Fill.GetComponent<SpriteRenderer>();
        Invoke(nameof(BarShine),warningTime + awakeTime);
        if (autoDestruct)
        {
            Destroy(gameObject,awakeTime + warningTime + attackLastTime + shineTime*2f);
        }
    }

    // Update is called once per frame
    protected void BarShine()
    {
        fillRenderer.color = new Color(1, 1, 1, 0);
        _tweener = fillRenderer.DOColor(new Color(1, 1, 1, 0.5f), shineTime);
        _tweener.OnComplete(() =>
        {
            _tweener = fillRenderer.DOColor(new Color(1, 1, 1, 0), shineTime);
        });
    }

    protected override void OnDestroy()
    {
        CancelInvoke();
    }
}
