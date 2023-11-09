using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class UI_HB002_Legend_01 : MonoBehaviour
{
    private StatusManager _statusManager;
    List<GameObject> effectObjects = new List<GameObject>();

    private void Awake()
    {
        _statusManager = transform.parent.parent.GetComponent<StatusManager>();
        _statusManager.OnBuffEventDelegate += CheckSpecialBuffStack;
        _statusManager.OnBuffExpiredEventDelegate += CheckSpecialBuffStack;
        for(int i = 0; i < 4; i++)
        {
            effectObjects.Add(transform.GetChild(i).gameObject);
        }
        
    }

    private void CheckSpecialBuffStack(BattleCondition condition)
    {
        
            if (_statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.ManaOverloaded) > 0)
            {
                effectObjects[3].SetActive(true);
                effectObjects[2].SetActive(false);
                effectObjects[1].SetActive(false);
                effectObjects[0].SetActive(false);
                return;
            }

            int stack = _statusManager.GetConditionWithSpecialID(8102401).Count;

            switch (stack)
            {
                case 0:
                {
                    effectObjects[3].SetActive(false);
                    effectObjects[2].SetActive(false);
                    effectObjects[1].SetActive(false);
                    effectObjects[0].SetActive(false);
                    break;
                }
                case 1:
                {
                    effectObjects[3].SetActive(false);
                    effectObjects[2].SetActive(false);
                    effectObjects[1].SetActive(false);
                    effectObjects[0].SetActive(true);
                    break;
                }
                case 2:
                {
                    effectObjects[3].SetActive(false);
                    effectObjects[2].SetActive(false);
                    effectObjects[1].SetActive(true);
                    effectObjects[0].SetActive(false);
                    break;
                }
                case 3:
                {
                    effectObjects[3].SetActive(false);
                    effectObjects[2].SetActive(true);
                    effectObjects[1].SetActive(false);
                    effectObjects[0].SetActive(false);
                    break;
                }
                default:
                {
                    effectObjects[3].SetActive(false);
                    effectObjects[2].SetActive(false);
                    effectObjects[1].SetActive(false);
                    effectObjects[0].SetActive(false);
                    break;
                }
            }



        
    }


}
