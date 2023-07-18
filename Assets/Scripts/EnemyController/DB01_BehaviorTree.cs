using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class DB01_BehaviorTree : EnemyBehaviorManager
{
    protected EnemyControllerFlyingHigh enemyController;
    protected EnemyMoveController_DB01 enemyAttackManager;

    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerFlyingHigh>();
        enemyAttackManager = GetComponent<EnemyMoveController_DB01>();
        GetBehavior();
    }


    

    // protected override void DoAction(int state, int substate)
    // {
    //     if (playerAlive == false)
    //         return;
    //
    //     if (state == 0)
    //     {
    //         switch (substate)
    //         {
    //             case 0:
    //                 currentAction = StartCoroutine(ACT_CycloneCrusher(3f));
    //                 break;
    //             case 1:
    //                 currentAction = StartCoroutine(ACT_StormStrike(4f));
    //                 break;
    //             case 2:
    //                 currentAction = StartCoroutine(ACT_Combo(3f));
    //                 break;
    //             case 3:
    //                 currentAction = StartCoroutine(ACT_Roar(1f));
    //                 break;
    //             case 4:
    //                 currentAction = StartCoroutine(ACT_CycloneCrusher(2f));
    //                 break;
    //             case 5:
    //                 currentAction = StartCoroutine(ACT_RendingBlasts(2f));
    //                 break;
    //             case 6:
    //                 currentAction = StartCoroutine(ACT_StormWallFixed(5f));
    //                 break;
    //             case 7:
    //                 currentAction = StartCoroutine(ACT_BounceWind(5f));
    //                 break;
    //             case 8:
    //                 currentAction = StartCoroutine(ACT_AroundAttack(3f));
    //                 break;
    //             case 9:
    //                 currentAction = StartCoroutine(ACT_ForwardWind(3f));
    //                 break;
    //             case 10:
    //                 currentAction = StartCoroutine(ACT_SummonHelp(2f));
    //                 break;
    //             case 11:
    //                 currentAction = StartCoroutine(ACT_Roar(2f));
    //                 break;
    //             case 12:
    //                 currentAction = StartCoroutine(ACT_RendingBlasts(2f));
    //                 break;
    //             case 13:
    //                 currentAction = StartCoroutine(ACT_Combo(3f));
    //                 break;
    //             case 14:
    //                 currentAction = StartCoroutine(ACT_BounceWind(3f));
    //                 break;
    //             case 15:
    //                 currentAction = StartCoroutine(ACT_StormWallChasing(3f));
    //                 break;
    //             case 16:
    //                 currentAction = StartCoroutine(ACT_StormRush(3f));
    //                 break;
    //             case 17:
    //                 currentAction = StartCoroutine(ACT_Roar(3f));
    //                 break;
    //             
    //             case 18:
    //                 this.substate = 0;
    //                 break;
    //         }
    //     }
    //
    //     else if (state == 1)
    //     {
    //         switch (substate)
    //         {
    //             case 0:
    //                 currentAction = StartCoroutine(ACT_GaleBlasts(0.25f));
    //                 break;
    //             case 1:
    //                 currentAction = StartCoroutine(ACT_StormStrike(6f));
    //                 break;
    //             case 2:
    //                 currentAction = StartCoroutine(ACT_StormWallFixed(4));
    //                 break;
    //             case 3:
    //                 currentAction = StartCoroutine(ACT_Combo(3));
    //                 break;
    //             case 4:
    //                 currentAction = StartCoroutine(ACT_CycloneCrusher(1f));
    //                 break;
    //             case 5:
    //                 currentAction = StartCoroutine(ACT_Roar(1));
    //                 break;
    //             case 6:
    //                 currentAction = StartCoroutine(ACT_RendingBlasts(3f));
    //                 break;
    //             case 7:
    //                 currentAction = StartCoroutine(ACT_AroundAttack(3f));
    //                 break;
    //             case 8:
    //                 currentAction = StartCoroutine(ACT_StormWallChasing(3f));
    //                 break;
    //             case 9:
    //                 currentAction = StartCoroutine(ACT_BounceWind(3f));
    //                 break;
    //             case 10:
    //                 currentAction = StartCoroutine(ACT_Combo(3f));
    //                 break;
    //             case 11:
    //                 currentAction = StartCoroutine(ACT_ForwardWind(3f));
    //                 break;
    //             case 12:
    //                 currentAction = StartCoroutine(ACT_Roar(1f));
    //                 break;
    //             case 13:
    //                 this.substate = 0;
    //                 break;
    //             
    //         }
    //     }
    //
    //
    //
    //
    //
    //
    //
    //
    //     
    //
    // }

    protected override void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        
        _currentPhase = _pattern.phasePattern[state];
        _currentActionStage = _currentPhase.action_list[substate];

        var action_name = _currentActionStage.action_name;

        if (action_name == "null")
        {
            ActionEnd();
            return;
        }

        switch (action_name)
        {
            case "forward":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_ForwardWind(interval));
                break;
            }
            case "combo":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Combo(interval));
                break;
            }
            case "around":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_AroundAttack(interval));
                break;
            }
            case "roar":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_Roar(interval));
                break;
            }
            case "tornado":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GaleBlasts(interval));
                break;
            }
            case "wallFixed":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_StormWallFixed(interval));
                break;
            }
            case "wallChasing":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_StormWallChasing(interval));
                break;
            }
            case "bounce":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_BounceWind(interval));
                break;
            }
            case "lock":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_StormStrike(interval));
                break;
            }
            case "pillar":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_RendingBlasts(interval));
                break;
            }
            case "multi":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_CycloneCrusher(interval));
                break;
            }
            case "golem":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonHelp(interval));
                break;
            }
            case "launcher":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_StormRush(interval));
                break;
            }
            case "upward":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_UpwardWind(interval));
                break;
            }
            case "ground":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SurgingTempestGround(interval));
                break;
            }
            case "sky":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SurgingTempestSky(interval));
                break;
            }
            case "leif":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonLeif(interval));
                break;
            }
            case "meene":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonMeene(interval));
                break;
            }
            case "lathna":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonLathna(interval));
                break;
            }
            case "tobias":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonTobias(interval));
                break;
            }
            case "melsa":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_SummonMelsa(interval));
                break;
            }
            case "gale":
            {
                float interval = float.Parse(_currentActionStage.args[0]);
                currentAction = StartCoroutine(ACT_GaleCatastrophe(interval));
                break;
            }
            default: break;

        }
        
        
        
    }
    
    

    public IEnumerator ACT_ApproachTarget(float arriveDistanceX,
        float allowDistanceY, float interval = 0)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        
        //if issue occurs, may allowDistanceY should be a negative value
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            arriveDistanceX, allowDistanceY, 5));
        yield return currentMoveAction;

        yield return new WaitForSeconds(interval);
        ActionEnd();

    }

    public IEnumerator ACT_ForwardWind(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            15, -1, 3));
        yield return currentMoveAction;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action01());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_Combo(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            6, -1, 3));
        yield return currentMoveAction;
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action02());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_AroundAttack(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            3, -1, 3));
        yield return currentMoveAction;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action03());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_CycloneCrusher(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        float posY;

        if (transform.position.y > targetPlayer.transform.position.y + 5)
        {
            posY = transform.position.y;
        }
        else
        {
            posY = targetPlayer.transform.position.y + 5;
        }

        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(transform.position.x, posY),
                1,Ease.InOutSine));
        yield return currentMoveAction;
        enemyController.SetGravityScale(0);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action05());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            999, -1, 2));
        yield return currentMoveAction;
        enemyController.ResetGravityScale();
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_Roar(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action04());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_StormWallFixed(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, transform.position.y),
                1,Ease.InOutSine));
        yield return currentMoveAction;
        yield return new WaitUntil(() => !enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action06(0));
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_StormWallChasing(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action06(1));
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_BounceWind(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            8, -1, 3));
        yield return currentMoveAction;

        yield return new WaitForSeconds(0.5f);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action07());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_StormStrike(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action08());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_RendingBlasts(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(enemyController.FlyTowardTargetOnSamePlatform(targetPlayer,
            5, -1, 3));
        yield return currentMoveAction;

        yield return new WaitForSeconds(0.5f);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action09());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_GaleBlasts(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action10());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_SummonHelp(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 4f),
                1,Ease.InOutSine));
        yield return currentMoveAction;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action11());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_StormRush(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 13.5f),
                1,Ease.InOutSine));
        yield return currentMoveAction;

        yield return new WaitForSeconds(0.5f);
        
        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action12());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
        
    }

    public IEnumerator ACT_UpwardWind(float interval)
    {
        ActionStart();
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, -2f),
                1,Ease.InOutSine));
        yield return currentMoveAction;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action13());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_SurgingTempestGround(float interval)
    {
        breakable = false;
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, -2f),
                1,Ease.InOutSine));
        yield return currentMoveAction;

        //yield return new WaitUntil(() => !enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action14());
        yield return new WaitUntil(()=>currentAttackAction == null);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_SummonLeif(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action15());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

    public IEnumerator ACT_SummonMeene(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action16());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_SummonTobias(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action17());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_SummonLathna(float interval)
    {
        ActionStart(); 
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action18());
        yield return new WaitUntil(()=>currentAttackAction == null);
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_SummonMelsa(float interval)
    {
        ActionStart();
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action19());
        yield return new WaitUntil(()=>currentAttackAction == null);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_SurgingTempestSky(float interval)
    {
        ActionStart(); 
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);
        
        currentMoveAction = StartCoroutine(
            enemyController.FlyToPoint(new Vector2(0, 13.5f),
                1,Ease.InOutSine));
        yield return currentMoveAction;

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action20());
        yield return new WaitUntil(()=>currentAttackAction == null);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }
    
    public IEnumerator ACT_GaleCatastrophe(float interval)
    {
        ActionStart(); 
        breakable = false;
        yield return new WaitUntil(() => !enemyController.hurt && enemyController.flyingGrounded);

        currentAttackAction = StartCoroutine(enemyAttackManager.DB01_Action21());
        yield return new WaitUntil(()=>currentAttackAction == null);
        breakable = true;
        
        yield return new WaitForSeconds(interval);
        ActionEnd();
    }

}
