using System;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    public static readonly int DEFAULT_GRAVITY = 4;

    public int chara_id;
    public int skill_id;
    public bool destroyAfterHit = false;

    public List<AttackInfo> attackInfo = new();
    public int firedir; //要改掉

    public BasicCalculation.AttackType attackType;


    [SerializeField] protected AudioClip hitSoundEffect;
    [SerializeField] protected AudioClip[] attackSE;
    [SerializeField] public float extraODModifier = 0;

    protected BattleEffectManager _effectManager;
    
    public List<ConditionalAttackEffect> conditionalAttackEffects = new();
    
    public delegate void AttackBaseDelegate(AttackBase attackBase, GameObject target);
    public AttackBaseDelegate OnAttackHit;
    public AttackBaseDelegate BeforeAttackHit;
    /// <summary>
    /// 当造成伤害时(前)触发，arg1:自身 arg2:目标 ,arg3:攻击 arg4:造成的伤害
    /// </summary>
    public Action<StatusManager, StatusManager, AttackBase, float> OnAttackDealDamage;

    

    protected void DestroyContainer()
    {
        var container = GetComponentInParent<AttackContainer>();
        if (container != null)
        {
            container.DestroyInvoke();
            //Destroy(container.gameObject);
        }

        
        //print("DestroyContainer");
    }

    public virtual void NextAttack()
    {
    }

    public virtual void ResetWithConditionFlags()
    {
    }

    public int GetHitCount()
    {
        return 1;
    }

    public int GetHitCountInfo()
    {
        //print("Hit="+attackInfo[0].dmgModifier.Count);
        return attackInfo[0].dmgModifier.Count;
    }

    public float GetDmgModifier(int id)
    {
        return 0;
    }

    public float GetDmgModifierInfo(int id)
    {
        return attackInfo[0].dmgModifier[id];
    }

    public virtual float GetDmgConstInfo(int id)
    {
        if (attackInfo[0].constDmg.Count > id)
            return attackInfo[0].constDmg[id];
        return 0;
    }

    public virtual Vector2 GetKBDirection(BasicCalculation.KnockBackType knockBackType, GameObject target)
    {
        var kbdirtemp = attackInfo[0].knockbackDirection;
        switch (knockBackType)
        {
            case BasicCalculation.KnockBackType.FaceDirection:
                // if (firedir != 0)
                // {
                //     firedir = transform.lossyScale.x > 0 ? firedir : -firedir;
                // }
                kbdirtemp = new Vector2(firedir * kbdirtemp.x,kbdirtemp.y);
                break;

            case BasicCalculation.KnockBackType.FromCenterRay:
                kbdirtemp = transform.InverseTransformPoint(target.transform.position);
                break;
            case BasicCalculation.KnockBackType.FromCenterFixed:
                kbdirtemp = transform.position.x > target.transform.position.x
                    ? new Vector2(-attackInfo[0].knockbackDirection.x, attackInfo[0].knockbackDirection.y)
                    : attackInfo[0].knockbackDirection;
                break;
            case BasicCalculation.KnockBackType.None:
                kbdirtemp = Vector2.zero;
                break;
        }

        return kbdirtemp;
    }

    public virtual void InitAttackBasicAttributes(float knockbackPower, float knockbackForce, float knockbackTime,
        float dmgModifier, int firedir = 0)
    {
        attackInfo[0].knockbackForce = knockbackForce;
        attackInfo[0].knockbackPower = knockbackPower;
        attackInfo[0].knockbackTime = knockbackTime;
        attackInfo[0].dmgModifier = new List<float>();
        attackInfo[0].dmgModifier.Add(dmgModifier);
        attackInfo[0].firedir = firedir;
    }
    
    public void AddConditionalAttackEffect(ConditionalAttackEffect conditionalAttackEffect)
    {
        conditionalAttackEffects.Add(conditionalAttackEffect);
    }

    /// <summary>
    /// Method is unimplemented
    /// </summary>
    public virtual void AddWithConditionAll(BattleCondition condition, int chance, int identifier = 0)
    {
    }

    /// <summary>
    /// Method is unimplemented
    /// </summary>
    public virtual void AddWithCondition(int hitNo, BattleCondition condition, int chance, int identifier = 0)
    {
        
    }

    public virtual void CheckSpecialConditionalEffectBeforeAttack(StatusManager statusManager)
    {
        // InfernoMode (102)
        if (statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.InfernoMode) > 0)
        {
            if (attackType == BasicCalculation.AttackType.STANDARD)
            {
                int i = 0;
                foreach (var element in attackInfo)
                {
                    var defdebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,
                        10, 10, 1, 0);
                    element.AddWithCondition(defdebuff, 30,i);
                    i++;
                }
            }
        }


        // StandardAttackBurner (111)
        if(statusManager.
               GetConditionsOfType((int)BasicCalculation.BattleCondition.StandardAttackBurner).Count>0)
        {
            if(attackType!=BasicCalculation.AttackType.STANDARD)
                return;
            
            var effect = statusManager.GetConditionTotalValue(
                (int)BasicCalculation.BattleCondition.StandardAttackBurner
            );
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Burn, effect, 12f,100),100);
        }
        
    }





}

