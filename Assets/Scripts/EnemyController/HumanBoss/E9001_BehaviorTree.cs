using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class E9001_BehaviorTree : DragaliaEnemyBehavior
{
    private EnemyControllerHumanoid enemyController;
    private EnemyMoveController_E9001 attackController;

    protected int baseAttackInStat;
    protected override void CheckPhase()
    {
        
    }

    
    

    protected override void DoAction(int state, int substate)
    {
        if(!playerAlive)
            return;
        
        
        if (substate == 0)
        {
            var hpRegenBuff = new TimerBuff((int)BasicCalculation.BattleCondition.HealOverTime,
                12, -1, 100, 0);
            hpRegenBuff.dispellable = false;
            status.ObtainTimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,-1,-1,1,-1);
            status.ObtainTimerBuff(hpRegenBuff);
            currentAction = StartCoroutine(ACT_ReadInformation());
            
            
            
            this.substate = 1;
        }
    }

    // Update is called once per frame
    protected override void Awake()
    {
        base.Awake();
        enemyController = GetComponent<EnemyControllerHumanoid>();
        attackController = GetComponent<EnemyMoveController_E9001>();
        status.OnHPBelow0 += CheckPowerOfBonds;
        baseAttackInStat = status.baseAtk;
        if (true)
        {
            BattleSceneUIManager.Instance.ReplacePauseMenu(attackController.InstantiateNewPrefabMenu());
        }

    }

    protected override void Update()
    {
        //status.baseAtk和status.currentHP/MaxHP的比值有关，比值越小，攻击越高，最大为10倍
        var hpRatio = (float)status.currentHp / status.maxHP;
        var atkRatio = 1 + (1 - hpRatio) * 99;
        //print(atkRatio);
        status.baseAtk = (int) (baseAttackInStat * atkRatio);
        
        
    }
    
    void CheckPowerOfBonds()
    {
       if(enemyController.CheckPowerOfBonds())
       {
           status.ObtainTimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,-1,-1,1,-1);
       }


       
       
       
       
    }

    IEnumerator ACT_ReadInformation()
    {
        ActionStart();
        
        currentMoveAction = StartCoroutine(attackController.ReadInformation());

        yield return currentMoveAction;
        
        ActionEnd();
    }



}
