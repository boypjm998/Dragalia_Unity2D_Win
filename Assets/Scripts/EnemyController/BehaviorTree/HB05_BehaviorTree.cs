using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class HB05_BehaviorTree : EnemyBehaviorManager
{

    protected EnemyControllerHumanoidHigh enemyController;
    protected EnemyMoveController_HB05 enemyAttackManager;
    protected TimerBuff demonSealDebuff;
    protected TimerBuff sealReleaseBuff;
    public bool dragonDrive = false;
    public bool sealRemoved = false;
    //protected Tween dragondriveTween;
    [SerializeField] private GameObject p2Prefab;
    [SerializeField] private AudioClip p2BGM;
    
    protected bool supermoveAdvanced = false;
    
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoidHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_HB05>();
        GetBehavior();
        enemyController.OnMoveFinished += FinishMove;
        enemyAttackManager.OnAttackFinished += FinishAttack;

        demonSealDebuff = new TimerBuff((int)BasicCalculation.BattleCondition.DemonSeal,
            1, 300, 1, -1);
        sealReleaseBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DemonSealReleased,
            1, -1, 1, -1);
        OnBehaviorStart += GrantDemonSeal;
        
        if (enemyController.canDeath == false)
        {
            controllAfflictionProtect = true;
            
            status.OnHPBelow0 += () =>
            {
                status.ResetAllStatusForced();
                enemyController.StopAllCoroutines();
                ResetHumanActionsBeforeTransform();
        
                currentAction = StartCoroutine(ChangePhaseAnimationRoutine());
                
                status.OnHPBelow0 = null;
            };
            
        }

    }

    protected virtual void GrantDemonSeal()
    {
        OnBehaviorStart -= GrantDemonSeal;
        
        status.ObtainTimerBuff(new TimerBuff(demonSealDebuff));
        
        status.OnBuffExpiredEventDelegate += SealReleased;
        status.OnBuffDispelledEventDelegate += SealReleased;
    }

    protected void SealReleased(BattleCondition condition)
    {
        if (condition.buffID == (int)BasicCalculation.BattleCondition.DemonSeal)
        {
            status.ObtainTimerBuff(sealReleaseBuff);
            sealRemoved = true;
            
            status.FlashburnRes = 100;
            status.BurnRes = 100;
            status.ScorchrendRes = 100;
            status.StormlashRes = 100;
            status.BogRes = 100;
            status.StunRes = 100;
            status.SleepRes = 100;
            status.BlindnessRes = 100;
            status.ShadowblightRes = 100;
            status.ParalysisRes = 100;
            status.PoisonRes = 100;
            status.FrostbiteRes = 200;
            
            
            status.OnBuffExpiredEventDelegate -= SealReleased;
            status.OnBuffDispelledEventDelegate -= SealReleased;
        }
            
    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        var conditionName = args[0];

        switch (conditionName)
        {
            case "seal_removed":
            {
                if (sealRemoved)
                {
                    dest_state = int.Parse(args[1],CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2],CultureInfo.InvariantCulture);
                    return false;
                }
            }
            case "dragondrive":
            {
                if (dragonDrive)
                {
                    dest_state = int.Parse(args[1],CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2],CultureInfo.InvariantCulture);
                    return false;
                }
            }
            case "hp_below":
            {
                if (status.currentHp < status.maxHP * float.Parse(args[1],CultureInfo.InvariantCulture))
                {
                    dest_state = int.Parse(args[2],CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[3],CultureInfo.InvariantCulture);
                    return false;
                }
            }
            case "advanced":
            {
                
                if (supermoveAdvanced)
                {
                    dest_state = int.Parse(args[1],CultureInfo.InvariantCulture);
                    return true;
                }
                else
                {
                    dest_state = int.Parse(args[2],CultureInfo.InvariantCulture);
                    return false;
                }
            }
            default:
            {
                dest_state = substate + 1;
                break;
            }
        }

        
        return false;
    }

    protected override void DoAction(int state, int substate)
    {
        if(playerAlive == false)
            return;
        
        ParseAction(state,substate);
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
                if (_currentActionStage.args.Length == 0)
                {
                    currentAction = StartCoroutine(ACT_DragondriveCancel());
                    break;
                }
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
        }
        
        
        
        
    }



    protected IEnumerator ACT_ComboA(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        
        if (transform.DistanceX(targetPlayer) > 20f ||
            transform.DistanceY(targetPlayer) > 5f)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action05());
            yield return _attackIsNull;
            
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,2,10));
            yield return _moveIsNull;
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,10));
            yield return _moveIsNull;
        }
        

        enemyController.SetKBRes(999);
        //TODO: 判定是否强化
        if (dragonDrive)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action01_B());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action01_N());
        }
        
        
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }
    protected IEnumerator ACT_ComboB(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        if (transform.DistanceX(targetPlayer) > 20f ||
            transform.DistanceY(targetPlayer) > 5f)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action05());
            yield return _attackIsNull;
            
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,2,10));
            yield return _moveIsNull;
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,10));
            yield return _moveIsNull;
        }

        enemyController.SetKBRes(999);
        //TODO: 判定是否强化
        if (dragonDrive)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action02_B());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action02_N());
        }
        
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_IceBlast(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action05(0.8f));
        
        enemyController.SetKBRes(999);
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
    }

    protected IEnumerator ACT_CocytusWhirl(float interval, bool move = true)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (dragonDrive)
        {
            if (move)
            {
                currentMoveAction = StartCoroutine
                (enemyController.MoveToSameGround(
                    targetPlayer,3,18));
        
                yield return _moveIsNull;
            }

            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action03_B());
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,3,5));
        
            yield return _moveIsNull;

            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action03_N());
        }
        
        
        
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
        
    }

    protected IEnumerator ACT_BanishEvil(float interval)
    {
        ActionStart();

        controllAfflictionProtect = true;

        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (dragonDrive)
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,12));
            
            yield return _moveIsNull;
            
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action04_B());
        }
        else
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action04_N());
            
        }
        
        yield return _attackIsNull;
        
        controllAfflictionProtect = false;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        
        ActionEnd();
        
    }

    protected IEnumerator ACT_Dragondrive(float interval)
    {
        ActionStart();

        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (!dragonDrive)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action06());
            yield return _attackIsNull;
            enemyController.SetKBRes(status.knockbackRes);
            yield return new WaitForSeconds(interval);
        }

        ActionEnd();
        
    }

    protected IEnumerator ACT_DragondriveCancel()
    {
        ActionStart();

        enemyAttackManager.HB05_Action06_C();
        yield return null;
        
        ActionEnd();
    }

    protected IEnumerator ACT_AcheronFount(float interval)
    {
        ActionStart();
        enemyController.SetActionUnable(false);
        
        yield return new WaitUntil(() => !enemyController.hurt);
        
        enemyController.SetKBRes(999);
        if (dragonDrive)
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action07(true));
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action07());
        }
        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);
        yield return new WaitForSeconds(interval);

        ActionEnd();
        
    }
    
    protected IEnumerator ACT_WarpToPosition(Vector2 position, float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        if (Mathf.Abs(transform.position.x - position.x) > 1)
        {

            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action08(position));
            yield return _attackIsNull;


            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(interval);
        }

        ActionEnd();
    }
    
    protected IEnumerator ACT_MagicCircle(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action09());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    protected IEnumerator ACT_TopdownIce(float interval)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        enemyController.SetActionUnable(false);

        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action10());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    
    protected IEnumerator ACT_NiflheimMist(float interval)
    {
        enemyController.SetKBRes(999);
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (difficulty >= 4)
        {
            var superAtk = new TimerBuff(1, 50, status is SpecialStatusManager ? 15:90, 4, 8105401);
            superAtk.dispellable = false;
            status.ObtainTimerBuff(superAtk);
        }

        if (!Projectile_C007_2_Boss.Instance.FogStarted)
        {
            enemyController.SetActionUnable(false);
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action11());
            yield return _attackIsNull;
        
            enemyController.SetKBRes(status.knockbackRes);

            yield return new WaitForSeconds(interval);
        }

        ActionEnd();
    }

    protected IEnumerator ACT_SnowStorm(float interval)
    {
        ActionStart();
        breakable = false;
        enemyController.SetKBRes(999);
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action12());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        breakable = true;
        ActionEnd();
    }
    protected IEnumerator ACT_PowerUp(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManager.HB05_Action13());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        

        ActionEnd();
    }
    
    protected IEnumerator ChangePhaseAnimationRoutine()
    {
        ActionStart();
        enemyController.SetHitSensor(false);
        enemyController.SetActionUnable(false);
        yield return new WaitUntil(() => !enemyController.hurt);
        yield return null;
        
        print("startPhaseChange");
        currentAttackAction = 
            StartCoroutine(enemyAttackManager.HB05_Action14());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        if (Projectile_C007_2_Boss.Instance != null)
        {
            //Destroy(Projectile_C007_2_Boss.Instance.gameObject);
        }
        
        print("转P2");
        
        var p2_boss = Instantiate(p2Prefab,transform.position,Quaternion.identity,transform.parent);
        p2_boss.GetComponent<EnemyController>().TurnMove(targetPlayer);
        
        BattleStageManager.currentDisplayingBossInfo = 2;
        FindObjectOfType<UI_BossStatus>().RedirectBoss(p2_boss,1);
        p2_boss.GetComponent<StatusManager>()?.OnHPChange?.Invoke();
        BattleEffectManager.Instance.PlayBGM(false);
        ActionEnd();
        yield return null;
        
        BattleEffectManager.Instance.SetBGM(p2BGM);
        BattleEffectManager.Instance.PlayBGM(true);
        //BattleStageManager.Instance.RemoveFieldAbility((int)BasicCalculation.EnemyAbility.BlessingWorld);

        Destroy(gameObject);
    }
    
    
}
