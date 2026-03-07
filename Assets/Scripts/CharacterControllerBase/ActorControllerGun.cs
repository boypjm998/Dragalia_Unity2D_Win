using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class ActorControllerGun : ActorController
{
    protected override void Awake()
    {
        base.Awake();
        voiceController = GetComponentInChildren<VoiceControllerPlayer>();
    }

    protected override void Update()
    {
        base.Update();
        CheckSkill();
    }


    public override void FaceDirectionAutoFix(int moveID)
    {
        switch (moveID)
        {
            case 1:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 18f, 1f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 18f, 1f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }

                break;
            }
            case 2:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 10f, 3f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 10f, 3f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }
                break;
            }

        }

    }

    public void ComboBackwardStep()
    {
        var endPos = transform.position.x;
        

        Collider2D currentPlatform = GetComponent<PlayerOnewayPlatformEffector>().
            GetCurrentAttachedGroundCol();

        if (currentPlatform == null)
        {
            currentPlatform = gameObject.RaycastedPlatform();
        }

        if (facedir == 1 && endPos - 0.5f > currentPlatform.bounds.min.x)
        {
            endPos = Mathf.Clamp(endPos, currentPlatform.bounds.min.x, endPos - 0.5f);
        }else if (facedir == -1 && endPos + 0.5f < currentPlatform.bounds.max.x)
        {
            endPos = Mathf.Clamp(endPos,endPos + 0.5f, currentPlatform.bounds.max.x);
        }
        
        endPos = Mathf.Clamp(endPos,BattleStageManager.Instance.mapBorderL,  
            BattleStageManager.Instance.mapBorderR);

        _tweener = rigid.DOMoveX(endPos, 0.2f).SetEase(Ease.OutSine);

    }
    
    public override void OnStandardAttackEnter()
    {
        base.OnStandardAttackEnter();
        ActionDisable((int)PlayerActionType.MOVE);//move
        ActionDisable((int)PlayerActionType.JUMP);//jump
        pi.SetInputDisabled("move");
        pi.LockDirection(0);
        StartAttack();
        voiceController.PlayAttackVoice(1);

        _tweener?.Kill();

        
    }
    public override void OnStandardAttackExit()
    {
        base.OnStandardAttackExit();
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.MOVE);
        ExitAttack();
    }
    
    public override void OnSkillEnter()
    {
        base.OnSkillEnter();
        pi.inputAttackEnabled = true;
        OnAttackInterrupt?.Invoke();

    }

    public override void OnSkillExit()
    {
        base.OnSkillExit();
        ResetGravityScale();
        //SetWeaponVisibility(true);
    }

    public override void onRollEnter()
    {
        base.onRollEnter();
        _tweener?.Kill();
        pi.LockDirection(0);
    }
    
    public override void onRollExit()
    {
        base.onRollExit();
        // pi.SetInputRoll(1);
        // pi.SetInputAttack(1);
    }
}
