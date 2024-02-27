using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C009 : AttackManagerRanged
{
    [SerializeField] private float delayingTime = 0.2f;
    private UI_ForceStrikeAimerTargeting forceStrikeIndicator;

    protected override void Start()
    {
        base.Start();
        (ac as ActorController).pi.buttonAttack.delayingDuration = delayingTime;
        
    }

    public void Skill1_Attack()
    {
        AttackBase.AttackBaseDelegate eventHandler = null;

        bool flag = false;

        eventHandler = (atk,target) =>
        {
            atk.OnAttackHit -= eventHandler;

            if (!flag)
            {
                flag = true;
                SkillUpgrade(100901);
            }
            
        };
        
        //AttackFromPlayer attackFromPlayer = null;

        
        
        var laserCount = 3 + GetSkillUpgradeLevel(100901);
        
        var enemiesInRange = ta.GetAllEnemiesWithMarkingCheck();
        enemiesInRange.Sort((a, b) =>
        {
            var distance1 = Vector2.Distance(a.position, transform.position);
            var distance2 = Vector2.Distance(b.position, transform.position);

            if (distance1 > distance2)
                return 1;
            else if (distance1 < distance2)
                return -1;
            else return 0;

        });

        var targetList = enemiesInRange.GetMultipleTargetDistributionList(laserCount);
        
        //print(targetList.Count);

        //List<Tween> tweens = new();
        float interval = .2f;

        var container = InitContainer(false,laserCount,true);

        for (int i = 0; i < laserCount; i++)
        {
            var index = i;
            DOVirtual.DelayedCall(interval * (index + 1),() =>
            {
                if (targetList == null)
                {
                    var proj = InstantiateRanged(skill1FX[0], 
                        gameObject.RaycastedPosition(), container, 1);
                    return;
                }
                
                var target = targetList[index];
                
                
                if (target != null)
                {
                    var proj = InstantiateRanged(skill1FX[0], target.RaycastedPosition(), container, 1);
                    proj.GetComponent<ForcedAttackFromPlayer>().target = target;
                    proj.GetComponent<AttackFromPlayer>().OnAttackHit += eventHandler;
                }
                else
                {
                    var proj = InstantiateRanged(skill1FX[0], gameObject.RaycastedPosition(), container, 1);
                    proj.GetComponent<AttackFromPlayer>().OnAttackHit += eventHandler;
                }
                
                
                
                
            },false);
        }

        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AlteredStrikeCleo,
            1, -1, 1,-1);

        //SkillUpgrade(100901);


    }

    public void Skill2_Attack()
    {
        AttackBase.AttackBaseDelegate eventHandler = null;

        bool flag = false;

        eventHandler = (atk,target) =>
        {
            atk.OnAttackHit -= eventHandler;

            if (!flag)
            {
                flag = true;
                SkillUpgrade(100903);
            }
            
        };
        
        var target = ta.GetNearestTargetInRangeDirection
            (ac.facedir, 20, 8, LayerMask.GetMask("Enemies"));

        var atk = InstantiateRanged(skill2FX[0],
            target == null ? gameObject.RaycastedPosition() : target.gameObject.RaycastedPosition(),
            InitContainer(false, 1, true), 1).GetComponent<AttackFromPlayer>();
        
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,10,20,100,-1),
            100);

        atk.OnAttackHit += eventHandler;

        var level = GetSkillUpgradeLevel(100903);

        if (level >= 1)
        {
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,10,20,
                100,-1),100,1);
        }

        if (level >= 2)
        {
            _statusManager.HPRegenImmediately(23,0,true);
            _statusManager.ObtainHealOverTimeBuff(17,20,false);
        }

        //SkillUpgrade(100903);

    }

    public void Skill3_Attack()
    {
        _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            25, 20, 100));

        var container = InitContainer(false, 2, true);

        var proj1 = 
            InstantiateRanged(skill3FX[0], transform.position, container, 1);
        var atk1 = proj1.GetComponent<AttackFromPlayer>();
        
        var proj2 = 
            InstantiateRanged(skill3FX[0], transform.position, container, -1);
        var atk2 = proj2.GetComponent<AttackFromPlayer>();
        atk2.attackInfo[0].knockbackDirection.x = -1;
        
        
        atk1.AddWithConditionAll(new TimerBuff(999),100);
        atk2.AddWithConditionAll(new TimerBuff(999),100);
        

        if (_statusManager.GetConditionWithSpecialID(100902).Count > 0)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
                1, 20, 1),false);
            atk1.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,20,20,-1),100,1);
            atk2.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.AtkDebuff,20,20,-1),100,1);
        }
        
    }

    public void Skill4()
    {
        _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            15, 60, 1, 100904), true);
        
        _statusManager.HPRegenImmediately(110,0,true);
        
        (_statusManager as PlayerStatusManager).FillSP(0,15);
        (_statusManager as PlayerStatusManager).FillSP(1,15);
        (_statusManager as PlayerStatusManager).FillSP(2,15);
        (_statusManager as PlayerStatusManager).FillSP(3,15);
        
        _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());

        _statusManager.ReliefOneAffliction();

        if (_statusManager.GetConditionWithSpecialID(100902).Count > 0)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.Shield,
                100, -1, 1, 0),false);
        }
    }
    

    public void ForceStrikeCharging()
    {
        if (forceStrikeIndicator == null)
        {
            var prefabIndicator = Instantiate(ForceFX[0], transform.position, Quaternion.identity,
                BuffFXLayer.gameObject.transform);
            forceStrikeIndicator = prefabIndicator.GetComponent<UI_ForceStrikeAimerTargeting>();
            prefabIndicator.name = "ForceStrikeIndicator";
            forceStrikeIndicator.SetActorController(ac as ActorControllerRangedWithFS);
            forceStrikeIndicator.SetMaxForceInfo(new List<float>(){0.3f});
            forceStrikeIndicator.SetTargetingType(true,true,12);
            forceStrikeIndicator.gameObject.SetActive(true);
        }
        else
        {
            forceStrikeIndicator.gameObject.SetActive(true);
        }
    }

    public override void ForceStrikeRelease(int forcelevel = 0)
    {
        if(forcelevel <= 0)
            return;

        _statusManager.RemoveSpecificTimerbuff
            ((int)BasicCalculation.BattleCondition.AlteredStrikeCleo, -1, true);

        //todo: 记录信息。
        Instantiate(ForceFX[1], forceStrikeIndicator.transform.position,
            Quaternion.identity, RangedAttackFXLayer.transform);

        (ac as ActorControllerRangedWithFS).PlayAttackVoice(9);

    }
}