[Serializable]
public class AttackInfo
{
    public List<float> dmgModifier;
    public List<float> constDmg;
    public float knockbackPower;
    public float knockbackForce;
    public float knockbackTime;
    public Vector2 knockbackDirection = Vector2.right;
    public BasicCalculation.KnockBackType KBType;
    public int firedir;
    public List<ConditionWithAttackInfo> withConditions;


    public AttackInfo()
    {
        dmgModifier = new List<float>();
        constDmg = new List<float>();
        withConditions = new List<ConditionWithAttackInfo>();
    }
    
    public AttackInfo(List<float> dmgModifier)
    {
        this.dmgModifier = dmgModifier;
        constDmg = new List<float>();
        withConditions = new List<ConditionWithAttackInfo>();
    }

    public void AddWithCondition(BattleCondition condition, int chance = 100, int identifier = 0)
    {
        var conditionWithAttackInfo = new ConditionWithAttackInfo();
        conditionWithAttackInfo.condition = condition;
        conditionWithAttackInfo.withConditionChance = chance;
        conditionWithAttackInfo.identifier = identifier;
        withConditions.Add(conditionWithAttackInfo);
    }

    public float GetTotalDmgModifier()
    {
        float total = 0;
        foreach (var dmg in dmgModifier) total += dmg;

        return total;
    }

    [Serializable]
    public class ConditionWithAttackInfo
    {
        public BattleCondition condition;

        //public int conditionID = -1;
        public int identifier; //用于标记这个异常状态的唯一ID，以便于检测是否已经检测过。内部ID，不会和外部冲突。
        public int withConditionChance;
    }
}

/// <summary>
/// 条件判断敌人身上是否有某些buff来改变攻击属性
/// </summary>
[Serializable]
public class ConditionalAttackEffect
{
    public enum ConditionType
    {
        TargetHasCondition,
        //DependOnTargetHP,
        CrisisModifier,
        Custom
    }

    public enum ExtraEffect
    {
        Custom,
        ExtraConditionToSelf,
        ExtraCritRate,
        ChangeDmgModifier,
        CrisisModifier
        
    }
    
    public ConditionType conditionType;
    public ExtraEffect extraEffect;
    /// <summary>
    /// args about condition
    /// </summary>
    public string[] args1;
    /// <summary>
    /// args about extra effect
    /// </summary>
    public string[] args2;

    # region Private Lists
    //Init by parsing args
    private List<BasicCalculation.BattleCondition> needCheckTargetConditionsList = new();
    private List<BasicCalculation.BattleCondition> needCheckSelfConditionsList = new();
    private Func<StatusManager,StatusManager,bool> customConditionFunc;
    private Func<(StatusManager sourceStat,StatusManager targetStat),AttackBase,int> customEffectFunction;

    // private Tuple<BattleCondition,int> needAppendSelfCondition;
    // private Tuple<BattleCondition,int> needAppendTargetCondition;
    //
    // private Tuple<BattleCondition,int> needRemoveSelfCondition;
    // private Tuple<BattleCondition,int> needRemoveTargetCondition;

    private float extraModifier;
    private int extraCritRate;
    /// <summary>
    /// <para>(minHP(0,1), maxHP(0,1), crisisModifier)</para>
    /// <para>crisisModifier背水系数，在minHP处，倍率为原倍率*crisisModifier,在maxHP处，倍率为原倍率，背水曲线为抛物线。</para>
    /// </summary>
    private Tuple<float, float, float> crisisModifier;


