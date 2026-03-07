using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class HB05_BehaviorTree_Legend : HB05_BehaviorTree
{
    private EnemyMoveController_HB05_Legend enemyAttackManagerL;
    public const int ResurrectionSpring = 20251;
    public const int CocytusTorture = 20261;

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManagerL = GetComponent<EnemyMoveController_HB05_Legend>();
        OnBehaviorStart += InitAbilities;
    }

    private void InitAbilities()
    {
        //dragonDrive = true;
        
        OnBehaviorStart -= InitAbilities;
        
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(
            ResurrectionSpring,new EnemyAbilityIconEvent(0),status);
        BattleStageManager.Instance.InvokeEnemyAbilityEvent(
            CocytusTorture,new EnemyAbilityIconEvent(0),status);
        status.ImmuneToAllControlAffliction = true;
        
    }


    protected override void GrantDemonSeal()
    {
        OnBehaviorStart -= GrantDemonSeal;

        // var demonSealDebuffInstance = status.GetConditionsOfType(demonSealDebuff.buffID)[0];
        // demonSealDebuffInstance.SetDuration(120);
        var duration = difficulty <= 4 ? 240 : 180;
        
        demonSealDebuff.SetDuration(duration);
        demonSealDebuff.lastTime = duration;
        demonSealDebuff.dispellable = false;
        status.ObtainTimerBuff(new TimerBuff(demonSealDebuff));
        
        status.OnBuffExpiredEventDelegate += SealReleased;
        status.OnBuffDispelledEventDelegate += SealReleased;
    }


    protected override void ParseAction(int state, int substate)
    {
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.HB1005 actionType = 
            (DragaliaEnemyActionTypes.HB1005) Enum.Parse(typeof(DragaliaEnemyActionTypes.HB1005), action_name);

        switch (actionType)
        {
            case DragaliaEnemyActionTypes.HB1005.ComboA:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboA(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.ComboB:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboB(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.CocytusWhirl:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CocytusWhirl(interval,_currentActionStage.args.Length==1));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.ConquestEvil:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BanishEvil(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Dragondrive:
            {
                // if (_currentActionStage.args.Length == 0)
                // {
                //     currentAction = StartCoroutine(ACT_DragondriveCancel());
                //     break;
                // }
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Dragondrive(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.AcheronFount:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AcheronFount(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Warp:
            {
                float posX = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                float posY = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                currentAction = StartCoroutine(ACT_WarpToPosition(new Vector2(posX,posY),interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.IcePillar:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_TopdownIce(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.IceBreaker:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_MagicCircle(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Fog:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_NiflheimMist(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.SnowStorm:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SnowStorm(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.IceBlast:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_IceBlast(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Buff:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_PowerUp(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Platform:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_IcePlatform(interval,_currentActionStage.args.Length>1));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.SetWorld:
            {
                int world = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (world == 1)
                {
                    currentAction = StartCoroutine(ACT_SummonFountain(interval));
                }else if (world == 2)
                {
                    currentAction = StartCoroutine(ACT_StartHellMode(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_PauseHellMode(interval));
                }
                
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.SmashDown:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SmashDown(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.ComboC:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboC(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Forward:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Forward(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Pact:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_PactSealed(interval,type));
                
                break;
            }
            
            case DragaliaEnemyActionTypes.HB1005.Meteor:
            {
                float interval;
                if (_currentActionStage.args.Length > 1)
                {
                    interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    var delay = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_DelayedMeteor(interval,
                        Mathf.Clamp(delay,5,30)));
                }
                else
                {
                    interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_DelayedMeteor(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Around:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Around(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Cascade:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Cascade(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Ultimate:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Ultimate(interval));
                if (_currentActionStage.args.Length > 1)
                {
                    supermoveAdvanced = true;
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.WaterSpout:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WaterSpout(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.Perish:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_LifeExchange(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1005.GroundFrost:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_EternalFrost(interval));
                break;
            }
            
            
        }
        
        
        
        
    }
    
    
    private IEnumerator ACT_IcePlatform(float interval, bool destroy = false)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt);
        
        
        if (destroy)
        {
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action15_C());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action15());
        }
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        
        breakable = true;

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    private IEnumerator ACT_SummonFountain(float interval)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt);

        
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action16());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    private IEnumerator ACT_StartHellMode(float interval)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action17());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    private IEnumerator ACT_PauseHellMode(float interval)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action18());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    private IEnumerator ACT_SmashDown(float interval)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action19());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    private IEnumerator ACT_ComboC(float interval)
    {
        ActionStart();
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
        (enemyController.MoveToSameGround(
            targetPlayer,4,15));
        yield return _moveIsNull;

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action20());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_Forward(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,10));
        yield return _moveIsNull;
        

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action21());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_PactSealed(float interval,int type = 1)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action22(type));
        
        yield return _attackIsNull;
        
        breakable = true;
        enemyController.SetKBRes(status.knockbackRes);

        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_DelayedMeteor(float interval,float delay = 10)
    {
        ActionStart();
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action25(delay));
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Around(float interval,float delay = 10)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        if (transform.DistanceX(targetPlayer) > 20f ||
            transform.DistanceY(targetPlayer) > 8f)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action19());
            yield return _attackIsNull;
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,10));
            yield return _moveIsNull;
        }

        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action26());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Cascade(float interval,float delay = 10)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);

        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action27());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_Ultimate(float interval)
    {
        ActionStart();
        
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action28());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_WaterSpout(float interval)
    {
        ActionStart();
        
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action29());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_LifeExchange(float interval)
    {
        ActionStart();
        enemyController.SetKBRes(999);
        breakable = false;
        status.ImmuneToAllControlAffliction = true;
        yield return new WaitUntil(() => !enemyController.hurt);

        
        enemyController.SetActionUnable(false);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action30());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        status.ImmuneToAllControlAffliction = false;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    protected IEnumerator ACT_EternalFrost(float interval)
    {
        ActionStart();
        
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB05_Action31());
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    
    
}
