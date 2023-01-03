
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
                return ("攻击力提升");
            case BattleCondition.AtkDebuff:
                return ("攻击力下降");
            case BattleCondition.DefBuff:
                return ("防御力提升");
            case BattleCondition.DefDebuff:
                return ("防御力下降");
            case BattleCondition.HotRecovery:
                return ("HP持续回复");
            case BattleCondition.CritRateBuff:
                return ("暴击率提升");
            case BattleCondition.CritRateDebuff:
                return ("暴击率下降");
            case BattleCondition.CritDmgBuff:
                return ("暴击威力提升");
            case BattleCondition.CritDmgDebuff:
                return ("暴击威力下降");
            case BattleCondition.RecoveryBuff:
                return ("回复效果提升");
            case BattleCondition.RecoveryDebuff:
                return ("回复效果下降");
            case BattleCondition.SkillDmgBuff:
                return ("技能伤害提升");
            case BattleCondition.SkillDmgDebuff:
                return ("技能伤害下降");
            case BattleCondition.LifeShield:
                return ("生命护盾");
            case BattleCondition.DamageCut:
                return ("伤害减少");
            case BattleCondition.DamageCutConst:
                return ("伤害减少");
            case BattleCondition.Shield:
                return ("护盾");
            case BattleCondition.SkillHasteBuff:
                return ("技能槽获取提升");
            case BattleCondition.SkillHasteDebuff:
                return ("技能槽获取下降");
            case BattleCondition.SPRegen:
                return ("技能槽持续提升");
            case BattleCondition.SPDegen:
                return ("技能槽持续下降");
            case BattleCondition.Vulnerable:
                return ("所受伤害增加");
            
            case BattleCondition.ScorchrendRes:
                return ("劫火抗性提升");
            case BattleCondition.FlashburnRes:
                return ("闪热抗性提升");
            case BattleCondition.BurnRes:
                return ("烧伤抗性提升");
            case BattleCondition.ShadowBlightRes:
                return ("暗殇抗性提升");
            case BattleCondition.ParalysisRes:
                return ("麻痹抗性提升");
            case BattleCondition.FrostbiteRes:
                return ("冻伤抗性提升");
            case BattleCondition.StormlashRes:
                return ("裂风抗性提升");
            case BattleCondition.PoisonRes:
                return ("中毒抗性提升");
            case BattleCondition.FreezeRes:
                return ("冰冻抗性提升");
            case BattleCondition.StunRes:
                return ("昏迷抗性提升");
            case BattleCondition.SleepRes:
                return ("睡眠抗性提升");
            case BattleCondition.BlindnessRes:
                return ("黑暗抗性提升");

            //Special buffs:
            case BattleCondition.AlchemicCatridge:
                return ("炼金弹夹装填");
            case BattleCondition.InfernoMode:
                return ("地狱模式");
            case BattleCondition.HolyFaith:
                return ("圣洁信念");
            case BattleCondition.BlazewolfsRush:
                return ("巫女气焰");

            //Special debuffs:
            case BattleCondition.EvilsBane:
                return ("破邪巫咒");

            //Afflictions
            case BattleCondition.Flashburn:
                return ("闪热");
            case BattleCondition.Scorchrend:
                return ("劫火");
            case BattleCondition.Burn:
                return ("烧伤");
            case BattleCondition.Blindness:
                return ("黑暗");
            case BattleCondition.ShadowBlight:
                return ("暗殇");
            case BattleCondition.Frostbite:
                return ("冻伤");
            case BattleCondition.Freeze:
                return ("冰冻");
            case BattleCondition.Stun:
                return ("昏迷");
            case BattleCondition.Sleep:
                return ("睡眠");
            case BattleCondition.Bog:
                return ("湿身");
            case BattleCondition.Paralysis:
                return ("麻痹");
            case BattleCondition.Poison:
                return ("中毒");
            case BattleCondition.Stormlash:
                return ("裂风");
            case BattleCondition.Cursed:
                return ("诅咒");


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

        //攻击 Attack

        var atk = sourceStat.baseAtk * (1 + sourceStat.attackBuff);

        //暴击 爆伤 crit


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

        //攻击 Attack

        var atk = sourceStat.baseAtk * (1 + sourceStat.attackBuff);

        //暴击 爆伤 crit


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

        //攻击 Attack

        var atk = sourceStat.baseAtk * (1 + sourceStat.attackBuff);

        //暴击 爆伤 crit


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
    /// HP回复计算
    /// </summary>
    /// <param name="modifier">回血系数</param>
    /// <param name="percentageModifier">百分比回血系数</param>
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
       //比如这个Fixed Vector是(1,0)那么左边的敌人就会收到（-1,0）的击退方向，右边的则是(1,0)。
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
    /// 获取当前动画的最后一帧的标准时间。
    /// </summary>
    /// <returns></returns>
    public static float GetLastAnimationNormalizedTime(Animator anim)
    {
        float totalFrame = (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length*anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
        return (1 - 1 / totalFrame);
    }
    
    

}