using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C043 : AttackManagerMeeleWithFS
{
    private GameObject skill1Container;
    private GameObject skill2Container;

    private TimerBuff _forceStrikeBuff = new TimerBuff((int)BasicCalculation.BattleCondition.ForceStrikeDmgBuff,
        50, 15, 1, 104301);
    
    private TimerBuff _lastOffenseBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        50, 20, 1, 104302);
    private TimerBuff _lastDefenseBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
        50, 20, 1, 104302);

    private TimerBuff _lastOffenseBuff2 = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        30, 60, 1, 104303);
    private TimerBuff _lastDefenseBuff2 = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
        30, 60, 1, 104303);


    private ConditionalAttackEffect _poisonPunisher;

    private Tween _abilityTween = null;
    private float _abilityCD = 60;
    private bool _abilityReady = true;

    protected override void Awake()
    {
        base.Awake();
        
        var checkConditionString = ((int)BasicCalculation.BattleCondition.Poison).ToString();
        
        _poisonPunisher = new ConditionalAttackEffect
        (ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {"1", checkConditionString},
            new string[] {"0.75"});
        
        _lastDefenseBuff.dispellable = false;
        _lastOffenseBuff.dispellable = false;
        _lastDefenseBuff2.dispellable = false;
        _lastOffenseBuff2.dispellable = false;
    }

    protected override void Start()
    {
        base.Start();
        _statusManager.OnReviveOrDeath += RemoveAllLastBuff;
    }

    private void RemoveAllLastBuff()
    {
        _statusManager.RemoveAllConditionWithSpecialID(104302);
        //_statusManager.RemoveAllConditionWithSpecialID(104303);
    }
    private void Update()
    {
        if (_statusManager.currentHp <= _statusManager.maxHP * 0.5f && _abilityReady)
        {
            _abilityReady = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_lastDefenseBuff));
            _statusManager.ObtainTimerBuff(new TimerBuff(_lastOffenseBuff));
            _statusManager.ObtainTimerBuff(new TimerBuff(_lastDefenseBuff2),false);
            _statusManager.ObtainTimerBuff(new TimerBuff(_lastOffenseBuff2),false);
            _abilityTween = DOVirtual.DelayedCall(_abilityCD, () =>
            {
                _abilityReady = true;
            },false);
        }
    }


    private void OnSkillExit()
    {
        (ac as ActorController).SetWeaponVisibility(true);_statusManager.ResetKBRes();
        
    }

    public override void ForceStrike_Axe()
    {
        (ac as ActorControllerMeeleWithFS).PlayAttackVoice(9);
        var proj = InstantiateMeele(forceFX[0], transform.position, InitContainer(true));

        if (_statusManager.HasBuffWithSPID(104301))
        {
            var atk = proj.GetComponent<AttackFromPlayer>();
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Poison, 72, 30, 100),110);
        }

    }

    public void Skill1_ThrowWeapon()
    {
        (ac as ActorController).SetWeaponVisibility(false);

        skill1Container = InitContainer(false, 2,true);
        skill1Container.AddComponent<ObjectInvokeDestroy>().destroyTime = 2.5f;

        var proj = InstantiateRanged(skillFX[0], transform.position + new Vector3(ac.facedir, 0),
            skill1Container, ac.facedir);

        float tweenTime = 0.6f;

        proj.AddComponent<ObjectInvokeDestroy>().destroyTime = tweenTime;

        //将proj回旋飞到目标位置并返回
        proj.transform.DOMove(proj.transform.position + new Vector3(ac.facedir * 15, 0), tweenTime / 2)
            .SetUpdate(UpdateType.Fixed).SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                proj.transform.DOMove(proj.transform.position - new Vector3(ac.facedir * 15, 0), tweenTime / 2)
                    .SetEase(Ease.InOutSine).SetUpdate(UpdateType.Fixed).OnComplete(() =>
                    {
                        (ac as ActorController).SetWeaponVisibility(true);
                    }).OnKill(() =>
                    {
                        (ac as ActorController).SetWeaponVisibility(true);
                    });;
            });
        
        proj.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(_poisonPunisher);
        proj.GetComponent<AttackFromPlayer>().AddWithConditionAll
            (new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,5,15,1,104304),100);



    }

    public void Skill1_JumpSmash()
    {
        (ac as ActorController).SetWeaponVisibility(true);
        
        var target = (ac as ActorController).ta.GetNearestTargetInRangeDirection(ac.facedir, 12, 3,
            LayerMask.GetMask("Enemies"));

        float targetPosX = transform.position.x + ac.facedir * 6;


        if (target != null)
        {
            targetPosX = target.position.x - ac.facedir * 4;

            if ((targetPosX < transform.position.x && ac.facedir == 1)
                ||
                (targetPosX > transform.position.x && ac.facedir == -1)
               )
            {
                targetPosX = transform.position.x;
            }
            else
            {
                var currentPlatform = gameObject.RaycastedPlatform();
                targetPosX = Mathf.Clamp(targetPosX, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);
            }
        }
        else
        {
            var currentPlatform = gameObject.RaycastedPlatform();
            targetPosX = Mathf.Clamp(targetPosX, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);
        }

        var midPosX = (transform.position.x + targetPosX) / 2;

        _statusManager.knockbackRes = 999;

        ac.rigid.DOJump(new Vector2(targetPosX, transform.position.y), 4, 1, 0.4f).OnComplete(() =>
        {
            _statusManager.ResetKBRes();
            var proj = InstantiateRanged(skillFX[1], transform.position + new Vector3(ac.facedir * 1.5f, 0), skill1Container, 1);
            proj.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(_poisonPunisher);
        }).SetUpdate(UpdateType.Fixed);
        
        

    }


    public void Skill2_Smash1()
    {
        skill2Container = InitContainer(false, 3,true);
        skill2Container.AddComponent<ObjectInvokeDestroy>().destroyTime = 3f;
        
        var proj = InstantiateRanged(skillFX[2], transform.position + new Vector3(ac.facedir, 0),
            skill2Container, ac.facedir);
        
        proj.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(_poisonPunisher);
    }
    
    public void Skill2_Smash2()
    {
        var proj = InstantiateRanged(skillFX[3], transform.position + new Vector3(ac.facedir, 0),
            skill2Container, ac.facedir);
        
        proj.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(_poisonPunisher);
    }
    
    public void Skill2_Smash3()
    {
        var proj = InstantiateRanged(skillFX[4], transform.position ,
            skill2Container, ac.facedir);
        
        proj.GetComponent<AttackFromPlayer>().AddConditionalAttackEffect(_poisonPunisher);
    }

    public void Skill3_Buff()
    {
        InstantiateBuff(skillFX[5], transform.position);

        var buff = new TimerBuff(_forceStrikeBuff);
        buff.dispellable = false;

        _statusManager.ObtainTimerBuff(buff);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff, 30, 5);
    }
    
    
    
}