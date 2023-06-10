using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class UI_SimpleHPGauge : MonoBehaviour
{
    private string displayedName;
    private StatusManager statusManager;
    private SpriteRenderer HPGaugeSpriteRenderer;
    private SpriteRenderer HPValueSpriteRenderer;
    private TextMeshPro nameText;
    private Tweener[] _tweeners;

    private float currentHP;
    private float fullSize;

    Coroutine hpBarAnimationCoroutine;
    private void Awake()
    {
        _tweeners = new Tweener[3];
        nameText = GetComponentInChildren<TextMeshPro>();
        HPGaugeSpriteRenderer = GetComponent<SpriteRenderer>();
        HPValueSpriteRenderer = transform.Find("Value").GetComponent<SpriteRenderer>();
        statusManager = GetComponentInParent<StatusManager>();
        displayedName = statusManager.displayedName;
        nameText.text = displayedName;
        statusManager.OnHPChange += ExcuteHPBar;
        fullSize = HPValueSpriteRenderer.size.x;
        SetFakeInactive();
    }

    private void OnDestroy()
    {
        statusManager.OnHPChange -= ExcuteHPBar;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void ExcuteHPBar()
    {
        HPValueSpriteRenderer.size = 
            new Vector2(fullSize * statusManager.currentHp / statusManager.maxHP,
                HPValueSpriteRenderer.size.y);

        if (statusManager.currentHp <= 0)
        {
            HPValueSpriteRenderer.color = new Color(HPValueSpriteRenderer.color.r, HPValueSpriteRenderer.color.g,
                HPValueSpriteRenderer.color.b, 0);
        }
        else
        {
            HPValueSpriteRenderer.color = new Color(HPValueSpriteRenderer.color.r, HPValueSpriteRenderer.color.g,
                HPValueSpriteRenderer.color.b, 1);
        }

        if(hpBarAnimationCoroutine != null)
            StopCoroutine(hpBarAnimationCoroutine);
        hpBarAnimationCoroutine = StartCoroutine(HPBarAnimation());
        
        
    }
    
    IEnumerator HPBarAnimation()
    {
        ClearTweeners();
        
        HPGaugeSpriteRenderer.color = new Color(1, 1, 1, 1);
        HPValueSpriteRenderer.color = new Color(1, 0, 0, 1);
        nameText.color = new Color(1, 1, 1, 1);

        yield return new WaitForSeconds(2f);
        
        _tweeners[0] = HPGaugeSpriteRenderer.DOColor(new Color(1, 1, 1, 0), 2f);
        _tweeners[1] = HPValueSpriteRenderer.DOColor(new Color(1, 0, 0, 0), 2f);
        _tweeners[2] = nameText.DOColor(new Color(1, 1, 1, 0), 2f);
        
        
    }
    
    void ClearTweeners()
    {
        for (int i = 0; i < _tweeners.Length; i++)
        {
            if (_tweeners[i] != null)
            {
                _tweeners[i].Kill();
                _tweeners[i] = null;
            }
        }
    }

    void SetFakeInactive()
    {
        HPGaugeSpriteRenderer.color = new Color(1, 1, 1, 0);
        HPValueSpriteRenderer.color = new Color(1, 0, 0, 0);
        nameText.color = new Color(1, 1, 1, 0);
    }

}
