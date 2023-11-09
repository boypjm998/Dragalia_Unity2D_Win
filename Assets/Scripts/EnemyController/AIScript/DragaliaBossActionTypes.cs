using UnityEngine;
using UnityEditor;

public class DragaliaBossActionTypes
{
    public enum Default
    {
        None
    }
    public enum HB1001
    {
        ComboA,
        ComboB,
        ComboC,
        ComboD,
        WarpAttack,
        DodgeAttack,
        DashAttack,
        CarmineRush,
        FlameRaid,
        BrightCarmineRush,
        SavageFlameRaid,
        Inferno,
        Heaven,
        Buff,
        ScorArea,
        UpdateBuff,
        SetWorld,
        Mirage,
        Counter,
        Charge,
        AOE,
        DualFlame
    }
    public enum HB1002
    {
        KeepDistance,
        ApproachTarget,
        ComboA,
        ComboB,
        ComboC,
        DashAttack,
        GloriousSanctuary,
        HolyCrown,
        TwilightCrown,
        CelestialPrayer,
        WarpAttack,
        FaithEnhancement,
        EarthBarrier,
        CombinedTwilightAttack,
        PhaseShift,
        SpinDash,
        SpinDashFast,
        SummonOrbs,
        GloriousSanctuaryG,
        GloriousSanctuaryC,
        PickUpBuff,
        ReflectionOn,
        BusterOn,
        WorldReset,
        CelestialPrayerF,
        CelestialPrayerG,
        HolyCrownG,
        BackWarpSmash,
        BackWarpGround,
        GalaxyPrayerOn,
        GalaxyPrayerOff,
        GenesisCrown,
        GenesisCrownSingle

    }

    public enum HB1003
    {
        free,
        buff,
        summon,
        force,
        gate
    }

    public enum HB1003M1
    {
        Combo,
        ForceStrike,
        GenesisCirclet
    }
    
    public enum HB1004
    {
        comboA,
        comboB,
        blazingFount,
        affectionRing,
        warp,
        wall,
        buff,
        projectiles,
        celestial,
        ball,
        glare
    }

    public enum DB2001
    {
        forward,
        combo,
        around,
        roar,
        tornado,
        wallFixed,
        wallChasing,
        bounce,
        locked,
        pillar,
        multi,
        golem,
        launcher,
        upward,
        ground,
        sky,
        leif,
        lathna,
        melsa,
        tobias,
        gale,
        meene
    }

    /// <summary>
    /// 堕天使加百列
    /// </summary>
    public enum DB2011
    {
        summon_soldier,
        around,
        multi_around,
        charge,
        corrosion,
        buff
    }
    /// <summary>
    /// 堕天使乌列
    /// </summary>
    public enum DB2014
    {
        water_jet,
        around,
        dash,
        slap,
        corrosion,
        buff
    }
    /// <summary>
    /// 堕天使米迦勒
    /// </summary>
    public enum DB2015
    {
        crystal_fixed,
        crystal_chase,
        around,
        buff,
        combo,
        fireball,
        nihil
    }

    public enum HECommon
    {
        swd_1,
        swd_2,
        swd_3,
        rod_1,
        rod_2,
        rod_3,
        fx_wroth
    }




}
