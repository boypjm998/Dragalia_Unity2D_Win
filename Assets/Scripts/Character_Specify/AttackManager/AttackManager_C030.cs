using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class AttackManager_C030 : AttackManagerRanged
{
    private bool skill2Defense = false;
    private TimerBuff _atkRateBuff = new TimerBuff((int)BasicCalculation.BattleCondition.AttackRateUp,
        10,-1,1,103001);
    private TimerBuff _skillDmgBuff = new TimerBuff((int)BasicCalculation.BattleCondition.SkillDmgBuff,
        20,-1,1,103002);
    private bool accerating = false;
    private bool skillBuffed = false;
    
    private TimerBuff _atkBuff2 = new TimerBuff((int)BasicCalculation.BattleCondition.AtkBuff,
        20,25,100);
    private TimerBuff _defbuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
        20,25,100);

    protected override void Start()
    {
        base.Start();
        (_statusManager as PlayerStatusManager).SetSPChargeRate(2,0);
        _atkRateBuff.dispellable = false;
        _skillDmgBuff.dispellable = false;
        _defbuff.dispellable = false;
        _atkBuff2.dispellable = false;
        
        _statusManager.comboConnectMaxInterval += 1;
        
        _statusManager.OnComboConnect += CheckComboBuff;
        _statusManager.OnBuffEventDelegate += CheckConditionGet;
        _statusManager.OnBuffExpiredEventDelegate += CheckConditionPurge;
        _statusManager.OnBuffDispelledEventDelegate += CheckConditionPurge;
    }

    public void Skill1_Muzzle()
    {
        var container = InitContainer(false, 1, true);
        
        var muzzleMain = Instantiate(skill1FX[0],
            transform.position + new Vector3(2*ac.facedir,0),Quaternion.identity,
            container.transform);

        float waitTime = 1;

        float attackRate = 0.01f*(ac as ActorController).attackRate + 1;

        waitTime -= (ac as ActorController).attackRate * 0.01f;

        if (ac.facedir == -1)
        {
            muzzleMain.GetComponent<DOTweenSimpleController>().moveDirection.x *= -1;
            muzzleMain.transform.localScale = new Vector3(-1,1,1);
        }
        
        muzzleMain.GetComponent<DOTweenSimpleController>().SetWaitTime(waitTime);
        
        var blast = muzzleMain.GetComponent<BlastProjectileCustom>();
        blast.src = gameObject;
        blast.OnBlast += (atk) =>
        {
            atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Burn,95,
                12,100),120);
            atk.AddWithConditionAll(new TimerBuff(999),100,1);
        };

        ActorBase.OnHurt handler = null;

        handler = () =>
        {
            Destroy(container);
        };
        
        ac.OnAttackInterrupt += handler;

        DOVirtual.DelayedCall(waitTime, () =>
        {
            muzzleMain.GetComponent<Collider2D>().enabled = true;
            ac.OnAttackInterrupt -= handler;
        }, false);
        
        var smallProjPrefab = skill1FX[1];

        int totalBuffStack = _statusManager.GetBuffStackNum();
        
        if (totalBuffStack > 0)
        {
            _statusManager.OnSpecialBuffDelegate?.Invoke(
                UI_BuffLogPopManager.SpecialConditionType.BuffCount.ToString() + "_" + totalBuffStack);
        }
        
        totalBuffStack = Mathf.Clamp(totalBuffStack, 0, 10);
        
        
        List<Vector2> positionList = new List<Vector2>()
        {
            new(-2 * ac.facedir,0),
            
            new(-1.98f*ac.facedir, -0.24f),
            new(-1.98f*ac.facedir,  0.24f),
            
            new(-1.88f*ac.facedir, -0.68f),
            new(-1.88f*ac.facedir,  0.68f),
            
            new(-1.64f*ac.facedir, -1.1f),
            new(-1.64f*ac.facedir,  1.1f),
            
            new(-1.36f*ac.facedir, -1.44f),
            new(-1.36f*ac.facedir,  1.44f),
            
            new(-1*ac.facedir, -1.73f),
            new(-1*ac.facedir,  1.73f),
        };

        List<Vector2> offsetList = new();

        var target = ta.GetNearestTargetInRangeDirection(ac.facedir, 30, 8,
            LayerMask.GetMask("Enemies"));

        Collider2D targetCollider = null;
        if (target != null)
        {
            targetCollider = target.GetComponent<Collider2D>();
        }
        
        switch (totalBuffStack)
        {
            case 1:
                offsetList.Add(positionList[0]);
                break;
            case 2:
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                break;
            case 3:
                offsetList.Add(positionList[0]);
                offsetList.Add(positionList[5]);
                offsetList.Add(positionList[6]);
                break;
            case 4:
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                offsetList.Add(positionList[9]);
                offsetList.Add(positionList[10]);
                break;
            case 5:
                offsetList.Add(positionList[0]);
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                offsetList.Add(positionList[7]);
                offsetList.Add(positionList[8]);
                break;
            case 6:
                offsetList.Add(positionList[1]);
                offsetList.Add(positionList[2]);
                offsetList.Add(positionList[5]);
                offsetList.Add(positionList[6]);
                offsetList.Add(positionList[9]);
                offsetList.Add(positionList[10]);
                break;
            case 7:
                offsetList.Add(positionList[0]);
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                offsetList.Add(positionList[5]);
                offsetList.Add(positionList[6]);
                offsetList.Add(positionList[9]);
                offsetList.Add(positionList[10]);
                break;
            case 8:
                offsetList.Add(positionList[1]);
                offsetList.Add(positionList[2]);
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                offsetList.Add(positionList[7]);
                offsetList.Add(positionList[8]);
                offsetList.Add(positionList[9]);
                offsetList.Add(positionList[10]);
                break;
            case 9:
                offsetList.Add(positionList[0]);
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                offsetList.Add(positionList[5]);
                offsetList.Add(positionList[6]);
                offsetList.Add(positionList[7]);
                offsetList.Add(positionList[8]);
                offsetList.Add(positionList[9]);
                offsetList.Add(positionList[10]);
                break;
            case 10:
                offsetList.Add(positionList[1]);
                offsetList.Add(positionList[2]);
                offsetList.Add(positionList[3]);
                offsetList.Add(positionList[4]);
                offsetList.Add(positionList[5]);
                offsetList.Add(positionList[6]);
                offsetList.Add(positionList[7]);
                offsetList.Add(positionList[8]);
                offsetList.Add(positionList[9]);
                offsetList.Add(positionList[10]);
                break;
            default: break;

        }

        float distance = 30;
        if (targetCollider != null)
        {
            distance = Vector2.Distance(targetCollider.bounds.center,
                muzzleMain.transform.position);
        }


        foreach (var offset in offsetList)
        {
            var proj = InstantiateRanged(smallProjPrefab,transform.position + (Vector3)offset,
                container,ac.facedir);

            float time = 0.6f;

            if (targetCollider != null)
            {
                time = (distance / 30f) + 0.1f;

                proj.transform.DOMove(targetCollider.bounds.center, time).OnComplete(() =>
                {
                    proj.GetComponent<ForcedAttackFromPlayer>().target = target.parent.gameObject;
                    proj.GetComponent<ForcedAttackFromPlayer>().NextAttack();
                }).OnStart(() =>
                {
                    proj.GetComponent<Collider2D>().enabled = true;
                }).SetDelay(waitTime).SetUpdate(UpdateType.Fixed);
            }
            else
            {
                proj.transform.DOMoveX(transform.position.x + ac.facedir * 30, time).
                   OnStart(() =>
                {
                    proj.GetComponent<Collider2D>().enabled = true;
                }).SetDelay(waitTime).SetUpdate(UpdateType.Fixed);
            }
            
            
        }


    }

    public void Skill2()
    {
        if (skill2Defense)
        {
            _statusManager.ObtainTimerBuff(new TimerBuff(_defbuff),false);
            BattleStageManager.Instance.TriggerSkillIconEvent(2,0);
            
            skill2Defense = false;
            
            var currentBuffAmount = _statusManager.GetConditionTotalValue((int)BasicCalculation.BattleCondition.MaxHPBuff);

            var remainBuff = Mathf.Min(30 - currentBuffAmount, 10);

            if (remainBuff > 0)
            {
                _statusManager.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.MaxHPBuff,
                    remainBuff, -1, 100),false);
            }
            
            _statusManager.HPRegenImmediatelyWithoutRandomDirectly(_statusManager,
                (Mathf.CeilToInt(_statusManager.maxBaseHP * 0.1f)));
            
            
        }
        else
        {
            BattleStageManager.Instance.TriggerSkillIconEvent(2,1);
            skill2Defense = true;
        }
        
        _statusManager.ObtainTimerBuff(new TimerBuff(_atkBuff2));
        (_statusManager as PlayerStatusManager).FillSP(0,100);
        _statusManager.OnSpecialBuffDelegate?.Invoke(UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());

        InstantiateBuff(skill2FX[0], transform.position);

    }

    private void CheckConditionGet(BattleCondition cond)
    {
        if (!accerating && cond.buffID == (int)BasicCalculation.BattleCondition.AtkBuff)
        {
            if (ac.anim.GetBool("isAttack"))
            {
                ac.anim.speed = 1.1f;
            }
            accerating = true;

            _statusManager.ObtainTimerBuff(_atkRateBuff);
        }

        if (!skillBuffed && cond.buffID == (int)BasicCalculation.BattleCondition.DefBuff)
        {
            _statusManager.ObtainTimerBuff(_skillDmgBuff);
            skillBuffed = true;
        }
        
    }

    private void CheckConditionPurge(BattleCondition cond)
    {
        if (GlobalController.currentGameState == GlobalController.GameState.End)
            return;
        
        if(_statusManager.currentHp <= 0)
            return;
        
        
        
        if (accerating && !_statusManager.HasCondition((int)BasicCalculation.BattleCondition.AtkBuff))
        {
            accerating = false;
            if(ac.anim.GetBool("isAttack"))
            {
                ac.anim.speed = 1f;
            }
            _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.AttackRateUp,
                _atkRateBuff.specialID);
        }
        
        if (skillBuffed && !_statusManager.HasCondition((int)BasicCalculation.BattleCondition.DefBuff))
        {
            skillBuffed = false;
            _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.SkillDmgBuff,
                _skillDmgBuff.specialID);
        }
    }

    private void CheckCondition(BattleCondition cond)
    {
        if(cond.buffID != (int)BasicCalculation.BattleCondition.AtkBuff &&
           cond.buffID != (int)BasicCalculation.BattleCondition.DefBuff)
        {
            return;
        }
        
        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.AtkBuff))
        {
            if (!accerating)
            {
                if (ac.anim.GetBool("isAttack"))
                {
                    ac.anim.speed = 1.1f;
                }
                accerating = true;

                _statusManager.ObtainTimerBuff(_atkRateBuff);
            }
        }
        else
        {
            if(ac.anim.GetBool("isAttack"))
            {
                ac.anim.speed = 1f;
            }
            accerating = false;
            
            _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.AtkBuff, _atkRateBuff.specialID);
        }

        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.DefBuff))
        {
            if (!skillBuffed)
            {
                _statusManager.ObtainTimerBuff(_skillDmgBuff);
                skillBuffed = true;
            }

        }else
        {
            _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.DefBuff, _skillDmgBuff.specialID);
        }
        
        
    }

    private void CheckComboBuff()
    {
        if (_statusManager.comboHitCount % 20 == 0)
        {
            _statusManager.
                ObtainTimerBuff((int)BasicCalculation.BattleCondition.AtkBuff, 3, 15);
            _statusManager.
                ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff, 2, 15);
            _statusManager.EnergyLevelUp(1);
            
        }
        
        
        
    }
    
    
    
    
    
    
    
}
