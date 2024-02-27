using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityNavMeshAgent;
using UnityEngine;

public class AnimationEventSender_Enemy : MonoBehaviour
{
    protected EnemyController _enemyController;
    //protected PlayerInput _playerInput;
    protected EnemyMoveManager _moveManager;
    protected SkinnedMeshRenderer _model;
    // Start is called before the first frame update
    private Coroutine breakShineRoutine;

    private void Awake()
    {
        _model = transform.Find("model/mBodyAll").GetComponent<SkinnedMeshRenderer>();
        _enemyController = transform.parent.parent.GetComponent<EnemyController>();
        _moveManager = transform.parent.parent.GetComponent<EnemyMoveManager>();
    }

    private void Start()
    {
        _model = transform.Find("model/mBodyAll").GetComponent<SkinnedMeshRenderer>();
        _enemyController = transform.parent.parent.GetComponent<EnemyController>();
        _moveManager = transform.parent.parent.GetComponent<EnemyMoveManager>();
    }


    // protected void OnAttackEnter()
    // {
    //     _enemyController.OnAttackEnter();
    // }
    // protected void OnAttackExit()
    // {
    //     _enemyController.OnAttackExit();
    // }
    // protected void OnHurtEnter()
    // {
    //     _enemyController.OnHurtEnter();
    // }
    // protected void OnHurtExit()
    // {
    //     _enemyController.OnHurtExit();
    // }

    // protected void TurnToMiddle()
    // {
    //     //transform.rotation = Quaternion.Euler(0, 120, 0);
    //     if(_enemyController.facedir ==1)
    //         transform.rotation = Quaternion.Euler(0, 120, 0);
    //     else
    //     {
    //         transform.rotation = Quaternion.Euler(0, -120, 0);
    //     }
    // }
    //
    // protected void TurnToSide()
    // {
    //     //transform.rotation = Quaternion.Euler(0, 102, 0);
    //     if(_enemyController.facedir ==1)
    //         transform.rotation = Quaternion.Euler(0, 102, 0);
    //     else
    //     {
    //         transform.rotation = Quaternion.Euler(0, -102, 0);
    //     }
    // }
    
    public void ChangeFaceExpression(float offset = 0)
    {
        //_model.materials[1].SetTextureOffset("_SurfaceInput", new Vector2(offset, 0));
        //_model.materials[2].SetTextureOffset("_SurfaceInput", new Vector2(offset, 0));
        _model.materials[1].mainTextureOffset = new Vector2(offset, 0);
        _model.materials[2].mainTextureOffset = new Vector2(offset, 0);
        
    }

    protected IEnumerator BreakShine(float maxTime = 10)
    {
        var modelShine = transform.Find("model/Break");
        while (maxTime > 0)
        {
            modelShine.gameObject.SetActive(true);
            yield return new WaitForSeconds(1f);
            modelShine.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
            maxTime -= 2;
        }
        modelShine.gameObject.SetActive(false);
    }

    public void OnBreakEnter()
    {
        if (breakShineRoutine != null)
        {
            StopCoroutine(breakShineRoutine);
        }
        breakShineRoutine = StartCoroutine(BreakShine());
    }
    
    public void OnBreakExit()
    {
        if (breakShineRoutine != null)
        {
            StopCoroutine(breakShineRoutine);
        }
    
        breakShineRoutine = null;
        var modelShine = transform.Find("model/Break");
        modelShine.gameObject.SetActive(false);
    }

}
