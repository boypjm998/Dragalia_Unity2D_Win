
using UnityEngine;

public static class BasicCalculation
{
    public enum Affliction
    {
        None = 0,
        Burn = 1,
        Poison = 2,
        Paralyze = 3,
        Frostbite = 4
    }

    public enum AttackType
    {
        STANDARD = 1,
        DASH = 2,
        FORCE = 3,
        SKILL = 4,
        ABILITY = 5,
        OTHER = 6
    }

    public enum BattleCondition
    {
        //Basic Buff
        AtkBuff = 1,
        DefBuff = 2,
        CritRateBuff = 3,
        CritDmgBuff = 4,
        HotRecovery = 5,
        RecoveryBuff = 6,
        MaxHPBuff = 7,
        SkillDmgBuff = 8,
        SkillHasteBuff = 9,
        DamageCut = 10,
        DamageCutConst = 11,
        Shield = 12,
        LifeShield = 13,
        SPRegen = 14,
        
        BurnRes = 21,
        PoisonRes = 22,
        FrostbiteRes = 23,
        ParalysisRes = 24,

        ScorchrendRes = 25,
        StormlashRes = 26,
        FlashburnRes = 27,
        ShadowBlightRes = 28,
        
        StunRes = 31,
        SleepRes = 32,
        BogRes = 33,
        FreezeRes = 34,
        BlindnessRes = 35,

        //Special Buff
        AlchemicCatridge = 101,
        InfernoMode = 102,
        HolyFaith = 103,
        BlazewolfsRush = 104,

        //Basic Debuff
        AtkDebuff = 201,
        DefDebuff = 202,
        CritRateDebuff = 203,
        CritDmgDebuff = 204,
        RecoveryDebuff = 206,
        SkillDmgDebuff = 208,
        SkillHasteDebuff = 209,
        Vulnerable = 210,

        SPDegen = 214,


        //Special Debuff
        EvilsBane = 301,

        //Dot Affliction
        Burn = 401,
        Poison = 402,
        Frostbite = 403,
        Paralysis = 404,

        Scorchrend = 405,
        Stormlash = 406,
        Flashburn = 407,
        ShadowBlight = 408,

        //Control Affliction
        Stun = 411,
        Sleep = 412,
        Bog = 413,
        Freeze = 414,
        Blindness = 415,
        Cursed = 416,
        NoJump = 417,
        NoRoll = 418,
        
        Dispell = 999
        
    }

