using System;
using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C029 : AttackManagerDagger
{

    [SerializeField] private GameObject skill1FXUpgrade;
    
    protected void Skill1Upgrade()
    {
        
        _statusManager.InspirationLevelUp(1);
        
        var buffList =
            _statusManager.
                GetExactConditionsOfType((int)(BasicCalculation.BattleCondition.SkillUpgrade), 102901);

        TimerBuff buff;
        if (buffList.Count == 0)
        {
            buff = new TimerBuff((int)BasicCalculation.BattleCondition.SkillUpgrade,
                1, -1, 1, 102901);
            
            _statusManager.ObtainTimerBuff(buff);
            return;
        }
        
        
        buff = buffList[0] as TimerBuff;

        if (buff.effect >= 2)
        {
            _statusManager.HPRegenImmediately(30,0,true);
            _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.SkillUpgrade, 102901);
            _statusManager.OnSpecialBuffDelegate?.
                Invoke(UI_BuffLogPopManager.SpecialConditionType.SkillUpgradedReset.ToString());
        }
        else
        {
            var newbuff = new TimerBuff((int)BasicCalculation.BattleCondition.SkillUpgrade,
                buff.effect+1, -1, 1, 102901);
            
            _statusManager.ObtainTimerBuff(newbuff);
        }


    }

    protected void Skill3Inspiration()
    {
        _statusManager.InspirationLevelUp(1);
    }

    public override void Skill1(int eventID)
    {
        var container = Instantiate(attackContainer,
            transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        
        container.GetComponent<AttackContainer>().InitAttackContainer(1,true);

        var skillLevel = GetSkillUpgradeLevel(102901);
        
        
        var proj = InstantiateMeele(skillLevel < 2 ? skill1FX : skill1FXUpgrade,transform.position,container);

        var atk = proj.GetComponent<AttackFromPlayer>();

        var paralysis = new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,
            88.3f,13f,100);

        //注册技能升级事件
        AttackBase.AttackBaseDelegate handler = null;
        handler = (@base, target) =>
        {
            Skill1Upgrade();
            atk.OnAttackHit -= handler;
        };
        atk.OnAttackHit += handler;


        //检查技能升级
        var buffList =
            _statusManager.GetExactConditionsOfType
                ((int)(BasicCalculation.BattleCondition.SkillUpgrade), 102901);

        int effLvl = 0;
        if (buffList.Count > 0)
        {
            effLvl = (int)buffList[0].effect;
        }

        if (effLvl > 0)
        {
            atk.AddWithConditionAll(paralysis,160);
            atk.attackInfo[0].knockbackPower += 30;
        }
        else
        {
            atk.AddWithConditionAll(paralysis,110);
        }

        //注册麻痹特攻
        
        var checkConditionString = ((int)BasicCalculation.BattleCondition.Paralysis).ToString();
        
        var conditional_eff = new ConditionalAttackEffect
        (ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {"1", checkConditionString},
            new string[] {"0.3"});
        
        atk.AddConditionalAttackEffect(conditional_eff);
        

    }

    public override void Skill2(int eventID)
    {
        _statusManager.InspirationLevelUp(3);
        var spAttackBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                30,10, 1, 102902);
        _statusManager.ObtainTimerBuff(spAttackBuff,false);
        (_statusManager as PlayerStatusManager).FillSP(0,100);
        _statusManager.OnSpecialBuffDelegate?.
            Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        Instantiate(skill2FX,transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
    }
    
    public override void Skill3(int eventID)
    {
        if (eventID == 1)
        {
            //向前发出一道2D射线，如果击中敌人，获取碰撞点的位置
            
            var hit = Physics2D.Raycast(transform.position,Vector2.right* ac.facedir, 10,
                LayerMask.GetMask("Enemies"));
            Vector2 hitPos = transform.position;

            if (hit.collider != null && Mathf.Abs(transform.position.x - hit.point.x) > 2.5f)
            {
                //print("打到了");
                hitPos = BattleStageManager.Instance.OutOfRangeCheck(hit.point - new Vector2(ac.facedir * 2.5f,0));
            }else if (hit.collider == null)
            {
                //print("没打到");
                hitPos = BattleStageManager.Instance.OutOfRangeCheck(transform.position + new Vector3(ac.facedir * 2.5f,0));
            }

            //使用Dotween在0.5f内移动到该位置
            //print(hitPos.x);

            transform.DOMoveX(hitPos.x, 0.4f);
            
            
            
            var container = Instantiate(attackContainer,
                transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
            
            container.GetComponent<AttackContainer>().InitAttackContainer(1,true);
            var proj = InstantiateMeele(skill3FX,transform.position,container);
            var proj_controller = proj.GetComponent<Projectile_C029_1>();
            if (_statusManager.GetExactConditionsOfType
                ((int)BasicCalculation.BattleCondition.AtkBuff,
                    102902).Count > 0)
            {
                proj_controller.extraAttack = true;
            }
            var atk = proj.GetComponent<AttackFromPlayer>();

            AttackBase.AttackBaseDelegate handler = null;
            handler = (@base, target) =>
            {
                Skill3Inspiration();
                atk.OnAttackHit -= handler;
            };
            atk.OnAttackHit += handler;
            
        }
    }
    
}
