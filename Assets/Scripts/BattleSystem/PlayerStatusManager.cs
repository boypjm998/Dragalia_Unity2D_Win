using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMechanics;
public class PlayerStatusManager : StatusManager
{
    // Record Character's Status
    

    //Current Status
    //public BasicCalculation.Affliction currentAffliction;
    public int maxSkillNum = 4;

    //SP

    public float[] requiredSP;
    public float[] currentSP;
    public bool[] skillRegenByAttack;
    private float spRegenPerSecond = 200;

    
    public float attackspeed = 1.0f;
    public float movespeed = 7.0f;
    public float jumpforce = 20.0f;
    public float rollspeed = 10.0f;
    
    public int remainReviveTimes = 9;
    public bool debug;

    

    [HideInInspector] public ComboIndicatorUI _comboIndicator;
    private ActorController ac;

    public float skillHasteUp
    {
        get => GetSkillHasteBuff();
    }





    protected override void Awake()
    {
        base.Awake();
        skillRegenByAttack = new bool[4]{ true, true, true, true };
        ac = GetComponent<ActorController>();
        
        if(_conditionBar == null)
            _conditionBar = GameObject.Find("ConditionBar")?.GetComponent<UI_ConditionBar>();
        
        if(maxBaseHP > 9999999)
            maxBaseHP = 9999999;
    }

    public void GetPlayerConditionBar()
    {
        _conditionBar = GameObject.Find("ConditionBar").GetComponent<UI_ConditionBar>();
    }

    public void ClearSP()
    {
        for (int i = 0; i < currentSP.Length; i++)
        {
            currentSP[i] = 0;
        }
    }

    protected override void Start()
    {
        base.Start();


    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        SpGainInStatus(0, spRegenPerSecond * Time.deltaTime,true);
        SpGainInStatus(1, spRegenPerSecond * Time.deltaTime,true);
        SpGainInStatus(2, spRegenPerSecond * Time.deltaTime,true);
        SpGainInStatus(3, spRegenPerSecond * Time.deltaTime,true);

        if (debug)
        {
            debug = false;
            print(conditionList.Count);
        }


    }

    void FixedUpdate()
    {
        
    }

    public void SpGainInStatus(int id, float num,bool autoCharge = false)
    {
        //从0开始
        if(!enabled)
            return;
        if(skillRegenByAttack[id] || autoCharge)
            currentSP[id] += num;

        if (currentSP[id] > requiredSP[id])
        {
            currentSP[id] = requiredSP[id];
        }
    }

    

    protected override IEnumerator ComboCheck()
    {
        if (comboHitCount > 0)
        {
            lastComboRemainTime = comboConnectMaxInterval;
            if (calroutine == null)
            {
                calroutine = StartCoroutine(CalculateRemain(comboConnectMaxInterval));
            }
            else
            {
                StopCoroutine(calroutine);
                calroutine = null;
                calroutine = StartCoroutine(CalculateRemain(comboConnectMaxInterval));
            }
            
            _comboIndicator?.PrintComboNum();
            yield return new WaitForSeconds(comboConnectMaxInterval);
            comboHitCount = 0;
            
            StopCoroutine(calroutine);
            calroutine = null;
            
            lastComboRemainTime = 0;
            _comboIndicator?.HideComboNum();
        }
    }

    public override void ComboConnect()
    {
        comboHitCount++;
        
        lastComboRemainTime = comboConnectMaxInterval;
        if(comboRoutine!=null)
            StopCoroutine(comboRoutine);
        comboRoutine = StartCoroutine(ComboCheck());
    }

    public void SetRequiredSP(int sidFromZero, float sp)
    {
        requiredSP[sidFromZero] = sp;
    }

    public void ChargeSP(int skillID, float sp)
    {
        currentSP[skillID] += sp;
    }




    #region Buff Related

    protected float GetSkillHasteBuff(int type = 0)
    {
        float buff = 0, debuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.SkillHasteBuff)
            {
                buff += (int)condition.effect;
            }
            if (condition.buffID == (int)BasicCalculation.BattleCondition.SkillHasteDebuff)
            {
                debuff += (int)condition.effect;
            }

        }

        if (buff > BasicCalculation.BattleConditionLimit(9))
        {
            buff = BasicCalculation.BattleConditionLimit(9);
        }

        if (debuff > BasicCalculation.BattleConditionLimit(209))
        {
            debuff = BasicCalculation.BattleConditionLimit(209);
        }
        
        buff *= 0.01f;
        debuff *= 0.01f;

        if (type == 1)
        {
            return buff;
        }
        else if (type == 2)
        {
            return debuff;
        }


        return buff - debuff;
        
    }



    #endregion

    
    
    
    
    



}
