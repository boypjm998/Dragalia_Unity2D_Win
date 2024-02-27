using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C032 : AttackManagerMeeleWithFS
{
    GameObject skill1Container;
    private int skillUpgradeLevel = 0;
    
    protected override void Start()
    {
        base.Start();
        
        (ac as ActorController).SetSkillAirPerformProperty(3,true);
        
    }

    public void Skill1_AroundAttack()
    {
        skill1Container = InitContainer(false,2,true);
        var atkGO = InstantiateRanged(skillFX[0],transform.position,skill1Container,ac.facedir);
        var paralysis = new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,
            72.7f, 15f, 100);
        var atk = atkGO.GetComponent<AttackFromPlayer>();
        atk.AddWithCondition(0, paralysis,110,0);

        int level = GetSkillUpgradeLevel(103201);

        skillUpgradeLevel = level;

        if (level >= 1)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
                25, 15);
            
            atk.AddConditionalAttackEffect
                (new ConditionalAttackEffect
                (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                    ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                    new string[] {"1", "404"},
                    new string[] {"0.3"}));
        }

        if (level >= 2)
        {
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,
                20, 20);
        }



    }

    public void Skill1_BackFlip()
    {
        var actor = (ac as ActorController);
        ac.SetGravityScale(0);
        
        var targetPos = new Vector2(transform.position.x - ac.facedir*6,
            transform.position.y + 4).SafePosition();

        var currentPlatform = gameObject.RaycastedPlatform();

        targetPos.x = 
            Mathf.Clamp(targetPos.x, currentPlatform.bounds.min.x, currentPlatform.bounds.max.x);
        
        var _tweener1 = 
            ac.transform.DOMoveX(targetPos.x, 0.7f).SetEase(Ease.InOutSine);
        
        var _tweener2 = 
            ac.transform.DOMoveY(targetPos.y,0.5f).SetEase(Ease.InOutCubic);

        ActorBase.OnHurt handler = null;
        handler = () =>
        {
            ac.OnAttackInterrupt -= handler;
            _tweener2?.Kill();
            _tweener1?.Kill();
        };
        ac.OnAttackInterrupt += handler;

    }

    public void Skill1_ThrowWeapon()
    {
        (ac as ActorController).SetWeaponVisibility(false);
        
        int level = skillUpgradeLevel;
        GameObject prefab;

        if (level == 0)
        {
            prefab = skillFX[1];
        }else if (level == 1)
        {
            prefab = skillFX[4];
        }
        else
        {
            prefab = skillFX[5];
        }


        var atkGo = InstantiateRanged(prefab,
            transform.position+new Vector3(ac.facedir*9,-4),
            skill1Container,ac.facedir);

        AttackFromPlayer atk = atkGo.GetComponent<AttackFromPlayer>();

        
        
        if (level >= 1)
        {
            atk.AddConditionalAttackEffect
            (new ConditionalAttackEffect
            (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", "404"},
                new string[] {"0.5"}));
        }
        
        DOVirtual.DelayedCall(1.3f, () =>
        {
            if (skillUpgradeLevel >= 2)
            {
                _statusManager.HPRegenImmediately(100,0,true);
            }
            SkillUpgrade(103201);
        }, false);

    }

    public void Skill1_ToGround()
    {

        var distance = transform.position.y - gameObject.RaycastedPosition().y - 1.2f;
        
        var _tweener = 
            ac.rigid.DOMoveY(gameObject.RaycastedPosition().y + 1.2f,
                0.3f).SetEase(Ease.OutCubic);
        
        ActorBase.OnHurt handler = null;
        
        handler = () =>
        {
            ac.OnAttackInterrupt -= handler;
            _tweener?.Kill();
        };
        
        ac.OnAttackInterrupt += handler;

    }
    

    public void Skill1_RespawnWeapon()
    {
        (ac as ActorController).SetWeaponVisibility(true);
        ac.ResetGravityScale();

        
        
        
    }

    public void Skill2_Buff()
    {
        var vampireMaiden = new TimerBuff((int)BasicCalculation.BattleCondition.VampireMaiden,
            1, 15, 1);

        var buff = InstantiateBuff(skillFX[2], gameObject.RaycastedPosition());

        
        _statusManager.ObtainTimerBuff(vampireMaiden);

    }

    private void OnSkillExit()
    {
        if (_statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.VampireMaiden) > 0)
        {
            _statusManager.EnergyLevelUp(1);
        }

        _statusManager.knockbackRes = 0;
    }

    public void Skill2_Stab()
    {
        var atkGO = InstantiateMeele(skillFX[3], transform.position,
            InitContainer(true,1,true));

        var atk = atkGO.GetComponent<AttackFromPlayer>();

        AttackBase.AttackBaseDelegate eventhandler = null;

        eventhandler += (@base, target) =>
        {
            (_statusManager as PlayerStatusManager).ChargeSPAll(700);
            atk.OnAttackHit -= eventhandler;
        };

        atk.OnAttackHit += eventhandler;
        
        atk.AddConditionalAttackEffect
        (new ConditionalAttackEffect
        (ConditionalAttackEffect.ConditionType.TargetHasCondition,
            ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
            new string[] {"1", "404"},
            new string[] {"0.5"}));

    }

    public void Skill3()
    {
        _statusManager.knockbackRes = 999;
        
        Instantiate(skillFX[6], transform.position, Quaternion.identity, 
            BattleStageManager.Instance.RangedAttackFXLayer.transform);

        var targetTransform = (ac as ActorController).ta.GetNearestTargetInRangeDirection(ac.facedir,
            15f, 3f,
            LayerMask.GetMask("Enemies"));
        Vector2 targetPos;
        
        //GetVelocity
        ac.DisappearRenderer();
        //Invoke("AppearRenderer",0.1f);
        float targetY;

        if (targetTransform != null)
        {
            targetPos = new Vector3(targetTransform.position.x,
                            targetTransform.gameObject.RaycastedPosition().y) +
                        new Vector3(-ac.facedir, 6f);
            targetPos = BattleStageManager.Instance.OutOfRangeCheck(targetPos);
            targetY = targetTransform.gameObject.RaycastedPosition().y + 1.3f;
        }
        else
        {
            targetPos = transform.position + new Vector3(ac.facedir * 6f,
                6+gameObject.RaycastedPosition().y);
            targetPos = BattleStageManager.Instance.OutOfRangeCheck(targetPos);
            targetY = gameObject.RaycastedPosition().y + 1.3f;
        }
        
        transform.position = targetPos;
        ac.SetGravityScale(0);
        (ac as ActorController).SetGroundCollision(false);
        ac.rigid.velocity = Vector2.zero;

        DOVirtual.DelayedCall(0.3f, () =>
        {
            

            Instantiate(skillFX[6], transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            Instantiate(skillFX[7], transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);

            var tweener = ac.rigid.DOMoveY(targetY, 0.15f).OnComplete(() =>
            {
                _statusManager.knockbackRes = 0;
                ac.ResetGravityScale();
                (ac as ActorController).SetGroundCollision(true);
                
                var atk = InstantiateRanged(skillFX[8],gameObject.RaycastedPosition(),
                    InitContainer(false,1,true),1).GetComponent<AttackFromPlayer>();

                AttackBase.AttackBaseDelegate eventHandler = null;

                eventHandler = (@base, target) =>
                {
                    _statusManager.HPRegenImmediatelyWithoutRandom(0,20);
                    atk.OnAttackHit -= eventHandler;
                };

                atk.OnAttackHit += eventHandler;

            }).OnKill(() =>
            {
                _statusManager.knockbackRes = 0;
                ac.ResetGravityScale();
                (ac as ActorController).SetGroundCollision(true);
            }).SetEase(Ease.OutCubic);
            

        }, false);
        


    }

    public void Skill3_Appear()
    {
        ac.AppearRenderer();

        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.ParalysisRateUp,
            50, 20,1,103202);

    }

    public void Skill4_Super()
    {
        InstantiateBuff(skillFX[9], transform.position);
        float potency = 110;
        
        _statusManager.ReliefOneDebuff();
        
        

        if ((float)_statusManager.currentHp < (float)_statusManager.maxHP * 0.5f)
        {
            (_statusManager as PlayerStatusManager).FillSP(3,35);
            _statusManager.OnSpecialBuffDelegate?.Invoke("SPCharge");
        }
        else
        {
            if (_statusManager.ImmuneToAllDotAffliction == false)
            {
                _statusManager.ImmuneToAllDotAffliction = true;
                _statusManager.OnSpecialBuffDelegate?.Invoke("ImmuneToDoTAffliction");
            
                StatusManager.TestDelegate handler = null;

                handler = (condition) =>
                {
                    if (StatusManager.IsDotAffliction(condition.buffID))
                    {
                        _statusManager.ImmuneToAllDotAffliction = false;
                        _statusManager.OnAfflictionGuarded -= handler;
                    }
                };

                _statusManager.OnAfflictionGuarded += handler;
            }
        }

        _statusManager.HPRegenImmediately(potency,0,true);
        _statusManager.ObtainTimerBuff(1, 15, 60, 1, 103202);

    }








}
