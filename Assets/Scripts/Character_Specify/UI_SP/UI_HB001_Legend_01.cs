using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using GameMechanics;
using UnityEngine;

public class UI_HB001_Legend_01 : MonoBehaviour
{
    public float currentValue = 0;
    public float maxValue = 1;
    public int level = 1;
    public List<float> levels = new();
    
    public bool ticking = false;
    public int colliderWithAreaNum;

    private SpriteRenderer frontGauge;
    private SpriteRenderer backGauge;
    private GameObject fxParent;
    
    public Color level1Color = Color.yellow;
    public Color level2Color = new Color(1,0.3f,0);
    public Color level3Color = new Color(0.5f, 0.2f, 1);
    public Color level0Color = Color.gray;
    
    MaterialPropertyBlock block;

    private StatusManager _statusManager;

    private int scorchingEnergy
    {
        get => (int)_statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy);

        set
        {
            if (value > 0)
            {
                var newbuff = new TimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,
                    value, -1, 1,8101401);

                if (value < scorchingEnergy)
                {
                    _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,false
                        ,8101401);
                }

                _statusManager.ObtainTimerBuff(newbuff);
                BuffWithFlameLevelCheck(value-1);
            }
            else
            {
                _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.ScorchingEnergy,
                    8101401, true);
                BuffWithFlameLevelCheck(-1);
            }


        }

    }

        
    

    
    
    //20081: Level Down
    //20091: Level Up
    public int CurrentWorld
    {
        get
        {
            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20081))
                return 1;
            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20091))
                return 2;
            return 0;
        }
    }

    public int currentWorld;

    private void Awake()
    {
        frontGauge = transform.Find("front").GetComponent<SpriteRenderer>();
        
        backGauge = transform.Find("back").GetComponent<SpriteRenderer>();
        fxParent = transform.Find("fx").gameObject;
        
        block = new MaterialPropertyBlock();
        
        BattleStageManager.Instance.OnFieldAbilityAdd += CheckCurrentWorld;
        BattleStageManager.Instance.OnFieldAbilityRemove += CheckCurrentWorldWithoutReset;
        _statusManager = GetComponentInParent<StatusManager>();
        _statusManager.OnBuffEventDelegate += CheckNumOfStack;
        _statusManager.OnBuffDispelledEventDelegate += CheckNumOfStack;
        
        frontGauge.color = level0Color;
        backGauge.color = level0Color;
    }

    private void Start()
    {
        CheckNumOfStack(null);
    }


    // Update is called once per frame
    void Update()
    {
        if (!ticking)
        {
            currentValue = 0;
            UpdateGauge();
            return;
        }

        
        
        switch (currentWorld)
        {
            case 1:
            {
                if (scorchingEnergy <= 0)
                {
                    CheckNumOfStack(null);
                    return;
                }

                
                currentValue += Time.deltaTime;
                if (level <= levels.Count)
                {
                    if (currentValue > levels[level - 1])
                    {
                        level++;
                        currentValue = 0;
                        scorchingEnergy--;
                        CheckNumOfStack(null);
                    }
                }
                else
                {
                    if (currentValue > levels[levels.Count - 1])
                    {
                        currentValue = 0;
                        scorchingEnergy--;
                        CheckNumOfStack(null);
                    }
                }
                break;
            }
            case 2:
            {
                if (scorchingEnergy >= 3)
                {
                    CheckNumOfStack(null);
                    return;
                }
                currentValue += Time.deltaTime;
                if (level <= levels.Count)
                {
                    if (currentValue > levels[level - 1])
                    {
                        level++;
                        currentValue = 0;
                        scorchingEnergy++;
                        CheckNumOfStack(null);
                    }
                }
                else
                {
                    if (currentValue > levels[levels.Count - 1])
                    {
                        currentValue = 0;
                        scorchingEnergy++;
                        CheckNumOfStack(null);
                    }
                }
                break;
            }
        }
        
        UpdateGauge();
        
        
        
        // currentValue -= Time.deltaTime;
        // if (currentValue <= 0)
        // {
        //     switch (level)
        //     {
        //         case 1:
        //             maxValue = 5;
        //             break;
        //         case 2:
        //             maxValue = 15;
        //             break;
        //         default:
        //             maxValue = 30;
        //             break;
        //     }
        //     level++;
        //     if(level > 3)
        //         level = 3;
        //     currentValue = maxValue;
        // }
        //
        // UpdateGauge();
    }

    void UpdateGauge()
    {
        try
        {
            maxValue = levels[level - 1];
        }
        catch
        {
            maxValue = levels[^1];
        }

        if (!ticking)
        {
            frontGauge.color = new Color(frontGauge.color.r, frontGauge.color.g, frontGauge.color.b, 0);
            backGauge.color = new Color(backGauge.color.r, backGauge.color.g, backGauge.color.b, 0);
        }
        else
        {
            frontGauge.color = new Color(frontGauge.color.r, frontGauge.color.g, frontGauge.color.b, 1);
            backGauge.color = new Color(backGauge.color.r, backGauge.color.g, backGauge.color.b, 1);
        }

        float fillAmount = currentValue / maxValue;
        frontGauge.GetPropertyBlock(block);
        block.SetFloat("_FillAmount", fillAmount);
        frontGauge.SetPropertyBlock(block);
    }
    
    void CheckNumOfStack(BattleCondition condition)
    {
        var scorchingEnergy =
            _statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchingEnergy);
        CheckCurrentWorldWithoutReset(999);
        if (scorchingEnergy <= 0)
        {
            fxParent.SetActive(false);
            // backGauge.enabled = false;
            // frontGauge.enabled = false;
            if (currentWorld != 1)
            {
                backGauge.color = level0Color;
                frontGauge.color = level1Color;
            }
            else
            {
                backGauge.color = level0Color;
                frontGauge.color = level0Color;
            }

        }
        else if(scorchingEnergy == 1)
        {
            fxParent.SetActive(true);
            fxParent.transform.GetChild(0).gameObject.SetActive(true);
            fxParent.transform.GetChild(1).gameObject.SetActive(false);
            fxParent.transform.GetChild(2).gameObject.SetActive(false);
            if (currentWorld == 1)
            {
                backGauge.color = level1Color;
                frontGauge.color = level0Color;
            }else if(currentWorld == 2)
            {
                backGauge.color = level1Color;
                frontGauge.color = level2Color;
            }
            else
            {
                backGauge.color = level1Color;
                frontGauge.color = level1Color;
            }


        }else if(scorchingEnergy == 2)
        {
            fxParent.SetActive(true);
            fxParent.transform.GetChild(0).gameObject.SetActive(false);
            fxParent.transform.GetChild(1).gameObject.SetActive(true);
            fxParent.transform.GetChild(2).gameObject.SetActive(false);

            if (currentWorld == 1)
            {
                backGauge.color = level2Color;
                frontGauge.color = level1Color;
            }
            else if(currentWorld == 2)
            {
                backGauge.color = level2Color;
                frontGauge.color = level3Color;
            }
            else
            {
                backGauge.color = level2Color;
                frontGauge.color = level2Color;
            }

        }else
        {
            fxParent.SetActive(true);
            fxParent.transform.GetChild(0).gameObject.SetActive(false);
            fxParent.transform.GetChild(1).gameObject.SetActive(false);
            fxParent.transform.GetChild(2).gameObject.SetActive(true);
            
            if(currentWorld == 1)
            {
                backGauge.color = level3Color;
                frontGauge.color = level2Color;
            }
            else
            {
                backGauge.color = level3Color;
                frontGauge.color = level3Color;
            }

        }
        
        
    }

    private void OnDestroy()
    {
        BattleStageManager.Instance.OnFieldAbilityAdd -= CheckCurrentWorld;
        BattleStageManager.Instance.OnFieldAbilityRemove -= CheckCurrentWorldWithoutReset;
    }

    public void TriggerIncrement()
    {
        StartTicking();
        colliderWithAreaNum++;
    }
    public void TriggerDecrement()
    {
        colliderWithAreaNum--;
        if(colliderWithAreaNum <= 0)
            StartTicking(false);
    }
    

    public void StartTicking(bool flag = true)
    {
        if(currentWorld == 0)
            return;
        
        ticking = flag;
        currentWorld = CurrentWorld;
        
    }

    void CheckCurrentWorld(int id)
    {
        if(CurrentWorld == 0)
            StartTicking(false);
        if (currentWorld != CurrentWorld)
        {
            currentValue = 0;
            //ResetLevel();
            currentWorld = CurrentWorld;
        }

        if (CurrentWorld == 0)
        {
            ResetLevel();
        }
    }

    void CheckCurrentWorldWithoutReset(int id)
    {
        if(CurrentWorld == 0)
            StartTicking(false);
        if (currentWorld != CurrentWorld)
        {
            currentValue = 0;
            //ResetLevel();
            currentWorld = CurrentWorld;
        }
    }



    public void ResetLevel()
    {
        level = 1;
    }


    public void BuffWithFlameLevelCheck(int scorchingLevel)
    {
        
        CheckNumOfStack(null);
        UpdateGauge();
        
        if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20081))
        {

            _statusManager.RemoveAllConditionWithSpecialID(8101402);
                if(scorchingLevel<0)
                    return;
                
                
                var buff1 = new TimerBuff(HB01_BehaviorTree_Legend2.brightBuff_1);
                buff1.SetEffect(buff1.effect + scorchingLevel * 5);
                var buff2 = new TimerBuff(HB01_BehaviorTree_Legend2.brightBuff_2);
                buff2.SetEffect(buff2.effect + scorchingLevel * 5);
                var buff3 = new TimerBuff(HB01_BehaviorTree_Legend2.brightBuff_3);
                buff3.SetEffect(buff3.effect + scorchingLevel * 20);
                var buff4 = new TimerBuff(HB01_BehaviorTree_Legend2.brightBuff_4);
                buff4.SetEffect(buff4.effect + scorchingLevel * 5);
                
                _statusManager.ObtainTimerBuff(buff1);
                _statusManager.ObtainTimerBuff(buff2,false);
                _statusManager.ObtainTimerBuff(buff3,false);
                _statusManager.ObtainTimerBuff(buff4,false);
            

        }
        else if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20091)) 
        {
            _statusManager.RemoveAllConditionWithSpecialID(8101402);
            
            if(scorchingLevel<0)
                return;
            
            var buff1 = new TimerBuff(HB01_BehaviorTree_Legend2.catastrophicBuff_1);
            buff1.SetEffect(buff1.effect + scorchingLevel * 5);
            var buff2 = new TimerBuff(HB01_BehaviorTree_Legend2.catastrophicBuff_2);
            buff2.SetEffect(buff2.effect + scorchingLevel * 3);
            // var buff3 = new TimerBuff(HB01_BehaviorTree_Legend2.catastrophicBuff_3);
            // buff3.SetEffect(buff3.effect + scorchingLevel * 5);
            var buff4 = new TimerBuff(HB01_BehaviorTree_Legend2.catastrophicBuff_4);
            buff4.SetEffect(buff4.effect + scorchingLevel * 5);
            
            _statusManager.ObtainTimerBuff(buff1);
            _statusManager.ObtainTimerBuff(buff2,false);
            //_statusManager.ObtainTimerBuff(buff3,false);
            _statusManager.ObtainTimerBuff(buff4,false);
            
        }

    }


}
