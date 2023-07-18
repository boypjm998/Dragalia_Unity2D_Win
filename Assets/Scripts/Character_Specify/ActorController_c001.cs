using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController_c001 : ActorController
{
    AlchemicGauge alchemicGauge;
    
    public override void UseSkill(int id)
    {
        if(voiceController.voiceLoaded)
            voiceController.PlaySkillVoice(id);
        
        if (isAttackSkill[id - 1])
        {
            pi.InvokeAttackSignal();
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
            
            case 7:
                pi.isSkill = true;
                anim.Play("s3_boost");
                _statusManager.currentSP[2] = 0;
                break;

            default:
                break;
        }
    }



    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();

        alchemicGauge = GameObject.Find("UI").transform.Find("CharacterInfo/AlchemicGauge")?.GetComponent<AlchemicGauge>();
        voiceController = GetComponentInChildren<VoiceController_C001>();



    }

    public void GetGauge()
    {
        alchemicGauge = GameObject.Find("AlchemicGauge")?.GetComponent<AlchemicGauge>();
    }




    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
        CheckTransport();
        CheckSkill();
        
      
    }
    void FixedUpdate()
    {
        Move();



    }

    protected void CheckTransport()
    {
        if (pi.buttonUp.OnPressed)
        {
            
            string[] canTransformStates = { "idle", "walk", "fall", "jump", "jump2", "roll" };
            bool canTransform = pi.CheckCharacterClipState(anim, 0, canTransformStates);
            if (!pi.hurt && canTransform)
            {
                var portals = FindObjectOfType<AdventurerSpecial_Portal>();
                if (portals != null)
                {
                    portals.Transport(transform);
                    
                }

                
            }
        }


        
    }

    protected override void CheckSkill()
    {


        if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            if (alchemicGauge.IsCatridgeActive())
            {
                UseSkill(5);
                alchemicGauge.CatridgeConsume();
                _statusManager.RemoveTimerBuff(101);
            }
            else
            {
                UseSkill(1);
            }
        }

        if (pi.skill[1] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill && alchemicGauge.GetCatridgeNumber()>0)
        {
            if (alchemicGauge.IsCatridgeActive())
            {
                UseSkill(6);
                alchemicGauge.CatridgeConsume();
                _statusManager.RemoveTimerBuff(101);
            }
            else
            {
                UseSkill(2);
            }
        }
        
        if (pi.skill[2] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            if (alchemicGauge.IsCatridgeActive())
            {
                UseSkill(7);
                alchemicGauge.CatridgeConsume();
                _statusManager.RemoveTimerBuff(101);
            }
            else
            {
                UseSkill(3);
            }
        }
        
        if (pi.skill[3] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
        
    }



    //Event functions and Setting functions






    #region Animation States Events

    public void ActiveAlchemicEnhancement()
    {
        alchemicGauge.SetCatridgeActive();
    }


    //人物滚动时附加的位移效果
    public override void EventRoll()
    {
        //还需要优化，不一定能打中最近的目标
        Transform tarTrans = ta.GetNearestReachableTarget(16.0f, LayerMask.GetMask("Enemies"));
        bool needTurnBack = TurnAroundCheck(tarTrans);

        //SetVelocity(facedir*rollspeed,rigid.velocity.y);



        if (needTurnBack)
        {
            float tarx = ta.GetNearestReachableTarget(16.0f, LayerMask.GetMask("Enemies")).position.x;
            if (tarx < rigid.position.x)
            {
                SetFaceDir(-1);

                if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
                {
                    StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));
                }else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
                {
                    StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "roll"));
                }
                else
                {
                    StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "roll"));
                }


                //StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));

            }

            else if (tarx > rigid.position.x)
            {
                SetFaceDir(1);
                
                if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
                {
                    StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "roll"));
                }else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
                {
                    StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));
                }
                else
                {
                    StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "roll"));
                }

            }

            //StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "roll"));
        }
        else if (tarTrans != null)
        {
            if ((pi.buttonLeft.IsPressing && facedir == 1) || (pi.buttonRight.IsPressing && facedir == -1))
            {
                StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "roll"));
            }
            else
            {
                StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));
            }

        }
        else
        {
            if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
            {
                SetFaceDir(-1);
            }
            else if (pi.buttonRight.IsPressing && !pi.buttonLeft.IsPressing)
            {
                SetFaceDir(1);
            }
            StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));
        }


    }
    public override void EventDash()
    {
        //SetVelocity(facedir*rollspeed,rigid.velocity.y);
        StartCoroutine(HorizontalMove(rollspeed * 3f, 10.0f, 0.1f, "dash"));

    }
  
    #endregion

    #region Messages Process Moudles
   

    public override void OnStandardAttackEnter()
    {
        base.OnStandardAttackEnter();
        ActionDisable((int)PlayerActionType.MOVE);//move
        ActionDisable((int)PlayerActionType.JUMP);//jump
        pi.SetInputDisabled("move");
        
        if(anim.GetBool("isAttack")==false)
            voiceController.PlayAttackVoice(1);
        
        
        StartAttack();
        
        

        //ActionEnable(3);//roll
        //pi.jumpEnabled = false;
        //pi.moveEnabled = false;
        //pi.rollEnabled = true;

    }
    public override void OnStandardAttackExit()
    {
        base.OnStandardAttackExit();
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.MOVE);
        ExitAttack();
        //pi.jumpEnabled = true;
        //pi.moveEnabled = true;
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);
        //anim.SetLayerWeight(anim.GetLayerIndex("Actor"), 1.0f);
        //print("Exit");
    }

    public override void OnStandardAttackConnect(AttackBase attackStat)
    {

        AlchemicGauge alchemicGauge = this.alchemicGauge;
        if (alchemicGauge == null)
        {
            alchemicGauge = GameObject.Find("UI").transform.Find("CharacterInfo/AlchemicGauge")
                .GetComponent<AlchemicGauge>();
        }


        AttackContainer container = attackStat.GetComponentInParent<AttackContainer>();

        if (container.hitConnectNum >= container.attackTotalNum)
            return;

        if (!alchemicGauge.IsCatridgeActive())
        {
            alchemicGauge.Charge(2);
            if (_statusManager.comboHitCount > 30)
            {
                alchemicGauge.Charge(4);
            }
        }

        



    }

    public override void OnOtherAttackConnect(AttackBase attackStat)
    {
        AlchemicGauge alchemicGauge = this.alchemicGauge;
        if (alchemicGauge == null)
        {
            alchemicGauge = GameObject.Find("UI").transform.Find("CharacterInfo/AlchemicGauge")
                .GetComponent<AlchemicGauge>();
        }

        AttackContainer container = attackStat.GetComponentInParent<AttackContainer>();

        if (container.hitConnectNum >= container.attackTotalNum)
            return;


        if (!alchemicGauge.IsCatridgeActive())
        {
            alchemicGauge.Charge(1);
            if (_statusManager.comboHitCount > 30)
            {
                alchemicGauge.Charge(2);
            }
        }

        //翻滚充能1.
    }


    #endregion



    private bool TurnAroundCheck(Transform targetTransform)
    {
        if (targetTransform == null)
            return false;
        if (targetTransform.position.x > rigid.transform.position.x)
        {
            if (facedir == 1)
                return false;
            else return true;
        }
        if (targetTransform.position.x < rigid.transform.position.x)
        {
            if (facedir == -1)
                return false;
            else return true;
        }
        return false;

    }

    public override void OnDashEnter()
    {
        base.OnDashEnter();
        voiceController.PlayAttackVoice(0);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="moveID">1:普攻,2:s1,3:s2</param>
    public override void FaceDirectionAutoFix(int moveID)
    {
        base.FaceDirectionAutoFix(moveID);
        switch (moveID)
        {
            case 1:
            {
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 20f, 1f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 20f, 1f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }

                break;
            }
            case 2:
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 30f, 8f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 20f, 8f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }
                break;
            case 3:
                if (ta.GetNearestTargetInRangeDirection
                    (facedir, 18f, 4f,
                        LayerMask.GetMask("Enemies")) == null
                    &&
                    ta.GetNearestTargetInRangeDirection
                    (-facedir, 18f, 4f,
                        LayerMask.GetMask("Enemies")) != null)
                {
                    SetFaceDir(-facedir);
                }
                break;
            default:
                break;


        }
    }
}
