using UnityEngine;
using UnityEditor;

/// <summary>
/// 静态类，包含了所有的敌人行为类型
/// </summary>
public class DragaliaEnemyActionTypes
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

    public enum HB1003L
    {
        AlchemicEnhancement,
        AlchemicShield,
        Arrow,
        AstralStream,
        AstralSurge,
        DoomTempest,
        DriveBuster,
        Minion,
        OtherworldGate,
        VerticalArrow,
        SetWorld,
        StormShield
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
        glare,
        forward,
        shared,
        setWorld,
        prayers,
        ring,
        rainbows,
        squares,
        pillars,
        heal,
        genesis
    }
    
    public enum HB1005
    {
        ComboA,
        ComboB,
        CocytusWhirl,
        ConquestEvil,
        AcheronFount,
        IceBlast,
        Dragondrive,
        Warp,
        Fog,
        IcePillar,
        SnowStorm,
        IceBreaker,
        Buff,
        Platform,
        SetWorld,
        SmashDown,
        ComboC,
        Forward,
        Pact,
        Meteor,
        Around,
        Cascade,
        Ultimate,
        WaterSpout,
        Perish,
        GroundFrost
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

    public enum DB2002
    {
        Claw,
        Pillar,
        Inferno,
        Fireball,
        Breathe,
        Whirlwind,
        Chaser,
        Muspelheim,
        Shield,
        Memories,
        Summon
    }
    
    /// <summary>
    /// Primal Jupiter
    /// </summary>
    public enum DB2004
    {
        around,
        blast,
        bolt,
        claw,
        dash,
        dual,
        memory1,
        memory2,
        memory3,
        memory4,
        random,
        shells,
        summon1,
        summon2,
        summon3,
        summon4,
        summon5,
        summon6,
        sweep,
        sweep_double,
        twist
    }
    
    /// <summary>
    /// Primal Zodiark
    /// </summary>
    public enum DB2005
    {
        Around,
        Blast,
        Chaser,
        Claw,
        CursedFlame,
        CursedFlame2,
        GroundFire,
        Memories,
        PoisonSide,
        PoisonFront,
        Spit,
        Sprint,
        Summon,
        Tail
    }

    /// <summary>
    /// 堕天使加百列
    /// </summary>
    public enum DB2011
    {
        summon_soldier,
        summon_monster,
        around,
        multi_around,
        charge,
        corrosion,
        buff,
        prison,
        target_pillar,
        platform_splash
    }

    public enum DB2012
    {
        corrosion,
        nihil,
        sphere,
        chasing,
        buff,
        spike,
        slap,
        multi_dash,
        pizza,
        jalapeno
    }
    
    /// <summary>
    /// 堕天使拉斐尔
    /// </summary>
    public enum DB2013
    {
        combo,
        free,
        ground,
        infight,
        mine,
        nihil,
        straight,
        orbs,
        orbs_tut,
        weak_point

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
        buff,
        scatter,
        sphere,
        cascade,
        whirl
    }
    /// <summary>
    /// 堕天使米迦勒
    /// </summary>
    public enum DB2015
    {
        crystal_fixed,
        crystal_chase,
        crystal_mixed,
        around,
        buff,
        combo,
        fireball,
        nihil,
        wave,
        cross,
        explosion
    }
    
    public enum DB2015V
    {
        Combo,
        Buff,
        AngeticWind,
        Spike,
        Throw,
        Shield
    }

    /// <summary>
    /// agni
    /// </summary>
    public enum DB2016
    {
        Dash,
        FrontStrike,
        ClawAttack,
        Devastation
    }
    
    
    //Jaldabaoth
    public enum H001
    {
        TargetingPillar,
        SummonChild,
        SummonElite,
        Buff,
        Laser,
        BouncingOrb,
        ChasingPillar,
        Corrosion,
        HealOnebyOne,
        HealSimultaneously,
        Executioners,
        AbsoluteLaw,
        ElementSwitch
    }
    
    
    //Lilith
    public enum H002
    {
        Nihil,
        Corrosion,
        Buff,
        Combo,
        Rush,
        Smash,
        TargetingCandies,
        CrossCandies,
        SweetStockade,
        CombinedRush,
        WandGroup,
        BounceCandies,
        GroundBurst,
        JamSpin
    }
    
    /// <summary>
    /// Asura
    /// </summary>
    public enum H003
    {
        Nihil,
        Orbs,
        Mine,
        Wave,
        Around,
        /// 三业往生
        Line,
        Pizza,
        Suppression,
        WeakPoint,
        Face,
        Infight,
        Ruin,
        Thunder,
        BalancePrepare,
        Balance
    }

    public enum H004
    {
        Melody,
        Nihil,
        Corrosion,
        Forward,
        Rings,
        Encore,
        Buff,
        Scatter,
        Shuffle,
        DpsCheck,
        Follow,
        Echo,
        MultiWay,
        Targeting,
        WarpAttack,
        Teleport,
        Dissonance,
        Fan,
        Cross
    }

    public enum H005
    {
        Nihil,
        Corrosion,
        BurningOn,
        PhoenixOn,
        Wave,
        TargetingCrystal,
        Around,
        Cross,
        Chaser,
        Refraction,
        MixedCrystal,
        StoneGroup,
        LavaCarpet,
        LavaTsunami,
        Explosion,
        CrystalInferno,
        MovingBlocks,
        CrystalBreaker
        
        
    }

    public enum HECommon
    {
        swd_1,
        swd_2,
        swd_3,
        axe_1,
        axe_2,
        axe_3,
        lan_hi_1,
        lan_hi_2,
        lan_hi_3,
        lan_hi_4,
        lan_hi_5,
        rod_1,
        rod_2,
        rod_3,
        fx_wroth,
        summon_1,
        phantom_1,
        phantom_2
    }

    public enum Goblin
    {
        slash,
        smash,
        around,
        buff
    }
    
    public enum Gobmancer
    {
        target,
        straight
    }




}
