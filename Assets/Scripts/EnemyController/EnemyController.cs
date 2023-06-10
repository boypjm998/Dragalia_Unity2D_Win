using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyController : ActorBase
{
    //Stats of Enemy
    [SerializeField] protected bool isSummonEnemy = false;
    public GameObject rendererObject;

    //public Animator anim;
    public Coroutine ActionTask = null;
    public bool isAction;
    protected EnemyMoveManager MoveManager;
    protected DragaliaEnemyBehavior _behavior;

    public delegate void OnTask(bool success);
    //public delegate void OnHurt();
    //委托
    public OnTask OnMoveFinished;
    //public OnHurt OnAttackInterrupt;
    //public OnTask OnAttackFinished;
    
    
    //public Rigidbody2D rigid;
    private int enemyid;
    private bool isBoss;
    public bool canDeath = true;
    //public int facedir = 1;
    public bool hurt;
    public bool grounded => anim.GetBool("isGround");
    
    public bool counterOn = false;
    protected float isMove = 0;

    //Hurt Effect
    [SerializeField]
    protected GameObject flashBody; //FlashTarget
    //protected SpriteRenderer spriteRenderer;
    //[SerializeField]private SpriteRenderer animRenderer;
    //protected Material originMaterial;
    protected GameObject counterUI;
    public GameObject minimapIcon;
    public MyShadowCaster shadowCaster;
    protected Coroutine hurtEffectCoroutine;
    protected Coroutine KnockbackRoutine;
    public Coroutine VerticalMoveRoutine;
    protected Coroutine breakRoutine;
    
    [SerializeField]
    protected float hurtEffectDuration = 0.1f;

    protected StatusManager _statusManager;
    protected BattleEffectManager _effectManager;
    
    public int currentKBRes;
    



    // Start is called before the first frame update
    protected virtual void Start()
    {
        hitSensor = transform.Find("HitSensor").gameObject;
        minimapIcon = transform.Find("MinimapIcon").gameObject;
        _statusManager = GetComponentInParent<StatusManager>();
        _effectManager = BattleEffectManager.Instance;
        currentKBRes = _statusManager.knockbackRes;
        rigid = GetComponent<Rigidbody2D>();
        shadowCaster = GetComponentInChildren<MyShadowCaster>();
        var attackfromplayers = FindObjectsOfType<AttackFromPlayer>();
        foreach (var attackfromplayer in attackfromplayers)
        {
            attackfromplayer.hitFlags.Add(gameObject.GetInstanceID());
        }

        if (_statusManager is SpecialStatusManager)
        {
            ((SpecialStatusManager)_statusManager).onBreak += StartBreak;
        }

    }
    protected override void Awake()
    {
        base.Awake();
        anim = GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //anim = GetComponent<Animator>();
        
    }
    
    protected virtual void CheckFaceDir()
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

    public override void TakeDamage(float kbpower, float kbtime,float kbForce, Vector2 kbDir) 
    {
        //anim = GetComponent<Animator>();
        //Debug.Log(anim.name);
        //anim.SetTrigger("hurt");
        Flash();
    }
    
    public override void TakeDamage(AttackBase atkBase, Vector2 kbdir)
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
        if (rand > kbpower-currentKBRes || currentKBRes - kbpower >= 100)
        {
            if(_statusManager is not SpecialStatusManager)
                return;
            
            if (counterOn && kbpower > currentKBRes)
            {
                if (atkBase.GetComponentInParent<AttackContainer>().IfODCounter == false)
                {
                    DamageNumberManager.GenerateCounterText(transform);
                    atkBase.GetComponentInParent<AttackContainer>().IfODCounter = true;
                }
            }

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
        if (counterOn)
        {
            //_effectManager.DisplayCounterIcon(gameObject,false);
             DamageNumberManager.GenerateCounterText(transform);
            
             _statusManager.ObtainUnstackableTimerBuff
             ((int)BasicCalculation.BattleCondition.Vulnerable,
                 10,10,9999);
             _statusManager.ObtainUnstackableTimerBuff
             ((int)BasicCalculation.BattleCondition.AtkDebuff,
                 30,7,9999);
            atkBase.GetComponentInParent<AttackContainer>().IfODCounter = true;
            //counterOn = false;
        }
    }
    
    public void EnemyActionStart(int actionID)
    {
        MoveManager.UseMove(actionID);
    }

    protected virtual IEnumerator HurtEffectCoroutine()
    {
        var time = hurtEffectDuration;
        while (time > 0)
        {
            time -= Time.deltaTime;
            flashBody.SetActive(true);
            //spriteRenderer.material = hurtEffectMaterial;
            //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b,100);
            yield return null;
        }
        flashBody.SetActive(false);
        //spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        hurtEffectCoroutine = null;
    }

    public void Flash()
    {
        if (hurtEffectCoroutine != null)
        {
            StopCoroutine(hurtEffectCoroutine);

        }
        hurtEffectCoroutine = StartCoroutine(HurtEffectCoroutine());
    }

    public virtual IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistance, float startFollowDistance)
    {
        throw new NotImplementedException();
    }
    
    public virtual IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistanceX, float arriveDistanceY, float startFollowDistance,bool continueThoughConditionOK=false)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 修正面朝目标
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
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

    protected float GetTargetDistanceX(GameObject target)
    {
        return target.transform.position.x - transform.position.x;
    }
    protected float GetTargetDistanceY(GameObject target)
    {
        return target.transform.position.y - transform.position.y;
    }
    
    protected float GetTargetGroundDistanceY(GameObject target)
    {
        var box = target.GetComponent<Collider2D>();
        
        
        return box.bounds.center.y - transform.position.y;
    }

    public virtual void OnAttackEnter()
    {
        return;
    }
    
    public virtual void OnAttackEnter(int newKnockbackRes)
    {
        return;
    }
    
    public virtual void OnAttackExit()
    {
        return;
    }

    public virtual void OnHurtEnter()
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
            

            if (currentKBRes >= 100)
            {
                if (_statusManager is SpecialStatusManager)
                {
                    if (!(_statusManager as SpecialStatusManager).broken)
                    {
                        _effectManager.DisplayCounterIcon(gameObject,false);
                        DamageNumberManager.GenerateCounterText(transform);
            
                        _statusManager.ObtainUnstackableTimerBuff
                        ((int)BasicCalculation.BattleCondition.Vulnerable,
                            10,10,9999);
                        _statusManager.ObtainUnstackableTimerBuff
                        ((int)BasicCalculation.BattleCondition.AtkDebuff,
                            30,7,9999);
                    }
                }
                else
                {
                    _effectManager.DisplayCounterIcon(gameObject,false);
                    DamageNumberManager.GenerateCounterText(transform);
            
                    _statusManager.ObtainUnstackableTimerBuff
                    ((int)BasicCalculation.BattleCondition.Vulnerable,
                        10,10,9999);
                    _statusManager.ObtainUnstackableTimerBuff
                    ((int)BasicCalculation.BattleCondition.AtkDebuff,
                        30,7,9999);
                    //print(_behavior.GetCurrentState()); 
                }
                
            }
            _behavior.StopCoroutine(_behavior.currentAttackAction);
            _behavior.currentAttackAction = null;
            currentKBRes = _statusManager.knockbackRes;
            SetCounter(false);
        }

        //rigid.gravityScale = 1;
        //SetVelocity(rigid.velocity.x,0);
        anim.speed = 1;
        MoveManager.SetGroundCollider(true);
        var meeles = transform.Find("MeeleAttackFX");
        for (int i = 0; i < meeles.childCount; i++)
        {

            //meeles.GetChild(i).GetComponent<AttackContainer>()?.DestroyInvoke();
            meeles.GetChild(i).GetComponent<EnemyAttackHintBar>()?.DestroySelf();
        }
        
        //transform.GetChild(0).GetComponentInChildren<AnimationEventSender_Enemy>()?.ChangeFaceExpression(0.75f);
        //MoveManager.AppearRenderer();
    }

    public virtual void OnHurtExit()
    {
        //rigid.gravityScale = _defaultgravityscale;
        anim.speed = 1;
        if (VerticalMoveRoutine != null)
        {
            StopCoroutine(VerticalMoveRoutine);
            VerticalMoveRoutine = null;
        }
        TurnMove(_behavior.targetPlayer);
    }

    public virtual void StartBreak()
    {
    }

    public virtual void OnBreakEnter()
    {
    }

    public virtual void OnBreakExit()
    {
    }

    protected virtual void OnDeath()
    {
        if (isSummonEnemy)
        {
            return;
        }

        //FindObjectOfType<BattleStageManager>().EnemyEliminated(gameObject);
        BattleStageManager.Instance.EnemyEliminated(gameObject);
    }
    
    public virtual void SwapWeaponVisibility(bool flag)
    {
        //weaponObject.transform.GetChild(0).gameObject.SetActive(flag);
    }
    
    public override void SetGravityScale(float scale)
    {
        rigid.gravityScale = scale;
    }

    public override void ResetGravityScale()
    {
        rigid.gravityScale = DefaultGravity;
    }

    protected void SetGroundCollision(bool on)
    {
        
    var pltformCol = transform.Find("Platform Sensor").GetComponent<Collider2D>();
    if (pltformCol != null)
    {
        pltformCol.enabled = on;
    }
    transform.Find("GroundSensor").GetComponent<Collider2D>().enabled = on;
        
        
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
    
    public void SetKBRes(int value)
    {
        currentKBRes = value;
    }

    protected virtual IEnumerator BreakWait(float time,float recoverTime = 1.67f)
    {
        yield return new WaitForSeconds(time);
        breakRoutine = null;
        anim.SetBool("break",false);
    }

    public void SetCounter(bool flag)
    {
        counterOn = flag;
        BattleEffectManager.Instance.DisplayCounterIcon(gameObject,flag);
    }

    public void SetMove(float value)
    {
        isMove = value;
    }

}
