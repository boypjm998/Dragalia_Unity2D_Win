using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour
{
    
    public PlayerInput pi;
    public StatusManager stat;
    public float movespeed = 6.0f;
    public float rollspeed = 9.0f;
    public float jumpforce = 18.0f;
    [SerializeField]
    private Animator anim; 
    public Rigidbody2D rigid;
    //private Vector2 movingVec;
    [SerializeField]
    public int facedir = 1;
    
    [SerializeField]
    private TargetAimer ta;
    
    enum PlayerActionType
    {
        MOVE = 1,
        JUMP = 2,
        ROLL = 3,
        ATTACK = 4
    }
    private bool curValue;
    private bool newValue;


    public void InputClearBoolSignal(string s)
    {
        anim.SetBool(s, false);
    }
    private void Move()
    {
        if (pi.moveEnabled == false)
        {
            return;
        }

        rigid.position += new Vector2(movespeed * (pi.isMove), 0) * Time.fixedDeltaTime;

        if (pi.directionLock == true)
        { return; }
        
        //if(anim.GetBool("attack") == false)
        checkFaceDir();

        
    }
    private void Jump()
    {
        anim.SetBool("jump",true);   
    }
    private void DoubleJump()
    {
        anim.SetBool("wjump", true);
    }
    private void Roll()
    {
        anim.SetBool("roll", true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }
    private void StdAtk()
    {
        anim.SetBool("attack",true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }
    private void AirDashAtk()
    {
        anim.SetBool("attack", true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }
    private void UseSkill(int id)
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
    public void ClearFloatSignal(string varname)
    {
        
        anim.SetFloat(varname, 0f);

        

    }

    public void ClearBoolSignal(string varname)
    {

        anim.SetBool(varname, false);

    }

    public void StartAttack()
    {
        anim.SetBool("isAttack", true);
    }
    public void ExitAttack()
    {
        anim.SetBool("isAttack", false);
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

        curValue = true;
        newValue = true;
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
            else {
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
        //checkObstacles();
        
       
    }

    //Event functions and Setting functions


    //设置主控角色的速度

    #region Move Horizontally
    public void SetVelocity(float vx, float vy)
    {
        rigid.velocity = new Vector2(vx, vy);

        print(rigid.velocity);

    }

    private IEnumerator HorizontalMoveInteria(float time, float groundSpeed,float airSpeed)
    {
        while (time > 0)
        {
            if (anim.GetBool("isGround") == true)
            { 
                transform.position = new Vector2(transform.position.x + transform.right.x * groundSpeed * Time.fixedDeltaTime, transform.position.y); 
            }
            else { 
                transform.position = new Vector2(transform.position.x + transform.right.x * airSpeed * Time.fixedDeltaTime, transform.position.y);
            }
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

    }

    //主控角色在一定的时间内光滑水平位移，speed为移动速度，time为移动时间，acceration为加速度（大于0是减速）
    //参数为3个时，代表当退出某动画状态时中断位移。
    private IEnumerator HorizontalMove(float speed,float time,string move)
    {
        
        while(time>0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector2(transform.position.x+transform.right.x * speed * Time.fixedDeltaTime,transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;           
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move)==false)        
            {               
                if (rigid.velocity.x > movespeed)
                    rigid.velocity = new Vector2(movespeed,rigid.velocity.y);
                //pi.SetMoveEnabled();
                yield break;
            }
            yield return new WaitForFixedUpdate();
        }
        

    }
    private IEnumerator HorizontalMove(float speed, float time)
    {

        while (time > 0)
        {
            
            transform.position = new Vector2(transform.position.x + transform.right.x * speed * Time.fixedDeltaTime, transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            
            yield return new WaitForFixedUpdate();
        }


    }
    private IEnumerator HorizontalMove(float speed,float acceration, float time, string move)
    {
        while (time > 0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector2(transform.position.x + transform.right.x * speed * Time.fixedDeltaTime, transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            if (anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move) == false)
            {
                if (rigid.velocity.x > movespeed)
                    rigid.velocity = new Vector2(movespeed, rigid.velocity.y); 
                pi.SetMoveEnabled();
                yield break;
            }
            speed -= acceration * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


    }
    private IEnumerator HorizontalMove(float speed, float acceration, float time)
    {
        while (time > 0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector2(transform.position.x + transform.right.x * speed * Time.fixedDeltaTime, transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            
            speed -= acceration * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


    }

    public void ResetMovement()
    {
        pi.DRight = 0;

    }


    public void checkFaceDir()
    {
        
        if (pi.DRight > 0.05f)
        {   
            rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        }
            
        else if (pi.DRight < -0.05f)
        {
            
            rigid.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
    public void SetFaceDir(int dir)
    {
        print(dir+"??");
        if (dir==1)
            rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        else if (dir==-1)
            rigid.transform.eulerAngles = new Vector3(0, 180, 0);
    }
    #endregion

    #region Animation States Events

    //人物滚动时附加的位移效果
    public void EventRoll()
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

                StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));

            }

            else if (tarx > rigid.position.x)
            {
                SetFaceDir(1);


                StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));

            }

            //StartCoroutine(HorizontalMove(-rollspeed, 0.4f, "Roll"));
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
        else {
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
    public void EventDash()
    {
        //SetVelocity(facedir*rollspeed,rigid.velocity.y);
        StartCoroutine(HorizontalMove(rollspeed*3f, 10.0f, 0.1f, "dash"));

    }
    public void InertiaMove(float time)
    {
        float speedrate = anim.GetFloat("forward");
        
        StartCoroutine(HorizontalMoveInteria(time ,1.8f * movespeed * speedrate, 1.5f * movespeed * speedrate));
        
    }
    #endregion

    #region Messages Process Moudles
    private void onJumpEnter()
    {
        //print("onJump"); 
        rigid.velocity = new Vector2(rigid.velocity.x, jumpforce);
        //pi.rollEnabled = false;
        anim.SetBool("jump", false);
    }

    private void onDoubleJumpEnter()
    {
        //print("onDoubleJump");
        rigid.velocity = new Vector2(rigid.velocity.x, jumpforce);
        anim.SetBool("wjump", false);
        //pi.rollEnabled = false;
    }
    private void onJumpExit()
    {
        print("onJumpExit");
    }
    public void IsGround()
    {
        //print("isGround");
        anim.SetBool("isGround", true);
        //anim.SetBool("wjump", false);
        //anim.SetBool("jump", true);
        pi.jumptime = 2;
        pi.rollEnabled = true;
    }
    public void isNotGround()
    {
        //print("isNotGround");
        pi.rollEnabled = false;
        anim.SetBool("isGround", false);
    }
    private void onRollEnter() 
    {
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.moveEnabled = false;
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);
        //SetVelocity(movespeed * 2.5f* facedir, 0);
        Debug.Log("EnterRoll");
    }
    private void onRollExit()
    {
        pi.attackEnabled = true;
        pi.jumpEnabled = true;
        pi.moveEnabled = true;
        //if(anim.GetBool("attack")==false)
            //checkFaceDir(); 
        anim.SetBool("roll", false);
        Debug.Log("ExitRoll");
    }
    public void OnFall()
    {
        Debug.Log("OnfallEnter");
        pi.SetJumpEnabled();
        pi.SetRollDisabled();
    }


    private void OnDashEnter()
    {
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);
        pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        
        //Debug.Log("OndashEnter");
    }
    private void OnDashExit()
    {
        ActionEnable((int)PlayerActionType.MOVE);//move
        ActionEnable((int)PlayerActionType.JUMP);//jump
        ActionEnable((int)PlayerActionType.ROLL);//roll
        ActionEnable((int)PlayerActionType.ATTACK);
        anim.SetBool("attack", false);
    }


    private void OnStandardAttackEnter()
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
    private void OnStandardAttackExit()
    {
        ActionEnable(2);
        ActionEnable(1);
        ExitAttack();
        //pi.jumpEnabled = true;
        //pi.moveEnabled = true;
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);
        //anim.SetLayerWeight(anim.GetLayerIndex("Actor"), 1.0f);
        //print("Exit");
    }

    private void OnSkillEnter()
    {
        pi.isSkill = true;
        pi.directionLock = false;

        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);
        //pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        print("skillEnter");
        
    }

    private void OnSkillExit()
    {
        pi.isSkill = false;
        ActionEnable((int)PlayerActionType.MOVE);//move
        ActionEnable((int)PlayerActionType.JUMP);//jump
        ActionEnable((int)PlayerActionType.ROLL);//roll
        ActionEnable((int)PlayerActionType.ATTACK);
        pi.SetInputEnabled("move");
    }


    #endregion


    //单独行动指令的开关
    public void ActionEnable(int type)
    {
        //0:全部,1:移动，2:跳跃，3:翻滚，4:攻击
        if (type == 0)
        {
            pi.SetAttackEnabled();
            pi.SetJumpEnabled();
            pi.SetRollEnabled();
            pi.SetMoveEnabled();

        }
        else if (type == 1)
        {
            pi.SetMoveEnabled();
        }
        else if (type == 2)
        {
            pi.SetJumpEnabled();
        }
        else if (type == 3)
        {
            pi.SetRollEnabled();
        }
        else if (type == 4)
        {
            pi.SetMoveEnabled();
        }
    }

    public void ActionDisable(int type)
    {
        //0:全部,1:移动，2:跳跃，3:翻滚，4:攻击
        if (type == 0)
        {
            pi.SetAttackDisabled();
            pi.SetJumpDisabled();
            pi.SetRollDisabled();
            pi.SetMoveDisabled();

        }
        else if (type == 1)
        {
            pi.SetMoveDisabled();
        }
        else if (type == 2)
        {
            pi.SetJumpDisabled();
        }
        else if (type == 3)
        {
            pi.SetRollDisabled();
        }
        else if (type == 4)
        {
            pi.SetMoveDisabled();
        }
    }

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


    