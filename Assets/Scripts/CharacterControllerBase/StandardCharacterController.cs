using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using GameMechanics;
public class StandardCharacterController : ActorBase , IKnockbackable, IHumanActor
{
    public GameObject rendererObject;
    public GameObject weaponObject; //need SeriazeField
    
    
    public float _defaultgravityscale = 4;
    public float movespeed = 7.0f;
    public float rollspeed = 10.0f;
    public float jumpforce = 20.0f;
    protected int jumpTime = 2;
    public int JumpTime => jumpTime;
    public float jumpHeight => jumpforce*jumpforce/(_defaultgravityscale*20);
    public float jumpAscentTime => jumpforce/(10*_defaultgravityscale);
    
    //public int facedir = 1;
    [SerializeField] protected float isMove = 0;
    public float IsMove => isMove;
    public bool hurt;
    public bool moveEnable;
    public bool attackEnable = true;
    public bool dodging = false;
    protected bool isAction;
    public int combo = 0;
    
    
    //public Animator anim;
    //public Rigidbody2D rigid;
    public StandardGroundSensor _groundSensor;
    protected StatusManager _statusManager;
    protected BattleEffectManager _effectManager;
    protected NpcController _controller;
    
    
    protected Coroutine hurtEffectCoroutine;
    protected Coroutine KnockbackRoutine;
    public Coroutine SubMoveRoutine;
    
    public delegate void OnTask(bool success);
    //public delegate void OnHurt();
    
    //public event OnHurt OnAttackInterrupt;

    protected AttackManager MoveManager;

    protected override void Awake()
    {
        base.Awake();
        rendererObject = transform.Find("Model").GetChild(0).Find("model").gameObject;
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponentInChildren<Rigidbody2D>();
        
        //MoveManager = GetComponent<EnemyMoveManager>();
        if(!_groundSensor)
            _groundSensor = GetComponentInChildren<StandardGroundSensor>();
        _statusManager = GetComponent<StatusManager>();
        _controller = GetComponent<NpcController>();
        //_behavior = GetComponent<DragaliaEnemyBehavior>();
        
    }

    protected virtual void Start()
    {
        _statusManager.OnHPBelow0 += CheckLife;
        //currentKBRes = _statusManager.knockbackRes;
        //debugRes = currentKBRes;

        _groundSensor.IsGround += GroundCheck;

    }

    protected void OnDestroy()
    {
        _groundSensor.IsGround -= GroundCheck;
    }

    protected void Update()
    {
        if (hurt)
        {
            anim.SetBool("hurt", true);
        }
        else
        {
            anim.SetBool("hurt", false);
        }

        anim.SetFloat("forward", isMove);

        // if (attackEnable && anim.GetBool("attack"))
        // {
        //     if (anim.GetBool("isGround") == true)
        //         StdAtk();
        //     else
        //     {
        //         AirDashAtk();
        //     }
        // }




    }

    protected void FixedUpdate()
    {
        Move();
    }

    void Move()
    {
        if(!moveEnable)
            return;
        rigid.position += new Vector2(movespeed * facedir * isMove, 0) * Time.fixedDeltaTime;
        //print(6 * facedir * isMove * Time.fixedDeltaTime);
        CheckFaceDir();
    }
    
    public void Jump()
    {
        if (jumpTime == 1)
        {
            anim.Play("jump2");
            SetVelocity(rigid.velocity.x,jumpforce);
            jumpTime--;
        }else if (jumpTime == 2)
        {
            SetVelocity(rigid.velocity.x,jumpforce);
            jumpTime--;
            anim.Play("jump");
            
        }
    }
    
    public void TurnMove(GameObject target)
    {
        if (target.transform.position.x > transform.position.x)
        {
            SetFaceDir(1);
        }
        if (target.transform.position.x < transform.position.x)
        {
            SetFaceDir(-1);
        }
        
    }

    public void TurnMove(Vector2 position)
    {
        if(position.x > transform.position.x)
        {
            SetFaceDir(1);
        }
        if(position.x < transform.position.x)
        {
            SetFaceDir(-1);
        }
    }

    public void StartMove(int flag)
    {
        isMove = flag;
    }

