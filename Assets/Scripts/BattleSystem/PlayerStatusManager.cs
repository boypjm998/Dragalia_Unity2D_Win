using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using GameMechanics;

public class PlayerStatusManager : StatusManager
{
    // 龙化相关
    public float DModeGauge { get; private set; } = 0;
    private float customedDModeGauge = 50;

    public float MaxDModeGauge
    {
        //如果ac.dc为空，返回100，否则返回ac.dc.maxDModeGauge
        get => ac.dc ? ac.dc.MaxDModeGauge : 100;
    }
    
    public float ReqDModeGauge
    {
        //如果ac.dc为空，返回100，否则返回ac.dc.maxDModeGauge
        get => ac.dc ? ac.dc.ReqDModeGauge : customedDModeGauge;
    }


    //public float specialDModeGauge = 0;
    public bool isShapeshifting => ac.DModeIsOn;
    public float shapeshiftingCD = 10;
    public float shapeshiftingCDTimer = 0;
    
    
    
    /// <summary>
    /// 标准模式下的碰撞体属性(碰撞体offsetX, 碰撞体offsetY, 碰撞体宽度, 碰撞体高度)
    /// </summary>
    public readonly static Vector4 NormalHitSensorProperty = new Vector4(0, -0.3f, 1, 2);

    
    



    //Current Status
    //public BasicCalculation.Affliction currentAffliction;
    public int maxSkillNum = 4;

    //SP

    public float[] requiredSP;
    public float[] currentSP;
    public bool[] skillRegenByAttack = new bool[4] { true, true, true, true };
    private float spRegenPerSecond = 200;

    
    //public float attackspeed = 1.0f;
    public float movespeed = 7.0f;
    public float jumpforce = 20.0f;
    public float rollspeed = 10.0f;
    
    public int remainReviveTimes = 9;
    public bool debug;


    public event StatusManagerVoidDelegate OnShapeshiftingEnter;
    public event StatusManagerVoidDelegate OnShapeshiftingExit;

    [HideInInspector] public ComboIndicatorUI _comboIndicator;
    private ActorController ac;

    public float skillHasteUp
    {
        get => GetSkillHasteBuff();
    }





    protected override void Awake()
    {
        base.Awake();
        //skillRegenByAttack = new bool[4]{ true, true, true, true };
        ac = GetComponent<ActorController>();
        
        if(_conditionBar == null)
            _conditionBar = GameObject.Find("ConditionBar")?.GetComponent<UI_ConditionBar>();
        
        if(maxBaseHP > 9999999)
            maxBaseHP = 9999999;

        shapeshiftingCDTimer = 0;
        OnShapeshiftingEnter += ResizeCameraSizeToShapeshiftingMode;
        OnShapeshiftingExit += ResizeCameraSizeToNormalMode;
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

    public bool CheckSkillSPEnough(int id)
    {
        if (isShapeshifting)
        {
            if (ac.dc == null || ac.dc.requiredDSP.Length <= id)
                return false;
            return ac.dc.currentDSP[id] >= ac.dc.requiredDSP[id];
        }
        else
        {
            return currentSP[id] >= requiredSP[id];
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
        
        if(ac.DModeIsOn)
            return;

        SpGainInStatus(0, spRegenPerSecond * Time.deltaTime,true);
        SpGainInStatus(1, spRegenPerSecond * Time.deltaTime,true);
        SpGainInStatus(2, spRegenPerSecond * Time.deltaTime,true);
        SpGainInStatus(3, spRegenPerSecond * Time.deltaTime,true);
        ShapeShiftingCDTick();

        if (debug)
        {
            debug = false;
            print(conditionList.Count);
        }


    }

    private void ShapeShiftingCDTick()
    {
        if (shapeshiftingCDTimer > 0)
        {
            shapeshiftingCDTimer -= Time.deltaTime;
        }
    }

    public void SetReqDModeGauge(float amount)
    {
        customedDModeGauge = amount;
    }

    private void OnDestroy()
    {
        OnShapeshiftingEnter -= ResizeCameraSizeToShapeshiftingMode;
        OnShapeshiftingExit -= ResizeCameraSizeToNormalMode;
    }

    public void SpGainInStatus(int id, float num,bool autoCharge = false)
    {
        //从0开始
        if(!enabled)
            return;

        if (isShapeshifting)
        {
            SpGainWhenShapeShifting(id,num);
            return;
        }


        if (skillRegenByAttack[id] || autoCharge)
        {
            //Debug.Log($"SKL{id+1} Charged, autoCharge:{autoCharge}");
            currentSP[id] += num;
        }

        

        if (currentSP[id] > requiredSP[id])
        {
            currentSP[id] = requiredSP[id];
        }

    }


    public void SpGainWhenShapeShifting(int id, float num)
    {
        if (ac.dc == null)
            return;

        if (id >= ac.dc.currentDSP.Length)
            return;
        
        ac.dc.currentDSP[id] += num;
        if (ac.dc.currentDSP[id] > ac.dc.requiredDSP[id])
        {
            ac.dc.currentDSP[id] = ac.dc.requiredDSP[id];
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
        OnComboConnect?.Invoke();
        
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
    
    public void FillSP(int skillID, int percent)
    {
        currentSP[skillID] += requiredSP[skillID] * percent * 0.01f;
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

    public void InvokeShapeshiftingEnter()
    {
        OnShapeshiftingEnter?.Invoke();
    }
    
    public void InvokeShapeshiftingExit()
    {
        OnShapeshiftingExit?.Invoke();
    }

    public void InvokeShapeshiftingPurged()
    {
        OnShapeshiftingExit?.Invoke();
        
        OnSpecialBuffDelegate?.Invoke(
            UI_BuffLogPopManager.SpecialConditionType.DModePurged.ToString());
    }

    public void ChargeDP(float quantity, bool abilityCharge = true)
    {
        if(!ac.canTransform)
            return;
        
        if(ac.BlockDPCharge(abilityCharge))
            return;
        
        DModeGauge += quantity * MaxDModeGauge / 100;
        
        // if(!ac.dc)
        //     return;
        

        //MaxDModeGauge <- ac.dc.maxDModeGauge
        if(DModeGauge > MaxDModeGauge)
            DModeGauge = MaxDModeGauge;
        
        print("获得DP"+quantity * MaxDModeGauge / 100+"点,max:"+MaxDModeGauge);
        //todo: 需要新增一个事件，当dp充能时Invoke
    }
    
    public void DepleteDP(float trueQuantity)
    {
        if(!ac.canTransform)
            return;

        DModeGauge -= trueQuantity;

        if (DModeGauge < 0)
            DModeGauge = 0;
        
    }

    protected void ResizeCameraSizeToShapeshiftingMode()
    {
        StageCameraController.Instance.ToShapeshiftingView();
    }
    
    protected void ResizeCameraSizeToNormalMode()
    {
        StageCameraController.Instance.ToNormalView();
    }





}
