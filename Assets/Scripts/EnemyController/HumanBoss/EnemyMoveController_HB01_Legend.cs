using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveController_HB01_Legend : EnemyMoveController_HB01
{
    VoiceControllerEnemy voice;
    protected override void Start()
    {
        voice = GetComponentInChildren<VoiceControllerEnemy>();
        _stageManager = FindObjectOfType<BattleStageManager>();
        MeeleAttackFXLayer = transform.Find("MeeleAttackFX").gameObject;
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ac = GetComponent<EnemyControllerHumanoid>();
        anim = GetComponentInChildren<Animator>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        _effectManager = GameObject.Find("StageManager").GetComponent<BattleEffectManager>();
        _statusManager = GetComponent<StatusManager>();
        GetAllAnchors();
    }


    


}
