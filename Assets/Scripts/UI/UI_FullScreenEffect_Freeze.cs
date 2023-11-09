using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEngine.UI;

public class UI_FullScreenEffect_Freeze : UI_FullScreenEffect
{
    PlayerStatusManager playerStatusManager;
    SpriteRenderer playerBlindView;
    //CanvasGroup miniMapCanvasGroup;
    //private Tweener _tweener2;
    private Color normalColor = new Color(0.5f, 0.6f, 0.6f, 0.7f);
    
    IEnumerator Start()
    {
        _image = GetComponent<Image>();
        yield return new WaitUntil(()=>GlobalController.currentGameState == GlobalController.GameState.Inbattle);
        playerStatusManager = GameObject.Find("PlayerHandle").GetComponent<PlayerStatusManager>();
        //playerBlindView = playerStatusManager.transform.Find("BlindView/Mask").GetComponent<SpriteRenderer>();
        playerStatusManager.OnBuffEventDelegate += OnFreezeStart;
        playerStatusManager.OnBuffExpiredEventDelegate += OnFreezeRelease;
        playerStatusManager.OnBuffDispelledEventDelegate += OnFreezeRelease;
        //miniMapCanvasGroup = transform.parent.parent.Find("Minimap")?.GetComponent<CanvasGroup>();
    }

    public override void Disable()
    {
        state = false;
        _tweener?.Complete();
        //_tweener2?.Complete();
        _tweener = _image.DOColor(new Color(0,0,0,0), 0.5f);
        //_tweener2 = playerBlindView.DOColor(new Color(1,1,1,0), 0.3f);
        //miniMapCanvasGroup.alpha = 1f;
    }

    public override void Enable()
    {
        state = true;
        _tweener?.Complete();
        //_tweener2?.Complete();
        _image.color = new Color(0,0,0,0f);
        _tweener = _image.DOColor(normalColor, 0.5f);
        //_tweener2 = playerBlindView.DOColor(new Color(1,1,1,1), 0.3f);
        //miniMapCanvasGroup.alpha = 0f;
    }

    private void OnDestroy()
    {
        if (playerStatusManager != null)
        {
            playerStatusManager.OnBuffEventDelegate -= OnFreezeStart;
            playerStatusManager.OnBuffExpiredEventDelegate -= OnFreezeRelease;
            playerStatusManager.OnBuffDispelledEventDelegate -= OnFreezeRelease;
        }
        
    }

    public void OnFreezeStart(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.Freeze)
        {
            if(state == false)
                Enable();
        }
    }

    public void OnFreezeRelease(BattleCondition condition)
    {
        if (playerStatusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.Freeze).Count <= 0)
        {
            if(state == true)
                Disable();
        }
    }
}
