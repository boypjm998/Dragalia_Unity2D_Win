using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Object = UnityEngine.Object;

public class AttackManager_C006 : AttackManagerMeeleWithFS
{
    [SerializeField] protected GameObject rollAttackFX;
    [SerializeField] protected GameObject counterAttackFX;
    
    private List<AttackBase> counterTriggeredList = new();

    private TimerBuff counterTimerBuff;
    private TimerBuff skill2TimerBuff;
    private TimerBuff skill2TimerDebuff;
    private int effectIncrement = 0;
    private GameObject skill2FXInstance;

    private bool enhanced = false;
    public bool summoned { get; private set; } = false;

    protected override void Start()
    {
        base.Start();
        (ac as ActorController).OnDodgeSuccessed += PerformCounterAttack;
        counterTimerBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefDebuff,
            5, 15, 1, 100602);
        skill2TimerDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.CritRateDebuff,
            10, 20, 1, 100606);
        skill2TimerBuff = new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            65, 20, 1, 100604);
        skill2TimerBuff.dispellable = false;
        skill2TimerDebuff.dispellable = false;
        
        _statusManager.OnBuffEventDelegate += GrantImmunityToControl;
        _statusManager.OnBuffDispelledEventDelegate += CancelImmunityToControl;
        _statusManager.OnBuffExpiredEventDelegate += CancelImmunityToControl;
    }

    private void OnDestroy()
    {
        (ac as ActorController).OnDodgeSuccessed -= PerformCounterAttack;
    }


    private void GrantImmunityToControl(BattleCondition condition)
    {
        if (condition.specialID == 100604)
        {
            enhanced = true;
            _statusManager.ImmuneToAllControlAffliction = true;
            _statusManager.OnSpecialBuffDelegate?.
                Invoke(UI_BuffLogPopManager.SpecialConditionType.ImmuneToControlAffliction.ToString());
        }

        if (condition.buffID == (int)BasicCalculation.BattleCondition.AlmightyRage)
        {
            summoned = true;
        }

    }
    private void CancelImmunityToControl(BattleCondition condition)
    {
        if (condition.specialID == 100604)
        {
            _statusManager.ImmuneToAllControlAffliction = false;
            enhanced = false;
            skill2FXInstance?.SetActive(false);
        }

        if (condition.buffID == (int)BasicCalculation.BattleCondition.AlmightyRage)
        {
            summoned = false;
        }
        
    }



    private void onRollEnter()
    {
        //counterTriggeredList.Clear();
        counterTriggeredList.RemoveAll(attackbase => attackbase == null);
    }

    private void OnRollEnterBase()
    {
        //counterTriggeredList.Clear();
        counterTriggeredList.RemoveAll(attackbase => attackbase == null);
    }


    protected void PerformCounterAttack(AttackBase enemyAttack, GameObject target)
    {
        if (!(ac as ActorController).dodging)
        {
            return;
        }
        
        if (ac.anim.GetCurrentAnimatorStateInfo(0).IsName("roll") ||
            ac.anim.GetCurrentAnimatorStateInfo(0).IsName("roll_attack"))
        {
            if (counterTriggeredList.Contains(enemyAttack))
                return;
            
            counterTriggeredList.Add(enemyAttack);
            
            var proj = InstantiateRanged(counterAttackFX, target.transform.position,
                InitContainer(false), target.transform.position.x < transform.position.x ? 1 : -1);

            var atk = proj.GetComponent<ForcedAttackFromPlayer>();
            
            atk.target = target;
            
            DamageNumberManager.GenerateDodgeText(transform);

        }
        

    }

    public override void ForceStrikeRelease(int currentFSLV)
    {
        if(currentFSLV <= 0)
            return;
        
        base.ForceStrikeRelease(currentFSLV);
        (ac as ActorControllerMeeleWithFS).BladeForceStrikeMove();
        (ac as ActorController_c006).PlayAttackVoice(9);
    }

    protected void AddEventChargeDModeGauge(AttackBase attackBase, float amount)
    {
        AttackBase.AttackBaseDelegate handler = null;
        
        handler = (ab, o) =>
        {
            (_statusManager as PlayerStatusManager).ChargeDP(amount);
            attackBase.OnAttackHit -= handler;
        };
        
        attackBase.OnAttackHit += handler;
    }

    public void Combo1()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[0],container.transform.position,
            container);

        if (enhanced)
            AddCAF(proj.GetComponent<AttackBase>());
    }

    public void Combo2()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[1],container.transform.position,
            container);

        if (enhanced)
        {
            var atkBase = proj.GetComponent<AttackBase>();
            AddCAF(proj.GetComponent<AttackBase>());
            //atkBase.BeforeAttackHit += CheckSpecialBuffBefore;
        }
    }
    
    public void Combo3()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(comboFX[2],container.transform.position,
            container,ac.facedir);
        
        var atkBase = proj.GetComponent<AttackBase>();
        if (enhanced)
        {
            AddCAF(proj.GetComponent<AttackBase>());
            //atkBase.BeforeAttackHit += CheckSpecialBuffBefore;
        }

        if (summoned)
        {
            atkBase.attackInfo[0].dmgModifier[0] *= 1.5f;
        }

        AddEventChargeDModeGauge(atkBase, 1);
    }
    
    public void Combo3_Dash()
    {
        
        
        
        
        
        var targetpos = transform.position + new Vector3(10 * ac.facedir, 0, 0);
        
        //向前发射一条射线，如果碰到Characters的layer，则返回其碰撞点
        var hit = Physics2D.Raycast(transform.position,
            new Vector2(ac.facedir, 0), 10,
            LayerMask.GetMask("Enemies"));
        
        if (hit.collider != null)
        {
            if(Mathf.Abs(targetpos.x - transform.position.x) > 2.5f)
                targetpos = hit.point - 2.5f * new Vector2(ac.facedir,0);
            
        }
        
        
        
        targetpos = BattleStageManager.Instance.OutOfRangeCheck(targetpos);
        
        var currentCollider = gameObject.RaycastedPlatform();
        
        
        
        if(targetpos.x < transform.position.x && ac.facedir == 1)
            targetpos.x = transform.position.x;
        else if(targetpos.x > transform.position.x && ac.facedir == -1)
            targetpos.x = transform.position.x;
        
        
        if(targetpos.x > gameObject.RaycastedPlatform().bounds.max.x)
            targetpos.x = currentCollider.bounds.max.x;
        else if(targetpos.x < gameObject.RaycastedPlatform().bounds.min.x)
            targetpos.x = currentCollider.bounds.min.x;
        
        
        StartCoroutine((ac as ActorController).HorizontalMoveFixedTime(
            targetpos.x,
            0.25f,"combo3"));
        
        
    }
    
    public void Combo4()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[3],container.transform.position,
            container);
        
        if (enhanced)
        {
            var atkBase = proj.GetComponent<AttackBase>();
            AddCAF(proj.GetComponent<AttackBase>());
            //atkBase.BeforeAttackHit += CheckSpecialBuffBefore;
        }
    }
    
    public void Combo5()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[4],container.transform.position,
            container);
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            15,30,3,100603);
        
        var atkBase = proj.GetComponent<AttackBase>();
        if (enhanced)
        {
            AddCAF(proj.GetComponent<AttackBase>());
            //atkBase.BeforeAttackHit += CheckSpecialBuffBefore;
        }
        if (summoned)
        {
            atkBase.attackInfo[0].dmgModifier[0] *= 1.5f;
            atkBase.attackInfo[0].dmgModifier[1] *= 1.5f;
            atkBase.attackInfo[1].dmgModifier[0] *= 1.5f;
        }
        
        AddEventChargeDModeGauge(proj.GetComponent<AttackBase>(), 2);
    }
    
    public void Combo6()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[5],container.transform.position,
            container);

        if (enhanced)
        {
            var atkBase = proj.GetComponent<AttackBase>();
            AddCAF(proj.GetComponent<AttackBase>());
            //atkBase.BeforeAttackHit += CheckSpecialBuffBefore;
        }

        
        
    }
    
    public void Combo7()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[6],container.transform.position,
            container);
        
        var atkBase = proj.GetComponent<AttackBase>();
        if (enhanced)
        {
            //atkBase.BeforeAttackHit += CheckSpecialBuffBefore;
            AddCAF(proj.GetComponent<AttackBase>());
        }
        AddEventChargeDModeGauge(proj.GetComponent<AttackBase>(), 2);
    }
    
    public void Combo8()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[7],container.transform.position,
            container);
        
        var atkBase = proj.GetComponent<AttackBase>();
        
        if(enhanced)
            AddCAF(proj.GetComponent<AttackBase>());
        
        if (summoned)
        {
            atkBase.attackInfo[0].dmgModifier[0] *= 1.5f;
        }
        
        AddEventChargeDModeGauge(proj.GetComponent<AttackBase>(), 2);
    }
    
    public void Combo9A()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(comboFX[8],container.transform.position,
            container);
        
        if (enhanced)
        {
            var atkBase = proj.GetComponent<AttackBase>();
            AddCAF(proj.GetComponent<AttackBase>());
        }
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            15,30,3,100603);
    }
    
    public void Combo9B()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);

        Transform targetTrans = (ac as ActorController).ta.GetNearestTargetInRangeDirection
            (ac.facedir, 12, 8f, LayerMask.GetMask("Enemies"));

        Vector3 targetPos = targetTrans
            ? targetTrans.gameObject.RaycastedPosition()
            : new Vector3(transform.position.x, gameObject.RaycastedPosition().y);
        
        
        var proj = InstantiateRanged(comboFX[9],
            targetPos,
            container,1);

        var atkBase = proj.GetComponent<AttackBase>();
        if(enhanced)
            AddCAF(proj.GetComponent<AttackBase>());
        if (summoned)
        {
            atkBase.attackInfo[0].dmgModifier[0] *= 1.5f;
        }
        AddEventChargeDModeGauge(atkBase, 4);
        
    }


    private void CheckSpecialBuffBefore(AttackBase atk, GameObject tar)
    {
        print("检查特殊buff");
        if (tar.GetComponent<StatusManager>().GetConditionWithSpecialID(100602).Count > 0)
        {
            print("有特殊buff");
            atk.BeforeAttackHit -= CheckSpecialBuffBefore;
            (atk as AttackFromPlayer).inspired = true;
        }
    }

    private void AddCAF(AttackBase atk)
    {
        Func<StatusManager,StatusManager,bool> condFunc =
            (sourceStat, targetStat) =>
            {
                if(targetStat.GetConditionWithSpecialID(100602).Count > 0)
                    return true;
                return false;
            };
        var caf = new ConditionalAttackEffect(condFunc,ConditionalAttackEffect.ExtraEffect.ExtraCritRate,
            new string[]{},new string[]{"999"});
            
       atk.AddConditionalAttackEffect(caf);
    }

    public void RollAttack()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(rollAttackFX,container.transform.position
                                                        +new Vector3(0,0.5f,0),
            container);

        if (enhanced)
        {
            var buff = new TimerBuff(counterTimerBuff);
            proj.GetComponent<AttackFromPlayer>().AddWithConditionAll(buff,100);
        }


    }


    public void Skill1(int eventID)
    {
        if (eventID == 1)
        {
            InstantiateRanged(skillFX[0], transform.position, InitContainer(false),1);
        }
        else
        {
            var projAtk = InstantiateMeele(skillFX[1], transform.position,
                InitContainer(true,1,true)).GetComponent<AttackFromPlayer>();
            projAtk.attackInfo[0].dmgModifier[0] *= (1 + 0.33f * (ac as ActorController_c006).skill1Increment);
        }


    }

    public void Skill2()
    {
        
        var proj = InstantiateRanged(skillFX[2], transform.position, InitContainer(false),1);
        proj.AddComponent<RelativePositionRetainer>().SetParent(transform);
        
        skill2TimerBuff.SetEffect(65 + effectIncrement*15);
        effectIncrement = effectIncrement >= 3 ? 3 : effectIncrement + 1;
        _statusManager.ObtainTimerBuff(new TimerBuff(skill2TimerBuff));
        _statusManager.ObtainTimerBuff(new TimerBuff(skill2TimerDebuff));

        if (skill2FXInstance == null)
        {
            skill2FXInstance = Instantiate(skillFX[3], transform.position, Quaternion.identity,
                MeeleAttackFXLayer.transform);
        }
        else
        {
            skill2FXInstance.SetActive(true);
        }


    }


    public void DragonDriveSkill(int eventID)
    {
        if (eventID == 1)
        {
            InstantiateBuff(skillFX[4], transform.position);
            _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.AlmightyRage,
                1, 30, 1,
                100605);
        }
        else if (eventID == 2)
        {
            var bahamutAttackGO = 
                InstantiateSealedContainer(skillFX[5], false, true);

            bahamutAttackGO.transform.position = gameObject.RaycastedPosition() + new Vector2(-ac.facedir * 3, 4.5f);
            bahamutAttackGO.transform.localScale = new Vector3(ac.facedir, 1, 1);
            bahamutAttackGO.transform.position = bahamutAttackGO.transform.SafePosition();

            var atk = bahamutAttackGO.transform.Find("burst").GetComponent<AttackFromPlayer>();
            atk.playerpos = transform;

        }

    }


    private void TestAddConditionalEffect(AttackBase attackBase)
    {
        Func<StatusManager, StatusManager, bool> condfunc = (source, target) =>
        {
            if(enhanced && target.GetConditionWithSpecialID(100602).Count > 0)
                return true;
            return false;
        };


        ConditionalEffect conditionalEffect = new ConditionalEffect(condfunc, new object[] { 999 },
            ConditionalEffect.EffectType.CritRateUp);




    }




}
