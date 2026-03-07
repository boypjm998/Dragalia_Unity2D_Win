using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class AttackManager_C054 : AttackManagerMeeleWithFS
{

    private GameObject _skillContainerInstance;
    
    private bool _counterStanceOn;
    private bool _isCounter = false;
    private Tween _counterTween;
    private GameObject _target;
    private GameObject _shieldFXInstance;
    private GameObject _buffFXInstance;
    
    private ConditionalAttackEffect _frostbitePunisher;
    private ConditionalAttackEffect _frostbitePunisher2;

    private TimerBuff _rangedResistBuff = new TimerBuff((int)BasicCalculation.BattleCondition.RangedAttackResistance,
        1, 3, 1, 105402);
    private TimerBuff _skillHasteBuff = new TimerBuff((int)BasicCalculation.BattleCondition.SkillHasteBuff,
        7, 30, 1, 105405);
    
    private bool _lastBraveryAvailable = true;
    private AbilityClock _frostbiteClock = new AbilityClock(5f);
    
    private TimerBuff _lastBraveryBuffAtk = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        40, -1, 1, 105404);
    private TimerBuff _lastBraveryBuffDef = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
        30, -1, 1, 105404);

    protected override void Awake()
    {
        base.Awake();
        //var checkConditionString = ((int)BasicCalculation.BattleCondition.Frostbite).ToString();
        _rangedResistBuff.extra_iconID = (int)BasicCalculation.BattleCondition.DamageCut;
        _rangedResistBuff.dispellable = false;

        _frostbitePunisher = new ConditionalAttackEffect(ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            0.5f, BasicCalculation.BattleCondition.Frostbite);
        _frostbitePunisher2 = new ConditionalAttackEffect(ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            0.3f, BasicCalculation.BattleCondition.Frostbite);
        
        _lastBraveryBuffAtk.dispellable = false;
        _lastBraveryBuffDef.dispellable = false;

    }

    protected override void Start()
    {
        base.Start();
        _statusManager.OnTakeDirectDamageFrom += Skill2_CounterTrigger;
        _statusManager.AddEffectFunction(SpeicalRangedAttackRes, AbilityCalculation.ProductArea.DMGCUT);
        //_statusManager.SpecialDamageCutEffectFunc += SpeicalRangedAttackRes;
        _statusManager.OnBuffEventDelegate += CheckBuff;
        _statusManager.OnBuffDispelledEventDelegate += CheckBuff;
        _statusManager.OnBuffExpiredEventDelegate += CheckBuff;
        _statusManager.OnAfflictionInflict += FrostbiteInflictBuff;
        _statusManager.OnReviveOrDeath += RemoveLastBravery;
    }

    private void Update()
    {
        if (!_lastBraveryAvailable)
            return;
        
        if (_statusManager.currentHp < _statusManager.maxHP * 0.5f)
        {
            _lastBraveryAvailable = false;
            _statusManager.ObtainTimerBuff(new TimerBuff(_lastBraveryBuffAtk),false);
            _statusManager.ObtainTimerBuff(new TimerBuff(_lastBraveryBuffDef),true);
        }
    }


    public void Skill1_ForwardDash()
    {

        var atk = InstantiateRanged
        (skillFX[0], transform.position + new Vector3(ac.facedir * 4, 0),
            _skillContainerInstance = InitContainer(false,2,true),ac.facedir).
            GetComponent<AttackFromPlayer>();

        var freeze = new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
            1, Random.Range(4f, 6f), 1);
        
        atk.AddWithCondition(0,freeze,110,0);
        atk.AddConditionalAttackEffect(_frostbitePunisher);
        atk.gameObject.AddComponent<RelativePositionRetainer>().SetParent(transform);
        
        if (_statusManager.HasBuffWithSPID(105405))
        {
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.FrostbiteResDown,
                20, 30, 1, 105405),100,2);
        }


    }

    public void Skill1_Jump()
    {
        ac.SetGravityScale(0.01f);
        _statusManager.knockbackRes = 999;
        var pos = transform.position.y;
        var tweenerY = 
            transform.DOMoveY(pos + 4f, 0.3f).SetEase(Ease.OutCirc)
            .OnComplete(() => transform.DOMoveY(pos, 0.2f).SetEase(Ease.InCirc));
        
        
        var posX = (ac as ActorControllerMeeleWithFS).
            GetHorizontalMovementPositionX(5,2,2,0.4f);

        ac.SetTweener(transform.DOMoveX(posX, 0.4f).SetEase(Ease.OutSine).SetUpdate(UpdateType.Fixed));
        

    }

    public void Skill1_Smashdown()
    {
        var atk = InstantiateRanged
            (skillFX[1], transform.position + new Vector3(ac.facedir * 2f, 0),
                _skillContainerInstance, ac.facedir).
            GetComponent<AttackFromPlayer>();
        
        var frostbite = new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
            41, 21f, 100);
        
        atk.AddWithCondition(0,frostbite,120,1);
        atk.AddConditionalAttackEffect(_frostbitePunisher);
        ac.ResetGravityScale();
        
    }

    public void Skill2_CounterStart()
    {
        _target = null;
        _counterStanceOn = true;
        _isCounter = false;
        _statusManager.knockbackRes = 999;
        (ac as ActorControllerMeeleWithFS).dodging = false;
        _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.CounterReady.ToString());
        _statusManager.ObtainTimerBuff(new TimerBuff(_rangedResistBuff));
        _statusManager.HPRegenImmediately(0, 10, false);
        
        if(_shieldFXInstance == null)
            _shieldFXInstance = InstantiateBuff(skillFX[4], transform.position);
        else
            _shieldFXInstance.SetActive(true);
        
    }

    private void Skill2_CounterTrigger(StatusManager self, StatusManager target, AttackBase enmAtk, float dmg)
    {
        if(!_counterStanceOn)
            return;
        
        _counterStanceOn = false;
        _isCounter = true;
        (ac as ActorControllerMeeleWithFS).dodging = true;
        BattleStageManager.Instance.TimeScaleEffect(0.1f,0.5f);
        InstantiateBuff(skillFX[2], transform.position);
        _target = target.gameObject;
        

        _counterTween = DOVirtual.DelayedCall(0.03f, Skill2_CounterActive,false);
        
    }

    private void Skill2_CounterActive()
    {
        ac.anim.Play("s2_boost");
        _statusManager.knockbackRes = 999;
        _statusManager.ObtainTimerBuff(1, 30, 20, 1, 105401,false);
    }

    public void Skill2_NoCounter()
    {
        _counterStanceOn = false;
        if(_isCounter)
            return;
        
        ac.anim.Play("s2_boost");
        _statusManager.knockbackRes = 999;
    }

    public void Skill2_JumpAimingTarget()
    {
        if(_shieldFXInstance != null)
            _shieldFXInstance.SetActive(false);

        if (_statusManager.HasBuffWithSPID(105405))
        {
            _statusManager.ImmuneToAllControlAffliction = true;
        }
        
        
        _statusManager.knockbackRes = 999;
        if (_target == null)
        {
            var aimTransform = (ac as ActorController).ta.GetNearestTargetInRangeDirection(ac.facedir, 20, 5,
                LayerMask.GetMask("Enemies"));
            
            if(aimTransform != null)
                _target = aimTransform.gameObject;
        }

        Vector2 targetPos = 
            new Vector2(BattleStageManager.Instance.OutOfPlatformBoundsCheck(gameObject,
                transform.position.x + 8 * ac.facedir), transform.position.y + 4).SafePosition();



        try
        {
            if (_target != null)
            {
                ac.TurnMove(_target);
                
                print("Target:"+_target.name);
                
                var targetCollider = _target.GetComponent<Collider2D>();
                
                targetPos = new Vector2(_target.transform.position.x - ac.facedir * 1.5f,
                    _target.RaycastedPosition().y + 5).SafePosition();
                
                // targetPos = new Vector2(_target.transform.position.x - ac.facedir * 1.5f,
                //     targetCollider.bounds.max.y + 2.5f).SafePosition();


                if (Mathf.Abs(_target.RaycastedPosition().y - transform.position.y) > 15)
                {
                    targetPos = new Vector2(_target.RaycastedPosition().x - 1.5f, transform.position.y).SafePosition();
                    _target = null;
                }

                if (Mathf.Abs(_target.transform.position.x - transform.position.x) > 30)
                {
                    targetPos = new Vector2(transform.position.x + 30 * ac.facedir, transform.position.y)
                        .SafePosition();
                    _target = null;
                }
            }
        }
        catch
        {
            targetPos = 
                new Vector2(BattleStageManager.Instance.OutOfPlatformBoundsCheck(gameObject,
                    transform.position.x + 8 * ac.facedir), transform.position.y + 4).SafePosition();
        }
        
        (ac as ActorController).SetGroundCollision(false);
        
        ac.SetTweener(ac.rigid.DOMove(targetPos, 0.35f).SetEase(Ease.OutSine).
            SetUpdate(UpdateType.Fixed).
            OnComplete(()=>
            {
                (ac as ActorController).SetGroundCollision(true);
                ac.SetGravityScale(0);
            }).OnKill(()=>(ac as ActorController).SetGroundCollision(true)));
        
        
        
    }
    
    public void Skill2_Smashdown()
    {
        ac.ResetGravityScale();
        ac.SetTweener(ac.rigid.DOMoveY(transform.position.y - 3.5f, 
            0.3f).SetEase(Ease.InOutCirc).SetUpdate(UpdateType.Fixed).
            OnComplete(()=>
            {
                _statusManager.ImmuneToAllControlAffliction = false;
                
                var atk = InstantiateRanged
                    (skillFX[3], gameObject.RaycastedPosition(),
                        InitContainer(false,1,true), 1).
                    GetComponent<ForcedAttackFromPlayer>();
                
                if(_target != null)
                    atk.target = _target;

                AttackBase.AttackBaseDelegate handler = null;

                handler = (atk, tar) =>
                {
                    atk.BeforeAttackHit -= handler;
                    DOVirtual.DelayedCall(0.1f, () =>
                    {
                        if(GlobalController.currentGameState == GlobalController.GameState.Inbattle)
                            BattleStageManager.Instance.TimeScaleEffect(0.1f,0.2f);
                    }, false);
                    
                };
                atk.AddConditionalAttackEffect(_frostbitePunisher2);
                atk.BeforeAttackHit += handler;
                
                int maxCap = (int)(_statusManager.maxHP * 0.2f);
                int currentHealed = 0;
                atk.OnAttackDealDamage += (statusManagerSelf, statusManagerTarget, attack, dmg) =>
                {
                    if(currentHealed >= maxCap) return;
                    var clamp = Mathf.Abs(maxCap - currentHealed);
                    currentHealed += LifeStealWithReturn(statusManagerSelf,
                        (int)dmg,8,20,clamp);
                };

                if (_isCounter)
                {
                    var debuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff, 
                        10, 30, 1,105403);
                    atk.AddWithConditionAll(debuff,100);
                    atk.extraODModifier += 0.5f;
                }
                
                
                
                _statusManager.ResetKBRes();
            }));
        
    }
    
    public void Skill3_Buff()
    {
        _statusManager.ObtainTimerBuff(new TimerBuff(_skillHasteBuff));
        
        InstantiateBuff(skillFX[5], transform.position);

    }

    private void RemoveLastBravery()
    {
        _statusManager.RemoveAllConditionWithSpecialID(105404);
        _lastBraveryAvailable = true;
    }
    
    
    
    protected void OnSkillEnter()
    {
        if(ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s2_boost"))
            _statusManager.knockbackRes = 999;

        if (_statusManager.HasBuffWithSPID(105405))
        {
            if (ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s2") ||
                ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s2_boost"))
            {
                _statusManager.ImmuneToAllControlAffliction = true;
            }
            
            if(ac.anim.GetCurrentAnimatorStateInfo(0).IsName("s2_boost"))
                return;
            
            
            (_statusManager as PlayerStatusManager).FillSP(0,5);
            (_statusManager as PlayerStatusManager).FillSP(1,5);
            (_statusManager as PlayerStatusManager).FillSP(2,5);
            (_statusManager as PlayerStatusManager).FillSP(3,5);
            _statusManager.OnSpecialBuffDelegate?.
                Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        }
        
             
        
    }

    protected void OnSkillExit()
    {
        _statusManager.ResetKBRes();
        ac.ResetGravityScale();
        
        
        _statusManager.ImmuneToAllControlAffliction = false;
        
        
    }

    public static (float, float) SpeicalRangedAttackRes(StatusManager self, AttackBase atk, StatusManager target)
    {
        if (target.HasCondition((int)BasicCalculation.BattleCondition.RangedAttackResistance))
        {
            if(atk is ForcedAttackFromEnemy)
                return (0.3f, 0);

            if (atk is CustomMeeleFromEnemy)
                return (0.3f, 0);

            if (atk is CustomRangedFromEnemy)
            {
                if(Vector2.Distance(self.transform.position, target.transform.position) < 12)
                    return (0.3f, 0);
            }
            
            print("Ranged Attack Resisted");
            return (0.7f, 0);
        }
        else
        {
            return (0, 0);
        }
    }

    private void CheckBuff(BattleCondition condition)
    {
        if (_statusManager.HasBuffWithSPID(105405))
        {
            if(_buffFXInstance == null)
                _buffFXInstance = InstantiateBuff(skillFX[6], transform.position);
            else
            {
                _buffFXInstance.SetActive(true);
            }
        }else
        {
            if(_buffFXInstance != null)
                _buffFXInstance.SetActive(false);
        }
    }

    private void FrostbiteInflictBuff(BattleCondition condition)
    {
        
       if(!_frostbiteClock.Available)
           return;
       
       print("Triggered");
       
       if (condition.buffID == (int)BasicCalculation.BattleCondition.Frostbite)
       {
           _frostbiteClock.StartTick();
           _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff, 13, 10);
           _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 13, 10);
       }
        
    }
    
    
}
