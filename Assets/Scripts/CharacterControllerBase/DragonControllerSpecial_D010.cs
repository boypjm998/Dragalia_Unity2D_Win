using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonControllerSpecial_D010 : DragonController
{
    

    protected DragonAttackManager_D010 da;
    public GameObject weaponRenderer;

    
    public int Combo
    {
        get { return dAnim.GetInteger("combo"); }
        set { dAnim.SetInteger("combo",value); }
    }


    protected override void CheckCombo()
    {
        base.CheckCombo();
        Combo++;

        //TODO: 播放语音
        if (Combo <= 1)
        {
            
        }else if (Combo <= 2)
        {
            
        }else if (Combo <= 3)
        {
            
        }
          
        if (Combo >= 3)
            Combo = 0;
    }

    protected override void Awake()
    {
        base.Awake();
        //pi.moveEnabled = true;
        da = GetComponent<DragonAttackManager_D010>();
        
    }

    protected void OnEnable()
    {
        EnterShapeShifting();
        SetWeaponVisibility(true);
    }

    protected void FixedUpdate()
    {
        Move();
    }

    protected override void Move()
    {
        base.Move();
        
        if (pi.enabled == false)
        {
            dAnim.SetFloat("forward", 0f);
            dAnim.SetFloat("upward", 0);
            //anim.Play("idle");
            return;
        }

        if (pi.moveEnabled == false)
        {
            dAnim.SetFloat("forward", 0f);
            dAnim.SetFloat("upward", 0);
            return;
        }

        if (pi.enabled)
        {
            dAnim.SetFloat("forward", Mathf.Abs(pi.DRight));
            dAnim.SetFloat("upward", Mathf.Abs(pi.DUp));
        }

        var normalizedSpeed = NormalizedSpeed(new Vector2(pi.isMove, pi.DUp));
        
        rigid.position += new Vector2
            ( moveSpeed * normalizedSpeed.x, moveSpeed * normalizedSpeed.y * 1.25f) * Time.fixedDeltaTime;
        
        if (pi.directionLock == true)
        {
            return;
        }

        //if(anim.GetBool("attack") == false)
        CheckFaceDir();
        
    }

    

    protected override void OnDModeRollEnter()
    {
        base.OnDModeRollEnter();
    }
    
    protected override void OnDModeRollExit()
    {
        base.OnDModeRollExit();
        Combo = 0;
    }

    protected override void OnDModeSkillEnter()
    {
        base.OnDModeSkillEnter();
        ac.SetGravityScale(0);
        ac.SetVelocity(0,0);
        Combo = 0;
    }
    
    protected override void OnDModeSkillExit()
    {
        base.OnDModeSkillExit();
        ac.SetGravityScale(gravityScale);
        SetWeaponVisibility(true);
    }

    protected override IEnumerator ResetComboStage(float time)
    {
        yield return new WaitForSeconds(time);
        Combo = 0;
        comboStageResetRoutine = null;
    }

    protected override void CheckSkill()
    {
        if (pi.skill[0] && ac.DModeIsOn && !pi.isSkill)
        {
            UseSkill(1);
        }

        if (pi.skill[1] && ac.DModeIsOn && !pi.isSkill)
        {
            UseSkill(2);
        }
        
    }

    protected override void UseSkill(int skillID)
    {
        da._voiceController.PlaySkillVoice(skillID+4);
        
        //todo:发出攻击信号
        
        AttackFromPlayer.CheckEnergyLevel(_statusManager);
        AttackFromPlayer.CheckInspirationLevel(_statusManager);
        
        
        switch (skillID)
        {
            case 1:
                pi.isSkill = true;
                dAnim.Play("s1");
                currentDSP[0] = 0;
                break;

            case 2:
                pi.isSkill = true;
                dAnim.Play("s2");
                currentDSP[1] = 0;
                break;
            

            default:
                break;
        }
    }

    public void SetWeaponVisibility(bool flag)
    {
        weaponRenderer.SetActive(flag);
    }
}
