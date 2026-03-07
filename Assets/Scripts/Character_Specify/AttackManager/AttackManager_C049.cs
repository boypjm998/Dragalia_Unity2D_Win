using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C049 : AttackManagerMeeleWithFS
{
    private GameObject skill1Container;
    private bool skill1Connect = false;
    private TargetAimer ta;
    private TimerBuff _facelessMoon = new TimerBuff
        ((int)BasicCalculation.BattleCondition.FacelessMoon, 1, 30, 1);
    private TimerBuff _vulnerable = new TimerBuff
        ((int)BasicCalculation.BattleCondition.Vulnerable, 5, 20, 1, 104902);
    
    private TimerBuff _atkdebuff = new TimerBuff
        ((int)BasicCalculation.BattleCondition.AtkDebuff, 20, 10, 1, 104903);
    private TimerBuff _atkdebuff2 = new TimerBuff
        ((int)BasicCalculation.BattleCondition.AtkDebuff, 10, 10, 1, 104904);
    private TimerBuff _defdebuff = new TimerBuff
        ((int)BasicCalculation.BattleCondition.DefDebuff, 5, 10, 1, 104904);

    private int skillLevel;
    
    private Tween _skill2Tween;
    
    protected override void Start()
    {
        base.Start();
        ta = (ac as ActorController).ta;
    }

    public void Skill1_Muzzle1()
    {
        var muzzle = InstantiateBuff(skillFX[1], transform.position);
        
        Invoke("Skill1_Attack2",0.1f);
        
    }
    
    public void Skill1_Muzzle2(bool firstAttack)
    {
        var muzzle = InstantiateBuff(skillFX[2], transform.position);
        if (firstAttack)
        {
            Invoke("Skill1_Attack",0.1f);
        }
        else
        {
            Invoke("Skill1_Attack2",0.1f);
        }
    }

    private void Skill1_Attack()
    {
        skill1Container = InitContainer(false, 1,true);

        var proj = InstantiateDirectionalRanged(skillFX[0],
            transform.position + new Vector3(ac.facedir * 2, 0),
            skill1Container, ac.facedir, 0);

        var atk = proj.GetComponent<AttackFromPlayer>();
        
        //注册技能升级事件
        skill1Connect = false;
        AttackBase.AttackBaseDelegate handler = null;
        handler = (@base, target) =>
        {
            if(skill1Connect)
                return;
            SkillUpgrade(104901);
            skill1Connect = true;
            (_statusManager as PlayerStatusManager).FillSP(2,35);
            _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
            atk.OnAttackHit -= handler;
        };
        atk.OnAttackHit += handler;

        skillLevel = GetSkillUpgradeLevel(104901);
        atk.AddWithConditionAll(new TimerBuff(999),100,2);
        

        if (skillLevel == 1)
        {
            atk.AddWithConditionAll(_atkdebuff2,100);
            atk.attackInfo[0].dmgModifier[0] *= (23f / 22f);
            proj.transform.localScale *= 1.1f;
        }else if (skillLevel >= 2)
        {
            atk.AddWithConditionAll(_atkdebuff2,100);
            atk.AddWithConditionAll(_defdebuff,100,1);
            atk.attackInfo[0].dmgModifier[0] *= (24f / 22f);
            proj.transform.localScale *= 1.2f;
        }
        
        

    }
    
    private void Skill1_Attack2()
    {
        var proj = InstantiateDirectionalRanged(skillFX[0],
            transform.position + new Vector3(ac.facedir * 2, 0),
            skill1Container, ac.facedir, 0);

        var atk = proj.GetComponent<AttackFromPlayer>();
        
        AttackBase.AttackBaseDelegate handler = null;
        handler = (@base, target) =>
        {
            if (skill1Connect)
            {
                atk.OnAttackHit -= handler;
                return;
            }
            skill1Connect = true;
            SkillUpgrade(104901);
            (_statusManager as PlayerStatusManager).FillSP(2,35);
            _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
            atk.OnAttackHit -= handler;
        };
        atk.OnAttackHit += handler;

        atk.AddWithConditionAll(new TimerBuff(999),100,2);

        if (skillLevel == 1)
        {
            atk.AddWithConditionAll(_atkdebuff2,100);
            atk.attackInfo[0].dmgModifier[0] *= (23f / 22f);
            proj.transform.localScale *= 1.1f;
        }else if (skillLevel >= 2)
        {
            atk.AddWithConditionAll(_atkdebuff2,100);
            atk.AddWithConditionAll(_defdebuff,100,1);
            atk.attackInfo[0].dmgModifier[0] *= (24f / 22f);
            proj.transform.localScale *= 1.2f;
        }
        
        
    }

    public void Skill2_Attack()
    {
        var target = ta.GetNearestTargetInRangeDirection(ac.facedir, 20, 7,
            LayerMask.GetMask("Enemies"));
        Vector2 targetPos;
        if(target != null)
        {
            targetPos = target.gameObject.RaycastedPosition();
        }
        else
        {
            targetPos = gameObject.RaycastedPosition();
        }
        
        var container = InitContainer(false, 1,true);
        
        var proj = InstantiateRanged
            (skillFX[3], targetPos,
                container,1);
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(new TimerBuff(_vulnerable),100);
        
        var facelessMoonLevel = _statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.FacelessMoon);

        if (facelessMoonLevel == 1)
        {
            _skill2Tween = DOVirtual.DelayedCall(0.4f, () =>
            {
                Skill2_ChildAttack(container);
            },false);
        }else if (facelessMoonLevel >= 2)
        {
            _skill2Tween = DOVirtual.DelayedCall
            (0.3f, () =>
            {
                Skill2_ChildAttack(container);
            },false).OnComplete(() =>
            {
                _skill2Tween = DOVirtual.DelayedCall(0.2f, () =>
                {
                    Skill2_ChildAttack(container);
                },false);
            });
        }
        
        
    }

    private void Skill2_ChildAttack(GameObject container)
    {
        var target = ta.GetNearestTargetInRangeDirection(ac.facedir, 20, 7,
            LayerMask.GetMask("Enemies"));
        Vector2 targetPos;
        if(target != null)
        {
            targetPos = target.gameObject.RaycastedPosition();
        }
        else
        {
            targetPos = gameObject.RaycastedPosition();
        }
        
        var proj = InstantiateRanged
        (skillFX[4], targetPos + new Vector2(Random.Range(-0.25f,0.25f),0),
            container,1);
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(new TimerBuff(_vulnerable),100);
    }

    public void Skill3_Prepare()
    {
        var proj = InstantiateRanged(skillFX[5],
            transform.position,
            InitContainer(false,1,true), ac.facedir);
        
        var atk = proj.GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(new TimerBuff(_atkdebuff),100);
        
        Invoke("Skill3_Buff",1.5f);
    }

    private void Skill3_Buff()
    {
        var currentBuff =
            _statusManager.GetConditionOfTypeWithMaxEffect((int)BasicCalculation.BattleCondition.FacelessMoon);

        if (currentBuff == null)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff(_facelessMoon));
        }
        else
        {
            if (currentBuff.effect == 1)
            {
                currentBuff.SetEffect(2);
                currentBuff.SetDuration(30);
                currentBuff.lastTime = 30;
                _statusManager.OnBuffEventDelegate?.Invoke(currentBuff);
            }
            else if (currentBuff.effect >= 2)
            {
                currentBuff.SetEffect(3);
                currentBuff.SetDuration(60);
                currentBuff.lastTime = 60;
                _statusManager.OnBuffEventDelegate?.Invoke(currentBuff);
            }
        }



    }
    
    
    
    
}
