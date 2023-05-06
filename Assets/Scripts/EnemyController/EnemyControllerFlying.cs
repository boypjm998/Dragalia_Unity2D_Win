using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime.Tasks.Unity.UnityInput;
using DG.Tweening;
using UnityEngine;

public class EnemyControllerFlying : EnemyController
{
    [SerializeField] protected float isMove = 0;
    public float moveSpeed = 10f;
    public float _defaultgravityscale = 1f;
    
    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        //rendererObject = transform.Find("Model").GetChild(0).Find("model/mBodyAll").gameObject;
        
        rigid = GetComponentInChildren<Rigidbody2D>();
        MoveManager = GetComponent<EnemyMoveManager>();
        _behavior = GetComponent<DragaliaEnemyBehavior>();
        
    }

    protected override void Start()
    {
        base.Start();
        if(canDeath)
            _statusManager.OnHPBelow0 += OnDeath;
    }


    public override void OnAttackEnter()
    {
        isMove = 0;
        isAction = true;
    }
    public override void OnAttackExit()
    {
        isAction = false;
        SetCounter(false);
        currentKBRes = _statusManager.knockbackRes;
    }

    public override void OnAttackEnter(int newKnockbackRes)
    {
        isMove = 0;
        isAction = true;
        currentKBRes = newKnockbackRes;

        if (newKnockbackRes < 200)
        {
            SetCounter(true);
        }
    }

    /// <summary>
    /// 匀速移动到目标
    /// </summary>
    /// <param name="target"></param>
    /// <param name="maxFollowTime"></param>
    /// <param name="arriveDistanceX"></param>
    /// <param name="arriveDistanceY"></param>
    /// <param name="startFollowDistance"></param>
    /// <returns></returns>
    public override IEnumerator MoveTowardTarget(GameObject target, float maxFollowTime, float arriveDistanceX, float arriveDistanceY,
        float startFollowDistance)
    {
        SetGravityScale(0);
        SetGroundCollision(false);
        TurnMove(target);
        
        
        if (CheckTargetDistance(target,arriveDistanceX,arriveDistanceY))
        {
            SetKBRes(999);
            OnMoveFinished?.Invoke(true);
            yield break;
        }
        
        //匀速移动到目标
        while (maxFollowTime > 0)
        {
            maxFollowTime -= Time.deltaTime;

            Vector2 direction = (target.transform.position - transform.position).normalized;
            transform.Translate(direction * moveSpeed * Time.fixedDeltaTime);
            
            if (CheckTargetDistance(target, arriveDistanceX, arriveDistanceY))
            {
                /*if (VerticalMoveRoutine != null)
                {
                    StopCoroutine(VerticalMoveRoutine);
                    VerticalMoveRoutine = null;
                }*/
                TurnMove(target);
                //isMove = 0;
                currentKBRes = 999;
                OnMoveFinished?.Invoke(true);
                yield break;
            }

            yield return new WaitForFixedUpdate();

        }
        
        //isMove = 0;
        SetKBRes(999);
        OnMoveFinished?.Invoke(false);
    }

    public virtual IEnumerator FlyTowardTarget(GameObject target, float timeToReach, Ease ease)
    {
        SetGravityScale(0);
        SetGroundCollision(false);
        TurnMove(target);
        Vector2 direction = (target.transform.position - transform.position).normalized;
        var endPoint = target.transform.position;
        if (endPoint.x > BattleStageManager.Instance.mapBorderR)
        {
            endPoint.x = BattleStageManager.Instance.mapBorderR;
        }else if(endPoint.x < BattleStageManager.Instance.mapBorderL)
        {
            endPoint.x = BattleStageManager.Instance.mapBorderL;
        }

        endPoint.y = target.transform.position.y + 2f;

        var tweenerCore = transform.DOMove(endPoint, timeToReach).SetEase(ease).OnComplete(() =>
        {
            SetGroundCollision(true);
            SetGravityScale(1);
            OnMoveFinished?.Invoke(true);
        });
        yield return new WaitUntil(() => !tweenerCore.IsPlaying());
    }

    public override void StartBreak()
    {
        var spStatus = _statusManager as SpecialStatusManager;
        if (_behavior.breakable && spStatus.broken == false)
        {
            spStatus.broken = true;
            anim.SetBool("break",true);
            anim.Play("broken");
            
        }
    }

    public override void OnBreakEnter()
    {
        OnHurtEnter();
        isAction = true;
        isMove = 0;
        BattleEffectManager.Instance.PlayBreakEffect();
        SetCounter(false);
        breakRoutine = StartCoroutine(BreakWait((_statusManager as SpecialStatusManager).breakTime));
    }
    
    public override void OnBreakExit()
    {
        OnHurtExit();
        SetCounter(false);
        transform.Find("Model/model/Break")?.gameObject.SetActive(false);
        var spStatus = _statusManager as SpecialStatusManager;
        if(spStatus.ODLock == false)
            spStatus.currentBreak = spStatus.baseBreak;
        else spStatus.currentBreak = 0.1f;
        
        spStatus.broken = false;
        isAction = false;
    }

    protected override void OnDeath()
    {
        base.OnDeath();
        StartCoroutine(DeathRoutine());
    }
    
    IEnumerator DeathRoutine()
    {
        SetKBRes(999);
        _effectManager.DisplayCounterIcon(gameObject,false);
        GetComponentInChildren<AudioSource>()?.Stop();
        transform.Find("MeeleAttackFX").gameObject.SetActive(false);
        transform.Find("HitSensor").gameObject.SetActive(false);
        anim.SetBool("defeat",true);
        MoveManager?.PlayVoice(0);//死亡
        anim.SetBool("hurt",false);
        OnAttackInterrupt?.Invoke();
        _behavior.enabled = false;
        _statusManager.enabled = false;
        _statusManager.StopAllCoroutines();
        
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
        //SetVelocity(rigid.velocity.x,0);
        //moveEnable = false;
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
        _statusManager.enabled = false;
        _statusManager.StopAllCoroutines();
        if (hurtEffectCoroutine != null)
        {
            StopCoroutine(hurtEffectCoroutine);
            flashBody.SetActive(false);
            hurtEffectCoroutine = null;
            
        }

        //yield return new WaitUntil(()=>!anim.GetCurrentAnimatorStateInfo(0).IsName("hurt"));
        yield return null;
        
        anim.Play("defeat");
        yield return null;
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.8f);
        anim.speed = 0;
        
        if(isSummonEnemy)
            Destroy(gameObject);
        
    }

    private void OnDestroy()
    {
        if(canDeath)
            _statusManager.OnHPBelow0 -= OnDeath;
    }
}
