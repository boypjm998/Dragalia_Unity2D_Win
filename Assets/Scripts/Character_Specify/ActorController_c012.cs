using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class ActorController_c012 : ActorControllerMeeleWithFS
{
    public float currentSP;

    private SpecialSkillGauge_C012 _skillGauge;
    
    public enum SkillChainState
    {
        None,
        NormalChain,
        DispelChain,
        BreakChain
    }
    
    
    
    public SkillChainState _skill1EffectCurrent = SkillChainState.None;
    public SkillChainState _skill2EffectCurrent = SkillChainState.None;
    public SkillChainState _skill1EffectNext = SkillChainState.None;
    public SkillChainState _skill2EffectNext = SkillChainState.None;
    
    
    //public bool _skillChainIsActive { private set; get; } = false;
    

    protected override void Awake()
    {
        base.Awake();
        _skillGauge = SpecialSkillGauge_C012.Instance;
        _skillGauge.SetActor(this);
    }

    protected override void CheckSkill()
    {
        if (pi.isSkill)
        {
            Debug.Log("is Using Skill");
        }
        
        
        if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill &&
            currentSP >= SpecialSkillGauge_C012.MaxSPPerLevel)
        {
            _skillGauge.ConsumeOneLevel();
            UseSkill(1);
        }else if(currentSP < SpecialSkillGauge_C012.MaxSPPerLevel)
        {
            Debug.LogWarning("Not enough SP to use skill 1");
        }

        if (pi.skill[1] && (anim.GetBool("isGround") || canPerformInAir[1]) && !pi.hurt && !pi.isSkill
            && currentSP >= SpecialSkillGauge_C012.MaxSPPerLevel)
        {
            _skillGauge.ConsumeOneLevel();
            UseSkill(2);
        }else if(currentSP < SpecialSkillGauge_C012.MaxSPPerLevel)
        {
            Debug.LogWarning("Not enough SP to use skill 2");
        }

        // if (pi.skill[2] && (anim.GetBool("isGround") || canPerformInAir[2]) && !pi.hurt && !pi.isSkill)
        // {
        //     UseSkill(3);
        // }

        if (pi.skill[3] && (anim.GetBool("isGround") || canPerformInAir[3]) && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
    }
    
    private void Start()
    {
        _statusManager.OnAttackGainSP += UpdateSkillGauge;
    }

    private void UpdateSkillGauge(AttackBase attack, float sp)
    {
        if (attack.attackType == BasicCalculation.AttackType.SKILL || sp <= 0)
            return;

        _skillGauge.Charge((int)sp);
    }

    public void Skill1_BackStep()
    {
        var endPos = Mathf.Clamp(transform.position.x - facedir * 2,BattleStageManager.Instance.mapBorderL,
            BattleStageManager.Instance.mapBorderR);
        _tweener = rigid.DOMoveX(endPos, 0.2f).SetEase(Ease.OutSine);
    }
    
    public void Skill2_BackFlip()
    {
        SetGravityScale(0);
        
        var targetPos = new Vector2(transform.position.x - facedir*2,
            transform.position.y + 1.5f).SafePosition();

        var currentPlatform = gameObject.RaycastedPlatform();

        targetPos.x = 
            Mathf.Clamp(targetPos.x, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);
        
        var _tweener1 = 
            transform.DOMoveX(targetPos.x, 0.5f).SetEase(Ease.InSine).SetUpdate(UpdateType.Fixed);
        
        var _tweener2 = 
            transform.DOMoveY(targetPos.y,0.35f).SetEase(Ease.InOutCubic).SetUpdate(UpdateType.Fixed);

        ActorBase.OnHurt handler = null;
        handler = () =>
        {
            OnAttackInterrupt -= handler;
            _tweener2?.Kill();
            _tweener1?.Kill();
        };
        OnAttackInterrupt += handler;

    }
    
    
    public void Skill2_ToGround()
    {
        ResetGravityScale();
        
        var targetPos = new Vector2(transform.position.x - facedir*1,
            gameObject.RaycastedPosition().y + 1.2f).SafePosition();

        var currentPlatform = gameObject.RaycastedPlatform();

        targetPos.x = 
            Mathf.Clamp(targetPos.x, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);

        //var distance = transform.position.y - gameObject.RaycastedPosition().y - 1.2f;
        
        var _tweener1 = 
            transform.DOMoveY(targetPos.y,
                0.4f).SetEase(Ease.OutCubic);
        
        var _tweener2 = 
            transform.DOMoveX(targetPos.x, 0.4f).
                SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed);
        
        ActorBase.OnHurt handler = null;
        
        handler = () =>
        {
            OnAttackInterrupt -= handler;
            _tweener1?.Kill();
            _tweener2?.Kill();
        };
        
        OnAttackInterrupt += handler;

    }


    public override void OnSkillEnter()
    {
        base.OnSkillEnter();
        
    }

    public override void OnSkillExit()
    {
        ResetGravityScale();
        base.OnSkillExit();
        
        // if(currentSP < SpecialSkillGauge_C012.MaxSPPerLevel)
        //     return;
        //
        // if (anim.GetCurrentAnimatorStateInfo(0).IsName("s1")||
        //     anim.GetCurrentAnimatorStateInfo(0).IsName("s2"))
        // {
        //     _skill1EffectCurrent = _skill1EffectNext;
        //     _skill2EffectCurrent = _skill2EffectNext;
        //
        //     _skill1EffectNext = SkillChainState.None;
        //     _skill2EffectNext = SkillChainState.None;
        // }
        
        
    }

    
}
