using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviorTextAssetGeneration : MonoBehaviour
{
    [SerializeField] private string _bossActionSetName;
    
    [SerializeField] private TextAsset _behaviorText;

    [SerializeField] private EnemyActionPatternSimple _pattern;

    private DragaliaEnemyActionTypes Types;
    
    [Serializable]
    public class EnemyActionPatternSimple
    {
        [Serializable]
        public class PhasePattern
        {
            [Serializable]
            public class PhaseAction
            {
                
                public string action_name;
                [NonSerialized] public int state = 0;
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
            public int loopStartPoint;
            [NonSerialized] public int loopCount = 0;
            public string[] args;
        }
        public List<PhasePattern> phasePattern = new();
    }

    private void Awake()
    {
        _pattern = JsonUtility.FromJson<EnemyActionPatternSimple>(_behaviorText.text);
    }
    
    private Type GetTypeOfBossBehaviorSet(string enumTypeStr)
    {
        var fieldInfo = typeof(DragaliaEnemyActionTypes).GetNestedType(enumTypeStr);

        if (fieldInfo == null) fieldInfo = typeof(DragaliaEnemyActionTypes).GetNestedType("Default");


        return fieldInfo;
    }
    
}
