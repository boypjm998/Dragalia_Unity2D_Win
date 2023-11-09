using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class HB01_BehaviorTree_02 : HB01_BehaviorTree
{
    public bool debug;
    protected override void CheckPhase()
    {
        if (status.currentHp < status.maxHP * 0.7 && state==0)
        {
            state = 1;
            substate = 0;
        }
        if (status.currentHp < status.maxHP * 0.4 && state==1)
        {
            state = 2;
            substate = 0;
        }
    }

    protected override void ExcutePhase()
    {
        if (!isAction)
        {
            DoAction(state, substate);
        }
    }

    

    private void DoAction(int state, int substate)
    {
        if (playerAlive == false)
            return;
        if (state == 0)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_ComboA(3));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_ComboB(3));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_ComboC(3));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_ComboD(3));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_SingleDodgeCombo(2));
                    break;
                case 5:
                    this.substate = 0;
                    break;
                
            }
        }
        else if (state == 1)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_SingleDodgeCombo(2));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_CarmineRush(2));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 3:
                    currentAction = StartCoroutine(ACT_ComboB(2));
                    break;
                case 4:
                    currentAction = StartCoroutine(ACT_ComboC(2));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_ComboD(2));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_FlameRaid(2));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_ComboB(2));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_ComboC(2));
                    break;
                case 10:
                    currentAction = StartCoroutine(ACT_ComboD(2));
                    break;
                case 11:
                    this.substate = 0;
                    break;
                
            }
        }
        else if (state == 2)
        {
            switch (substate)
            {
                case 0:
                    currentAction = StartCoroutine(ACT_BlazingEnhancement_2(2));
                    break;
                case 1:
                    currentAction = StartCoroutine(ACT_CarmineRush(0));
                    break;
                case 2:
                    currentAction = StartCoroutine(ACT_SavageFlameRaid(0,0));
                    break;
                case 3:
                {
                    var checkBuff =
                        status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush);
                    if (checkBuff.Count > 0)
                    {
                        status.RemoveConditionWithLog(checkBuff[0]);
                        currentAction = StartCoroutine(ACT_BrightCarmineRush(2.5f,-1));
                    }
                    else
                    {
                        this.substate++;
                    }

                    break;
                }
                case 4:
                    currentAction = StartCoroutine(ACT_ComboA(2));
                    break;
                case 5:
                    currentAction = StartCoroutine(ACT_ComboB(2));
                    break;
                case 6:
                    currentAction = StartCoroutine(ACT_ComboC(2));
                    break;
                case 7:
                    currentAction = StartCoroutine(ACT_ComboD(2));
                    break;
                case 8:
                    currentAction = StartCoroutine(ACT_FlameRaid(0));
                    break;
                case 9:
                    currentAction = StartCoroutine(ACT_BrightCarmineRush(0,0));
                    break;
                case 10:
                {
                    var checkBuff =
                        status.GetConditionsOfType((int)BasicCalculation.BattleCondition.BlazewolfsRush);
                    if (checkBuff.Count>0)
                    {
                        status.RemoveConditionWithLog(checkBuff[0]);
                        currentAction = StartCoroutine(ACT_SavageFlameRaid(1,-1));
                    }
                    else
                    {
                        this.substate++;
                    }
                    break;
                }
                case 11:
                    this.substate = 0;
                    break;
                
            }
        }
    }

}
