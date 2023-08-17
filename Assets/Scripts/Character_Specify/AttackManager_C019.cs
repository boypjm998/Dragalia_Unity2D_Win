using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;

public class AttackManager_C019 : AttackManager
{
    protected ActorBase ac;
    private TargetAimer ta;
    StatusManager _statusManager;
    
    [SerializeField] private GameObject skill2Projectile; //y-0.85f
    [SerializeField] private GameObject skill2Projectile_sub;

    
    protected override void Awake()
    {
        base.Awake();
        RangedAttackFXLayer = GameObject.Find("AttackFXPlayer");
        ta = GetComponentInChildren<TargetAimer>();
        ac = GetComponent<ActorBase>();
        _statusManager = GetComponent<StatusManager>();
        
    }
    
    /// <summary>
    /// Ring of Affliction
    /// </summary>

    public void Skill2()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainer,transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        InstantiateBuff(skill2Projectile, new Vector2(transform.position.x, transform.position.y - 0.85f));
        var allies = GetAllObjects(GameObject.Find("Player").transform);
        foreach (var stat in allies)
        {
            if (stat.gameObject != gameObject)
            {
                Instantiate(skill2Projectile_sub, new Vector2(stat.transform.position.x, stat.transform.position.y - 1.85f),Quaternion.identity,stat.transform.Find("BuffLayer"));
            }
            stat.ObtainUnstackableTimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,-1,-1);
            stat.ObtainTimerBuff(new TimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,10,90,3,101901)
            ,false);
        }
    }

}