    public static string ConditionInfo_zh(BattleCondition cond)
    {
        switch (cond)
        {
            //Basic Conditions
            case BattleCondition.AtkBuff:
                return ("???????????????");
            case BattleCondition.AtkDebuff:
                return ("???????????????");
            case BattleCondition.DefBuff:
                return ("???????????????");
            case BattleCondition.DefDebuff:
                return ("???????????????");
            case BattleCondition.HotRecovery:
                return ("HP????????????");
            case BattleCondition.CritRateBuff:
                return ("???????????????");
            case BattleCondition.CritRateDebuff:
                return ("???????????????");
            case BattleCondition.CritDmgBuff:
                return ("??????????????????");
            case BattleCondition.CritDmgDebuff:
                return ("??????????????????");
            case BattleCondition.RecoveryBuff:
                return ("??????????????????");
            case BattleCondition.RecoveryDebuff:
                return ("??????????????????");
            case BattleCondition.SkillDmgBuff:
                return ("??????????????????");
            case BattleCondition.SkillDmgDebuff:
                return ("??????????????????");
            case BattleCondition.LifeShield:
                return ("????????????");
            case BattleCondition.DamageCut:
                return ("????????????");
            case BattleCondition.DamageCutConst:
                return ("????????????");
            case BattleCondition.Shield:
                return ("??????");
            case BattleCondition.SkillHasteBuff:
                return ("?????????????????????");
            case BattleCondition.SkillHasteDebuff:
                return ("?????????????????????");
            case BattleCondition.SPRegen:
                return ("?????????????????????");
            case BattleCondition.SPDegen:
                return ("?????????????????????");
            case BattleCondition.Vulnerable:
                return ("??????????????????");
            
            case BattleCondition.ScorchrendRes:
                return ("??????????????????");
            case BattleCondition.FlashburnRes:
                return ("??????????????????");
            case BattleCondition.BurnRes:
                return ("??????????????????");
            case BattleCondition.ShadowBlightRes:
                return ("??????????????????");
            case BattleCondition.ParalysisRes:
                return ("??????????????????");
            case BattleCondition.FrostbiteRes:
                return ("??????????????????");
            case BattleCondition.StormlashRes:
                return ("??????????????????");
            case BattleCondition.PoisonRes:
                return ("??????????????????");
            case BattleCondition.FreezeRes:
                return ("??????????????????");
            case BattleCondition.StunRes:
                return ("??????????????????");
            case BattleCondition.SleepRes:
                return ("??????????????????");
            case BattleCondition.BlindnessRes:
                return ("??????????????????");

            //Special buffs:
            case BattleCondition.AlchemicCatridge:
                return ("??????????????????");
            case BattleCondition.InfernoMode:
                return ("????????????");
            case BattleCondition.HolyFaith:
                return ("????????????");
            case BattleCondition.BlazewolfsRush:
                return ("????????????");

            //Special debuffs:
            case BattleCondition.EvilsBane:
                return ("????????????");

            //Afflictions
            case BattleCondition.Flashburn:
                return ("??????");
            case BattleCondition.Scorchrend:
                return ("??????");
            case BattleCondition.Burn:
                return ("??????");
            case BattleCondition.Blindness:
                return ("??????");
            case BattleCondition.ShadowBlight:
                return ("??????");
            case BattleCondition.Frostbite:
                return ("??????");
            case BattleCondition.Freeze:
                return ("??????");
            case BattleCondition.Stun:
                return ("??????");
            case BattleCondition.Sleep:
                return ("??????");
            case BattleCondition.Bog:
                return ("??????");
            case BattleCondition.Paralysis:
                return ("??????");
            case BattleCondition.Poison:
                return ("??????");
            case BattleCondition.Stormlash:
                return ("??????");
            case BattleCondition.Cursed:
                return ("??????");


            default:
            {
                Debug.LogWarning("Buff text not found");
                return ("");
            }
        }
    }
    

    


    public static float BattleConditionLimit(int id)
    {
        switch (id)
        {
            case 1:
                return 200;
            case 2:
                return 200;
            case 3:
                return 200;
            default:
                return 999;
        }
    }

    public static int MAXCONDITIONSTACKNUMBER = 100;

    public static float DistanceBetween2Object2D(Transform A, Transform B)
    {
        return Mathf.Sqrt(A.position.x * B.position.x - A.position.y - B.position.y);
    }

    public static float DistanceBetween2Object2D(Vector3 A, Vector3 B)
    {
        return Mathf.Sqrt(A.x * A.x - B.x - B.x);
    }

    public static int CalculateDamageGeneral(StatusManager sourceStat, StatusManager targetStat, float modifier,
        AttackType atkType, ref bool isCrit)
    {
        //Source

        //?????? Attack

        var atk = sourceStat.baseAtk * (1 + sourceStat.attackBuff);

        //?????? ?????? crit


        var critRate = sourceStat.critRate + sourceStat.critRateBuff;
        float critDmgModifier = 1;
        //isCrit = false;
        if (Random.Range(0, 100) < critRate)
        {
            isCrit = true;
            critDmgModifier += 0.7f + sourceStat.critDmgBuff;
        }

        float skillDmgModifier = 1;
        if (atkType == AttackType.SKILL) skillDmgModifier += sourceStat.skillDmgBuff;

        //Target

        var tarDef = targetStat.baseDef * (1 + targetStat.GetDefenseBuff());
        var dmgCutModifier = targetStat.GetDamageCut();
        var dmgCutConst = targetStat.GetDamageCutConst();


        //Calculate

        var attackSource = atk * skillDmgModifier * critDmgModifier * modifier;
        var defendTarget = tarDef * (1 - dmgCutModifier);

        var damage = 5f / 3f * (attackSource / defendTarget) - dmgCutConst;

        if (damage < 0) damage = 0;

        //Debug.Log(damage);
        return (int)damage;
    }

