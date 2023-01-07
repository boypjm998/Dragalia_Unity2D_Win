using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//Put in Charater/Enemy's child.
public class UI_BuffLogPopManager : MonoBehaviour
{
    //Font size = 5
    private TextMeshPro txt;
    private GameObject txtGameObject;
    private Animator anim;
    private StatusManager _statusManager;

    private int currentCnt = 0;
    public Queue<string> ConditionStr;
    public Queue<int> ConditionStrInfo;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        transform.rotation = Quaternion.identity;
        txtGameObject = transform.GetChild(0).gameObject;
        txt = txtGameObject.GetComponent<TextMeshPro>();
        anim = txtGameObject.GetComponent<Animator>();
        txtGameObject.SetActive(false);
        _statusManager = GetComponentInParent<StatusManager>();
        currentCnt = _statusManager.conditionList.Count;
        _statusManager.OnBuffEventDelegate += Condition2Text; //delegate
        _statusManager.OnBuffDispelledEventDelegate += DispellCondition2Text;
        //_statusManager.OnBuffRemovedEventDelegate += DispellCondition2Text;
        ConditionStr = new Queue<string>();
        ConditionStrInfo = new Queue<int>();
    }

    // Update is called once per frame
    void Update()
    {
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
               if (type > 400 && type<500)
               {
                   txt.fontSize = 8;
                   var num = _statusManager.GetConditionStackNumber(type);
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
        

        sb.Append(BasicCalculation.ConditionInfo_zh((BasicCalculation.BattleCondition)condition.buffID));
        if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value)
        {
            sb.Append(condition.effect);
            sb.Append("%");
        }
        //print(sb.ToString());
        EnqueueNewCondition(sb.ToString());
        ConditionStrInfo.Enqueue(condition.buffID);

    }

    private void DispellCondition2Text(BattleCondition condition)
    {
        StringBuilder sb = new StringBuilder();
        
        sb.Append(BasicCalculation.ConditionInfo_zh((BasicCalculation.BattleCondition)condition.buffID));
        if (condition.DisplayType == BattleCondition.buffEffectDisplayType.Value)
        {
            sb.Append(condition.effect);
            sb.Append("%");
        }

        sb.Append(" 解除");
        
        EnqueueNewCondition(sb.ToString());
        ConditionStrInfo.Enqueue(999);//驱散都是999

    }

    void SetActiveFalse()
    {
        txtGameObject.SetActive(false);   
    }


}
