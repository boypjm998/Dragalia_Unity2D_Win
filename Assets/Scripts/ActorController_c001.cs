using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController_c001 : ActorController
{
    AlchemicGauge alchemicGauge;

   

    public override void UseSkill(int id)
    {

        switch (id)
        {
            case 1:
                anim.Play("s1");
                stat.currentSP[0] = 0;
                break;

            case 2:
                anim.Play("s2");
                stat.currentSP[1] = 0;
                break;

            default:
                break;
        }
    }



    // Start is called before the first frame update
    void Awake()
    {
        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        anim = rigid.GetComponent<Animator>();

        rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        facedir = 1;
        ta = gameObject.transform.parent.GetComponentInChildren<TargetAimer>();

        stat = GetComponent<StatusManager>();
        jumpforce = stat.jumpforce;
        movespeed = stat.movespeed;
        rollspeed = 9.0f;

        alchemicGauge = GameObject.Find("AlchemicGauge").GetComponent<AlchemicGauge>();

    }


    // Update is called once per frame
    protected override void Update()
    {

        base.Update();
        CheckSkill();
      
    }
    void FixedUpdate()
    {
        Move();



    }

    protected override void CheckSkill()
    {


        if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            if (alchemicGauge.IsCatridgeActive())
            {

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

            }
            else
            {
                UseSkill(2);
            }
        }
    }



    //Event functions and Setting functions


    //�������ؽ�ɫ���ٶ�

    #region Move Horizontally

    #endregion

    #region Animation States Events

    public void ActiveAlchemicEnhancement()
    {
        alchemicGauge.SetCatridgeActive();
    }


    //�������ʱ���ӵ�λ��Ч��
    public override void EventRoll()
    {
        //����Ҫ�Ż�����һ���ܴ��������Ŀ��
        Transform tarTrans = ta.GetNearestReachableTarget(16.0f, LayerMask.GetMask("Enemies"));
        bool needTurnBack = TurnAroundCheck(tarTrans);

        //SetVelocity(facedir*rollspeed,rigid.velocity.y);



        if (needTurnBack)
        {
            float tarx = ta.GetNearestReachableTarget(16.0f, LayerMask.GetMask("Enemies")).position.x;
            if (tarx < rigid.position.x)
            {
                SetFaceDir(-1);

                //StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));

            }

            else if (tarx > rigid.position.x)
            {
                SetFaceDir(1);


                //StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));

            }

            StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));
        }
        else if (tarTrans != null)
        {
            if ((Input.GetKey(pi.keyLeft) && facedir == 1) || (Input.GetKey(pi.keyRight) && facedir == -1))
            {
                StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));
            }
            else
            {
                StartCoroutine(HorizontalMove(rollspeed, 0.4f, "Roll"));
            }

        }
        else
        {
            if (Input.GetKey(pi.keyLeft) && !Input.GetKey(pi.keyRight))
            {
                SetFaceDir(-1);
            }
            else if (Input.GetKey(pi.keyRight) && !Input.GetKey(pi.keyLeft))
            {
                SetFaceDir(1);
            }
            StartCoroutine(HorizontalMove(rollspeed, 0.4f, "Roll"));
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
        ActionDisable((int)PlayerActionType.MOVE);//move
        ActionDisable((int)PlayerActionType.JUMP);//jump
        pi.SetInputDisabled("move");
        StartAttack();

        //ActionEnable(3);//roll
        //pi.jumpEnabled = false;
        //pi.moveEnabled = false;
        //pi.rollEnabled = true;

    }
    public override void OnStandardAttackExit()
    {
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.MOVE);
        ExitAttack();
        //pi.jumpEnabled = true;
        //pi.moveEnabled = true;
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);
        //anim.SetLayerWeight(anim.GetLayerIndex("Actor"), 1.0f);
        //print("Exit");
    }

    public override void OnStandardAttackConnect(AttackFromPlayer attackStat)
    {

        AlchemicGauge alchemicGauge = GameObject.Find("AlchemicGauge").GetComponent<AlchemicGauge>();
        AttackContainer container = attackStat.GetComponentInParent<AttackContainer>();

        if (container.hitConnectNum >= container.attackTotalNum)
            return;

        alchemicGauge.CPCharge(1);
        if (stat.comboHitCount > 30)
        {
            alchemicGauge.CPCharge(2);
        }



    }

    public override void OnOtherAttackConnect(AttackFromPlayer attackStat)
    {
        AlchemicGauge alchemicGauge = GameObject.Find("AlchemicGauge").GetComponent<AlchemicGauge>();

        AttackContainer container = attackStat.GetComponentInParent<AttackContainer>();

        if (container.hitConnectNum >= container.attackTotalNum)
            return;


        alchemicGauge.CPCharge(1);
        if (stat.comboHitCount > 30)
        {
            alchemicGauge.CPCharge(2);
        }

        //��������1.
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





}
