using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEventSender_Enemy : MonoBehaviour
{
    protected EnemyController _enemyController;
    //protected PlayerInput _playerInput;
    protected EnemyMoveManager _moveManager;
    protected SkinnedMeshRenderer _model;
    // Start is called before the first frame update

    private void Start()
    {
        _model = transform.Find("model/mBodyAll").GetComponent<SkinnedMeshRenderer>();
        _enemyController = transform.parent.parent.GetComponent<EnemyController>();
        _moveManager = transform.parent.parent.GetComponent<EnemyMoveManager>();
    }


    protected void OnAttackEnter()
    {
        _enemyController.OnAttackEnter();
    }
    protected void OnAttackExit()
    {
        _enemyController.OnAttackExit();
    }
    protected void OnHurtEnter()
    {
        _enemyController.OnHurtEnter();
    }
    protected void OnHurtExit()
    {
        _enemyController.OnHurtExit();
    }

    protected void TurnToMiddle()
    {
        //transform.rotation = Quaternion.Euler(0, 120, 0);
        if(_enemyController.facedir ==1)
            transform.rotation = Quaternion.Euler(0, 120, 0);
        else
        {
            transform.rotation = Quaternion.Euler(0, -120, 0);
        }
    }

    protected void TurnToSide()
    {
        //transform.rotation = Quaternion.Euler(0, 102, 0);
        if(_enemyController.facedir ==1)
            transform.rotation = Quaternion.Euler(0, 102, 0);
        else
        {
            transform.rotation = Quaternion.Euler(0, -102, 0);
        }
    }
    
    public void ChangeFaceExpression(float offset = 0)
    {
        //_model.materials[1].SetTextureOffset("_SurfaceInput", new Vector2(offset, 0));
        //_model.materials[2].SetTextureOffset("_SurfaceInput", new Vector2(offset, 0));
        _model.materials[1].mainTextureOffset = new Vector2(offset, 0);
        _model.materials[2].mainTextureOffset = new Vector2(offset, 0);
    }

}
