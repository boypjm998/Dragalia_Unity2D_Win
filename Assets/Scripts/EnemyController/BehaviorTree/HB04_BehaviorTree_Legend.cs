using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class HB04_BehaviorTree_Legend : HB04_BehaviorTree
{
    [SerializeField] private GameObject p2_prefab;
    [SerializeField] private AudioClip p2_bgm;
    private EnemyMoveController_HB04_Legend enemyAttackManagerL;

    protected override void Awake()
    {
        base.Awake();
        enemyAttackManagerL = enemyAttackManager as EnemyMoveController_HB04_Legend;
        
        status.OnHPBelow0 += CheckPowerOfBonds;
        
        if(enemyAttackManagerL == null)
            status.OnHPBelow0 += ToPhase2;

    }

    protected override void CheckPowerOfBonds()
    {
        if (enemyAttackManagerL == null)
        {
            var cond =
                status.GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds);
            if (cond.Count > 0)
            {
                status.currentHp = 1;
                status.RemoveConditionWithLog(cond[0]);
                status.HPRegenImmediately(0,30);
                status.ImmuneToAllControlAffliction = false;
                totalRevived++;
            }
        }
        else
        {
            var cond =
                status.GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds);
            if (cond.Count > 0)
            {
                status.currentHp = 1;
                status.RemoveConditionWithLog(cond[0]);
                status.HPRegenImmediately(0,20);
                status.ObtainTimerBuff((int)BasicCalculation.BattleCondition.Invincible,
                    1, 20, 1,-1);
                status.ImmuneToAllControlAffliction = false;
                totalRevived++;
            }
        }
    }

    protected override bool CheckCondition(string[] args, out int dest_state)
    {
        if (args[0] == "worldReset")
        {
            if (enemyAttackManagerL.WorldReset)
            {
                dest_state = int.Parse(args[1]);
                return true;
            }
            else
            {
                dest_state = int.Parse(args[2]);
                return false;
            }
        }
        else
        {
            return base.CheckCondition(args, out dest_state);
        }
    }

    protected override void ParseAction(int state, int substate)
    {
        if (enemyAttackManagerL == null)
        {
            base.ParseAction(state, substate);
            return;
        }
        
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }
        
        DragaliaEnemyActionTypes.HB1004 actionType = 
            (DragaliaEnemyActionTypes.HB1004) Enum.Parse(typeof(DragaliaEnemyActionTypes.HB1004), action_name);
        
        switch (actionType)
        {
            case DragaliaEnemyActionTypes.HB1004.comboA:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboA(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.comboB:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ComboB(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.blazingFount:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BlazingFount(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.affectionRing:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                if (_currentActionStage.args.Length > 1)
                {
                    currentAction = StartCoroutine(ACT_RingOfAffection(interval,false));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_RingOfAffection(interval,true));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.warp:
            {
                if (_currentActionStage.args.Length == 3)
                {
                    float posX = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    float posY = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_WarpToPosition(new Vector2(posX,posY),interval));
                }else if (_currentActionStage.args.Length == 1)
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_WarpToNear(interval));
                }
                else
                {
                    float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                    float xPos = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                    currentAction = StartCoroutine(ACT_WarpToAvoid(xPos,interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.wall:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_BlessedWall(1, interval));
                }else if (type == -1)
                {
                    currentAction = StartCoroutine(ACT_BlessedWall(-1, interval));
                }
                else if (_currentActionStage.args.Length == 3)
                {
                    float distance = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    currentAction = StartCoroutine(ACT_BlessedWallDoubleDirection(distance,interval));
                }
                else
                {
                    float distance = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);
                    float center = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[3]);
                    currentAction = 
                        StartCoroutine(ACT_BlessedWallDoubleDirectionFixed(distance,
                            center,interval));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1004.buff:
            {
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.projectiles:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                if (type == 1)
                {
                    currentAction = StartCoroutine(ACT_ProjectilesAttackI(interval));
                }else if (type == 2)
                {
                    currentAction = StartCoroutine(ACT_ProjectilesAttackII(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_ProjectilesAttackIII(interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.celestial:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_VertiStars(type, interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.ball:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ColorfulPillars(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.glare:
            {
                int needGround = int.Parse(_currentActionStage.args[0]);
                int fix = int.Parse(_currentActionStage.args[1]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[2]);

                if (fix == 1)
                {
                    currentAction = StartCoroutine(ACT_StunningGlareFixed(interval));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_StunningGlare(0,needGround,interval));
                }
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.heal:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HolyHeal(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.forward:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_WandingLight(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.shared:
            {
                float waitTime = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SharedAttack(waitTime,interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.ring:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_HolyRing(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.setWorld:
            {
                int type = int.Parse(_currentActionStage.args[0]);
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                currentAction = StartCoroutine(ACT_SetWorld(type,interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.rainbows:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_DawnCirclet(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.squares:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SquaredAOE(interval));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.pillars:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                bool avoidable = _currentActionStage.args.Length == 2;
                currentAction = StartCoroutine(ACT_RipplingLightPillar(interval,avoidable));
                break;
            }
            case DragaliaEnemyActionTypes.HB1004.prayers:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[0]);
                
                if(_currentActionStage.args.Length == 2)
                    currentAction = StartCoroutine(ACT_PrayerProjectiles(interval,
                        int.Parse(_currentActionStage.args[1])));
                else if (_currentActionStage.args.Length == 3)
                {
                    currentAction = StartCoroutine(ACT_PrayerProjectiles(interval,
                        int.Parse(_currentActionStage.args[1]),
                        int.Parse(_currentActionStage.args[2])));
                }
                else if(_currentActionStage.args.Length == 1)
                {
                    currentAction = StartCoroutine(ACT_PrayerProjectiles(interval,
                        Random.Range(0, 1) == 0 ? 1 : -1));
                }
                else if (_currentActionStage.args.Length == 4)
                {
                    currentAction = StartCoroutine(ACT_PrayerProjectiles(interval,
                        int.Parse(_currentActionStage.args[1]),
                        0,
                        int.Parse(_currentActionStage.args[2]),
                        int.Parse(_currentActionStage.args[3])));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_PrayerProjectiles(interval,
                        int.Parse(_currentActionStage.args[1]),
                        int.Parse(_currentActionStage.args[2]),
                        int.Parse(_currentActionStage.args[3]),
                        int.Parse(_currentActionStage.args[4])));
                }

                break;
            }
            case DragaliaEnemyActionTypes.HB1004.genesis:
            {
                float interval = ObjectExtensions.ParseInvariantFloat(_currentActionStage.args[1]);
                int hp = int.Parse(_currentActionStage.args[0]);
                var args = _currentActionStage.args;
                if (args.Length >= 6)
                {
                    List<float> triggerTimes = new List<float>();
                    for (int i = 5; i < args.Length; i++)
                    {
                        triggerTimes.Add(ObjectExtensions.ParseInvariantFloat(args[i]));
                    }

                    currentAction = StartCoroutine(ACT_GenesisRespendence(interval,
                        int.Parse(args[2]),
                        int.Parse(args[3]),
                        int.Parse(args[4]), triggerTimes.ToArray(),hp));
                }
                else
                {
                    currentAction = StartCoroutine(ACT_GenesisRespendence(interval,
                        0, 0, 0, new float[] { -1 },hp));
                }
                
                
                
                break;
            }

        }
        
    }

    protected IEnumerator ACT_WandingLight(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (transform.DistanceX(targetPlayer) > 35f ||
            transform.DistanceY(targetPlayer) > 12f)
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true,true));
            yield return new WaitUntil(() => currentAttackAction == null);
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action15());
        }
        else
        {
            currentMoveAction = StartCoroutine
            (enemyController.MoveToSameGround(
                targetPlayer,4,8));
            yield return new WaitUntil(() => currentMoveAction == null);
            enemyController.SetKBRes(999);
            if (TaskSuccess)
            {
                currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action15());
            }
            else
            {
                currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true));
            }
        }
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    protected IEnumerator ACT_ColorfulPillars(float interval)
    {
        ActionStart();
        breakable = false;

        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action16());
        
        yield return _attackIsNull;

        breakable = true;
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_SharedAttack(float waitTime, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        enemyAttackManagerL.HB04_Action17(waitTime);
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_HolyRing(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action22());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_DawnCirclet(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action21());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_SquaredAOE(float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action23());

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_RipplingLightPillar(float interval,bool avoidable)
    {
        ActionStart();
        breakable = false;
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action24(avoidable));

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);

        breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_GenesisRespendence(float interval, int celestial,
        int squared, int burst, float[] triggerTimes, int hp)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine
            (enemyAttackManagerL.HB04_Action25(hp,celestial==1, squared==1, 
                burst==1, triggerTimes));

        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        yield return new WaitForSeconds(interval);

        ActionEnd();
    }

    protected IEnumerator ACT_HolyHeal(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        
        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action26());
        yield return _attackIsNull;
        
        enemyController.SetKBRes(status.knockbackRes);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        
        ActionEnd();
        
    }
    protected IEnumerator ACT_SetWorld(int worldID, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt);

        enemyController.SetKBRes(999);
        if (worldID == 1)
        {
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action18());
        }
        else
        {
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action19());
        }

        yield return _attackIsNull;
        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);

        ActionEnd();
    }
    
    protected IEnumerator ACT_BlessedWallDoubleDirectionFixed(float distance, float center, float interval)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);
        

        enemyController.SetKBRes(999);
        currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action06_V2(center,distance));
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);


        ActionEnd();
    }
    
    protected IEnumerator ACT_PrayerProjectiles(float interval, int dir, int facedir = 0,
        int fixedPosition = 0, int avoidable = 0)
    {
        ActionStart();
        
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.grounded);

        if (fixedPosition == 0)
        {
            if (transform.DistanceX(targetPlayer) > 35f ||
                transform.DistanceY(targetPlayer) > 12f)
            {
                enemyController.SetKBRes(999);
                currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true,true));
                yield return new WaitUntil(() => currentAttackAction == null);
                currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action20(dir,facedir,avoidable));
            }
            else
            {
                currentMoveAction = StartCoroutine
                (enemyController.MoveToSameGround(
                    targetPlayer,4,14));
                yield return new WaitUntil(() => currentMoveAction == null);
                enemyController.SetKBRes(999);
                if (TaskSuccess)
                {
                    currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action20(dir,facedir,avoidable));
                }
                else
                {
                    currentAttackAction = StartCoroutine(enemyAttackManager.HB04_Action07(true));
                }
            }
        }
        else
        {
            enemyController.SetKBRes(999);
            currentAttackAction = StartCoroutine(enemyAttackManagerL.HB04_Action20(dir,facedir,avoidable));
        }

        
        yield return new WaitUntil(() => currentAttackAction == null);


        enemyController.SetKBRes(status.knockbackRes);

        yield return new WaitForSeconds(interval);
        ActionEnd();
    }


    protected IEnumerator ChangePhaseAnimationRoutine()
    {
        ActionStart();
        print("startPhaseChange");
        currentMoveAction = 
            StartCoroutine(enemyAttackManager.HB04_Action14());
        yield return new WaitUntil(()=>currentMoveAction == null);
        
        print("转P2");
        
        var p2_boss = Instantiate(p2_prefab,transform.position,Quaternion.identity,transform.parent);
        p2_boss.GetComponent<EnemyController>().TurnMove(targetPlayer);
        //UI_BossStatus.Instance.RedirectBoss(p2_boss);
        //BattleEffectManager.Instance.bgmVoiceSource.Stop();
        BattleStageManager.currentDisplayingBossInfo = 2;
        FindObjectOfType<UI_BossStatus>().RedirectBoss(p2_boss,1);
        p2_boss.GetComponent<StatusManager>()?.OnHPChange?.Invoke();
        BattleEffectManager.Instance.PlayBGM(false);
        ActionEnd();
        yield return null;
        // BattleStageManager.Instance.RemoveFieldAbility(20081);
        // BattleStageManager.Instance.RemoveFieldAbility(20091);
        BattleEffectManager.Instance.SetBGM(p2_bgm);
        BattleEffectManager.Instance.PlayBGM(true);
        BattleStageManager.Instance.RemoveFieldAbility(20151);

        Destroy(gameObject);
    }
    
    private void ToPhase2()
    {
        if(status.GetConditionStackNumber((int)BasicCalculation.BattleCondition.PowerOfBonds)>0 || status.currentHp > 0)
            return;
        
        
        status.OnHPBelow0 = null;
        enemyController.StopAllCoroutines();
        
        ResetHumanActionsBeforeTransform();
        
        currentAction = StartCoroutine(ChangePhaseAnimationRoutine());
        
    }
}
