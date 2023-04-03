using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : ActorBase
{
    //Stats of Enemy
    [SerializeField] protected bool isSummonEnemy = false;
    [HideInInspector] public GameObject rendererObject;

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
    //public int facedir = 1;
    public bool hurt;
    

    //Hurt Effect
    [SerializeField]
    protected GameObject flashBody; //FlashTarget
    //protected SpriteRenderer spriteRenderer;
    //[SerializeField]private SpriteRenderer animRenderer;
    //protected Material originMaterial;
    protected GameObject counterUI;
    protected Coroutine hurtEffectCoroutine;
    protected Coroutine KnockbackRoutine;
    public Coroutine VerticalMoveRoutine;
    
    [SerializeField]
    protected float hurtEffectDuration = 0.1f;

    protected StatusManager _statusManager;
    protected BattleEffectManager _effectManager;
    
    public int currentKBRes;
    



    // Start is called before the first frame update
    protected virtual void Start()
    {
        //spriteRenderer = TargetObject.GetComponent<SpriteRenderer>();
        _statusManager = GetComponentInParent<StatusManager>();
        //animRenderer = GetComponentInParent<SpriteRenderer>();
        _effectManager = BattleEffectManager.Instance;
        //counterUI = transform.Find("BuffLayer").Find("CounterUI").gameObject;
        currentKBRes = _statusManager.knockbackRes;
    }
    protected virtual void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //anim = GetComponent<Animator>();
        
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

    public override void TakeDamage(float kbpower, float kbtime,float kbForce, Vector2 kbDir) 
    {
        //anim = GetComponent<Animator>();
        //Debug.Log(anim.name);
        //anim.SetTrigger("hurt");
        Flash();
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
    
    public virtual IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistanceX, float arriveDistanceY, float startFollowDistance)
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
    }

    public virtual void OnHurtExit()
    {
        
    }

    protected virtual void OnDeath()
    {
        if (isSummonEnemy)
        {
            return;
        }

        FindObjectOfType<BattleStageManager>().EnemyEliminated(gameObject);
    }
    
    public virtual void SwapWeaponVisibility(bool flag)
    {
        //weaponObject.transform.GetChild(0).gameObject.SetActive(flag);
    }
    
    protected void SetGravityScale(float scale)
    {
        rigid.gravityScale = scale;
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



}
