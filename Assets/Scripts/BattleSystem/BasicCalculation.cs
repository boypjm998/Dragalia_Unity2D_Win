
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CharacterSpecificProjectiles;
using LitJson;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace GameMechanics
{
    public static class BasicCalculation
    {

        public static string ToButtonString(KeyCode obj)
        {
            string str = obj.ToString();
            if(str == "Mouse0")
                return "LMouse";
            if(str == "Mouse1")
                return "RMouse";
            return str;

        }

        public enum AttackType
        {
            STANDARD = 1,
            DASH = 2,
            FORCE = 3,
            SKILL = 4,
            ABILITY = 5,
            OTHER = 6,
            NONE = 7
        }

        public static int[] conditionsDisplayedByStacknum = new int[]
        {
            5, 13, 14, 15, 19, 57,
            101, 102, 103, 104, 105, 106, 108, 109, 111,
            213, 214, 299, 300,
            301, 302,
            401, 402, 403, 404, 405, 406, 407, 408, 411, 412, 413, 414, 415, 416
        };
        public static int[] conditionsDisplayedByLevel = new int[]
        {
            112,113
        };

        public static int[] conditionDisplayedByExactValue = new int[]
        {
            11
        };

        public static int[] conditionsImmuneToNihility = new int[]
        {
            5, 6, 7, 12, 15, 57
        };
    
        public enum BattleCondition
        {
            //Basic Buff
            AtkBuff = 1,
            DefBuff = 2,
            CritRateBuff = 3,
            CritDmgBuff = 4,
            HealOverTime = 5,
            RecoveryBuff = 6,
            MaxHPBuff = 7,
            SkillDmgBuff = 8,
            SkillHasteBuff = 9,
            DamageCut = 10,
            DamageCutConst = 11,
            Shield = 12,
            LifeShield = 13,
            SPRegen = 14,
            KnockBackImmune = 15,
            DamageUp = 16,
            AttackRateUp = 17,
            ForceStrikeDmgBuff = 18,
            Energy = 19,
            Inspiration = 20,
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
            
            OverdriveAccerlerator = 38,
            
            BurnPunisher = 41,
            PoisonPunisher = 42,
            FrostbitePunisher = 43,
            ParalysisPunisher = 44,
            ScorchrendPunisher = 45,
            StormlashPunisher = 46,
            FlashburnPunisher = 47,
            ShadowblightPunisher = 48,
            
            StunPunisher = 51,
            SleepPunisher = 52,
            BogPunisher = 53,
            FreezePunisher = 54,
            BlindnessPunisher = 55,
            
            AfflictedPunisher = 57,
            
            BreakPunisher = 59,
            
            BurnRateUp = 61,
            PoisonRateUp = 62,
            FrostbiteRateUp = 63,
            ParalysisRateUp = 64,
            ScorchrendRateUp = 65,
            StormlashRateUp = 66,
            FlashburnRateUp = 67,
            ShadowblightRateUp = 68,
            
            DebuffRateUp = 70,
            StunRateUp = 71,
            SleepRateUp = 72,
            BogRateUp = 73,
            FreezeRateUp = 74,
            BlindnessRateUp = 75,
            
    
            //Special Buff
            AlchemicCatridge = 101,
            InfernoMode = 102,
            HolyFaith = 103,
            BlazewolfsRush = 104,
            PowerOfBonds = 105,
            HolyAccord = 106,
            GabrielsBlessing = 107,
            TwilightMoon = 108,
            Invincible = 109,
            
            StandardAttackBurner = 111,
            HeartAflame = 112,
            ScorchingEnergy = 113,
            
            
            
    
            //Basic Debuff
            AtkDebuff = 201,
            DefDebuff = 202,
            CritRateDebuff = 203,
            CritDmgDebuff = 204,
            RecoveryDebuff = 206,
            SkillDmgDebuff = 208,
            SkillHasteDebuff = 209,
            Vulnerable = 210,
    
            Bleeding = 213,
            SPDegen = 214,
    
            DamageDown = 216,
            
            BurnResDown = 221,
            PoisonResDown = 222,
            FrostbiteResDown = 223,
            ParalysisResDown = 224,
            ScorchrendResDown = 225,
            StormlashResDown = 226,
            FlashburnResDown = 227,
            ShadowBlightResDown = 228,
            
            
            
            Taunt = 299,
            Nihility = 300,
            //Special Debuff
            EvilsBane = 301,
            ManaOverloaded = 302,
    
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
            Stun = 412,
            Sleep = 411,
            Bog = 413,
            Freeze = 414,
            Blindness = 415,
            Cursed = 416,
            NoJump = 417,
            NoRoll = 418,
            
            Dispell = 999
            
        }

        public static string GetTraningHintString(PlayerInput playerInput,GlobalController.Language language = GlobalController.Language.ZHCN)
        {
            StringBuilder sb = new();
            string attackText = String.Empty;
            string jumpText = String.Empty;
            string rollText = String.Empty;
            string specialText_c001 = String.Empty;
            string specialText_c001_cond = String.Empty;
            string specialText_c003 = String.Empty;
            string specialText_c003_cond = String.Empty;
            
            

            if (language == GlobalController.Language.ZHCN)
            {
                attackText = "攻击键";
                jumpText = "跳跃键";
                rollText = "回避键";
                specialText_c001 = "特殊行动(传送)";
                specialText_c003 = "蓄力攻击:持续按下";
                specialText_c001_cond = "(当传送门存在时)";
                specialText_c003_cond = "(当【暮光之月】增益存在时)";
            }else if (language == GlobalController.Language.EN)
            {
                attackText = "Attack Key";
                jumpText = "Jump Key";
                rollText = "Dodge Key";
                specialText_c001 = "Special Action(Teleport)";
                specialText_c003 = "Force Strike:Hold ";
                specialText_c001_cond = "(When the portal is on the field)";
                specialText_c003_cond = "(When the user has 'Twilight Moon')";
                
            }




            

            sb.Append($"{attackText}:{ToButtonString(playerInput.keyAttack)}\n");
            sb.Append($"{jumpText}:{ToButtonString(playerInput.keyJump)}\n");
            sb.Append($"{rollText}:{ToButtonString(playerInput.keyRoll)}\n");
        
            
            if (GlobalController.currentCharacterID == 1)
            {

                sb.Append($"{specialText_c001}:{ToButtonString(playerInput.keyUp)}\n{specialText_c001_cond}");
            }
            if (GlobalController.currentCharacterID == 3)
            {

                sb.Append($"{specialText_c003}{ToButtonString(playerInput.keyAttack)}\n{specialText_c003_cond}");
            }

            return sb.ToString();
        }

        public static string ConditionInfo(BattleCondition cond, GlobalController.Language language)
        {
            switch (language)
            {
                case GlobalController.Language.ZHCN:
                    return ConditionInfo_ZH(cond);
                case GlobalController.Language.JP:
                    return ConditionInfo_JP(cond);
                case GlobalController.Language.EN:
                    return ConditionInfo_EN(cond);
                default: return "";
            }
        }
    
        public static string ConditionInfo_JP(BattleCondition cond)
        {
            switch (cond)
            {
                //Basic Conditions
                case BattleCondition.AtkBuff:
                    return ("攻撃力{0}％アップ");
                case BattleCondition.AtkDebuff:
                    return ("攻撃力{0}％ダウン");
                case BattleCondition.DefBuff:
                    return ("防御力{0}％アップ");
                case BattleCondition.DefDebuff:
                    return ("防御力{0}％ダウン");
                case BattleCondition.HealOverTime:
                    return ("HP継続回復");
                case BattleCondition.CritRateBuff:
                    return ("クリティカル率{0}％アップ");
                case BattleCondition.CritRateDebuff:
                    return ("クリティカル率{0}％ダウン");
                case BattleCondition.CritDmgBuff:
                    return ("クリティカルダメージ{0}％アップ");
                case BattleCondition.CritDmgDebuff:
                    return ("クリティカルダメージ{0}％ダウン");
                case BattleCondition.RecoveryBuff:
                    return ("回復スキル効果{0}％アップ");
                case BattleCondition.RecoveryDebuff:
                    return ("回復スキル効果{0}％ダウン");
                case BattleCondition.SkillDmgBuff:
                    return ("スキルダメージ{0}％アップ");
                case BattleCondition.SkillDmgDebuff:
                    return ("スキルダメージ{0}％ダウン");
                case BattleCondition.LifeShield:
                    return ("生命护盾");
                case BattleCondition.DamageCut:
                    return ("受けるダメージ{0}%カット");
                case BattleCondition.DamageCutConst:
                    return ("受けるダメージ{0}ダウン");
                case BattleCondition.Shield:
                    return ("");
                case BattleCondition.SkillHasteBuff:
                    return ("スキルブースト{0}%アップ");
                case BattleCondition.SkillHasteDebuff:
                    return ("スキルブースト{0}%ダウン");
                case BattleCondition.SPRegen:
                    return ("スキルゲージ継続上昇");
                case BattleCondition.SPDegen:
                    return ("スキルゲージ継続減少");
                case BattleCondition.Vulnerable:
                    return ("受けるダメージ{0}%アップ");
                
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
                    return ("アルケミーカートリッジ装填");
                case BattleCondition.InfernoMode:
                    return ("");
                case BattleCondition.HolyFaith:
                    return ("");
                case BattleCondition.BlazewolfsRush:
                    return ("巫女の気炎");
    
                //Special debuffs:
                case BattleCondition.EvilsBane:
                    return ("破邪の巫呪");
    
                //Afflictions
                case BattleCondition.Flashburn:
                    return ("閃熱");
                case BattleCondition.Scorchrend:
                    return ("劫火");
                case BattleCondition.Burn:
                    return ("火傷");
                case BattleCondition.Blindness:
                    return ("黑暗");
                case BattleCondition.ShadowBlight:
                    return ("暗殇");
                case BattleCondition.Frostbite:
                    return ("冷傷");
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
                    return ("裂風");
                case BattleCondition.Cursed:
                    return ("诅咒");
    
    
                default:
                {
                    Debug.LogWarning("Buff text not found");
                    return ("");
                }
            }
        }
        
        public static string ConditionInfo_EN(BattleCondition cond)
        {
            switch (cond)
            {
                //Basic Conditions
                case BattleCondition.AtkBuff:
                    return ("Strength +{0}%");
                case BattleCondition.AtkDebuff:
                    return ("Strength -{0}%");
                case BattleCondition.DefBuff:
                    return ("Defense +{0}%");
                case BattleCondition.DefDebuff:
                    return ("Defense -{0}%");
                case BattleCondition.HealOverTime:
                    return ("HP Regen");
                case BattleCondition.CritRateBuff:
                    return ("Critical Rate +{0}%");
                case BattleCondition.CritRateDebuff:
                    return ("Critical Rate -{0}%");
                case BattleCondition.CritDmgBuff:
                    return ("Critical Damage +{0}%");
                case BattleCondition.CritDmgDebuff:
                    return ("Critical Damage -{0}%");
                case BattleCondition.RecoveryBuff:
                    return ("Recovery Potency +{0}%");
                case BattleCondition.RecoveryDebuff:
                    return ("Recovery Potency -{0}%");
                case BattleCondition.SkillDmgBuff:
                    return ("Skill Damage +{0}%");
                case BattleCondition.SkillDmgDebuff:
                    return ("Skill Damage -{0}%");
                case BattleCondition.ForceStrikeDmgBuff:
                    return ("Force Strike Damage +{0}%");
                case BattleCondition.Shield:
                    return ("Shield");
                case BattleCondition.LifeShield:
                    return ("Life Shield");
                case BattleCondition.DamageCut:
                    return ("Damage Taken -{0}%");
                case BattleCondition.DamageCutConst:
                    return ("Damage Taken -{0}");
                case BattleCondition.DamageUp:
                    return ("Damage Dealt +{0}%");
                
                
                case BattleCondition.SkillHasteBuff:
                    return ("Skill Fill Rate +{0}%");
                case BattleCondition.SkillHasteDebuff:
                    return ("Skill Fill Rate -{0}%");
                case BattleCondition.SPRegen:
                    return ("Skill Energy Regen");
                case BattleCondition.SPDegen:
                    return ("Skill Energy Degen");
                case BattleCondition.Vulnerable:
                    return ("Damage Taken +{0}%");
                
                case BattleCondition.KnockBackImmune:
                    return ("Knockback Immunity");
                case BattleCondition.ScorchrendRes:
                    return ("Scorchrend Res +{0}%");
                case BattleCondition.FlashburnRes:
                    return ("Flashburn Res +{0}%");
                case BattleCondition.BurnRes:
                    return ("Burn Res +{0}%");
                case BattleCondition.ShadowBlightRes:
                    return ("Shadowblight Res +{0}%");
                case BattleCondition.ParalysisRes:
                    return ("Paralysis Res +{0}%");
                case BattleCondition.FrostbiteRes:
                    return ("Frostbite Res +{0}%");
                case BattleCondition.StormlashRes:
                    return ("Stormlash Res +{0}%");
                case BattleCondition.PoisonRes:
                    return ("Poison Res +{0}%");
                case BattleCondition.FreezeRes:
                    return ("Freeze Res +{0}%");
                case BattleCondition.StunRes:
                    return ("Stun Res +{0}%");
                case BattleCondition.SleepRes:
                    return ("Sleep Res +{0}%");
                case BattleCondition.BlindnessRes:
                    return ("Blindness Res +{0}%");
                case BattleCondition.FlashburnResDown:
                    return ("Flashburn Res -{0}%");
                case BattleCondition.ScorchrendResDown:
                    return ("Scorchrend Res -{0}%");
                case BattleCondition.BurnResDown:
                    return ("Burn Res -{0}%");
                case BattleCondition.ShadowBlightResDown:
                    return ("Shadowblight Res -{0}%");
                case BattleCondition.ParalysisResDown:
                    return ("Paralysis Res -{0}%");
                case BattleCondition.FrostbiteResDown:
                    return ("Frostbite Res -{0}%");
                case BattleCondition.StormlashResDown:
                    return ("Stormlash Res -{0}%");
                case BattleCondition.PoisonResDown:
                    return ("Poison Res -{0}%");
                case BattleCondition.BurnRateUp:
                    return ("Burn Infliction Rate +{0}%");
                case BattleCondition.FlashburnRateUp:
                    return ("Flashburn Infliction Rate +{0}%");
                case BattleCondition.ScorchrendRateUp:
                    return ("Flashburn Infliction Rate +{0}%");
                
                case BattleCondition.Energy:
                    return ("Energy Level +{0}");
                case BattleCondition.Inspiration:
                    return ("Inspiration Level +{0}");
                
                case BattleCondition.OverdriveAccerlerator:
                    return ("OD Gauge Decrease Rate +{0}%");
                
                
                case BattleCondition.AfflictedPunisher:
                    return ("Afflicted Punisher");
                case BattleCondition.BreakPunisher:
                    return ("Break Punisher +{0}%");
                case BattleCondition.BlindnessPunisher:
                    return ("Blindness Punisher +{0}%");
                case BattleCondition.BogPunisher:
                    return ("Bog Punisher +{0}%");
                case BattleCondition.BurnPunisher:
                    return ("Burn Punisher +{0}%");
                case BattleCondition.FlashburnPunisher:
                    return ("Flashburn Punisher +{0}%");
                case BattleCondition.FreezePunisher:
                    return ("Freeze Punisher +{0}%");
                case BattleCondition.FrostbitePunisher:
                    return ("Frostbite Punisher +{0}%");
                case BattleCondition.ParalysisPunisher:
                    return ("Paralysis Punisher +{0}%");
                case BattleCondition.PoisonPunisher:
                    return ("Poison Punisher +{0}%");
                case BattleCondition.ScorchrendPunisher:
                    return ("Scorchrend Punisher +{0}%");
                case BattleCondition.ShadowblightPunisher:
                    return ("Shadowblight Punisher +{0}%");
                case BattleCondition.SleepPunisher:
                    return ("Sleep Punisher +{0}%");
                case BattleCondition.StormlashPunisher:
                    return ("Stormlash Punisher +{0}%");
                case BattleCondition.StunPunisher:
                    return ("Stun Punisher +{0}%");
                
    
                //Special buffs:
                
                case BattleCondition.AlchemicCatridge:
                    return ("Cartridges Loaded");
                case BattleCondition.InfernoMode:
                    return ("Inferno Mode");
                case BattleCondition.HolyFaith:
                    return ("Holy Faith");
                case BattleCondition.BlazewolfsRush:
                    return ("Blazewolf's Rush");
                case BattleCondition.PowerOfBonds:
                    return ("Power of Bonds");
                case BattleCondition.HolyAccord:
                    return ("Holy Accord");
                case BattleCondition.GabrielsBlessing:
                    return ("Gabriel's Blessing");
                case BattleCondition.TwilightMoon:
                    return ("Twilight Moon");
                case BattleCondition.Invincible:
                    return ("Invulnerability");
                
                case BattleCondition.StandardAttackBurner:
                    return ("Standard Attacks Inflicts Burn");
                case BattleCondition.HeartAflame:
                    return ("Heart Aflame {0}");
                case BattleCondition.ScorchingEnergy:
                    return ("Scorching Energy {0}");
                ;
                    
    
                //Special debuffs:
                case BattleCondition.EvilsBane:
                    return ("Evil's Bane");
                case BattleCondition.ManaOverloaded:
                    return ("Energy Overloaded");
                
                case BattleCondition.Taunt:
                    return ("Marked");
                case BattleCondition.Nihility:
                    return ("Nihility");

                //Afflictions
                case BattleCondition.Flashburn:
                    return ("Flashburn");
                case BattleCondition.Scorchrend:
                    return ("Scorchrend");
                case BattleCondition.Burn:
                    return ("Burn");
                case BattleCondition.Blindness:
                    return ("Blindness");
                case BattleCondition.ShadowBlight:
                    return ("Shadowblight");
                case BattleCondition.Frostbite:
                    return ("Frostbite");
                case BattleCondition.Freeze:
                    return ("Freeze");
                case BattleCondition.Stun:
                    return ("Stun");
                case BattleCondition.Sleep:
                    return ("Sleep");
                case BattleCondition.Bog:
                    return ("Bog");
                case BattleCondition.Paralysis:
                    return ("Paralysis");
                case BattleCondition.Poison:
                    return ("Poison");
                case BattleCondition.Stormlash:
                    return ("Stormlash");
                case BattleCondition.Cursed:
                    return ("Cursed");
    
    
                default:
                {
                    Debug.LogWarning("Buff text not found");
                    return ("");
                }
            }
        }
        public static string ConditionInfo_ZH(BattleCondition cond)
        {
            switch (cond)
            {
                //Basic Conditions
                case BattleCondition.AtkBuff:
                    return ("攻击力提升{0}%");
                case BattleCondition.AtkDebuff:
                    return ("攻击力下降{0}%");
                case BattleCondition.DefBuff:
                    return ("防御力提升{0}%");
                case BattleCondition.DefDebuff:
                    return ("防御力下降{0}%");
                case BattleCondition.HealOverTime:
                    return ("HP持续回复");
                case BattleCondition.CritRateBuff:
                    return ("暴击率提升{0}%");
                case BattleCondition.CritRateDebuff:
                    return ("暴击率下降{0}%");
                case BattleCondition.CritDmgBuff:
                    return ("暴击威力提升{0}%");
                case BattleCondition.CritDmgDebuff:
                    return ("暴击威力下降{0}%");
                case BattleCondition.RecoveryBuff:
                    return ("回复效果提升{0}%");
                case BattleCondition.RecoveryDebuff:
                    return ("回复效果下降{0}%");
                case BattleCondition.SkillDmgBuff:
                    return ("技能伤害提升{0}%");
                case BattleCondition.SkillDmgDebuff:
                    return ("技能伤害下降{0}%");
                case BattleCondition.ForceStrikeDmgBuff:
                    return ("爆发攻击伤害提升{0}%");
                case BattleCondition.Shield:
                    return ("护盾");
                case BattleCondition.LifeShield:
                    return ("生命护盾");
                case BattleCondition.DamageCut:
                    return ("所受伤害减少{0}%");
                case BattleCondition.DamageCutConst:
                    return ("所受伤害减少{0}");
                case BattleCondition.DamageUp:
                    return ("攻击威力提升{0}%");
                
                
                case BattleCondition.SkillHasteBuff:
                    return ("技能槽获取提升{0}%");
                case BattleCondition.SkillHasteDebuff:
                    return ("技能槽获取下降{0}%");
                case BattleCondition.SPRegen:
                    return ("技能槽持续提升");
                case BattleCondition.SPDegen:
                    return ("技能槽持续下降");
                case BattleCondition.Vulnerable:
                    return ("所受伤害增加{0}%");
                
                
                case BattleCondition.KnockBackImmune:
                    return ("免疫击退效果");
                case BattleCondition.ScorchrendRes:
                    return ("劫火抗性提升{0}%");
                case BattleCondition.FlashburnRes:
                    return ("闪热抗性提升{0}%");
                case BattleCondition.BurnRes:
                    return ("烧伤抗性提升{0}%");
                case BattleCondition.ShadowBlightRes:
                    return ("暗殇抗性提升{0}%");
                case BattleCondition.ParalysisRes:
                    return ("麻痹抗性提升{0}%");
                case BattleCondition.FrostbiteRes:
                    return ("冻伤抗性提升{0}%");
                case BattleCondition.StormlashRes:
                    return ("裂风抗性提升{0}%");
                case BattleCondition.PoisonRes:
                    return ("中毒抗性提升{0}%");
                case BattleCondition.FreezeRes:
                    return ("冰冻抗性提升{0}%");
                case BattleCondition.StunRes:
                    return ("昏迷抗性提升{0}%");
                case BattleCondition.SleepRes:
                    return ("睡眠抗性提升{0}%");
                case BattleCondition.BlindnessRes:
                    return ("黑暗抗性提升{0}%");
                case BattleCondition.BogRes:
                    return ("湿身抗性提升{0}%");
                case BattleCondition.FlashburnResDown:
                    return ("闪热抗性下降{0}%");
                case BattleCondition.ScorchrendResDown:
                    return ("劫火抗性下降{0}%");
                case BattleCondition.BurnResDown:
                    return ("烧伤抗性下降{0}%");
                case BattleCondition.ShadowBlightResDown:
                    return ("暗殇抗性下降{0}%");
                case BattleCondition.ParalysisResDown:
                    return ("麻痹抗性下降{0}%");
                case BattleCondition.FrostbiteResDown:
                    return ("冻伤抗性下降{0}%");
                case BattleCondition.StormlashResDown:
                    return ("裂风抗性下降{0}%");
                case BattleCondition.PoisonResDown:
                    return ("中毒抗性下降{0}%");
                case BattleCondition.BurnRateUp:
                    return ("造成烧伤成功率提升{0}%");
                case BattleCondition.FlashburnRateUp:
                    return ("造成闪热成功率提升{0}%");
                case BattleCondition.ScorchrendRateUp:
                    return ("造成劫火成功率提升{0}%");



                case BattleCondition.Energy:
                    return ("斗志提升×{0}");
                case BattleCondition.Inspiration:
                    return ("灵感提升×{0}");
                
                
                case BattleCondition.OverdriveAccerlerator:
                    return ("怒气槽削减提升{0}%");
                
                
                
                case BattleCondition.AfflictedPunisher:
                    return ("异常状态特效");
                case BattleCondition.BreakPunisher:
                    return ("破防特效提升{0}%");
                case BattleCondition.BlindnessPunisher:
                    return ("黑暗特效提升{0}%");
                case BattleCondition.BogPunisher:
                    return ("湿身特效提升{0}%");
                case BattleCondition.BurnPunisher:
                    return ("烧伤特效提升{0}%");
                case BattleCondition.FlashburnPunisher:
                    return ("闪热特效提升{0}%");
                case BattleCondition.FreezePunisher:
                    return ("冰冻特效提升{0}%");
                case BattleCondition.FrostbitePunisher:
                    return ("冻伤特效提升{0}%");
                case BattleCondition.ParalysisPunisher:
                    return ("麻痹特效提升{0}%");
                case BattleCondition.PoisonPunisher:
                    return ("中毒特效提升{0}%");
                case BattleCondition.ScorchrendPunisher:
                    return ("劫火特效提升{0}%");
                case BattleCondition.ShadowblightPunisher:
                    return ("暗殇特效提升{0}%");
                case BattleCondition.SleepPunisher:
                    return ("睡眠特效提升{0}%");
                case BattleCondition.StormlashPunisher:
                    return ("裂风特效提升{0}%");
                case BattleCondition.StunPunisher:
                    return ("昏迷特效提升{0}%");

                //Special buffs:
                case BattleCondition.AlchemicCatridge:
                    return ("炼金弹夹装填");
                case BattleCondition.InfernoMode:
                    return ("地狱模式");
                case BattleCondition.HolyFaith:
                    return ("圣洁信念");
                case BattleCondition.BlazewolfsRush:
                    return ("巫女气焰");
                case BattleCondition.PowerOfBonds:
                    return ("信赖之力");
                case BattleCondition.HolyAccord:
                    return ("圣天启示");
                case BattleCondition.GabrielsBlessing:
                    return ("加百列的祝福");
                case BattleCondition.TwilightMoon:
                    return ("暮光之月");
                case BattleCondition.Invincible:
                    return ("铁壁");
                
                case BattleCondition.StandardAttackBurner:
                    return ("普通攻击赋予烧伤");
                case BattleCondition.HeartAflame:
                    return ("炽热之恋{0}");
                case BattleCondition.ScorchingEnergy:
                    return ("灼热炽焰{0}");
    
                //Special debuffs:
                case BattleCondition.EvilsBane:
                    return ("破邪巫咒");
                case BattleCondition.ManaOverloaded:
                    return ("魔力过载");
                
                
                case BattleCondition.Taunt:
                    return ("敌方目标");
                case BattleCondition.Nihility:
                    return ("虚无");
    
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
            //TODO: 只是显示上限，实际上限还需要在计算时再次判断，以后记得修改
            switch (id)
            {
                case (int)BattleCondition.AtkBuff:
                    return 200;
                case (int)BattleCondition.DefBuff:
                    return 200;
                case (int)BattleCondition.CritRateBuff:
                    return 200;
                case (int)BattleCondition.CritDmgBuff:
                    return 500;
                case 6:
                    return 200;
                case 7:
                    return 30;
                case 8:
                    return 200;
                case (int)BattleCondition.SkillHasteBuff:
                    return 100;
                case 10:
                    return 100;
                case 11:
                    return 9999999;
                case 12:
                    return 200;
                case 13:
                    return 99999;
                case 14:
                    return 99999;
                
                case (int)(BattleCondition.DamageUp):
                    return 500;
                
                case 201:
                    return 50;
                case 202:
                    return 50;
                case 203:
                    return 100;
                case 204:
                    return 500;
                case 206:
                    return 200;
                case 207:
                    return 30;
                case (int)BattleCondition.SkillHasteDebuff:
                    return 100;
                case 210:
                    return 100;
                case 214:
                    return 99999;
                
                case (int)(BattleCondition.DamageDown):
                    return 100;
                
                
                
                
                
                default:
                    return 200;
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
            AttackBase atkStat, ref bool isCrit)
        {
            //Source

            float newModifier = modifier;

            if (atkStat.conditionalAttackEffects.Count > 0)
            {
                var extraModifier = 0f;
                foreach (var caf in atkStat.conditionalAttackEffects)
                {
                    extraModifier += caf.GetExtraModifiers(targetStat,sourceStat);
                }
                newModifier *= (1 + extraModifier);
                Debug.Log("Extra modifier: " + extraModifier);
            }


            //攻击 Attack

            // var atkBuff = sourceStat.attackBuff;
            // var atkAbility = CheckSpecialAttackEffect(sourceStat,targetStat,atkStat).Item1;
            // RulesInBattleField.GetFieldEffect_Attack(atkStat,sourceStat,targetStat,ref atkAbility,ref atkBuff);
            // var atk = sourceStat.baseAtk * (1 + atkBuff) * (1 + atkAbility);
            float atk = CalculateAttackInfo(atkStat, sourceStat, targetStat);
            
            //攻击威力

            // var dmgBuff = sourceStat.dmgUpBuff;
            // var dmgAbility = CheckSpecialDamageUpEffect(sourceStat, targetStat, atkStat).Item1;
            // RulesInBattleField.GetFieldEffect_Damage(atkStat,sourceStat,targetStat,ref dmgAbility,ref dmgBuff);
            // var dmgBuffModifier = dmgAbility + dmgBuff;
            var dmgBuffModifier = CalculateDamageUpInfo(atkStat, sourceStat, targetStat);
    
            //暴击 爆伤 crit
    
    
            var critRateBuff = sourceStat.critRateBuff;
            var critAbility= (float)CheckSpecialCritEffect(sourceStat,targetStat,atkStat).Item1;
            RulesInBattleField.GetFieldEffect_CritRate(atkStat,sourceStat,targetStat,ref critAbility,ref critRateBuff);
            var critRate = sourceStat.critRate + critAbility + critRateBuff;
            
            
            float critDmgModifier = 1;
            if (Random.Range(0, 100) < critRate)
            {
                isCrit = true;
                critDmgModifier += 0.7f;
                var critDmgBuff = sourceStat.critDmgBuff;
                var critDmgAbility = CheckSpecialCritDmgEffect(sourceStat, targetStat, atkStat).Item1;
                RulesInBattleField.GetFieldEffect_CritDamage(atkStat,sourceStat,targetStat,ref critDmgAbility,ref critDmgBuff);
                critDmgModifier += (critDmgBuff + critDmgAbility);
            }
            
            
    
            float skillDmgModifier = 1;
            if (atkStat.attackType == AttackType.SKILL)
            {
                skillDmgModifier += sourceStat.skillDmgBuff;
                //TODO:检查技能伤害的被动。
                //TODO:检查技能伤害的场地效果。
            }

            float fsDmgModifier = 1;
            if (atkStat.attackType == AttackType.FORCE)
            {
                fsDmgModifier += sourceStat.fsDmgBuff;
                //TODO:检查FS伤害的被动。
                //TODO:检查FS伤害的场地效果。
            }

            //特攻
            var punisherBuff = CheckTotalPunisher(targetStat, sourceStat);
            var punisherAbility = CheckSpecialPunisherEffect(sourceStat, targetStat, atkStat).Item1;
            //TODO:检查特攻的场地效果。
            var punisherModifier = 1 + punisherBuff + punisherAbility;
            
            
    
            //Target
            //TODO: 检测目标的防御！

            var tarDefBuff = (targetStat.defenseBuff);
            var defAbility = CheckSpecialDefenseEffect(sourceStat, targetStat, atkStat).Item1;
            RulesInBattleField.GetFieldEffect_Defense(atkStat,sourceStat,targetStat,ref defAbility,ref tarDefBuff);
            var tarDef = targetStat.baseDef * (1 + tarDefBuff + defAbility);
            
            //TODO:检查目标的减伤
            var dmgCutBuff = targetStat.dmgCutBuff;
            var dmgCutAbility = CheckSpecialDamageCutEffect(sourceStat,targetStat,atkStat).Item1;
            RulesInBattleField.GetFieldEffect_DamageCut(atkStat,sourceStat,targetStat,ref dmgCutAbility,ref dmgCutBuff);
            var dmgCutModifier = dmgCutBuff + dmgCutAbility;
            //Debug.Log(dmgCutModifier);



            //Calculate
            
            //攻击者数值 : 总攻击(基础值*buff*被动) * 技能?伤害(目前只有buff) * 暴击?伤害(buff+被动) * 特攻修正 * 倍率
            var attackSource = atk * skillDmgModifier * fsDmgModifier * critDmgModifier * punisherModifier * newModifier;
            var defendTarget = tarDef;

            attackSource *= (1 - dmgCutModifier + dmgBuffModifier); //计算 减伤 + 增伤

            if (targetStat is SpecialStatusManager)
            {
                var specialTarget = targetStat as SpecialStatusManager;
                if (specialTarget.broken)
                {
                    //破防特效
                    defendTarget *= specialTarget.breakDefRate;
                    attackSource *= (1 + sourceStat.breakPunisher);
                }
            }


            var damage = 5f / 3f * (attackSource / defendTarget);
    
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
            //var atk = CalculateAttackInfo();
            
            var hp = targetStat.maxHP;
            var potencybuff = targetStat.recoveryPotencyBuff;
            potencybuff += CheckSpecialRecoveryBuff(targetStat).Item1;
    
            var damagePart1 = (0.16 * hp + 0.06 * atk) * modifier * (1 + potencybuff) * 0.012f;
    
            var damagePart2 = hp * percentageModifier * 0.01 * (1 + potencybuff);
            
            //Debug.Log("atk:"+atk);
    
            return (int)(damagePart1 + damagePart2);
            
            
            
        }



        #region SpecialConditionCalculation

        /// <summary>
        /// 处理一些条件下，buff生效条件与目标有关的判断。
        /// TODO:将StatusManger里的SpeicalConditionCheck移动到这里。
        /// </summary>
        /// <param name="sourceStat"></param>
        /// <param name="targetStat"></param>
        /// <param name="attackStat"></param>
        /// <returns></returns>
        public static Tuple<float, float, float> CheckSpecialAttackCondition(StatusManager sourceStat,
            StatusManager targetStat,
            AttackBase attackStat)
        {
            return new Tuple<float, float, float>(0, 0, 0);
        }


        #endregion

        #region SpecialAbilityCalculation
        
        public static Tuple<float,float,float> CheckSpecialAttackEffect(StatusManager sourceStat, StatusManager targetStat,
            AttackBase attackStat)
        {
            //float extraAttackModifier = 0;
            float buffModifier = 0;
            float debuffModifier = 0;
            
            if (sourceStat.GetAbility(10011))
            {
                //疾风怒涛攻
                if(sourceStat.comboHitCount >= 15)
                    buffModifier += 0.2f;
            }

            if (sourceStat.GetAbility(10006) ||
                sourceStat.GetAbility(20031)
                )
            {
                //巫女之祈愿
                buffModifier += (sourceStat.currentHp / sourceStat.maxHP) * 0.2f;
            }

            if (sourceStat.GetAbility(20032))
            {
                buffModifier += (sourceStat.currentHp / sourceStat.maxHP) * 0.2f;

                //反伤领域生效，自身的攻击力下降20%。
                if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
                {
                    debuffModifier += 0.2f;
                }

            }
            return new Tuple<float, float, float>(buffModifier-debuffModifier,buffModifier,debuffModifier);
        }

        public static Tuple<float,float,float> CheckSpecialDamageUpEffect(StatusManager sourceStat, StatusManager targetStat,
            AttackBase attackStat)
        {
            float buffModifier = 0;
            float debuffModifier = 0;
            
            //炽热之炎（legend）
            // if (sourceStat.GetConditionTotalValue((int)BattleCondition.ScorchingEnergy) >
            //     targetStat.GetConditionTotalValue((int)BattleCondition.ScorchingEnergy))
            // {
            //     buffModifier += 0.3f;
            // }
            
            return new Tuple<float, float, float>(buffModifier - debuffModifier, buffModifier, debuffModifier);
        }


        /// <summary>
        /// 对【目标】的防御进行判定。
        /// </summary>
        public static Tuple<float,float,float> CheckSpecialDefenseEffect(StatusManager sourceStat, StatusManager targetStat,
            AttackBase attackStat)
        {
            //float extraModifier = 0;
            float buffModifier = 0;
            float debuffModifier = 0;
            
            //巫女之祈愿（legend）
            if (targetStat.GetAbility(20032))
            {
                //攻击领域生效，自身的防御力下降20%。
                if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
                {
                    debuffModifier += 0.2f;
                }

            }

            return new Tuple<float, float, float>(buffModifier-debuffModifier,buffModifier,debuffModifier);
        }

        public static Tuple<int,int,int> CheckSpecialCritEffect(StatusManager sourceStat, StatusManager targetStat, 
            AttackBase attackStat)
        {
            int buffModifier = 0;
            int debuffModifier = 0;

            
            if (sourceStat.GetAbility(20011) || sourceStat.GetAbility(10009))//闪狼战技
            {
                if (attackStat.skill_id == 2 &&
                    targetStat.GetConditionStackNumber((int)BattleCondition.EvilsBane) > 0)
                {
                    buffModifier = 999;
                }
            }
            
            if (sourceStat.GetAbility(10008) && sourceStat.comboHitCount>= 15)
            {
                buffModifier += 15;
            }



            return new Tuple<int, int, int>(buffModifier-debuffModifier,buffModifier,debuffModifier);
        }
        
    
        public static Tuple<float,float,float> CheckSpecialCritDmgEffect(StatusManager sourceStat, StatusManager targetStat,
            AttackBase attackStat)
        {
            float buffModifier = 0;
            float debuffModifier = 0;

            if (sourceStat.GetAbility(10002))//暴击输出
            {
                if (targetStat.GetConditionStackNumber((int)BattleCondition.Flashburn) > 0)
                {
                    buffModifier += 0.3f;
                }
            }


            if (sourceStat.GetAbility(20011) || sourceStat.GetAbility(10009))//闪狼战技
            { 
                if (attackStat.attackType == AttackType.STANDARD &&
                  targetStat.GetConditionStackNumber((int)BattleCondition.EvilsBane) > 0)
                {
                    buffModifier += 0.2f;
                }
            }
            
            
    
            return new Tuple<float, float, float>(buffModifier-debuffModifier,buffModifier,debuffModifier);
        }

        public static Tuple<float, float, float> CheckSpecialRecoveryBuff(StatusManager sourceStat)
        {
            float buffModifier = 0;
            float debuffModifier = 0;

            if (sourceStat.GetAbility(80001))
            {
                var hpRatio = (float)sourceStat.currentHp / sourceStat.maxHP;
                buffModifier += (1 - hpRatio) * 1.5f;
            }



            return new Tuple<float, float, float>(buffModifier-debuffModifier,buffModifier,debuffModifier);
        }

        /// <summary>
        /// 对【目标】的减伤进行判定。
        /// </summary>
        public static Tuple<float,float,float> CheckSpecialDamageCutEffect(StatusManager sourceStat, StatusManager targetStat,
            AttackBase attackStat)
        {
            float buffModifier = 0;
            float debuffModifier = 0;
            
            //席菈（绯红幻影）
            if (targetStat.GetAbility(10009))
            {
                if ((sourceStat.GetConditionStackNumber((int)BattleCondition.Scorchrend) > 0 ||
                     sourceStat.GetConditionStackNumber((int)BattleCondition.Burn) > 0) &&
                    Projectile_C005_4.Instance != null)
                {
                    buffModifier += 0.3f;
                    Debug.Log("Damage Cut Effect");
                }

                
            }

            




            if (targetStat.GetAbility(20032))
            {
                buffModifier += Mathf.Pow(((float)targetStat.currentHp / (float)targetStat.maxHP),2) * 0.7f;
            }
            
            
            
            

            return new Tuple<float, float, float>(buffModifier-debuffModifier,buffModifier,debuffModifier);
        }

        public static Tuple<int, int, int> CheckSpecialDebuffRateEffect(StatusManager sourceStat,
            StatusManager targetStat, AttackBase attackStat)
        {
            int buffModifier = 0;
            int debuffModifier = 0;

            if (targetStat.GetAbility(10003))
            {
                if (targetStat.comboHitCount >= 15)
                {
                    buffModifier += 20;
                }
            }

            return new Tuple<int, int, int>(buffModifier - debuffModifier, buffModifier, debuffModifier);

        }

        public static Tuple<float, float, float> CheckSpecialPunisherEffect(StatusManager sourceStat,
            StatusManager targetStat, AttackBase attackStat)
        {
            float buffModifier = 0;
            float debuffModifier = 0;
            
            //防御力下降特效30%
            if (sourceStat.GetAbility(10004))
            {
                if (targetStat.GetConditionTotalValue((int)BasicCalculation.BattleCondition.DefDebuff) > 0)
                {
                    buffModifier += 0.3f;
                }
            }

            return  new Tuple<float, float, float>(buffModifier - debuffModifier, buffModifier, debuffModifier);
            
            
        }

        public static float CheckSpecialODAccerleratorEffect(StatusManager sourceStat, StatusManager targetStat,
            AttackBase attackStat)
        {
            float totalODAccerlerator = 0;

            if (sourceStat.GetAbility(80009))
            {
                //铳的补正
                totalODAccerlerator += 0.2f;
            }


            if (sourceStat.GetAbility(90001))
            {
                totalODAccerlerator += 2f;
            }

            return totalODAccerlerator;
        }


        #endregion

        
    
    
    
        public static float CalculateAttackInfo(AttackBase atkStat, StatusManager sourceStat,StatusManager targetStat)
        {
            var atkBuff = sourceStat.attackBuff;
            var atkAbility = CheckSpecialAttackEffect(sourceStat,targetStat,atkStat).Item1;
            RulesInBattleField.GetFieldEffect_Attack(atkStat,sourceStat,targetStat,ref atkAbility,ref atkBuff);
            var atk = sourceStat.baseAtk * (1 + atkBuff) * (1 + atkAbility);
            
            return atk;
        }

        public static float CalculateDamageUpInfo(AttackBase atkStat, StatusManager sourceStat,
            StatusManager targetStat)
        {
            var dmgBuff = sourceStat.dmgUpBuff;
            var dmgAbility = CheckSpecialDamageUpEffect(sourceStat, targetStat, atkStat).Item1;
            RulesInBattleField.GetFieldEffect_Damage(atkStat,sourceStat,targetStat,ref dmgAbility,ref dmgBuff);
            var dmgBuffModifier = dmgAbility + dmgBuff;
            return dmgBuffModifier;
        }

        public static float CalculateDefenseInfo(StatusManager stat)
        {
            var totalDef = stat.baseDef * (1 + stat.defenseBuff);
            if (stat is SpecialStatusManager)
            {
                var spStat = (stat as SpecialStatusManager);
                if (spStat.broken)
                {
                    totalDef *= spStat.breakDefRate;
                }
            }

            return totalDef;
            
        }


        private static float CheckTotalPunisher(StatusManager targetStat, StatusManager sourceStat)
        {
            int totalAffliction = 0;
            float totalBuff = 0;
            if (targetStat.GetConditionStackNumber((int)BattleCondition.Burn) > 0)
            {
                totalBuff += sourceStat.burnPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Poison) > 0)
            {
                totalBuff += sourceStat.poisonPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Freeze) > 0)
            {
                totalBuff += sourceStat.freezePunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Flashburn) > 0)
            {
                totalBuff += sourceStat.flashburnPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.ShadowBlight) > 0)
            {
                totalBuff += sourceStat.shadowblightPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Blindness) > 0)
            {
                totalBuff += sourceStat.blindnessPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Frostbite) > 0)
            {
                totalBuff += sourceStat.frostbitePunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Scorchrend) > 0)
            {
                totalBuff += sourceStat.scorchrendPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Paralysis) > 0)
            {
                totalBuff += sourceStat.paralysisPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Stormlash) > 0)
            {
                totalBuff += sourceStat.stormlashPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Bog) > 0)
            {
                totalBuff += sourceStat.bogPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.Stun) > 0)
            {
                totalBuff += sourceStat.stunPunisher;
                totalAffliction++;
            }
            if(targetStat.GetConditionStackNumber((int)BattleCondition.SleepPunisher) > 0)
            {
                totalBuff += sourceStat.sleepPunisher;
                totalAffliction++;
            }

            
            totalBuff += sourceStat.conditionPunisher;

            if (targetStat.GetConditionStackNumber((int)BattleCondition.AfflictedPunisher) > 0)
            {
                if (totalAffliction >= 4)
                {
                    totalBuff += 0.4f;
                }
                else
                {
                    totalBuff += totalAffliction * 0.05f + 0.2f;
                }
            }
            
            //Debug.Log("total_punisher:"+totalBuff);

            return totalBuff;

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
        
    
        /// <summary>
        /// 获取当前动画的最后一帧的标准时间。
        /// </summary>
        /// <returns></returns>
        public static float GetLastAnimationNormalizedTime(Animator anim)
        {
            float totalFrame = (anim.GetCurrentAnimatorClipInfo(0)[0].clip.length*anim.GetCurrentAnimatorClipInfo(0)[0].clip.frameRate);
            return (1 - 1 / totalFrame);
        }
    
        public static string ConvertID(string str, int id)
        {
            if (id < 10)
            {
                return $"{str}00{id}";
            }
    
            if (id < 100)
            {
                return $"{str}0{id}";
            }
    
            if (id < 1000)
            {
                return $"{str}{id}";
            }
    
            return "NULL";
    
    
        }
    
        /// <summary>
        /// 输入六位数ID，返回字符串
        /// </summary>
        /// <param name="questID"></param>
        /// <returns></returns>
        public static string GetQuestNameZH(string questID)
        {
            //01 001 3:席菈的试炼 超级
            var idStr = questID;
            var sb = new StringBuilder();
            //Debug.Log(questID);
    
            switch (idStr[2..4])
            {
                case "001":
                    sb.Append("席菈的试炼 ");
                    break;
                    
                default: break;
            }
    
            switch (idStr[5])
            {
                case '1':
                    sb.Append("中级");
                    break;
                case '2':
                    sb.Append("高级");
                    break;
                case '3':
                    sb.Append("超级");
                    break;
                case '4':
                    sb.Append("绝级");
                    break;
            }
    
            return sb.ToString();
        }
        public static string GetQuestNameJP(string questID)
        {
            //01 001 3:シーラの試練 超級
            var idStr = questID;
            var sb = new StringBuilder();
    
            switch (idStr[2..4])
            {
                case "001":
                    sb.Append("シーラの試練 ");
                    break;
                    
                default: break;
            }
    
            switch (idStr[5])
            {
                case '1':
                    sb.Append("中級");
                    break;
                case '2':
                    sb.Append("高級");
                    break;
                case '3':
                    sb.Append("超級");
                    break;
                case '4':
                    sb.Append("絶級");
                    break;
            }
    
            return sb.ToString();
        }
    
        /// <summary>
        /// Read File From StreamingAssets
        /// </summary>
        /// <param name="name">Path in StreamingAsset</param>
        /// <returns></returns>
        public static JsonData ReadJsonData(string name)
        {
            string path = Application.streamingAssetsPath + "/"+ name;
            StreamReader sr = new StreamReader(path);
            var str = sr.ReadToEnd();
            sr.Close();
            return JsonMapper.ToObject(str);
        }
    
        public static string ToTimerFormat(float seconds)
        {
            if (seconds <= 0)
                return "00:00";
            
            var min = (int)Mathf.Floor(seconds / 60);
            var sec = (int)seconds % 60;
            return $"{min:D2}:{sec:D2}";
        }
        
        public static Collider2D CheckRaycastedPlatform(GameObject target)
        {
            var _groundSensor = target.GetComponentInChildren<IGroundSensable>();
            

            GameObject myGround;

            try
            {
                myGround = _groundSensor.GetCurrentAttachedGroundInfo();
            }
            catch
            {
                _groundSensor = target.transform.parent.GetComponentInChildren<IGroundSensable>();
                myGround = _groundSensor?.GetCurrentAttachedGroundInfo();
            }

            if(myGround==null)
            {
                RaycastHit2D myRayL = 
                    Physics2D.Raycast(target.transform.position + new Vector3(-1,0,0), Vector2.down,
                        999f,LayerMask.GetMask("Ground","Platforms"));
                RaycastHit2D myRayR = 
                    Physics2D.Raycast(target.transform.position + new Vector3(1,0,0), Vector2.down,
                        999f,LayerMask.GetMask("Ground","Platforms"));
        
                var myGround1 = myRayL.collider.gameObject;
                var myGround2 = myRayR.collider.gameObject;
                if(myGround1 == myGround2)
                    myGround = myGround1;
                else
                    myGround = myGround1.transform.position.y > myGround2.transform.position.y ? myGround1 : myGround2;
            
            }

            return myGround.GetComponentInChildren<Collider2D>();
        }

        public static float GetRaycastedPlatformY(Vector2 position)
        {
            RaycastHit2D myRay = 
                Physics2D.Raycast(position, Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
            
            var myGround = myRay.collider.gameObject;
            
            if (myGround == null)
                return 0;
            
            var col = myGround.GetComponentInChildren<Collider2D>();
            
            //var distance = col.bounds.max.y - selfCollider.bounds.min.y;

            return col.bounds.max.y;
        }

        public static float GetRaycastedPlatformY(GameObject target)
        {
            var _groundSensor = target.GetComponentInChildren<IGroundSensable>();
            GameObject myGround;
            var selfCollider = _groundSensor.GetSelfCollider();
            try
            {
                myGround = _groundSensor.GetCurrentAttachedGroundInfo();
            }
            catch
            {
                _groundSensor = target.transform.parent.GetComponentInChildren<IGroundSensable>();
                myGround = _groundSensor.GetCurrentAttachedGroundInfo();
            }

            if(myGround==null)
            {
                RaycastHit2D myRayL = 
                    Physics2D.Raycast(new Vector2(selfCollider.bounds.min.x,selfCollider.bounds.min.y), Vector2.down,
                        999f,LayerMask.GetMask("Ground","Platforms"));
                RaycastHit2D myRayR = 
                    Physics2D.Raycast(new Vector2(selfCollider.bounds.max.x,selfCollider.bounds.min.y), Vector2.down,
                        999f,LayerMask.GetMask("Ground","Platforms"));
        
                var myGround1 = myRayL.collider.gameObject;
                var myGround2 = myRayR.collider.gameObject;
                if(myGround1 == myGround2)
                    myGround = myGround1;
                else
                    myGround = myGround1.transform.position.y > myGround2.transform.position.y ? myGround1 : myGround2;
            }

            if (myGround == null)
                return 0;
            
            var col = myGround.GetComponentInChildren<Collider2D>();
            
            //var distance = col.bounds.max.y - selfCollider.bounds.min.y;

            return col.bounds.max.y;

        }

        public static bool CheckOnSameRaycastPlatform(GameObject target1, GameObject target2)
        {
            return CheckRaycastedPlatform(target1) == CheckRaycastedPlatform(target2);
        }

        static Vector2 ClosestPointOnLineSegment(Vector2 point, Vector2 lineStart, Vector2 lineEnd) 
        {
            Vector2 lineDirection = (lineEnd - lineStart).normalized;
            float distanceFromLineStartToPoint = Vector2.Dot(point - lineStart, lineDirection);
            if (distanceFromLineStartToPoint < 0) {
                return lineStart;
            }
            if (distanceFromLineStartToPoint > Vector2.Distance(lineStart, lineEnd)) {
                return lineEnd;
            }
            return lineStart + lineDirection * distanceFromLineStartToPoint; 
        }

        private static bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, out Vector2 intersection) {
        intersection = Vector2.zero;

        float s1_x = p2.x - p1.x;
        float s1_y = p2.y - p1.y;
        float s2_x = p4.x - p3.x;
        float s2_y = p4.y - p3.y;

        float s = (-s1_y * (p1.x - p3.x) + s1_x * (p1.y - p3.y)) / (-s2_x * s1_y + s1_x * s2_y);
        float t = (s2_x * (p1.y - p3.y) - s2_y * (p1.x - p3.x)) / (-s2_x * s1_y + s1_x * s2_y);

        if (s >= 0 && s <= 1 && t >= 0 && t <= 1) {
            intersection.x = p1.x + (t * s1_x);
            intersection.y = p1.y + (t * s1_y);
            return true;
        }

        return false;
    }

        public static List<Vector2> GetIntersectionOfPolygonCollider(Vector2 pointA, Vector2 pointB, PolygonCollider2D collider)
        {
            List<Vector2> intersectionPoints = new List<Vector2>();
            Vector2[] points = collider.points;
            for (int i = 0; i < points.Length; i++) 
            {
                Vector2 edgeStart = collider.transform.TransformPoint(points[i]);
                Vector2 edgeEnd = collider.transform.TransformPoint(points[(i + 1) % points.Length]);
                Vector2 intersectionPoint;
                if (LineSegmentsIntersection(pointA, pointB, edgeStart, edgeEnd, out intersectionPoint)) {
                    intersectionPoints.Add(intersectionPoint); 
                }
            }

            return intersectionPoints;

        }
    
        public static List<Vector2> GetIntersectionOfBoxCollider(Vector2 pointA, Vector2 pointB, BoxCollider2D collider)
        {
            List<Vector2> intersectionPoints = new List<Vector2>();
    
            Vector2 size = collider.size;
            Vector2 center = collider.transform.TransformPoint(collider.offset);
            Vector2 topLeft = center + new Vector2(-size.x / 2, size.y / 2);
            Vector2 topRight = center + new Vector2(size.x / 2, size.y / 2);
            Vector2 bottomLeft = center + new Vector2(-size.x / 2, -size.y / 2);
            Vector2 bottomRight = center + new Vector2(size.x / 2, -size.y / 2);
            Vector2[] points = new Vector2[] { topLeft, topRight, bottomRight, bottomLeft };
            for (int i = 0; i < points.Length; i++) {
                Vector2 edgeStart = points[i];
                Vector2 edgeEnd = points[(i + 1) % points.Length];
                Vector2 intersectionPoint;
                if (LineSegmentsIntersection(pointA, pointB, edgeStart, edgeEnd, out intersectionPoint)) {
                    intersectionPoints.Add(intersectionPoint);
                }
            }
            return intersectionPoints; 
        }

        public static List<Vector2> GetIntersectionOfCircleCollider(Vector2 pointA,Vector2 pointB, CircleCollider2D collider)
        {
            List<Vector2> intersectionPoints = new List<Vector2>();

            Vector2 center = collider.transform.TransformPoint(collider.offset);
            float radius = collider.radius;
            float distanceFromCenterToLine = Vector2.Distance(center, ClosestPointOnLineSegment(center, pointA, pointB));
            if (distanceFromCenterToLine < radius) {
                float distanceFromCenterToIntersection = Mathf.Sqrt(radius * radius - distanceFromCenterToLine * distanceFromCenterToLine);
                Vector2 lineDirection = (pointB - pointA).normalized;
                Debug.Log("Line Direction: " + lineDirection + "");
                Vector2 intersectionPoint1 = center - lineDirection * distanceFromCenterToIntersection;
                Vector2 intersectionPoint2 = center + lineDirection * distanceFromCenterToIntersection;
                
                var height = (pointB.y - pointA.y);
                var length = (pointB.x - pointA.x);
                var normalizedPosition1 = (intersectionPoint1.x - pointA.x) / length;
                var normalizedPosition2 = (intersectionPoint2.x - pointA.x) / length;
                
                intersectionPoints.Add(new Vector2(intersectionPoint1.x, pointA.y + height*normalizedPosition1));
                intersectionPoints.Add(new Vector2(intersectionPoint2.x, pointA.y + height*normalizedPosition2));
            }
            return intersectionPoints;
        }

        public static RaycastHit2D GetPositionOfAttackEnemyBelow(Vector2 currentPos)
        {
            var raycastHit2D = Physics2D.Raycast(currentPos, Vector2.down, 999f, LayerMask.GetMask("AttackEnemy"));
            return raycastHit2D;
        }
        
        /// <summary>
        /// 求两个有序区间的交集
        /// </summary>
        /// <param name="listA"></param>
        /// <param name="listB"></param>
        /// <returns></returns>
        public static List<(float, float)> GetTupleIntersection(List<(float, float)> listA, List<(float, float)> listB)
        {
            var result = new List<(float, float)>();
            int i = 0, j = 0;
            while (i < listA.Count && j < listB.Count)
            {
                (float startA, float endA) = listA[i];
                (float startB, float endB) = listB[j];
                if (endA < startB)
                    i++;
                else if (endB < startA)
                    j++;
                else
                {
                    result.Add((Mathf.Max(startA, startB), Mathf.Min(endA, endB)));
                    if (endA < endB)
                        i++;
                    else
                        j++;
                }
            }
            return result;
        }
    
        public static float HasGap(Collider2D a, Collider2D b)
        {
            var leftA = a.bounds.min.x;
            var leftB = b.bounds.min.x;
            var rightA = a.bounds.max.x;
            var rightB = b.bounds.max.x;
        
            if(leftA > rightB || leftB > rightA)
            {
                return leftA > rightB ? leftA - rightB : leftB - rightA;
            }
            else
            {
                return 0;
            }
        }

    }

    public static class RulesInBattleField
    {
        public static int GetFieldResistanceOfCondition(BasicCalculation.BattleCondition condition)
        {
            var abilityIDList = BattleStageManager.Instance.FieldAbilityIDList;
            
            
            return -1;
            //Unimplemented
        }

        public static void GetFieldEffect_Attack(AttackBase atkStat,
            StatusManager sourceStat, StatusManager targetStat, ref float ability, ref float buff)
        {
            float totalAbilityUp = 0,totalAbilityDown = 0;
            float totalBuffUp = 0, totalBuffDown = 0;
            
            //巫女的祈愿legend
            if (sourceStat.GetAbility(20032))
            {
                //因果链接生效期间，自身攻击力降低30%
                if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20033))
                {
                    totalAbilityDown += 0.3f;
                }
            }
            
            ability = ability + totalAbilityUp - totalAbilityDown;
            buff = buff + totalBuffUp - totalBuffDown;
        }

        public static void GetFieldEffect_CritRate(AttackBase atkStat,
                     StatusManager sourceStat, StatusManager targetStat, ref float ability, ref float buff)
                 {
                     float totalAbilityUp = 0,totalAbilityDown = 0;
                     float totalBuffUp = 0, totalBuffDown = 0;
                     
                     
                     
                     
                     ability = ability + totalAbilityUp - totalAbilityDown;
                     buff = buff + totalBuffUp - totalBuffDown;
                     
                 }

        
        public static void GetFieldEffect_CritDamage(AttackBase atkStat,
            StatusManager sourceStat, StatusManager targetStat, ref float ability, ref float buff)
        {
            float totalAbilityUp = 0,totalAbilityDown = 0;
            float totalBuffUp = 0, totalBuffDown = 0;
            
            
            
            
            ability = ability + totalAbilityUp - totalAbilityDown;
            buff = buff + totalBuffUp - totalBuffDown;
            
        }

        public static void GetFieldEffect_Defense(AttackBase atkStat,
            StatusManager sourceStat, StatusManager targetStat, ref float ability, ref float buff)
        {
            float totalAbilityUp = 0,totalAbilityDown = 0;
            float totalBuffUp = 0, totalBuffDown = 0;
            
            //巫女的祈愿legend
            if (targetStat.GetAbility(20032))
            {
                //魔力增幅生效期间，自身防御力降低30%。
                if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20034))
                {
                    totalAbilityDown += 0.3f;
                }
            }
            
            //无视防御
            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20034))
            {
                totalAbilityDown += BasicCalculation.CheckSpecialDefenseEffect(sourceStat,targetStat,atkStat).Item3;
                ability = 0;
                buff = 0;
                totalBuffDown = 0.01f*targetStat.GetDefenseBuff(2);
            }

            ability = ability + totalAbilityUp - totalAbilityDown;
            buff = buff + totalBuffUp - totalBuffDown;

        }
        public static void GetFieldEffect_Damage(AttackBase atkStat,
            StatusManager sourceStat, StatusManager targetStat, ref float ability, ref float buff)
        {
            
            float totalAbilityUp = 0,totalAbilityDown = 0;
            float totalBuffUp = 0, totalBuffDown = 0;
            
            //因果链接魔力增幅:攻击威力+30%
            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20034))
            {
                totalAbilityUp += 0.3f;
            }
            
            
            ability = ability + totalAbilityUp - totalAbilityDown;
            buff = buff + totalBuffUp - totalBuffDown;
            
        }
        public static void GetFieldEffect_DamageCut(AttackBase atkStat,
            StatusManager sourceStat, StatusManager targetStat, ref float ability, ref float buff)
        {
            
            float totalAbilityUp = 0,totalAbilityDown = 0;
            float totalBuffUp = 0, totalBuffDown = 0;
            
            //无视减伤
            if (BattleStageManager.Instance.FieldAbilityIDList.Contains(20034))
            {
                totalAbilityDown = BasicCalculation.CheckSpecialDamageCutEffect(sourceStat,targetStat,atkStat).Item3;
                //totalBuffDown = 0.01f*targetStat.GetConditionTotalValue((int)(BasicCalculation.BattleCondition.Vulnerable));
                totalBuffDown = 0.01f*targetStat.GetDamageCut(2);
                ability = 0;
                buff = 0;
            }
            
            ability = ability + totalAbilityUp - totalAbilityDown;
            buff = buff + totalBuffUp - totalBuffDown;
        }





    }
}
