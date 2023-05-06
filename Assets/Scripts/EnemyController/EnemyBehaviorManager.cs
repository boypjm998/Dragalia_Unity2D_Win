using System;
using UnityEngine;
using GameMechanics;
using System.Collections;
using System.Collections.Generic;

public class EnemyBehaviorManager : DragaliaEnemyBehavior
{
    [Serializable]
    public class EnemyActionPattern
    {
        [Serializable]
        public class PhasePattern
        {
            [Serializable]
            public class PhaseAction
            {
                 public string action_name;
                 public string[] args; //等待时间等参数
                 public bool lockPhase = false;
                 public bool unbreakable = false;
                 public string jumpAction = "next";
            }
            public enum JumpOutCondition
            {
                None,
                HP,
                LoopCycle,
                Function
            }
            public List<PhaseAction> phase = new();
            public JumpOutCondition jumpOutCondition;
            public int loopStartPoint;
        }
        public List<PhasePattern> phasePattern = new();
    }
    
    protected override void CheckPhase()
    {
        
    }

    protected override void DoAction(int state, int substate)
    {
        
    }

    protected void GetBehavior(TextAsset behaviorTextAsset)
    {
        EnemyActionPattern enemyActionPattern = JsonUtility.FromJson<EnemyActionPattern>(behaviorTextAsset.text);
    }

}
