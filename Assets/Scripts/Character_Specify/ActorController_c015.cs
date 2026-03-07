using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityGameObject;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class ActorController_c015 : ActorControllerMeeleWithFS
{
    
    public bool IsDelayed { get; private set; }
    public bool UseDelayedAnimation { get; set; }

    private Tween _delayedTween;
    private float _currentPosY;

    private AttackManager_C015 _attackManager;
    private int _currentPressingDirection;
    public bool directionCheckEnable = false;

    public bool Skill3IsForward { get; private set; }
    public bool Skill3IsStatic { get; private set; }

    private void Start()
    {
        _attackManager = GetComponent<AttackManager_C015>();
    }

    protected override void Update()
    {
        base.Update();
        CheckSkillMoveDirection();
    }

    private void CheckSkillMoveDirection()
    {
        if (directionCheckEnable)
        {
            if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
            {
                _currentPressingDirection = -1;
            }
            else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
            {
                _currentPressingDirection = 1;
            }
            else
            {
                _currentPressingDirection = 0;
            }
        }
    }

    public override void UseSkill(int id)
    {
        var edenMode = 
            _statusManager.HasCondition((int)BasicCalculation.BattleCondition.EdenMode);
        
        if (isAttackSkill[id - 1] || (id==1&&edenMode))
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
                if (edenMode)
                {
                    //todo: 改成s1_boost
                    anim.Play("s1_boost",0,0);
                    voiceController?.PlaySkillVoice(1,1);
                }
                else
                {
                    anim.Play("s1",0,0);
                    voiceController?.PlaySkillVoice(1,0);
                }
                _statusManager.currentSP[0] = 0;
                break;

            case 2:
                pi.isSkill = true;
                if (edenMode)
                {
                    anim.Play("s2_boost",0,0);
                    voiceController?.PlaySkillVoice(2,1);
                }
                else
                {
                    anim.Play("s2",0,0);
                    voiceController?.PlaySkillVoice(2,0);
                }
                _statusManager.currentSP[1] = 0;
                break;

            case 3:
                pi.isSkill = true;
                anim.Play("s3",0,0);
                _statusManager.currentSP[2] = 0;
                voiceController?.PlaySkillVoice(3,Random.Range(0,2));
                break;

            case 4:
                pi.isSkill = true;
                anim.Play("s4",0,0);
                _statusManager.currentSP[3] = 0;
                voiceController?.PlaySkillVoice(4,Random.Range(0,2));
                break;

            default:
                break;
        }
    }

    public override void OnStandardAttackEnter()
    {
        StartAttack();
        anim.SetBool("delayed",false);
        anim.SetBool("reset",false);
        _delayedTween?.Kill();
        
        //base.standardattackenter
        ta.FaceDirectionAutofixWithMarking();
        ta.TargetSwapByAttack();
        speedModifier = 1;
        SetAttackRateToAnimator();
        
        
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

        if (Combo <= 1)
        {
            voiceController?.PlayAttackVoice(1);
        }else if (Combo <= 3)
        {
            voiceController?.PlayAttackVoice(2);
        }else if (Combo <= 4)
        {
            voiceController?.PlayAttackVoice(3);
        }


        ResetCombo();
    }

    protected override void ResetCombo()
    {
        if (Combo >= 4)
            Combo = 0;
    }

    protected void ForceResetCombo()
    {
        Combo = 1;
    }

    public override void OnStandardAttackExit()
    {
        ExitAttack();
        anim.speed = 1;//base.OnStandardAttackExit();
        pi.jumpEnabled = true;
        pi.moveEnabled = true;
        pi.attackEnabled = true;
        pi.jumpEnabled = true;
        pi.inputAttackEnabled = true;
        pi.inputRollEnabled = true;
        pi.rollEnabled = true;

        if (anim.GetBool("reset"))
        {
            Combo = 0;
        }
        
    }

    protected override bool SpecialConditionCheck(int sid)
    {
        if (sid == 1)
        {
            if (_attackManager.chargeGauge.currentCp < 33 && !_attackManager.EdenModeActive)
                return false;
        }

        return true;
    }

    

    protected void ResetFig()
    {
        var fig = Projectile_C015_1.Instance;

        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.EdenMode))
        {
            if(fig.isOccupied)
                fig.ReturnToPosition(0.01f,Ease.Linear);
            fig.isOccupied = false;
            if(fig.HasTarget)
                fig.SetActive(true);
        }
        else
        {
            fig.isOccupied = false;
            fig.SetActive(false);
            fig.SetRenderer(false);
        }
    }

    protected IEnumerator ResetComboStageAndDelay(float time)
    {
        
        yield return new WaitForSeconds(time);
        Combo = 0;
        IsDelayed = false;
        UseDelayedAnimation = false;
        comboStageResetRoutine = null;
    }

    public void Combo4A_Jump()
    {
        //pi.moveEnabled = false;
        dodging = true;
        SetGroundCollision(false);

        var posx = GetHorizontalMovementPositionX
            (20, 2, 6, 0.1f, Ease.OutSine);

        //_currentPlatform = gameObject.RaycastedPlatform();
        _currentPosY = transform.position.y;

        Vector2 targetPos;
        
        SetGravityScale(0.01f);
        var tweenerY =
            transform.DOMoveY( _currentPosY + 8f, 0.1f).SetEase(Ease.OutCirc);

        _tweener = transform.DOMoveX(posx, 0.1f);
    }
    
    public void Combo4A_Smash()
    {
        //pi.moveEnabled = true;
        dodging = false;
        SetGroundCollision(true);

        ResetGravityScale();
        var tweenerY =
            transform.DOMoveY(_currentPosY, 0.1f).SetEase(Ease.InCirc);

        //_tweener = rigid.DOMoveX(posx, 0.15f);
    }

    public void Skill2_Jump()
    {
        SetGroundCollision(false);
        
        _currentPosY = transform.position.y;

        SetGravityScale(0f);
        
        _tweener =
            transform.DOMoveY( _currentPosY + 3, 0.2f).SetEase(Ease.OutCirc);

    }
    
    public void Skill2_Land()
    {
        SetGroundCollision(true);
        
        Vector2 targetPos;
        
        ResetGravityScale();
        
        _tweener =
            transform.DOMoveY( _currentPosY, 0.2f).SetEase(Ease.OutCirc);
    }
    
    public void Skill2_Boost_Dash()
    {
        
        if(_currentPressingDirection != 0)
            SetFaceDir(_currentPressingDirection);

        if (pi.buttonLeft.IsPressing || pi.buttonRight.IsPressing)
        {
            GeneralHorizontalMovement(7, 0.4f, Ease.InOutSine);
        }
        else
        {
            GeneralHorizontalMovementWithEnemyCheck(7, 3, 4, 0.4f, Ease.InOutSine);
        }
        
        ActiveDirectionCheck(false);

    }

    public void Skill3_AutoCheckSkillType()
    {
        if (_currentPressingDirection == 0)
        {
            var nearestTarget = ta.GetNearestTargetInRangeDirection(facedir, 6, 4,
                LayerMask.GetMask("Enemies"));

            if (nearestTarget)
            {
                Skill3IsForward = false;
                Skill3IsStatic = true;
            }
            else
            {
                Skill3IsForward = true;
                Skill3IsStatic = false;
            }
        }
        else
        {
            Skill3IsStatic = false;
            
            if (_currentPressingDirection == facedir)
            {
                Skill3IsForward = true;
                ActiveDirectionCheck(false);
            }
            else
            {
                Skill3IsForward = false;
            }
        }
        
    }

    public void Skill3_AutoCheckStatic()
    {
        if (_currentPressingDirection == 0 || _currentPressingDirection == facedir)
        {
            Skill3IsStatic = true;
        }
        else
        {
            Skill3IsStatic = false;
        }

        ActiveDirectionCheck(false);
    }

    public void Skill3_BackwardWarp()
    {
        float rangeL = BattleStageManager.Instance.mapBorderL;
        float rangeR = BattleStageManager.Instance.mapBorderR;
        
        var currentPlatform = gameObject.RaycastedPlatform();
        
        var atkRay = Physics2D.Raycast(transform.position - new Vector3(facedir*5,0),
            new Vector2(-facedir, 0), 3, LayerMask.GetMask("AttackEnemy"));
        
        var backwardRay =
            Physics2D.Raycast(transform.position,
                new Vector2(-facedir, 0), 20, LayerMask.GetMask("Border"));

        var defaultPos = transform.position.x - facedir * 8;
        
        if (backwardRay.collider)
        {
            if (facedir > 0)
            {
                rangeL = Mathf.Max(backwardRay.collider.bounds.max.x - 0.5f, currentPlatform.bounds.min.x, rangeL);
            }
            else
            {
                rangeR = Mathf.Min(backwardRay.collider.bounds.min.x + 0.5f, currentPlatform.bounds.max.x, rangeR);
            }
        }
        else
        {
            rangeL = Mathf.Max(rangeL, currentPlatform.bounds.min.x);
            rangeR = Mathf.Min(rangeR, currentPlatform.bounds.max.x);
        }
        
        if (atkRay.collider)
        {
            var col = atkRay.collider;
            if (facedir < 0 && col.bounds.min.x > transform.position.x + 5)
            {
                defaultPos = col.bounds.min.x - 1;
            }
            else if (facedir > 0 && col.bounds.max.x < transform.position.x - 5)
            {
                defaultPos = col.bounds.max.x + 1;
            }
        }

        float targetPos = Mathf.Clamp(defaultPos, rangeL, rangeR);

        _tweener = transform.DOMoveX(targetPos, 0.15f);

    }
    
    
    public void Skill3_ForwardWarp()
    {
        float rangeL = BattleStageManager.Instance.mapBorderL;
        float rangeR = BattleStageManager.Instance.mapBorderR;
        
        var currentPlatform = gameObject.RaycastedPlatform();
        
        var backwardRay =
            Physics2D.Raycast(transform.position,
                new Vector2(facedir, 0), 20, LayerMask.GetMask("Border"));

        var atkRay = Physics2D.Raycast(transform.position + new Vector3(facedir*8,0),
            new Vector2(facedir, 0), 4, LayerMask.GetMask("AttackEnemy"));

        if (backwardRay.collider)
        {
            if (facedir < 0)
            {
                rangeL = Mathf.Max(backwardRay.collider.bounds.max.x - 0.5f, currentPlatform.bounds.min.x, rangeL);
            }
            else
            {
                rangeR = Mathf.Min(backwardRay.collider.bounds.min.x + 0.5f, currentPlatform.bounds.max.x, rangeR);
            }
        }
        else
        {
            rangeL = Mathf.Max(rangeL, currentPlatform.bounds.min.x);
            rangeR = Mathf.Min(rangeR, currentPlatform.bounds.max.x);
        }

        var defaultPos = transform.position.x + facedir * 12;
        
        if (atkRay.collider)
        {
            var col = atkRay.collider;
            if (facedir > 0 && col.bounds.min.x > transform.position.x + 8)
            {
                defaultPos = col.bounds.min.x - 1;
            }
            else if (facedir < 0 && col.bounds.max.x < transform.position.x - 8)
            {
                defaultPos = col.bounds.max.x + 1;
            }
        }
        
        float targetPos = Mathf.Clamp(defaultPos, rangeL, rangeR);

        _tweener = transform.DOMoveX(targetPos, 0.3f);

    }
    

    public override void onRollEnter()
    {
        base.onRollEnter();
        CancelDelay();
    }

    public override void OnHurtEnter()
    {
        base.OnHurtEnter();
        CancelDelay();
    }

    public override void OnSkillEnter()
    {
        base.OnSkillEnter();
        CancelDelay();
        ActiveDirectionCheck(true);
    }
    
    protected override void OnIdleEnter()
    {
        if (comboStageResetRoutine != null)
        {
            StopCoroutine(comboStageResetRoutine);
            comboStageResetRoutine = null;
            //print("interrupted");
        }

        comboStageResetRoutine = StartCoroutine(ResetComboStageAndDelay(.8f));
    }

    public override void OnSkillExit()
    {
        base.OnSkillExit();
        ResetFig();
        AppearRenderer();
        ActiveDirectionCheck(false);
    }

    private void ActiveDirectionCheck(bool flag)
    {
        directionCheckEnable = flag;
        if (flag == false)
            _currentPressingDirection = 0;
    }
    
    public void CancelDelay()
    {
        UseDelayedAnimation = false;
        anim.SetBool("delayed", false);
        IsDelayed = false;
        anim.SetBool("reset",false);
        _delayedTween?.Kill();
    }


    public bool StartDelayTween(float delayTime = 0.66f)
    {
        var isdelayed = IsDelayed;
        UseDelayedAnimation = IsDelayed;
        
        var currentStageInfo = anim.GetCurrentAnimatorStateInfo(0);

        // 如果C3是绿光，那么需要将重置设为true。当点击过快时会直接重置回红光C1。
        if (currentStageInfo.IsName("combo3") && UseDelayedAnimation)
        {
            anim.SetBool("reset",true);
        }
        else
        {
            anim.SetBool("reset",false);
        }

        if (!anim.GetBool("delayed"))
        {
            IsDelayed = false;
        }
        else
        {
            
        }

        anim.SetBool("delayed",false);

        bool useDelay = UseDelayedAnimation;
        
        _delayedTween?.Kill(false);

        _delayedTween = DOVirtual.DelayedCall(delayTime, () =>
        {
            

            if (currentStageInfo.IsName("combo1") ||
                currentStageInfo.IsName("combo2") ||
                currentStageInfo.IsName("combo4a")||
                currentStageInfo.IsName("combo4b"))
            {
                IsDelayed = true;
                anim.SetBool("delayed",true);
            }
            else
            {
                if (currentStageInfo.IsName("combo3"))
                {
                    if (!useDelay)
                    {
                        Debug.Log("回到C1，应该是红光");
                        //如果C3阶段是红光，但是触发了延迟，那么直接重置回到C1
                        anim.SetBool("reset",true);
                        anim.SetBool("delayed",true);
                        IsDelayed = true;
                    }
                    else
                    {
                        //如果C3阶段是绿光，正常C4
                        anim.SetBool("reset",false);
                        anim.SetBool("delayed",true);
                        IsDelayed = true;
                    }
                }
            }
            
            
        },false);

        return isdelayed;

    }

    public void StopDelayTween()
    {
        _delayedTween.Kill(false);
    }
    
}