    public bool CheckTargetDistance(GameObject target, float x, float y)
    {
        if (Mathf.Abs(target.transform.position.x - transform.position.x ) > x)
        {
            return false;
        }
        if (Mathf.Abs(target.transform.position.y - transform.position.y ) > y)
        {
            return false;
        }

        return true;
    }


    /// <summary>
    /// 检查目标是否和自己在同一个平面
    /// </summary>
    /// <returns>返回0为在同一平面，1为目标高于自身，-1为目标低于自身</returns>
    public int CheckTargetStandOnSameGround(GameObject target)
    {
        GameObject myGround;

        myGround = _groundSensor.GetCurrentAttachedGroundInfo();
        
        if(myGround==null)
        {
            RaycastHit2D myRayL = 
                Physics2D.Raycast(transform.position + new Vector3(-1,0,0), Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
            RaycastHit2D myRayR = 
                Physics2D.Raycast(transform.position + new Vector3(1,0,0), Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
        
            var myGround1 = myRayL.collider.gameObject;
            var myGround2 = myRayR.collider.gameObject;
            if(myGround1 == myGround2)
                myGround = myGround1;
            else
                myGround = myGround1.transform.position.y > myGround2.transform.position.y ? myGround1 : myGround2;
            
        }

        GameObject tarGround;
        var targetSensor = target.GetComponentInChildren<IGroundSensable>();
        if (targetSensor == null)
        {
            targetSensor = target.transform.parent.GetComponentInChildren<IGroundSensable>();
        }

        if(targetSensor.GetCurrentAttachedGroundInfo()!=null)
        {
            tarGround = targetSensor.GetCurrentAttachedGroundInfo();
        }else{
            RaycastHit2D tarRayL = 
                Physics2D.Raycast(target.transform.position + new Vector3(-1,0,0), Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
            RaycastHit2D tarRayR = 
                Physics2D.Raycast(target.transform.position + new Vector3(1,0,0), Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
        
            var tarGround1 = tarRayL.collider?.gameObject;
            var tarGround2 = tarRayR.collider?.gameObject;
            var nullItem = 0;
            if (tarGround1 == null)
            {
                tarGround = tarGround2;
                nullItem = 1;
            }
            if(tarGround2 == null)
            {
                tarGround = tarGround1;
                nullItem = 2;
            }

            if (tarGround1 == tarGround2)
            {
                tarGround = tarGround1;
            }else if(nullItem == 0){
                tarGround = tarGround1.transform.position.y > tarGround2.transform.position.y ? tarGround1 : tarGround2;
            }
            else
            {
                tarGround=nullItem==1?tarGround2:tarGround1;
            }
        }


        if (tarGround == myGround)
        {
            return 0;
        }
        
        
        else if (tarGround.transform.position.y > myGround.transform.position.y)
        {
            return 1;
        }
        else if (tarGround.transform.position.y < myGround.transform.position.y)
        {
            return -1;
        }
        else
        {
            Debug.LogWarning("Error Value");
            return 999;
        }


    }
    


    protected virtual IEnumerator KnockBackEffect(float time,float force, Vector2 kbDir)
    {
        SetVelocity(0,0);
        kbDir = kbDir.normalized;
        hurt = true;
        anim.SetBool("hurt",true);
        SetVelocity(force * kbDir.x,force * kbDir.y);
        //print(kbDir);
        var totalTime = time;
        while (time > 0)
        {
            time -= Time.deltaTime;
            rigid.drag += Time.deltaTime * 2 / totalTime;
            if (totalTime - time > 0.3f && rigid.gravityScale < _defaultgravityscale)
            {
                rigid.gravityScale += (Time.deltaTime * _defaultgravityscale);
                
            }
            yield return null;
            
        }

        rigid.drag = 0;
        SetVelocity(0,rigid.velocity.y);
        anim.SetBool("hurt",false);
        hurt = false;
        KnockbackRoutine = null;
    }
    
    public void SetVelocity(float vx, float vy)
    {
        rigid.velocity = new Vector2(vx, vy);

        //print(rigid.velocity);

    }

    /// <summary>
    ///   <para>人形敌人受到伤害（此方法最好通过BattleStageManager来调用）</para>
    /// </summary>
    public override void TakeDamage(float kbPower, float kbtime,float kbForce, Vector2 kbDir)
    {
        if(_statusManager.knockbackRes >= 100)
            return;
        
        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
        }
        KnockbackRoutine = StartCoroutine(KnockBackEffect(kbtime,kbForce,kbDir));
        //print("Hurt");
        //Unimplemented

    }
    public override void TakeDamage(AttackBase attackBase, Vector2 kbdir)
    {
        if (_statusManager.knockbackRes >= 100)
            return;
        
        var kbtime = attackBase.attackInfo[0].knockbackTime;
        var kbForce = attackBase.attackInfo[0].knockbackForce;
        var kbPower = attackBase.attackInfo[0].knockbackPower;
        var random = Random.Range(0, 100);
        if(random > kbPower-_statusManager.KnockbackRes)
        {
            return;
        }

        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
        }

        KnockbackRoutine = StartCoroutine(KnockBackEffect(kbtime, kbForce, kbdir));
    }

    

    protected void SetAnimSpeed(float percentage)
    {
        anim.speed = percentage;
    }
    
    public virtual void OnHurtEnter()
    {
        
        OnAttackInterrupt?.Invoke();
        
        if (SubMoveRoutine != null)
        {
            StopCoroutine(SubMoveRoutine);
            SubMoveRoutine = null;
        }
        
        

        // if (_behavior.currentAttackAction != null)
        // {
        //     _behavior.StopCoroutine(_behavior.currentAttackAction);
        //     _behavior.currentAttackAction = null;
        // }

        rigid.gravityScale = 1;
        //SetVelocity(rigid.velocity.x,0);
        moveEnable = false;
        anim.speed = 1;
        //MoveManager.SetGroundCollider(true);
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {

            //meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }
        
        transform.GetChild(0).GetComponentInChildren<AnimationEventSender>()?.ChangeFaceExpression(0.75f);
        

    }
    
    public virtual void OnHurtExit()
    {
        moveEnable = true;
        rigid.gravityScale = _defaultgravityscale;
        anim.speed = 1;
        // if (SubMoveRoutine != null)
        // {
        //     StopCoroutine(SubMoveRoutine);
        //     SubMoveRoutine = null;
        // }
        //TurnMove(_behavior.targetPlayer);

        transform.GetChild(0).GetComponentInChildren<AnimationEventSender>()?.ChangeFaceExpression(0f);

    }
    
    protected virtual void CheckLife()
    {
        if (CheckPowerOfBonds())
        {
            return;
        }
        
        OnDeath();
        
    }
    
    public bool CheckPowerOfBonds()
    {
        if(_statusManager.GetConditionsOfType((int)BasicCalculation.BattleCondition.PowerOfBonds).Count > 0)
        {
            _statusManager.currentHp = 0;
            _statusManager.HPRegenImmediately(100,0);
            BattleEffectManager.Instance.SpawnHealEffect(gameObject);
            _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.PowerOfBonds,true);
            //StartCoroutine(InvincibleRoutineWithoutRecover(1f));
            return true;
        }

        return false;
    }

    protected void OnDeath()
    {
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        SetKBRes(999);
        var enemyBehaviors = FindObjectsOfType<DragaliaEnemyBehavior>();
        foreach (var behavior in enemyBehaviors)
        {
            behavior.RemoveTarget(gameObject);
        }
        rigid.gravityScale = _defaultgravityscale;
        GetComponentInChildren<AudioSource>().Stop();
        
        transform.Find("MeeleAttackFX").gameObject.SetActive(false);
        transform.Find("HitSensor").gameObject.SetActive(false);
        anim.SetBool("defeat",true);
        //MoveManager.PlayVoice(0);//死亡
        anim.SetBool("hurt",false);
        OnAttackInterrupt?.Invoke();
        
        if (SubMoveRoutine != null)
        {
            StopCoroutine(SubMoveRoutine);
            SubMoveRoutine = null;
        }
        // if (MoveManager._tweener!=null)
        // {
        //     MoveManager._tweener.Kill();
        // }
        rigid.gravityScale = _defaultgravityscale;
        SetVelocity(rigid.velocity.x,0);
        moveEnable = false;
        
        anim.speed = 1;
        GetComponent<NpcController>().enabled = false;
        //MoveManager.SetGroundCollider(true);
        //MoveManager.enabled = false;
        //MoveManager.StopAllCoroutines();
        //_behavior.StopAllCoroutines();
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {

            //meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }

        //_behavior.enabled = false;
        _statusManager.enabled = false;
        _statusManager.StopAllCoroutines();
        if (hurtEffectCoroutine != null)
        {
            StopCoroutine(hurtEffectCoroutine);
            hurtEffectCoroutine = null;
            
        }

        //yield return new WaitUntil(()=>!anim.GetCurrentAnimatorStateInfo(0).IsName("hurt"));
        yield return null;
        
        anim.Play("defeat");
        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);
        anim.speed = 0;

        

    }

    public virtual void OnAttackEnter(int newKnockbackRes)
    {
        isMove = 0;
        //moveEnable = false;
        if (SubMoveRoutine != null)
        {
            StopCoroutine(SubMoveRoutine);
            SubMoveRoutine = null;
        }
        isAction = true;
    }
    
    public virtual void OnAttackExit()
    {
        _effectManager.DisplayCounterIcon(gameObject,false);
        isMove = 0;
        moveEnable = false;
        isAction = false;
        
        if (SubMoveRoutine != null)
            SubMoveRoutine = null;
    }
    
    public virtual void OnStandardAttackEnter()
    {
        isMove = 0;
        moveEnable = false;
        attackEnable = false;
        
    }

    public virtual void OnStandardAttackExit()
    {
        moveEnable = true;
        attackEnable = true;
    }
    
    public virtual void OnSkillEnter()
    {
        isMove = 0;
        moveEnable = false;
        attackEnable = false;
        dodging = true;
    }
    
    public virtual void OnSkillExit()
    {
        moveEnable = true;
        attackEnable = true;
        dodging = false;
    }


    public void SetKBRes(int value)
    {
        //currentKBRes = value;
    }

    /// <summary>
    /// 跳跃之后将着地判定暂时挂起
    /// </summary>
    /// <returns></returns>
    

    void GroundCheck(bool flag)
    {
        if (flag && rigid.velocity.y < 0.1f)
        {
            anim.SetBool("isGround",true);
            jumpTime = 2;
        }
        else
        {
            anim.SetBool("isGround",false);
        }

        
    }

    public void GoDownPlatform()
    {
        _groundSensor.StartCoroutine("DisableCollision");
    }

    public void DisableMovement()
    {
        anim.Play("idle");
        isMove = 0;
    }

    public virtual void SwapWeaponVisibility(bool flag)
    {
        weaponObject.transform.GetChild(0).gameObject.SetActive(flag);
    }
    
    
    
    protected void CheckFaceDir()
    {
        if (facedir == 1)
        {
            transform.localScale = new Vector3(1, 1, 1);
            //transform.localScale = new Vector3(1, 1, 1);
            //rigid.transform.eulerAngles = new Vector3(0, 0, 0);
        }
        else
        {
            transform.localScale = new Vector3(-1, 1, 1);
            //transform.localScale = new Vector3(1, 1, -1);
            //rigid.transform.eulerAngles = new Vector3(0, 180, 0);
        }
    }
    
    public void SetFaceDir(int dir)
    {
        facedir = dir;
        CheckFaceDir();
    }
    public bool GetDodge()
    {
        return dodging;
    }

    public override void AppearRenderer()
    {
        rendererObject.SetActive(true);
        weaponObject.SetActive(true);
    }
    
    public override void DisappearRenderer()
    {
        rendererObject.SetActive(false);
        weaponObject.SetActive(false);
    }

    protected virtual void StdAtk()
    {
        anim.SetBool("attack",false);
        //anim.Play($"combo"+(combo+1));
    }

    protected virtual void AirDashAtk()
    {
    }

    public void EventRoll()
    {
        StartCoroutine(HorizontalMove(rollspeed, 0.4f, "roll"));
    }
    
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

    public void onRollEnter()
    {
        dodging = true;
        isMove = 0;
    }
    
    public void onRollExit()
    {
        dodging = false;
        
    }
}
