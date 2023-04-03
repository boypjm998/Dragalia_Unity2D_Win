using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.ProBuilder;


public class ActorController : ActorBase, IKnockbackable, IHumanActor
{
    public int jumptime => pi.jumptime;
    public int defaultGravity = 4;
    protected AudioManagerPlayer voiceController;
    public PlayerInput pi;
    public PlayerStatusManager _statusManager;
    public float movespeed = 6.0f;
    public float rollspeed = 9.0f;
    public float jumpforce = 20.0f;
    
    //private Vector2 movingVec;

    
    public bool dodging = false;


    [SerializeField] public bool[] isAttackSkill = new bool[4];
    public TargetAimer ta;
    private Coroutine KnockbackRoutine = null;

    public enum PlayerActionType
    {
        MOVE = 1,
        JUMP = 2,
        ROLL = 3,
        ATTACK = 4
    }


    /// <summary>
    ///   <para>横向移动判定，此方法通过FixedUpdate一直调用.</para>
    /// </summary>
    public virtual void Move()
    {
        if (pi.enabled == false)
        {
            return;
        }

        if (pi.moveEnabled == false)
        {
            return;
        }

        rigid.position += new Vector2(movespeed * (pi.isMove), 0) * Time.fixedDeltaTime;

        if (pi.directionLock == true)
        {
            return;
        }

        //if(anim.GetBool("attack") == false)
        checkFaceDir();


    }

    public void Jump()
    {
        anim.SetBool("jump", true);
    }

    public void DoubleJump()
    {
        anim.SetBool("wjump", true);
    }

