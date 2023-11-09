using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class EnemyControllerTherian : EnemyController
{
    protected override void Awake()
    {
        base.Awake();
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
    
    
    
    
    
    
    
    public override void StartBreak()
    {
        var spStatus = _statusManager as SpecialStatusManager;
        if (_behavior.breakable && spStatus.broken == false)
        {
            _behavior.isAction = true;
            if (_behavior.currentAction != null)
            {
                StopCoroutine(_behavior.currentAction);
                _behavior.currentAction = null;
            }
            spStatus.broken = true;
            anim.SetBool("break",true);
            anim.Play("break_enter");
        }
    }

    protected override IEnumerator BreakWait(float time, float recoverTime = 1.67f)
    {
        SetKBRes(999);
        yield return new WaitForSeconds(time - recoverTime);
        anim.Play("break_exit");
        yield return new WaitForSeconds(recoverTime);
    }

    public override void OnBreakEnter()
    {
        SetKBRes(999);
        OnHurtEnter();
        isAction = true;
        _behavior.isAction = true;
        
        isMove = 0;
        BattleEffectManager.Instance.PlayBreakEffect();
        SetCounter(false);
        print((_statusManager as SpecialStatusManager).breakTime);
        UI_BossODBar.Instance?.ODBarClear();
        breakRoutine = StartCoroutine(BreakWait((_statusManager as SpecialStatusManager).breakTime));
    }
    
    protected override void OnDeath()
    {
        base.OnDeath();
        if(_statusManager.currentHp > 0)
            return;
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
        ResetGravityScale();
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
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
        anim.speed = 0;
        
        if (disappearTimeAfterDeath > 0)
        {
            yield return new WaitForSeconds(disappearTimeAfterDeath);
            Destroy(gameObject);
        }
        
    }
    
    public override void SetActionUnable(bool flag)
    {
        if (flag)
        {
            anim.SetBool("hurt",true);
            
            hurt = true;
            //moveEnable = false;
            //_behavior.isAction = true;
            isMove = 0;
        }
        else
        {
            anim.SetBool("hurt",false);
            hurt = false;
            //moveEnable = true;
            //_behavior.isAction = false;
            // if(breakRoutine == null)
            //anim.Play("idle");
        }
        
    }
}
