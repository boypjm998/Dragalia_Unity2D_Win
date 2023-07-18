using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;


public class EnemyMoveController_HB03 : EnemyMoveManager
{

    [SerializeField] protected GameObject Helper_Zethia;
    
    [Header("HB03_Animation")] public Vector3 baseRotationBeforeAiming;

    public AnimationClip comboAnimClip;
    
    [SerializeField] protected Transform jChestTransform;
    [SerializeField] protected Transform muzzleTransform;
    protected bool lateUpdateCalled;

    [SerializeField] private List<APlatformNode> PlatformNodes = new();
    [SerializeField] private Vector2 currentTargetPosition;
    
    [SerializeField] protected List<NPCNavigateAnchorSensor> anchorSensor = new();

    public bool transportable => FindObjectsOfType<Projectile_C001_2_Boss>().Length > 0;

    VoiceControllerEnemy voice;

    protected enum VoiceGroup
    {
        Combo = 0,
        DashAttack = 1,
        RollAttack = 2,
        DriveBuster = 3,
        AlchemicEnhancement = 4,
        OtherworldPortal = 5,
        Heal = 6,
        OverdriveBuster = 7,
        AlchemicGrenade = 8,
        OtherworldGate = 9,
        BlessingOfGale = 10,
        SummonZethia = 11,
        Defeated = 12

    }



    protected GameObject moveableNavigator
    {
        get
        {
            return anchorSensor.Find
                (x => x.gameObject.name.Contains("Mobile")).gameObject;
        }
    }

    public AllMapNode allNodeForDebug;

    protected int currentMovePattern = 0;


