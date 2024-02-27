using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using GameMechanics;
//Put in Charater/Enemy's child.
public class UI_BuffLogPopManager : MonoBehaviour
{
    //Font size = 5
    private TextMeshPro txt;
    private GameObject txtGameObject;
    private Animator anim;
    private StatusManager _statusManager;
    private Transform _transform;

    private int currentCnt = 0;
    public Queue<string> ConditionStr;
    public Queue<(int,int)> ConditionStrInfo;
    private GlobalController.Language _language;
    
    public enum SpecialConditionType
    {
        Reset,
        ReliefAllDebuff,
        ReliefAllAffliction,
        SPCharge,
        CounterReady,
        DModePurged,
        Energized,
        Inspired,
        SkillUpgradedReset,
        ReduceSigilTime,
        ImmuneToControlAffliction,
        ImmuneToDoTAffliction,
        AutoChargeRateUp,
        DragondriveCharge,
        DragondrivePurged,
        BuffCount
    }

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            _language = FindObjectOfType<GlobalController>().GameLanguage;
        }
        catch
        {
            
            _language = GlobalController.Language.ZHCN;
        }
        
        transform.rotation = Quaternion.identity;
        txtGameObject = transform.GetChild(0).gameObject;
        txt = txtGameObject.GetComponent<TextMeshPro>();
        anim = txtGameObject.GetComponent<Animator>();
       
        txtGameObject.SetActive(false);
        _statusManager = GetComponentInParent<StatusManager>();
        currentCnt = _statusManager.conditionList.Count;
        
         _transform = _statusManager.transform;
        
        _statusManager.OnBuffEventDelegate += Condition2Text; //delegate
        _statusManager.OnBuffDispelledEventDelegate += DispellCondition2Text;
        _statusManager.OnSpecialBuffDelegate += CustomText;
            
        ConditionStr = new Queue<string>();
        ConditionStrInfo = new();
    }

    // Update is called once per frame
    void Update()
    {
        if(_transform.localScale.x == -1)
            transform.localScale = new Vector3(-1,1,1);
        else
        {
            transform.localScale = new Vector3(1,1,1);
        }

        transform.rotation = Quaternion.identity;

       if (ConditionStr.Count == 0 && txtGameObject.activeSelf)
       {
           if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
           { 
               txtGameObject.SetActive(false);
               
           }
       }
       else if(ConditionStr.Count > 0)
       {
           txtGameObject.SetActive(true);
           
           if(ConditionStr.Count>5)
               anim.speed = 1.5f;
           else
               anim.speed = 1;
           
           
           
           if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
           {
               txt.text = ConditionStr.Dequeue();
               var type = ConditionStrInfo.Dequeue();
               
               if (StatusManager.IsAffliction(type.Item1))
               {
                   txt.fontSize = 7;
                   var num = (type.Item2);
                   //var num = _statusManager.GetConditionStackNumber(type);
                   if (num > 1)
                   {
                       txt.text += ("×" + num);
                   }
               }
               else txt.fontSize = 5;

               anim.Play("pop");
               
           }
       }


    }

    public void EnqueueNewCondition(string str)
    {
        ConditionStr.Enqueue(str);
    }

    private void Condition2Text(BattleCondition condition)
    {
        StringBuilder sb = new StringBuilder();
        if (condition.GetIcon() != null)
        {
            var buffID = condition.buffID - 1;
            if (buffID >= 500)
            {
                buffID = (condition as TimerBuff).extra_iconID - 1;
            }

            sb.Append("<sprite=" + buffID + ">");
        }

        if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value ||
            condition.DisplayType == BattleCondition.buffEffectDisplayType.ExactValue ||
            condition.DisplayType == BattleCondition.buffEffectDisplayType.EnergyOrInspiration)
        {
            var formatStr = String.Format
                (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),condition.effect);
            sb.Append(formatStr);
        }
        else if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Level)
        {
            var formatStr = String.Format
                (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),$"Lv.{condition.effect}");
            sb.Append(formatStr);
        }
        else
        {
            sb.Append(BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID,_language));
        }
        
        
        //if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value)
        //{
        //    String.Format
        //    (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),
        //        condition.effect);
        //    sb.Append(condition.effect);
        //    sb.Append("%");
        //}
        //print(sb.ToString());
        EnqueueNewCondition(sb.ToString());
        ConditionStrInfo.Enqueue(new (condition.buffID,
            _statusManager.GetConditionStackNumber(condition.buffID)));

    }

    private void DispellCondition2Text(BattleCondition condition)
    {
        StringBuilder sb = new StringBuilder();
        
        
        
        if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value)
        {
            var formatStr = String.Format
                (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),condition.effect);
            sb.Append(formatStr);
        }
        else if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Level)
        {
            var formatStr = String.Format
                (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),String.Empty);
            sb.Append(formatStr);
        }else if (condition.DisplayType == BattleCondition.buffEffectDisplayType.EnergyOrInspiration)
        {
            if (condition.effect < 5)
            {
                var formatStr = String.Format
                    (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),condition.effect);
                sb.Append(formatStr);
            }
            else
            {
                if (condition.buffID == (int)BasicCalculation.BattleCondition.Energy)
                {
                    if (_language == GlobalController.Language.ZHCN)
                    {
                        sb.Append("超强斗志");
                    }
                    else if(_language == GlobalController.Language.EN)
                        sb.Append("Energized");
                }else if (condition.buffID == (int)BasicCalculation.BattleCondition.Inspiration)
                {
                    if (_language == GlobalController.Language.ZHCN)
                    {
                        sb.Append("超强灵感");
                    }
                    else if(_language == GlobalController.Language.EN)
                        sb.Append("Inspired");
                }


            }
        }
        else
        {
            sb.Append(BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID,_language));
        }
        
        //sb.Append(BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID,_language));
        //if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value)
        //{
        //    sb.Append(condition.effect);
        //    sb.Append("%");
        //}

        if (_language != GlobalController.Language.EN)
        {
            sb.Append(" 解除");
        }
        else sb.Append(" Purged");

        
        
        EnqueueNewCondition(sb.ToString());
        ConditionStrInfo.Enqueue(new(999,0));//驱散都是999
        //ConditionStrInfo.Enqueue(999);//驱散都是999

    }

    /// <summary>
    /// Reset
    /// </summary>
    /// <param name="message"></param>
    private void CustomText(string message)
    {
        switch (_language)
        {
            case GlobalController.Language.ZHCN:
                CustomText_ZH(message);
                break;
            case GlobalController.Language.JP:
                CustomText_JP(message);
                break;
            default:
                CustomText_EN(message);
                break;
                
        }
    }
    private void CustomText_ZH(string message)
    {
        //将message解析为枚举SpecialConditionType的类型,存在msgType

        string extraMsg = "";

        if (message.Contains("_"))
        {
            extraMsg = message.Split("_")[^1];
            message = message.Split("_")[0];
        }
        
        
        SpecialConditionType msgType = (SpecialConditionType)Enum.Parse(typeof(SpecialConditionType), message);

        switch (msgType)
        {
            case SpecialConditionType.Reset:
                EnqueueNewCondition("全状态重置");
                ConditionStrInfo.Enqueue(new(1000,0));//Other
                break;
            case SpecialConditionType.ReliefAllDebuff:
                EnqueueNewCondition("全减益状态解除");
                ConditionStrInfo.Enqueue(new(1001,0));//Other
                break;
            case SpecialConditionType.ReliefAllAffliction:
                EnqueueNewCondition("全异常状态解除");
                ConditionStrInfo.Enqueue(new(1002,0));//Other
                break;
            case SpecialConditionType.SPCharge:
                EnqueueNewCondition("技能充能");
                ConditionStrInfo.Enqueue(new(1003,0));//Other
                break;
            case SpecialConditionType.CounterReady:
                EnqueueNewCondition("反击待命");
                ConditionStrInfo.Enqueue(new(1004,0));//Other
                break;
            case SpecialConditionType.DModePurged:
                EnqueueNewCondition("龙化解除");
                ConditionStrInfo.Enqueue(new(1005,0));//Other
                break;
            case SpecialConditionType.Energized:
                EnqueueNewCondition("超强斗志发动");
                ConditionStrInfo.Enqueue(new(1007,0));//Other
                break;
            case SpecialConditionType.Inspired:
                EnqueueNewCondition("超强灵感发动");
                ConditionStrInfo.Enqueue(new(1008,0));//Other
                break;
            case SpecialConditionType.SkillUpgradedReset:
                EnqueueNewCondition("技能重置");
                ConditionStrInfo.Enqueue(new(1009,0));//Other
                break;
            case SpecialConditionType.ReduceSigilTime:
                EnqueueNewCondition("圣痕枷锁剩余时间减少");
                ConditionStrInfo.Enqueue(new(1010,0));//Other
                break;
            case SpecialConditionType.ImmuneToControlAffliction:
                EnqueueNewCondition("免疫控制类异常状态");
                ConditionStrInfo.Enqueue(new(1011,0));//Other
                break;
            case SpecialConditionType.ImmuneToDoTAffliction:
                EnqueueNewCondition("免疫持续伤害类异常状态");
                ConditionStrInfo.Enqueue(new(1011,0));//Other
                break;
            case SpecialConditionType.AutoChargeRateUp:
                EnqueueNewCondition("技能自动充能率提升");
                ConditionStrInfo.Enqueue(new(1012,0));//Other
                break;
            case SpecialConditionType.DragondriveCharge:
                EnqueueNewCondition("强袭充能");
                ConditionStrInfo.Enqueue(new(1013,0));
                break;
            case SpecialConditionType.DragondrivePurged:
                EnqueueNewCondition("强袭解除");
                ConditionStrInfo.Enqueue(new(1013,0));
                break;
            
            case SpecialConditionType.BuffCount:
                EnqueueNewCondition($"增强效果×{extraMsg}");
                ConditionStrInfo.Enqueue(new(1014,0));
                break;
            
            default:
                EnqueueNewCondition("未知状态");
                ConditionStrInfo.Enqueue(new(1006,0));//Other
                break;
                
        }
    }

    private void CustomText_JP(string message)
    {
        switch (message)
        {
            case "Reset":
                EnqueueNewCondition("Reset All Status");
                ConditionStrInfo.Enqueue(new(1000,0));//Other
                break;
            case "ReliefAllDebuff":
                EnqueueNewCondition("Recover from Debuffs");
                ConditionStrInfo.Enqueue(new(1001,0));//Other
                break;
            case "ReliefAllAffliction":
                EnqueueNewCondition("Recover from Afflictions");
                ConditionStrInfo.Enqueue(new(1002,0));//Other
                break;
            case "SPCharge":
                EnqueueNewCondition("Skill Prep");
                ConditionStrInfo.Enqueue(new(1003,0));//Other
                break;
            case "CounterReady":
                EnqueueNewCondition("Counter Ready");
                ConditionStrInfo.Enqueue(new(1004,0));//Other
                break;
            default:
                break;
        }
    }
    
    private void CustomText_EN(string message)
    {
        string extraMsg = "";

        if (message.Contains("_"))
        {
            extraMsg = message.Split("_")[^1];
            message = message.Split("_")[0];
        }
        
        var msgType = (SpecialConditionType)Enum.Parse(typeof(SpecialConditionType), message);
        switch (msgType)
        {
            case SpecialConditionType.Reset:
                EnqueueNewCondition("Reset All Status");
                ConditionStrInfo.Enqueue(new(1000,0));//Other
                break;
            case SpecialConditionType.ReliefAllDebuff:
                EnqueueNewCondition("Recover from Debuffs");
                ConditionStrInfo.Enqueue(new(1001,0));//Other
                break;
            case SpecialConditionType.ReliefAllAffliction:
                EnqueueNewCondition("Recover from Afflictions");
                ConditionStrInfo.Enqueue(new(1002,0));//Other
                break;
            case SpecialConditionType.SPCharge:
                EnqueueNewCondition("Skill Prep");
                ConditionStrInfo.Enqueue(new(1003,0));//Other
                break;
            case SpecialConditionType.CounterReady:
                EnqueueNewCondition("Counter Stance");
                ConditionStrInfo.Enqueue(new(1004,0));//Other
                break;
            case SpecialConditionType.DModePurged:
                EnqueueNewCondition("Shapeshifting Purged");
                ConditionStrInfo.Enqueue(new(1005,0));//Other
                break;
            case SpecialConditionType.Energized:
                EnqueueNewCondition("Energized");
                ConditionStrInfo.Enqueue(new(1007,0));//Other
                break;
            case SpecialConditionType.Inspired:
                EnqueueNewCondition("Inspired");
                ConditionStrInfo.Enqueue(new(1008,0));//Other
                break;
            case SpecialConditionType.SkillUpgradedReset:
                EnqueueNewCondition("Skill Reset");
                ConditionStrInfo.Enqueue(new(1009,0));//Other
                break;
            case SpecialConditionType.ReduceSigilTime:
                EnqueueNewCondition("Unlocking Sigil...");
                ConditionStrInfo.Enqueue(new(1010,0));//Other
                break;
            case SpecialConditionType.ImmuneToControlAffliction:
                EnqueueNewCondition("Control Affliction Immunity");
                ConditionStrInfo.Enqueue(new(1011,0));//Other
                break;
            case SpecialConditionType.ImmuneToDoTAffliction:
                EnqueueNewCondition("DoT Affliction Immunity");
                ConditionStrInfo.Enqueue(new(1011,0));//Other
                break;
            case SpecialConditionType.AutoChargeRateUp:
                EnqueueNewCondition("SP Regen Rate Up");
                ConditionStrInfo.Enqueue(new(1012,0));//Other
                break;
            case SpecialConditionType.DragondriveCharge:
                EnqueueNewCondition("Dragondrive Prep");
                ConditionStrInfo.Enqueue(new(1013,0));
                break;
            case SpecialConditionType.DragondrivePurged:
                EnqueueNewCondition("Dragondrive Purged");
                ConditionStrInfo.Enqueue(new(1013,0));
                break;
            case SpecialConditionType.BuffCount:
                EnqueueNewCondition($"Skill Boost × {extraMsg}");
                ConditionStrInfo.Enqueue(new(1014,0));
                break;
            
            
            default:
                EnqueueNewCondition("Unknown Status");
                ConditionStrInfo.Enqueue(new(1006,0));//Other
                break;
                
        }
    }



    void SetActiveFalse()
    {
        txtGameObject.SetActive(false);   
    }

    private void OnDestroy()
    {
        try
        {
            _statusManager.OnBuffEventDelegate -= Condition2Text; //delegate
            _statusManager.OnBuffDispelledEventDelegate -= DispellCondition2Text;
            _statusManager.OnSpecialBuffDelegate -= CustomText;
        }catch{}

        
    }
}
