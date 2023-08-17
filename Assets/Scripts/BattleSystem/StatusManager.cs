
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GameMechanics;

public class StatusManager : MonoBehaviour
{

    public string displayedName;
    public int dialogIconID;

    [Header("Basic Attributes")] public int maxBaseHP;
    [Range(1, 1024)] public int baseDef = 10;
    public int baseAtk;
    public int critRate;
    public int maxHP;
    public int currentHp;

    public float comboConnectMaxInterval = 4.0f;
    protected Coroutine calroutine; //Calculate Left Combo tim
    public int comboHitCount = 0;
    public float lastComboRemainTime { get; protected set; } = 0;
    protected Coroutine comboRoutine = null;

    [Space(10)] [Header("Resistances")] public int knockbackRes = 0;
    [SerializeField] protected int burnRes = 0;
    [SerializeField] protected int poisonRes = 0;
    [SerializeField] protected int frostbiteRes = 0;
    [SerializeField] protected int paralysisRes = 0;
    [SerializeField] protected int scorchrendRes = 0;
    [SerializeField] protected int stormlashRes = 0;
    [SerializeField] protected int flashburnRes = 0;
    [SerializeField] protected int shadowblightRes = 0;
    [SerializeField] protected int stunRes = 0;
    [SerializeField] protected int sleepRes = 0;
    [SerializeField] protected int bogRes = 0;
    [SerializeField] protected int freezeRes = 0;
    [SerializeField] protected int blindnessRes = 0;
    [SerializeField] protected int cursedRes = 0;


    [Space(10)] [Header("Ability")] [SerializeField]
    public List<int> abilityList = new();

    //public int 

    protected BattleStageManager _battleStageManager;
    protected BattleEffectManager _battleEffectManager;


    public Coroutine healRoutine = null;
    public Dictionary<int, Coroutine> dotRoutineDict;
    public Tuple<int, Coroutine> controlRoutine = null;

    public delegate void TestDelegate(BattleCondition condition);

    /// <summary>
    /// 当buff被添加时触发
    /// </summary>
    public TestDelegate OnBuffEventDelegate;

    /// <summary>
    /// 当buff被驱散时触发
    /// </summary>
    public TestDelegate OnBuffDispelledEventDelegate;

    /// <summary>
    /// 当buff因为时间到期而消失时触发
    /// </summary>
    public TestDelegate OnBuffExpiredEventDelegate;

    /// <summary>
    /// 当异常状态遭到抵抗时触发
    /// </summary>
    public TestDelegate OnAfflictionResist;

    public delegate void SpecialBuffEventDelegate(string message);

    /// <summary>
    /// Reset/ReliefAllDebuff/ReliefAllAffliction/SPCharge/CounterReady
    /// </summary>
    public SpecialBuffEventDelegate OnSpecialBuffDelegate;

    public delegate void StatusManagerVoidDelegate();

    public delegate void StatusManagerIntDelegate(int value);

    public delegate void StatusManagerDelegate(StatusManager self);
    public delegate void DualStatusManagerDelegate(StatusManager self, StatusManager source);

    public StatusManagerVoidDelegate OnHPBelow0;
    public StatusManagerVoidDelegate OnHPChange;

    /// <summary>
    /// 当受到直接伤害时触发
    /// </summary>
    public StatusManagerDelegate OnTakeDirectDamage;
    public DualStatusManagerDelegate OnTakeDirectDamageFrom;

    /// <summary>
    /// 当HP减少时触发
    /// </summary>
    public StatusManagerIntDelegate OnHPDecrease;

    [SerializeField] protected UI_ConditionBar _conditionBar;

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

    public float dmgUpBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetDmgBuff();
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

