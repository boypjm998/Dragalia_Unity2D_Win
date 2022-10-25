using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController_c001 : ActorController
{


   

    public override void UseSkill(int id)
    {

        switch (id)
        {
            case 1:
                anim.Play("s1");
                stat.currentSP[0] = 0;
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

    }

    // Update is called once per frame
    void Update()
    {


        if (rigid.transform.eulerAngles.y == 0)
        {
            facedir = 1;
        }
        else if (rigid.transform.eulerAngles.y == 180)
        {
            facedir = -1;
        }
        anim.SetFloat("forward", Mathf.Abs(pi.DRight));//动画的渐进效果
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
        if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(1);
        }
        if (pi.roll && pi.rollEnabled)
        {
            Roll();


        }
        //movingVec = rigid.transform.forward;
        //print(movingVec);
    }
    void FixedUpdate()
    {
        Move();



    }

    //Event functions and Setting functions


    //设置主控角色的速度

    #region Move Horizontally
    
    #endregion

    #region Animation States Events

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
        if (targetTransform.position.x < rigid.transform.position.y)
        {
            if (facedir == -1)
                return false;
            else return true;
        }
        return false;

    }





}
