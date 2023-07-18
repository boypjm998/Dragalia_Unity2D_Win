using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UI_FullScreenEffect_Blind : UI_FullScreenEffect
{
    PlayerStatusManager playerStatusManager;
    SpriteRenderer playerBlindView;
    CanvasGroup miniMapCanvasGroup;
    private Tweener _tweener2;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        _image = GetComponent<Image>();
        yield return new WaitUntil(()=>GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        playerStatusManager = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        playerBlindView = playerStatusManager.transform.Find("BlindView/Mask").GetComponent<SpriteRenderer>();
        playerStatusManager.OnBuffEventDelegate += OnBlindStart;
        playerStatusManager.OnBuffExpiredEventDelegate += OnBlindRelease;
        playerStatusManager.OnBuffDispelledEventDelegate += OnBlindRelease;
        miniMapCanvasGroup = transform.parent.parent.Find("Minimap")?.GetComponent<CanvasGroup>();
    }

    public override void Disable()
    {
        state = false;
        _tweener?.Complete();
        _tweener2?.Complete();
        _tweener = _image.DOColor(new Color(0,0,0,0), 0.3f);
        _tweener2 = playerBlindView.DOColor(new Color(1,1,1,0), 0.3f);
        miniMapCanvasGroup.alpha = 1f;
    }

    public override void Enable()
    {
        state = true;
        _tweener?.Complete();
        _tweener2?.Complete();
        _image.color = new Color(0,0,0,0f);
        _tweener = _image.DOColor(new Color(0,0,0,1f), 0.3f);
        _tweener2 = playerBlindView.DOColor(new Color(1,1,1,1), 0.3f);
        miniMapCanvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (playerStatusManager != null)
        {
            playerStatusManager.OnBuffEventDelegate -= OnBlindStart;
            playerStatusManager.OnBuffExpiredEventDelegate -= OnBlindRelease;
            playerStatusManager.OnBuffDispelledEventDelegate -= OnBlindRelease;
        }
        if(miniMapCanvasGroup != null)
            miniMapCanvasGroup.alpha = 1f;
    }

    public void OnBlindStart(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.Blindness)
        {
            if(state == false)
                Enable();
        }
    }

    public void OnBlindRelease(BattleCondition condition)
    {
        if (playerStatusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.Blindness).Count <= 0)
        {
            if(state == true)
                Disable();
        }
    }
}