    public virtual void Roll()
    {
        anim.SetBool("roll", true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
    }

    public virtual void StdAtk()
    {
        anim.SetBool("attack", true);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
        pi.InvokeAttackSignal();
    }

    public virtual void AirDashAtk()
    {
        anim.SetBool("attack", true);
        voiceController.PlayAttackVoice(0);
        //rigid.velocity.x = pi.isMove * 2* movespeed;
        pi.InvokeAttackSignal();
    }

    public virtual void UseSkill(int id)
    {
        if (isAttackSkill[id - 1])
        {
            pi.InvokeAttackSignal();
        }

        switch (id)
        {
            case 1:
                anim.Play("s1");
                _statusManager.currentSP[0] = 0;
                break;

            case 2:
                anim.Play("s2");
                _statusManager.currentSP[1] = 0;
                break;

            case 3:
                anim.Play("s3");
                _statusManager.currentSP[2] = 0;
                break;

            case 4:
                anim.Play("s4");
                _statusManager.currentSP[3] = 0;
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
    protected virtual void Awake()
    {

        pi = GetComponent<PlayerInput>();
        rigid = GetComponent<Rigidbody2D>();
        anim = rigid.GetComponentInChildren<Animator>();

        rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        facedir = 1;
        ta = gameObject.transform.parent.GetComponentInChildren<TargetAimer>();

        _statusManager = GetComponent<PlayerStatusManager>();
        jumpforce = _statusManager.jumpforce;
        movespeed = _statusManager.movespeed;
        rollspeed = _statusManager.rollspeed;

        _statusManager.OnHPBelow0 += CheckLife;

    }

    protected void CheckLife()
    {
        if (_statusManager.remainReviveTimes > 0)
        {
            _statusManager.remainReviveTimes--;
            OnRevive();
        }
        else
        {
            OnDeath();
        }
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


        //if (rigid.transform.eulerAngles.y == 0)
        //{
        //    facedir = 1;
        //}
        //else if (rigid.transform.eulerAngles.y == 180)
        //{
        //    facedir = -1;
        //}
        if (rigid.transform.localScale.x == 1)
        {
            facedir = 1;
        }
        else if (rigid.transform.localScale.x == -1)
        {
            facedir = -1;
        }


        anim.SetFloat("forward", Mathf.Abs(pi.DRight));

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


        if (pi.roll && pi.rollEnabled)
        {
            if (anim.GetBool("isGround") == true)
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

        //print(rigid.velocity);

    }

    public IEnumerator HorizontalMoveInteria(float time, float groundSpeed, float airSpeed)
    {
        while (time > 0)
        {
            if (anim.GetBool("isGround") == true)
            {
                transform.position = new Vector2(transform.position.x + facedir * groundSpeed * Time.fixedDeltaTime,
                    transform.position.y);
                //transform.position = new Vector2(transform.position.x + transform.right.x * groundSpeed * Time.fixedDeltaTime, transform.position.y); 
            }
            else
            {
                transform.position = new Vector2(transform.position.x + facedir * airSpeed * Time.fixedDeltaTime,
                    transform.position.y);
            }

            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

    }


    //主控角色在一定的时间内光滑水平位移，speed为移动速度，time为移动时间，acceration为加速度（大于0是减速）
    //参数为3个时，代表当退出某动画状态时中断位移。
    public IEnumerator HorizontalMove(float speed, float time, string move)
    {

        while (time > 0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector3(transform.position.x + facedir * speed * Time.fixedDeltaTime,
                transform.position.y, transform.position.z);
            //transform.position = new Vector2(transform.position.x+transform.right.x * speed * Time.fixedDeltaTime,transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;
            if (anim.GetCurrentAnimatorStateInfo(0).IsName(move) == false)
            {
                //print("interrupt");
                if (Mathf.Abs(rigid.velocity.x) > movespeed)
                    rigid.velocity = new Vector2(movespeed, rigid.velocity.y);
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
            //transform.right->facedir
            transform.position = new Vector2(transform.position.x + facedir * speed * Time.fixedDeltaTime,
                transform.position.y);
            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }


    }

    public IEnumerator HorizontalMove(float speed, float acceration, float time, string move)
    {
        while (time > 0)
        {
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));
            transform.position = new Vector2(transform.position.x + facedir * speed * Time.fixedDeltaTime,
                transform.position.y);
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
            //Debug.Log(anim.GetCurrentAnimatorClipInfo(0)[0].clip.name.Equals(move));trsfm.right->facedir
            transform.position = new Vector2(transform.position.x + facedir * speed * Time.fixedDeltaTime,
                transform.position.y);
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
        switch (pi.buttonLeft.IsPressing)
        {
            //if (pi.DRight > 0.01f) 
            case false when pi.buttonRight.IsPressing:
                //rigid.transform.eulerAngles = new Vector3(0, 0, 0);
                SetFaceDir(1);
                break;
            //else if (pi.DRight < -0.01f)
            case true when !pi.buttonRight.IsPressing:
                //rigid.transform.eulerAngles = new Vector3(0, 180, 0);
                SetFaceDir(-1);
                break;
        }
    }

    public void SetFaceDir(int dir)
    {
        facedir = dir;

        if (dir == 1)
            rigid.transform.localScale = new Vector3(1, 1, 1);
        //rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        else if (dir == -1)
            rigid.transform.localScale = new Vector3(-1, 1, 1);
        //rigid.transform.eulerAngles = new Vector3(0, 180, 0);

        facedir = dir;
    }

    #endregion

    #region Animation _statusManageres Events

    //人物滚动时附加的位移效果
    public virtual void EventRoll()
    {
        if (pi.buttonLeft.IsPressing && !pi.buttonRight.IsPressing)
        {
            SetFaceDir(-1);
        }
        else if (!pi.buttonLeft.IsPressing && pi.buttonRight.IsPressing)
        {
            SetFaceDir(1);
        }

        StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));

    }

    public virtual void EventDash()
    {
        //SetVelocity(facedir*rollspeed,rigid.velocity.y);
        StartCoroutine(HorizontalMove(rollspeed * 3f, 10.0f, 0.1f, "dash"));

    }

    public virtual void InertiaMove(float time)
    {
        float speedrate = anim.GetFloat("forward");

        if (speedrate < 0.1f && anim.GetBool("isGround"))
            speedrate = 0.5f;

        StartCoroutine(HorizontalMoveInteria(time, 1.8f * movespeed * speedrate, 1.5f * movespeed * speedrate));

    }

    protected virtual IEnumerator InvincibleRoutine()
    {
        //var renderer = GetComponent<SpriteRenderer>();

        var hitsensor = transform.Find("HitSensor").GetComponent<Collider2D>();
        //renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 0.5f);
        hitsensor.enabled = false;
        _statusManager.HPRegenImmediatelyWithoutRandom(0, 100);
        _statusManager.currentHp = _statusManager.maxHP;


        yield return new WaitForSeconds(3f);

        //renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, 1);
        hitsensor.enabled = true;

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
        //print("onJumpExit");
    }

    public void IsGround()
    {
        //print("GROUND:"+transform.position.x);

        anim.SetBool("isGround", true);

        pi.jumptime = 2;
        pi.rollEnabled = true;
    }

    public void isNotGround()
    {
        //print(transform.position.x);
        pi.rollEnabled = false;
        anim.SetBool("isGround", false);
    }

    public virtual void onRollEnter()
    {
        pi.attackEnabled = false;
        pi.jumpEnabled = false;
        pi.moveEnabled = false;
        voiceController?.PlayDodgeVoice();

        dodging = true;
    }

    public virtual void onRollExit()
    {
        pi.attackEnabled = true;
        pi.jumpEnabled = true;
        pi.moveEnabled = true;
        //pi.rollEnabled = true;
        dodging = false;

        anim.SetBool("roll", false);
        pi.SetInputEnabled("move");
        //Debug.Log("ExitRoll");
    }

    public void OnFall()
    {
        //Debug.Log("OnfallEnter");
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
        ActionEnable((int)PlayerActionType.MOVE); //move
        ActionEnable((int)PlayerActionType.JUMP); //jump
        ActionEnable((int)PlayerActionType.ROLL); //roll
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
        dodging = true;

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
        dodging = false;
        ActionEnable((int)PlayerActionType.MOVE); //move
        ActionEnable((int)PlayerActionType.JUMP); //jump
        ActionEnable((int)PlayerActionType.ROLL); //roll
        ActionEnable((int)PlayerActionType.ATTACK);
        pi.SetInputEnabled("move");
    }

    public virtual void OnGravityWeaken()
    {
        rigid.gravityScale = 1;
        SetVelocity(rigid.velocity.x, 0);

    }

    public virtual void OnGravityRecover()
    {
        rigid.gravityScale = defaultGravity;
    }

    public void OnHurtEnter()
    {
        OnAttackInterrupt?.Invoke();

        pi.SetInputDisabled("roll");
        pi.SetInputDisabled("jump");
        pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");
        pi.directionLock = false;
        pi.isSkill = false;
        rigid.gravityScale = 1;
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);
        //SetVelocity(rigid.velocity.x,0);
        anim.speed = 1;
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {
            meeles.GetChild(i).GetComponent<AttackContainer>().DestroyInvoke();
        }

        voiceController.PlayHurtVoice(_statusManager);
        transform.GetChild(0).GetComponentInChildren<AnimationEventSender>()?.ChangeFaceExpression(0.75f);

    }

    public void OnHurtExit()
    {
        pi.SetInputEnabled("roll");
        pi.SetInputEnabled("jump");
        pi.SetInputEnabled("attack");
        pi.SetInputEnabled("move");
        pi.directionLock = false;
        rigid.gravityScale = defaultGravity;
        ActionEnable((int)PlayerActionType.MOVE);
        ActionEnable((int)PlayerActionType.JUMP);
        ActionEnable((int)PlayerActionType.ROLL);
        ActionEnable((int)PlayerActionType.ATTACK);
        anim.speed = 1;
        transform.GetChild(0).GetComponentInChildren<AnimationEventSender>()?.ChangeFaceExpression();
    }

    protected virtual void OnRevive()
    {
        _statusManager.ResetAllStatus();
        _statusManager.ClearSP();
        BattleEffectManager effectManager = FindObjectOfType<BattleEffectManager>();
        effectManager.PlayReviveSoundEffect();
        BattleEffectManager.BWEffect();
        effectManager.SpawnReviveEffect(gameObject);
        StartCoroutine(InvincibleRoutine());
        _statusManager.waitForRevive = false;
    }

    protected virtual void OnDeath()
    {
        pi.SetInputDisabled("roll");
        pi.SetInputDisabled("jump");
        pi.SetInputDisabled("attack");
        pi.SetInputDisabled("move");

        pi.isSkill = false;
        rigid.gravityScale = defaultGravity;
        ActionDisable((int)PlayerActionType.MOVE);
        ActionDisable((int)PlayerActionType.JUMP);
        ActionDisable((int)PlayerActionType.ROLL);
        ActionDisable((int)PlayerActionType.ATTACK);

        OnAttackInterrupt?.Invoke();


        _statusManager.ResetAllStatusForced();
        _statusManager.enabled = false;
        transform.Find("HitSensor").GetComponent<Collider2D>().enabled = false;
        pi.enabled = false;
        var enemies = FindObjectsOfType<DragaliaEnemyBehavior>();
        foreach (var enemy in enemies)
        {
            enemy.playerAlive = false;
        }

        pi.hurt = false;
        anim.speed = 1;
        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
            KnockbackRoutine = null;
        }

        SetVelocity(0, 0);

        //播放死亡动画

        StartCoroutine(DeathRoutine());

        //this.enabled = false;


    }

    protected IEnumerator DeathRoutine()
    {

        pi.enabled = false;
        pi.moveEnabled = false;
        pi.inputMoveEnabled = false;

        yield return null;


        anim.Play("die");
        anim.SetFloat("forward", 0);

        _statusManager.enabled = false;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);

        //anim.speed = 0;

        var _statusManagers = FindObjectsOfType<PlayerStatusManager>();

        foreach (var _statusManagerus in _statusManagers)
        {
            if (_statusManagerus.currentHp > 0 || _statusManagerus.remainReviveTimes > 0)
                yield break;
        }

        GlobalController.BattleFinished(false);

    }

