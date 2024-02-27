using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using DG.Tweening.Core;
using GameMechanics;
using UnityEngine;

public class AttackManager_C033 : AttackManagerMeeleWithFS
{

    private TimerBuff preserveBuff;

    private TimerBuff frostbiteAffliction;
    private TimerBuff burnAffliction;
    private TimerBuff paralysisAffliction;
    private TimerBuff poisonAffliction;
    private TimerBuff scorchrendAffliction;
    private TimerBuff flashburnAffliction;
    private TimerBuff shadowblightAffliction;
    private TimerBuff stormlashAffliction;
    
    private List<TimerBuff> afflictions = new();

    protected override void Start()
    {
        base.Start();

        preserveBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            20, 30, 3, 103303);
        
        InitAfflictions();

    }

    private void InitAfflictions()
    {
        frostbiteAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
            31, 21, 100);
        shadowblightAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.ShadowBlight,
            31, 21, 100);
        flashburnAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
            31, 21, 100);
        scorchrendAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Scorchrend,
            31, 21, 100);
        stormlashAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Stormlash,
            31, 21, 100);
        
        burnAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Burn,
            72.7f, 12, 100);
        paralysisAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Paralysis,
            72.7f, 12, 100);
        
        poisonAffliction = new TimerBuff((int)BasicCalculation.BattleCondition.Poison,
            43.6f, 15, 100);
        
        afflictions = (new TimerBuff[]
        {
            burnAffliction,
            paralysisAffliction,
            poisonAffliction,
            scorchrendAffliction,
            flashburnAffliction,
            shadowblightAffliction,
            stormlashAffliction
        }).ToList();

        (ac as ActorController_c033).OnSigilReleaseDelegate += UpdateAfflictionDuration;



    }


    private void UpdateAfflictionDuration()
    {
        (ac as ActorController_c033).OnSigilReleaseDelegate -= UpdateAfflictionDuration;

        foreach (var affliction in afflictions)
        {
            affliction.SetDuration(affliction.duration + 10);
        }
    }

    protected override void OnForcingUpdate()
    {
        base.OnForcingUpdate();
        
        if((ac as ActorController_c033).sigilReleased)
            return;
        
        var ac_033 = ac as ActorController_c033;
        
        ac_033.modeTime += Time.deltaTime;

        if (ac_033.modeTime > 1.5f)
        {
            ac_033.modeTime = 0f;
            ac_033.mode = (ac_033.mode + 1) % 3;
            Instantiate(forceFX[ac_033.mode+1],
                gameObject.RaycastedPosition(),Quaternion.identity,
                RangedAttackFXLayer.transform);
            ac_033.SwitchMode(ac_033.mode);
        }


    }

    private void ReduceSigilTime(float time)
    {
        var sigilLockedBuff = _statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.LockedSigil)[0];

        if (sigilLockedBuff != null)
        {
            _statusManager.OnSpecialBuffDelegate?.
                Invoke(UI_BuffLogPopManager.SpecialConditionType.ReduceSigilTime.ToString());
            
            if(sigilLockedBuff.duration < 0)
                return;
            
            sigilLockedBuff.lastTime -= time;
            
            
        }
    }

    public void Skill_ScreenFX(bool s1 = true)
    {
        var fx = InstantiateMeele(skillFX[0], transform.position, InitContainer(true));

        ActorBase.OnHurt delegateOnHurt = null;

        delegateOnHurt = () =>
        {
            Destroy(fx.gameObject);
            ac.OnAttackInterrupt -= delegateOnHurt;
        };
        
        ac.OnAttackInterrupt += delegateOnHurt;

        if(s1 && !(ac as ActorController_c033).sigilReleased)
            ReduceSigilTime(6);
    }


    public void Skill1_Attack()
    {
        if ((ac as ActorController_c033).sigilReleased)
        {
            InstantiateMeele(skillFX[4], transform.position,
                InitContainer(true,1,true));

            _statusManager.ObtainTimerBuff(new TimerBuff(preserveBuff));
            _statusManager.HPRegenImmediately(50,0);
            _statusManager.ReliefOneAffliction();


            return;
        }
        
        
        switch ((ac as ActorController_c033).mode)
        {
            case 0:
            {
                _statusManager.ObtainTimerBuff(new TimerBuff(preserveBuff));
                InstantiateMeele(skillFX[1], transform.position,
                    InitContainer(true,1,true));
                
                break;
            }
            case 1:
            {
                _statusManager.HPRegenImmediately(50,0);
                InstantiateMeele(skillFX[2], transform.position,
                    InitContainer(true,1,true));
                break;
            }
            case 2:
            {
                
                InstantiateMeele(skillFX[3], transform.position,
                    InitContainer(true,1,true));
                if(_statusManager.ReliefOneAffliction());
                {
                    ReduceSigilTime(12);
                }
                
                break;
            }
        }
        
        
    }


    public void Skill2_ScanEffect()
    {
        InstantiateBuff(skillFX[7], ac.gameObject.RaycastedPosition());
    }

    public void Skill2_ThrowProjectile()
    {
        var fx = Instantiate(skillFX[5], 
            transform.position + new Vector3(ac.facedir , 1), Quaternion.identity,
            RangedAttackFXLayer.transform);

        var tweenerCore = fx.transform.DOMove
            (transform.position + new Vector3(0,6.5f,0), 0.4f);

        var generatePos = transform.position + new Vector3(0, 2.5f);

        tweenerCore.OnComplete(() =>
        {
            var proj = InstantiateRanged(skillFX[6],
                generatePos,InitContainer(false,1,true),1);
            
            //生成1-2的随机数,决定旋转角度
            int random = Random.Range(1, 3);
            proj.transform.GetChild(0).localRotation = Quaternion.Euler(0,random * 30,0);
            
            var atk = proj.GetComponent<AttackFromPlayer>();
            
            //从afflictions里随机选2个不重复的值
            var afflictionsCopy = new List<TimerBuff>(afflictions);
            
            var affliction1 = afflictionsCopy[Random.Range(0, afflictionsCopy.Count)];
            afflictionsCopy.Remove(affliction1);
            var affliction2 = afflictionsCopy[Random.Range(0, afflictionsCopy.Count)];

            var chance = 110;
            
            
            atk.AddWithConditionAll(new TimerBuff(frostbiteAffliction),chance,1);
            atk.AddWithConditionAll(new TimerBuff(affliction1),chance,2);
            atk.AddWithConditionAll(new TimerBuff(affliction2),chance,3);
            
            
        });

    }

    public void Skill3_AimProjectile()
    {
        var targetTransform = (ac as ActorController).ta.GetNearestTargetInRangeDirection(ac.facedir,
            20, 8, LayerMask.GetMask("Enemies"));
        
        var fx = Instantiate(skillFX[5], 
            transform.position + new Vector3(ac.facedir * 2 , 0), Quaternion.identity,
            RangedAttackFXLayer.transform);

        Tweener tweener;
        
        if (targetTransform == null)
        {
            tweener = fx.transform.DOMove
                (transform.position + new Vector3(ac.facedir * 20, 0), 0.5f);
            return;
        }
        else
        {
            tweener = fx.transform.DOMove
                (targetTransform.position, 0.5f).OnComplete(() =>
            {
                Destroy(fx,0.1f);
                (ac as ActorController_c033).SetTrap(true);
                Vector3 scale = 
                    BattleEffectManager.Instance.GetScaleFactor(targetTransform.parent.gameObject);
                var container = InstantiateSealedContainer(skillFX[8],
                    false, true);
                container.transform.position = targetTransform.position;

                
                var controller = container.GetComponent<Projectile_C033_1>();
                controller.SetTarget(targetTransform.gameObject);
                controller.SetSource(ac as ActorController_c033);
                controller.SetEffectFactor(scale);


            });
        }



    }

    public void Skill4_Buff()
    {
        var ac033 = ac as ActorController_c033;

        if (ac033.sigilReleased)
        {
            _statusManager.HPRegenImmediately(0,20,true);
            _statusManager.ObtainHealOverTimeBuff(10,15,true);
            
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
                1, 20, 1, 103304);
            _statusManager.ReliefOneDebuff();
        }
        else
        {
            _statusManager.ObtainHealOverTimeBuff(10,15,true);
            // _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            //     30, 30, 1, 103304);
            
            if (ac033.mode == 0)
            {
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
                    1, 20, 1, 103304);
            }else if (ac033.mode == 1)
            {
                _statusManager.HPRegenImmediately(0,20,true);
            }
            else
            {
                _statusManager.ReliefOneDebuff();
            }
        }
        
        
        BattleEffectManager.Instance.SpawnHealEffect(gameObject);
    }


}
