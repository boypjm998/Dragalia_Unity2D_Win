using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;
using GameMechanics;
public class EnemyControllerHumanoid : EnemyController , IKnockbackable, IHumanActor
{
    public GameObject weaponObject; //need SeriazeField
    
    public float movespeed = 6.0f;
    public float rollspeed = 10.0f;
    public float jumpforce = 20.0f;
    protected int jumpTime = 2;
    protected float jumpAscentTime = 0.5f;
    [SerializeField] protected float jumpHeight = 5f;
    
    
    
    public bool dodging = false;

    public bool moveEnable;
    
    
    public GameObject tar;
    
    
    //protected Animator anim;
    
    
    public float _defaultgravityscale;
    public StandardGroundSensor _groundSensor;

    
    protected AStar _aStar;
    protected List<Platform> mapInfo;
    protected List<APlatformNode> pathInfo;
    protected APlatformNode currentNode;
    

    protected override void Awake()
    {
        //base.Awake();
        rendererObject = transform.Find("Model").GetChild(0).Find("model/mBodyAll").gameObject;
        anim = GetComponentInChildren<Animator>();
        rigid = GetComponentInChildren<Rigidbody2D>();
        MoveManager = GetComponent<EnemyMoveManager>();
        _groundSensor = GetComponentInChildren<StandardGroundSensor>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        
    }

    protected override void Start()
    {
        base.Start();
        if(canDeath)
            _statusManager.OnHPBelow0 += OnDeath;
        currentKBRes = _statusManager.knockbackRes;
        
        
        _statusManager.OnControlAfflictionRemoved += OnControlAfflictionRemoved;
        
        

        _groundSensor.IsGround += GroundCheck;
        mapInfo = BattleStageManager.InitMapInfo();

        jumpHeight = jumpforce * jumpforce / (2 * _defaultgravityscale * 10);
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
    





    /// <summary>
    /// 寻找玩家目标。
    /// </summary>
    /// <returns></returns>
    public virtual GameObject FindTarget()
    {
        var player = FindObjectOfType<PlayerInput>();
        return player.gameObject;
    }

    
    #region Old Methods
    /// <summary>
    /// 主动靠近玩家,只有行为树在用
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
        currentKBRes = 999;
        ActionTask = null;
        OnMoveFinished?.Invoke(true);

    }
    
