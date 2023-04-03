using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameMechanics;
public abstract class AttackBase : MonoBehaviour
{
    
    public static readonly int DEFAULT_GRAVITY = 4;
    
    public int chara_id;
    public int skill_id = 0;
    
    public List<AttackInfo> attackInfo = new();
    public int firedir;//要改掉
    
    public BasicCalculation.AttackType attackType;
    
    // [SerializeField] protected float[] dmgModifier;
    // [SerializeField] protected List<float> nextDmgModifier;
    // [SerializeField] protected List<float> nextKnockbackPower;
    // [SerializeField] protected List<float> nextKnockbackForce;
    // [SerializeField] protected List<float> nextKnockbackTime;
    // 
    // public float knockbackPower = 100;
    // public float knockbackForce;
    // public float knockbackTime;
    // public Vector2 knockbackDirection = Vector2.right;
    // public BasicCalculation.KnockBackType KBType;

    [SerializeField] protected AudioClip hitSoundEffect;
    protected BattleEffectManager _effectManager;
    protected void DestroyContainer()
    {
        var container = GetComponentInParent<AttackContainer>();
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
        {
            return attackInfo[0].constDmg[id];
        }else
        {
            return 0;
        }
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

}

[Serializable]
public class AttackInfo
{
    public class ConditionWithAttackInfo
    {
        //public int conditionID = -1;
        public int identifier; //用于标记这个异常状态的唯一ID，以便于检测是否已经检测过。内部ID，不会和外部冲突。
        public BattleCondition condition;
        public int withConditionChance;
        
        public ConditionWithAttackInfo()
        {
        }

    }
    
    public List<float> dmgModifier;
    public List<float> constDmg;
    public float knockbackPower;
    public float knockbackForce;
    public float knockbackTime;
    public Vector2 knockbackDirection = Vector2.right;
    public BasicCalculation.KnockBackType KBType;
    public List<ConditionWithAttackInfo> withConditions;
    public int firedir;

    public void AddWithCondition(BattleCondition condition, int chance = 100,int identifier = 0)
    {
        var conditionWithAttackInfo = new ConditionWithAttackInfo();
        conditionWithAttackInfo.condition = condition;
        conditionWithAttackInfo.withConditionChance = chance;
        conditionWithAttackInfo.identifier = identifier;
        withConditions.Add(conditionWithAttackInfo);
    }
    
    

    public AttackInfo()
    {
        dmgModifier = new List<float>();
        constDmg = new List<float>();
        withConditions = new List<ConditionWithAttackInfo>();
    }
    

}