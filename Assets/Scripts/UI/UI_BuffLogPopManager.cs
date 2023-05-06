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
        catch (Exception e)
        {
            Console.WriteLine(e);
            print("not found");
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
           if (anim.GetCurrentAnimatorStateInfo(0).IsName("idle"))
           {
               txt.text = ConditionStr.Dequeue();
               var type = ConditionStrInfo.Dequeue();
               if (type.Item1 > 400 && type.Item1<500)
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
            sb.Append("<sprite=" + (condition.buffID-1) + ">");
        }

        if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value || condition.DisplayType == BattleCondition.buffEffectDisplayType.ExactValue)
        {
            var formatStr = String.Format
                (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),condition.effect);
            sb.Append(formatStr);
        }
        else if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Level)
        {
            var formatStr = String.Format
                (BasicCalculation.ConditionInfo((BasicCalculation.BattleCondition)condition.buffID, _language),condition.effect);
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
        switch (message)
        {
            case "Reset":
                EnqueueNewCondition("全状态重置");
                ConditionStrInfo.Enqueue(new(1000,0));//Other
                break;
            case "ReliefAllDebuff":
                EnqueueNewCondition("全减益状态解除");
                ConditionStrInfo.Enqueue(new(1001,0));//Other
                break;
            case "SPCharge":
                EnqueueNewCondition("技能充能");
                ConditionStrInfo.Enqueue(new(1002,0));//Other
                break;
                
        }
    }

    private void CustomText_JP(string message)
    {
        switch (message)
        {
            case "Reset":
                EnqueueNewCondition("Empty");
                ConditionStrInfo.Enqueue(new(1000,0));//Other
                break;
            case "ReliefAllDebuff":
                EnqueueNewCondition("全减益状态解除");
                ConditionStrInfo.Enqueue(new(1001,0));//Other
                break;
                
        }
    }
    
    private void CustomText_EN(string message)
    {
        switch (message)
        {
            case "Reset":
                EnqueueNewCondition("Empty");
                ConditionStrInfo.Enqueue(new(1000,0));//Other
                break;
            case "ReliefAllDebuff":
                EnqueueNewCondition("全减益状态解除");
                ConditionStrInfo.Enqueue(new(1001,0));//Other
                break;
                
        }
    }



    void SetActiveFalse()
    {
        txtGameObject.SetActive(false);   
    }

    private void OnDestroy()
    {
        _statusManager.OnBuffEventDelegate -= Condition2Text; //delegate
        _statusManager.OnBuffDispelledEventDelegate -= DispellCondition2Text;
        _statusManager.OnSpecialBuffDelegate -= CustomText;
    }
}