    public bool debug;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        base.Start();
        ac = GetComponent<EnemyControllerHumanoidHigh>();
        PlatformNodes = AStar.GetAllNodes();
        anchorSensor = FindObjectsOfType<NPCNavigateAnchorSensor>().ToList();
        allNodeForDebug = new AllMapNode();
        allNodeForDebug.InitInfo();
        voice = GetComponentInChildren<VoiceControllerEnemy>();
    }

    private void Update()
    {
        if (debug)
        {
            debug = false;
            //print(Vector2.SignedAngle(currentTargetPosition-(Vector2)transform.position,Vector2.right));

             //_behavior.currentMoveAction = StartCoroutine((ac as EnemyControllerHumanoidHigh).MoveTowardsTargetNavigatorWithDesignedRoutine(
            //     nameof(COND_StandardAttackAimCheck), _behavior.targetPlayer,1,56f));
            _behavior.currentAttackAction = StartCoroutine(HB03_Action11());
        }

        
    }

    

    private IEnumerator TestComboAim()
    {
        anim.Play("combo1");

        yield return null;

        
        jChestTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
            
        yield return null;
        
        //var lastRotationEuler = new Vector3(0,0, -120f);
        var lastRotationEuler2 = new Vector3(baseRotationBeforeAiming.x, baseRotationBeforeAiming.y, -45f);
        //jChestTransform.DOLocalRotate(lastRotationEuler, 0.15f).SetEase(Ease.InOutSine);
        var sequence = DOTween.Sequence();

        sequence.Append(jChestTransform.DOLocalRotate(lastRotationEuler2, 0.25f).SetEase(Ease.OutExpo)
            .SetUpdate(UpdateType.Late)).Append
            (jChestTransform.DOLocalRotate(baseRotationBeforeAiming, 0.85f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Late)).SetUpdate(UpdateType.Late);


        sequence.Play();


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f);
        //_tweener.Complete();
        Instantiate(projectile1, muzzleTransform.transform.position,Quaternion.identity);
        
        
        lateUpdateCalled = false;
        yield return new WaitUntil(() => lateUpdateCalled);
        jChestTransform.localRotation = Quaternion.Euler(lastRotationEuler2);
        //_tweener = jChestTransform.DOLocalRotate(baseRotationBeforeAiming, 0.85f).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Late);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.96f);
        
        
        
        
        
        
        jChestTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
        
        


        anim.Play("idle");
        

    }

    protected void OnAimExit()
    {
        jChestTransform.localRotation = Quaternion.Euler(new Vector3(0, 0, -90));
    }

    private void LateUpdate()
    {
        lateUpdateCalled = true;
    }

    public GameObject GetAnchoredSensor(string name)
    {
        return anchorSensor.Find
        (x =>
            x.gameObject.name.Equals(name))?.gameObject;
    }


    // Usages:
    // MoveTowardsTargetNavigatorWithDesignedRoutine in EnemyControllerHumanoidHigh
    public bool COND_StandardAttackAimCheck(GameObject target)
    {
        var hitSensor = target.GetComponent<ActorBase>().HitSensor;
        
        //如果目标的最高点比自己的位置还要低，从自身的位置到目标的最高点发射一条长为25f的射线。
        var targetHighestPoint = hitSensor.bounds.max;
        var selfPosition = transform.position;
        
        //以自身为中心 向目标发送25长度的射线检测，LayerMask为Characters
        var ray = new Ray(selfPosition, targetHighestPoint - selfPosition);
        var hit = Physics2D.Raycast
            (ray.origin, ray.direction,
                25f, LayerMask.GetMask("Characters"));
        
        //如果射线没有击中目标，返回false

        //如果自身位置到目标的X轴距离小于10f，返回false
        if (Mathf.Abs(targetHighestPoint.x - selfPosition.x) < 10f)
        {
            currentMovePattern = 2;
            return false;
        }
        
        //如果射线和水平线的夹角的绝对值大于30度，返回false
        var angle = Vector2.Angle(Vector2.right * ac.facedir, ray.direction);
        if (angle > 90)
        {
            angle = 180 - angle;
        }
        if (angle > 30f || hit.collider==null)
        {
            currentMovePattern = 1;
            return false;
        }
        
        currentMovePattern = 0;
        return true;





    }

    public override APlatformNode GetNextRoutineNode(ref GameObject anchorTarget)
    {
        //anchorTarget = anchorSensor[DebugIndexForAnchorSensor].gameObject;
        //return PlatformNodes[DebugIndexForPlatformNode];

        var target = _behavior.targetPlayer;
        var mySensor = GetComponentInChildren<IGroundSensable>();


        //TODO: 如果是因为距离过远或者角度不够。
        if (currentMovePattern == 1)
        {
            //距离敌人最远的最近节点移动

            GameObject nearestPoint = gameObject;
            float minDistance = float.MaxValue;

            //遍历所有anchorSensor，找到离自己最近的那个



            foreach (var sensor in anchorSensor)
            {
                // 计算敌人与预设位置之间的距离
                float distanceOfMe = Vector2.Distance(sensor.transform.position, transform.position);
                float distance = Vector2.Distance(target.transform.position, sensor.transform.position);

                // 判断距离是否在有效范围内
                if (distance > 10 && distance < 25)
                {
                    // 判断夹角是否小于30度
                    Vector3 direction = (sensor.transform.position - target.transform.position).normalized;
                    var dirRay = transform.position.x < target.transform.position.x ? Vector2.right : Vector2.left;
                    float angle = Vector2.Angle(dirRay, direction);
                    if (angle > 90)
                    {
                        angle = 180 - angle;
                    }

                    print("angle=" + angle);
                    if (angle < 30)
                    {
                        // 更新最近的有效位置
                        if (distanceOfMe < minDistance)
                        {
                            nearestPoint = sensor.gameObject;
                            minDistance = distanceOfMe;
                        }
                    }
                }
                print(nearestPoint.name+"isNearest");
            }

            if (mySensor.GetCurrentAttachedGroundInfo() != null)
            {
                if (mySensor.GetCurrentAttachedGroundInfo().name.Contains("Ground"))
                {
                    float pointX;
                    if (target.transform.position.x < transform.position.x)
                    {
                        // 如果敌人在左边，从敌人的右边，与水平夹角30度斜向下发射一条射线，并检测与Ground层的交点
                        var ray = new Ray(target.transform.position, new Vector3(1.73f, -1, 0).normalized);
                        var hit = Physics2D.Raycast
                        (ray.origin, ray.direction,
                            25f, LayerMask.GetMask("Ground"));
                        //将PointX设置为射线与Ground层的交点的X坐标-1和BattleStageManager.mapBorderR的较小值
                        pointX = Mathf.Min(hit.point.x - 1, BattleStageManager.Instance.mapBorderR);
                    }
                    else
                    {
                        // 如果敌人在右边，从敌人的左边，与水平夹角30度斜向下发射一条射线，并检测与Ground层的交点
                        var ray = new Ray(target.transform.position, new Vector3(-1.73f, -1, 0).normalized);
                        var hit = Physics2D.Raycast
                        (ray.origin, ray.direction,
                            25f, LayerMask.GetMask("Ground"));
                        //将PointX设置为射线与Ground层的交点的X坐标+1和BattleStageManager.mapBorderL的较小值
                        pointX = Mathf.Max(hit.point.x + 1, BattleStageManager.Instance.mapBorderL);
                    }

                    //如果nearestPoint到GameObject的距离大于new Vector2(pointX, transform.position.y)到GameObject的距离，
                    //则将nearestPoint设置为new Vector2(pointX, transform.position.y)
                    if (Vector2.Distance(nearestPoint.transform.position, transform.position) >
                        Vector2.Distance(new Vector2(pointX, transform.position.y), transform.position))
                    {
                        moveableNavigator.transform.position =
                            new Vector2(pointX, moveableNavigator.transform.position.y);
                        nearestPoint = moveableNavigator;
                    }
                }
            }

            //ref anchorTarget
            anchorTarget = nearestPoint;

            GameObject platform;
            try{
                platform = nearestPoint.GetComponent<NPCNavigateAnchorSensor>().GetCurrentAttachedGroundInfo();
           
            }
            catch
            { 
                platform = mySensor.GetCurrentAttachedGroundInfo();
            }
            
            

            return AStar.GetNodeOfName(platform.name);


        }
        //TODO: 如果因为玩家距离过近。
        else if (currentMovePattern == 2)
        {
            var myPlatform = BasicCalculation.CheckRaycastedPlatform(gameObject);
            switch (myPlatform.name)
            {
                case "GroundPic":
                {
                    if (target.transform.position.x > transform.position.x)
                    {
                        if (target.transform.position.x - BattleStageManager.Instance.mapBorderL < 10 &&
                            target.transform.position.y < transform.position.y + 5)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("LeftUpperR")).gameObject;
                            return AStar.GetNodeOfName("LeftUpper");
                        }
                        else if (Mathf.Abs(transform.position.x) < 8f)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("MiddleM")).gameObject;
                            return AStar.GetNodeOfName("Middle");
                        }
                        else
                        {
                            anchorTarget = moveableNavigator;
                            var centerPointer = transform.position.x < 0?1:-1;
                            moveableNavigator.transform.position
                                = new Vector2(target.transform.position.x + 10.5f*centerPointer,
                                    moveableNavigator.transform.position.y);
                            return AStar.GetNodeOfName("GroundPic");
                        }
                    }
                    else
                    {
                        if (BattleStageManager.Instance.mapBorderR - target.transform.position.x < 10 &&
                            target.transform.position.y < transform.position.y + 5)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("RightUpperL")).gameObject;
                            return AStar.GetNodeOfName("RightUpper");
                        }
                        else if (Mathf.Abs(transform.position.x) < 8f)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("MiddleM")).gameObject;
                            return AStar.GetNodeOfName("Middle");
                        }
                        else
                        {
                            anchorTarget = moveableNavigator;
                            var centerPointer = transform.position.x < 0?1:-1;
                            moveableNavigator.transform.position
                                = new Vector2(target.transform.position.x + 10.5f*centerPointer,
                                    moveableNavigator.transform.position.y);
                            return AStar.GetNodeOfName("GroundPic");
                        }
                    }

                    break;
                }
                case "Middle":
                {
                    if (transform.position.x > target.transform.position.x)
                    {
                        anchorTarget = anchorSensor.Find
                        (x =>
                            x.gameObject.name.Equals("RightUpperR")).gameObject;
                        return AStar.GetNodeOfName("RightUpper");
                    }
                    else
                    {
                        anchorTarget = anchorSensor.Find
                        (x =>
                            x.gameObject.name.Equals("LeftUpperR")).gameObject;
                        return AStar.GetNodeOfName("LeftUpper");
                    }

                    break;
                }
                case "LeftUpper":
                {
                    if (target.transform.position.x > transform.position.x)
                    {
                        if (target.transform.position.x - myPlatform.bounds.min.x > 10f)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("LeftUpperL")).gameObject;
                            return AStar.GetNodeOfName("LeftUpper");
                        }
                        else if(target.transform.position.y > transform.position.y - 1.3f)
                        {
                            anchorTarget = moveableNavigator;
                            moveableNavigator.transform.position =
                                new Vector2(0, moveableNavigator.transform.position.y);
                            return AStar.GetNodeOfName("GroundPic");
                        }
                        else
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("MiddleM")).gameObject;
                            return AStar.GetNodeOfName("Middle");
                        }
                    }
                    else
                    {
                        if (target.transform.position.x < myPlatform.bounds.center.x)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("MiddleM")).gameObject;
                            return AStar.GetNodeOfName("Middle");
                        }
                        else
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("RightUpperL")).gameObject;
                            return AStar.GetNodeOfName("RightUpper");
                        }
                    }
                }
                case "RightUpper":
                {
                    if (target.transform.position.x < transform.position.x)
                    {
                        if (myPlatform.bounds.max.x - target.transform.position.x > 10f)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("RightUpperR")).gameObject;
                            return AStar.GetNodeOfName("RightUpper");
                        }
                        else if(target.transform.position.y > transform.position.y - 1.3f)
                        {
                            anchorTarget = moveableNavigator;
                            moveableNavigator.transform.position =
                                new Vector2(0, moveableNavigator.transform.position.y);
                            return AStar.GetNodeOfName("GroundPic");
                        }
                        else
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("MiddleM")).gameObject;
                            return AStar.GetNodeOfName("Middle");
                        }
                    }
                    else
                    {
                        if (target.transform.position.x > myPlatform.bounds.center.x)
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("MiddleM")).gameObject;
                            return AStar.GetNodeOfName("Middle");
                        }
                        else
                        {
                            anchorTarget = anchorSensor.Find
                            (x =>
                                x.gameObject.name.Equals("LeftUpperR")).gameObject;
                            return AStar.GetNodeOfName("LeftUpper");
                        }
                    }
                }
                default: return AStar.GetNodeOfName(myPlatform.name);

            }
        }

        var currentPlatform = BasicCalculation.CheckRaycastedPlatform(gameObject);
        return AStar.GetNodeOfName(currentPlatform.name);
    }


    /// <summary>
    /// Simple Double Combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action01()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("combo1");
        
        yield return null;

        var angle = Combo_Lock(0.3f);


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f);
        //_tweener.Complete();
        Combo_Single(angle);
        
        voice.PlayMyVoice(0);
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.65f);
        
        lateUpdateCalled = false;
        yield return new WaitUntil(() => lateUpdateCalled);
        jChestTransform.localRotation = Quaternion.Euler(baseRotationBeforeAiming);
        
        anim.Play("combo1",0,0);
        yield return null;

        angle = Combo_Lock();


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f);
        //_tweener.Complete();
        Combo_Single(angle);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.96f);
        
        lateUpdateCalled = false;
        yield return new WaitUntil(() => lateUpdateCalled);
        jChestTransform.localRotation = Quaternion.Euler(baseRotationBeforeAiming);
        
        //anim.Play("idle");
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Scattered Combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action02()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("combo1");
        
        yield return null;

        var angle = Combo_Lock();


        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f);
        //_tweener.Complete();
        Combo_Scattered(angle);
        voice.PlayMyVoice(0);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        lateUpdateCalled = false;
        yield return new WaitUntil(() => lateUpdateCalled);
        jChestTransform.localRotation = Quaternion.Euler(baseRotationBeforeAiming);

        //anim.Play("idle");
        anim.Play("idle");
        QuitAttack();
    }

    /// <summary>
    /// Dash Attack
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action03()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("dash");
        //voice?.PlayMyVoice(1);

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.237f);
        
        DashAttack();
        voice.PlayMyVoice(1);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.25f);
        if (anim.GetBool("isGround"))
        {
            (ac as EnemyControllerHumanoid).InertiaMove(0.3f);
        }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);
        ac.anim.Play("idle");
        
        QuitAttack();
    }


    /// <summary>
    /// Combo Dodge
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action04()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("roll");
        voice.PlayMyVoice(2);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (7f/40f));
        
        RollMove(_behavior.targetPlayer);
        (ac as EnemyControllerHumanoid).dodging = true;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (13f/40f));
        
        RollAttack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
        
        ac.SetFaceDir(-ac.facedir);
        (ac as EnemyControllerHumanoid).dodging = false;

        //anim.Play("idle");
        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// DriveBuster
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action05()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB03_Action05");
        
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3f);
        
        yield return new WaitForSeconds(1f);
        
        
        
        anim.Play("s1");
        voice.PlayMyVoice(3);

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (48f/103f));
        
        DriveBuster();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Overdrive Buster
    /// </summary>
    /// <returns></returns>
    
    public IEnumerator HB03_Action06()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB03_Action06");
        
        
        ac.TurnMove(_behavior.targetPlayer);
        
        var container = OverdriveBuster_Start();
        
        yield return new WaitForSeconds(1.17f);
        voice.PlayMyVoice(7);
        
        anim.Play("s1_boost");
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);

        yield return null;
        //32帧开始瞄准
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (32f/146f));
        
        OverdriveBuster_Aiming(container);
        container.GetComponent<EnemyAttackHintBarRotater>().SetRotateSpeed(10);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (50f/146f));
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,30,15);
        ac.SetCounter(true);
        ac.SetKBRes(100);
        

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (70f/146f));

        OverDriveBuster_Pushforce(); 
        
        //126帧开始恢复
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= (126f/146f));
        
        _tweener?.Kill();
        ac.SetKBRes(999);
        ac.SetCounter(false);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// AlchemicEnhancement
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action07()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB03_Action07");
        yield return new WaitForSeconds(0.5f);
        
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("s2");

        yield return null;
        voice.PlayMyVoice(4);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (34f/90f));
        
        AlchemicEnhancement();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();
    }
    
    /// <summary>
    /// Alchemic Grenade
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action08()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        bossBanner?.PrintSkillName("HB03_Action08");
        
        ac.TurnMove(_behavior.targetPlayer);

        yield return new WaitForSeconds(1f);
        
        anim.Play("s2_boost");
        voice.PlayMyVoice(8);
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);

        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (42f/136f));
        
        AlchemicGrenade_Muzzle();
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer.transform.position,
                RangedAttackFXLayer.transform,2f);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (60f/136f));
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,30,15);
        var targetCol = BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer);
        AlchemicGrenade_Launch(_behavior.targetPlayer,targetCol);
        
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();
    }


    /// <summary>
    /// Transport
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action09()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter();
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("HB03_Action09");
        
        var hint = GenerateWarningPrefab(WarningPrefabs[0],transform.position+new Vector3(1.5f*ac.facedir,-.3f,0),
            Quaternion.identity,MeeleAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>();
        
        yield return new WaitForSeconds(hint.warningTime-0.3f);
        
        
        
        anim.Play("s3");

        yield return null;
        voice.PlayMyVoice(5);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (38f/114f));
        
        OtherworldPortal_Attack();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (42f/114f));
        
        OtherworldPortal_Pushforce();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (92f/114f));
        
        OtherworldPortal_Open();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();
    }

    public IEnumerator HB03_Action09T()
    {
        yield return new WaitUntil(() => !ac.hurt);
        ac.OnAttackEnter(999);
        
        OtherworldPortal_Transport();
        
        QuitAttack();
    }




    /// <summary>
    /// 异界之门
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action10(GameObject target = null)
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        
        
        bossBanner?.PrintSkillName("HB03_Action10");
        
        _statusManager.RemoveTimerBuff((int)BasicCalculation.BattleCondition.AlchemicCatridge);
        
        var hint = GenerateWarningPrefab(WarningPrefabs[1],target != null?target.transform.position:new Vector3(0,7,0),
            Quaternion.identity,RangedAttackFXLayer.transform).GetComponent<EnemyAttackHintBar>();
        if(target != null)
            hint.GetComponent<EnemyAttackHintBarChaser>().target = target;
        

        yield return null;

        yield return new WaitForSeconds(3.5f);

        anim.Play("s3_boost");
        
        voice.BroadCastMyVoice(9);
        
        StageCameraController.SwitchOverallCamera();
        Invoke(nameof(SwitchMainCamera),4f);

        yield return new WaitForSeconds(1.5f);
        
        OtherWorldGate_Blast(hint.transform.position+new Vector3(0,2,0));
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,30,15);
        
        //110-140
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > (110f/148f));

        StartCoroutine(ac.HorizontalMoveFixedTime
            (transform.position.x + 0.5f*ac.facedir, 0.2f, "s3_boost", Ease.Linear));
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        
        yield return null;

        QuitAttack();
        
    }

    public IEnumerator HB03_Action11()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
      
        bossBanner?.PrintSkillName("HB03_Action11");
        voice.BroadCastMyVoice(10);

        yield return new WaitForSeconds(1f);
        
        anim.Play("buff");
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f);
        
        BlessingOfGale();
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        
        
        anim.Play("idle");
        
        QuitAttack();
    }

    public IEnumerator HB03_Action12()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        _behavior.breakable = false;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
      
        bossBanner?.PrintSkillName("HB03_Action12");
        voice?.BroadCastMyVoice((int)VoiceGroup.SummonZethia);
        
        yield return new WaitForSeconds(1f);

        var zethia = SummonZethia();
        var dmgCutBuff = new TimerBuff((int)(BasicCalculation.BattleCondition.DamageCut),
            50, -1, 1,8103301);
        dmgCutBuff.dispellable = false;
        _statusManager.ObtainTimerBuff(dmgCutBuff);
        
        yield return null;
        StageCameraController.SwitchMainCamera();
        yield return null;
        StageCameraController.SwitchMainCameraFollowObject(zethia);
        yield return null;
        var zethia_status = zethia.GetComponent<StatusManager>();
        zethia_status.OnHPBelow0 += () =>
        {
            _statusManager.RemoveSpecificTimerbuff((int)(BasicCalculation.BattleCondition.DamageCut),
                8103301);
            (_behavior as HB03_BehaviorTree).summoned = false;


            zethia_status.OnHPBelow0 = null;
        };
        _statusManager.OnHPBelow0 += () =>
        {
            if (zethia_status.currentHp > 0)
            {
                zethia_status.currentHp = 0;
                zethia.GetComponent<DragaliaEnemyBehavior>().StopAction();
                zethia_status.OnHPBelow0 = null;
            }
            //.OnHPBelow0 = null;
        };
        yield return new WaitForSeconds(1.5f);
        StageCameraController.SwitchMainCameraFollowObject(_behavior.viewerPlayer);
        yield return new WaitForSeconds(1.5f);
        
        _behavior.breakable = true;
        
        QuitAttack();


    }



    /// <summary>
    /// forceField
    /// </summary>
    /// <returns></returns>
    public IEnumerator HB03_Action13()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
      
        bossBanner?.PrintSkillName("HB03_Action13");
        
        StageCameraController.SwitchOverallCamera();
        Invoke(nameof(SwitchMainCamera),3f);

        yield return new WaitForSeconds(1f);
        
        anim.Play("buff");
        
        yield return new WaitForSeconds(0.1f);
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        ForcefieldEnhancement();
        
        anim.Play("idle");
        
        QuitAttack();
    }











    protected float GetLockAngle(float angle)
    {

        var weaponAngle = ac.facedir == 1 ? angle : (180 - angle);
        //weaponAngle = angle;
        if (weaponAngle > 180)
        {
            weaponAngle = weaponAngle - 360;
        }

        //print(weaponAngle);
        
        if(weaponAngle > 40)
        {
            weaponAngle = 40;
        }
        else if(weaponAngle < -40)
        {
            weaponAngle = -40;
        }

        return weaponAngle;
    }

    protected virtual float Combo_Lock(float duration = 0.85f)
    {
        //get the angle between gameObject and target.gameObject
        //DON'T USE BasicCalculation CLASS!
        
        var target = _behavior.targetPlayer;
        var targetPosition = target.transform.position;
        var myPosition = transform.position;
        
        var angle = Vector2.SignedAngle(Vector2.right, targetPosition - myPosition);

        var weaponAngle = ac.facedir == 1 ? angle : (180 - angle);
        //weaponAngle = angle;
        if (weaponAngle > 180)
        {
            weaponAngle = weaponAngle - 360;
        }

        print(weaponAngle);
        
        if(weaponAngle > 40)
        {
            weaponAngle = 40;
        }
        else if(weaponAngle < -40)
        {
            weaponAngle = -40;
        }
        var lastRotationEuler2 = new Vector3(baseRotationBeforeAiming.x, baseRotationBeforeAiming.y, -weaponAngle+baseRotationBeforeAiming.z);
        //jChestTransform.DOLocalRotate(lastRotationEuler, 0.15f).SetEase(Ease.InOutSine);
        var sequence = DOTween.Sequence();

        sequence.Append(jChestTransform.DOLocalRotate(lastRotationEuler2, 0.25f).SetEase(Ease.OutExpo)
            .SetUpdate(UpdateType.Late)).Append
            (jChestTransform.DOLocalRotate(baseRotationBeforeAiming, duration).SetEase(Ease.InOutSine).SetUpdate(UpdateType.Late)).SetUpdate(UpdateType.Late);


        sequence.Play();

        return weaponAngle;

    }

    protected void Combo_Single(float angle)
    {
        angle = Mathf.Round(angle);
        if (angle > 40)
        {
            angle = 40;
        }
        if (angle < -40)
        {
            angle = -40;
        }

        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.gameObject.transform);
        
        var shotPoint = 
            new Vector2
            (transform.position.x + 2 * ac.facedir * Mathf.Cos(angle*Mathf.Deg2Rad),
                transform.position.y + 2f * Mathf.Sin(angle*Mathf.Deg2Rad));

        var muzzle =
            Instantiate(projectile1, shotPoint, Quaternion.Euler(0, ac.facedir==1?0:180, angle), container.transform);
        
        var bullet = Instantiate(projectile2,shotPoint, Quaternion.Euler(0, ac.facedir==1?0:180, angle), container.transform);
        
        //container.GetComponent<AttackContainer>().InitAttackContainer(1,false);
        bullet.GetComponent<AttackFromEnemy>().enemySource = gameObject;
        bullet.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        
    }

    protected void Combo_Scattered(float angle)
    {
        angle = Mathf.Round(angle);
        if (angle > 40)
        {
            angle = 40;
        }
        if (angle < -40)
        {
            angle = -40;
        }

        var container = Instantiate(attackContainer, transform.position, Quaternion.identity,
            RangedAttackFXLayer.gameObject.transform);
        
        var shotPoint = 
            new Vector2
            (transform.position.x + 1.5f * ac.facedir * Mathf.Cos(angle*Mathf.Deg2Rad),
                transform.position.y + 1.5f * Mathf.Sin(angle*Mathf.Deg2Rad));

        var muzzle =
            Instantiate(projectile1, shotPoint, Quaternion.Euler(0, ac.facedir==1?0:180, angle), container.transform);

        for (int i = 0; i < 5; i++)
        {
            var temp_bullet = Instantiate(projectile3,shotPoint, Quaternion.Euler(0, ac.facedir==1?0:180, angle-10+5*i), container.transform);
            //container.GetComponent<AttackContainer>().InitAttackContainer(1,false);
            temp_bullet.GetComponent<AttackFromEnemy>().enemySource = gameObject;
            temp_bullet.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        }

        var pushbackPositionX = transform.position.x - 2f * ac.facedir * Mathf.Abs(Mathf.Cos(angle));
        var border = pushbackPositionX;
        var groundCol = (ac as EnemyControllerHumanoid)._groundSensor?.GetCurrentAttachedGroundCol();

        float finalPosX = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(pushbackPositionX,0)).x;
        
        if (groundCol != null)
        {
            if (ac.facedir == 1)
            {
                border = (ac as EnemyControllerHumanoid).
                    _groundSensor.GetCurrentAttachedGroundCol().bounds.min.x;
                finalPosX = Mathf.Max(border, pushbackPositionX);
            }
            else
            {
                border = (ac as EnemyControllerHumanoid).
                    _groundSensor.GetCurrentAttachedGroundCol().bounds.max.x;
                finalPosX = Mathf.Min(border, pushbackPositionX);
            }
        }
        
        finalPosX = BattleStageManager.Instance.OutOfRangeCheck(new Vector2(finalPosX,0)).x;

        _tweener = transform.DOMoveX
            (finalPosX, 0.2f).SetEase(Ease.OutSine);
        


    }

    protected void DashAttack()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, MeeleAttackFXLayer.transform);
        var proj = InstantiateMeele(projectile4, transform.position, container);
    }
    protected void RollMove(GameObject target)
    {
        if (transform.position.x < target.transform.position.x)
        {
            (ac as EnemyControllerHumanoid).InertiaMove(0.4f,-1);
        }
        else
        {
            (ac as EnemyControllerHumanoid).InertiaMove(0.4f,1);
        }
    }
    protected void RollAttack()
    {
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);

        var muzzlePosition = new Vector3(transform.position.x + 0.7f * ac.facedir,
            transform.position.y - 0.3f);
        var muzzle =
            Instantiate(projectile5, muzzlePosition,
                Quaternion.Euler(0, ac.facedir==1?0:180, 0), container.transform);
        
        for (int i = 0; i < 3; i++)
        {
            var temp_bullet = Instantiate(projectile6,muzzlePosition,
                Quaternion.Euler(0, ac.facedir==1?0:180, -3f+3f*i), container.transform);
            //container.GetComponent<AttackContainer>().InitAttackContainer(1,false);
            temp_bullet.GetComponent<AttackFromEnemy>().enemySource = gameObject;
            temp_bullet.GetComponent<AttackFromEnemy>().firedir = ac.facedir;
        }
    }
    protected void DriveBuster()
    {
        var container = Instantiate(attackContainer,
            transform.position,
            Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var muzzlePosition = new Vector3(transform.position.x + 1.4f * ac.facedir,
            transform.position.y - 0.3f);
        
        var muzzle = 
            Instantiate(projectile1, muzzlePosition,
                Quaternion.Euler(0, ac.facedir==1?0:180, 0), container.transform);
        
        float[] angleX = { 0.6f, 0.7f, 0.8f, 0.88f, 0.96f, 0.96f, 0.88f, 0.8f, 0.7f, 0.6f };
        float[] angleY = { 0.4f, 0.3f, 0.2f, 0.12f, 0.04f, -0.04f, -0.12f, -0.2f, -0.3f, -0.4f };
        List<GameObject> projectiles = new();

        for (int i = 0; i < 10; i++)
        {
            projectiles.Add(Instantiate(projectile7,
                muzzlePosition + new Vector3(Random.Range(-.5f, .5f),
                    ac.facedir * Random.Range(-.5f, .5f)),Quaternion.identity,container.transform));
            
            var attack = projectiles[i].GetComponent<AttackFromEnemy>();
            var homing = projectiles[i].GetComponent<HomingAttack>();
            
            homing.angle = new Vector2(ac.facedir*angleX[i],angleY[i]);
            homing.target = _behavior.targetPlayer.transform;
            
            attack.enemySource = gameObject;
            attack.firedir = ac.facedir;
            attack.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                    41.6f,21f,
                    1),120);
        }

        StartCoroutine((ac as EnemyControllerHumanoid).HorizontalMoveInteria(0.2f,
            3,-ac.facedir));

    }

    protected GameObject OverdriveBuster_Start()
    {
        var container = Instantiate(projectile8,transform.position-new Vector3(0,0.3f,0),
            ac.facedir==1?Quaternion.identity:Quaternion.Euler(0,0,180),
            RangedAttackFXLayer.transform);
        
        container.GetComponent<RelativePositionRetainer>().SetParent(transform);
        
        
        var sealedContainer = container.GetComponent<EnemySealedContainer>();
        sealedContainer.SetEnemySource(gameObject);
        sealedContainer.SetTarget(_behavior.targetPlayer);
        
        var rotater = container.GetComponent<EnemyAttackHintBarRotater>();
        rotater.center = ac.facedir == 1? 0 : 180;
        rotater.target = _behavior.targetPlayer;
        
        container.transform.GetChild(0).GetComponent<AttackFromEnemy>().
            AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Flashburn,
                41.6f,21f,
                1),120);
        
        
        return container;
    }

    protected void OverdriveBuster_Aiming(GameObject hintAngle)
    {
        var angle = -GetLockAngle(hintAngle.transform.eulerAngles.z)-3; 
        print(angle);
        var startAngle = jChestTransform.localEulerAngles;

        DOVirtual.Float(0, 1f, 1.6f,null).OnUpdate(()=>
        {
            jChestTransform.localEulerAngles = new Vector3(0, 0,
                -GetLockAngle(hintAngle.transform.eulerAngles.z) - 3);
        }).SetUpdate(UpdateType.Late);
        
    }

    protected void OverDriveBuster_Pushforce()
    {
        StartCoroutine((ac as EnemyControllerHumanoid).HorizontalMoveInteria(0.5f,
            3,-ac.facedir));
        
        
    }
    protected void AlchemicEnhancement()
    {
        _statusManager.ObtainTimerBuffs((int)BasicCalculation.BattleCondition.AlchemicCatridge,
            -1,
        3,3,-1);
    }

    protected void AlchemicGrenade_Muzzle()
    {
        //42/136
        InstantiateDirectional(projectile10,transform.position+new Vector3(1.6f*ac.facedir,0.4f,0),
            MeeleAttackFXLayer.transform,ac.facedir,1,1);
    }
    protected void AlchemicGrenade_Launch(GameObject target, Collider2D targetCol)
    {
        //60/136
        var container = Instantiate(attackContainer, transform.position, Quaternion.identity, RangedAttackFXLayer.transform);
        
        
        
        var proj = Instantiate(projectile9,transform.position+new Vector3(1.6f*ac.facedir,0.4f,0),
            Quaternion.identity,container.transform);

        var proj_controller = proj.GetComponent<Projectile_C001_1_Boss>();
        proj_controller.enemySource = gameObject;

        if (target.transform.position.x > transform.position.x && ac.facedir == -1)
        {
            proj_controller.SetVelocity(new Vector2(-5,7));
            proj_controller.SetContactPlatform(null);
            return;
        }
        else if (target.transform.position.x < transform.position.x && ac.facedir == 1)
        {
            proj_controller.SetVelocity(new Vector2(5,7));
            proj_controller.SetContactPlatform(null);
            return;
        }



        var targetPosition = new Vector2(target.transform.position.x,targetCol.bounds.max.y);

        var angle = 60;

        var gravityScaleProj = 60;
        
        
        
        float distance = Vector2.Distance(proj.transform.position, targetPosition);
        float height = targetPosition.y - proj.transform.position.y;
        float width = Mathf.Abs(targetPosition.x - proj.transform.position.x);

        if (height < 1)
        {
            angle = 56;
        }

        
        
        
        
        float angleInRadians = angle * Mathf.Deg2Rad;
        float v =  Mathf.Sqrt((gravityScaleProj * distance * distance) /
                              (2 * (distance * Mathf.Tan(angleInRadians) - height) * Mathf.Cos(angleInRadians) * Mathf.Cos(angleInRadians)));

        
        float t = distance / (v * Mathf.Cos(angleInRadians));
        
        // print("落地时间"+t+"秒");
        //
        // //print(distance+"sudu");
        // print(height+"高度差");
        // print(angleInRadians+"rad");
        
        var modifierX = (1 + height * 0.02f) > 1f ? 1f : (1 + height * 0.02f);
        if(modifierX < 0.98f)
            modifierX = 0.98f;
        
        
        
        float vx = v * Mathf.Cos(angleInRadians) * modifierX;
        float vy = v * Mathf.Sin(angleInRadians);
        
        //print("vx:"+vx+" vy:"+vy);
        
        proj_controller.SetContactPlatform(targetCol);

        if (vx < 4)
        {
            proj_controller.SetContactPlatform(null);
            vx = 4;
        }

        vx *= ac.facedir;

        if(vy < 6)
            vy = 6;
        
        proj_controller.SetVelocity(new Vector2(vx,vy));
        

    }


    protected void OtherworldPortal_Attack()
    {
        //38
        
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            MeeleAttackFXLayer.transform);

        var proj = InstantiateMeele(projectilePoolEX[0], transform.position + new Vector3(ac.facedir * 2, -0.4f),
            container);

    }

    protected void OtherworldPortal_Pushforce()
    {
        //40
        OverDriveBuster_Pushforce();
    }

    protected void OtherworldPortal_Open()
    {
        //92
        
        //114
        Vector3 gatePosition = new Vector3(transform.position.x + 12*ac.facedir, transform.position.y+0.3f);
        gatePosition = BattleStageManager.Instance.OutOfRangeCheck(gatePosition);
        InstantiateRanged(projectilePoolEX[1], gatePosition, RangedAttackFXLayer, 1);
    }

    protected void OtherworldPortal_Transport()
    {
        Projectile_C001_2_Boss.Instance.Transport(transform);
    }

    protected void OtherWorldGate_Blast(Vector3 position)
    {
        var container = Instantiate(attackContainer,transform.position,Quaternion.identity,
            RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(projectilePoolEX[2], position, container, 1);

        if (_behavior.difficulty > 2)
            proj.GetComponent<ObjectInvokeDestroy>().destroyTime += 5f;

    }

    protected void BlessingOfGale()
    {
        var difficulty = _behavior.difficulty;
        
        var fx = Instantiate(projectilePoolEX[3], transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);

        var kbimmuneBuff = new TimerBuff((int)BasicCalculation.BattleCondition.KnockBackImmune,
            -1, 30, 1);
        
        var defBuff = new TimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            difficulty>2?200:100, 30,100);
        
        var flashburnPunisher = new TimerBuff((int)BasicCalculation.BattleCondition.FlashburnPunisher,
            difficulty>2?200:50, 30,100);
        
        var critBuff = new TimerBuff((int)BasicCalculation.BattleCondition.CritRateBuff,
            10, -1,100);
        
        defBuff.dispellable = false;
        flashburnPunisher.dispellable = false;
        kbimmuneBuff.dispellable = false;
        
        _statusManager.ObtainTimerBuff(flashburnPunisher);
        _statusManager.ObtainTimerBuff(defBuff,false);
        _statusManager.ObtainTimerBuff(kbimmuneBuff,false);
        _statusManager.ObtainTimerBuff(critBuff,false);
        _statusManager.ObtainTimerBuffs((int)BasicCalculation.BattleCondition.AlchemicCatridge,-1,3,3,-1);
        
        
    }

    protected void SwitchMainCamera()
    {
        StageCameraController.SwitchMainCamera();
    }

    protected void ForcefieldEnhancement()
    {
        var blackHole = FindObjectOfType<Projectile_C001_4_Boss>();
        _statusManager.ObtainTimerBuffs((int)BasicCalculation.BattleCondition.AlchemicCatridge,-1,3,3,-1);
        
        if(blackHole == null)
            return;

        blackHole.force = 5;
        blackHole.radius = 40f;
        
        blackHole.floatPunisher = 8f;
        blackHole.SetParticleSize(45f);
        blackHole.GetComponent<AttackFromEnemy>().AddWithConditionAll(
            new TimerBuff((int)BasicCalculation.BattleCondition.Blindness,-1,7.5f,1),
            100);
        blackHole.transform.Find("Around").gameObject.SetActive(true);

    }



    protected GameObject SummonZethia()
    {
        var zethia = Instantiate(Helper_Zethia, new Vector3(0,3,0), Quaternion.identity,
            transform.parent);
        
        UI_MultiBossManager.Instance.AddNewBoss(zethia,1);

        return zethia;
    }


}