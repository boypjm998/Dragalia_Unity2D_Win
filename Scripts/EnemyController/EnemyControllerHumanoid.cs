using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityParticleSystem;
using DG.Tweening;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Random = UnityEngine.Random;

public class EnemyControllerHumanoid : Enemy
{
    
    
    public float movespeed = 6.0f;
    public float rollspeed = 9.0f;
    public float jumpforce = 20.0f;
    protected int jumpTime = 2;
    private float jumpHeight = 4f;
    
    [SerializeField] protected float isMove = 0;
    public bool dodging = false;

    public bool moveEnable;

    public bool testMove;
    public GameObject tar;
    
    
    //private Animator anim;
    
    
    public float _defaultgravityscale;
    private EnemyGroundSensor _groundSensor;

    protected override void Awake()
    {
        base.Awake();
        anim.GetComponentInChildren<Animator>();
        rigid = GetComponentInChildren<Rigidbody2D>();
        MoveManager = GetComponent<EnemyMoveManager>();
        _groundSensor = GetComponentInChildren<EnemyGroundSensor>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
    }

    protected override void Start()
    {
        base.Start();
        currentKBRes = _statusManager.knockbackRes;

        EnemyGroundSensor.IsGround += GroundCheck;

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
        //print(transform.position.y);
        
        

    }

    private void FixedUpdate()
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








    /// <summary>
    /// 寻找玩家目标。
    /// </summary>
    /// <returns></returns>
    public virtual GameObject FindTarget()
    {
        var player = FindObjectOfType<PlayerInput>();
        return player.gameObject;
    }

