using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ForceStrikeAimerArrow : MonoBehaviour
{
    //private int maxForceLevel = 1;
    private List<float> forceTimeRequired = new();
    private float currentForceTime = 0;
    int currentFSLV = 0;

    private Transform fillMask;
    private float maxFillWidth;
    private MuzzleSESender _SEsender;
    
    public ActorControllerRangedWithFS ac;
    protected AttackManagerRanged _attackManagerRanged;
    public AudioClip forceSE;
    protected SpriteRenderer innerGaugeSprite;
    protected Color originColor;

    private Transform fx;
    private void Awake()
    {
        fillMask = transform.Find("inner/gauge");
        maxFillWidth = fillMask.localScale.x;
        _SEsender = GetComponent<MuzzleSESender>();
        fx = transform.Find("fx");
        innerGaugeSprite = transform.Find("inner").GetComponent<SpriteRenderer>();
        originColor = innerGaugeSprite.color;
    }

    private void OnEnable()
    {
        fx.GetChild(0).gameObject.SetActive(true);
        fx.GetChild(1).gameObject.SetActive(false);
        innerGaugeSprite.color = originColor;
    }

    private void OnDisable()
    {
        fx.GetChild(1).gameObject.SetActive(false);
        fx.GetChild(0).gameObject.SetActive(false);
        if (ac.hurt)
            currentFSLV = 0;
        _attackManagerRanged.ForceStrikeRelease(currentFSLV);
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
            fillMask.transform.localPosition =
                new Vector3((ac.forcingTime / forceTimeRequired[ac.forceLevel] - 1)*maxFillWidth,
                    0,0);
        }
        else
        {
            fillMask.transform.localPosition = Vector3.zero;
        }
        
        if (currentFSLV < ac.forceLevel)
        {
            _SEsender.SendVoiceToPlay(forceSE);
            if (ac.forceLevel == ac.maxForceLevel)
            {
                fx.GetChild(0).gameObject.SetActive(false);
                fx.GetChild(1).gameObject.SetActive(true);
                innerGaugeSprite.color = Color.yellow;
            }
        }
        currentFSLV = ac.forceLevel;
    }

    public void SetActorController(ActorControllerRangedWithFS ac)
    {
        this.ac = ac;
        _attackManagerRanged = ac.GetComponent<AttackManagerRanged>();
    }

    public void SetMaxForceInfo(List<float> info)
    {
        forceTimeRequired = info;
        
    }
}
