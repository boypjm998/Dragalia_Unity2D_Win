using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using GameMechanics;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameMechanics
{
    public static class Ability
    {


        public static void GetEffectFunc(this StatusManager self, int abilityID)
        {
            
            switch (abilityID)
            {
                case 10002://暴击输出-伊莉雅
                {
                    self.SpecialCritDamageEffectFunc += Ability_CritDamage_10002;
                    break;
                }
                case 10004://防御力下降特效-艾赛莉特
                {
                    self.SpecialPunisherEffectFunc += Ability_Punisher_10004;
                    break;
                }
                case 10006://巫女之祈愿
                {
                    self.SpecialAttackEffectFunc += Ability_Attack_10006;
                    self.SpecialCritEffectFunc += Ability_CritRate_10006;
                    break;
                }
                case 10008://疾风怒涛会心
                {
                    self.SpecialCritEffectFunc += Ability_CritRate_10008;
                    break;
                }
                case 10009://闪狼战技/绯红幻影
                {
                    self.SpecialCritEffectFunc += Ability_CritRate_10009;
                    self.SpecialCritDamageEffectFunc += Ability_CritDamage_10009;
                    self.SpecialDamageCutEffectFunc += Ability_DamageCut_10009;
                    break;
                }
                case 10011://疾风怒涛攻
                {
                    self.SpecialAttackEffectFunc += Ability_Attack_10011;
                    break;
                }
                case 10012://羽化秘术
                {
                    self.SpecialPunisherEffectFunc += Ability_Punisher_10012;
                    self.SpecialDamageCutEffectFunc += Ability_DamageCut_10012;
                    break;
                }
                case 10016://使徒：恶魔堕天使特攻
                {
                    self.SpecialPunisherEffectFunc += Ability_Punisher_10016;
                    break;
                }
                case 10057://芙露露：麻痹特攻
                {
                    self.SpecialPunisherEffectFunc += Ability_Punisher_10057;
                    break;
                }
                case 10058://芙露露：技能增强
                {
                    self.SpecialSkillRateEffectFunc += Ability_SkillRate_10058;
                    break;
                }
                
                
                
                
                
                case 20011://席菈的试炼 闪狼战技
                {
                    self.SpecialCritEffectFunc += Ability_CritRate_20011;
                    self.SpecialCritDamageEffectFunc += Ability_CritDamage_20011;
                    break;
                }
                case 20031://泽娜的试炼 巫女之祈愿
                {
                    self.SpecialAttackEffectFunc += Ability_Attack_20031;
                    break;
                }
                case 20032://泽娜的试炼 巫女之祈愿 绝级
                {
                    self.SpecialAttackEffectFunc += Ability_Attack_20032;
                    self.SpecialDefenseEffectFunc += Ability_Defense_20032;
                    self.SpecialDamageCutEffectFunc += Ability_DamageCut_20032;
                    break;
                }
                case 20121://塞西娅的试炼 守护的意志
                {
                    self.SpecialSkillDamageEffectFunc += Ability_SkillDamage_20121;
                    break;
                }
                case 20131:
                {
                    self.SpecialDamageCutEffectFunc += Ability_DamageCut_20131;
                    break;
                }


                default:
                {
                    //其他操作
                    break;
                }
                
                
                
        }

        }




    #region Attack

    ///巫女之祈愿
    private static Tuple<float,float> Ability_Attack_10006(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        return new Tuple<float, float>((sourceStat.currentHp / sourceStat.maxHP) * 0.2f,0);
    }
    /// <summary>
    /// 疾风怒涛攻
    /// </summary>
    private static Tuple<float,float> Ability_Attack_10011(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        if (sourceStat.comboHitCount >= 15)
            return new Tuple<float, float>(0.2f,0);

        return new Tuple<float, float>(0,0);
    }
    
    private static Tuple<float,float> Ability_Attack_20031(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        return Ability_Attack_10006(sourceStat, atkStat, targetStat);
    }
    
    private static Tuple<float, float> Ability_Attack_20032(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        var buffModifier = (sourceStat.currentHp / sourceStat.maxHP) * 0.2f;
        var debuffModifier = 0f;

        //反伤领域生效，自身的攻击力下降20%。
        if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
        {
            debuffModifier += 0.2f;
        }
        
        return new Tuple<float, float>(buffModifier,debuffModifier);
    }

    #endregion

    # region Defense
    private static Tuple<float,float> Ability_Defense_20032(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        var debuffModifier = 0f;
        if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
        {
            debuffModifier += 0.2f;
        }
        return new Tuple<float, float>(0,debuffModifier);
    }
    # endregion

    # region Critical Rate
    
    /// <summary>
    /// 巫女之祈愿（连击暴击率）
    /// </summary>
    private static Tuple<float,float> Ability_CritRate_10006(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        return sourceStat.comboHitCount >= 15 ? new Tuple<float, float>(8, 0) : new Tuple<float, float>(0, 0);
    }
    
    /// <summary>
    /// 疾风怒涛 会心
    /// </summary>
    private static Tuple<float,float> Ability_CritRate_10008(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        return sourceStat.comboHitCount >= 15 ? new Tuple<float, float>(15, 0) : new Tuple<float, float>(0, 0);
    }
    
    
    
    /// <summary>
    /// 闪狼战技
    /// </summary>
    private static Tuple<float,float> Ability_CritRate_10009(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        if (atkStat.skill_id == 2 &&
            targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.EvilsBane) > 0)
        {
            return new Tuple<float, float>(999,0);
        }

        return new Tuple<float, float>(0,0);
        
    }
    
    private static Tuple<float,float> Ability_CritRate_20011(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        return Ability_CritRate_10009(sourceStat,atkStat,targetStat);
    }
    
    
    # endregion
    
    # region Critical Damage
    
    /// <summary>
    /// 暴击输出
    /// </summary>
    private static Tuple<float,float> Ability_CritDamage_10002(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Flashburn) > 0)
        { 
            return new Tuple<float, float>(0.5f,0);
        }
        return new Tuple<float, float>(0,0);
    }
    
    /// <summary>
    /// 闪狼战技
    /// </summary>
    private static Tuple<float,float> Ability_CritDamage_10009(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.EvilsBane) > 0)
        { 
            return new Tuple<float, float>(0.2f,0);
        }
        return new Tuple<float, float>(0,0);
    }
    
    private static Tuple<float,float> Ability_CritDamage_20011(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.EvilsBane) > 0
            && atkStat.attackType == BasicCalculation.AttackType.STANDARD)
        { 
            return new Tuple<float, float>(0.2f,0);
        }
        return new Tuple<float, float>(0,0);
    }
    
    
    # endregion
    
    #region SkillDamage
    
    /// <summary>
    /// 守护的意志（塞西娅）
    /// </summary>
    private static Tuple<float,float> Ability_SkillDamage_20121(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        
        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.PowerOfBonds) > 0)
        {
            return new Tuple<float, float>(0.6f,0);
        }
        return new Tuple<float, float>(0.2f,0);
    }
    
    
    
    #endregion

    #region SkillRate
    
    /// <summary>
    /// 芙露露：技能增强
    /// </summary>
    private static Tuple<float, float> Ability_SkillRate_10058(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        
        
        float buffModifier = 0;

        if (sourceStat.currentHp > sourceStat.maxHP * 0.7f)
        {
            buffModifier += 0.1f;
        }
        
        
        return new Tuple<float, float>(buffModifier,0);
        
    }

    #endregion

    #region ForceStrikeDamage
    #endregion

    #region Damage

    

    #endregion

    #region DamageCut
    
    /// <summary>
    /// 席菈：绯红幻影
    /// </summary>
    private static Tuple<float, float> Ability_DamageCut_10009(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        var buffModifier = 0f;
        
        if ((sourceStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Scorchrend) > 0 ||
             sourceStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Burn) > 0) &&
            Projectile_C005_4.Instance != null)
        {
            buffModifier += 0.3f;
        }
        
        
        return new Tuple<float, float>(buffModifier,0);
        
    }
    
    /// <summary>
    /// 纳姆：羽化秘术
    /// </summary>
    private static Tuple<float, float> Ability_DamageCut_10012(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        var buffModifier = 0f;
        
        if (targetStat is PlayerStatusManager)
        {
            var playerStat = targetStat as PlayerStatusManager;
            if(playerStat.isShapeshifting)
                buffModifier += 0.5f;
        }

        return new Tuple<float, float>(buffModifier,0);
        
    }
    
    /// <summary>
    /// 泽娜（绝级/敌人：巫女之祈愿）
    /// </summary>
    private static Tuple<float, float> Ability_DamageCut_20032(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        var buffModifier = 
            Mathf.Pow(((float)targetStat.currentHp / (float)targetStat.maxHP),2) * 0.7f;
        
        return new Tuple<float, float>(buffModifier,0);
    }
    
    
    /// <summary>
    /// 塞西娅（敌方：起源的庇佑）
    /// </summary>
    private static Tuple<float, float> Ability_DamageCut_20131(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {

        float buffModifier = 0f;
        if (atkStat.attackType == BasicCalculation.AttackType.DSKILL || atkStat.attackType == BasicCalculation.AttackType.DSTANDARD)
        {
            buffModifier += 0.8f;
        }
        
        return new Tuple<float, float>(buffModifier,0);
    }

    #endregion

    #region Recovery

    /// <summary>
    /// 战斗人偶（回复）
    /// </summary>
    private static Tuple<float, float> Ability_RecoveryPotency_80001(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        var hpRatio = (float)sourceStat.currentHp / sourceStat.maxHP;
        var buffModifier = (1 - hpRatio) * 1.5f;
        
        
        return new Tuple<float, float>(buffModifier,0);
        
    }




    #endregion

    #region Punisher

    /// <summary>
    /// 防御力下降特效
    /// </summary>
    private static Tuple<float, float> Ability_Punisher_10004(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        float buffModifier = 0;
        if (targetStat.GetConditionTotalValue((int)BasicCalculation.BattleCondition.DefDebuff) > 0)
        {
            buffModifier += 0.3f;
        }
        return new Tuple<float, float>(buffModifier,0);
    }
    
    
    /// <summary>
    /// 羽化秘术(裂风特攻)
    /// </summary>
    private static Tuple<float, float> Ability_Punisher_10012(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        float buffModifier = 0;
        if(sourceStat is PlayerStatusManager)
        {
            var playerStat = sourceStat as PlayerStatusManager;
            if (playerStat.isShapeshifting && 
                targetStat.GetConditionStackNumber(
                    (int)BasicCalculation.BattleCondition.Stormlash) > 0)
            {
                buffModifier += 0.1f;
            }
        }
        return new Tuple<float, float>(buffModifier,0);
    }
    
    /// <summary>
    /// 皮诺：恶魔特攻
    /// </summary>
    private static Tuple<float, float> Ability_Punisher_10016(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        float buffModifier = 0;
        if (targetStat.GetAbility(90004))
        {
            buffModifier += 0.3f;
        }
        return new Tuple<float, float>(buffModifier,0);
    }
    
    
    /// <summary>
    /// 芙露露：麻痹、减益特攻
    /// </summary>
    private static Tuple<float, float> Ability_Punisher_10057(StatusManager sourceStat, AttackBase atkStat,
        StatusManager targetStat)
    {
        float buffModifier = 0;
        if (targetStat.GetConditionStackNumber((int)BasicCalculation.BattleCondition.Paralysis) > 0)
        {
            buffModifier += 0.2f;
        }

        int debuffCount = 0;
        List<int> debuffDict = new List<int>();
        foreach (var cond in targetStat.conditionList)
        {
            if (StatusManager.IsDebuff(cond.buffID) && debuffDict.Contains(cond.buffID) == false)
            {
                debuffCount++;
                debuffDict.Add(cond.buffID);
            }

            if (debuffCount >= 4)
            {
                debuffCount = 4;
                break;
            }
        }

        if(debuffCount > 0)
            buffModifier += debuffCount * 0.05f + 0.05f;
        return new Tuple<float, float>(buffModifier,0);
    }

    #endregion


    #region DebuffRate

    

    #endregion


    #region ODAccelerator

    

    #endregion





    }
    public static class AbilityCalculation
    {
        
        public enum ProductArea
        {
            ATK,
            DEF,
            CRITRATE,
            CRITDMG,
            SKLDMG,
            SKLRATE,
            DMG,
            DMGCUT,
            FSDMG,
            RCV,
            PUNISHER,
            DBFRATE,
            ODACC
        }
        
        
        public static Tuple<float, float, float> GetAbilityAmountInfo
            (StatusManager source,StatusManager target, AttackBase atk, ProductArea area)
        {
            float buffModifier = 0;
            float debuffModifier = 0;
            Delegate[] methodList;
            
            //Debug.Log("Checking:"+area.ToString());
            
            switch (area)
            {
                case ProductArea.ATK:
                {
                    methodList = source.SpecialAttackEffectFunc?.GetInvocationList();
                    break;
                }

                case ProductArea.DMG:
                {
                    methodList = source.SpecialDamageEffectFunc?.GetInvocationList();
                    break;
                }

                case ProductArea.CRITRATE:
                {
                    methodList = source.SpecialCritEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.CRITDMG:
                {
                    methodList = source.SpecialCritDamageEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.SKLDMG:
                {
                    methodList = source.SpecialSkillDamageEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.SKLRATE:
                {
                    methodList = source.SpecialSkillRateEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.FSDMG:
                {
                    methodList = source.SpecialForceStrikeDamageEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.RCV:
                {
                    methodList = source.SpecialRecoveryPotencyEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.PUNISHER:
                {
                    methodList = source.SpecialPunisherEffectFunc?.GetInvocationList();
                    break;
                }

                case ProductArea.DEF:
                {
                    methodList = target.SpecialDefenseEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.DMGCUT:
                {
                    methodList = target.SpecialDamageCutEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.DBFRATE:
                {
                    methodList = source.SpecialDebuffRateEffectFunc?.GetInvocationList();
                    break;
                }
                
                case ProductArea.ODACC:
                {
                    methodList = source.SpecialODAcceralatorEffectFunc?.GetInvocationList();
                    break;
                }
                
                
                
                

                default:
                {
                    methodList = null;
                    break;
                }

            }

            if (methodList == null)
                return new Tuple<float, float, float>(0, 0, 0);
            
            

            foreach (var func in methodList)
            {
                var res = ((StatusManager.SpecialEffectFunc)func).Invoke(source, atk, target);
                buffModifier += res.Item1;
                debuffModifier += res.Item2;
            }
            


            return new Tuple<float, float, float>(buffModifier - debuffModifier, buffModifier, debuffModifier);

        }







    }

    [Serializable]
    public class ConditionalEffect
    {
        public enum ConditionType
        {
            HasConditionWithBuffID,
            HasConditionWithSpecialID,
            SourceCrisisModifier,
            TargetCrisisModifier,
            Custom
        }

        public enum EffectType
        {
            DamageModifierUp,
            CritRateUp,
            CrisisFunction,
            AddConditions,
            Custom
        }

        public ConditionType conditionType { get; private set; }
        public EffectType effectType { get; private set; }
        
        private List<int> CheckConditionList = new();
        private Func<StatusManager, StatusManager, bool> conditionFunc;
        private Func<StatusManager, StatusManager, object[]> effectFunc;
        private List<object> effectMessage = new();

        /// <summary>
        /// 检查目标是否有指定的BUFF
        /// </summary>
        /// <param name="conditionIDArray">需要检查的BUFF ID/SPID</param>
        /// <param name="effectType">附加效果类型（背水除外）</param>
        /// <param name="effectMessage">附加效果信息</param>
        /// <param name="isSpecial">需要检查的是SPID，则为1</param>
        /// <param name="checkAll">逻辑与（检查所有BUFF一起生效才为true）</param>
        /// <param name="customEffectFunc">当effectType为Custom时添加自定义方法</param>
        public ConditionalEffect(int[] conditionIDArray, EffectType effectType, object[] effectMessage,
            bool isSpecial, bool checkAll = true, Func<StatusManager,StatusManager,object[]> customEffectFunc = null)
        {
            //检查条件
            if (isSpecial)
            {
                conditionType = ConditionType.HasConditionWithSpecialID;
            }else
            {
                conditionType = ConditionType.HasConditionWithBuffID;
            }
            this.effectType = effectType;
            
            CheckConditionList.AddRange(conditionIDArray);

            if (isSpecial)
            {
                conditionFunc = checkAll ? HasAllConditionWithSpecialID : HasAnyConditionWithSpecialID;
            }
            else
            {
                conditionFunc = checkAll ? HasAllConditionWithBuffID : HasAnyConditionWithBuffID;
            }
            
            //检查效果
            switch (effectType)
            {
                case EffectType.CritRateUp:
                {
                    effectFunc = DamageModifierOrCritRateUp;
                    break;
                }
                case EffectType.DamageModifierUp:
                {
                    effectFunc = DamageModifierOrCritRateUp;
                    break;
                }
                case EffectType.AddConditions:
                {
                    effectFunc = AddCondition;
                    break;
                }
                case EffectType.Custom:
                {
                    if (customEffectFunc == null)
                    {
                        Debug.LogError("Custom Effect Func is null");
                    }
                    else
                    {
                        effectFunc = customEffectFunc;
                    }
                    break;
                }

            }
            
            



        }

        /// <summary>
        /// 添加攻击的背水效果
        /// </summary>
        /// <param name="effectMessage[0]">背水系数，大于1代表血量越低伤害越高</param>
        /// <param name="effectMessage[1]">背水曲线斜度，大于1代表血量减少对于伤害的影响会越来越大</param>
        /// <param name="effectMessage[2]">区间最小生命值百分比(0-1)</param>
        /// <param name="effectMessage[3]">区间最大生命值百分比(0-1)</param>
        /// <param name="crisisTarget">背水对象，0代表自身,1代表目标</param>
        public ConditionalEffect( object[] effectMessage, int crisisTarget = 0)
        {
            this.effectMessage.AddRange(effectMessage);

            if (crisisTarget == 0)
            {
                conditionType = ConditionType.SourceCrisisModifier;
            }
            else
            {
                conditionType = ConditionType.TargetCrisisModifier;
            }
            
            effectType = EffectType.CrisisFunction;
            
            effectFunc = CrisisFunction;


        }


        /// <summary>
        /// 自定义条件
        /// </summary>
        /// <param name="customConditionFunc">自定义条件函数</param>
        /// <param name="effectType">附加效果类型（背水除外）</param>
        /// <param name="effectMessage">附加效果信息</param>
        /// <param name="customEffectFunc">当effectType为Custom时添加自定义方法</param>
        public ConditionalEffect(Func<StatusManager, StatusManager, bool> customConditionFunc,
            object[] effectMessage, EffectType effectType,
            Func<StatusManager, StatusManager, object[]> customEffectFunc = null)
        {
            conditionFunc = customConditionFunc;
            
            conditionType = ConditionType.Custom;
            
            this.effectMessage.AddRange(effectMessage);

            switch (effectType)
            {
                case EffectType.AddConditions:
                {
                    effectFunc = AddCondition;
                    break;
                }
                case EffectType.DamageModifierUp:
                {
                    effectFunc = DamageModifierOrCritRateUp;
                    break;
                }
                case EffectType.CritRateUp:
                {
                    effectFunc = DamageModifierOrCritRateUp;
                    break;
                }
                case EffectType.Custom:
                {
                    if (customEffectFunc == null)
                    {
                        Debug.LogError("Custom Effect Func is null");
                    }
                    else
                    {
                        effectFunc = customEffectFunc;
                    }
                    break;
                }
            }



        }



        # region ConditionalFunction
        
        private bool CustomFunction(StatusManager source, StatusManager target)
        {
            return conditionFunc(source, target);
        }
        
        private bool HasAllConditionWithBuffID(StatusManager source, StatusManager target)
        {
            foreach (var conditionID in CheckConditionList)
            {
                if (target.GetConditionStackNumber(conditionID) <= 0)
                {
                    return false;
                }
            }

            return true;
        }
        
        private bool HasAnyConditionWithBuffID(StatusManager source, StatusManager target)
        {
            foreach (var conditionID in CheckConditionList)
            {
                if (target.GetConditionStackNumber(conditionID) > 0)
                {
                    return true;
                }
            }

            return false;
        }
        
        private bool HasAllConditionWithSpecialID(StatusManager source, StatusManager target)
        {
            foreach (var conditionID in CheckConditionList)
            {
                if (target.GetConditionWithSpecialID(conditionID).Count <= 0)
                {
                    return false;
                }
            }

            return true;
        }
        
        private bool HasAnyConditionWithSpecialID(StatusManager source, StatusManager target)
        {
            foreach (var conditionID in CheckConditionList)
            {
                if (target.GetConditionWithSpecialID(conditionID).Count > 0)
                {
                    return true;
                }
            }

            return false;
        }
        
        #endregion

        #region EffectFunction

        private object[] DamageModifierOrCritRateUp(StatusManager source, StatusManager target)
        {
            return new object[] {effectMessage[0]};
        }
        
        private object[] CrisisFunction(StatusManager source, StatusManager target)
        {
            //背水系数（该项越大，随着生命值的下降，伤害越高，小于1代表生命值越高伤害越高）
            //最大HP时的背水系数，正常的背水时，该项的值应该小于最小HP时的背水系数，正常情况下为1。
            
            //假如某角色背水系数为1.5,最少生命值为0，最大生命值为1
            //那么代表当角色生命值为0%时，造成的伤害为150%，当角色生命值为100%时，造成的伤害为100%。
            
            //假如某角色背水系数为0.5,最少生命值为0，最大生命值为1
            //那么代表当角色生命值为0%时，造成的伤害为100%，当角色生命值为100%时，造成的伤害为200%。
            float crisisFactor = (float) effectMessage[0];
            
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
            
            
            float curveFactor = (float) effectMessage[1];
            
            float minBound = (float) effectMessage[2];
            //背水最大生命值(背水系数小于1时，代表生命越高伤害越高）
            float maxBound = (float) effectMessage[3];

            float hpFraction = 0; // X（HP）值，自变量
            
            
            if (conditionType == ConditionType.SourceCrisisModifier)
            {
                hpFraction = (float)source.currentHp / source.maxHP;
            }else if (conditionType == ConditionType.TargetCrisisModifier)
            {
                hpFraction = (float)target.currentHp / target.maxHP;
            }

            float result = 1;
            
            //根据公式计算背水系数，自变量为HpFraction
            if (hpFraction < minBound)
            {
                return new object[] { crisisFactor };
            }
            else if (hpFraction > maxBound)
            {
                return new object[] { 1 };
            }
            
            if (crisisFactor == 1)
            {
                return new object[] { 1 };
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

            return new object[] { result };


        }

        private object[] AddCondition(StatusManager source, StatusManager target)
        {
            var targetStat = (int) effectMessage[0] == 0 ? source : target;
            
            var conditionID = (int) effectMessage[1];
            var conditionDuration = (int) effectMessage[2];
            var conditionEffect = (float) effectMessage[3];
            var chance = (float) effectMessage[4];
            
            var conditionSpecialID = -1;
            var maxStackNum = 100;
            bool dispelable = true;
            
            if (effectMessage.Count > 6)
            {
                conditionSpecialID = (int) effectMessage[5];
                maxStackNum = (int) effectMessage[6];
            }

            if (effectMessage.Count > 7)
            {
                dispelable = (int)effectMessage[7] == 0 ? false : true;
            }

            TimerBuff buff = new TimerBuff(conditionID,
                conditionEffect, conditionDuration, maxStackNum, conditionSpecialID);
            
            buff.dispellable = dispelable;

            return new object[] { buff, targetStat, chance };


        }


        #endregion
        







    }


}










