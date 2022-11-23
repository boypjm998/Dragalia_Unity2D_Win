using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;


public class ActorController : MonoBehaviour
{
    
    public PlayerInput pi;
    public PlayerStatusManager stat;
    public float movespeed = 6.0f;
    public float rollspeed = 9.0f;
    public float jumpforce = 18.0f;
    public Animator anim; 
    public Rigidbody2D rigid;
    //private Vector2 movingVec;
    
    public int facedir = 1;
    
    
    public TargetAimer ta;
    
    public enum PlayerActionType
    {
        MOVE = 1,
        JUMP = 2,
        ROLL = 3,
        ATTACK = 4
    }


    
    public virtual void Move()
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
    public void Jump()
    {
        anim.SetBool("jump",true);   
    }
    public void DoubleJump()
    {
        anim.SetBool("wjump", true);
    }
    public void Roll()
    {
        anim.SetBool("roll", true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }
    public void StdAtk()
    {
        anim.SetBool("attack",true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }
    public void AirDashAtk()
    {
        anim.SetBool("attack", true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }
    public virtual void UseSkill(int id)
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
            
            case 3:
                anim.Play("s3");
                stat.currentSP[2] = 0;
                break;
            
            case 4:
                anim.Play("s4");
                stat.currentSP[3] = 0;
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


    }

    protected virtual void CheckSkill()
    {
        if (pi.skill[0] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(1);
        }

        if (pi.skill[1] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(2);
        }
        
        if (pi.skill[2] && anim.GetBool("isGround") && !pi.hurt && !pi.isSkill)
        {
            UseSkill(3);
        }
        
        if (pi.skill[3] && !pi.hurt && !pi.isSkill)
        {
            UseSkill(4);
        }
        
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
  

        if (rigid.transform.eulerAngles.y == 0)
        {
            facedir = 1;
        }
        else if (rigid.transform.eulerAngles.y == 180)
        {
            facedir = -1;
        }
        anim.SetFloat("forward", Mathf.Abs(pi.DRight));
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


    //设置主控角色的速度。

    #region Move Horizontally
    public void SetVelocity(float vx, float vy)
    {
        rigid.velocity = new Vector2(vx, vy);

        print(rigid.velocity);

    }

    public IEnumerator HorizontalMoveInteria(float time, float groundSpeed,float airSpeed)
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
    public IEnumerator HorizontalMove(float speed,float time,string move)
    {
        
        while(time>0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector2(transform.position.x+transform.right.x * speed * Time.fixedDeltaTime,transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(move) == false)
            {
                print("interrupt");
                if (Mathf.Abs(rigid.velocity.x) > movespeed)
                    rigid.velocity = new Vector2(movespeed,rigid.velocity.y);
                //pi.SetMoveEnabled();
                yield break;
            }
            
            yield return new WaitForFixedUpdate();
        }
        

    }
    public IEnumerator HorizontalMove(float speed, float time)
    {

        while (time > 0)
        {
            
            transform.position = new Vector2(transform.position.x + transform.right.x * speed * Time.fixedDeltaTime, transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            
            yield return new WaitForFixedUpdate();
        }


    }
    public IEnumerator HorizontalMove(float speed,float acceration, float time, string move)
    {
        while (time > 0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector2(transform.position.x + transform.right.x * speed * Time.fixedDeltaTime, transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(move) == false)
            {
                if (Mathf.Abs(rigid.velocity.x) > movespeed)
                    rigid.velocity = new Vector2(movespeed, rigid.velocity.y); 
                
                yield break;
            }
            speed -= acceration * Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


    }
    public IEnumerator HorizontalMove(float speed, float acceration, float time)
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
        
        if (pi.DRight > 0.01f)
        {   
            rigid.transform.eulerAngles = new Vector3(0, 0, 0);
            facedir = 1;
        }
            
        else if (pi.DRight < -0.01f)
        {
            
            rigid.transform.eulerAngles = new Vector3(0, 180, 0);
            facedir = -1;
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
    public virtual void EventRoll()
    {

        StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));  

    }
    public virtual void EventDash()
    {
        //SetVelocity(facedir*rollspeed,rigid.velocity.y);
        StartCoroutine(HorizontalMove(rollspeed*3f, 10.0f, 0.1f, "dash"));

    }
    public virtual void InertiaMove(float time)
    {
        float speedrate = anim.GetFloat("forward");
        
        StartCoroutine(HorizontalMoveInteria(time ,1.8f * movespeed * speedrate, 1.5f * movespeed * speedrate));
        
    }




    #endregion

    #region Messages Process Moudles
    public void onJumpEnter()
    {
        //print("onJump"); 
        rigid.velocity = new Vector2(rigid.velocity.x, jumpforce);
        //pi.rollEnabled = false;
        anim.SetBool("jump", false);
    }

    public void onDoubleJumpEnter()
    {
        //print("onDoubleJump");
        rigid.velocity = new Vector2(rigid.velocity.x, jumpforce);
        anim.SetBool("wjump", false);
        //pi.rollEnabled = false;
    }
    public void onJumpExit()
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
    public void onRollEnter() 
    {
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.moveEnabled = false;
        //anim.SetLayerWeight(anim.GetLayerIndex("Attack Layer"), 0);
        //SetVelocity(movespeed * 2.5f* facedir, 0);
        Debug.Log("EnterRoll");
    }
    public void onRollExit()
    {
        pi.attackEnabled = true;
        pi.jumpEnabled = true;
        pi.moveEnabled = true;
        //if(anim.GetBool("attack")==false)
            //checkFaceDir(); 
        anim.SetBool("roll", false);
        pi.SetInputEnabled("move");
        Debug.Log("ExitRoll");
    }
    public void OnFall()
    {
        Debug.Log("OnfallEnter");
        pi.SetJumpEnabled();
        pi.SetRollDisabled();
    }


    public virtual void OnDashEnter()
    {
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);
        pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        
        //Debug.Log("OndashEnter");
    }
    public virtual void OnDashExit()
    {
        ActionEnable((int)PlayerActionType.MOVE);//move
        ActionEnable((int)PlayerActionType.JUMP);//jump
        ActionEnable((int)PlayerActionType.ROLL);//roll
        ActionEnable((int)PlayerActionType.ATTACK);
        anim.SetBool("attack", false);
    }


    public virtual void OnStandardAttackEnter()
    {
     
       
    }
    public virtual void OnStandardAttackExit()
    {

    }

    

    public virtual void OnSkillEnter()
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

    public virtual void OnSkillExit()
    {
        pi.isSkill = false;
        ActionEnable((int)PlayerActionType.MOVE);//move
        ActionEnable((int)PlayerActionType.JUMP);//jump
        ActionEnable((int)PlayerActionType.ROLL);//roll
        ActionEnable((int)PlayerActionType.ATTACK);
        pi.SetInputEnabled("move");
    }

    public virtual void OnGravityWeaken()
    {
        rigid.gravityScale = 1;
        SetVelocity(rigid.velocity.x,0);
    }
    
    public virtual void OnGravityRecover()
    {
        rigid.gravityScale = 4;
    }

    protected void OnHurtEnter()
    {
        pi.SetInputDisabled("roll");
        pi.SetInputDisabled("jump");
        pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        pi.directionLock = false;
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);
        SetVelocity(rigid.velocity.x,0);
        anim.speed = 1;

    }
    
    protected void OnHurtExit()
    {
        pi.SetInputEnabled("roll");
        pi.SetInputEnabled("jump");
        pi.SetInputEnabled("attack");
        pi.SetInputEnabled("move");
        pi.directionLock = false;
        ActionEnable((int)PlayerActionType.MOVE);
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.ROLL);
        ActionEnable((int)PlayerActionType.ATTACK);
        anim.speed = 1;

    }

    protected void SetAnimSpeed(float percentage)
    {
        anim.speed = percentage;
    }


    #endregion

    #region 攻击返回指令

    public virtual void OnStandardAttackConnect() { }
    public virtual void OnSkillConnect() { }
    public virtual void OnForceConnect() { }
    public virtual void OnOtherAttackConnect() { }

    public virtual void OnStandardAttackConnect(AttackFromPlayer attackStat) { }
    public virtual void OnSkillConnect(AttackFromPlayer attackStat) { }
    public virtual void OnForceConnect(AttackFromPlayer attackStat) { }
    public virtual void OnOtherAttackConnect(AttackFromPlayer attackStat) { }


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

    



}


    