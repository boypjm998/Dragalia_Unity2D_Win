using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C036 : AttackManagerMeeleWithFS
{
    private GameObject skill2Container;
    private int buffCountWhenUsingSkill2;

    private int type = 1;

    private TimerBuff doubleBuff = 
        new((int)BasicCalculation.BattleCondition.SkillDmgBuff, 6, 15, 5, 103601);

    protected override void Start()
    {
        base.Start();
        _statusManager.OnBuffEventDelegate += CheckDoublebuff;
    }

    private void CheckDoublebuff(BattleCondition condition)
    {
        if (condition.buffID == (int)(BasicCalculation.BattleCondition.DefBuff) &&
            condition.buffID != 103601)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff(doubleBuff));
        }
    }
    
    

    public void Skill1_Throw()
    {
        (ac as ActorController).SetWeaponVisibility(false);
        
        
        var ball = Instantiate(skillFX[0], transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);

        ActorBase.OnHurt onAttackInterruptEvent = null;
        
        var target =
            (ac as ActorController).ta.GetNearestTargetInRangeDirection(ac.facedir, 17, 7.5f,
                LayerMask.GetMask("Enemies"));

        Vector2 castlePosition;
        float safePosition = Mathf.Clamp(transform.position.x + ac.facedir * 12, 
            BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);

        castlePosition = new Vector3(safePosition,
            BasicCalculation.GetRaycastedPlatformY(
            new Vector2(safePosition, transform.position.y)));
        

        var sandCastle = Instantiate(skillFX[3], target != null
            ? target.gameObject.RaycastedPosition()
            : castlePosition,
            Quaternion.identity, RangedAttackFXLayer.transform);
        
        var tween = DOVirtual.DelayedCall((53f / 60f),
            () =>
            {
                ac.OnAttackInterrupt -= onAttackInterruptEvent;
                

                var proj = 
                Instantiate(skillFX[1], ball.transform.GetChild(0).position, Quaternion.identity,
                    RangedAttackFXLayer.transform);
                
                Destroy(ball);

                var controller = proj.GetComponent<Projectile_C036_1>();
                
                controller.SetSource(transform,ac.facedir);
                
                if(target != null)
                    controller.SetTarget(target.gameObject);
                
                controller.SetGoal();
                if(sandCastle != null)
                    controller.SetSandCastle(sandCastle);

            },false);

        

        onAttackInterruptEvent = () =>
        {
            ac.OnAttackInterrupt -= onAttackInterruptEvent;
            tween?.Kill();
            Destroy(ball);
        };

        ac.OnAttackInterrupt += onAttackInterruptEvent;





    }

    public void Skill2_Slash()
    {
        var container = InitContainer(true,2,true);
        skill2Container = container;
        
        var slashFX = InstantiateMeele(skillFX[4], transform.position,
            container);

        var slashAttack = slashFX.GetComponent<AttackFromPlayer>();

        buffCountWhenUsingSkill2 = _statusManager.GetBuffStackNum();
        var dmgUp = Mathf.Clamp(buffCountWhenUsingSkill2 * 0.05f, 0, 0.8f);
        
        
        var caf = new ConditionalAttackEffect((s1, s2) => { return true; },
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier, new string[] {},
            new string[] { (dmgUp).ToString() });
        slashAttack.AddConditionalAttackEffect(caf);
        _statusManager.OnSpecialBuffDelegate?.Invoke($"BuffCount_{buffCountWhenUsingSkill2}");

        ActorBase.OnHurt eventHandler = null;
        
        ac.OnAttackInterrupt -= eventHandler;

        eventHandler = () =>
        {
            ac.OnAttackInterrupt -= eventHandler;
            if(slashFX == null)
                return;
            Destroy(slashFX);
        };

        ac.OnAttackInterrupt += eventHandler;

    }

    public void Skill2_SummonAlex()
    {
        
        
        var alexModel = Instantiate(skillFX[2], 
            gameObject.RaycastedPosition() + new Vector2(0, 3), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var controller = alexModel.GetComponentInChildren<Projectile_C036_2>();
        
        controller.SetSource(transform,buffCountWhenUsingSkill2);
        controller.SetContainer(skill2Container);
        var distance = controller.GetDistance(4);
        print(distance);

        var target = (ac as ActorController).ta.GetNearestTargetInRangeDirection(ac.facedir, 20, 5,
            LayerMask.GetMask("Enemies"));

        //var posX = transform.position.x - 4;
        var facedir = ac.facedir;
        
        if (target != null)
        {
            // 如果有目标敌人
            float targetPosX = target.position.x;
            float archerPosX;

            if (facedir == -1)
            {
                // 如果玩家面朝左边
                archerPosX = targetPosX + distance;
                alexModel.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                // 如果玩家面朝右边
                archerPosX = targetPosX - distance;
                alexModel.transform.localScale = new Vector3(1, 1, 1);
            }

            print(archerPosX);
            print(BattleStageManager.Instance.mapBorderR);
            print(BattleStageManager.Instance.mapBorderL);
            // 检查弓箭手的位置是否超出了地图边界
            if (archerPosX < BattleStageManager.Instance.mapBorderL)
            {
                archerPosX = targetPosX + distance;
                alexModel.transform.localScale = new Vector3(-1, 1, 1);
            }
            else if (archerPosX > BattleStageManager.Instance.mapBorderR)
            {
                archerPosX = targetPosX - distance;
                alexModel.transform.localScale = new Vector3(1, 1, 1);
            }

            alexModel.transform.position = new Vector3(archerPosX, gameObject.RaycastedPosition().y + 4, alexModel.transform.position.z);
        }
        else
        {
            // 如果没有目标敌人
            float archerPosX = ac.transform.position.x - ac.facedir * distance / 2;

            // 检查弓箭手的位置是否超出了地图边界
            if (archerPosX < BattleStageManager.Instance.mapBorderL)
            {
                archerPosX = BattleStageManager.Instance.mapBorderL;
            }
            else if (archerPosX > BattleStageManager.Instance.mapBorderR)
            {
                archerPosX = BattleStageManager.Instance.mapBorderR;
            }

            alexModel.transform.localScale = new Vector3(facedir, 1, 1);
            alexModel.transform.position = new Vector3(archerPosX, gameObject.RaycastedPosition().y + 4, alexModel.transform.position.z);
        }

    }

    public void Skill3_Buff()
    {
        Instantiate(skillFX[5], gameObject.RaycastedPosition(), Quaternion.identity,
            RangedAttackFXLayer.transform);

        // _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
        //     20, 15);
        _statusManager.EnergyLevelUp(3);
        
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            25, 20);
        if (type == 1)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.ScorchrendRateUp,
                50, 15);
            type = 2;
        }
        else
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.ShadowblightRateUp,
                50, 15);
            type = 1;
        }
        
        

    }

    private void OnSkillEnter()
    {
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            10, 10);
    }

    private void OnSkillExit()
    {
        (ac as ActorController).SetWeaponVisibility(true);
    }
}