    # endregion
    
    

    
    
    

    /// <param name="args1">格式1:{检查几个buff,buffID1,buffID2,...}</param>
    /// <param name="args2">格式1:{附加倍率}</param>
    public ConditionalAttackEffect(ConditionType conditionType, ExtraEffect extraEffect, string[] args1, string[] args2)
    {
        this.conditionType = conditionType;
        this.extraEffect = extraEffect;
        this.args1 = args1;
        this.args2 = args2;
        ParseArguments();
    }
    
    
    
    public ConditionalAttackEffect(Func<StatusManager,StatusManager,bool> condition,
        ExtraEffect extraEffect, string[] args1, string[] args2)
    {
        this.conditionType = ConditionType.Custom;
        this.extraEffect = extraEffect;
        this.args1 = args1;
        this.args2 = args2;
        customConditionFunc = condition;
        ParseArguments();
    }

    public ConditionalAttackEffect(float crisisFactor, float curveFactor = 0.5f, float maxBound = 1,
        float minBound = 0)
    {
        conditionType = ConditionType.CrisisModifier;
        extraEffect = ExtraEffect.CrisisModifier;
        
        crisisFactor = Mathf.Clamp(crisisFactor, 0.1f, 2);
        curveFactor = Mathf.Clamp(curveFactor, 0.25f, 4);
        maxBound = Mathf.Clamp(maxBound, 0.1f, 1);
        minBound = Mathf.Clamp(minBound, 0, maxBound - 0.01f);
        
        
        
        args1 = new[] {crisisFactor.ToString(), curveFactor.ToString(), maxBound.ToString(), minBound.ToString()};
    }


    public ConditionalAttackEffect SetEffectFunction(Func<(StatusManager sourceStat,StatusManager targetStat),AttackBase,int> func)
    {
        customEffectFunction = func;
        return this;
    }

    public ConditionalAttackEffect SetConidtionalFunction(
        Func<StatusManager, StatusManager, bool> func)
    {
        customConditionFunc = func;
        return this;
    }

    // public float GetCrisisModifier(StatusManager sourceStat)
    // {
    //     
    // }

    public float GetExtraModifiers(StatusManager targetStat, StatusManager sourceStat)
    {
        if (extraEffect != ExtraEffect.ChangeDmgModifier)
            return 0;
        else
        {
            if (CheckConditional(targetStat, sourceStat))
            {
                Debug.Log("Conditional OK");
                return extraModifier;
            }

            return 0;
        }
    }
    
    public float GetExtraCritRate(StatusManager targetStat, StatusManager sourceStat)
    {
        if (extraEffect != ExtraEffect.ExtraCritRate)
            return 0;
        else
        {
            if (CheckConditional(targetStat, sourceStat))
            {
                Debug.Log("Conditional OK");
                return extraCritRate;
            }

            return 0;
        }
    }

