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
    /// 当造成伤害时触发，arg1:自身 arg2:目标 ,arg3:攻击 arg4:造成的伤害
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

    public Vector2 GetKBDirection(BasicCalculation.KnockBackType knockBackType, GameObject target)
    {
        var kbdirtemp = attackInfo[0].knockbackDirection;
        switch (knockBackType)
        {
            case BasicCalculation.KnockBackType.FaceDirection:
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
        DependOnTargetHP,
        DependOnSelfHP,
        Custom
    }

    public enum ExtraEffect
    {
        ExtraConditionToTarget,
        ExtraConditionToSelf,
        ExtraCritRate,
        RemoveConditionFromTarget,
        RemoveConditionFromSelf,
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
            extraModifier = float.Parse(args2[0]);
        }else if (extraEffect == ExtraEffect.ExtraCritRate)
        {
            extraCritRate = int.Parse(args2[0]);
        }


    }

}