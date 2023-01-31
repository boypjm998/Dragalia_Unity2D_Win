
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusManager : MonoBehaviour
{

    public string displayedName;
    
    [Header("Basic Attributes")]
    
    public int maxBaseHP;
    public int baseDef;
    public int baseAtk;
    public int critRate;
    public int maxHP;
    public int currentHp;

    [Space(10)]
    [Header("Resistances")]
    public int knockbackRes = 0;
    [SerializeField]protected int burnRes = 0;
    [SerializeField]protected int poisonRes = 0;
    [SerializeField]protected int frostbiteRes = 0;
    [SerializeField]protected int paralysisRes = 0;
    [SerializeField]protected int scorchrendRes = 0;
    [SerializeField]protected int stormlashRes = 0;
    [SerializeField]protected int flashburnRes = 0;
    [SerializeField]protected int shadowblightRes = 0;
    [SerializeField]protected int stunRes = 0;
    [SerializeField]protected int sleepRes = 0;
    [SerializeField]protected int bogRes = 0;
    [SerializeField]protected int freezeRes = 0;
    [SerializeField]protected int blindnessRes = 0;
    [SerializeField]protected int cursedRes = 0;
    
    //public int 

    protected BattleStageManager _battleStageManager;
    protected BattleEffectManager _battleEffectManager;

    
    public Coroutine healRoutine = null;
    public Dictionary<int, Coroutine> dotRoutineDict;

    public delegate void TestDelegate(BattleCondition condition);
    public TestDelegate OnBuffEventDelegate;
    public TestDelegate OnBuffDispelledEventDelegate;

    public delegate void SpecialBuffEventDelegate(string message);
    public SpecialBuffEventDelegate OnSpecialBuffDelegate;

    public delegate void StatusManagerVoidDelegate();
    public StatusManagerVoidDelegate OnHPBelow0;

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
        HPCheck();
        ConditionTick();
        StatusLimitCheck();
        
    }

    protected virtual void HPCheck()
    {
        if (currentHp <= 0)
        {
            OnHPBelow0?.Invoke();
        }
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
    /// 访问抗性
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public int GetAfflictionResistance(BasicCalculation.BattleCondition condition)
    {
        if (IsDotAffliction((int)condition) || IsControlAffliction((int)condition))
        {
            switch (condition)
            {
                case BasicCalculation.BattleCondition.Flashburn:
                    return flashburnRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.FlashburnRes);
                case BasicCalculation.BattleCondition.Burn:
                    return burnRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.BurnRes);
                case BasicCalculation.BattleCondition.Scorchrend:
                    return scorchrendRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.ScorchrendRes);
                case BasicCalculation.BattleCondition.Poison:
                    return poisonRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.PoisonRes);
                case BasicCalculation.BattleCondition.Freeze:
                    return freezeRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.FreezeRes);
                case BasicCalculation.BattleCondition.Cursed:
                    return cursedRes;
                case BasicCalculation.BattleCondition.Paralysis:
                    return paralysisRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.ParalysisRes);
                case BasicCalculation.BattleCondition.Frostbite:
                    return frostbiteRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.FrostbiteRes);
                case BasicCalculation.BattleCondition.Sleep:
                    return sleepRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.SleepRes);
                case BasicCalculation.BattleCondition.Stun:
                    return stunRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.StunRes);
                case BasicCalculation.BattleCondition.Blindness:
                    return blindnessRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.BlindnessRes);
                case BasicCalculation.BattleCondition.Bog:
                    return bogRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.BogRes);
                default:
                    break;
            }
        }
        //Debug.LogWarning("No Such Resistance");
        return 0;
        

    }

    public void IncreaseAfflictionResistance(int buffID)
    {
        BasicCalculation.BattleCondition condition = (BasicCalculation.BattleCondition)buffID;
        switch (condition)
        {
            default:
                break;
            case BasicCalculation.BattleCondition.Scorchrend:
                scorchrendRes += 5;
                scorchrendRes = scorchrendRes >= 100 ? 99 : scorchrendRes;
                break;
            case BasicCalculation.BattleCondition.Flashburn:
                flashburnRes += 5;
                flashburnRes = flashburnRes >= 100 ? 99 : flashburnRes;
                break;
            case BasicCalculation.BattleCondition.Burn:
                burnRes += 5;
                burnRes = burnRes >= 100 ? 99 : burnRes;
                break;
            case BasicCalculation.BattleCondition.Paralysis:
                paralysisRes += 5;
                paralysisRes = paralysisRes >= 100 ? 99 : paralysisRes;
                break;
            case BasicCalculation.BattleCondition.Poison:
                poisonRes += 5;
                poisonRes = poisonRes >= 100 ? 99 : poisonRes;
                break;
            case BasicCalculation.BattleCondition.Stormlash:
                stormlashRes += 5;
                stormlashRes = stormlashRes >= 100 ? 99 : stormlashRes;
                break;
            case BasicCalculation.BattleCondition.ShadowBlight:
                shadowblightRes += 5;
                shadowblightRes = shadowblightRes >= 100 ? 99 : shadowblightRes;
                break;
            case BasicCalculation.BattleCondition.Frostbite:
                frostbiteRes += 5;
                frostbiteRes = frostbiteRes >= 100 ? 99 : frostbiteRes;
                break;
            case BasicCalculation.BattleCondition.Freeze:
                freezeRes += 20;
                break;
            case BasicCalculation.BattleCondition.Blindness:
                blindnessRes += 20;
                break;
            case BasicCalculation.BattleCondition.Bog:
                bogRes += 20;
                break;
            case BasicCalculation.BattleCondition.Stun:
                stunRes += 20;
                break;
            case BasicCalculation.BattleCondition.Sleep:
                sleepRes += 20;
                break;
            case BasicCalculation.BattleCondition.Cursed:
                cursedRes += 20;
                break;
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
    public static bool IsControlAffliction(int buffID)
    {
        if (buffID > 410 && buffID < 417)
            return true;
        return false;
    }

    ///Get a new Timerbuff
    public virtual void ObtainTimerBuff(int buffID, float effect, float duration,
        BattleCondition.buffEffectDisplayType type, int maxStack, int spID)
    {
        RemoveBuffIfReachedLimit(buffID, maxStack,spID);
        var buff = new TimerBuff(buffID, effect, duration, type, maxStack, spID);
        
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

    public void ObtainTimerBuff(BattleCondition condition)
    {
        RemoveBuffIfReachedLimit(condition.buffID, condition.maxStackNum,condition.specialID);

        if (IsDotAffliction(condition.buffID) && GetConditionsOfType(condition.buffID).Count==0)
        {
            if (dotRoutineDict.ContainsKey(condition.buffID))
            {
                Coroutine oldRoutine;
                dotRoutineDict.Remove(condition.buffID,out oldRoutine);
                StopCoroutine(oldRoutine);
            }
            var newRoutine = StartCoroutine(DotTick((BasicCalculation.BattleCondition)(condition.buffID)));
            dotRoutineDict.Add(condition.buffID,newRoutine);
        }
        else
        {
            _battleEffectManager.SpawnEffect(gameObject,(BasicCalculation.BattleCondition)(condition.buffID));
        }

        conditionList.Add(condition);
        
        _conditionBar?.OnConditionAdd(condition);
        
        OnBuffEventDelegate?.Invoke(condition);
    }

    /// <summary>
    ///   <para>给自身附加多层同类BUFF.</para>
    /// </summary>
    public virtual void ObtainTimerBuffs(int buffID, float duration, BattleCondition.buffEffectDisplayType type,
        int stackNum, int maxStack, int spID)
    {
        

        var buff = new TimerBuff(buffID, 0, duration, type, maxStack, spID);
        
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
        OverrideUnstackableBuff(buffID, spID, effect,duration);
        
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
                //OnBuffDispelledEventDelegate?.Invoke(conditionList[i]);
                RemoveConditionWithLog(conditionList[i]);
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
    public virtual void RemoveConditionWithLog(BattleCondition buff)
    {
        conditionList.Remove(buff);
        _conditionBar?.OnConditionRemove(buff.buffID);
        OnBuffDispelledEventDelegate?.Invoke(buff);
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

    public virtual void ResetAllStatus()
    {
        
        for (int i = conditionList.Count-1; i >= 0; i--)
        {
            print(i+"个BUFF");
            if (conditionList[i].dispellable == false || (conditionList[i].buffID > 100 && conditionList[i].buffID < 200))
            {
                continue;
            }
            RemoveCondition(conditionList[i]);
        }
        OnSpecialBuffDelegate?.Invoke("Reset");
    }
    
    public virtual void ResetAllStatusForced()
    {
        for (int i = conditionList.Count-1; i >= 0; i--)
        {
            RemoveCondition(conditionList[i]);
        }
        //OnSpecialBuffDelegate?.Invoke("Reset");
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
    protected virtual void RemoveBuffIfReachedLimit(int buffID, int maxStack, int spID)
    {
        int totalBuffNum = GetExactConditionsOfType(buffID,spID).Count;
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

    protected virtual void OverrideUnstackableBuff(int buffID, int spID, float effect, float duration)
    {
        BattleCondition cond = null;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == buffID)
            {
                if (condition.effect <= effect || condition.lastTime < duration)
                {
                    cond = condition;
                    break;
                }

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

        _battleStageManager.TargetHeal(gameObject,potency, potency2,true);
        
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
        _battleStageManager.TargetHeal(gameObject,potency, potency2,true);
        //_battleEffectManager.SpawnHealEffect(gameObject);
        
        //var fx = GetComponent<AttackManager>().healbuff;
        //Instantiate(fx, transform.position, transform.rotation, GetComponent<AttackManager>().BuffFXLayer.transform);
    }
    /// <summary>
    ///   <para>目标立即回复生命值（float 回复倍率，float 百分比回复倍率）</para>
    /// </summary>
    public void HPRegenImmediatelyWithoutRandom(float potency,float potency2)
    {
        _battleStageManager.TargetHeal(gameObject,potency, potency2,false);
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