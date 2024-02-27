using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 敌人触发器，用于激活敌人的行为
/// </summary>
public class EnemyAwakeTrigger : MonoBehaviour
{
    private GameObject triggerGO;
    [SerializeField] private BoxCollider2D triggerCollider;
    private DragaliaEnemyBehavior _behavior;
    private ActorBase _actorBase;
    private Action onTriggeredAction;
    [SerializeField] private Vector2 triggerSize;
    [SerializeField] private bool actionIsAwake = true;

    private void Awake()
    {
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        _behavior.playerAlive = false;
        _actorBase = GetComponent<ActorBase>();
        
        
    }

    private void OnDestroy()
    {
        onTriggeredAction = null;
        _actorBase.OnAttackInterrupt -= SetAwake;
        
    }

    private void Start()
    {
        if (triggerGO == null && triggerCollider == null)
        {
            triggerGO = Instantiate(new GameObject("Trigger"), transform);
        }

        if (triggerCollider == null)
        {
            triggerCollider = triggerGO.AddComponent<BoxCollider2D>();
            triggerCollider.isTrigger = true;
            triggerCollider.size = triggerSize;
            triggerGO.layer = LayerMask.NameToLayer("AttackEnemy");
        }

        if (actionIsAwake)
        {
            SetTriggerAction(SetAwake);
            _actorBase.OnAttackInterrupt += SetAwake;
        }
    }

    private void SetAwake()
    {
        BattleEffectManager.Instance.SpawnExclamation(gameObject,
            transform.position + new Vector3(0,2));
        _actorBase.OnAttackInterrupt -= SetAwake;
        //triggerCollider.enabled = false;
        _behavior.playerAlive = true;
    }

    public void SetTriggerCollider(Vector2 size)
    {
        triggerSize = size;
    }


    public void SetTriggerAction(Action action)
    {
        onTriggeredAction += action;
    }

    public void DisableCollider()
    {
        triggerCollider.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") == false)
            return;
        

        _behavior.playerAlive = true;
        
        if(onTriggeredAction != null)
            onTriggeredAction?.Invoke();
        
        triggerCollider.enabled = false;
        
        _actorBase.OnAttackInterrupt -= SetAwake;
        
    }
}