    public static int CalculateDamagePlayer(StatusManager sourceStat, StatusManager targetStat, float modifier,
        AttackFromPlayer atkStat, ref bool isCrit)
    {
        //Source

        //?????? Attack

        var atk = sourceStat.baseAtk * (1 + sourceStat.attackBuff);

        //?????? ?????? crit


        var critRate = sourceStat.critRate + sourceStat.critRateBuff;

        critRate += CheckSpecialCritEffect(sourceStat,targetStat,atkStat);
        
        float critDmgModifier = 1;
        //isCrit = false;
        if (Random.Range(0, 100) < critRate)
        {
            isCrit = true;
            critDmgModifier += 0.7f + sourceStat.critDmgBuff;
            critDmgModifier += CheckSpecialCritDmgEffect(sourceStat, targetStat, atkStat);
        }

        float skillDmgModifier = 1;
        if (atkStat.attackType == AttackType.SKILL) skillDmgModifier += sourceStat.skillDmgBuff;

        //Target

        var tarDef = targetStat.baseDef * (1 + targetStat.GetDefenseBuff());
        var dmgCutModifier = targetStat.GetDamageCut();
        var dmgCutConst = targetStat.GetDamageCutConst();


        //Calculate

        var attackSource = atk * skillDmgModifier * critDmgModifier * modifier;
        var defendTarget = tarDef * (1 + dmgCutModifier);

        var damage = 5f / 3f * (attackSource / defendTarget) - dmgCutConst;

        if (damage < 0) damage = 0;

        //Debug.Log(damage);
        return (int)damage;
    }
    public static int CalculateDamageEnemy(StatusManager sourceStat, StatusManager targetStat, float modifier,
        AttackFromEnemy atkStat, ref bool isCrit)
    {
        //Source

        //?????? Attack

        var atk = sourceStat.baseAtk * (1 + sourceStat.attackBuff);

        //?????? ?????? crit


        var critRate = sourceStat.critRate + sourceStat.critRateBuff;

        critRate += CheckSpecialCritEffect(sourceStat,targetStat,atkStat);
        
        float critDmgModifier = 1;
        //isCrit = false;
        if (Random.Range(0, 100) < critRate)
        {
            isCrit = true;
            critDmgModifier += 0.7f + sourceStat.critDmgBuff;
            critDmgModifier += CheckSpecialCritDmgEffect(sourceStat, targetStat, atkStat);
        }

        float skillDmgModifier = 1;
        if (atkStat.attackType == AttackType.SKILL) skillDmgModifier += sourceStat.skillDmgBuff;

        //Target

        var tarDef = targetStat.baseDef * (1 + targetStat.GetDefenseBuff());
        var dmgCutModifier = targetStat.GetDamageCut();
        var dmgCutConst = targetStat.GetDamageCutConst();


        //Calculate

        var attackSource = atk * skillDmgModifier * critDmgModifier * modifier;
        var defendTarget = tarDef * (1 + dmgCutModifier);

        var damage = 5f / 3f * (attackSource / defendTarget) - dmgCutConst;

        if (damage < 0) damage = 0;

        //Debug.Log(damage);
        return (int)damage;
    }
    



    /// <summary>
    /// HP????????????
    /// </summary>
    /// <param name="modifier">????????????</param>
    /// <param name="percentageModifier">?????????????????????</param>
    /// <returns></returns>
    public static int CalculateHPRegenGeneral(StatusManager targetStat, float modifier, float percentageModifier)
    {
        var atk = targetStat.baseAtk * (1 + targetStat.attackBuff);
        var hp = targetStat.maxHP;
        var potencybuff = targetStat.recoveryPotencyBuff;

        var damagePart1 = (0.16 * hp + 0.06 * atk) * modifier * (1 + potencybuff) * 0.012f;

        var damagePart2 = hp * percentageModifier * 0.01 * (1 + potencybuff);

        return (int)(damagePart1 + damagePart2);
    }

