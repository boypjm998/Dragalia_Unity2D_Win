using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameMechanics;
using UnityEngine;

public class AttackManager_C018 : AttackManagerRanged
{
    private AttackContainer combo2Container;
    private AttackContainer combo4Container;

    private UI_ForceStrikeAimerArrow forceStrikeIndicator;

    public GameObject[] combo6FX;

    private TimerBuff _timerBuff1;
    private TimerBuff _timerBuff2;
    private TimerBuff _timerBuff3;

    protected override void Awake()
    {
        base.Awake();
        _timerBuff1 = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
            50, -1, 1, 101802);
        _timerBuff2 = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            20, -1, 1, 101802);
        _timerBuff3 = new TimerBuff((int)BasicCalculation.BattleCondition.Shield,
            30, -1, 1, 101802);
        // _timerBuff1.dispellable = false;
        // _timerBuff2.dispellable = false;
        // _timerBuff3.dispellable = false;
        _statusManager.OnBuffExpiredEventDelegate += RemoveBuffsWithBlessing;
        _statusManager.OnBuffDispelledEventDelegate += RemoveBuffsWithBlessing;
        
    }

    public override void ComboAttack1()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");

        var atk1 = InstantiateDirectionalRanged(combo1FX[0],shotPoint.position,container,ac.facedir,0);
        var atk2 = InstantiateDirectionalRanged(combo1FX[1],shotPoint.position,container,ac.facedir,3);
        var atk3 = InstantiateDirectionalRanged(combo1FX[1],shotPoint.position,container,ac.facedir,-3);

    }

    public override void ComboAttack2()
    {
        
        var container = InitContainer(false);
        combo2Container = container.GetComponent<AttackContainer>();
        
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo2FX[0],shotPoint.position,
            combo2Container.gameObject,ac.facedir,0);

    }
    
    public void ComboAttack2_2()
    {
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo2FX[0],shotPoint.position,
            combo2Container.gameObject,ac.facedir,0);
    }
    
    public override void ComboAttack3()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");

        var atk1 = InstantiateDirectionalRanged(combo3FX[0],shotPoint.position,container,ac.facedir,0);
        var atk2 = InstantiateDirectionalRanged(combo3FX[1],shotPoint.position,container,ac.facedir,3);
        var atk3 = InstantiateDirectionalRanged(combo3FX[1],shotPoint.position,container,ac.facedir,-3);

    }
    
    public override void ComboAttack4()
    {
        
        var container = InitContainer(false);
        combo4Container = container.GetComponent<AttackContainer>();
        
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo4FX[0],shotPoint.position,
            combo4Container.gameObject,ac.facedir,0);

    }
    
    public void ComboAttack4_2()
    {
        var shotPoint = FindShotpointInChildren("StandardAttack");
        
        var atk = InstantiateDirectionalRanged(combo4FX[0],shotPoint.position,
            combo4Container.gameObject,ac.facedir,0);
    }

    public override void ComboAttack5()
    {
        var container = InitContainer(false);

        var shotPoint = FindShotpointInChildren("StandardAttack");

        var atk1 = InstantiateDirectionalRanged(combo5FX[0],shotPoint.position,container,ac.facedir,0);
        var atk2 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,2);
        var atk3 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,-2);
        var atk4 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,4);
        var atk5 = InstantiateDirectionalRanged(combo5FX[1],shotPoint.position,container,ac.facedir,-4);
    }

    public void Combo6_ForcingFX()
    {

        var fx = InstantiateBuff(combo6FX[0],
            transform.position + new Vector3(2*ac.facedir, 0, 0));

        ActorBase.OnHurt handler = null;

        handler = () =>
        {
            ac.OnAttackInterrupt -= handler;
            //var transform = GetComponent<Transform>();
            Destroy(fx.gameObject);
        };

        ac.OnAttackInterrupt += handler;
        
        AddParticleSystemSpeedModifier(fx);
    }

    public void Combo6Attack()
    {
        var container = InitContainer(false);

        var fx = InstantiateDirectionalRanged(combo6FX[1],
            transform.position + new Vector3(1.5f * ac.facedir, 0, 0),
            container,ac.facedir,0);

        var attack = fx.GetComponent<AttackFromPlayer>();

        attack.OnAttackDealDamage += (statusManagerSelf, statusManagerTarget, attack, dmg) =>
        {
            LifeSteal(statusManagerSelf,(int)dmg,5,7);
        };
        var rand = BasicCalculation.RandInt(0, 100);
        print("随机数:"+rand);
        if (rand < 70)
        {
            _statusManager.EnergyLevelUp(1);
        }
    }






    

    public void BowJumpShootAttack(float angle)
    {
        angle = Mathf.Round(angle);
        if (angle > 40)
        {
            angle = 40;
        }
        if (angle < -40)
        {
            angle = -40;
        }

        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.gameObject.transform);
        
        var shotPoint = 
            new Vector2
            (transform.position.x + 2.5f * ac.facedir * Mathf.Cos(angle*Mathf.Deg2Rad),
                transform.position.y + 2f * Mathf.Sin(angle*Mathf.Deg2Rad));
        
        
        var atk = InstantiateDirectionalRanged(dashFX[1],shotPoint,container,ac.facedir,angle*ac.facedir);
        var atk2 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle+3)*ac.facedir);
        var atk3 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle+6)*ac.facedir);
        var atk4 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle-6)*ac.facedir);
        var atk5 = InstantiateDirectionalRanged(dashFX[2],shotPoint,container,ac.facedir,(angle-3)*ac.facedir);

        if ((ac as ActorController_c018).sigilReleased)
        {
            var attacks = new GameObject[] {atk,atk2,atk3,atk4,atk5};
            foreach (var attack in attacks)
            {
                attack.GetComponent<AttackFromPlayer>().AddWithConditionAll(new TimerBuff(999),100);
            }
        }

        
        
        
        
    }

    public void ForceStrikeCharging()
    {
        if (forceStrikeIndicator == null)
        {
            var prefabIndicator = Instantiate(ForceFX[0], transform.position, Quaternion.identity,
                BuffFXLayer.gameObject.transform);
            forceStrikeIndicator = prefabIndicator.GetComponent<UI_ForceStrikeAimerArrow>();
            prefabIndicator.name = "ForceStrikeIndicator";
            forceStrikeIndicator.SetActorController(ac as ActorController_c018);
            forceStrikeIndicator.SetMaxForceInfo(new float[] {0.5f,2}.ToList());
        }
        else
        {
            forceStrikeIndicator.gameObject.SetActive(true);
        }

    }

    public override void ForceStrikeRelease(int forceLevel = 0)
    {
        if(forceLevel <= 0)
            return;
        
        
        (ac as ActorController_c018).PlayAttackVoice(9);

        var container = InitContainer(false);

        var attackProjectile = InstantiateRanged(ForceFX[1],
            transform.position + new Vector3(ac.facedir * 2.5f, 0, 0),
            container, ac.facedir);
        
        

        if (ac.facedir == -1)
        {
            attackProjectile.GetComponent<DOTweenSimpleController>().moveDirection *= -1;
        }

        var attack = attackProjectile.GetComponent<AttackFromPlayer>();

        if (forceLevel >= 2)
        {
            attack.hitShakeIntensity += 3;
            attack.hitConnectEffect = null;
            attack.hitConnectEffect = ForceFX[2];
            
            attackProjectile.transform.localScale = new Vector3(ac.facedir,1,1);
            attack.attackInfo[0].dmgModifier[0] = 6;
            attack.extraODModifier = 0.5f;
            attack.SetSpGain(710);
            
            
            var checkConditionString = ((int)BasicCalculation.BattleCondition.Frostbite).ToString();
        
            var conditional_eff = new ConditionalAttackEffect
            (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                new string[] {"1", checkConditionString},
                new string[] {"2"});
            
            attack.AddConditionalAttackEffect(conditional_eff);
            if((ac as ActorController_c018).sigilReleased)
                return;
            attack.OnAttackHit += ReduceSigilTime;

        }


    }

    public override void Skill1(int eventID)
    {
        if (eventID == 0)
        {
            var fx = InstantiateBuff(skill1FX[0], transform.position - new Vector3(0,1.3f,0));
            AddAnimationSpeedModifier(fx);
            
            ActorBase.OnHurt handler = null;

            handler = () =>
            {
                ac.OnAttackInterrupt -= handler;
                //var transform = GetComponent<Transform>();
                Destroy(fx.gameObject);
            };

            ac.OnAttackInterrupt += handler;
        }
        
        else if (eventID == 1)
        {
            var container = InitContainer(false, 1, true);
            var attackProj = InstantiateDirectionalRanged(skill1FX[1],
                transform.position + new Vector3(3 * ac.facedir, 0, 0), container, ac.facedir, 0);


            if ((ac as ActorController_c018).sigilReleased)
            {
                attackProj.GetComponent<AttackFromPlayer>().AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
                    41f,21f,100,-1),170);
            }
            else
            {
                attackProj.GetComponent<AttackFromPlayer>().AddWithConditionAll
                (new TimerBuff((int)BasicCalculation.BattleCondition.Frostbite,
                    41f,21f,100,-1),120);
            }

            
        }
    }

    public override void Skill2(int eventID)
    {
        TimerBuff blessingBuff = new TimerBuff((int)BasicCalculation.BattleCondition.GabrielsBlessing,
            -1, 40, 1);
        blessingBuff.dispellable = true;
        
        _statusManager.ObtainTimerBuff(blessingBuff);
        _statusManager.ObtainTimerBuff(_timerBuff1,false);
        _statusManager.ObtainTimerBuff(_timerBuff2,false);
        _statusManager.ObtainTimerBuff(_timerBuff3,false);

        

        var fx = InstantiateBuff(skill2FX[0], transform.position);
        AddParticleSystemSpeedModifier(fx);
    }

    public override void Skill3(int eventID)
    {

        if (eventID == 1)
        {
            var container = InitContainer(false, 1, true);

            var proj = InstantiateRanged(skill3FX[0], transform.position, container, 1);

        
            var atk = proj.GetComponent<AttackFromPlayer>();
            
            var freeze = new TimerBuff((int)BasicCalculation.BattleCondition.Freeze,
                -1,7,1,-1);
            atk.AddWithConditionAll(freeze,110);

            if ((ac as ActorController_c018).sigilReleased)
            {
                
                var checkConditionString = ((int)BasicCalculation.BattleCondition.Frostbite).ToString();
        
                var conditional_eff = new ConditionalAttackEffect
                (ConditionalAttackEffect.ConditionType.TargetHasCondition,
                    ConditionalAttackEffect.ExtraEffect.ChangeDmgModifier,
                    new string[] {"1", checkConditionString},
                    new string[] {"1"});
            
                atk.AddConditionalAttackEffect(conditional_eff);
            }

            
        }
        else
        {
            Instantiate(skill3FX[1], transform.position, Quaternion.identity,
                RangedAttackFXLayer.transform);
        }




    }
    
    

    private void ReduceSigilTime(AttackBase attackBase,GameObject target)
    {

        attackBase.OnAttackHit -= ReduceSigilTime;
        var sigilLockedBuff = _statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.LockedSigil)[0];

        if (sigilLockedBuff != null)
        {
            sigilLockedBuff.lastTime -= 9;
            _statusManager.OnSpecialBuffDelegate?.
                Invoke(UI_BuffLogPopManager.SpecialConditionType.ReduceSigilTime.ToString());
            
        }
    }

    private void RemoveBuffsWithBlessing(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.GabrielsBlessing)
        {
            //var relatedBuffs = _statusManager.GetConditionWithSpecialID(101802);
            _statusManager.RemoveAllConditionWithSpecialID(101802);
        }



    }





}
