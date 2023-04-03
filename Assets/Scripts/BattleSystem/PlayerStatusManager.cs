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
    public float movespeed = 6.0f;
    public float jumpforce = 20.0f;
    public float rollspeed = 9.0f;
    public float comboConnectMaxInterval = 3.0f;
    public int remainReviveTimes = 9;

    private Coroutine calroutine; //Calculate Left Combo time
    
    
    
    
    


    public int comboHitCount = 0;
    public float lastComboRemainTime { get; private set; } = 0;
    private Coroutine comboRoutine = null;

    [HideInInspector] public ComboIndicatorUI _comboIndicator;
    private ActorController ac;

    //[SerializeField]
    //private UI_ConditionBar _conditionBar;


    protected override void Awake()
    {
        base.Awake();
        skillRegenByAttack = new bool[4]{ true, true, true, true };
        ac = GetComponent<ActorController>();
        
        if(_conditionBar == null)
            _conditionBar = GameObject.Find("ConditionBar").GetComponent<UI_ConditionBar>();
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
        SpGainInStatus(0, spRegenPerSecond * Time.deltaTime);
        SpGainInStatus(1, spRegenPerSecond * Time.deltaTime);
        SpGainInStatus(2, spRegenPerSecond * Time.deltaTime);
        SpGainInStatus(3, spRegenPerSecond * Time.deltaTime);

        

    }

    void FixedUpdate()
    {
        
    }

    public void SpGainInStatus(int id, float num)
    {
        //从0开始
        if(skillRegenByAttack[id])
            currentSP[id] += num;

        if (currentSP[id] > requiredSP[id])
        {
            currentSP[id] = requiredSP[id];
        }
    }

    

    private IEnumerator ComboCheck()
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
            
            _comboIndicator.PrintComboNum();
            yield return new WaitForSeconds(comboConnectMaxInterval);
            comboHitCount = 0;
            
            StopCoroutine(calroutine);
            calroutine = null;
            
            lastComboRemainTime = 0;
            _comboIndicator.HideComboNum();
        }
    }

    public void ComboConnect()
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

    private IEnumerator CalculateRemain(float time)
    {
        //calculate remmain combo last time.
        lastComboRemainTime = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            lastComboRemainTime = time;
            yield return null;
        }
    }


    #region Buff Related

    
    
    
        
    
        /// <summary>
        ///   <para>目标获得多层同类BUFF</para>
        /// </summary>
        
        
        public override void ObtainUnstackableTimerBuff(int buffID, float effect, float duration,
            BattleCondition.buffEffectDisplayType type, int spID)
        {
            OverrideUnstackableBuff(buffID, spID,effect,duration);
        
            var buff = new TimerBuff(buffID, effect, duration, type, 1);
            
            if (IsDotAffliction(buffID) && GetConditionsOfType(buffID).Count==0)
            {
                if (dotRoutineDict.ContainsKey(buffID))
                {
                    Coroutine oldRoutine;
                    dotRoutineDict.Remove(buffID,out oldRoutine);
                    StopCoroutine(oldRoutine);
                }
                var newRoutine = StartCoroutine(DotTick((BasicCalculation.BattleCondition)buffID));
                dotRoutineDict.Add(buffID,newRoutine);
            }
            else
            {
                _battleEffectManager.SpawnEffect(gameObject,(BasicCalculation.BattleCondition)buffID);
            }
        
            buff.SetUniqueBuffInfo(spID);
        
            conditionList.Add(buff);
            
            _conditionBar.OnConditionAdd(buff);
        
            OnBuffEventDelegate?.Invoke(buff);
        }
        
        /// <summary>
        ///   <para>目标移除一个Condition</para>
        /// </summary>
        public override void RemoveCondition(BattleCondition buff)
        {
            
            conditionList.Remove(buff);
            
            _conditionBar.OnConditionRemove(buff.buffID);
        }

    #endregion

    
    
    
    
    



}