    #region CheckSpecialEffect

    

    
    public static int CheckSpecialCritEffect(StatusManager sourceStat, StatusManager targetStat, 
        AttackFromPlayer attackStat)
    {
        int extraCritModifier = 0;
        switch (attackStat.chara_id)
        {
            case 5:
            {
                if (attackStat.skill_id == 2 &&
                    targetStat.GetConditionStackNumber((int)BattleCondition.EvilsBane) > 0)
                {
                    extraCritModifier = 100;
                }
                break;
            }
        }
        
        
        
        
        return extraCritModifier;
    }
    public static int CheckSpecialCritEffect(StatusManager sourceStat, StatusManager targetStat, 
        AttackFromEnemy attackStat)
    {
        int extraCritModifier = 0;
        switch (attackStat.chara_id)
        {
            case 1:
            {
                if (attackStat.skill_id == 2 &&
                    targetStat.GetConditionStackNumber((int)BattleCondition.EvilsBane) > 0)
                {
                    extraCritModifier = 100;
                }
                break;
            }
        }
        
        
        
        
        return extraCritModifier;
    }

    public static float CheckSpecialCritDmgEffect(StatusManager sourceStat, StatusManager targetStat,
        AttackFromPlayer attackStat)
    {
        float extraCritDmgModifier = 0;
        switch (attackStat.chara_id)
        {
            case 5:
            {
                if (attackStat.attackType == AttackType.STANDARD &&
                    targetStat.GetConditionStackNumber((int)BattleCondition.EvilsBane) > 0)
                {
                    extraCritDmgModifier += 0.2f;
                }
                break;
            }
            default: break;
        }

        return extraCritDmgModifier;
    }
    public static float CheckSpecialCritDmgEffect(StatusManager sourceStat, StatusManager targetStat,
        AttackFromEnemy attackStat)
    {
        float extraCritDmgModifier = 0;
        switch (attackStat.chara_id)
        {
            case 1:
            {
                if (attackStat.attackType == AttackType.STANDARD &&
                    targetStat.GetConditionStackNumber((int)BattleCondition.EvilsBane) > 0)
                {
                    extraCritDmgModifier += 0.2f; 
                }
                break;
            }
            default: break;
        }

        return extraCritDmgModifier;
    }
    
    #endregion



    public static float CalculateAttackInfo(StatusManager stat)
    {
        //stat.baseAtk * (1 + stat.attackBuff);
        return stat.baseAtk * (1 + stat.attackBuff);
    }

    public static float CalculateDefenseInfo(StatusManager stat)
    {
        return stat.baseDef * (1 + stat.defenseBuff);
        
    }

    private enum CharacterInfo
    {
        Ilia = 1,
        Ezelith = 2,
        Zena = 3,
        Renelle = 4,
        Sheila = 5
    }

    public enum KnockBackType
    {
       None = 0, //No Knockback Distance
       FaceDirection = 1, 
       FromCenterRay = 2, //The ray from the attack center to target
       FromCenterFixed = 3, //Knockback target in a fixed direction related to the center position
       FixedDirection = 4 //A Fixed Direction
       //????????????Fixed Vector???(1,0)????????????????????????????????????-1,0????????????????????????????????????(1,0)???
    }
    
    
    
    
    
    
    
    public static float ParticleSystemLength(ParticleSystem ps)
    {
        float maxDuration = 0;

        var main = ps.main;
        return main.startLifetime.constant + main.duration;
        
        //if(ps.enableEmission){
        //    if(ps.loop){
        //        return -1f;
        //    }
        //    float dunration = 0f;
        //    if(ps.emissionRate <=0){
        //        dunration = ps.startDelay + ps.startLifetime;
        //    }else{
        //        dunration = ps.startDelay + Mathf.Max(ps.duration,ps.startLifetime); 
        //    }
        //    if (dunration > maxDuration) {
        //        maxDuration = dunration;
        //    }
        //}
        return maxDuration;
    }

    /// <summary>
    /// ???????????????????????????????????????????????????
    /// </summary>
    /// <returns></returns>
    public static float GetLastAnimationNormalizedTime(Animator anim)
    {
        float totalFrame = (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length*anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
        return (1 - 1 / totalFrame);
    }
    
    

}