    public float fsDmgBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetForceStrikeDamageBuff();
            else return 0;
        }
    }

    public float fsSpeedBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetForceStrikeSpeedBuff();
            else return 0;
        }
    }

    public float dmgCutBuff
    {
        get
        {
            if (conditionList.Count > 0)
                return GetDamageCut();
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

    public float burnPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetBurnPunisher();
            else return 0;
        }
    }

    public float poisonPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetPoisonPunisher();
            else return 0;
        }
    }

    public float frostbitePunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetFrostbitePunisher();
            else return 0;
        }
    }

    public float paralysisPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetParalysisPunisher();
            else return 0;
        }
    }

    public float scorchrendPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetScorchrendPunisher();
            else return 0;
        }
    }

    public float stormlashPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetStormlashPunisher();
            else return 0;
        }
    }

    public float flashburnPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetFlashburnPunisher();
            else return 0;
        }
    }

    public float shadowblightPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetShadowblightPunisher();
            else return 0;
        }
    }

    public float stunPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetStunPunisher();
            else return 0;
        }
    }

    public float sleepPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetSleepPunisher();
            else return 0;
        }
    }

    public float bogPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetBogPunisher();
            else return 0;
        }
    }

    public float freezePunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetFreezePunisher();
            else return 0;
        }
    }

    public float blindnessPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetBlindnessPunisher();
            else return 0;
        }
    }

    public float conditionPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetConditionPunisher();
            else return 0;
        }
    }

    public float breakPunisher
    {
        get
        {
            if (conditionList.Count > 0)
                return GetBreakPunisher();
            else return 0;
        }
    }

    /// <summary>
    /// The real kbRes including buff.
    /// </summary>
    public int KnockbackRes => GetKBRes();

    #region AfflictionRes

    public int FlashburnRes
    {
        get
        {
            return (int)(flashburnRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.FlashburnRes) +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.FlashburnResDown));
        }
    }

    public int BurnRes
    {
        get
        {
            print((int)(burnRes +
                        GetConditionTotalValue((int)BasicCalculation.BattleCondition.BurnRes) -
                        GetConditionTotalValue((int)BasicCalculation.BattleCondition.BurnResDown)));
            return (int)(burnRes +
                         GetBurnResistanceBuff());
        }
    }

    public int PoisonRes
    {
        get
        {
            return (int)(poisonRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.PoisonRes) -
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.PoisonResDown));
        }
    }

    public int ParalysisRes
    {
        get
        {
            return (int)(paralysisRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.ParalysisRes) +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.ParalysisResDown));
        }
    }

    public int FrostbiteRes
    {
        get
        {
            return (int)(frostbiteRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.FrostbiteRes) +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.FrostbiteResDown));
        }
    }

    public int ScorchrendRes
    {
        get
        {
            return (int)(scorchrendRes +
                         GetScorchrendResistanceBuff());
        }
    }

    public int StormlashRes
    {
        get
        {
            return (int)(stormlashRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.StormlashRes) +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.StormlashResDown));
        }
    }

    public int ShadowblightRes
    {
        get
        {
            return (int)(shadowblightRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.ShadowBlightRes) +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.ShadowBlightResDown));
        }
    }

    public int StunRes
    {
        get
        {
            return (int)(stunRes +
                         GetConditionTotalValue((int)BasicCalculation.BattleCondition.StunRes));
            //+GetConditionTotalValue((int)BasicCalculation.BattleCondition.StunResDown));
        }
    }


    #endregion

    public float ODAccerator => GetODAccelerator();


    #endregion

    private int knockbackPowerBuff;

    //Offensive Debuff


    //Defensive Buff

    protected int lifeShield;
    public bool waitForRevive = false;
    protected bool knockbackImmune = false;

    protected int healBuffNum = 0;

    public List<BattleCondition> conditionList = new();





    protected virtual void Awake()
    {
        conditionList = new List<BattleCondition>();
        dotRoutineDict = new Dictionary<int, Coroutine>();
        InitDisplayedName();
    }


    // Start is called before the first frame update
    protected virtual void Start()
    {
        
        ReloadBuffTextAsset();
        _battleStageManager = BattleStageManager.Instance;
        _battleEffectManager = BattleEffectManager.Instance;
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
        if (GlobalController.currentGameState != GlobalController.GameState.Inbattle)
            return;
        if (currentHp <= 0)
        {

            //waitForRevive = true;
            OnHPBelow0?.Invoke();
        }

        GetMaxHP();
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

    public virtual void ComboConnect()
    {
        comboHitCount++;

        lastComboRemainTime = comboConnectMaxInterval;
        if (comboRoutine != null)
            StopCoroutine(comboRoutine);
        comboRoutine = StartCoroutine(ComboCheck());
    }

    protected virtual IEnumerator ComboCheck()
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

            yield return new WaitForSeconds(comboConnectMaxInterval);
            comboHitCount = 0;

            StopCoroutine(calroutine);
            calroutine = null;

            lastComboRemainTime = 0;
        }
    }

    protected IEnumerator CalculateRemain(float time)
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

    #region Normal Buff Getter




    public void GetMaxHP()
    {
        float hpBuff = 0;
        foreach (var condition in conditionList)
        {

            if (condition.buffID == (int)BasicCalculation.BattleCondition.MaxHPBuff)
            {
                hpBuff += condition.effect;
            }

            if (condition.buffID == (int)BasicCalculation.BattleCondition.ManaOverloaded)
            {
                maxHP = 1;
                return;
            }

        }

        if (hpBuff > 30)
        {
            hpBuff = 0.3f;
        }

        if (hpBuff < -30)
        {
            hpBuff = -0.3f;
        }

        maxHP = (int)(maxBaseHP * (1 + hpBuff));

    }

    public float GetAttackBuff(int type = 0)
    {
        float totalbuff = 0;
        float buff = 0, debuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 1)
            {

                buff += condition.effect;
            }
            else if (condition.buffID == 201)
            {
                //totalbuff -= condition.effect;
                debuff += condition.effect;
            }
            else
            {
                var eff = GetSpecialAttackBuff(condition.buffID);
                //totalbuff += eff;
                if (eff > 0)
                {
                    buff += eff;
                }
                else
                {
                    debuff += eff;
                }
            }
        }

        if (buff > BasicCalculation.BattleConditionLimit(1))
        {
            buff = BasicCalculation.BattleConditionLimit(1);
        }

        if (debuff > BasicCalculation.BattleConditionLimit(201))
        {
            debuff = BasicCalculation.BattleConditionLimit(201);
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


        // if (totalbuff > 200)
        // {
        //     return 2;
        // }
        // else if (totalbuff < -50)
        // {
        //     return -0.5f;
        // }

        return (buff - debuff);

    }

    protected float GetDmgBuff(int type = 0)
    {
        float totalbuff = 0;
        float buff = 0, debuff = 0;

        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.DamageUp)
            {
                //totalbuff += condition.effect;
                buff += condition.effect;
            }
            else if (condition.buffID == (int)BasicCalculation.BattleCondition.DamageDown)
            {
                //totalbuff -= condition.effect;
                debuff += condition.effect;
            }
            else
            {
                //TODO: totalbuff += GetSpecialDmgBuff(condition.buffID);
            }
        }

        if (buff > BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.DamageUp))
        {
            buff = BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.DamageUp);
        }

        if (debuff > BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.DamageDown))
        {
            debuff = BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.DamageDown);
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



        return (float)buff - debuff;


    }

    public float GetDefenseBuff(int type = 0)
    {
        float totalbuff = 0;
        float buff = 0, debuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 2)
            {
                //totalbuff += condition.effect;
                buff += condition.effect;
            }
            else if (condition.buffID == 202)
            {
                //totalbuff -= condition.effect;
                debuff += condition.effect;
            }
            else
            {
                var eff = GetSpecialDefenseBuff(condition.buffID);
                //totalbuff += eff;
                if (eff > 0)
                {
                    buff += eff;
                }
                else
                {
                    debuff += eff;
                }
            }
        }

        if (buff > BasicCalculation.BattleConditionLimit(2))
        {
            buff = BasicCalculation.BattleConditionLimit(2);
        }

        if (debuff > BasicCalculation.BattleConditionLimit(202))
        {
            debuff = BasicCalculation.BattleConditionLimit(202);
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


        return (buff - debuff);

    }

    public float GetDamageCut(int type = 0)
    {
        float totalbuff = 0;
        float buff = 0, debuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 10)
            {
                //totalbuff += condition.effect;
                buff += condition.effect;
            }
            else if (condition.buffID == 210)
            {
                //totalbuff -= condition.effect;
                debuff += condition.effect;
            }
            else
            {
                var eff = GetSpecialDamageCut(condition.buffID);

                if (eff > 0)
                {
                    buff += eff;
                }
                else
                {
                    debuff += eff;
                }
            }
        }

        if (buff > BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.DamageCut))
        {
            buff = BasicCalculation.BattleConditionLimit(10);
        }

        if (debuff > BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.Vulnerable))
        {
            debuff = BasicCalculation.BattleConditionLimit(210);
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

        //return damageCut > 100 ? 1 : (0.01f * (float)damageCut);
    }

    public float GetDamageCutConst()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 11)
            {
                totalbuff += condition.effect;
            }
            else
            {
                if (condition.buffID == (int)BasicCalculation.BattleCondition.ScorchingEnergy)
                {
                    totalbuff += 100;
                }
            }
        }

        return totalbuff;
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
                totalbuff += GetSpecialSkillDamageBuff(condition.buffID, condition.effect);
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

    public float GetForceStrikeDamageBuff()
    {
        float totalbuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.ForceStrikeDmgBuff)
            {
                totalbuff += condition.effect;
            }
            else if (condition.buffID == 218)
            {
                totalbuff -= condition.effect;
            }
            else
            {
                totalbuff += GetSpecialFSDamageBuff(condition.buffID);
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

    public float GetForceStrikeSpeedBuff(int type = 0)
    {
        float totalbuff = 0;
        float buff = 0, debuff = 0;
        //返回一个float.
        foreach (var condition in conditionList)
        {
            var eff = GetSpecialFSSpeedBuff(condition.buffID);
            totalbuff += eff;
            if (eff > 0)
            {
                buff += eff;
            }
            else
            {
                debuff += eff;
            }
            //}
        }

        if (type == 1)
        {
            totalbuff = buff;
        }
        else if (type == 2)
        {
            totalbuff = debuff;
        }

        if (totalbuff > 100)
        {
            return 1f;
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

//按照系数的回血计算
    public float GetRecoveryPotency()
    {
        float totalbuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 5)
            {
                if (condition.effect > 0)
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
                if (condition.effect < 0)
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
                //totalbuff += GetSpecialSkillDamageBuff(condition.buffID);
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

    #endregion

    #region punisherbuffs




    protected float GetBurnPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.BurnPunisher));
        return totalbuff / 100f;
    }

    protected float GetFreezePunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.FreezePunisher));
        return totalbuff / 100f;
    }

    protected float GetPoisonPunisher()
    {
        float buff = 0;
        
        buff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.PoisonPunisher));

        return buff / 100f;
    }

    protected float GetParalysisPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.ParalysisPunisher));
        return totalbuff / 100f;
    }

    protected float GetStunPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.StunPunisher));
        return totalbuff / 100f;
    }

    protected float GetSleepPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.SleepPunisher));
        return totalbuff / 100f;
    }

    protected float GetBogPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.BogPunisher));
        return totalbuff / 100f;
    }

    protected float GetBlindnessPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.BlindnessPunisher));
        return totalbuff / 100f;
    }

    protected float GetFlashburnPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.FlashburnPunisher));
        return totalbuff / 100f;
    }

    protected float GetScorchrendPunisher(int type = 0)
    {
        float buff = 0, debuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.ScorchrendPunisher)
            {
                buff += (int)condition.effect;
            }

            if(condition.buffID == (int)(BasicCalculation.BattleCondition.ScorchingEnergy))
            {
                buff += 10;
            }
        }

        if (buff > BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.ScorchrendPunisher))
        {
            buff = BasicCalculation.BattleConditionLimit((int)BasicCalculation.BattleCondition.ScorchrendPunisher);
        }
        
        buff = buff / 100f;
        debuff = debuff / 100f;

        if (type == 1)
            return buff;
        if (type == 2)
            return debuff;


        return (buff - debuff);
    }

    protected float GetShadowblightPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.ShadowblightPunisher));
        return totalbuff / 100f;
    }

    protected float GetFrostbitePunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.FrostbitePunisher));
        return totalbuff / 100f;
    }

    protected float GetStormlashPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.StormlashPunisher));
        return totalbuff / 100f;
    }

    protected float GetConditionPunisher()
    {
        return 0;
    }

    protected float GetBreakPunisher()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.BreakPunisher));
        return totalbuff / 100f;
    }

    protected float GetODAccelerator()
    {
        float totalbuff = 0;
        totalbuff += GetConditionTotalValue((int)(BasicCalculation.BattleCondition.OverdriveAccerlerator));
        return totalbuff / 100f;
    }

    #endregion

    #region RateAndResistance

    protected int GetDebuffRateUp()
    {
        int totalbuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.DebuffRateUp)
            {
                totalbuff += (int)condition.effect;
            }

            // if (condition.buffID == 61)
            // {
            //     totalbuff += (int)condition.effect;
            // }

        }

        return totalbuff;
    }

    protected int GetBurnRateUpBuff()
    {
        int totalbuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == 112)
            {
                totalbuff += 30;
            }

            if (condition.buffID == 61)
            {
                totalbuff += (int)condition.effect;
            }

        }

        return totalbuff;
    }

    protected int GetScorchrendRateUpBuff()
    {
        int totalbuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.ScorchrendRateUp)
            {
                totalbuff += (int)condition.effect;
            }

        }

        return totalbuff;
    }

    protected int GetBurnResistanceBuff(int type = 0)
    {
        int totalbuff = 0;
        int buff = 0, debuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.BurnRes)
            {
                buff += (int)condition.effect;
            }

            if (condition.buffID == (int)BasicCalculation.BattleCondition.BurnResDown)
            {
                debuff += (int)condition.effect;
            }
        }
        
        if(buff > 500)
            buff = 500;
        if (debuff > 200)
            debuff = 200;

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

    protected int GetScorchrendResistanceBuff(int type = 0)
    {
        int totalbuff = 0;
        int buff = 0, debuff = 0;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == (int)BasicCalculation.BattleCondition.ScorchrendRes)
            {
                buff += (int)condition.effect;
            }

            if (condition.buffID == (int)BasicCalculation.BattleCondition.ScorchrendResDown)
            {
                debuff += (int)condition.effect;
            }
            
            if(condition.buffID == (int)(BasicCalculation.BattleCondition.ScorchingEnergy))
            {
                debuff += (int)(80+condition.effect*20);
            }
            
            
        }
        
        if(buff > 500)
            buff = 500;
        if (debuff > 200)
            debuff = 200;

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
    
    
    public int GetKBRes()
    {
        int totalbuff = knockbackRes;
        if(GetConditionStackNumber((int)(BasicCalculation.BattleCondition.KnockBackImmune))> 0)
        {
            return 999;
        }
        if(GetConditionStackNumber((int)(BasicCalculation.BattleCondition.HolyFaith))> 0)
        {
            return 999;
        }

        for (int i = 0; i < conditionList.Count; i++)
        {
            // if (IsControlAffliction(conditionList[i].buffID))
            // {
            //     return 999;
            // }
        }

        return totalbuff;
    }
    
    public int GetKBResBuff()
    {
        int totalbuff = 0;
        if(GetConditionStackNumber((int)(BasicCalculation.BattleCondition.KnockBackImmune))> 0)
        {
            return 999;
        }
        if(GetConditionStackNumber((int)(BasicCalculation.BattleCondition.HolyFaith))> 0)
        {
            return 999;
        }

        //Todo: Cancel the KBImmune buff later
        for (int i = 0; i < conditionList.Count; i++)
        {
            if (IsControlAffliction(conditionList[i].buffID))
            {
                return 999;
            }
        }

        return totalbuff;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public int GetConditionRateBuff(BasicCalculation.BattleCondition condition)
    {
        int extraChance = 0;
        if (IsAffliction((int)condition))
        {
            switch (condition)
            {
                case BasicCalculation.BattleCondition.Burn:
                    extraChance = burnRes + GetBurnRateUpBuff();
                    break;
                
                default:
                    break;
            }
        }
        else if(IsDebuff((int)condition))
        {
            extraChance += GetDebuffRateUp();
        }

        return extraChance;
    }

    /// <summary>
    /// 访问抗性,后续需要修改添加
    /// </summary>
    /// <param name="condition"></param>
    /// <returns></returns>
    public int GetAfflictionResistance(BasicCalculation.BattleCondition condition)
    {
        int res = 0;
        if (IsAffliction((int)condition))
        {
            switch (condition)
            {
                case BasicCalculation.BattleCondition.Flashburn:
                    res = FlashburnRes;
                    break;
                case BasicCalculation.BattleCondition.Burn:
                    res = BurnRes;
                    break;
                case BasicCalculation.BattleCondition.Scorchrend:
                    res = ScorchrendRes;
                    break;
                case BasicCalculation.BattleCondition.Poison:
                    res = PoisonRes;
                    break;
                case BasicCalculation.BattleCondition.Freeze:
                    res = freezeRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.FreezeRes);
                    break;
                case BasicCalculation.BattleCondition.Cursed:
                    res = cursedRes;
                    break;
                case BasicCalculation.BattleCondition.Paralysis:
                    res = ParalysisRes;
                    break;
                case BasicCalculation.BattleCondition.Frostbite:
                    res = FrostbiteRes;
                    break;
                case BasicCalculation.BattleCondition.Sleep:
                    res = sleepRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.SleepRes);
                    break;
                case BasicCalculation.BattleCondition.Stun:
                    res = stunRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.StunRes);
                    break;
                case BasicCalculation.BattleCondition.Blindness:
                    res = blindnessRes +
                          (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.BlindnessRes);
                    break;
                case BasicCalculation.BattleCondition.Bog:
                    res = bogRes + (int)GetConditionTotalValue((int)BasicCalculation.BattleCondition.BogRes);
                    break;
                default:
                    break;
            }

            //return res;
        }

        //Debug.LogWarning("No Such Resistance");
        return res;


    }

    public void IncreaseAfflictionResistance(int buffID)
    {
        if (GetAbility(80001))
        {
            return;
        }

        BasicCalculation.BattleCondition condition = (BasicCalculation.BattleCondition)buffID;
        switch (condition)
        {
            default:
                break;
            case BasicCalculation.BattleCondition.Scorchrend:
                if(scorchrendRes>=100)
                    return;
                scorchrendRes += 5;
                scorchrendRes = scorchrendRes >= 100 ? 99 : scorchrendRes;
                break;
            case BasicCalculation.BattleCondition.Flashburn:
                if(flashburnRes>=100)
                    return;
                flashburnRes += 5;
                flashburnRes = flashburnRes >= 100 ? 99 : flashburnRes;
                break;
            case BasicCalculation.BattleCondition.Burn:
                if(burnRes>=100)
                    return;
                burnRes += 5;
                burnRes = burnRes >= 100 ? 99 : burnRes;
                break;
            case BasicCalculation.BattleCondition.Paralysis:
                if(paralysisRes>=100)
                    return;
                paralysisRes += 5;
                paralysisRes = paralysisRes >= 100 ? 99 : paralysisRes;
                break;
            case BasicCalculation.BattleCondition.Poison:
                if(poisonRes>=100)
                    return;
                poisonRes += 5;
                poisonRes = poisonRes >= 100 ? 99 : poisonRes;
                break;
            case BasicCalculation.BattleCondition.Stormlash:
                if (stormlashRes >= 100)
                    return;
                stormlashRes += 5;
                stormlashRes = stormlashRes >= 100 ? 99 : stormlashRes;
                break;
            case BasicCalculation.BattleCondition.ShadowBlight:
                if(shadowblightRes>=100)
                    return;
                shadowblightRes += 5;
                shadowblightRes = shadowblightRes >= 100 ? 99 : shadowblightRes;
                break;
            case BasicCalculation.BattleCondition.Frostbite:
                if (frostbiteRes>= 100)
                    return;
                frostbiteRes += 5;
                frostbiteRes = frostbiteRes >= 100 ? 99 : frostbiteRes;
                break;
            case BasicCalculation.BattleCondition.Freeze:
                if(freezeRes < 100)
                    freezeRes += 20;
                break;
            case BasicCalculation.BattleCondition.Blindness:
                if (blindnessRes < 100)
                {
                    blindnessRes += 20;
                }
                break;
            case BasicCalculation.BattleCondition.Bog:
                if(bogRes<100)
                    bogRes += 20;
                break;
            case BasicCalculation.BattleCondition.Stun:
                if(stunRes<100)
                    stunRes += 20;
                break;
            case BasicCalculation.BattleCondition.Sleep:
                if (sleepRes < 100)
                {
                    sleepRes += 20; 
                }
                break;
            case BasicCalculation.BattleCondition.Cursed:
                cursedRes += 20;
                break;
        }


    }



    #region Special Condition Getter
    
    /// <summary>
    /// 获取目标减伤百分比(减伤BUFF-破甲BUFF)
    /// </summary>
    /// <returns></returns>
    

    protected float GetSpecialAttackBuff(int buffID)
    {
        switch (buffID)
        {
            case 102:
                return 50;
            case 106:
                return 30;
            default:
                return 0;
        }
    }

    protected float GetSpecialCritBuff(int conditionBuffID)
    {
        return 0;
    }

    protected float GetSpecialDefenseBuff(int conditionBuffID)
    {
        return 0;
    }

    protected float GetSpecialSkillDamageBuff(int conditionBuffID, float effect = -1f)
    {
        switch (conditionBuffID)
        {
            case 112:
            {
                if (effect == 1)
                    return 10;
                else if (effect == 2)
                    return 30;
                break;
            }
        }
        return 0;
    }

    protected float GetSpecialCritDamageBuff(int conditionBuffID)
    {
        return 0;
    }
    
    protected float GetSpecialFSDamageBuff(int conditionBuffID)
    {
        switch (conditionBuffID)
        {
            case ((int)BasicCalculation.BattleCondition.HolyFaith):
            {
                if (GetAbility(90002))
                    return 0;
                return 20;
            }
        }
        return 0;
    }
    
    protected float GetSpecialFSSpeedBuff(int conditionBuffID)
    {
        switch (conditionBuffID)
        {
            case ((int)BasicCalculation.BattleCondition.HolyFaith):
            {
                return 40;
            }
        }
        return 0;
    }

    protected float GetSpecialRecoveryPotency(int conditionBuffID)
    {
        switch (conditionBuffID)
        {
            case 106:
                return 1.2f;
        }
        return 0;
    }

    protected float GetSpecialRecoveryBuff(int conditionBuffID)
    {
        return 0;
    }

    protected float GetSpecialDamageCut(int conditionBuffID)
    {
        switch (conditionBuffID)
        {
            case 103:
            {
                if (GetAbility(90002))
                    return 0;
                return 50;
            }
                
        }
        return 0;
    }

    #endregion

    public static bool IsBuff(int buffID)
    {
        if (buffID <= 200)
        {
            return true;
        }

        return false;
    }
    
    public static bool IsDebuff(int buffID)
    {
        if (buffID > 300 && buffID <= 400)
        {
            return true;
        }

        return false;
    }
    
    public static bool IsAffliction(int buffID)
    {
        if (buffID > 400 && buffID <= 420)
        {
            return true;
        }

        return false;
    }

    public static bool IsDotAffliction(int buffID)
    {
        if (buffID > 400 && buffID < 410)
            return true;
        return false;
    }

    public static bool IsControlAffliction(int buffID)
    {
        if (buffID > 410 && buffID < 417 && buffID!=415)
            return true;
        return false;
    }

    ///Get a new Timerbuff.
    /// <para>关于sp_id:角色特化以1+角色id+2位序号作为标识符。如（1003）01/关卡特化以8+关卡id+2位序号作为标识符。
    /// 如泽娜的试炼绝级为8(1024)01</para>
    public virtual void ObtainTimerBuff(int buffID, float effect, float duration,
        int maxStack, int spID)
    {
        //检测虚无状态
        if (GetConditionStackNumber((int)BasicCalculation.BattleCondition.Nihility) > 0)
        {
            if (buffID <= 100 && !BasicCalculation.conditionsImmuneToNihility.Contains(buffID))
                return;
        }


        var buffLimitCheck = RemoveBuffIfReachedLimit(buffID, effect,duration, maxStack, spID);
        
        if(buffLimitCheck==false)
            return;
        
        var buff = new TimerBuff(buffID, effect, duration, maxStack, spID);

        if (IsDotAffliction(buffID) && GetConditionsOfType(buffID).Count == 0)
        {
            if (dotRoutineDict.ContainsKey(buffID))
            {
                Coroutine oldRoutine;
                dotRoutineDict.Remove(buffID, out oldRoutine);
                StopCoroutine(oldRoutine);
            }

            var newRoutine = StartCoroutine(DotTick((BasicCalculation.BattleCondition)buffID));
            dotRoutineDict.Add(buffID, newRoutine);
        }
        else if(IsControlAffliction(buffID))
        {
            if (controlRoutine != null)
            {
                RemoveTimerBuff(buffID);
                print("Stop control routine");
                StopCoroutine(controlRoutine.Item2);
                controlRoutine = null;
            }

            print("Start control routine");
            controlRoutine = new Tuple<int, Coroutine>(buffID,
                StartCoroutine(ControlAfflictedTick((BasicCalculation.BattleCondition)buffID)));
        }
        else
        {
            //print("is not affliction");
            _battleEffectManager.SpawnEffect(gameObject, (BasicCalculation.BattleCondition)buffID);
        }

        conditionList.Add(buff);
        
        if ((GetRecoveryPotency() > 0 || GetRecoveryPotencyPercentage() > 0) && healRoutine == null)
        {
            healRoutine = StartCoroutine(HotRecoveryTick());
        }

        if (buffID == 300)
        {
            ResetAllBuffByNil();
        }

        _conditionBar?.OnConditionAdd(buff);

        OnBuffEventDelegate?.Invoke(buff);
    }


    public virtual void ObtainTimerBuff(int buffID, float effect, float duration)
    {

        if (GetConditionStackNumber((int)BasicCalculation.BattleCondition.Nihility) > 0)
        {
            if (buffID <= 100 && !BasicCalculation.conditionsImmuneToNihility.Contains(buffID))
                return;
        }

        var buff = new TimerBuff(buffID, effect, duration, BasicCalculation.MAXCONDITIONSTACKNUMBER);

        RemoveBuffIfReachedLimit(buffID);

        if (IsDotAffliction(buffID) && GetConditionsOfType(buffID).Count == 0)
        {
            if (dotRoutineDict.ContainsKey(buffID))
            {
                Coroutine oldRoutine;
                dotRoutineDict.Remove(buffID, out oldRoutine);
                StopCoroutine(oldRoutine);
            }

            var newRoutine = StartCoroutine(DotTick((BasicCalculation.BattleCondition)buffID));
            dotRoutineDict.Add(buffID, newRoutine);
        }
        else if(IsControlAffliction(buffID))
        {
            if (controlRoutine != null)
            {
                RemoveTimerBuff(buffID);
                print("Stop control routine");
                StopCoroutine(controlRoutine.Item2);
                controlRoutine = null;
            }

            print("Start control routine");
            controlRoutine = new Tuple<int, Coroutine>(buffID,
                StartCoroutine(ControlAfflictedTick((BasicCalculation.BattleCondition)buffID)));
        }
        else
        {
            _battleEffectManager.SpawnEffect(gameObject, (BasicCalculation.BattleCondition)buffID);
        }

        conditionList.Add(buff);

        if ((GetRecoveryPotency() > 0 || GetRecoveryPotencyPercentage() > 0) && healRoutine == null)
        {
            healRoutine = StartCoroutine(HotRecoveryTick());
        }

        if (buffID == 300)
        {
            ResetAllBuffByNil();
        }

        _conditionBar?.OnConditionAdd(buff);

        OnBuffEventDelegate?.Invoke(buff);

    }

    public void ObtainTimerBuff(BattleCondition condition, bool effect = true)
    {

        if (GetConditionStackNumber((int)BasicCalculation.BattleCondition.Nihility) > 0)
        {
            if (condition.buffID < 100 && !BasicCalculation.conditionsImmuneToNihility.Contains(condition.buffID) &&
                condition.dispellable)
                return;
        }


        //7.25 Added
        var conditionCheck = RemoveBuffIfReachedLimit(condition.buffID,condition.effect,condition.duration, condition.maxStackNum, condition.specialID);

        // if (!conditionCheck)
        // {
        //     return;
        // }
        
        
        if (IsDotAffliction(condition.buffID) && GetConditionsOfType(condition.buffID).Count == 0)
        {
            if (dotRoutineDict.ContainsKey(condition.buffID))
            {
                Coroutine oldRoutine;
                dotRoutineDict.Remove(condition.buffID, out oldRoutine);
                StopCoroutine(oldRoutine);
            }
            
            
            
            var newRoutine = StartCoroutine(DotTick((BasicCalculation.BattleCondition)(condition.buffID)));
            dotRoutineDict.Add(condition.buffID, newRoutine);
        }
        else if(IsControlAffliction(condition.buffID))
        {
            print("Check control routine");
            if (controlRoutine != null)
            {
                RemoveTimerBuff(condition.buffID);
                print("Stop control routine");
                StopCoroutine(controlRoutine.Item2);
                controlRoutine = null;
            }

            
            controlRoutine = new Tuple<int, Coroutine>(condition.buffID,
                StartCoroutine(ControlAfflictedTick((BasicCalculation.BattleCondition)condition.buffID)));
        }
        else if (effect)
        {
            print("is not affliction,effect = "+condition.buffID);
            print(_battleEffectManager + gameObject.name);
            
            BattleEffectManager.Instance.SpawnEffect(gameObject, (BasicCalculation.BattleCondition)(condition.buffID));
        }

        conditionList.Add(condition);

        if ((GetRecoveryPotency() > 0 || GetRecoveryPotencyPercentage() > 0) && healRoutine == null)
        {
            healRoutine = StartCoroutine(HotRecoveryTick());
        }

        if (condition.buffID == 300)
        {
            ResetAllBuffByNil();
        }

        _conditionBar?.OnConditionAdd(condition);

        OnBuffEventDelegate?.Invoke(condition);
        
    }

    /// <summary>
    ///   <para>给自身附加多层同类BUFF.</para>
    /// </summary>
    public virtual void ObtainTimerBuffs(int buffID, float duration,
        int stackNum, int maxStack, int spID)
    {
        if (GetConditionStackNumber((int)BasicCalculation.BattleCondition.Nihility) > 0)
        {
            if (buffID <= 100 && !BasicCalculation.conditionsImmuneToNihility.Contains(buffID))
                return;
        }

        var buff = new TimerBuff(buffID, 0, duration, maxStack, spID);

        _battleEffectManager.SpawnEffect(gameObject, (BasicCalculation.BattleCondition)buffID);

        for (int i = 0; i < stackNum; i++)
        {
            conditionList.Add(buff);
        }

        RemoveBuffIfReachedLimit(buffID, maxStack);

        if ((GetRecoveryPotency() > 0 || GetRecoveryPotencyPercentage() > 0) && healRoutine == null)
        {
            healRoutine = StartCoroutine(HotRecoveryTick());
        }

        if (buffID == 300)
        {
            ResetAllBuffByNil();
        }

        OnBuffEventDelegate?.Invoke(buff);

        _conditionBar?.OnConditionAdd(buff);



    }

    // Get a new Unstackable Timerbuff
    public virtual void ObtainUnstackableTimerBuff(int buffID, float effect, float duration,
        int spID = 0)
    {

        if (GetConditionStackNumber((int)BasicCalculation.BattleCondition.Nihility) > 0)
        {
            if (buffID <= 100 && !BasicCalculation.conditionsImmuneToNihility.Contains(buffID))
                return;
        }

        OverrideUnstackableBuff(buffID, spID, effect, duration);

        var buff = new TimerBuff(buffID, effect, duration, 1);

        if (IsDotAffliction(buffID) && GetConditionsOfType(buffID).Count == 0)
        {
            if (dotRoutineDict.ContainsKey(buffID))
            {
                Coroutine oldRoutine;
                dotRoutineDict.Remove(buffID, out oldRoutine);
                StopCoroutine(oldRoutine);
            }

            var newRoutine = StartCoroutine(DotTick((BasicCalculation.BattleCondition)buffID));
            dotRoutineDict.Add(buffID, newRoutine);
        }
        else if(IsControlAffliction(buffID))
        {
            if (controlRoutine != null)
            {
                RemoveTimerBuff(buffID);
                print("Stop control routine");
                StopCoroutine(controlRoutine.Item2);
                controlRoutine = null;
            }

            print("Start control routine");
            controlRoutine = new Tuple<int, Coroutine>(buffID,
                StartCoroutine(ControlAfflictedTick((BasicCalculation.BattleCondition)buffID)));
        }
        else
        {
            _battleEffectManager.SpawnEffect(gameObject, (BasicCalculation.BattleCondition)buffID);
        }

        buff.SetUniqueBuffInfo(spID);

        conditionList.Add(buff);

        if ((GetRecoveryPotency() > 0 || GetRecoveryPotencyPercentage() > 0) && healRoutine == null)
        {
            healRoutine = StartCoroutine(HotRecoveryTick());
        }

        if (buffID == 300)
        {
            ResetAllBuffByNil();
        }

        _conditionBar?.OnConditionAdd(buff);

        OnBuffEventDelegate?.Invoke(buff);
    }

    public virtual bool DispellTimerBuff()
    {
        for (int i = conditionList.Count - 1; i >= 0; i--)
        {
            if (conditionList[i].dispellable && conditionList[i].buffID <= 100)
            {
                //OnBuffDispelledEventDelegate?.Invoke(conditionList[i]);
                RemoveConditionWithLog(conditionList[i]);
                //驱散特效
                _battleEffectManager.SpawnEffect(gameObject, BasicCalculation.BattleCondition.Dispell);
                return true;
            }
        }

        return false;
    }

    public void ReliefAllDebuff()
    {
        for (int i = conditionList.Count - 1; i >= 0; i--)
        {
            if (conditionList[i].buffID <= 400 && conditionList[i].buffID > 200)
            {
                //OnBuffDispelledEventDelegate?.Invoke(conditionList[i]);
                RemoveCondition(conditionList[i]);

                _battleEffectManager.SpawnEffect(gameObject, BasicCalculation.BattleCondition.AtkBuff);

            }
        }

        OnSpecialBuffDelegate?.Invoke("ReliefAllDebuff");
    }

    public void RemoveAllConditionWithSpecialID(int spID)
    {
        for (int i = conditionList.Count - 1; i >= 0; i--)
        {
            if (conditionList[i].specialID == spID)
            {
                //OnBuffDispelledEventDelegate?.Invoke(conditionList[i]);
                RemoveCondition(conditionList[i]);

            }
        }
    }

    public void ReliefAllAfflication()
    {
        for (int i = conditionList.Count - 1; i >= 0; i--)
        {
            if (IsAffliction(conditionList[i].buffID))
            {
                //OnBuffDispelledEventDelegate?.Invoke(conditionList[i]);
                RemoveCondition(conditionList[i]);

                _battleEffectManager.SpawnEffect(gameObject, BasicCalculation.BattleCondition.AtkBuff);

            }
        }
        OnSpecialBuffDelegate?.Invoke("ReliefAllAffliction");
    }

    protected virtual void RemoveCondition(BattleCondition buff)
    {
        conditionList.Remove(buff);
        _conditionBar?.OnConditionRemove(buff.buffID);
        OnBuffExpiredEventDelegate?.Invoke(buff);
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
    public List<BattleCondition> GetExactConditionsOfType(int buffID, int spID)
    {
        //spid default is -1
        return conditionList.Where(t => (t.buffID == buffID) && (t.specialID == spID)).ToList();
    }

    public List<BattleCondition> GetConditionWithSpecialID(int spID)
    {
        return conditionList.Where(t => t.specialID == spID).ToList();
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
    ///   <para>移除角色一个最早的Condition，，和RemoveSpecificTimerBuff是完全一样的方法</para>
    /// </summary>
    public bool RemoveTimerBuff(int buffID, bool log = false, int spID = -1)
    {
        //角色访问移除BUFF的唯一途径！
        var list = GetExactConditionsOfType(buffID, spID);
        if (list.Count > 0)
        {
            if (log)
            {
                RemoveConditionWithLog(list[0]);
            }
            else
            {
                RemoveCondition(list[0]);
            }

            return true;
        }
        else return false;

    }

    /// <summary>
    ///   <para>移除角色一个最早的Condition，和RemoveTimerBuff是完全一样的方法</para>
    /// </summary>
    public bool RemoveSpecificTimerbuff(int buffID, int spID, bool log = false)
    {
        var list = GetExactConditionsOfType(buffID, spID);
        if (list.Count > 0)
        {
            if (log)
            {
                RemoveConditionWithLog(list[0]);
            }
            else
            {
                RemoveCondition(list[0]);
            }

            return true;
        }
        else return false;
    }

    public virtual void ResetAllStatus()
    {
        
        for (int i = conditionList.Count-1; i >= 0; i--)
        {
            //print(i+"个BUFF");
            if (conditionList[i].dispellable == false || (conditionList[i].buffID > 100 && conditionList[i].buffID < 200))
            {
                continue;
            }
            RemoveCondition(conditionList[i]);
        }
        OnSpecialBuffDelegate?.Invoke("Reset");
    }
    
    public virtual void ResetAllBuffByNil()
    {
        
        for (int i = conditionList.Count-1; i >= 0; i--)
        {
            if(conditionList[i].dispellable == false)
                continue;
            if(BasicCalculation.conditionsImmuneToNihility.Contains(conditionList[i].buffID))
                continue;
            if(conditionList[i].buffID > 200)
                continue;

            RemoveCondition(conditionList[i]);
        }
        //OnSpecialBuffDelegate?.Invoke("Reset");
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

        while (totalBuffNum > maxStack)
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
                totalBuffNum--;
            }
            
        }
    }
    protected virtual bool RemoveBuffIfReachedLimit(int buffID,float effect,float duration,
        int maxStack, int spID)
    {
        int totalBuffNum = GetExactConditionsOfType(buffID,spID).Count;
        var removedConditions = new List<BattleCondition>();
        if (totalBuffNum >= maxStack)
        {
            BattleCondition cond = null;
            foreach (var condition in conditionList)
            {
                if (condition.buffID == buffID && condition.specialID == spID)
                {
                    if (condition.effect > 0 && condition.effect > effect)
                    {
                        continue;
                    }

                    if (condition.lastTime > 0 && condition.lastTime > duration)
                    {
                        continue;
                    }

                    removedConditions.Add(condition);
                    
                    break;
                }
            }

            if (removedConditions.Count > 0)
            {
                foreach (var condition in removedConditions)
                {
                    conditionList.Remove(condition);
                }
                return true;
            }

            return false;
        }

        return true;
    }

    protected virtual void OverrideUnstackableBuff(int buffID, int spID, float effect, float duration)
    {
        BattleCondition cond = null;
        foreach (var condition in conditionList)
        {
            if (condition.buffID == buffID && spID == condition.specialID)
            {
                if (condition.effect <= effect || (condition.lastTime < duration && condition.duration>0))
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
        while (GetConditionsOfType((int)BasicCalculation.BattleCondition.HealOverTime).Count > 0)
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

    public void HPRegenImmediately(StatusManager statusManager, float potency, float potency2)
    {
        _battleStageManager.TargetHeal(statusManager,potency,potency2,true);
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

    public IEnumerator ControlAfflictedTick(BasicCalculation.BattleCondition condition)
    {
        BattleEffectManager.Instance.SpawnEffect(gameObject,condition);
        var ac = GetComponent<ActorBase>();
        //print("StartTick");
        yield return null;
        print(GetConditionStackNumber((int)condition) + condition.ToString());

        while (GetConditionStackNumber((int)condition)>0)
        {
            print(condition);
            yield return null;
            ac.SetActionUnable(true);
        }
        
        ac.SetActionUnable(false);
        controlRoutine = null;
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

    protected void InitDisplayedName()
    {
        var nameStr = FindObjectOfType<GlobalController>().GetNameOfID(dialogIconID);
        if (nameStr != String.Empty)
        {
            displayedName = nameStr;
        }
    }

    public bool GetAbility(int abilityID)
    {
        return abilityList.Contains(abilityID);
    }

    protected void ReloadBuffTextAsset()
    {
        var buffLayer = transform.Find("BuffLayer");
        if(buffLayer==null)
            return;
        print(GlobalController.Instance.GameLanguage);
        if (GlobalController.Instance.GameLanguage == GlobalController.Language.EN)
        {
            var buffTextGameObject = buffLayer.GetComponentInChildren<UI_BuffLogPopManager>();
            if(buffTextGameObject!=null)
                Destroy(buffTextGameObject.gameObject);
            //Instanciate Assets/Resources/UI/InBattle/BuffLogText/BuffText_EN.prefab
            var newBuffTextPrefab = Resources.Load<GameObject>("UI/InBattle/BuffLogText/BuffText_EN");
            var newBuffText = Instantiate(newBuffTextPrefab, transform.position + new Vector3(0,2,0),
                Quaternion.identity,buffLayer);
        }
    }



}
[Serializable]
public class StatusInformationList
{
    public List<StatusInformation> statusInformationList = new();
}
[Serializable]
public class StatusInformation
{
    public int id;
    public string name_en;
    public string name_zh;
    
}