    protected void SetAnimSpeed(float percentage)
    {
        anim.speed = percentage;
    }


    #endregion

    


    public override void TakeDamage(float kbPower, float kbtime, float kbForce, Vector2 kbDir)
    {
        if (_statusManager.knockbackRes >= 100)
            return;

        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
        }

        KnockbackRoutine = StartCoroutine(KnockBackEffect(kbtime, kbForce, kbDir));
        //print("Hurt");
        //Unimplemented

    }

    IEnumerator KnockBackEffect(float time, float force, Vector2 kbDir)
    {
        kbDir = kbDir.normalized;
        pi.hurt = true;
        //anim.SetBool("hurt",true);
        SetVelocity(force * kbDir.x, force * kbDir.y);

        var totalTime = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            rigid.drag += Time.deltaTime * 2 / totalTime;
            if (totalTime - time > 0.3f && rigid.gravityScale < defaultGravity)
            {
                rigid.gravityScale += (Time.deltaTime * 4);

            }

            yield return null;

        }

        rigid.drag = 0;



        SetVelocity(0, rigid.velocity.y);
        //anim.SetBool("hurt",false);
        pi.hurt = false;
        KnockbackRoutine = null;
    }








    ///单独行动指令的开关
    ///0:全部,1:移动，2:跳跃，3:翻滚，4:攻击
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
            pi.SetAttackEnabled();
        }
    }

    ///0:全部,1:移动，2:跳跃，3:翻滚，4:攻击
    public void ActionDisable(int type)
    {

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
            pi.SetAttackDisabled();
        }
    }

    public virtual void FaceDirectionAutoFix(int moveID)
    {

    }

    protected virtual void CheckSignal()
    {
        if (anim.GetBool("attack"))
        {
            if (anim.GetBool("isGround"))
            {
                StdAtk();
            }
            else AirDashAtk();
        }
    }

    public bool GetDodge()
    {
        return dodging;
    }



}


    