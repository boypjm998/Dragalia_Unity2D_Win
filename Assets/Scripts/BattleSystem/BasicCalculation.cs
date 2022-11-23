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
        SkillDamageBuff = 8,

        //Special Buff
        AlchemicCatridge = 101,


        //Basic Debuff
        AtkDebuff = 201,
        DefDebuff = 202,
        CritRateDebuff = 203,
        CritDmgDebuff = 204,
        RecoveryDebuff = 206,
        SkillDamageDebuff = 208,


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
        NoRoll = 318
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
        isCrit = false;
        if (Random.Range(0, 100) < critRate)
        {
            isCrit = true;
            critDmgModifier += 0.7f + sourceStat.critDmgBuff;
        }

        float skillDmgModifier = 1;
        if (atkType == AttackType.SKILL) skillDmgModifier += sourceStat.skillDmgBuff;

        //Target
        Debug.Log(targetStat);
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

    public static int CalculateHPRegenGeneral(StatusManager targetStat, float modifier, float percentageModifier)
    {
       
        var atk = targetStat.baseAtk *(1+targetStat.attackBuff);
        var hp = targetStat.maxHP;
        var potencybuff = targetStat.recoveryPotencyBuff;

        var damagePart1 = (0.16 * hp + 0.06 * atk) * modifier * (1 + potencybuff) * 0.012f;

        var damagePart2 = hp * percentageModifier * 0.01 * (1 + potencybuff);

        return (int)(damagePart1 + damagePart2);
    }

    private enum CharacterInfo
    {
        Ilia = 1,
        Ezelith = 2,
        Zena = 3,
        Renelle = 4,
        Sheila = 5
    }

    public struct BasicAttackInfo
    {
        public float kbpower;
        public float kbforce;
        public float kbtime;
        public float[] dmgmod;
    }
}