    public override IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistanceX,float arriveDistanceY, float startFollowDistance, bool continueThoughConditionOK = false)
    {
        if (VerticalMoveRoutine != null)
            VerticalMoveRoutine = null;
        
        TurnMove(target);
        if (CheckTargetDistance(target,arriveDistanceX,arriveDistanceY) && !continueThoughConditionOK)
        {
            SetKBRes(999);
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

            if (CheckTargetDistance(target, arriveDistanceX, arriveDistanceY) && !continueThoughConditionOK)
            {
                if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }

                
                TurnMove(target);
                isMove = 0;
                currentKBRes = 999;
                ActionTask = null; //只有行为树在用这个
                OnMoveFinished?.Invoke(true);
                
                yield break;
            }
            yield return null;
        }
        isMove = 0;
        currentKBRes = 999;
        ActionTask = null; //只有行为树在用这个
        OnMoveFinished?.Invoke(false);
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }

        

    }

    public virtual IEnumerator MoveTowardTargetOnGround(GameObject target, float maxFollowTime, float arriveDistanceX,float arriveDistanceY, float startFollowDistance)
    {
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }

        
        
        TurnMove(target);
        if (CheckTargetDistance(target,arriveDistanceX,arriveDistanceY) && anim.GetBool("isGround"))
        {
            SetKBRes(999);
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
                if ((distanceY > arriveDistanceY) && jumpTime > 0 )
                {
                    VerticalMoveRoutine = StartCoroutine(TryDoJump(target,arriveDistanceY));
                }else if (jumpTime > 0 && CheckTargetStandOnSameGround(target) == 1)
                {
                    VerticalMoveRoutine = StartCoroutine(TryDoJump(target,0.1f));
                }
                else if (distanceY < -arriveDistanceY && _groundSensor.currentPlatform)
                {
                    VerticalMoveRoutine = StartCoroutine(TryDownPlatform(target));
                }
            }

            if (CheckTargetDistance(target, arriveDistanceX, arriveDistanceY) && anim.GetBool("isGround"))
            {
                if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }

                
                TurnMove(target);
                isMove = 0;
                currentKBRes = 999;
                ActionTask = null; //只有行为树在用这个
                OnMoveFinished?.Invoke(true);
                
                yield break;
            }
            yield return null;
        }
        isMove = 0;
        currentKBRes = 999;
        ActionTask = null; //只有行为树在用这个
        OnMoveFinished?.Invoke(false);
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }

        

    }
    public virtual IEnumerator MoveToSameGround(GameObject target, float maxFollowTime, float arriveDistanceX)
    {
        if (VerticalMoveRoutine != null)
            VerticalMoveRoutine = null;
        
        print("Enter Moving To Same Ground");
        
        TurnMove(target);
        if (CheckTargetStandOnSameGround(target) == 0 && Mathf.Abs(GetTargetDistanceX(target)) <= arriveDistanceX)
        {
            SetKBRes(999);
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
            else if (VerticalMoveRoutine == null)
            {
                float targetPlatformY;
                
                if (CheckTargetStandOnSameGround(target) == 1 && jumpTime > 0)
                {
                    print("needJump");
                    if (!PlatformIsAccessible(out targetPlatformY))
                    {
                        //if(targetPlatformY < BasicCalculation.CheckRaycastedPlatform(target).bounds.max.y)
                        isMove = 1;
                        //else isMove = 0;
                        print(targetPlatformY);
                    }
                    else
                    {
                        if(targetPlatformY > 1 + BasicCalculation.CheckRaycastedPlatform(target).bounds.max.y)
                            isMove = 1;
                        else isMove = 0;
                        print(targetPlatformY);
                        
                        //isMove = 0;
                        VerticalMoveRoutine = StartCoroutine(TryDoJump(target,.1f));
                    }

                    
                }else if (CheckTargetStandOnSameGround(target)==-1 && _groundSensor.currentPlatform)
                {
                    VerticalMoveRoutine = StartCoroutine(TryDownPlatform(target));
                    //  2023/7/1添加
                    var targetPlatform = BasicCalculation.CheckRaycastedPlatform(target);
                    
                    if (targetPlatform.bounds.max.x < transform.position.x)
                    {
                        isMove = 1;
                        SetFaceDir(1);
                    }
                    else if (targetPlatform.bounds.min.x > transform.position.x)
                    {
                        isMove = 1;
                        SetFaceDir(-1);
                    }
                    else
                    {
                        isMove = 0;
                    }
                    
                }
                else if (CheckTargetStandOnSameGround(target) == -1 && !_groundSensor.currentPlatform)
                {
                    var targetPlatform = BasicCalculation.CheckRaycastedPlatform(target);

                    if (targetPlatform != null)
                    {
                        if (targetPlatform.bounds.max.x < transform.position.x)
                        {
                            isMove = 1;
                            SetFaceDir(-1);
                        }
                        else if (targetPlatform.bounds.min.x > transform.position.x)
                        {
                            isMove = 1;
                            SetFaceDir(1);
                        }
                    }
                    else
                    {
                        isMove = 0;
                    }
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
                currentKBRes = 999;
                
                OnMoveFinished?.Invoke(true);
                
                yield break;
            }
            yield return null;
        }
        isMove = 0;
        currentKBRes = 999;
        ActionTask = null; //只有行为树在用这个
        OnMoveFinished?.Invoke(false);
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
    }

    
    
    # endregion
    /// <summary>
    /// 检查目标是否和自己在同一个平面
    /// </summary>
    /// <returns>返回0为在同一平面，1为目标高于自身，-1为目标低于自身</returns>
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
        var targetSensor = target.GetComponentInChildren<IGroundSensable>();
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
        
        if (Mathf.Abs(target.transform.position.y - transform.position.y) < 1f)
        {
            return 0;
        }

        if (Mathf.Abs(tarGround.transform.position.y - myGround.transform.position.y) < 1f)
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
    
    /// <summary>
    /// New Method
    /// </summary>
    /// <param name="target"></param>
    /// <param name="arriveDistance"></param>
    /// <param name="maxFollowTime"></param>
    /// <returns></returns>
    public virtual IEnumerator MoveTowardsTarget(GameObject target, float arriveDistance, float maxFollowTime)
    {
        isAction = true;
        yield return new WaitUntil(() =>
            !hurt);
        var targetSensor = target.GetComponentInChildren<IGroundSensable>();
        if (targetSensor == null)
        {
            targetSensor = target.transform.parent.GetComponentInChildren<IGroundSensable>();
        }

        var mySensor = GetComponentInChildren<IGroundSensable>();
        while (maxFollowTime>0)
        {

            yield return new WaitUntil(() =>
                anim.GetBool("isGround") && !hurt);
            
            // 检查NPC和目标是否处于同一个平台上
            if (CheckTargetStandOnSameGround(target) == 0) {
                // 计算两者之间的x轴距离
                float distance = Mathf.Abs(transform.position.x - target.transform.position.x);

                if (distance > arriveDistance)
                {
                    TurnMove(target);
                    isMove = 1;
                }
                else
                   isMove = 0;
                
            
                // 如果x轴距离小于arriveDistance，则终止协程
                if (distance <= arriveDistance) {
                    isMove = 0;
                    OnMoveFinished?.Invoke(true);
                    yield break;
                }
            } else {

                while (targetSensor.GetCurrentAttachedGroundCol() == null)
                {
                    yield return null;
                    if(anim.GetBool("isGround")==false)
                        continue;
                    //4.3添加
                    if(transform.position.x >= mySensor.GetCurrentAttachedGroundCol().bounds.max.x ||
                       transform.position.x <= mySensor.GetCurrentAttachedGroundCol().bounds.min.x ||
                       (transform.position.x >= target.transform.position.x && facedir==1) ||
                       (transform.position.x <= target.transform.position.x && facedir==-1))
                        isMove = 0;
                    else isMove = 1;
                }
                // yield return new WaitUntil(() =>
                //     targetSensor.GetCurrentAttachedGroundCol() != null);
                
                
               
                GetPath(target);
                
                var path = pathInfo;
                
                if (path == null)
                {
                    float distance = Mathf.Abs(transform.position.x - target.transform.position.x);
                    if (arriveDistance > distance)
                    {
                        continue;
                    }

                    OnMoveFinished?.Invoke(false);
                    isMove = 0;
                    _behavior.currentMoveAction = null;
                    //currentMainRoutineType = MainRoutineType.None;
                    isAction = false;
                    yield break;
                }

                // 沿着路径移动
                foreach (var platform in path) {
                    
                    yield return new WaitUntil(() => anim.GetBool("isGround"));
                    if (platform.platform.collider == _groundSensor.GetCurrentAttachedGroundCol())
                    {
                        if (CheckTargetStandOnSameGround(target) == 0)
                        {
                            isMove = 0;
                            break;
                        }
                        continue;
                    }

                    // 移动到当前平台
                    if (VerticalMoveRoutine != null)
                    {
                        StopCoroutine(VerticalMoveRoutine);
                        VerticalMoveRoutine = null;
                    }
                    

                    VerticalMoveRoutine = StartCoroutine(MoveToPlatform(platform));
                
                    // 等待VerticalMoveRoutine结束或被中断
                    
                    yield return new WaitUntil(()=>VerticalMoveRoutine==null);
                    
                    

                    //isMove = 0;
                    isMove = 0;
                    
                    yield return new WaitUntil(() =>
                        anim.GetBool("isGround"));
                    
                    if(CheckTargetStandOnSameGround(target) == 0)
                        break;
                    // 重新计算路径
                    //path = GetPath(target);
                }
            }
        
            yield return null;
        }
        isMove = 0;
        OnMoveFinished?.Invoke(false);
        _behavior.currentMoveAction = null;
        //currentMainRoutineType = MainRoutineType.None;
        isAction = false;
    }

    /// <summary>
    /// 只在x轴上移动
    /// </summary>
    /// <param name="target"></param>
    /// <param name="maxFollowTime"></param>
    /// <param name="minDistance"></param>
    /// <param name="maxDistance"></param>
    /// <param name="continueThoughConditionOK"></param>
    /// <returns></returns>
    public virtual IEnumerator KeepDistanceFromTarget(GameObject target, float maxFollowTime, float minDistance, float maxDistance = 999f, bool continueThoughConditionOK = false)
    {
        isAction = true;
        var currentPlatform = BasicCalculation.CheckRaycastedPlatform(gameObject);
        var leftbound = currentPlatform.bounds.min.x;
        var rightbound = currentPlatform.bounds.max.x;
        
        if(leftbound < BattleStageManager.Instance.mapBorderL)
            leftbound = BattleStageManager.Instance.mapBorderL;
        if(rightbound > BattleStageManager.Instance.mapBorderR)
            rightbound = BattleStageManager.Instance.mapBorderR;
        
        var marginLimit = rightbound - leftbound >= minDistance ? minDistance : (rightbound - leftbound) * 0.33f;
        //print(marginLimit);
        yield return new WaitUntil(() => !hurt);

        while (maxFollowTime > 0)
        {
            var distance = target.transform.position.x - transform.position.x;
            if (((distance > minDistance && distance < maxDistance) || (distance < -minDistance && distance > -maxDistance)) 
                && !continueThoughConditionOK)
            {
                //在右边且距离大于最小距离
                isMove = 0;
                OnMoveFinished?.Invoke(true);
                _behavior.currentMoveAction = null;
                isAction = false;
                yield break;
            }
            else if(distance <= minDistance && distance >= -minDistance)
            {
                //目标在自身的右边
                if (distance > 0)
                {
                    if (target.transform.position.x - leftbound > marginLimit)
                    {
                        //1.如果目标在自身的右边，且目标远离左边界，则向左移动。
                        SetFaceDir(-1);
                        isMove = 1;
                        //print("正在进行操作1");
                    }
                    else
                    {
                        //2.如果目标在自身的右边，且目标靠近左边界，则向右移动。
                        SetFaceDir(1);
                        isMove = 1;
                        //print("正在进行操作2");
                    }
                }
                else
                {
                    if(target.transform.position.x - rightbound < -marginLimit)
                    {
                        //3.如果目标在自身的左边，且目标远离右边界，则向右移动。
                        SetFaceDir(1);
                        isMove = 1;
                        //print("正在进行操作3");
                    }
                    else
                    {
                        //4.如果目标在自身的左边，且目标靠近右边界，则向左移动。
                        SetFaceDir(-1);
                        isMove = 1;
                        //print("正在进行操作4");
                    }
                    
                }
            }
            else if (distance >= maxDistance || distance <= -maxDistance)
            {
                TurnMove(target);
                isMove = 1;
            }
            else
            {
                isMove = 0;
                TurnMove(target);
            }
            
            if (transform.position.x <= leftbound)
            {
                SetFaceDir(1);
                isMove = 0;
                print("正在进行操作5");
            }
            else if (transform.position.x >= rightbound)
            {
                SetFaceDir(-1);
                isMove = 0;
                print("正在进行操作6");
            }

            maxFollowTime -= Time.deltaTime;

            yield return null;
        }
        
        isMove = 0;
        OnMoveFinished?.Invoke(false);
        _behavior.currentMoveAction = null;
        isAction = false;

    }

    public virtual IEnumerator JumpToTarget(GameObject target, float arriveDistanceY, float arriveDistanceX)
    {
        //TODO: DOJUMPTOTARGET
        isAction = true;
        yield return new WaitUntil(() =>
            !hurt && grounded);
        isMove = 1;
        
        while (jumpTime > 0)
        {
            if (Mathf.Abs(target.transform.position.y - transform.position.y) < arriveDistanceY &&
                Mathf.Abs(target.transform.position.x - transform.position.x) < arriveDistanceX)
            {
                isMove = 0;
                TurnMove(target);
                OnMoveFinished?.Invoke(true);
                //_behavior.currentMoveAction = null;
                isAction = false;
                yield break;
            }
            if(target.transform.position.x > transform.position.x + arriveDistanceX)
                SetFaceDir(1);
            if(target.transform.position.x < transform.position.x - arriveDistanceX)
                SetFaceDir(-1);

            if (target.transform.position.y > transform.position.y + arriveDistanceY &&
                rigid.velocity.y < 0.5f)
            {
                Jump();
            }
            yield return new WaitForFixedUpdate();
            if(CheckTargetStandOnSameGround(target)!=0)
            {
                isMove = 0;
                TurnMove(target);
                OnMoveFinished?.Invoke(false);
                //_behavior.currentMoveAction = null;
                isAction = false;
                yield break;
            }
            

        }


    }

    # region Old Vertical Moves
    protected virtual IEnumerator TryDoJump(GameObject target, float requiredY)
    {
        var groundListener = target.GetComponentInChildren<IGroundSensable>();
        var groundTargetAttached = groundListener.GetCurrentAttachedGroundInfo();
        float distanceY = 0;
        if (groundTargetAttached)
        {
            print("PlayerGroundAttached");
            distanceY = GetTargetGroundDistanceY(groundTargetAttached);
        }
        else
        {
            distanceY = GetTargetDistanceY(target);
        }

        var targetY = 999f;
        if (Math.Abs(distanceY) > jumpHeight * 2 && !PlatformIsAccessible(out targetY))
        {
            print("PlatformIsNotAccessible");
            VerticalMoveRoutine = null;
            yield break;
        }
        
        if ((Math.Abs(distanceY) > jumpHeight))
        {
            //print("2段跳");
            Jump();
            yield return new WaitUntil(() => rigid.velocity.y < 0.5f);
            
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
            //print("一段跳");
            Jump();
            
            //yield return new WaitUntil(() => rigid.velocity.y < 0.5f);
            yield return new WaitForFixedUpdate();
            VerticalMoveRoutine = null;
            
            //一段跳
        }
        else VerticalMoveRoutine = null;

        VerticalMoveRoutine = null;

    }

    protected virtual IEnumerator TryJumpOver(GameObject target)
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

    protected virtual IEnumerator TryDownPlatform(GameObject target)
    {
        var groundListener = target.GetComponentInChildren<IGroundSensable>();
        
        //print(groundListener);

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
    
    #endregion

    #region New Vertical Moves

    protected virtual IEnumerator MoveToPlatform(APlatformNode node)
    {
        
        yield return new WaitUntil(() => anim.GetBool("isGround"));
        var targetPlatformCollider = node.platform.collider;
        var currentPlatformCollider = _groundSensor.GetCurrentAttachedGroundCol();
        currentNode = node;
        
        var targetPlatformLeft = node.platform.leftBorderPos;
        var targetPlatformRight = node.platform.rightBorderPos;
        float currentPlatformHeight;
        try
        {
            currentPlatformHeight = currentPlatformCollider.bounds.max.y;
        }
        catch
        {
            currentPlatformCollider = BasicCalculation.CheckRaycastedPlatform(gameObject);
            currentPlatformHeight = currentPlatformCollider.bounds.max.y;
        }


        var targetPlatformHeight = node.platform.height;


        if (targetPlatformLeft.x > transform.position.x)
        {
            facedir = 1;
            isMove=1;
        }else if(targetPlatformRight.x < transform.position.x)
        {
            facedir = -1;
            isMove=1;
        }
        
        var runUpInfo = GetRunUpInfo
            (targetPlatformHeight - currentPlatformHeight,currentPlatformCollider,targetPlatformCollider);

        print("JumpHeight:"+(targetPlatformHeight - currentPlatformHeight));
        
        //助跑阶段
        while (transform.position.x < targetPlatformLeft.x - runUpInfo[0] ||
               transform.position.x > targetPlatformRight.x + runUpInfo[0])
        {
            yield return null;
            if (transform.position.x < targetPlatformLeft.x - runUpInfo[0] && facedir == -1)
            {
                isMove=1;
                facedir = 1;
            }
            else if (transform.position.x > targetPlatformRight.x + runUpInfo[0] && facedir == 1)
            {
                isMove=1;
                facedir = -1;
            }
            print("转身"+runUpInfo[0]);

                    
        }
        
        



        if (targetPlatformHeight > currentPlatformHeight)
        {
            print(runUpInfo[1]);
            if (runUpInfo[1] == 2)
            {

                Jump();
                yield return new WaitForFixedUpdate();

                while (rigid.velocity.y > 0)
                {
                    yield return null;
                }
                
                Jump();
                while(!anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && facedir == -1)
                    {
                        isMove=1;
                        facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && facedir == 1)
                    {
                        isMove=1;
                        facedir = -1;
                    }

                    yield return null;
                }




            }
            else if (runUpInfo[1] == 1)
            {
                Jump();
                yield return new WaitForSeconds(jumpAscentTime);
                while(!anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && facedir == -1)
                    {
                        isMove=1;
                        facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && facedir == 1)
                    {
                        isMove=1;
                        facedir = -1;
                    }

                    yield return null;
                }
            }
        }
        else
        {
            var relativeHeight = transform.position.y - currentPlatformCollider.bounds.max.y;
            if (runUpInfo[1] == 2)
            {
                yield return new WaitUntil(() => transform.position.y <= _behavior.targetPlayer.transform.position.y
                || _groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (_groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    _groundSensor.StartCoroutine("DisableCollision");
                }

                yield return new WaitUntil(() => transform.position.y <= _behavior.targetPlayer.transform.position.y);
                Jump();
                yield return new WaitForSeconds(jumpAscentTime);
                yield return new WaitUntil(() => transform.position.y <= _behavior.targetPlayer.transform.position.y);
                Jump();
                while (!anim.GetBool("isGround"))
                {
                    var leftLimit = targetPlatformLeft.x;
                    var rightLimit = targetPlatformRight.x;
                    if (transform.position.x < leftLimit && facedir == -1)
                    {
                        isMove=1;
                        facedir = 1;
                    }
                    else if (transform.position.x > rightLimit && facedir == 1)
                    {
                        isMove=1;
                        facedir = -1;
                    }

                    yield return null;
                }

            }
            else if (runUpInfo[1] == 1)
            {
                yield return new WaitUntil(() => transform.position.y <= _behavior.targetPlayer.transform.position.y
                                                 || _groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (_groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    _groundSensor.StartCoroutine("DisableCollision");
                }

                yield return new WaitUntil(() => transform.position.y <= _behavior.targetPlayer.transform.position.y);
                Jump();
                //yield return new WaitForSeconds(jumpAscentTime);
                while (!anim.GetBool("isGround"))
                {
                    var leftLimit = Mathf.Max(_behavior.targetPlayer.transform.position.x - 1,targetPlatformLeft.x);
                    var rightLimit = Mathf.Min(_behavior.targetPlayer.transform.position.x + 1,targetPlatformRight.x);
                    if (transform.position.x < leftLimit && facedir == -1)
                    {
                        isMove=1;
                        facedir = 1;
                    }
                    else if (transform.position.x > rightLimit && facedir == 1)
                    {
                        isMove=1;
                        facedir = -1;
                    }

                    yield return null;
                }
            }
            else
            {
                yield return new WaitUntil(() => transform.position.y <= targetPlatformHeight + relativeHeight
                                                 || _groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (_groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    _groundSensor.StartCoroutine("DisableCollision");
                }
                
                
                yield return new WaitUntil(() => transform.position.y < currentPlatformCollider.bounds.min.y);
                yield return new WaitForSeconds(0.1f);

                while (!anim.GetBool("isGround"))
                {
                    float rightLimit = targetPlatformRight.x;
                    float leftLimit = targetPlatformLeft.x;
                    if (_behavior.targetPlayer != null)
                    {
                        //leftLimit = Mathf.Max(_behavior.targetPlayer.transform.position.x - 1,targetPlatformLeft.x);
                        //rightLimit = Mathf.Min(_behavior.targetPlayer.transform.position.x + 1,targetPlatformRight.x);
                    }

                    if (transform.position.x < leftLimit)
                    {
                        isMove=1;
                        facedir = 1;
                    }
                    else if (transform.position.x > rightLimit)
                    {
                        isMove=1;
                        facedir = -1;
                    }

                    yield return null;
                }
            }
        }

        yield return new WaitUntil(() => anim.GetBool("isGround"));
        VerticalMoveRoutine = null;
        print("跳过了");
        
    }

    #endregion

    
    /// <summary>
    /// 判断垂直上方方向是否有可以落脚的平台。
    /// </summary>
    /// <param name="groundTarget">弃用</param>
    /// <returns></returns>
    protected virtual bool PlatformIsAccessible(out float targetY)
    {
        //7.6修改
        targetY = 999;
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

        targetY = PlatformPosition[1];
        return true;


    }

    public void StartKnockback(float time, float force, Vector2 kbDir)
    {
        if (KnockbackRoutine == null)
        {
            SetVelocity(rigid.velocity.x,0);
            SetActionUnable(true);
            StartCoroutine(KnockBackEffect(time, force, kbDir));
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
        
        if (_statusManager.controlRoutine == null)
        {
            anim.SetBool("hurt",false);
            hurt = false;
        }

        
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
        if (_statusManager.KnockbackRes + currentKBRes - kbpower >= 100)
        {
            return;
        }

        var rand = Random.Range(0, 100);
        if (rand >= kbpower-currentKBRes-_statusManager.KnockbackRes)
        {
            return;
        }
        
        //print(rand+"小于"+(kbpower-currentKBRes));

        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
        }
        else
        {
            currentKBRes += (int)(kbtime*5)+1;
        }
        //print(rand+"小于"+(kbpower-currentKBRes));

        if (counterOn)
        {
            //反击
            // _effectManager.DisplayCounterIcon(gameObject,false);
            // DamageNumberManager.GenerateCounterText(transform);
            //
            // _statusManager.ObtainUnstackableTimerBuff
            // ((int)BasicCalculation.BattleCondition.Vulnerable,
            //     10,10,9999);
            // _statusManager.ObtainUnstackableTimerBuff
            // ((int)BasicCalculation.BattleCondition.AtkDebuff,
            //     30,7,9999);
        }

        KnockbackRoutine = StartCoroutine(KnockBackEffect(kbtime,kbForce,kbDir));
        
        
    
    }
    
    public override void TakeDamage(AttackBase atkBase, Vector2 kbDir)
    {
        Flash();
        var kbpower = atkBase.attackInfo[0].knockbackPower;
        var kbtime = atkBase.attackInfo[0].knockbackTime;
        var kbForce = atkBase.attackInfo[0].knockbackForce;
        
        // if (currentKBRes - kbpower >= 100)
        // {
        //     return;
        // }

        var rand = Random.Range(0, 100);
        //print(kbpower-currentKBRes-_statusManager.GetKBResBuff() + "<->" + rand);
        if (rand >= kbpower-currentKBRes-_statusManager.GetKBResBuff()
            || currentKBRes+_statusManager.GetKBResBuff() - kbpower >= 100)
        {

            if (counterOn && kbpower > currentKBRes+_statusManager.GetKBResBuff()
                          && _statusManager is SpecialStatusManager)
            {
                var container = atkBase.GetComponentInParent<AttackContainer>();
                if (container.IfODCounter == false)
                {
                    DamageNumberManager.GenerateCounterText(transform, true);
                    atkBase.GetComponentInParent<AttackContainer>().IfODCounter = true;
                }
            }

            return;
        }
        
        //print(rand+"小于"+(kbpower-currentKBRes));

        if (KnockbackRoutine != null)
        {
            StopCoroutine(KnockbackRoutine);
        }
        else
        {
            currentKBRes += (int)(kbtime*5)+1;
        }
        //print(rand+"小于"+(kbpower-currentKBRes));

        if (counterOn)
        {
            //TODO: 反击
            _effectManager.DisplayCounterIcon(gameObject,false);
            DamageNumberManager.GenerateCounterText(transform);
            
            _statusManager.ObtainUnstackableTimerBuff
            ((int)BasicCalculation.BattleCondition.Vulnerable,
                10,10,9999);
            _statusManager.ObtainUnstackableTimerBuff
            ((int)BasicCalculation.BattleCondition.AtkDebuff,
                30,7,9999);
            atkBase.GetComponentInParent<AttackContainer>().IfODCounter = true;
            
            OnBeingCountered?.Invoke();
            
            SetCounter(false);
            
        }

        KnockbackRoutine = StartCoroutine(KnockBackEffect(kbtime,kbForce,kbDir));
        
        
    
    }
    
    protected override IEnumerator HurtEffectCoroutine()
    {
        var flashWeapon = weaponObject.transform.Find("Flash").gameObject;
        var time = hurtEffectDuration;
        while (time > 0)
        {
            time -= Time.deltaTime;
            flashBody.SetActive(true);
            flashWeapon.SetActive(true);
            yield return null;
        }
        flashBody.SetActive(false);
        flashWeapon.SetActive(false);
        hurtEffectCoroutine = null;
    }

    

    protected void SetAnimSpeed(float percentage)
    {
        anim.speed = percentage;
    }

    protected override void OnReceiveControlAffliction()
    {
        hurt = true;
        SetCounter(false);
        // base.OnReceiveControlAffliction();
        
        if (_behavior.currentAction != null)
        {
            _behavior.StopCoroutine(_behavior.currentAction);
            _behavior.currentAction = null;
        }

        if (_behavior.currentMoveAction != null)
        {
            _behavior.StopCoroutine(_behavior.currentMoveAction);
            _behavior.currentMoveAction = null;
        }
            
        if (_behavior.currentAttackAction != null) {
            _behavior.StopCoroutine(_behavior.currentAttackAction);
            _behavior.currentAttackAction = null;
        }

        if (_behavior.controllAfflictionProtect)
        {
            StageCameraController.SwitchMainCamera();
        }


    }

    public override void OnHurtEnter()
    {
        
        OnAttackInterrupt?.Invoke();
        
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
            currentKBRes = _statusManager.knockbackRes;
            SetCounter(false);
        }

        rigid.gravityScale = 1;
        //SetVelocity(rigid.velocity.x,0);
        moveEnable = false;
        dodging = false;
        anim.speed = 1;
        MoveManager.SetGroundCollider(true);
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {

            //meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }
        
        transform.GetChild(0).GetComponentInChildren<AnimationEventSender_Enemy>()?.ChangeFaceExpression(0.75f);
        MoveManager.AppearRenderer();

    }
    
    public override void OnHurtExit()
    {
        moveEnable = true;
        //SetCounter(false);
        rigid.gravityScale = _defaultgravityscale;
        anim.speed = 1;
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
        TurnMove(_behavior.targetPlayer);

        transform.GetChild(0).GetComponentInChildren<AnimationEventSender_Enemy>()?.ChangeFaceExpression(0f);

        // if (_behavior.controllAfflictionProtect)
        // {
        //     _behavior.ActionEnd(false);
        // }

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
        
        if(currentKBRes + _statusManager.KnockbackRes - _statusManager.knockbackRes < 200)
            SetCounter(true);
    }

    public override void OnBreakEnter()
    {
        SetKBRes(999);
        OnHurtEnter();
        isAction = true;
        _behavior.isAction = true;
        StageCameraController.SwitchMainCamera();
        
        isMove = 0;
        BattleEffectManager.Instance.PlayBreakEffect();
        SetCounter(false);
        //print((_statusManager as SpecialStatusManager).breakTime);
        UI_BossODBar.Instance?.ODBarClear();
        breakRoutine = StartCoroutine(BreakWait((_statusManager as SpecialStatusManager).breakTime));
        
    }
    
    
    
    public override void OnBreakExit()
    {
        OnHurtExit();
        SetKBRes(_statusManager.knockbackRes);
        SetCounter(false);
        //transform.Find("Model/model/Break")?.gameObject.SetActive(false);
        var spStatus = _statusManager as SpecialStatusManager;
        if (spStatus.ODLock == false)
        {
            spStatus.currentBreak = spStatus.baseBreak;
        }
        else spStatus.currentBreak = 0.1f;
        
        spStatus.broken = false;

        if (_statusManager.controlRoutine == null)
        {
            _behavior.ActionEnd(false);
            _behavior.isAction = false;
            isAction = false;
        }

        
        
        
        UI_BossODBar.Instance?.ODBarRecharge();
    }

    public override void StartBreak()
    {
        var spStatus = _statusManager as SpecialStatusManager;
        if (_behavior.breakable && spStatus.broken == false)
        {
            _behavior.isAction = true;
            if (_behavior.currentAction != null)
            {
                _behavior.StopCoroutine(_behavior.currentAction);
                _behavior.currentAction = null;
            }

            if (_behavior.currentMoveAction != null)
            {
                _behavior.StopCoroutine(_behavior.currentMoveAction);
                _behavior.currentMoveAction = null;
            }

            
            spStatus.broken = true;
            SetKBRes(999);
            anim.SetBool("break",true);
            anim.Play("break_enter");
        }
        
    }

    protected override IEnumerator BreakWait(float time, float recoverTime = 1.67f)
    {
        SetKBRes(999);
        yield return new WaitForSeconds(time - 1.67f);
        anim.Play("break_exit");
        yield return new WaitForSeconds(1.67f);
        
        // var _statusManagerS = _statusManager as SpecialStatusManager;
        // if (!_statusManagerS.ODLock)
        // {
        //     _statusManagerS.ODLock = true;
        //     _behavior.breakable = false;
        //     _statusManagerS.currentBreak = 1f;
        //     var twc = DOTween.To(() => _statusManagerS.currentBreak,
        //         x => _statusManagerS.currentBreak = x,
        //         _statusManagerS.baseBreak, 1f);
        //     twc.OnComplete(() =>
        //         {
        //             _statusManagerS.ODLock = false;
        //             _behavior.breakable = true;
        //         }
        //     );
        // }

        
        
        breakRoutine = null;
        anim.SetBool("break",false);
    }


    protected override void OnDeath()
    {
        //TODO:信赖之力不会导致死亡

        base.OnDeath();
        if(_statusManager.currentHp > 0)
            return;
        StartCoroutine(DeathRoutine());
    }

    IEnumerator DeathRoutine()
    {
        SetKBRes(999);
        _effectManager.DisplayCounterIcon(gameObject,false);
        GetComponentInChildren<AudioSource>().Stop();
        transform.Find("MeeleAttackFX").gameObject.SetActive(false);
        transform.Find("HitSensor").gameObject.SetActive(false);
        anim.SetBool("defeat",true);
        anim.SetBool("break",true);
        MoveManager.PlayVoice(0);//死亡
        anim.SetBool("hurt",false);
        OnAttackInterrupt?.Invoke();
        _behavior.breakable = false;
        _behavior.enabled = false;
        moveEnable = false;
        _statusManager.enabled = false;
        _statusManager.StopAllCoroutines();

        if (_statusManager is SpecialStatusManager)
        {
            anim.SetBool("break",false);
            if (breakRoutine != null)
            {
                StopCoroutine(breakRoutine);
                breakRoutine = null;
            }
            (_statusManager as SpecialStatusManager).broken = false;
            (_statusManager as SpecialStatusManager).ODLock = true;
            (_statusManager as SpecialStatusManager).currentBreak = 0.1f;
        }


        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
        if (MoveManager._tweener!=null)
        {
            MoveManager._tweener.Kill();
        }
        rigid.gravityScale = _defaultgravityscale;
        SetVelocity(rigid.velocity.x,0);
        moveEnable = false;
        //SetVelocity(rigid.velocity.x,0);
        anim.speed = 1;
        MoveManager.SetGroundCollider(true);
        MoveManager.enabled = false;
        MoveManager.StopAllCoroutines();
        _behavior.StopAllCoroutines();
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {

            //meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }

        _behavior.enabled = false;
        _statusManager.ResetAllStatusForced();
        _statusManager.enabled = false;
        _statusManager.StopAllCoroutines();
        if (hurtEffectCoroutine != null)
        {
            StopCoroutine(hurtEffectCoroutine);
            flashBody.SetActive(false);
            var flashWeapon = weaponObject.transform.Find("Flash").gameObject;
            flashWeapon.SetActive(false);
            hurtEffectCoroutine = null;
        }

        //yield return new WaitUntil(()=>!anim.GetCurrentAnimatorStateInfo(0).IsName("hurt"));
        yield return null;
        moveEnable = false;
        isMove = 0;
        anim.Play("defeat");
        yield return null;
        anim.SetFloat("forward",0);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);
        anim.speed = 0;

        if (disappearTimeAfterDeath > 0)
        {
            yield return new WaitForSeconds(disappearTimeAfterDeath);
            Destroy(gameObject);
        }

        
        
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
        
        if(newKnockbackRes<200 && newKnockbackRes>=100)
            SetCounter(true);
    }
    
    public override void OnAttackExit()
    {
        SetCounter(false);
        //isMove = 0;
        //moveEnable = false;
        isAction = false;
        //currentKBRes = _statusManager.knockbackRes;
        if (VerticalMoveRoutine != null)
            VerticalMoveRoutine = null;
        SetCounter(false);
    }
    

    

    /// <summary>
    /// 跳跃之后将着地判定暂时挂起
    /// </summary>
    /// <returns></returns>
    

    void GroundCheck(bool flag)
    {
        if (flag && rigid.velocity.y < 0.15f)
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
    
    public virtual void InertiaMove(float time,int movedir = 0)
    {

        StartCoroutine(HorizontalMoveInteria(time, movespeed, movedir));

    }
    
    public IEnumerator HorizontalMoveInteria(float time, float speed, int movedir = 0)
    {
        //6.24修改
        if (movedir == 0)
            movedir = this.facedir;
        
        
        while (time > 0)
        {
            
            transform.position = new Vector2(transform.position.x + movedir * speed * Time.fixedDeltaTime,
                transform.position.y);
            

            //rigid.velocity = new Vector2(transform.localScale.x*speed, rigid.velocity.y);
            time -= Time.fixedDeltaTime;

            yield return new WaitForFixedUpdate();
        }

    }

    public void DisableMovement()
    {
        anim.Play("idle");
        isMove = 0;
    }

    public override void SwapWeaponVisibility(bool flag)
    {
        weaponObject.transform.GetChild(0).gameObject.SetActive(flag);
    }
    
    public bool GetDodge()
    {
        return dodging;
    }
    
    protected List<APlatformNode> GetPath(GameObject target)
    {
        print(_defaultgravityscale);
        _aStar = new AStar(_defaultgravityscale,jumpforce,movespeed);
        pathInfo = _aStar.Execute(mapInfo, transform, target.transform);
        return pathInfo;
    }
    
    protected float[] GetRunUpInfo(float distanceY, Collider2D from,Collider2D to)
    {
        float runUpDistance = 0;
        int jumpTimes = 0;
        print(jumpHeight+" is jump height");
        if (distanceY >= jumpHeight)
        {
            //跳跃两次，因为一次跳跃的高度不够。不需要管间隙，因为必须跳跃两次。
            var freeHangTime = Mathf.Sqrt(2*(jumpHeight * 2 - distanceY) / (_defaultgravityscale*10));
            runUpDistance = movespeed *(jumpAscentTime * 2 + freeHangTime);
            jumpTimes = 2;
        }
        else
        {
            var singleJumpLimit = jumpAscentTime * movespeed;
            var gapDistance = BasicCalculation.HasGap(from, to);
            if (gapDistance > singleJumpLimit && distanceY >= 0)
            {
                //也是跳跃两次，但是是因为第一次跳跃不够远。跳法为二连跳后自由落体。
                var freeHangTime = Mathf.Sqrt(2*(jumpHeight * 2 - distanceY) / (_defaultgravityscale*10));
                runUpDistance = movespeed *(jumpAscentTime * 3 + freeHangTime);
                jumpTimes = 2;
            }
            else if(gapDistance <= singleJumpLimit && distanceY >= 0)
            {
                //跳跃一次，因为距离够远。
                var freeHangTime = Mathf.Sqrt(2*(jumpHeight - distanceY) / (_defaultgravityscale*10));
                runUpDistance = movespeed *(jumpAscentTime + freeHangTime);
                jumpTimes = 1;
                print("从平台"+from.name+"跳到平台"+to.name+"，跳跃1次");
            }
            else if (gapDistance > 0 && distanceY < 0)
            {
                //自由落体的时间
                var freeHangTime = Mathf.Sqrt(2*( - distanceY) / (_defaultgravityscale*10));
                var hangDistance = movespeed * freeHangTime;
                var jumpHangTime = Mathf.Sqrt(2*(jumpHeight - distanceY) / (_defaultgravityscale*10));
                var jumpDistanceAfterSingleJump = movespeed * (jumpAscentTime + jumpHangTime);
                var jumpDistacneAfterDoubleJump = movespeed * (jumpAscentTime * 3 + jumpHangTime);
                if (gapDistance <= hangDistance)
                {
                    //可以自由落体到达
                    runUpDistance = freeHangTime * movespeed;
                    jumpTimes = 0;
                }else if (gapDistance <= jumpDistanceAfterSingleJump)
                {
                    //跳一次，跳法为先自由落体到对应Y轴位置，再跳一次。
                    runUpDistance = jumpDistanceAfterSingleJump;
                    jumpTimes = 1;
                }
                else
                {
                    //跳两次，跳法为先自由落体到对应Y轴位置，再跳两次。
                    runUpDistance = jumpDistacneAfterDoubleJump;
                    jumpTimes = 2;
                }

            }
            else
            {
                //没有间隙。且在下方，直接自由落体。
                var freeHangTime = Mathf.Sqrt(2*( - distanceY) / (_defaultgravityscale*10));
                runUpDistance = freeHangTime * movespeed;
                jumpTimes = 0;
                //print(freeHangTime);
            }
        } 
        return new float[] {runUpDistance, jumpTimes};
        
    }

    public void SetFlashBody(bool flag)
    {
        flashBody.SetActive(flag);
        var flashWeapon = weaponObject.transform.Find("Flash").gameObject;
        flashWeapon.SetActive(flag);
    }

    public override void SetActionUnable(bool flag)
    {
        hurt = flag;
    }

    public override void ResetGravityScale()
    {
        rigid.gravityScale = _defaultgravityscale;
    }
}
