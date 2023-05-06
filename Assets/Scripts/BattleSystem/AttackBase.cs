using System;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public abstract class AttackBase : MonoBehaviour
{
    public static readonly int DEFAULT_GRAVITY = 4;

    public int chara_id;
    public int skill_id;

    public List<AttackInfo> attackInfo = new();
    public int firedir; //要改掉

    public BasicCalculation.AttackType attackType;


    [SerializeField] protected AudioClip hitSoundEffect;
    [SerializeField] protected AudioClip[] attackSE;

    protected BattleEffectManager _effectManager;
    
    public List<ConditionalAttackEffect> conditionalAttackEffects = new();

    protected void DestroyContainer()
    {
        var container = GetComponentInParent<AttackContainer>();
        if (container != null)
            Destroy(container.gameObject);
        print("DestroyContainer");
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
                kbdirtemp = firedir * kbdirtemp;
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

public class ConditionalAttackEffect
{
    public enum ConditionType
    {
        TargetHasCondition,
        DependOnTargetHP,
        DependOnSelfHP
    }

    public enum ExtraEffect
    {
        ExtraConditionToTarget,
        ExtraConditionToSelf,
        RemoveConditionFromTarget,
        RemoveConditionFromSelf,
        ChangeDmgModifier,
        CrisisModifier
    }
    
    public List<ConditionType> conditionType = new();
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

    private Tuple<BattleCondition,int> needAppendSelfCondition;
    private Tuple<BattleCondition,int> needAppendTargetCondition;

    private Tuple<BattleCondition,int> needRemoveSelfCondition;
    private Tuple<BattleCondition,int> needRemoveTargetCondition;

    private float extraModifier;
    /// <summary>
    /// <para>(minHP(0,1), maxHP(0,1), crisisModifier)</para>
    /// <para>crisisModifier背水系数，在minHP处，倍率为原倍率*crisisModifier,在maxHP处，倍率为原倍率，背水曲线为抛物线。</para>
    /// </summary>
    private Tuple<float, float, float> crisisModifier;


    # endregion
    
    

    

    public ConditionalAttackEffect(List<ConditionType> conditionType, ExtraEffect extraEffect, string[] args1, string[] args2)
    {
        this.conditionType = conditionType;
        this.extraEffect = extraEffect;
        this.args1 = args1;
        this.args2 = args2;
        ParseArguments();
    }
    

    /// <param name="args1">格式1:{检查几个buff,buffID1,buffID2,...}</param>
    /// <param name="args2">格式1:{附加倍率}</param>
    public ConditionalAttackEffect(ConditionType conditionType, ExtraEffect extraEffect, string[] args1, string[] args2)
    {
        this.conditionType.Add(conditionType);
        this.extraEffect = extraEffect;
        this.args1 = args1;
        this.args2 = args2;
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

    private bool CheckConditional(StatusManager targetStat, StatusManager sourceStat)
    {
        foreach (var conditional in conditionType)
        {
            if (conditional == ConditionType.TargetHasCondition)
            {
                foreach (var con in needCheckTargetConditionsList)
                {
                    if (targetStat.GetConditionStackNumber((int)con) <= 0)
                        return false;
                }
            }
            else
            {
                continue;
            }
        }

        return true;
    }

    private void ParseArguments()
    {
        //parse conditional
        int index = 1;
        if (conditionType.Contains(ConditionType.TargetHasCondition))
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
        }


    }

}