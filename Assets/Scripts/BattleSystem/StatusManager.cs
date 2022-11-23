using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusManager : MonoBehaviour
{
    //Base properties
    public int maxBaseHP;
    public int baseDef;
    public int baseAtk;
    public int critRate;
    public int maxHP;
    public int currentHp;

    public int knockbackRes = 0;
    protected int burnRes = 0;
    protected int paralyzeRes = 0;
    protected int poisonRes = 0;
    protected int frostbiteRes = 0;
    protected int bogRes = 0;
    protected int stunRes = 0;
    protected int sleepRes = 0;
    protected int blindRes = 0;
    protected int frozenRes = 0;

    private BattleStageManager _battleStageManager;


    public Coroutine healRoutine = null;
    
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
    
    [SerializeField] protected int defenseDebuff = 0;
    [SerializeField] protected int damageCut = 0;
    [SerializeField] protected int damageCutConst = 0;
    protected int lifeShield;
    protected bool knockbackImmune = false;
    
    protected int healBuffNum = 0;

    public List<BattleCondition> conditionList;

    protected virtual void Awake()
    {
        conditionList = new List<BattleCondition>();
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        _battleStageManager = FindObjectOfType<BattleStageManager>();
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

    public float GetDamageCut()
    {
        return damageCut > 100 ? 1 : (0.01f * (float)damageCut);
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


    //Get a new Timerbuff
    public virtual void ObtainTimerBuff(int buffID, float effect, float duration,
        BattleCondition.buffEffectDisplayType type, int maxStack)
    {
        RemoveBuffIfReachedLimit(buffID, maxStack);
        var buff = new TimerBuff(buffID, effect, duration, type, maxStack);

        conditionList.Add(buff);
    }

    public virtual void ObtainTimerBuff(int buffID, float duration, BattleCondition.buffEffectDisplayType type,
        int maxStack)
    {
        RemoveBuffIfReachedLimit(buffID, maxStack);

        var buff = new TimerBuff(buffID, 0, duration, type, maxStack);

        conditionList.Add(buff);
    }

    public virtual void ObtainTimerBuff(int buffID,float effect, float duration, BattleCondition.buffEffectDisplayType type)
    {
        RemoveBuffIfReachedLimit(buffID);

        var buff = new TimerBuff(buffID, effect, duration, type, BasicCalculation.MAXCONDITIONSTACKNUMBER);

        conditionList.Add(buff);
    }

    // Get a new Unstackable Timerbuff
    public virtual void ObtainUnstackableTimerBuff(int buffID, float effect, float duration,
        BattleCondition.buffEffectDisplayType type, int spID)
    {
        OverrideUnstackableBuff(buffID, spID);
        var buff = new TimerBuff(buffID, effect, duration, type, 1);
        buff.SetUniqueBuffInfo(spID);
        conditionList.Add(buff);
    }

    public virtual void RemoveCondition(BattleCondition buff)
    {
        conditionList.Remove(buff);
    }

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

    //Get all conditions of type, can used in buffstack calculation.
    public List<BattleCondition> GetConditionsOfType(int buffID)
    {
        return conditionList.Where(t => (t.buffID == buffID)).ToList();
    }
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

        if (totalBuffNum >= maxStack)
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
        var fx = GetComponent<AttackManager>().healbuff;
        Instantiate(fx, transform.position, transform.rotation, GetComponent<AttackManager>().BuffFXLayer.transform);
    }

    public void HPRegenImmediately(float potency,float potency2)
    {
        _battleStageManager.TargetHeal(gameObject,potency, potency2);
        var fx = GetComponent<AttackManager>().healbuff;
        Instantiate(fx, transform.position, transform.rotation, GetComponent<AttackManager>().BuffFXLayer.transform);
    }




}