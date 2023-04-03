using System;
using System.Collections;
using System.Collections.Generic;
using GameMechanics;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class NpcController : MonoBehaviour
{
    protected static float SightHeight = 0.95f;
    protected static float FeetHeight = -1.3f;
    /// <summary>
    /// ActionMode决定当前NPC的行为模式。默认情况下为FollowMode。
    /// </summary>
    protected enum ActionMode
    {
        /// 在该模式下，NPC会主动寻找玩家锁定的敌人进行攻击，如果没有可以攻击到的敌人，则转为FollowMode。
        AttackMode,
        /// 在该模式下，NPC会时刻跟随玩家，在该模式下，只要玩家没有进行攻击指令，npc就不会主动攻击。玩家进行攻击指令后，npc会转为AttackMode。
        FollowMode,
        /// 一般接收到躲避信号会立刻转为EvadeMode,此时保存最后一次的攻击目标，等到目标附近判定安全后再转为AttackMode。
        EvadeMode
    }
    
    public enum MainRoutineType
    {
        None,
        MoveTowardTarget,
        MoveToSafePoint,//Evade用
        MoveToPlatform,//Evade用
        Attack
    }

    protected StandardCharacterController ac;
    protected AStar _aStar;
    protected List<Platform> mapInfo;

    protected List<APlatformNode> pathInfo;
    protected GameObject currentTarget;
    protected GameObject playerGameObject;
    [SerializeField]protected MainRoutineType currentMainRoutineType = MainRoutineType.None;
    protected TargetAimer playerTargetAimer;
    protected APlatformNode currentNode;
    protected List<Collider2D> ignoreList = new List<Collider2D>();

    public StandardCharacterController.OnTask OnMoveFinished;
    protected Coroutine currentMoveCoroutine;
    
    [SerializeField]protected bool isAction = false;
    //[SerializeField]protected bool isAttackMode = true;
    [SerializeField]protected ActionMode actionMode = ActionMode.FollowMode;
    public bool isEvadeMode => actionMode == ActionMode.EvadeMode;
    public bool isAttackMode => actionMode == ActionMode.AttackMode;
    public bool isFollowMode => actionMode == ActionMode.FollowMode;
    public MainRoutineType CurrentMainRoutineType => currentMainRoutineType;

    //Speicfic features
    [SerializeField] protected List<float> skillCD = new();
    protected List<float> skillCDTimer = new();
    protected float safeAttackDistance = 2f; //安全攻击距离
    protected float minAttackActiveDistance = 4f;
    protected float maxAttackActiveDistance = 6f; //敌人脱锁距离
    

    protected void Start()
    {
        ac = GetComponent<StandardCharacterController>();
        playerGameObject = GameObject.Find("PlayerHandle");
        playerTargetAimer = playerGameObject.GetComponentInChildren<TargetAimer>();
        PlayerInput.OnPressAttack += ReceiveAttackSignal;
        //ac.OnAttackInterrupt += StopCurrentMoveAction;

        mapInfo = BattleStageManager.InitMapInfo();
        _aStar = new AStar(ac._defaultgravityscale,ac.jumpforce,ac.movespeed);
        _aStar.Init(mapInfo);
        
        if(currentTarget != null)
            GetPath(currentTarget);
        
        OnMoveFinished += StopCurrentAction;
        //ac._groundSensor.OnGroundEnter += OnFallToGround;
        foreach(var cd in skillCD)
        {
            skillCDTimer.Add(cd);
        }
        skillCDTimer[0] = 0;
        skillCDTimer[1] = 0;
    }

    protected void OnDestroy()
    {
        //ac.OnAttackInterrupt -= StopCurrentMoveAction;
        OnMoveFinished -= StopCurrentAction;
        PlayerInput.OnPressAttack -= ReceiveAttackSignal;
        //ac._groundSensor.OnGroundEnter -= OnFallToGround;
    }

    protected virtual void Update()
    {
        TickSkillCD();
        print(actionMode + ": "+ currentMainRoutineType);

        if (!isAction)
        {
            if (actionMode == ActionMode.FollowMode)
            {
                currentTarget = playerGameObject;
                
                if (Math.Abs(currentTarget.transform.position.x - transform.position.x) > maxAttackActiveDistance ||
                    Math.Abs(currentTarget.transform.position.y - transform.position.y) > ac.jumpHeight)
                {


                    currentMoveCoroutine = StartCoroutine(MoveTowardsTarget(playerGameObject, 3, 999));
                    currentMainRoutineType = MainRoutineType.MoveTowardTarget;
                }
                else return;
            }
            else if(actionMode == ActionMode.AttackMode)
            {
                if (!playerTargetAimer.ReachableEnemies.Contains(currentTarget))
                {
                    print(currentTarget.name + " is not reachable");
                    //currentTarget = null;
                    actionMode = ActionMode.FollowMode;
                }


                if (currentTarget!=null && currentTarget!=playerGameObject)
                {
                    if (CheckDistanceXDoubleDirection( minAttackActiveDistance))
                    {
                        currentMoveCoroutine = StartCoroutine(DoAttack(currentTarget));
                        currentMainRoutineType = MainRoutineType.Attack;
                    }
                    //else if(CheckDistanceXDoubleDirection( maxAttackActiveDistance))
                    else if(playerTargetAimer.ReachableEnemies.Contains(currentTarget))
                    {
                        currentMoveCoroutine = 
                            StartCoroutine(MoveTowardsTarget(currentTarget, minAttackActiveDistance, 999));
                        currentMainRoutineType = MainRoutineType.MoveTowardTarget;
                    }
                    else
                    {
                        actionMode = ActionMode.FollowMode;
                        currentTarget = null;
                        if (currentMoveCoroutine != null && ac.SubMoveRoutine == null)
                        {
                            StopCoroutine(currentMoveCoroutine);
                            currentMoveCoroutine = null;
                            currentMainRoutineType = MainRoutineType.None;
                        }
                    }
                }
                else
                {
                    currentTarget = playerGameObject;
                    if (Math.Abs(currentTarget.transform.position.x - transform.position.x) > maxAttackActiveDistance)
                    {
                        currentMoveCoroutine = StartCoroutine(MoveTowardsTarget(currentTarget, 3, 999));
                        currentMainRoutineType = MainRoutineType.MoveTowardTarget;
                    }

                    
                    //else currentTarget = null;
                }

                //如果周边没有敌人
            }
            else
            {
                //TODO:完善躲避模式Evade Mode.在没有当前动作的情况下需要执行的动作。
                
                //等待目标可以行动
                if(ac.anim.GetBool("isGround")==false || ac.hurt)
                    return;
                
                if (currentMoveCoroutine != null)
                {
                    
                    return;
                }
                
                
                //if (IsSafe(transform.position,LayerMask.GetMask("AttackEnemy")))
                if(IsSafe())
                {
                    APlatformNode accessibleNode = null;
                    print("是安全的");
                    if(!IsReachable(currentTarget, out accessibleNode))
                    {
                        //TODO:过不去目标。
                        print("但是目标不可达");
                        ac?.StartMove(0);
                        
                        if (accessibleNode == null)
                        {
                            print("目标就在当前平台");
                            return;
                        }
                        else if(accessibleNode.parent == null)
                        {
                            ac.TurnMove(accessibleNode.pos);
                            //TODO:如果目标不可达，且没有父节点，说明只能在当前平台上移动。
                            print(accessibleNode.platform.collider.name+"没有父节点");
                            if(Math.Abs(accessibleNode.pos.x - transform.position.x) < 0.5f)
                            {
                                print("不需要移动");
                                return;
                            }
                            currentMoveCoroutine = StartCoroutine(MoveToPointOnPlatform(accessibleNode.pos.x));
                            currentMainRoutineType = MainRoutineType.MoveToSafePoint;
                            print("safe point is "+accessibleNode.pos.x);
                            print("return at line 194");
                            return;
                        }


                        currentMoveCoroutine = StartCoroutine(MoveTowardsTarget(currentTarget, 3, 999));
                        currentMainRoutineType = MainRoutineType.MoveToPlatform;
                        print("return at line 209,可以到达的平台节点为"+accessibleNode.platform.collider.name);
                        // currentMoveCoroutine = StartCoroutine(MoveToSpecificNode(accessibleNode));
                        // currentMainRoutineType = MainRoutineType.MoveToPlatform;
                        // print("return at line 209,可以到达的平台节点为"+accessibleNode.platform.collider.name);
                        return;
                    }




                    if (currentTarget == playerGameObject)
                    {
                        actionMode = ActionMode.FollowMode;
                        print("由于IsSafe()判定为True,且目标是玩家并且可达，所以切换到FollowMode");
                    }

                    
                    else
                        actionMode = ActionMode.AttackMode;
                    
                    print("return at line 202");
                    return;
                }
                

                var targetX = FindNearsetSafePoint();
                if (targetX < 999f)
                {
                    currentMoveCoroutine = StartCoroutine(MoveToPointOnPlatform(targetX));
                    currentMainRoutineType = MainRoutineType.MoveToSafePoint;
                    print(targetX+" is safe, move to it");
                }
                else
                {
                    //TODO:跳跃
                    
                    
                    //TODO:万策尽，只能开启跟随模式。
                    actionMode = ActionMode.FollowMode;

                }
                


            }
        }
        else
        {
            //TODO: 里面如果停止currentMoveRoutine协程并返回，必须将isAction设为false。
            //TODO: isAction进行中，打断当前动作。
            
            if (actionMode == ActionMode.EvadeMode)
            {
                if(ac.anim.GetBool("isGround")==false)
                    return;
                
                if (currentMoveCoroutine != null)
                {
                    if (currentMainRoutineType!=MainRoutineType.MoveToSafePoint && currentMainRoutineType!=MainRoutineType.MoveToPlatform)
                    {
                        
                        if (ac.SubMoveRoutine != null)
                        {
                            print("Return because subMoveRoutine is busy.");
                            return;
                            StopCoroutine(ac.SubMoveRoutine);
                            ac.SubMoveRoutine = null;
                        }
                        
                        StopCoroutine(currentMoveCoroutine);
                        isAction = false;
                        ac.StartMove(0);
                        currentMoveCoroutine = null;
                        currentMainRoutineType = MainRoutineType.None;
                        
                    }

                }
                
                return;
                
            }
            
            if(actionMode == ActionMode.FollowMode)
                return;

            
        }
        
    }

    
    public virtual IEnumerator MoveTowardsTarget(GameObject target, float arriveDistance, float maxFollowTime)
    {
        isAction = true;
        yield return new WaitUntil(() =>
            !ac.hurt);
        var targetSensor = target.GetComponentInChildren<IGroundSensable>();
        if (targetSensor == null)
        {
            targetSensor = target.transform.parent.GetComponentInChildren<IGroundSensable>();
        }

        var mySensor = GetComponentInChildren<IGroundSensable>();
        while (maxFollowTime>0)
        {

            if (ac.SubMoveRoutine == null && ac.anim.GetBool("isGround") == false)
            {
                if(currentTarget == null)
                    currentTarget = target;
                if (ac.JumpTime > 0 && currentTarget.transform.position.x -transform.position.x > 1)
                {
                    var targetPlatform = GetAccessiblePlatforms(ac.JumpTime);
                    ac.SubMoveRoutine = StartCoroutine(StruggleInAir(targetPlatform,ac.JumpTime));
                    yield return new WaitUntil(()=>ac.SubMoveRoutine==null);
                }
            }


            //处理失足
            // if (ac.SubMoveRoutine != null)
            // {
            //     StopCoroutine(ac.SubMoveRoutine);
            //     ac.SubMoveRoutine = null;
            // }
            
            //targetSensor.GetCurrentAttachedGroundCol() != null && 
            
            yield return new WaitUntil(() =>
                ac.anim.GetBool("isGround") && !ac.hurt);
            
            // 检查NPC和目标是否处于同一个平台上
            if (ac.CheckTargetStandOnSameGround(target) == 0) {
                // 计算两者之间的x轴距离
                float distance = Mathf.Abs(transform.position.x - target.transform.position.x);

                if (distance > arriveDistance)
                {
                    ac.TurnMove(target);
                    ac.StartMove(1);
                }
                else
                    ac.StartMove(0);
                
            
                // 如果x轴距离小于arriveDistance，则终止协程
                if (distance <= arriveDistance) {
                    ac.StartMove(0);
                    OnMoveFinished?.Invoke(true);
                    yield break;
                }
            } else {

                while (targetSensor.GetCurrentAttachedGroundCol() == null)
                {
                    yield return null;
                    if(ac.anim.GetBool("isGround")==false)
                        continue;
                    //4.3添加
                    if(transform.position.x >= mySensor.GetCurrentAttachedGroundCol().bounds.max.x ||
                       transform.position.x <= mySensor.GetCurrentAttachedGroundCol().bounds.min.x ||
                       (transform.position.x >= target.transform.position.x && ac.facedir==1) ||
                       (transform.position.x <= target.transform.position.x && ac.facedir==-1))
                        ac.StartMove(0);
                    else ac.StartMove(1);
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
                    ac.StartMove(0);
                    currentMoveCoroutine = null;
                    currentMainRoutineType = MainRoutineType.None;
                    isAction = false;
                    yield break;
                }

                // 沿着路径移动
                foreach (var platform in path) {
                    
                    yield return new WaitUntil(() => ac.anim.GetBool("isGround"));
                    if (platform.platform.collider == ac._groundSensor.GetCurrentAttachedGroundCol())
                    {
                        if (ac.CheckTargetStandOnSameGround(target) == 0)
                        {
                            ac.StartMove(0);
                            break;
                        }
                        continue;
                    }

                    // 移动到当前平台
                    if (ac.SubMoveRoutine != null)
                    {
                        StopCoroutine(ac.SubMoveRoutine);
                        ac.SubMoveRoutine = null;
                    }
                    

                    ac.SubMoveRoutine = StartCoroutine(MoveToPlatform(platform));
                
                    // 等待ac.VerticalMoveRoutine结束或被中断
                    
                    yield return new WaitUntil(()=>ac.SubMoveRoutine==null);
                    
                    

                    ac.StartMove(0);
                    
                    yield return new WaitUntil(() =>
                        ac.anim.GetBool("isGround"));
                    
                    if(ac.CheckTargetStandOnSameGround(target) == 0)
                        break;
                    // 重新计算路径
                    //path = GetPath(target);
                }
            }
        
            yield return null;
        }
        ac.StartMove(0);
        OnMoveFinished?.Invoke(false);
        currentMoveCoroutine = null;
        currentMainRoutineType = MainRoutineType.None;
        isAction = false;
    }

    public virtual IEnumerator MoveToPointOnPlatform(float posX, float biasDistance = 0)
    {
        isAction = true;
        yield return new WaitUntil(() =>
            !ac.hurt);
        if (biasDistance == 0)
        {
            biasDistance = ac.movespeed * 0.1f;
        }

        float PosX2 = posX + biasDistance * ac.facedir;
        var leftX = Mathf.Min(posX, PosX2);
        var rightX = Mathf.Max(posX, PosX2);
        
        while(transform.position.x < leftX || transform.position.x > rightX)
        {
            ac.StartMove(1);
            yield return new WaitForSeconds(0.1f);
            if (transform.position.x < leftX)
            {
                ac.facedir = 1;
            }
            else if (transform.position.x > rightX)
            {
                ac.facedir = -1;
            }
        }
        ac.StartMove(0);
        OnMoveFinished?.Invoke(true);
        currentMoveCoroutine = null;
        currentMainRoutineType = MainRoutineType.None;
        if(currentTarget!=null)
            ac?.TurnMove(currentTarget);
        isAction = false;
        
    }

    public virtual IEnumerator AvoidJump(float jumpTime1 = 0, float jumpTime2 = -1f)
    {
        yield return new WaitUntil(() =>
            ac.anim.GetBool("isGround") && !ac.hurt);
        yield return new WaitForSeconds(jumpTime1);
        ac.Jump();
        if (jumpTime2 > 0)
        {
            yield return new WaitForSeconds(jumpTime2);
            ac.Jump();
        }
        ac.SubMoveRoutine = null;
        
    }






    public virtual IEnumerator DoAttack(GameObject target)
    {
        yield return null;
        actionMode = ActionMode.FollowMode;
    }

    public virtual IEnumerator BeautifulJump(float jumpTime1, float jumpTime2, Vector2 safePoint)
    {
        var distance = -1f;
        Tuple<float, float> _range = new Tuple<float,float>(transform.position.x-0.1f,transform.position.x+0.1f);

        if (IsSafe(safePoint - new Vector2(0, 0.11f),LayerMask.GetMask("AttackEnemy")))
        {
            var hit = Physics2D.Raycast(safePoint, Vector2.left, 999f, LayerMask.GetMask("AttackEnemy"));
            if (safePoint.x - hit.point.x > distance)
            {
                distance = safePoint.x - hit.point.x;
                _range = new Tuple<float, float>(hit.point.x, safePoint.x);
            }
        }
        if (IsSafe(safePoint + new Vector2(0, 0.11f), LayerMask.GetMask("AttackEnemy")))
        {
            var hit = Physics2D.Raycast(safePoint, Vector2.right, 999f, LayerMask.GetMask("AttackEnemy"));
            if (hit.point.x - safePoint.x > distance)
            {
                distance = -(safePoint.x - hit.point.x);
                _range = new Tuple<float, float>(safePoint.x, hit.point.x);
            }
        }

        var currentTime = 0f;
        var leftLimit = Mathf.Max(_range.Item1,BasicCalculation.CheckRaycastedPlatform(gameObject).bounds.min.x);
        var rightLimit = Mathf.Min(_range.Item2,BasicCalculation.CheckRaycastedPlatform(gameObject).bounds.max.x);

        while (currentTime < jumpTime1)
        {
            


            if (transform.position.x > leftLimit && transform.position.x < rightLimit)
            {
                if (BasicCalculation.CheckRaycastedPlatform(playerGameObject) ==
                    BasicCalculation.CheckRaycastedPlatform(gameObject) &&
                    Mathf.Abs(gameObject.transform.position.x - playerGameObject.transform.position.x) >
                    maxAttackActiveDistance)
                {
                    ac.TurnMove(playerGameObject);
                    ac.StartMove(1);
                }
                else
                {
                    ac.StartMove(0);
                }
            }
            else if (transform.position.x <= leftLimit)
            {
                ac.SetFaceDir(1);
                ac.StartMove(1);
            }
            else if (transform.position.x >= rightLimit)
            {
                ac.SetFaceDir(-1);
                ac.StartMove(1);
            }
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }


        ac.Jump();

        while (currentTime < jumpTime2)
        {
            if (jumpTime2 > 99f)
            {
                currentMoveCoroutine = null;
                currentMainRoutineType = MainRoutineType.None;
                yield break;
            }
            if (transform.position.x > leftLimit && transform.position.x < rightLimit)
            {
                if (BasicCalculation.CheckRaycastedPlatform(playerGameObject) ==
                    BasicCalculation.CheckRaycastedPlatform(gameObject) &&
                    Mathf.Abs(gameObject.transform.position.x - playerGameObject.transform.position.x) >
                    maxAttackActiveDistance)
                {
                    ac.TurnMove(playerGameObject);
                    ac.StartMove(1);
                }
                else
                {
                    ac.StartMove(0);
                }
            }
            else if (transform.position.x <= leftLimit)
            {
                ac.SetFaceDir(1);
                ac.StartMove(1);
            }
            else if (transform.position.x >= rightLimit)
            {
                ac.SetFaceDir(-1);
                ac.StartMove(1);
            }
            currentTime += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            currentMainRoutineType = MainRoutineType.None;
        }

        
        ac.Jump();

        yield return new WaitUntil(() => ac.anim.GetBool("isGround"));

        currentMoveCoroutine = null;
        currentMainRoutineType = MainRoutineType.None;
        yield break;
    }
    
    


    #region Vertical Moves
    protected virtual IEnumerator MoveToPlatform(APlatformNode node)
    {
        
        yield return new WaitUntil(() => ac.anim.GetBool("isGround"));
        var targetPlatformCollider = node.platform.collider;
        var currentPlatformCollider = ac._groundSensor.GetCurrentAttachedGroundCol();
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
            ac.facedir = 1;
            ac.StartMove(1);
        }else if(targetPlatformRight.x < transform.position.x)
        {
            ac.facedir = -1;
            ac.StartMove(1);
        }
        
        var runUpInfo = GetRunUpInfo
            (targetPlatformHeight - currentPlatformHeight,currentPlatformCollider,targetPlatformCollider);

        
        //助跑阶段
        while (transform.position.x < targetPlatformLeft.x - runUpInfo[0] ||
               transform.position.x > targetPlatformRight.x + runUpInfo[0])
        {
            yield return null;
            if (transform.position.x < targetPlatformLeft.x - runUpInfo[0] && ac.facedir == -1)
            {
                ac.StartMove(1);
                ac.facedir = 1;
            }
            else if (transform.position.x > targetPlatformRight.x + runUpInfo[0] && ac.facedir == 1)
            {
                ac.StartMove(1);
                ac.facedir = -1;
            }
            print("转身"+runUpInfo[0]);

                    
        }
        
        //TODO:判定障碍物
        //自身和目标平台最近的点坐标
        if (actionMode == ActionMode.EvadeMode)
        {
            APlatformNode targetNode = null;
            while (!NextNodeReachable(node))
            {
                ac.StartMove(0);
                yield return null;
                if (BasicCalculation.CheckOnSameRaycastPlatform(currentTarget, gameObject))
                {
                    ac.SubMoveRoutine = null;
                    yield break;
                }

                if (IsReachable(currentTarget,out targetNode))
                {
                    ac.SubMoveRoutine = null;
                    yield break;
                }
            }
            ac.StartMove(1);
        }



        if (targetPlatformHeight > currentPlatformHeight)
        {
            if (runUpInfo[1] == 2)
            {

                ac.Jump();
                yield return new WaitForFixedUpdate();

                while (ac.rigid.velocity.y > 0)
                {
                    yield return null;
                }
                
                ac.Jump();
                while(!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }




            }
            else if (runUpInfo[1] == 1)
            {
                ac.Jump();
                yield return new WaitForSeconds(ac.jumpAscentTime);
                while(!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
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
                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y
                || ac._groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (ac._groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    ac._groundSensor.StartCoroutine("DisableCollision");
                }

                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y);
                ac.Jump();
                yield return new WaitForSeconds(ac.jumpAscentTime);
                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y);
                ac.Jump();
                while (!ac.anim.GetBool("isGround"))
                {
                    var leftLimit = targetPlatformLeft.x;
                    var rightLimit = targetPlatformRight.x;
                    if (transform.position.x < leftLimit && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > rightLimit && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }

            }
            else if (runUpInfo[1] == 1)
            {
                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y
                                                 || ac._groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (ac._groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    ac._groundSensor.StartCoroutine("DisableCollision");
                }

                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y);
                ac.Jump();
                //yield return new WaitForSeconds(ac.jumpAscentTime);
                while (!ac.anim.GetBool("isGround"))
                {
                    var leftLimit = Mathf.Max(currentTarget.transform.position.x - 1,targetPlatformLeft.x);
                    var rightLimit = Mathf.Min(currentTarget.transform.position.x + 1,targetPlatformRight.x);
                    if (transform.position.x < leftLimit && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > rightLimit && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }
            }
            else
            {
                yield return new WaitUntil(() => transform.position.y <= targetPlatformHeight + relativeHeight
                                                 || ac._groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (ac._groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    ac._groundSensor.StartCoroutine("DisableCollision");
                }
                
                
                yield return new WaitUntil(() => transform.position.y < currentPlatformCollider.bounds.min.y);
                yield return new WaitForSeconds(0.1f);

                while (!ac.anim.GetBool("isGround"))
                {
                    float rightLimit = targetPlatformRight.x;
                    float leftLimit = targetPlatformLeft.x;
                    if (currentTarget != null)
                    {
                        //leftLimit = Mathf.Max(currentTarget.transform.position.x - 1,targetPlatformLeft.x);
                        //rightLimit = Mathf.Min(currentTarget.transform.position.x + 1,targetPlatformRight.x);
                    }

                    if (transform.position.x < leftLimit)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > rightLimit)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }
            }
        }

        yield return new WaitUntil(() => ac.anim.GetBool("isGround"));
        ac.SubMoveRoutine = null;
        print("跳过了");
        
    }

    protected virtual IEnumerator StruggleInAir(APlatformNode node,int jumpTimeLeft)
    {
        var targetPlatformCollider = node.platform.collider;
        Collider2D currentPlatformCollider;
        try
        {
            currentPlatformCollider = ac._groundSensor.GetLastAttachedGroundInfo().GetComponent<Collider2D>();
        }
        catch
        {
            currentPlatformCollider = ac._groundSensor.GetCurrentAttachedGroundCol();
        }


        currentNode = node;
        
        var targetPlatformLeft = node.platform.leftBorderPos;
        var targetPlatformRight = node.platform.rightBorderPos;
        
        var currentPlatformHeight = currentPlatformCollider.bounds.max.y;
        var targetPlatformHeight = node.platform.height;
        if (targetPlatformHeight > currentPlatformHeight)
        {
            if (jumpTimeLeft == 2)
            {

                ac.Jump();
                yield return new WaitForFixedUpdate();

                while (ac.rigid.velocity.y > 0)
                {
                    yield return null;
                }
                
                ac.Jump();
                while(!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }




            }
            else if (jumpTimeLeft == 1)
            {
                ac.Jump();
                yield return new WaitForSeconds(ac.jumpAscentTime);
                while(!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }
            }
        }
        else
        {
            var relativeHeight = transform.position.y - currentPlatformCollider.bounds.max.y;
            if (jumpTimeLeft == 2)
            {
                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y
                || ac._groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (ac._groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    ac._groundSensor.StartCoroutine("DisableCollision");
                }

                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y);
                ac.Jump();
                yield return new WaitForSeconds(ac.jumpAscentTime);
                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y);
                ac.Jump();
                while (!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }

            }
            else if (jumpTimeLeft == 1)
            {
                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y
                                                 || ac._groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (ac._groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    ac._groundSensor.StartCoroutine("DisableCollision");
                }

                yield return new WaitUntil(() => transform.position.y <= currentTarget.transform.position.y);
                ac.Jump();
                //yield return new WaitForSeconds(ac.jumpAscentTime);
                while (!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x && ac.facedir == -1)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x && ac.facedir == 1)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }
            }
            else
            {
                yield return new WaitUntil(() => transform.position.y <= targetPlatformHeight + relativeHeight
                                                 || ac._groundSensor.GetCurrentAttachedGroundInfo() != null);
                if (ac._groundSensor.GetCurrentAttachedGroundInfo() != null)
                {
                    ac._groundSensor.StartCoroutine("DisableCollision");
                }
                

                
                
                yield return new WaitUntil(() => transform.position.y <= currentPlatformCollider.bounds.min.y);

                while (!ac.anim.GetBool("isGround"))
                {
                    if (transform.position.x < targetPlatformLeft.x)
                    {
                        ac.StartMove(1);
                        ac.facedir = 1;
                    }
                    else if (transform.position.x > targetPlatformRight.x)
                    {
                        ac.StartMove(1);
                        ac.facedir = -1;
                    }

                    yield return null;
                }
            }
        }

        yield return new WaitUntil(() => ac.anim.GetBool("isGround"));
        ac.SubMoveRoutine = null;

    }









    #endregion
    
    
    # region Basic Calculation

    /// <summary>
    /// 返回一个float数组，第一个元素是最早起跳距离，第二个元素是跳跃完成的极限距离
    /// </summary>
    /// <returns></returns>
    protected float[] GetRunUpInfo(float distanceY, Collider2D from,Collider2D to)
    {
        float runUpDistance = 0;
        int jumpTimes = 0;
        if (distanceY >= ac.jumpHeight)
        {
            //跳跃两次，因为一次跳跃的高度不够。不需要管间隙，因为必须跳跃两次。
            var freeHangTime = Mathf.Sqrt(2*(ac.jumpHeight * 2 - distanceY) / (ac._defaultgravityscale*10));
            runUpDistance = ac.movespeed *(ac.jumpAscentTime * 2 + freeHangTime);
            jumpTimes = 2;
        }
        else
        {
            var singleJumpLimit = ac.jumpAscentTime * ac.movespeed;
            var gapDistance = HasGap(from, to);
            if (gapDistance > singleJumpLimit && distanceY >= 0)
            {
                //也是跳跃两次，但是是因为第一次跳跃不够远。跳法为二连跳后自由落体。
                var freeHangTime = Mathf.Sqrt(2*(ac.jumpHeight * 2 - distanceY) / (ac._defaultgravityscale*10));
                runUpDistance = ac.movespeed *(ac.jumpAscentTime * 3 + freeHangTime);
                jumpTimes = 2;
            }
            else if(gapDistance <= singleJumpLimit && distanceY >= 0)
            {
                //跳跃一次，因为距离够远。
                var freeHangTime = Mathf.Sqrt(2*(ac.jumpHeight - distanceY) / (ac._defaultgravityscale*10));
                runUpDistance = ac.movespeed *(ac.jumpAscentTime + freeHangTime);
                jumpTimes = 1;
                print("从平台"+from.name+"跳到平台"+to.name+"，跳跃1次");
            }
            else if (gapDistance > 0 && distanceY < 0)
            {
                //自由落体的时间
                var freeHangTime = Mathf.Sqrt(2*( - distanceY) / (ac._defaultgravityscale*10));
                var hangDistance = ac.movespeed * freeHangTime;
                var jumpHangTime = Mathf.Sqrt(2*(ac.jumpHeight - distanceY) / (ac._defaultgravityscale*10));
                var jumpDistanceAfterSingleJump = ac.movespeed * (ac.jumpAscentTime + jumpHangTime);
                var jumpDistacneAfterDoubleJump = ac.movespeed * (ac.jumpAscentTime * 3 + jumpHangTime);
                if (gapDistance <= hangDistance)
                {
                    //可以自由落体到达
                    runUpDistance = freeHangTime * ac.movespeed;
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
                var freeHangTime = Mathf.Sqrt(2*( - distanceY) / (ac._defaultgravityscale*10));
                runUpDistance = freeHangTime * ac.movespeed;
                jumpTimes = 0;
                //print(freeHangTime);
            }
        } 
        return new float[] {runUpDistance, jumpTimes};
        
    }

    protected List<APlatformNode> GetPath(GameObject target)
    {
        _aStar = new AStar(ac._defaultgravityscale,ac.jumpforce,ac.movespeed);
        pathInfo = _aStar.Execute(mapInfo, transform, target.transform);
        return pathInfo;
    }

    protected APlatformNode GetAccessiblePlatforms(int jumpTimeLeft)
    {
        if (currentTarget == null)
            return null;
        if (currentNode != null)
        {
            if (Platform.CanReach(transform.position, currentNode.platform,
                    ac._defaultgravityscale, ac.jumpforce, ac.movespeed, jumpTimeLeft))
            {
                return currentNode;
            }
        }

        var minDistance = 99999f;
        Platform selectedPlatform = null;
        foreach (var pltform in mapInfo)
        {
            if (Platform.CanReach(transform.position, pltform,
                    ac._defaultgravityscale, ac.jumpforce, ac.movespeed, jumpTimeLeft))
            {
                var distance = Mathf.Min(Vector2.Distance(pltform.rightBorderPos , currentTarget.transform.position),
                    Vector2.Distance(pltform.leftBorderPos , currentTarget.transform.position));
                
                if (distance < minDistance)
                {
                    minDistance = distance;
                    selectedPlatform = pltform;
                }
            }
        }

        //Find in mapInfo, if mapInfo contains a platform which has the same collider with the selectedPlatform,
        //return it.
        foreach (var pltform in _aStar.mapNodes)
        {
            if (pltform.platform.collider == selectedPlatform.collider)
            {
                return pltform;
            }
        }

        return null;
    }
    protected List<APlatformNode> GetAccessiblePlatforms()
    {

        //var minDistance = 99999f;
        var listPlatforms = new List<Platform>();
        var listNodes = new List<APlatformNode>();
        foreach (var pltform in mapInfo)
        {
            if (Platform.CanReach(transform.position, pltform,
                    ac._defaultgravityscale, ac.jumpforce, ac.movespeed))
            {
                var distance = Mathf.Min(Vector2.Distance(pltform.rightBorderPos, currentTarget.transform.position),
                    Vector2.Distance(pltform.leftBorderPos, currentTarget.transform.position));

                listPlatforms.Add(pltform);
               
            
            }
        }
        
        if(listPlatforms == null || _aStar.mapNodes == null)
            return null;

        foreach (var pltform in listPlatforms)
        {
            foreach (var node in _aStar.mapNodes)
            {
                if (node.platform.collider == pltform.collider)
                {
                    listNodes.Add(node);
                }
            }
        }

        return listNodes;

    }

    protected void StopCurrentAction(bool flag)
    {
        if (currentMoveCoroutine != null)
        {
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
            currentMainRoutineType = MainRoutineType.None;
        }

        
        isAction = false;
        ac.StartMove(0);
        //currentTarget = null;
    }
    

    protected int IsOnSamePlatform(GameObject target, GameObject self)
    {
        var targetPlatform = target.GetComponentInChildren<IGroundSensable>();
        var selfPlatform = gameObject.GetComponentInChildren<IGroundSensable>();

        var targetCollider = targetPlatform.GetCurrentAttachedGroundCol();
        var selfCollider = selfPlatform.GetCurrentAttachedGroundCol();

        if (targetCollider == null || selfCollider == null)
        {
            Debug.LogWarning("targetCollider or selfCollider is null");
            return 999;
        }

        if (selfCollider.name == targetCollider.name)
        {
            return 0;
        }
        else if(selfCollider.bounds.max.y <= targetCollider.bounds.min.y)
        {
            return 1;
        }
        else if (selfCollider.bounds.min.y > targetCollider.bounds.max.y)
        {
            return -1;
        }
        else
        {
            return 999;
        }

        
    }

    protected float HasGap(Collider2D a, Collider2D b)
    {
        var leftA = a.bounds.min.x;
        var leftB = b.bounds.min.x;
        var rightA = a.bounds.max.x;
        var rightB = b.bounds.max.x;
        
        if(leftA > rightB || leftB > rightA)
        {
            return leftA > rightB ? leftA - rightB : leftB - rightA;
        }
        else
        {
            return 0;
        }
    }

    protected void TickSkillCD()
    {
        for (int i = 0; i < skillCDTimer.Count; i++)
        {
            skillCDTimer[i] -= Time.deltaTime;
            if(skillCDTimer[i] < 0)
            {
                skillCDTimer[i] = 0;
            }
        }
    }

    protected void GetEnemyInRange()
    {
        //寻找以自身为中心，高为2，长为4的矩形内的敌人，敌人的Layermask为"Enemies"。
        var enemies = Physics2D.OverlapBoxAll(transform.position, 
            new Vector2(minAttackActiveDistance*2, 4), 0, LayerMask.GetMask("Enemies"));
        
        
        
        if (enemies.Length>0)
        {
            //把距离自身最近的敌人作为当前目标。
            var minDistance = 9999f;
            GameObject selectedEnemy = null;
            foreach (var enemy in enemies)
            {
                var distance = Vector2.Distance(enemy.transform.position, transform.position);
                if (distance < minDistance)
                {
                    if(BasicCalculation.CheckRaycastedPlatform(gameObject)!=
                       BasicCalculation.CheckRaycastedPlatform(enemy.transform.parent.gameObject))
                        continue;
                    minDistance = distance;
                    selectedEnemy = enemy.gameObject;
                }
            }
            currentTarget = selectedEnemy;
            print("ENEMY INRANGE");
        }
        
        
        
    }

    protected bool CheckDistanceX(GameObject target, float distance, float distanceY = 999f)
    {
        //射线检测，找到射线和目标的交点
        var hit = Physics2D.Raycast(transform.position, ac.facedir*Vector2.right,
            distance,
            LayerMask.GetMask("Enemies"));
        //交点坐标
        
        if(hit.collider == null)
            return false;

        var distanceX = Mathf.Abs(hit.point.x - transform.position.x);
        //print(distanceX);
        
        
        
        return distanceX < distance;
    }

    protected bool CheckDistanceXDoubleDirection(float distance, float distanceY = 999f)
    {
        //射线检测，找到射线和目标的交点,检测左右两边
        var hit = Physics2D.Raycast(transform.position, Vector2.right,
            distance,
            LayerMask.GetMask("Enemies"));
        
        var hit2 = Physics2D.Raycast(transform.position, Vector2.left,
            distance,
            LayerMask.GetMask("Enemies"));
        
        if(hit.collider == null && hit2.collider == null)
            return false;
        return true;

    }

    protected bool CheckSkillUseable()
    {
        foreach (var skill in skillCDTimer)
        {
            if(skill == 0)
                return true;
        }

        return false;
    }

    public void SendEvadeMessage(Collider2D col = null)
    {
        if (actionMode != ActionMode.EvadeMode)
        {
            APlatformNode node;
            if (IsReachable(currentTarget,out node))
            {
                print("因为安全，所以无视闪避信号");
                return;
            }

            if (FindNearsetSafePoint() > 999f)
            {
                print("因为无路可走，所以无视闪避信号,并且禁用col和自身视线的碰撞");
                //禁用col和自身视线的碰撞
                if(ignoreList.Contains(col))
                    return;
            }


            print("由于受到了闪避信号，进入Evade模式");
            actionMode = ActionMode.EvadeMode;
        }
    }

    public void SendForcedEvadeMessageDuringEvade(Collider2D col)
    {

        if (FindNearsetSafePoint() > 999)
        {
            if(ignoreList.Contains(col))
                return;
            print("因为无路可走，所以无视Forced闪避信号");
            return;
        }
        
        
        if (actionMode == ActionMode.EvadeMode && ac.IsMove > 0)
        {
            if(ac.SubMoveRoutine != null)
                StopCoroutine(ac.SubMoveRoutine);
            ac.StartMove(0);
            StopCoroutine(currentMoveCoroutine);
            currentMoveCoroutine = null;
            currentMainRoutineType = MainRoutineType.None;
            isAction = false;
        }
    }

    /// <summary>
    /// 寻找当前平台上最近的安全点
    /// </summary>
    /// <returns></returns>
    protected float FindNearsetSafePoint(Collider2D myPlatform = null, float heightModifier = 0f) 
    {
        if(myPlatform == null)
            myPlatform = BasicCalculation.CheckRaycastedPlatform(gameObject);
        
        var feetModifier = -ac.GetActorHeight();//1.3f
        var sightModifier = 0.95f;
        
        var leftBorder = new Vector2(myPlatform.bounds.min.x,transform.position.y + heightModifier);
        var rightBorder = new Vector2(myPlatform.bounds.max.x,transform.position.y + heightModifier);
        
        if(leftBorder.x < BattleStageManager.Instance.mapBorderL)
            leftBorder.x = BattleStageManager.Instance.mapBorderL + 0.1f;
        if(rightBorder.x > BattleStageManager.Instance.mapBorderR)
            rightBorder.x = BattleStageManager.Instance.mapBorderR - 0.1f;

        var lb_high = leftBorder + new Vector2(0, sightModifier);
        var rb_high = rightBorder + new Vector2(0, sightModifier);
        
        var lb_low = leftBorder + new Vector2(0, feetModifier);
        var rb_low = rightBorder + new Vector2(0, feetModifier);
        //发射从leftBorder到rightBorder的射线，找到最近的安全点

        /*var linecast = Physics2D.LinecastAll(leftBorder,
            rightBorder,
            LayerMask.GetMask("AttackEnemy"));

        //List<BasicCalculation.TriggerIntersection> intersections = new();
        var points = new List<Vector2>();

        foreach (var hit in linecast)
        {
            if (hit.collider is BoxCollider2D)
            {
                points.AddRange(BasicCalculation.GetIntersectionOfBoxCollider(leftBorder,rightBorder,hit.collider as BoxCollider2D));
                //points.AddRange(BasicCalculation.GetIntersectionOfBoxCollider(leftBorder,rightBorder,hit.collider as BoxCollider2D));
            }

            if (hit.collider is PolygonCollider2D)
            {
                points.AddRange(BasicCalculation.GetIntersectionOfPolygonCollider(leftBorder,rightBorder,hit.collider as PolygonCollider2D));
            }

            if (hit.collider is CircleCollider2D)
            {
                points.AddRange(BasicCalculation.GetIntersectionOfCircleCollider(leftBorder,rightBorder,hit.collider as CircleCollider2D));
            }
        }

        
        
        //print(points.Count);
        //将points按照x轴排序
        points.Sort((a, b) => a.x.CompareTo(b.x));
        points.Insert(0,leftBorder);
        points.Add(rightBorder);
        
        
        int layerMask = LayerMask.GetMask("AttackEnemy");
        List<(float, float)> safeZones = new();
        bool leftSafe, rightSafe;
        bool inSafeZone = false;
        float safeZoneStart = leftBorder.x;

        for (int i = 0; i < points.Count; i++)
        {
            if (i == 0)
            {
                rightSafe = IsSafe(points[0] + (rightBorder-leftBorder).normalized*0.1f, layerMask);
                if (rightSafe)
                {
                    safeZoneStart = points[0].x;
                    inSafeZone = true;
                }
                else
                {
                    inSafeZone = false;
                }
            }
            else if (i == points.Count - 1)
            {
                if (inSafeZone)
                {
                    safeZones.Add((safeZoneStart, points[i].x));
                }
            }
            else
            {
                var rightPoint = points[i] + (rightBorder-leftBorder).normalized*0.1f;
                var leftPoint = points[i] - (rightBorder-leftBorder).normalized*0.1f;
                rightSafe = IsSafe(rightPoint, layerMask);
                leftSafe = IsSafe(leftPoint, layerMask);
                if (!inSafeZone && rightSafe && !leftSafe)
                {
                    inSafeZone = true;
                    safeZoneStart = points[i].x;
                }else if (inSafeZone && !rightSafe && leftSafe)
                {
                    inSafeZone = false;
                    safeZones.Add((safeZoneStart, points[i].x));
                }

            }
        }*/
        var safeZonesA = GetSafeZones(lb_high, rb_high);
        var safeZonesB = GetSafeZones(lb_low, rb_low);
        var safeZones = BasicCalculation.GetTupleIntersection(safeZonesA, safeZonesB);

        if (safeZones.Count == 0)
            return 9999f;
        
        //找到safeZones中x最接近transform.position.x的值
        float minDistance = 9999;
        float selectedPointX = transform.position.x;
        bool selectionIsLeft = false;
        

        //输出safeZones
        foreach (var safeZone in safeZones)
        {
            print(safeZone.Item1 + "," + safeZone.Item2);
        }

        foreach (var safeZone in safeZones)
        {
            if(transform.position.x - 1.26f > safeZone.Item1 && transform.position.x + 1.26f < safeZone.Item2)
                return transform.position.x;
            
            if(safeZone.Item2 - safeZone.Item1 <= 2.5f)
                continue;
            
            if (Mathf.Abs(safeZone.Item1 - transform.position.x) < minDistance)
            {
                minDistance = Mathf.Abs(safeZone.Item1 - transform.position.x);
                selectedPointX = safeZone.Item1 ;
                selectionIsLeft = true;
            }
            if (Mathf.Abs(safeZone.Item2 - transform.position.x) < minDistance)
            {
                minDistance = Mathf.Abs(safeZone.Item2 - transform.position.x);
                selectedPointX = safeZone.Item2 ;
                selectionIsLeft = false;
            }
            if (Mathf.Abs(safeZone.Item1 - transform.position.x) == minDistance)
            {
                selectedPointX = Mathf.Abs(safeZone.Item1 - currentTarget.transform.position.x) <
                                 Mathf.Abs(selectedPointX - currentTarget.transform.position.x)
                    ? safeZone.Item1
                    : selectedPointX;

                if (Mathf.Abs(safeZone.Item1 - currentTarget.transform.position.x) <
                    Mathf.Abs(selectedPointX - currentTarget.transform.position.x))
                {
                    selectionIsLeft = true;
                }
            }
            if (Mathf.Abs(safeZone.Item2 - transform.position.x) == minDistance)
            {
                selectedPointX = Mathf.Abs(safeZone.Item2 - currentTarget.transform.position.x) <
                                 Mathf.Abs(selectedPointX - currentTarget.transform.position.x)
                    ? safeZone.Item2 
                    : selectedPointX;
                if (Mathf.Abs(safeZone.Item2 - currentTarget.transform.position.x) <
                    Mathf.Abs(selectedPointX - currentTarget.transform.position.x))
                {
                    selectionIsLeft = false;
                }
            }
        }

        
        return selectedPointX + (selectionIsLeft? 1.26f : -1.26f);
    }

    protected List<(float, float)> GetSafeZones(Vector2 leftBorder, Vector2 rightBorder)
    {
        var linecast = Physics2D.LinecastAll(leftBorder,
            rightBorder,
            LayerMask.GetMask("AttackEnemy"));

        //List<BasicCalculation.TriggerIntersection> intersections = new();
        var points = new List<Vector2>();

        foreach (var hit in linecast)
        {
            if (hit.collider is BoxCollider2D)
            {
                points.AddRange(BasicCalculation.GetIntersectionOfBoxCollider(leftBorder,rightBorder,hit.collider as BoxCollider2D));
                //points.AddRange(BasicCalculation.GetIntersectionOfBoxCollider(leftBorder,rightBorder,hit.collider as BoxCollider2D));
            }

            if (hit.collider is PolygonCollider2D)
            {
                points.AddRange(BasicCalculation.GetIntersectionOfPolygonCollider(leftBorder,rightBorder,hit.collider as PolygonCollider2D));
            }

            if (hit.collider is CircleCollider2D)
            {
                points.AddRange(BasicCalculation.GetIntersectionOfCircleCollider(leftBorder,rightBorder,hit.collider as CircleCollider2D));
            }
        }

        
        
        //print(points.Count);
        //将points按照x轴排序
        points.Sort((a, b) => a.x.CompareTo(b.x));
        points.Insert(0,leftBorder);
        points.Add(rightBorder);
        
        
        int layerMask = LayerMask.GetMask("AttackEnemy");
        List<(float, float)> safeZones = new();
        bool leftSafe, rightSafe;
        bool inSafeZone = false;
        float safeZoneStart = leftBorder.x;

        for (int i = 0; i < points.Count; i++)
        {
            if (i == 0)
            {
                rightSafe = IsSafe(points[0] + (rightBorder-leftBorder).normalized*0.1f, layerMask);
                if (rightSafe)
                {
                    safeZoneStart = points[0].x;
                    inSafeZone = true;
                }
                else
                {
                    inSafeZone = false;
                }
            }
            else if (i == points.Count - 1)
            {
                if (inSafeZone)
                {
                    safeZones.Add((safeZoneStart, points[i].x));
                }
            }
            else
            {
                var rightPoint = points[i] + (rightBorder-leftBorder).normalized*0.1f;
                var leftPoint = points[i] - (rightBorder-leftBorder).normalized*0.1f;
                rightSafe = IsSafe(rightPoint, layerMask);
                leftSafe = IsSafe(leftPoint, layerMask);
                if (!inSafeZone && rightSafe && !leftSafe)
                {
                    inSafeZone = true;
                    safeZoneStart = points[i].x;
                }else if (inSafeZone && !rightSafe && leftSafe)
                {
                    inSafeZone = false;
                    safeZones.Add((safeZoneStart, points[i].x));
                }

            }
        }

        return safeZones;
    }

    /// <summary>
    /// 寻找向目标X轴最近的安全点
    /// </summary>
    /// <returns></returns>
    

    protected bool IsSafe(Vector2 point, int layerMask)
    {
        var col = Physics2D.OverlapPoint(point, layerMask);
        if(col == null)
            return true;
        //print("不安全点"+point);
        return false;
    }

    protected bool IsSafe()
    {
        var targetWithHitSensor = gameObject;
        Collider2D hitSensor;
        if ((hitSensor = targetWithHitSensor.transform.Find("HitSensor")?.GetComponent<Collider2D>()) == null)
            return true;
        else
        {
            //检查hitSensor是否与AttackEnemy层的触发器碰撞
            var isTouchingLayers = hitSensor.IsTouchingLayers(LayerMask.GetMask("AttackEnemy"));
            //print(isTouchingLayers);
            return !isTouchingLayers;
        }
    }

    /// <summary>
    /// 在同一个平台的情况下，能否安全到达目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    protected bool CanReachSafely(GameObject target)
    {
        
        var distance = Vector2.Distance(transform.position, target.transform.position) - minAttackActiveDistance;
        var distance2 = Mathf.Abs(transform.position.x - target.transform.position.x) - minAttackActiveDistance;
        print(distance + "/" + distance2);
        
        //发出一条射线，从transform.position到target.transform.position,检测是否有AttackEnemy层的物体
        var hit = Physics2D.Raycast(transform.position + new Vector3(0,-ac.GetActorHeight(),0), target.transform.position - transform.position,
            distance2,
            LayerMask.GetMask("AttackEnemy"));
        var hit2 = Physics2D.Raycast(transform.position + new Vector3(0,.95f,0), target.transform.position - transform.position,
            distance2,
            LayerMask.GetMask("AttackEnemy"));
        
        
        //修改了触发距离
        
        if (hit.collider == null && hit2.collider == null)
        {
            print("由于CanReachSafely，返回true");
            return true;
        }

        

        return false;
    }


    /// <summary>
    /// 判断下一个节点是否有障碍物
    /// </summary>
    /// <returns></returns>
    protected bool NextNodeReachable(APlatformNode nextNode)
    {
        RaycastHit2D lineCast;
        var leftBorder = nextNode.platform.leftBorderPos;
        var rightBorder = nextNode.platform.rightBorderPos;
        if (transform.position.x > leftBorder.x && transform.position.x < rightBorder.x)
        {
            lineCast = Physics2D.Linecast(transform.position,
                new Vector2(transform.position.x, nextNode.platform.height), LayerMask.GetMask("AttackEnemy"));
            
        }else if(transform.position.x <= leftBorder.x)
        {
            lineCast = Physics2D.Linecast(transform.position,
                new Vector2(leftBorder.x, nextNode.platform.height), LayerMask.GetMask("AttackEnemy"));
        }
        else if (transform.position.x >= rightBorder.x)
        {
            lineCast = Physics2D.Linecast(transform.position,
                new Vector2(rightBorder.x, nextNode.platform.height), LayerMask.GetMask("AttackEnemy"));
        }
        else return false;
        if (lineCast.collider != null)
        {
            return false;
        }
        return true;

    }


    protected bool IsReachable(GameObject target, out APlatformNode accessibleNode)
    {
        accessibleNode = null;
        
        if (target == null)
            return false;
        
        var pathInfo = GetPath(target);
        if (pathInfo == null)
        {
            accessibleNode = _aStar.GetAPlatformNode(BasicCalculation.CheckRaycastedPlatform(target));
        }

        //print(pathInfo[0].platform.collider.name);
        if (pathInfo==null)
        {
            // if(CanReachSafely(target))
            // {
            //     return true;
            // }
            //
            // return false;


            var direction = (target.transform.position.x - transform.position.x > 0 ) ? Vector2.right : Vector2.left;
            var rayLower = Physics2D.Raycast(transform.position+new Vector3(0,FeetHeight), direction,
                Mathf.Abs(transform.position.x - target.transform.position.x),
                LayerMask.GetMask("AttackEnemy"));
            var rayHigher = Physics2D.Raycast(transform.position+new Vector3(0,SightHeight), direction,
                Mathf.Abs(transform.position.x - target.transform.position.x),
                LayerMask.GetMask("AttackEnemy"));
            if (rayLower.collider == null && rayHigher.collider == null)
            {
                return true;
            }
            Vector2 point;
            //print("Ray:" + rayHigher.point.x+"/" + rayLower.point.x);

            if (direction == Vector2.left)
            {
                point = new Vector2(Mathf.Max(rayHigher.point.x, rayLower.point.x), transform.position.y);

            }
            else
            {
                point = new Vector2(Mathf.Min(rayHigher.point.x, rayLower.point.x), transform.position.y);
            }
            if (rayHigher.collider == null)
            {
                point = new Vector2(rayLower.point.x, transform.position.y);
            }
            else if (rayLower.collider == null)
            {
                point = new Vector2(rayHigher.point.x, transform.position.y);
            }

            accessibleNode.pos = point - direction * 1.26f;
            
            return false;
        }
        else
        {
            var actorHeight = ac.GetActorHeight();
            RaycastHit2D rayHorizontal,rayVertical;
            //不在同一个平台上
            for (int i = 0; i < pathInfo.Count - 1; i++)
            {
                var direction = (pathInfo[i+1].pos.x - pathInfo[i].pos.x > 0 ) ? Vector2.right : Vector2.left;
                rayHorizontal = Physics2D.Linecast(new Vector2(pathInfo[i].pos.x, pathInfo[i].pos.y+actorHeight),
                    new Vector2(pathInfo[i+1].pos.x, pathInfo[i].pos.y+actorHeight),
                    LayerMask.GetMask("AttackEnemy"));
                rayVertical = Physics2D.Linecast(new Vector2(pathInfo[i+1].pos.x + 0.5f, pathInfo[i].pos.y+actorHeight),
                    new Vector2(pathInfo[i+1].pos.x + 0.5f, pathInfo[i+1].pos.y+actorHeight),
                    LayerMask.GetMask("AttackEnemy"));
                //TODO:改动
                if(rayHorizontal.collider != null )
                {
                    accessibleNode = pathInfo[i];
                    accessibleNode.pos = rayHorizontal.point - direction * 1.26f;
                    return false;
                }else if (rayVertical.collider != null)
                {
                    accessibleNode = pathInfo[i];
                    accessibleNode.pos = new Vector2(pathInfo[i + 1].pos.x, pathInfo[i].pos.y + actorHeight);
                    print("垂直入射无法进行");
                    return false;
                }

            }
            accessibleNode = pathInfo[pathInfo.Count - 1];
            rayHorizontal = Physics2D.Linecast(new Vector2(accessibleNode.pos.x, target.transform.position.y),
                new Vector2(target.transform.position.x, target.transform.position.y),
                LayerMask.GetMask("AttackEnemy"));
            if(rayHorizontal.collider != null)
            {
                var direction = (target.transform.position.x - transform.position.x > 0 ) ? Vector2.right : Vector2.left;
                accessibleNode.pos = rayHorizontal.point - direction * 1.26f;
                return false;
            }
            
            accessibleNode.pos = target.transform.position;
            
            return true;

        }
    }

    /// <summary>
    /// 能否在攻击生效前到达目的地？（前提：同一个平台）
    /// </summary>
    /// <param name="targetPosition"></param>
    /// <returns></returns>
    protected bool CanReachSafelyInTime(Vector2 targetPosition)
    {
        //发出射线，从transform.position到targetposition,检测是否有AttackEnemy层的collider
        var hits = Physics2D.RaycastAll(targetPosition, -targetPosition + (Vector2)transform.position,
            Vector2.Distance(transform.position, targetPosition),
            LayerMask.GetMask("AttackEnemy"));
        
        foreach (var hit in hits)
        {
            var outpoint = hit.point; //离开攻击范围的点
            var outTime = Mathf.Abs(transform.position.x - outpoint.x) / ac.movespeed;
            if (outTime > hit.collider.GetComponent<EnemyAttackHintBar>().warningTimeLeft)
                return false;
        }

        return true;

    }

    protected bool CanReachSafelyInTime(Platform targetPlatform)
    {
        //不是绝对的，因为y轴被忽视了。
        if (transform.position.x > targetPlatform.rightBorderPos.x)
        {
            //ray为从transform.position指向rightBorderPos的射线
            Vector2 direction = ((Vector2)transform.position - targetPlatform.rightBorderPos).normalized;


            var ray = Physics2D.Raycast(transform.position, direction,
                Vector2.Distance(transform.position, targetPlatform.rightBorderPos), LayerMask.GetMask("AttackEnemy"));


            if (ray.collider != null)
            {
                var outpoint = ray.point; //离开攻击范围的点
                var outTime = Mathf.Abs(transform.position.x - outpoint.x) / ac.movespeed;
                if (outTime > ray.collider.GetComponent<EnemyAttackHintBar>().warningTimeLeft)
                    return false;
            }
        }
        else if (transform.position.x < targetPlatform.leftBorderPos.x)
        {
            Vector2 direction = (targetPlatform.leftBorderPos - (Vector2)transform.position).normalized;
            var ray = Physics2D.Raycast(transform.position, direction,
                Vector2.Distance(transform.position, targetPlatform.leftBorderPos), LayerMask.GetMask("AttackEnemy"));
            if (ray.collider != null)
            {
                var outpoint = ray.point; //离开攻击范围的点
                var outTime = Mathf.Abs(transform.position.x - outpoint.x) / ac.movespeed;
                if (outTime > ray.collider.GetComponent<EnemyAttackHintBar>().warningTimeLeft)
                    return false;
            }
        }
        else
        {
            if (BasicCalculation.CheckRaycastedPlatform(gameObject).bounds.max.y < targetPlatform.height)
            {
                var ray = Physics2D.Raycast(new Vector2(transform.position.x, targetPlatform.height + 2f), Vector2.down,
                    targetPlatform.height - transform.position.y + 2f, LayerMask.GetMask("AttackEnemy"));
                if (ray.collider != null)
                {
                    var outpoint = ray.point; //离开攻击范围的点
                    var averageVerticalSpeed = ac.jumpHeight / ac.jumpAscentTime;
                    var outTime = Mathf.Abs(transform.position.y - outpoint.y) / averageVerticalSpeed;
                    if (outTime > ray.collider.GetComponent<EnemyAttackHintBar>().warningTimeLeft)
                        return false;
                }
            }
            else
            {
                var ray = Physics2D.Raycast(new Vector2(transform.position.x, targetPlatform.height + 2f), Vector2.up,
                    -targetPlatform.height + transform.position.y + 2f, LayerMask.GetMask("AttackEnemy"));
                if (ray.collider != null)
                {
                    var outpoint = ray.point; //离开攻击范围的点
                    var fallHeight = Mathf.Abs(transform.position.y - outpoint.y);
                    var outTime = Mathf.Sqrt(2 * fallHeight / ac._defaultgravityscale);
                    if (outTime > ray.collider.GetComponent<EnemyAttackHintBar>().warningTimeLeft)
                        return false;
                }
            }
        }

        return true;
    }


    protected Tuple<float,float> CanAvoidByJump(float safePointInAir1,float safePointInAir2, out Vector2 outPoint)
    {
        if (
            (safePointInAir1 < 999f &&
             CanReachSafelyInTime(new Vector2(safePointInAir1, transform.position.y)))
            || (safePointInAir2 < 999f &&
                CanReachSafelyInTime(new Vector2(safePointInAir2, transform.position.y)))
        )
        {


            //TODO: 进行跳跃走位
            //StartCoroutine(华丽的跳跃)
            RaycastHit2D underRay1;
            float heightAboveGround, safePointInAir;
            EnemyAttackHintBar hintBar;
            

            if (safePointInAir1 < 999f)
            {
                underRay1 = BasicCalculation.GetPositionOfAttackEnemyBelow(
                    new Vector2(safePointInAir1, transform.position.y + ac.jumpHeight - 1));
                safePointInAir = safePointInAir1;
                outPoint = new Vector2(safePointInAir, ac.jumpHeight - 1);
            }
            else
            {
                underRay1 = BasicCalculation.GetPositionOfAttackEnemyBelow(
                    new Vector2(safePointInAir2, transform.position.y + 2 * ac.jumpHeight - 1));
                safePointInAir = safePointInAir2;
                outPoint = new Vector2(safePointInAir, 2*ac.jumpHeight - 1);
            }

            hintBar = underRay1.collider.GetComponent<EnemyAttackHintBar>();
            heightAboveGround = underRay1.point.y;


            var jumpTimeTuple = GetJumpTimeWhenAvoidAttackUnderfeet(hintBar, heightAboveGround);
            if (jumpTimeTuple.Item1 > 0 && jumpTimeTuple.Item2 > 0)
            {
                return jumpTimeTuple;
            }


        }

        outPoint = new Vector2(9999f, 9999f);
        return new Tuple<float, float>(-1, -1);

    }


    protected Tuple<float, float> GetJumpTimeWhenAvoidAttackUnderfeet(EnemyAttackHintBar hintBar, float highestPointY)
    {
        Tuple<float,float> result = new Tuple<float, float>(-1,-1);

        var attackLastTime = hintBar.attackLastTime;
        var warningTimeLeft = hintBar.warningTimeLeft;
        var needAscendDistance = highestPointY - transform.position.y;
        if (needAscendDistance >= ac.jumpHeight)
        {
            //一段跳躲不掉，只能二段跳
            var needAscendOnDoubleJump = needAscendDistance - ac.jumpHeight;
            //物体以ac.jumpSpeed的速度上升，求出上升距离大于needAscendOnDoubleJump的所需时间
            var ascendOverAttackTime = Mathf.Sqrt(2 * needAscendOnDoubleJump / ac.jumpforce);
            var maxAvoidTime = (ac.jumpAscentTime - ascendOverAttackTime) * 2;
            var avoidNeedTime = ac.jumpAscentTime + ascendOverAttackTime;
            if (avoidNeedTime >= warningTimeLeft || maxAvoidTime <= attackLastTime)
            {
                return result;
            }
            else
            {
                result = new Tuple<float, float>(avoidNeedTime - 0.1f, avoidNeedTime - 0.1f + ac.jumpAscentTime);
                return result;
            }
        }
        else
        {
            //一段跳高度可以躲掉
            var ascendOverAttackTime = Mathf.Sqrt(2 * needAscendDistance / ac.jumpforce);
            var avoidNeedTime = ascendOverAttackTime;
            var maxAvoidTime = (ac.jumpAscentTime - ascendOverAttackTime) * 2 + (ac.jumpAscentTime * 2);
            var minAvoidTime = maxAvoidTime - ac.jumpAscentTime * 2;
            if (avoidNeedTime >= warningTimeLeft || maxAvoidTime <= attackLastTime)
            {
                return result;
            }
            else if (avoidNeedTime < warningTimeLeft && attackLastTime < minAvoidTime)
            {
                //只需要跳一下
                return new Tuple<float, float>(avoidNeedTime - 0.1f, 9999f);
            }
            else
            {
                result = new Tuple<float, float>(avoidNeedTime - 0.1f, avoidNeedTime - 0.1f + (ac.jumpAscentTime - ascendOverAttackTime) * 2);
                return result;
            }
        }

    }

    protected float Get2ndJumpTimeWhenAvoidAttackUnderfeet(float height, float duration)
    {
        float result = -1;
        if (height >= ac.jumpHeight)
        {
            return ac.jumpAscentTime;
        }
        else
        {
            //跳跃高度超过height所需的时间
            var ascendOverAttackTime = Mathf.Sqrt(2 * height / ac.jumpforce);
            var safeTimeBySingleJump = 2*(ac.jumpAscentTime - ascendOverAttackTime);
            if (duration < safeTimeBySingleJump)
                return -1;
            else return ac.jumpAscentTime + safeTimeBySingleJump;
        }
        
        
    }


    # endregion

    void ReceiveAttackSignal()
    {
        if (playerTargetAimer.EnemyWatched != null)
        {
            //TODO:去攻击目标敌人
            if (CanReachSafely(playerTargetAimer.EnemyWatched))
            {
                actionMode = ActionMode.AttackMode;
            }
            
            if(currentTarget == playerGameObject || !playerTargetAimer.ReachableEnemies.Contains(currentTarget))
                currentTarget = playerTargetAimer.EnemyWatched;
        }
    }

    public void ReceiveEvadeSignal(float height, float duration,Collider2D col)
    {
        if(ac == null)
            return;
        
        print("收到躲避信号");
        
        print(height);
        print(duration);
        print(ac.jumpAscentTime);
        
        if(height < 0 || duration > ac.jumpAscentTime * 2)
            return;

        if (!ignoreList.Contains(col))
        {
            ignoreList.Add(col);
        }

        var jumpTime2ND = Get2ndJumpTimeWhenAvoidAttackUnderfeet(height, duration);
        
        if(ac.SubMoveRoutine != null)
            return;

        if (currentMoveCoroutine != null)
        {
            if (actionMode == ActionMode.FollowMode)
                ac.SubMoveRoutine = StartCoroutine(AvoidJump(0, jumpTime2ND));
        }
        else
        {
            ac.SubMoveRoutine = StartCoroutine(AvoidJump(0,jumpTime2ND));return;
        }
        print("因为未能满足条件，未能执行躲避");

    }
    
    public void DeleteIgnoreList(Collider2D col)
    {
        if (ignoreList.Contains(col))
        {
            ignoreList.Remove(col);
        }
    }



}
