using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class ActorController_c005 : ActorControllerDagger
{
    public bool dodgeAttackUsed = false;
    public IGroundSensable platformSensor;

    protected Collider2D lastPlatformBeforeCombo4;
    public Vector2 lastPositionAfterCombo6 = Vector2.zero;

    public GameObject warpFX;

    private TimerBuff buffWhileSkill;
    
    public bool skillBoosted = false;


    protected override void CheckSkill()
    {
        if (pi.skill[0] && !pi.hurt && !pi.isSkill && !skillBoosted)
        {
            if(anim.GetBool("isGround"))
                UseSkill(1);
            else if(anim.GetCurrentAnimatorStateInfo(0).IsName("combo4") ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("combo7"))
            {
                QuickLanding(false);
                UseSkill(1);
            }
        }

        if (pi.skill[1] && !pi.hurt && !pi.isSkill && !skillBoosted)
        {
            if(anim.GetBool("isGround"))
                UseSkill(2);
            else if(anim.GetCurrentAnimatorStateInfo(0).IsName("combo4") ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("combo7"))
            {
                QuickLanding(false);
                UseSkill(2);
            }
        }
          
        if (pi.skill[2] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(3);
        }

        if (pi.skill[3] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
        
        if (pi.skill[0] && !pi.hurt && !pi.isSkill && skillBoosted)
        {
            if(anim.GetBool("isGround"))
                UseSkill(5);
            else if(anim.GetCurrentAnimatorStateInfo(0).IsName("combo4") ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("combo7"))
            {
                QuickLanding(false);
                UseSkill(5);
            }
        }

        if (pi.skill[1] && !pi.hurt && !pi.isSkill && skillBoosted)
        {
            if(anim.GetBool("isGround"))
                UseSkill(6);
            else if(anim.GetCurrentAnimatorStateInfo(0).IsName("combo4") ||
                    anim.GetCurrentAnimatorStateInfo(0).IsName("combo7"))
            {
                QuickLanding(false);
                UseSkill(6);
            }
        }

    }

    public override void UseSkill(int id)
    {
        
        voiceController.PlaySkillVoice(id);

        if (isAttackSkill[id - 1])
        {
            pi.InvokeAttackSignal();
            AttackFromPlayer.CheckEnergyLevel(_statusManager);
            AttackFromPlayer.CheckInspirationLevel(_statusManager);
        }else if (isRecoverSkill[id - 1])
        {
            AttackFromPlayer.CheckEnergyLevel(_statusManager);
        }


        switch (id)
        {
            case 1:
                pi.isSkill = true;
                anim.Play("s1");
                _statusManager.currentSP[0] = 0;
                break;

            case 2:
                pi.isSkill = true;
                anim.Play("s2");
                _statusManager.currentSP[1] = 0;
                break;

            case 3:
                pi.isSkill = true;
                anim.Play("s3");
                _statusManager.currentSP[2] = 0;
                break;

            case 4:
                pi.isSkill = true;
                anim.Play("s4");
                _statusManager.currentSP[3] = 0;
                break;

            case 5:
                pi.isSkill = true;
                anim.Play("s1_boost");
                _statusManager.currentSP[0] = 0;
                break;

            case 6:
                pi.isSkill = true;
                anim.Play("s2_boost");
                _statusManager.currentSP[1] = 0;
                break;
            

            default:
                break;
        }
    }

    protected override void Awake()
    {
        base.Awake();
        platformSensor = GetComponentInChildren<IGroundSensable>();
        buffWhileSkill = new TimerBuff
            ((int)BasicCalculation.BattleCondition.DamageCut, 50, -1, 1,
            100501);
        buffWhileSkill.dispellable = false;
        
    }

    bool IsComboState(AnimatorStateInfo stateInfo)
    {
        if (stateInfo.IsName("combo1"))
            return true;
        if (stateInfo.IsName("combo2"))
            return true;
        if (stateInfo.IsName("combo3"))
            return true;
        if (stateInfo.IsName("combo4"))
            return true;
        if (stateInfo.IsName("combo5"))
            return true;
        if (stateInfo.IsName("combo6"))
            return true;
        if (stateInfo.IsName("combo7"))
            return true;
        

        return false;
    }

    protected override void Update()
    {
        if(DModeIsOn)
            return;
        
        
        if (rigid.transform.localScale.x == 1)
        {
            facedir = 1;
        }
        else if (rigid.transform.localScale.x == -1)
        {
            facedir = -1;
        }


        if(pi.enabled)
            anim.SetFloat("forward", Mathf.Abs(pi.DRight));
        else
        {
            //anim.SetFloat("forward", 0f);
        }

        if (pi.hurt)
        {
            anim.SetBool("hurt", true);
        }
        else
        {
            anim.SetBool("hurt", false);
        }


        if (pi.jump && pi.jumpEnabled)
        {
            Jump();
        }

        if (pi.wjump && pi.jumpEnabled)
        {
            DoubleJump();
        }

        if (pi.stdAtk && pi.attackEnabled)
        {
            if (anim.GetBool("isGround") == true)
                StdAtk();
            else
            {
                AirDashAtk();
            }
        }


        if (pi.roll)
        {
            Roll();
        }
        
        CheckAccelerateLanding();
        CheckSkill();

        

    }

    public void CheckAccelerateLanding()
    {
        if(pi.isSkill)
            return;
        
        
        if (pi.buttonDown.IsPressing && grounded == false && pi.hurt == false && rigid.velocity.y <= -1)
        {
            if (platformSensor.GetCurrentAttachedGroundCol() != null)
            {
                rigid.velocity = new Vector2(rigid.velocity.x, 0);
                pi.quicklandingEnabled = true;
                return;
            }
            
            if(rigid.velocity.y > -30 && pi.quicklandingEnabled)
                rigid.velocity = new Vector2(rigid.velocity.x, rigid.velocity.y - 0.5f);

        }
    }

    public override void Roll()
    {
        if (pi.inputRollEnabled == false)
        {
            pi.roll = false;
            return;
        }


        if (pi.rollEnabled && !pi.hurt)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            
            
            
            if ((IsComboState(stateInfo) || Combo > 0) && dodgeAttackUsed == false && pi.inputAttackEnabled)
            {
                
                pi.inputRollEnabled = false;
                pi.rollEnabled = false;
                pi.roll = false;
                print("IS COMBO STATE");
                lastPositionAfterCombo6 = Vector2.zero;
                anim.Play("roll_attack");
            }else if(stateInfo.IsName("roll_attack"))
            {
                // if(dodgeAttackUsed)
                //     Combo = 0;
                print("IS DODGE COMBO STATE");
                anim.SetBool("roll",true);
                return;
            }
            
        }
        if(grounded)
            anim.SetBool("roll",true);
        else
        {
            if ((IsComboState(anim.GetCurrentAnimatorStateInfo(0)) 
                 || Combo > 0) && dodgeAttackUsed == false && pi.inputAttackEnabled)
            {
                
                pi.inputRollEnabled = false;
                pi.rollEnabled = false;
                pi.roll = false;
                print("IS COMBO STATE");
                lastPositionAfterCombo6 = Vector2.zero;
                anim.Play("roll_attack");
            }
        }
    }

    public override void DisappearRenderer()
    {
        
    }

    public void QuickLanding(bool needDisappear = true)
    {
        Instantiate(warpFX, transform.position,
            Quaternion.identity, BattleStageManager.Instance.RangedAttackFXLayer.transform);
        
        transform.position = new Vector3(transform.position.x,
            BasicCalculation.CheckRaycastedPlatform(gameObject).bounds.max.y+GetActorHeight() + 0.1f);

        if (needDisappear)
        {
            DisappearRenderer();
            Invoke("AppearRenderer",0.12f);
        }

        

    }

    public void EventRollAttack()
    {
        dodgeAttackUsed = true;
        try
        {
            _tweener?.Kill();
        }
        catch
        {
        }

        if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
        {
            SetFaceDir(-1);
        }
        else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
        {
            SetFaceDir(1);
        }

        StartCoroutine(HorizontalMove(rollspeed*2, 0.4f/2.5f, "roll_attack"));
    }


    #region Animation Actions

    

    public void Skill1_DashForward()
    {
        SetGravityScale(0);
        SetGroundCollision(false);
        var goal = new Vector2(transform.position.x + facedir * 10f,
            transform.position.y);
        var currentPosition = transform.position;
        goal = BattleStageManager.Instance.OutOfRangeCheck(goal);
        _tweener = rigid.DOMoveX(goal.x,0.15f).SetEase(Ease.OutSine).OnComplete(() =>
        {
            DisappearRenderer();
            SetFaceDir(-facedir);
            Invoke("AppearRenderer",0.1f);
            _tweener = rigid.DOMoveX(currentPosition.x,0.1f).SetEase(Ease.OutSine).OnComplete(() =>
            {
                ResetGravityScale();
                SetGroundCollision(true);
            }).OnKill(() =>
            {
                AppearRenderer();
                ResetGravityScale();
                SetGroundCollision(true);
            });
        }).OnKill(() =>
        {
            ResetGravityScale();
            SetGroundCollision(true);
        });
    }

    public void Skill3_WarpMove()
    {
        Instantiate(warpFX, transform.position, Quaternion.identity, 
            BattleStageManager.Instance.RangedAttackFXLayer.transform);

        var targetTransform = ta.GetNearestTargetInRangeDirection(facedir, 15f, 2f,
            LayerMask.GetMask("Enemies"));
        Vector2 targetPos;
        
        SetVelocity(0,0);
        DisappearRenderer();
        Invoke("AppearRenderer",0.2f);

        if (targetTransform != null)
        {
            targetPos = new Vector3(targetTransform.position.x,transform.position.y) + new Vector3(facedir * 3f, 1.5f);
            targetPos = BattleStageManager.Instance.OutOfRangeCheck(targetPos);
            SetFaceDir(-facedir);
        }
        else
        {
            targetPos = transform.position + new Vector3(facedir * 6f, 1.5f);
            targetPos = BattleStageManager.Instance.OutOfRangeCheck(targetPos);
        }
        
        transform.position = targetPos;
        SetGravityScale(0);

    }

    public void Skill3_SlashDownward()
    {
        var targetPos = transform.position + new Vector3(facedir * 2f, -1.5f);
        targetPos = BattleStageManager.Instance.OutOfRangeCheck(targetPos);
        
        _tweener = rigid.DOMove(targetPos,
            0.15f).SetEase(Ease.OutCirc).
            OnKill(ResetGravityScale).
            OnComplete(ResetGravityScale);


    }

    


    public void Combo4_JumpBackward()
    {
        _statusManager.
            ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,10,10,1,100502);
        
        
        
        
        //TODO:没有目标的话，直接后跳
        
        
        //var controllPoint = new Vector2(transform.position.x - facedir * 4f,
            //transform.position.y + 4);
        
        //controllPoint = BattleStageManager.Instance.OutOfRangeCheck(controllPoint);

        lastPlatformBeforeCombo4 = gameObject.RaycastedPlatform();

        var endPoint = new Vector2(transform.position.x - facedir * 2f,
            transform.position.y + 2.5f);
        endPoint = BattleStageManager.Instance.OutOfRangeCheck(endPoint);

        var path = new Vector3[] { transform.position, endPoint };
        
        SetGravityScale(0);

        _tweener = transform.DOPath(path, 0.15f,PathType.CatmullRom).SetEase(Ease.OutSine).OnKill(() =>
        {
            ResetGravityScale();
        });
        
        //TODO: 如果翻滚打断动画，就对重力进行重设


    }
    
    public void Combo4_JumpSmashForward()
    {
        //TODO:有目标的话，砸到目标前方一定距离。

        var warpCheckGO = Combo4_WarpCheck();
        
        

        if (warpCheckGO != null)
        {
            if (Mathf.Abs(warpCheckGO.transform.position.x - transform.position.x) > 4 &&
                Mathf.Abs(warpCheckGO.transform.position.x - transform.position.x) < 12)
            {
                print("WARP");
                Instantiate(warpFX, transform.position, Quaternion.identity, 
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);

                if (facedir == 1)
                {
                    transform.position = new Vector3(warpCheckGO.transform.position.x - 8,
                        transform.position.y, transform.position.z);
                }
                else if (facedir == -1)
                {
                    transform.position = new Vector3(warpCheckGO.transform.position.x + 8,
                        transform.position.y, transform.position.z);
                }

            }
        }

        //TODO:没有目标的话，按照固定距离砸下去
        
        var controllPoint = new Vector2(transform.position.x + facedir * 4f,
            transform.position.y + 2);
        
        controllPoint = BattleStageManager.Instance.OutOfRangeCheck(controllPoint);


        Vector2 safeEndPoint = new Vector2(transform.position.x + facedir * 3, transform.position.y);


        var col = lastPlatformBeforeCombo4;
        
        if(col == null)
            col = safeEndPoint.RaycastedPlatform();
        
        
        
        
        

        // var endPoint = new Vector2(transform.position.x + facedir * 3f,
        //     BasicCalculation.CheckRaycastedPlatform(gameObject).bounds.max.y + GetActorHeight() + 0.1f);
        var endPoint = new Vector2(transform.position.x + facedir * 3f,
            col.bounds.max.y + GetActorHeight() + 0.1f);
        endPoint = BattleStageManager.Instance.OutOfRangeCheck(endPoint);
        endPoint.x = BattleStageManager.Instance.OutOfPlatformBoundsCheck(gameObject, endPoint.x);

        
        
        var path = new Vector3[] { transform.position, endPoint };
        

        _tweener = transform.DOPath(path, 0.15f,PathType.CatmullRom).SetEase(Ease.OutSine).OnComplete(
            () =>
            {
                ResetGravityScale();
            });
        
        //TODO: 如果翻滚打断动画，就对重力进行重设


    }

    public void Combo6_RecordLastPosition()
    {
        lastPositionAfterCombo6 = transform.position;
    }
    
    public void Combo7_ReturnToLastPosition()
    {
        
        _statusManager.ObtainTimerBuff
            ((int)BasicCalculation.BattleCondition.BlazewolfsRush,-1,-1,1,0);
        
        if (lastPositionAfterCombo6 != Vector2.zero)
        {
            Instantiate(warpFX, transform.position, Quaternion.identity,
                BattleStageManager.Instance.RangedAttackFXLayer.transform);
            transform.position = lastPositionAfterCombo6;
            
        }
        
        lastPositionAfterCombo6 = Vector2.zero;
        
    }

    public void Combo7_JumpSmashForward()
    {
        //TODO:有目标的话，砸到目标前方一定距离。
        
        var warpCheckGO = Combo4_WarpCheck();

        if (warpCheckGO != null)
        {
            if (Mathf.Abs(warpCheckGO.transform.position.x - transform.position.x) > 4 &&
                Mathf.Abs(warpCheckGO.transform.position.x - transform.position.x) < 12)
            {
                print("WARP");
                Instantiate(warpFX, transform.position, Quaternion.identity, 
                    BattleStageManager.Instance.RangedAttackFXLayer.transform);

                if (facedir == 1)
                {
                    transform.position = new Vector3(warpCheckGO.transform.position.x - 5,
                        transform.position.y, transform.position.z);
                }
                else if (facedir == -1)
                {
                    transform.position = new Vector3(warpCheckGO.transform.position.x + 5,
                        transform.position.y, transform.position.z);
                }

            }
        }
        
        
        //TODO:没有目标的话，按照固定距离砸下去
        
        var endPoint = new Vector2(transform.position.x,
            BasicCalculation.CheckRaycastedPlatform(gameObject).bounds.max.y + GetActorHeight() + 0.1f);
        endPoint = BattleStageManager.Instance.OutOfRangeCheck(endPoint);
        
        endPoint.x = BattleStageManager.Instance.OutOfPlatformBoundsCheck(gameObject, endPoint.x);
        
        var path = new Vector3[] { transform.position, endPoint };
        

        _tweener = rigid.DOMove(endPoint, 0.15f).SetEase(Ease.OutSine).OnComplete(
            () =>
            {
                ResetGravityScale();
            });
        
        //TODO: 如果翻滚打断动画，就对重力进行重设


    }
    public void Combo7_WarpToHighPoint()
    {
        var endPoint = new Vector2(transform.position.x,transform.position.y+2.5f);
        endPoint = BattleStageManager.Instance.OutOfRangeCheck(endPoint);
        transform.position = endPoint;
    }
    

    #endregion
    
    
    
    
    
    
    
    
    
    
    
    
    

    public void onRollEnterBase()
    {
        OnRollEnterBase();
        
        OnAttackInterrupt?.Invoke();
        pi.directionLock = false;
        dodgeAttackUsed = true;
    }

    public override void onRollExit()
    {
        OnRollExitBase();
        
        //Combo = 0;
    }

    public void onRollExitBase()
    {
        OnRollExitBase();
        //dodgeAttackUsed = false;
    }
    
    


    public override void OnStandardAttackEnter()
    {
        OnStandardAttackEnterBase();
        
        
        pi.stdAtk = false;
        pi.moveEnabled = false;
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.inputAttackEnabled = false;
          
        if(comboStageResetRoutine!=null)
            StopCoroutine(comboStageResetRoutine);
        comboStageResetRoutine = null;
          
        ClearBoolSignal("attack");
          
          
        Combo++;
        
        PlayComboVoice();

        // if (Combo <= 1)
        // {
        //     voiceController?.PlayAttackVoice(1);
        // }else if (Combo <= 3)
        // {
        //     voiceController?.PlayAttackVoice(2);
        // }else if (Combo <= 5)
        // {
        //     voiceController?.PlayAttackVoice(3);
        // }
        // else
        // {
        //     voiceController?.PlayAttackVoice(2);
        // }

        if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo6"))
        {
            _statusManager.knockbackRes = 999;
        }


        if (Combo >= 7)
            Combo = 0;
        
        dodgeAttackUsed = false;
    }
    
    protected override void PlayComboVoice()
    {
        if (Combo <= 1)
        {
            voiceController?.PlayAttackVoice(1);
        }else if (Combo <= 3)
        {
            voiceController?.PlayAttackVoice(2);
        }else if (Combo <= 5)
        {
            voiceController?.PlayAttackVoice(3);
        }
        else
        {
            voiceController?.PlayAttackVoice(2);
        }
    }

    public override void OnStandardAttackExit()
    {
        base.OnStandardAttackExit();
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("combo6") && Combo == 0)
        {
            
        }

        _statusManager.knockbackRes = 0;
    }

    /// <summary>
    /// 1: 普通攻击c1和c2
    /// 2: c3
    /// </summary>
    /// <param name="moveID"></param>
    public override void FaceDirectionAutoFix(int moveID)
    {
        switch (moveID)
        {
            case 1:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 12f, 1f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 12f, 1f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }

                break;
            }
            case 2:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 5f, 1f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 5f, 1f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }
                break;
            }
            case 3:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 11f, 5f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 11f, 5f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }
                break;
            }
            case 4:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 8f, 1f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 8f, 1f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }
                break;
            }
                
                
                
                
                
                
                

        }
    }

    protected GameObject Combo4_WarpCheck()
    {
        float closestX = 9999f;
        GameObject target = null;
        
        Collider2D[] AttackRangeAreaInfo =
            Physics2D.OverlapAreaAll
            (transform.position,
                transform.position + new Vector3(facedir*10f , -4f),
                LayerMask.GetMask("Enemies"));
        
        
        foreach (var hit in AttackRangeAreaInfo)
        {
            float distance = Mathf.Abs(hit.transform.position.x - transform.position.x);
            if (distance < closestX)
            {
                closestX = distance;
                target = hit.gameObject;
            }
        }

        return target;

    }


    public override void OnHurtEnter()
    {
        base.OnHurtEnter();
        AppearRenderer();
    }

    public override void OnSkillEnter()
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("s4") == false)
        {
            _statusManager.ObtainTimerBuff(buffWhileSkill);
            _statusManager.knockbackRes = 999;
        }
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("s1") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("s2") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("s1_boost") ||
            anim.GetCurrentAnimatorStateInfo(0).IsName("s2_boost"))
        {
            skillBoosted = true;
        }

        base.OnSkillEnter();
    }

    public override void OnSkillExit()
    {
        _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.DamageCut,
            100501, false);
        _statusManager.knockbackRes = 0;
        base.OnSkillExit();
        Invoke("CancelSkillBoost",0.5f);
    }

    public void SkillPrepCheck(int skillID)
    {
        
        if (_statusManager.GetConditionStackNumber((int)BasicCalculation.BattleCondition.BlazewolfsRush) > 0)
        {
            _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.BlazewolfsRush,true,0);
            _statusManager.FillSP(skillID, 100);
            _statusManager.OnSpecialBuffDelegate?.Invoke
                (UI_BuffLogPopManager.SpecialConditionType.SPCharge.ToString());
        }

    }


    protected void CancelSkillBoost()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("s1"))
            return;
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("s2"))
            return;
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("s1_boost"))
            return;
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("s2_boost"))
            return;
        skillBoosted = false;
    }
}