    /// <summary>
    /// 主动靠近玩家
    /// </summary>
    /// <param name="target">玩家GameObject</param>
    /// <param name="maxFollowTime">最大搜寻时间</param>
    /// <param name="arriveDistance">任务成功所需X距离</param>
    /// <returns></returns>
    public override IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistance, float startFollowDistance)
    {
        if (Mathf.Abs(target.transform.position.x - rigid.position.x) < startFollowDistance)
        {
            OnMoveFinished?.Invoke(true);
            yield break;
        }
  
        
        isMove = 1;
        //print(arrivaDistance);
        
        while (maxFollowTime > 0)
        {
            maxFollowTime -= Time.deltaTime;
            
            var distance = target.transform.position.x - rigid.position.x;

            if (distance > 0)
                SetFaceDir(1);
            if(distance < 0)
                SetFaceDir(-1);
            //print(maxFollowTime);
            
            if(Mathf.Abs(distance) < arriveDistance)
                break;

            yield return null;

        }

        isMove = 0;
        currentKBRes = _statusManager.knockbackRes;
        ActionTask = null;
        OnMoveFinished?.Invoke(true);

    }
    
    public override IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistanceX,float arriveDistanceY, float startFollowDistance)
    {
        if (VerticalMoveRoutine != null)
            VerticalMoveRoutine = null;
        
        TurnMove(target);
        if (CheckTargetDistance(target,arriveDistanceX,arriveDistanceY))
        {
            OnMoveFinished?.Invoke(true);
            yield break;
        }
        
        while (maxFollowTime > 0)
        {
            maxFollowTime -= Time.deltaTime;
            //print(VerticalMoveRoutine == null?"null":VerticalMoveRoutine.ToString());
            var distanceX = GetTargetDistanceX(target);
            var distanceY = GetTargetDistanceY(target);

            if (Mathf.Abs(distanceX) > arriveDistanceX)
            {
                TurnMove(target);
                isMove = 1;

                if (_groundSensor.currentPlatform == null &&
                    _groundSensor.currentGround == null &&
                    VerticalMoveRoutine == null &&
                    distanceY > 0)
                {
                    VerticalMoveRoutine = StartCoroutine(TryJumpOver(target));
                }

            }
            else if (VerticalMoveRoutine == null )
            {
                if (distanceY > arriveDistanceY && jumpTime > 0)
                {
                    VerticalMoveRoutine = StartCoroutine(TryDoJump(target,arriveDistanceY));
                }else if (distanceY < -arriveDistanceY && _groundSensor.currentPlatform)
                {
                    VerticalMoveRoutine = StartCoroutine(TryDownPlatform(target));
                }
            }

            if (CheckTargetDistance(target, arriveDistanceX, arriveDistanceY))
            {
                if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }

                
                TurnMove(target);
                isMove = 0;
                currentKBRes = _statusManager.knockbackRes;
                ActionTask = null; //只有行为树在用这个
                OnMoveFinished?.Invoke(true);
                
                yield break;
            }
            yield return null;
        }
        isMove = 0;
        currentKBRes = _statusManager.knockbackRes;
        ActionTask = null; //只有行为树在用这个
        OnMoveFinished?.Invoke(false);
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }

        

    }

    public IEnumerator MoveToSameGround(GameObject target, float maxFollowTime, float arriveDistanceX)
    {
        if (VerticalMoveRoutine != null)
            VerticalMoveRoutine = null;
        
        TurnMove(target);
        if (CheckTargetStandOnSameGround(target)==0 && Mathf.Abs(GetTargetDistanceX(target)) <= arriveDistanceX)
        {
            //print("距离"+Mathf.Abs(GetTargetDistanceX(target)));
            OnMoveFinished?.Invoke(true);
            //print("开局就出去了");
            yield break;
        }
        
        while (maxFollowTime > 0)
        {
            maxFollowTime -= Time.deltaTime;
            //print(VerticalMoveRoutine == null?"null":VerticalMoveRoutine.ToString());
            var distanceX = GetTargetDistanceX(target);
            var distanceY = GetTargetDistanceY(target);

            if (Mathf.Abs(distanceX) > arriveDistanceX)
            {
                TurnMove(target);
                isMove = 1;

                if (_groundSensor.currentPlatform == null &&
                    _groundSensor.currentGround == null &&
                    VerticalMoveRoutine == null &&
                    distanceY > 0)
                {
                    VerticalMoveRoutine = StartCoroutine(TryJumpOver(target));
                }

            }
            else if (VerticalMoveRoutine == null )
            {

                if (CheckTargetStandOnSameGround(target)==1 && jumpTime > 0)
                {
                    if (!PlatformIsAccessible())
                    {
                        isMove = 1;
                    }
                    else
                    {
                        isMove = 0;
                        VerticalMoveRoutine = StartCoroutine(TryDoJump(target,.1f));
                    }

                    
                }else if (CheckTargetStandOnSameGround(target)==-1 && _groundSensor.currentPlatform)
                {
                    VerticalMoveRoutine = StartCoroutine(TryDownPlatform(target));
                }
            }

            if (CheckTargetStandOnSameGround(target)==0 &&
                anim.GetBool("isGround") == true &&
                Mathf.Abs(distanceX) <= arriveDistanceX)
            {
                if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }

                
                TurnMove(target);
                isMove = 0;
                currentKBRes = _statusManager.knockbackRes;
                ActionTask = null; //只有行为树在用这个
                OnMoveFinished?.Invoke(true);
                
                yield break;
            }
            yield return null;
        }
        isMove = 0;
        currentKBRes = _statusManager.knockbackRes;
        ActionTask = null; //只有行为树在用这个
        OnMoveFinished?.Invoke(false);
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
    }

    /// <summary>
    /// 检查与目标的距离，小于指定值返回true.
    /// </summary>
    /// <returns></returns>
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

    public int CheckTargetStandOnSameGround(GameObject target)
    {
        GameObject myGround;
        if (_groundSensor.currentGround != null)
        {
            myGround = _groundSensor.currentGround;
        }else if (_groundSensor.currentPlatform != null)
        {
            myGround = _groundSensor.currentPlatform;
        }
        else
        {
            RaycastHit2D myRay = 
                Physics2D.Raycast(transform.position, Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
        
            myGround = myRay.collider.gameObject;
        }

        GameObject tarGround;
        var targetSensor = target.GetComponentInChildren<PlayerOnewayPlatformEffector>();
        if(targetSensor.GetCurrentAttachedGroundInfo()!=null)
        {
            tarGround = targetSensor.GetCurrentAttachedGroundInfo();
        }else{
            RaycastHit2D tarRay = 
                Physics2D.Raycast(target.transform.position, Vector2.down,
                    999f,LayerMask.GetMask("Ground","Platforms"));
        
            tarGround = tarRay.collider.gameObject;
        }


        if (tarGround == myGround)
        {
            return 0;
        }
        
        if (Mathf.Abs(target.transform.position.y - transform.position.y) < 0.5f)
        {
            return 0;
        }

        if (Mathf.Abs(tarGround.transform.position.y - myGround.transform.position.y) < 0.5f)
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





    /// <summary>
    /// <para>弃用的</para>
    /// </summary>
    public bool CheckTaskStatus(bool needOnGround)
    {
        if (!isAction && !hurt)
        {
            if (needOnGround && anim.GetBool("isGround"))
            {
                return true;
            }

            if (!needOnGround)
            {
                return true;
            }
            
            return false;
        }

        return false;
    }

    IEnumerator TryDoJump(GameObject target, float requiredY)
    {
        var groundListener = target.GetComponentInChildren<PlayerOnewayPlatformEffector>();
        var groundTargetAttached = groundListener.GetCurrentAttachedGroundInfo();
        float distanceY = 0;
        if (groundTargetAttached)
        {
            distanceY = GetTargetGroundDistanceY(groundTargetAttached);
        }
        else
        {
            distanceY = GetTargetDistanceY(target);
        }

        if (Math.Abs(distanceY) > jumpHeight * 2 && !PlatformIsAccessible())
        {
            VerticalMoveRoutine = null;
            yield break;
        }
        
        //print(distanceY);
        if ((Math.Abs(distanceY) > jumpHeight))
        {
            //print("2段跳");
            Jump();
            yield return new WaitUntil(() => rigid.velocity.y < 0.5f);
            yield return new WaitForFixedUpdate();
            if (_groundSensor.currentPlatform || _groundSensor.currentGround)
            {
                VerticalMoveRoutine = null;
                yield break;
            }
            
            
            Jump();
            VerticalMoveRoutine = null;
            //yield return new WaitUntil(() => rigid.velocity.y < 0.5f);
            yield return new WaitForFixedUpdate();
            
            yield break;
            //二段跳
        }else if (Math.Abs(distanceY) < jumpHeight && Math.Abs(distanceY) > requiredY)
        {
            print("一段跳");
            Jump();
            
            //yield return new WaitUntil(() => rigid.velocity.y < 0.5f);
            //yield return new WaitForFixedUpdate();
            VerticalMoveRoutine = null;
            
            //一段跳
        }
        else VerticalMoveRoutine = null;

        VerticalMoveRoutine = null;

    }

    IEnumerator TryJumpOver(GameObject target)
    {
        Jump();
        yield return new WaitUntil(() => rigid.velocity.y < 0.5f);
        yield return new WaitForFixedUpdate();
        if (target.transform.position.y > transform.position.y)
        {
            Jump();
            VerticalMoveRoutine = null;
            yield break;
        }
        
        yield return new WaitUntil(() => transform.position.y <= target.transform.position.y);
        if (_groundSensor.currentPlatform == null && _groundSensor.currentGround == null)
        {
            Jump();
        }
        VerticalMoveRoutine = null;
        yield break;
        
        
    }

    IEnumerator TryDownPlatform(GameObject target)
    {
        var groundListener = target.GetComponentInChildren<PlayerOnewayPlatformEffector>();
        
        print(groundListener);

        while (groundListener.GetCurrentAttachedGroundInfo() == null)
        {
            yield return null;
        }

        //yield return new WaitUntil(() => groundListener.GetCurrentAttachedGroundInfo()!=null);
        
        //print("可以下了！");
        
        GoDownPlatform();

        VerticalMoveRoutine = null;

        //var currentAttachedGround = groundListener.GetCurrentAttachedGroundInfo();

        //RaycastHit2D[] hitinfo = Physics2D.RaycastAll
        //(transform.position, Vector2.down, 999f,
        //LayerMask.GetMask("Platforms", "Ground"));





    }


    /// <summary>
    /// 检查自身是否在地面上。
    /// </summary>
    /// <returns></returns>
    public bool GroundCheck()
    {
        return anim.GetBool("isGround");
    }

    
    /// <summary>
    /// 判断垂直上方方向是否有可以落脚的平台。
    /// </summary>
    /// <param name="groundTarget">弃用</param>
    /// <returns></returns>
    protected bool PlatformIsAccessible()
    {
        //var pos = groundTarget.GetComponent<Collider2D>().
        RaycastHit2D[] hitinfo =
            Physics2D.RaycastAll
                (transform.position, Vector2.up, 999, 
                    LayerMask.GetMask("Platforms"));

        List<float> PlatformPosition = new List<float>();

        if (hitinfo.Length == 0)
            return false;
        
        //PlatformPosition.Add(hitinfo[0].collider.gameObject.transform.position.x - transform.position.x);
        PlatformPosition.Add(transform.position.y);

        //Problem Exist
        foreach(var obj in hitinfo)
        {
            PlatformPosition.Add(obj.collider.gameObject.GetComponent<Collider2D>().bounds.max.y);
        }
        PlatformPosition.Sort();
        for (int i = 0; i < PlatformPosition.Count - 1; i++)
        {
            if (PlatformPosition[i + 1] - PlatformPosition[i] > jumpHeight * 2)
            {
                return false;
            }
        }
        return true;


    }









    protected virtual IEnumerator KnockBackEffect(float time,float force, Vector2 kbDir)
    {
        kbDir = kbDir.normalized;
        hurt = true;
        anim.SetBool("hurt",true);
        SetVelocity(force * kbDir.x,force * kbDir.y);
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
    public override void TakeDamage(float kbpower, float kbtime,float kbForce, Vector2 kbDir)
    {
        Flash();
        if (currentKBRes - kbpower >= 200)
        {
            return;
        }

        var rand = Random.Range(0, 100);
        if (rand > kbpower-currentKBRes)
        {
            return;
        }

        
        

        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
        }
        else
        {
            currentKBRes += (int)(kbtime*5)+1;
        }

        KnockbackRoutine = StartCoroutine(KnockBackEffect(kbtime,kbForce,kbDir));
        if (currentKBRes > 99)
        {
            StartCoroutine(ResetKBRes(_statusManager));
        }
        //Unimplemented
        
    
    }
    
    protected void SetAnimSpeed(float percentage)
    {
        anim.speed = percentage;
    }
    
    protected void OnHurtEnter()
    {
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
        if (MoveManager._tweener!=null)
        {
            MoveManager._tweener.Kill();
        }

        if (_behavior.currentAttackAction != null)
        {
            _behavior.StopCoroutine(_behavior.currentAttackAction);
            _behavior.currentAttackAction = null;
        }

        rigid.gravityScale = 1;
        moveEnable = false;
        //SetVelocity(rigid.velocity.x,0);
        anim.speed = 1;
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {
            meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }

    }
    
    protected void OnHurtExit()
    {
        moveEnable = true;
        rigid.gravityScale = _defaultgravityscale;
        anim.speed = 1;
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }

        

    }

    public override void OnAttackEnter()
    {
        isMove = 0;
        //moveEnable = false;
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
        isAction = true;
        currentKBRes = 100;
        //TurnMove(tar);
    }
    
    public override void OnAttackEnter(int newKnockbackRes)
    {
        isMove = 0;
        //moveEnable = false;
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
        isAction = true;
        currentKBRes = newKnockbackRes;
    }
    
    public override void OnAttackExit()
    {
        //isMove = 0;
        //moveEnable = false;
        isAction = false;
        currentKBRes = _statusManager.knockbackRes;
        if (VerticalMoveRoutine != null)
            VerticalMoveRoutine = null;
    }

    protected IEnumerator ResetKBRes(StatusManager statusManager)
    {
        yield return new WaitForSeconds(5f);
        currentKBRes = statusManager.knockbackRes;
    }

    public void SetKBRes(int value)
    {
        currentKBRes = value;
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


}
