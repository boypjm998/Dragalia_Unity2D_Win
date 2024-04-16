using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ForceStrikeAimerMeeleAround : UI_ForceStrikeAimerMeele
{
    private Vector3 maxFillVector;
    private void Awake()
    {
        fillMask = transform.Find("Back/gauge");
        maxFillVector = fillMask.localScale;
        _SEsender = GetComponent<MuzzleSESender>();
        fx = transform.Find("fx");
        innerGaugeSprite = transform.Find("Back").GetComponent<SpriteRenderer>();
        originColor = innerGaugeSprite.color;
    }
    
    private void OnEnable()
    {

        fx.GetChild(0).gameObject.SetActive(true);
        fx.GetChild(1).gameObject.SetActive(false);
        innerGaugeSprite.color = originColor;
    }
    
    

    private void Update()
    {
        if (GlobalController.currentGameState != GlobalController.GameState.Inbattle)
        {
            gameObject.SetActive(false);
            ac.anim.SetInteger("force_level",-1);
            return;
        }

        if (ac.forceLevel < 0)
        {
            gameObject.SetActive(false);
            //Destroy(GetComponentInParent<AttackContainer>().gameObject);
            return;
        }


        if (ac.forceLevel < ac.maxForceLevel)
        {
            fillMask.transform.localScale =
                new Vector3((ac.forcingTime / forceTimeRequired[ac.forceLevel]),
                    (ac.forcingTime / forceTimeRequired[ac.forceLevel]),1);
            innerGaugeSprite.enabled = true;
        }
        else
        {
            //innerGaugeSprite.enabled = false;
            fillMask.transform.localScale = Vector3.one;
        }
            
        if (currentFSLV < ac.forceLevel)
        {
            _SEsender.SendVoiceToPlay(forceSE);
            if (ac.forceLevel == ac.maxForceLevel)
            {
                fx.GetChild(0).gameObject.SetActive(false);
                fx.GetChild(1).gameObject.SetActive(true);
                innerGaugeSprite.color = Color.green;
            }
        }
        
        currentFSLV = ac.forceLevel;
    }

    private void OnDisable()
    {
        fx.GetChild(1).gameObject.SetActive(false);
        fx.GetChild(0).gameObject.SetActive(false);
        print("Before:"+currentFSLV);
        if (ac.hurt)
            currentFSLV = 0;
        print("After:"+currentFSLV);
       
        
        //_attackManager.ForceStrikeRelease(currentFSLV);
    }
}
