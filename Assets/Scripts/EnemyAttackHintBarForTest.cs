using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyAttackHintBarForTest : EnemyAttackHintBar
{
    // Start is called before the first frame update

    public float demoWarningTimeLeft = 2;
    private bool demoSwitch = false;

    private void Update()
    {
        demoWarningTimeLeft -= Time.deltaTime;
        if (demoWarningTimeLeft <= warningTime - timeToSendEvadeSignal && demoSwitch == false)
        {
            if(timeToSendEvadeSignal == 0)
                return;
            demoSwitch = true;
        }
        
    }

    private void OnDestroy()
    {
        
    }


    protected override void Start()
    {
        base.Start();
        
        warningTime = 2;

        Fill = transform.Find("Fill")?.gameObject;
        if(Fill == null)
            return;

        MaxFill = transform.Find("Back").gameObject;
        var fillRenderer = Fill.GetComponent<SpriteRenderer>();

        var maxFillSize = MaxFill.GetComponent<SpriteRenderer>().size;
        fillRenderer.size = new Vector2(fillRenderer.size.x, maxFillSize.y);
        
        _tweener = DOTween.To(() => fillRenderer.size,
            x => fillRenderer.size = x,
            maxFillSize, warningTime);
        
        _tweener.OnComplete(()=>
        {
            fillRenderer.size = new Vector2(0.1f, maxFillSize.y);
            demoWarningTimeLeft = warningTime;
            demoSwitch = false;
            //循环播放
            _tweener.Restart();
        });
    }
    
    protected override float GetWarningTimeLeft()
    {
        return demoWarningTimeLeft;
    }
}