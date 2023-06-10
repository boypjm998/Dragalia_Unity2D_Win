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
                public int state = 0;
                 public string[] args; //等待时间等参数
                 public bool lock_phase = false;
                 public bool unbreakable = false;
                 public string jump_action = "next";
                 public string[] jump_args;
                 
            }
            public enum JumpOutCondition
            {
                None,
                HP,
                LoopCycle,
                Function
            }
            public List<PhaseAction> action_list = new();
            public JumpOutCondition jumpOutCondition;
            public string jump_condition;
            public int loopStartPoint;
            public int loopCount = 0;
            public string[] args;
        }
        public List<PhasePattern> phasePattern = new();
    }

    protected EnemyActionPattern _pattern;
    protected EnemyActionPattern.PhasePattern.PhaseAction _currentActionStage;
    protected EnemyActionPattern.PhasePattern _currentPhase;
    
    [SerializeField] TextAsset behaviorTextAsset;
    
    protected override void CheckPhase()
    {
        var phase = _currentPhase;
        
        if(_currentActionStage.lock_phase)
            return;
        
        if (phase.jumpOutCondition == EnemyActionPattern.PhasePattern.JumpOutCondition.HP)
        {
            var cond = float.Parse(phase.args[0]) / 100f;
            print(phase.loopCount);
            if (phase.args.Length == 2)
            {
                print("len=2");
                if(substate < int.Parse(phase.args[1]) && phase.loopCount < 1)
                    return;
            }
            if ((float)status.currentHp / status.maxHP < cond)
            {
                
                substate = 0;
                state++;
                _currentPhase = _pattern.phasePattern[state];
            }
        }else if (phase.jumpOutCondition == EnemyActionPattern.PhasePattern.JumpOutCondition.None)
        {
            if (substate >= _currentPhase.action_list.Count)
            {
                if (state < _pattern.phasePattern.Count - 1)
                {
                    state++;
                    _currentPhase = _pattern.phasePattern[state];
                    if (_currentPhase.loopCount == 0)
                    {
                        substate = 0;
                    }
                    else
                    {
                        substate = _currentPhase.loopStartPoint;
                    }

                }

            }
        }else if (phase.jumpOutCondition == EnemyActionPattern.PhasePattern.JumpOutCondition.LoopCycle)
        {
            var cond = int.Parse(phase.args[0]);
            if (_currentPhase.loopCount >= cond)
            {
                substate = 0;
                state++;
                _currentPhase = _pattern.phasePattern[state];
            }
        }
    }

    protected override void DoAction(int state, int substate)
    {
        var action = _pattern.phasePattern[state].action_list[substate];

        var action_name = action.action_name;
        
    }

    protected void GetBehavior()
    {
        _pattern = JsonUtility.FromJson<EnemyActionPattern>(behaviorTextAsset.text);
        foreach (var phase in _pattern.phasePattern)
        {
            if (phase.jump_condition == "none")
            {
                phase.jumpOutCondition = EnemyActionPattern.PhasePattern.JumpOutCondition.None;
            }

            if (phase.jump_condition == "hp")
            {
                phase.jumpOutCondition = EnemyActionPattern.PhasePattern.JumpOutCondition.HP;
            }
            
            if(phase.jump_condition == "function")
            {
                phase.jumpOutCondition = EnemyActionPattern.PhasePattern.JumpOutCondition.Function;
            }
            
            if(phase.jump_condition == "loop")
            {
                phase.jumpOutCondition = EnemyActionPattern.PhasePattern.JumpOutCondition.LoopCycle;
            }
        }
        _currentPhase = _pattern.phasePattern[0];
        _currentActionStage = _currentPhase.action_list[0];
    }

    public override void ActionEnd(bool substateIncrement = true)
    {
        print(_currentActionStage.jump_action);
        if (_currentActionStage.jump_action == "next")
        {
            base.ActionEnd();
        }
        else if (_currentActionStage.jump_action == "loop")
        {
            base.ActionEnd(false);
            this.substate = _currentPhase.loopStartPoint;
            _currentPhase.loopCount++;
        }else if (_currentActionStage.jump_action == "conditional")
        {
            base.ActionEnd(false);
            var cond_args = _currentActionStage.jump_args;
            print("IS CONDITIONAL");
            int dest;
            CheckCondition(cond_args, out dest);
            this.substate = dest;
        }else if (_currentActionStage.jump_action == "to")
        {
            base.ActionEnd(false);
            var jmp_args = _currentActionStage.jump_args;
            this.substate = int.Parse(jmp_args[0]);
            if (jmp_args.Length > 1)
                _currentPhase.loopCount++;
        }else if (_currentActionStage.jump_action == "random")
        {
            base.ActionEnd(false);
            var jmp_args = _currentActionStage.jump_args;
            var len = jmp_args.Length;
            var rand = UnityEngine.Random.Range(0, len);
            this.substate = int.Parse(jmp_args[rand]);
        }
        else
        {
            base.ActionEnd();
        }
        

    }
    
    protected virtual bool CheckCondition(string[] args,out int dest_state)
    {
        dest_state = substate+1> _currentPhase.action_list.Count-1?0:substate+1;
        return false;
    }
    
}