    /// <summary>
    /// 注意 只能添加一个
    /// </summary>
    /// <param name="statusManager"></param>
    /// <returns></returns>
    public float GetCrisisModifier(StatusManager statusManager)
    {
        //背水系数（该项越大，随着生命值的下降，伤害越高，小于1代表生命值越高伤害越高）
            //最大HP时的背水系数，正常的背水时，该项的值应该小于最小HP时的背水系数，正常情况下为1。
            
            //假如某角色背水系数为1.5,最少生命值为0，最大生命值为1
            //那么代表当角色生命值为0%时，造成的伤害为150%，当角色生命值为100%时，造成的伤害为100%。
            
            //假如某角色背水系数为0.5,最少生命值为0，最大生命值为1
            //那么代表当角色生命值为0%时，造成的伤害为100%，当角色生命值为100%时，造成的伤害为200%。
            float crisisFactor = ObjectExtensions.ParseInvariantFloat(args1[0]);
            
            //背水函数曲线系数，该项为背水曲线的次方，代表背水曲线的次方。背水函数曲线越大，血量减少带来的伤害影响逐渐增大，反之则相反。
            //当背水曲线为1时，代表背水曲线为线性函数：
            //假设某角色背水系数为1.5，最少生命值为0，最大生命值为1，背水曲线为1
            //那么当角色生命值为50%时，造成的伤害为(1.5-1)*50%+100% = 125%
            
            //假设同一个角色背水系数为0.5，最少生命值为0，最大生命值为1，背水曲线为0.5
            //那么当角色生命值为50%时，造成的伤害为0.5 + 0.5 * HP^(1/2) = 0.5 + 0.5 * 0.707 = 0.854
            
            //假设同一个角色背水系数为1.5，最少生命值为0，最大生命值为1，背水曲线为0.5
            //那么当角色生命值为50%时，造成的伤害为1.5 - 0.5 * HP^(1/2) = 1.5 - 0.5 * 0.707 = 1.146 
            
            //假设同一个角色背水系数为0.5，最少生命值为0，最大生命值为1，背水曲线为2
            //那么当角色生命值为50%时，造成的伤害为0.5 + 0.5 * HP^(1/0.5) = 0.5 + 0.5 * 1.414 = 0.707
            
            //假设同一个角色背水系数为1.5，最少生命值为0，最大生命值为1，背水曲线为2
            //那么当角色生命值为50%时，造成的伤害为1.5 - 0.5 * HP^(1/0.5) = 1.5 - 0.5 * 1.414 = 1.293
            
            
            float curveFactor = ObjectExtensions.ParseInvariantFloat(args1[1]);
            
            float minBound = ObjectExtensions.ParseInvariantFloat(args1[3]); //背水最小生命值(背水系数大于1时，代表生命越低伤害越高）
            //背水最大生命值(背水系数小于1时，代表生命越高伤害越高）
            float maxBound = ObjectExtensions.ParseInvariantFloat(args1[2]);

            float hpFraction = 0; // X（HP）值，自变量
            
            
            
            hpFraction = (float)statusManager.currentHp / (float)statusManager.maxHP;
            Debug.Log("hpFraction: " + hpFraction);
            

            float result = 1;
            
            //根据公式计算背水系数，自变量为HpFraction
            if (hpFraction < minBound)
            {
                return crisisFactor;
            }
            else if (hpFraction > maxBound)
            {
                return 1;
            }
            
            if (crisisFactor == 1)
            {
                return 1;
            }

            float hpFractionNormalized = (hpFraction - minBound) / (maxBound - minBound);

            // 计算结果
            
            if (crisisFactor > 1)
            {
                result = crisisFactor - (crisisFactor - 1) * Mathf.Pow(hpFractionNormalized, 1 / curveFactor);
            }
            else
            {
                result = crisisFactor + (1 - crisisFactor) * Mathf.Pow(hpFractionNormalized, 1 / curveFactor);
            }

            
            return (result);
    }

    /// <summary>
    /// 返回：附加伤害
    /// </summary>
    /// <returns></returns>
    public int InvokeCustomExtraEffect(StatusManager targetStat, StatusManager sourceStat, AttackBase attackStat)
    {
        if (CheckConditional(targetStat, sourceStat))
        {
            var res = customEffectFunction((sourceStat,targetStat),attackStat);
            return res;
        }
        else
        {
            return 0;
        }
    }

    private bool CheckConditional(StatusManager targetStat, StatusManager sourceStat)
    {
        if (conditionType == ConditionType.TargetHasCondition)
        {
            foreach (var con in needCheckTargetConditionsList)
            {
                if (targetStat.GetConditionStackNumber((int)con) <= 0)
                    return false;
            }
        }else if (conditionType == ConditionType.Custom)
        {
            var result = customConditionFunc(sourceStat, targetStat);
            return result;
        }
        return true;
    }

    private void ParseArguments()
    {
        //parse conditional
        int index = 1;
        if (conditionType==(ConditionType.TargetHasCondition))
        {
            int conditionNum = int.Parse(args1[0]);
            for (int i = 0; i < conditionNum; i++)
            {
                int buffID = int.Parse(args1[i+index]);
                needCheckTargetConditionsList.Add((BasicCalculation.BattleCondition)(buffID));
            }
            index += conditionNum;
        }

        
        
        
        //parse effect
        if (extraEffect == ExtraEffect.ChangeDmgModifier)
        {
            extraModifier = ObjectExtensions.ParseInvariantFloat(args2[0]);
        }else if (extraEffect == ExtraEffect.ExtraCritRate)
        {
            extraCritRate = int.Parse(args2[0]);
        }


    }

}