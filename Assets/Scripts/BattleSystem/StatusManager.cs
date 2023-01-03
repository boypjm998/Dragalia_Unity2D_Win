using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    public string displayedName;
    
    //Base properties
    public int maxBaseHP;
    public int baseDef;
    public int baseAtk;
    public int critRate;
    public int maxHP;
    public int currentHp;

    public int knockbackRes = 0;
    
    
    

    protected BattleStageManager _battleStageManager;
    protected BattleEffectManager _battleEffectManager;


    public Coroutine healRoutine = null;
    public Dictionary<int, Coroutine> dotRoutineDict;

    public delegate void TestDelegate(BattleCondition condition);
    public TestDelegate OnBuffEventDelegate;
    public TestDelegate OnBuffDispelledEventDelegate;
    
    [SerializeField]
    protected UI_ConditionBar _conditionBar;

    #region Buff Getter

    public float attackBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetAttackBuff();
            else return 0;
        }
    }
    
    public float defenseBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetDefenseBuff();
            else return 0;
        }
    }
    
    public float critRateBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetCritRateBuff();
            else return 0;
        }
    }
    
    public float critDmgBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetCritDamageBuff();
            else return 0;
        }
    }
    
    public float skillDmgBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetSkillDamageBuff();
            else return 0;
        }
    }

    public float recoveryPotencyBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetRecoveryBuff();
            else return 0;
        }
    }

    #endregion
    
    

    private int breakPunisherBuff = 0;
    private int punisherBuff = 0;

    private int knockbackPowerBuff;

    //Offensive Debuff


    //Defensive Buff
    
    [SerializeField] protected int damageCut = 0;
    [SerializeField] protected int damageCutConst = 0;
    protected int lifeShield;
    protected bool knockbackImmune = false;
    
    protected int healBuffNum = 0;

    public List<BattleCondition> conditionList;

    
    
    

    protected virtual void Awake()
    {
        conditionList = new List<BattleCondition>();
        dotRoutineDict = new Dictionary<int, Coroutine>();
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        _battleStageManager = FindObjectOfType<BattleStageManager>();
        _battleEffectManager = FindObjectOfType<BattleEffectManager>();
        maxHP = maxBaseHP;
        currentHp = maxBaseHP;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        ConditionTick();
        StatusLimitCheck();
        
    }

    private void StatusLimitCheck()
    {
        if (currentHp > maxHP)
        {
            currentHp = maxHP;
        }

        if (currentHp < 0)
        {
            currentHp = 0;
        }
        
    }

    public float GetAttackBuff()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 1)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 201)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialAttackBuff(condition.buffID);
            }
        }

        if (totalbuff > 200)
        {
            return 2;
        }
        else if (totalbuff < -50)
        {
            return -0.5f;
        }
        else
        {
            return 0.01f * (float)totalbuff;
        }
    }

    public float GetDefenseBuff()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 2)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 202)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialDefenseBuff(condition.buffID);
            }
        }
        
        if (totalbuff > 200)
        {
            return 2;
        }
        else if (totalbuff < -50)
        {
            return -0.5f;
        }
        else
        {
            return 0.01f * totalbuff;
        }
    }

    public int GetCritRateBuff()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 3)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 203)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialCritBuff(condition.buffID);
            }
        }
        return totalbuff > 200 ? 200 : (int)totalbuff;
    }
    
    public float GetCritDamageBuff()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 4)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 204)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialCritDamageBuff(condition.buffID);
            }
        }
        
        if (totalbuff > 500)
        {
            return 5;
        }
        else if (totalbuff < -120)
        {
            return -1.2f;
        }
        else
        {
            return 0.01f * (float)totalbuff;
        }
    }

    public float GetSkillDamageBuff()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 8)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 208)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialSkillDamageBuff(condition.buffID);
            }
        }
        
        if (totalbuff > 200)
        {
            return 2;
        }
        else if (totalbuff < -50)
        {
            return -0.5f;
        }
        else
        {
            return 0.01f * (float)totalbuff;
        }
    }

    //按照系数的回血计算
    public float GetRecoveryPotency()
    {
        float totalbuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 5)
            {
                if(condition.effect > 0)
                    totalbuff += condition.effect;
            }
            
            else
            {
                totalbuff += GetSpecialRecoveryPotency(condition.buffID);
            }
        }

        return totalbuff;
    }

    //按照百分比的回血计算
    public float GetRecoveryPotencyPercentage()
    {
        float totalbuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 5)
            {
                if(condition.effect < 0)
                    totalbuff -= condition.effect;
            }
            
        }

        return totalbuff;
    }
   // 治疗效果增加
    protected float GetRecoveryBuff()
    {
        //治疗效果UP.
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 6)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 206)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialSkillDamageBuff(condition.buffID);
            }
        }
        
        if (totalbuff > 200)
        {
            return 2;
        }
        else if (totalbuff < -100)
        {
            return -1f;
        }
        else
        {
            return 0.01f * (float)totalbuff;
        }
        
    }

    /// <summary>
    /// 获取目标减伤百分比(减伤BUFF-破甲BUFF)
    /// </summary>
    /// <returns></returns>
    public float GetDamageCut()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 10)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 210)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialDamageCut(condition.buffID);
            }
        }

        if (totalbuff > 100)
        {
            return 1;
        }
        else if (totalbuff < -50)
        {
            return -0.5f;
        }
        else
        {
            return 0.01f * (float)totalbuff;
        }
        //return damageCut > 100 ? 1 : (0.01f * (float)damageCut);
    }

    public float GetDamageCutConst()
    {
        return damageCutConst;
    }

    private float GetSpecialAttackBuff(int buffID)
    {
        switch (buffID)
        {
            case 102:
                return 50;
            default:
                return 0;
        }
    }
    private float GetSpecialCritBuff(int conditionBuffID)
    {
        return 0;
    }
    private float GetSpecialDefenseBuff(int conditionBuffID)
    {
        return 0;
    }
    private float GetSpecialSkillDamageBuff(int conditionBuffID)
    {
        return 0;
    }
    private float GetSpecialCritDamageBuff(int conditionBuffID)
    {
        return 0;
    }

    private float GetSpecialRecoveryPotency(int conditionBuffID)
    {
        return 0;
    }
    
    private float GetSpecialRecoveryBuff(int conditionBuffID)
    {
        return 0;
    }
    
    private float GetSpecialDamageCut(int conditionBuffID)
    {
        return 0;
    }

    public static bool IsDotAffliction(int buffID)
    {
        if (buffID > 400 && buffID < 410)
            return true;
        return false;
    }

    ///Get a new Timerbuff
    public virtual void ObtainTimerBuff(int buffID, float effect, float duration,
        BattleCondition.buffEffectDisplayType type, int maxStack)
    {
        RemoveBuffIfReachedLimit(buffID, maxStack);
        var buff = new TimerBuff(buffID, effect, duration, type, maxStack);
        
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

        conditionList.Add(buff);
        
        _conditionBar?.OnConditionAdd(buff);
        
        OnBuffEventDelegate?.Invoke(buff);
        
        

        
    }

    public virtual void ObtainTimerBuff(int buffID, float duration, BattleCondition.buffEffectDisplayType type,
        int maxStack)
    {
        

        var buff = new TimerBuff(buffID, 0, duration, type, maxStack);
        
        RemoveBuffIfReachedLimit(buffID, maxStack);

        conditionList.Add(buff);
        
        OnBuffEventDelegate?.Invoke(buff);
        
        _conditionBar?.OnConditionAdd(buff);

    }

    public virtual void ObtainTimerBuff(int buffID,float effect, float duration, BattleCondition.buffEffectDisplayType type)
    {

        var buff = new TimerBuff(buffID, effect, duration, type, BasicCalculation.MAXCONDITIONSTACKNUMBER);

        RemoveBuffIfReachedLimit(buffID);
        
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
        
        conditionList.Add(buff);

        _conditionBar?.OnConditionAdd(buff);
        
        OnBuffEventDelegate?.Invoke(buff);
        
        
 
    }
    
    /// <summary>
    ///   <para>给自身附加多层同类BUFF.</para>
    /// </summary>
    public virtual void ObtainTimerBuffs(int buffID, float duration, BattleCondition.buffEffectDisplayType type,
        int stackNum, int maxStack)
    {
        

        var buff = new TimerBuff(buffID, 0, duration, type, maxStack);
        
        _battleEffectManager.SpawnEffect(gameObject,(BasicCalculation.BattleCondition)buffID);

        for (int i = 0; i < stackNum; i++)
        {
            conditionList.Add(buff);
        }
        
        RemoveBuffIfReachedLimit(buffID, maxStack);
        
        OnBuffEventDelegate?.Invoke(buff);
        
        _conditionBar?.OnConditionAdd(buff);
        
        
  
    }

    // Get a new Unstackable Timerbuff
    public virtual void ObtainUnstackableTimerBuff(int buffID, float effect, float duration,
        BattleCondition.buffEffectDisplayType type, int spID)
    {
        OverrideUnstackableBuff(buffID, spID);
        
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
        
        _conditionBar?.OnConditionAdd(buff);
        
        OnBuffEventDelegate?.Invoke(buff);
    }

    public virtual void DispellTimerBuff()
    {
        for (int i = conditionList.Count - 1; i >= 0; i--)
        {
            if (conditionList[i].dispellable && conditionList[i].buffID <= 100)
            {
                OnBuffDispelledEventDelegate?.Invoke(conditionList[i]);
                RemoveCondition(conditionList[i]);
                //驱散特效
                _battleEffectManager.SpawnEffect(gameObject,BasicCalculation.BattleCondition.Dispell);
                return;
            }
        }
    }

    public virtual void RemoveCondition(BattleCondition buff)
    {
        conditionList.Remove(buff);
        _conditionBar?.OnConditionRemove(buff.buffID);
    }

    /// <summary>
    ///   <para>返回同类buff的层数.</para>
    /// </summary>
    public int GetConditionStackNumber(int buffID)
    {
        int cnt = 0;
        foreach (BattleCondition condition in conditionList)
        {
            if (condition.buffID == buffID)
            {
                cnt++;
            }
        }

        return cnt;
    }

    /// <summary>
    ///   <para>返回同类BUFF总额度.</para>
    /// </summary>
    public float GetConditionTotalValue(int buffID)
    {
        float value = 0;
        foreach (BattleCondition condition in conditionList)
        {
            if (condition.buffID == buffID)
            {
                value += condition.effect;
            }
        }

        return value;
    }

    
    /// <summary>
    ///   <para>返回所有BuffID相同的Condition</para>
    ///     <para>Get all conditions of same type, can used in buffstack calculation.</para>
    /// </summary>
    public List<BattleCondition> GetConditionsOfType(int buffID)
    {
        return conditionList.Where(t => (t.buffID == buffID)).ToList();
    }
    
    /// <summary>
    ///   <para>返回所有BuffID相同且组内ID相同的Condition</para>
    ///     <para>Get all conditions of same type and special ID.</para>
    /// </summary>
    public List<BattleCondition> GetExactConditionsOfType(int buffID,int spID)
    {
        //spid default is -1
        return conditionList.Where(t => (t.buffID == buffID) && (t.specialID == spID)).ToList();
    }

    protected void ConditionTick()
    {
        for (int i = 0; i < conditionList.Count; i++)
        {
            if (conditionList[i].duration > 0)
            {
                conditionList[i].lastTime -= Time.deltaTime;
                if (conditionList[i].lastTime <= 0)
                {
                    RemoveCondition(conditionList[i]);
                    break;
                }
            }
        }
    }

    /// <summary>
    ///   <para>移除角色一个最早的Condition</para>
    /// </summary>
    public bool RemoveTimerBuff(int buffID)
    {
        //角色访问移除BUFF的唯一途径！
        var list = GetExactConditionsOfType(buffID, -1);
        if (list.Count > 0)
        {
            RemoveCondition(list[0]);
            return true;
        }
        else return false;

    }


    //Buff override
    protected virtual void RemoveBuffIfReachedLimit(int buffID)
    {
        int totalBuffNum = GetExactConditionsOfType(buffID,-1).Count;
        //In this function, all buff should be stackable and no special identifier.

        if (totalBuffNum >= BasicCalculation.MAXCONDITIONSTACKNUMBER)
        {
            BattleCondition cond = null;
            foreach (var condition in conditionList)
            {
                if (condition.buffID == buffID)
                {
                    cond = condition;
                    //conditionList.Remove(condition);
                    break;
                }
            }

            if (cond != null)
            {
                conditionList.Remove(cond);
            }
        }
    }


    protected virtual void RemoveBuffIfReachedLimit(int buffID, int maxStack)
    {
        int totalBuffNum = GetExactConditionsOfType(buffID,-1).Count;
        /*foreach (var condition in conditionList)
        {
            if (condition.buffID == buffID)
            {
                totalBuffNum++;
            }
        }*/

        if (totalBuffNum > maxStack)
        {
            BattleCondition cond = null;
            foreach (var condition in conditionList)
            {
                if (condition.buffID == buffID)
                {
                    cond = condition;
                    break;
                }
            }

            if (cond != null)
            {
                conditionList.Remove(cond);
            }
        }
    }

    protected virtual void OverrideUnstackableBuff(int buffID, int spID)
    {
        BattleCondition cond = null;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == buffID)
            {
                //conditionList.Remove(condition);
                cond = condition;
                break;
            }
        }

        if (cond != null)
        {
            conditionList.Remove(cond);
        }
    }

    /// <summary>
    ///   <para>使目标开始HOT回血</para>
    /// </summary>
    public IEnumerator HotRecoveryTick() //HOT回血的效果.
    {
        yield return new WaitForSeconds(2.9f);
        //InvokeRepeating("HPRegen",2.9f,2.9f);
        while (GetConditionsOfType((int)BasicCalculation.BattleCondition.HotRecovery).Count > 0)
        {
            HPRegen();
            yield return new WaitForSeconds(2.9f);
            
        }

        healRoutine = null;
    }

    protected void HPRegen()
    {
        float potency = GetRecoveryPotency();
        float potency2 = GetRecoveryPotencyPercentage();

        _battleStageManager.TargetHeal(gameObject,potency, potency2);
        
        //回血,还没实现回血的公式.
        _battleEffectManager.SpawnHealEffect(gameObject);
        
        //var fx = GetComponent<AttackManager>().healbuff;
        //Instantiate(fx, transform.position, transform.rotation, GetComponent<AttackManager>().BuffFXLayer.transform);
    }

    /// <summary>
    ///   <para>目标立即回复生命值（float 回复倍率，float 百分比回复倍率）</para>
    /// </summary>
    public void HPRegenImmediately(float potency,float potency2)
    {
        _battleStageManager.TargetHeal(gameObject,potency, potency2);
        //_battleEffectManager.SpawnHealEffect(gameObject);
        
        //var fx = GetComponent<AttackManager>().healbuff;
        //Instantiate(fx, transform.position, transform.rotation, GetComponent<AttackManager>().BuffFXLayer.transform);
    }

    /// <summary>
    ///   <para>使目标开始DOT损失生命值</para>
    /// </summary>
    public IEnumerator DotTick(BasicCalculation.BattleCondition condition)
    {
        float tickInterval = 3.9f;
        if ((int)condition > 400 && (int)condition < 405 && condition!=BasicCalculation.BattleCondition.Frostbite)
        {
            tickInterval = 2.9f;
        }

        yield return new WaitForSeconds(tickInterval);
        while (GetConditionsOfType((int)condition).Count>0)
        {
            DotDamage(condition);
            yield return new WaitForSeconds(tickInterval);
        }
        
    }

    protected virtual void DotDamage(BasicCalculation.BattleCondition condition)
    {
        float modifier = GetConditionTotalValue((int)condition);

        _battleStageManager.TargetDot(gameObject, condition);
        
        _battleEffectManager.SpawnEffect(gameObject,condition);
        
        //switch (condition)
        //{
        //    case BasicCalculation.BattleCondition.Burn:
        //        _battleEffectManager.SpawnEffect(gameObject,condition);
        //        break;
        //    case BasicCalculation.BattleCondition.Scorchrend:
        //    case BasicCalculation.BattleCondition.Flashburn:
        //        _battleEffectManager.SpawnEffect(gameObject,condition);
        //        break;
        //    default:
        //        break;
        //}
    }

    public void SetConditionBar(UI_ConditionBar bar)
    {
        _conditionBar = bar;
    }





}