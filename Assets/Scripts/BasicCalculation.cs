using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public static class BasicCalculation
{
    public static float DistanceBetween2Object2D(Transform A, Transform B)
    {
        return Mathf.Sqrt(A.position.x * B.position.x - A.position.y - B.position.y);
    }
    public static float DistanceBetween2Object2D(Vector3 A, Vector3 B)
    {
        return Mathf.Sqrt(A.x * A.x - B.x - B.x);
    }
    enum CharacterInfo
    {
        Ilia = 1,
        Ezelith = 2,
        Zena = 3,
        Renelle = 4
    }
    public enum Affliction 
    {
        None = 0,
        Burn = 1,
        Poison = 2,
        Paralyze = 3,
        Frostbite = 4
    }

    public struct BasicAttackInfo
    {
        public float kbpower;
        public float kbforce;
        public float kbtime;
        public float[] dmgmod;

        

    }

    public enum AttackType {
        STANDARD = 1,
        DASH = 2,
        FORCE = 3,
        SKILL = 4,
        ABILITY = 5,
        OTHER = 6
    }
    
    public enum BattleBuff
    {
        //Basic Buff
        AtkBuff = 1,
        DefBuff = 2,
        CritRateBuff = 3,
        CritDmgBuff = 4,
        HotRecovery = 5,
        RecoveryBuff = 6,
        
        //Basic Debuff
        AtkDebuff = -1,
        DefDebuff = -2,
        CritRateDebuff = -3,
        CritDmgDebuff = -4,
        RecoveryDebuff = -6,
        
        //Special Buff
        AlchemicCatridge = 101,
        
        //Special Debuff
        
        //Dot Affliction
        Burn = 301,
        Poison = 302,
        Frostbite = 303,
        Paralysis = 304,
        
        Scorchrend = 305,
        Stormlash = 306,
        Flashburn = 307,
        ShadowBlight = 308,
        
        //Control Affliction
        Stun = 311,
        Sleep = 312,
        Bog = 313,
        Freeze = 314,
        Blindness = 315,
        Cursed = 316,
        NoJump = 317,
        NoRoll = 318,
        

    }

    public static int CalculateDamageGeneral(StatusManager sourceStat, StatusManager targetStat, float modifier, AttackType atkType, ref bool isCrit)
    {
        //Source
        //¹¥»÷
        float atk = sourceStat.baseAtk * (1 + sourceStat.GetAttackBuff());

        //±©»÷ ±¬ÉË

        int critRate = sourceStat.critRate + sourceStat.GetCritRateBuff();
        float critDmgModifier = 1;
        isCrit = false;
        if (Random.Range(0, 100) < critRate)
        {
            isCrit = true;
            critDmgModifier += 0.7f + sourceStat.GetCritDamageBuff();
        }

        float skillDmgModifier = 1;
        if (atkType == AttackType.SKILL)
        {
            skillDmgModifier += sourceStat.GetSkillDamageBuff();

        }
        
        //Target
        float tarDef = targetStat.baseDef * (1 + targetStat.GetDefenseBuff());
        float dmgCutModifier = targetStat.GetDamageCut();
        float dmgCutConst = targetStat.GetDamageCutConst();





        //Calculate

        float attackSource = atk * skillDmgModifier * critDmgModifier * modifier;
        float defendTarget = tarDef * (1 - dmgCutModifier);

        float damage = ((5f / 3f) * (attackSource / defendTarget)) - dmgCutConst;

        if (damage < 0)
        {
            damage = 0;
        }

        //Debug.Log(damage);
        return (int)damage;

    }


  
}
