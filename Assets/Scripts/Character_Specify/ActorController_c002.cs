using UnityEngine;
using System.Collections;
using DG.Tweening;
using GameMechanics;


public class ActorController_c002 : ActorControllerDagger
{
    private int flashOfGenuisCount = 0;
    public bool skillBoosted = false;

    public override void Move()
    {
        base.Move();
    }

    protected override void CheckSkill()
    {
        if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(1);
        }

        if (pi.skill[1] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(2);
        }
          
        if (pi.skill[2] && !pi.hurt && (!pi.isSkill || skillBoosted) )
        {
            UseSkill(3);
        }

        if (pi.skill[3] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
    }

    public override void OnSkillConnect(AttackBase attack_statusManager)
    {
        if (attack_statusManager.skill_id == 1)
        {
            flashOfGenuisCount++;
            if (flashOfGenuisCount >= 2)
            {
                flashOfGenuisCount = 0;
                _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,
                    20,7);
            }
        }
    }

    public void Skill1(int eventID)
    {
        if (eventID == 1)
        {
            skillBoosted = true;
            voiceController?.PlaySkillVoice(1);
            //speedModifier = 0.2f;
            StartCoroutine(HorizontalMove(movespeed * 0.15f, 3f, "s1"));
            //pi.moveEnabled = true;
            FaceDirectionAutoFix(4);
            pi.LockDirection(1);
        }else if (eventID == 2)
        {
            if (pi.isMove < -0.1f)
            {
                SetFaceDir(-1);
            }
            else if (pi.isMove > 0.1f)
            {
                SetFaceDir(1);
            }
        }
        else if (eventID == 3)
        {
            //180帧
            pi.moveEnabled = false;
            pi.isMove = 0;
            
            pi.LockDirection(0);
            SetGravityScale(0);
            _tweener = transform.DOMoveY(transform.position.y + 5f, 0.3f).SetEase(Ease.OutSine);
        }
        else if (eventID == 4)
        {
            //222
            var targetGroundHeight = BasicCalculation.GetRaycastedPlatformY(gameObject) + GetActorHeight();
            try
            {
                _tweener.Kill();
            }
            catch
            {
                
            }

            _tweener = transform.DOMoveY(targetGroundHeight, 0.1f).SetEase(Ease.InSine)
                .OnComplete(() =>
                {
                    ResetGravityScale();
                }).OnKill(() =>
                {
                    ResetGravityScale();
                });

        }
        else if (eventID == 5)
        {
            Invoke(nameof(CancelSkillBoost),1f);
            pi.rollEnabled = true;
            pi.inputRollEnabled = true;
        }



    }

    public void Skill3(int eventID)
    {
        
        OnAttackInterrupt?.Invoke();
        
        ResetGravityScale();
        
        if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
        {
            SetFaceDir(-1);
        }else if (pi.buttonRight.IsPressing && !pi.buttonLeft.IsPressing)
        {
            SetFaceDir(1);
        }

        //skillBoosted = false;
        var dest = BattleStageManager.Instance.OutOfRangeCheck
            (transform.position + new Vector3(7.5f * facedir, 0, 0));
        var dest_debug = transform.position + new Vector3(7.5f * facedir, 0, 0);

        _tweener = transform.DOMoveX(dest.x, 0.3f).
            SetEase(Ease.InOutSine);
        _tweener.Play();

    }


    protected void CancelSkillBoost()
    {
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("s1"))
            return;
        skillBoosted = false;
    }

    public override void OnSkillEnter()
    {
        
        base.OnSkillEnter();
    }


    public override void OnStandardAttackEnter()
    {
        skillBoosted = false;
        base.OnStandardAttackEnter();
    }

    public override void OnHurtEnter()
    {
        base.OnHurtEnter();
        skillBoosted = false;
    }
}
