using System.Collections;
using System.Collections.Generic;
using CharacterSpecificProjectiles;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyController_HB03_Legend : EnemyControllerHumanoidHigh
{
    public float WandingTolerance => _moveArgument3;
    public bool IsIdle => _actionType == ExtraMoveActionType.Idle;
    
    [SerializeField]private ExtraMoveActionType _actionType;
    /// <summary>
    /// tolerance
    /// </summary>
    [SerializeField] private float _moveArgument3;
    [SerializeField] private float _heightConstraint;
    [SerializeField] private GameObject _floatEffectInstance;

    private Tweener _extraMoveTween = null;
    private Vector2 _lastPlayerPosition;
    private Vector2 _direction = Vector2.zero;
    private bool _tweenFinished = true;

    private Vector2 _currentDirection;

    private float _baseBreak;
    private enum ExtraMoveActionType
    {
        Idle,
        Keep
    }

    protected override void Start()
    {
        base.Start();
        //_lastPlayerPosition = _behavior.targetPlayer.transform.position;
        _baseBreak = (_statusManager as SpecialStatusManager).baseBreak;
    }

    protected override void Move()
    {
        if(!moveEnable)
            return;
        
        if(hurt)
            return;
        
        float bogModifier = isBog ? 0.5f : 1;
        
        DoMove();
        
        CheckFaceDir();
    }

    public void SetFloatEffectActive(bool flag)
    {
        _floatEffectInstance.SetActive(flag);
    }
    public void StopExtraMove()
    {
        _actionType = ExtraMoveActionType.Idle;
        _extraMoveTween?.Kill();
    }

    public void StartExtraMove()
    {
        _actionType = ExtraMoveActionType.Keep;
    }

    public void SetMovementArgument(float arg)
    {
        _moveArgument3 = arg;
    }

    private void DoMove()
    {
        if (_actionType == ExtraMoveActionType.Idle)
        {
            _tweenFinished = true;
            return;
        }

        if (_tweenFinished)
        {
            Vector2 randomDirection = Random.insideUnitCircle.normalized;

            // randomDirection = 
            //     (randomDirection + PredictPlayerPosition(_behavior.targetPlayer.transform)).normalized;

            var targetPos = (Vector2)_behavior.targetPlayer.transform.position +
                            randomDirection * _moveArgument3;

            var distance = Vector2.Distance(_behavior.targetPlayer.transform.position,
                transform.position);

            //bool sameDirection = false;

            // if (_behavior.targetPlayer.transform.position.x < transform.position.x &&
            //     _behavior.targetPlayer.transform.position.x >= targetPos.x)
            // {
            //     targetPos.x *= -1;
            // }
            // else if
            //     (_behavior.targetPlayer.transform.position.x > transform.position.x &&
            //      _behavior.targetPlayer.transform.position.x <= targetPos.x)
            // {
            //     targetPos.x *= -1;
            // }
            
            
            
            if (targetPos.y <= BattleStageManager.Instance.mapBorderT - _heightConstraint)
            {
                targetPos.y = Mathf.Max(
                    targetPos.y,
                    _behavior.targetPlayer.transform.position.y + 3);
            }else if (targetPos.y < BattleStageManager.Instance.mapBorderB)
            {
                targetPos.y = BattleStageManager.Instance.mapBorderB + 2;
            }
            if (targetPos.x <= BattleStageManager.Instance.mapBorderL ||
                targetPos.x >= BattleStageManager.Instance.mapBorderR)
            {
                randomDirection.x *= -1;
                targetPos = (Vector2)_behavior.targetPlayer.transform.position +
                            randomDirection * _moveArgument3;
            }
            

            var moveDirection = (targetPos - (Vector2)transform.position);
            

            float tweenTime = 1;

            var standardMoveSpeed = Mathf.Clamp((distance / tweenTime) / movespeed,1,3);

            tweenTime *= standardMoveSpeed;

            float moveDistance = movespeed * tweenTime;
            
            //AnimationCurve curve = AnimationCurve.EaseInOut(0, 0, 1, 1);
            targetPos = (Vector2)transform.position + moveDirection.normalized * moveDistance;
            

            _tweenFinished = false;

            //_extraMoveTween?.Kill();
            TurnMove(_behavior.targetPlayer);
            
            
            

            // _extraMoveTween = rigid.DOPath(path, tweenTime + 0.02f,
            //         PathType.CatmullRom,PathMode.Sidescroller2D).OnComplete(() => _tweenFinished = true)
            //     .OnKill(() => _tweenFinished = true).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);

            _extraMoveTween = rigid.DOMove(targetPos, tweenTime + 0.02f).OnComplete(() => _tweenFinished = true)
                .OnKill(() => _tweenFinished = true).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed);

            _currentDirection = moveDirection;

        }


    }

    public override void SetDefaultGravityScale(float value, bool reset = true)
    {
        _defaultgravityscale = value;
        SetGravityScale(value);
    }

    protected override IEnumerator DeathRoutine()
    {
        SetKBRes(999);
        SetDefaultGravityScale(4);
        _extraMoveTween?.Kill();
        
        var environmentRenderers = 
            BattleEnvironmentManager.Instance.GetAllEnvironmentRenderer();
        var background2 = 
            BattleEnvironmentManager.Instance.
                GetEnvironmentSpriteRenderer("Background2") as SpriteRenderer;
        background2.DOColor(Color.clear, 1);
        foreach (var renderer in environmentRenderers)
        {
            if (renderer.name == "Background2")
            {
                
            }else if (renderer.name == "Background1")
            {

            }else if (renderer.name.StartsWith("Eff"))
            {
                renderer.gameObject.SetActive(false);
            }
            else if(renderer.name.StartsWith("Sprite"))
            {
                renderer.gameObject.SetActive(true);
            }
        }
        
        
        _effectManager.DisplayCounterIcon(gameObject,false);
        
        GetComponentInChildren<AudioSource>()?.Stop();
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
        _floatEffectInstance.SetActive(false);
        SetGravityScale(4);
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
            if (weaponObject)
            {
                var flashWeapon = weaponObject.transform.Find("Flash").gameObject;
                flashWeapon.SetActive(false);
            }
            hurtEffectCoroutine = null;
        }

        //yield return new WaitUntil(()=>!anim.GetCurrentAnimatorStateInfo(0).IsName("hurt"));
        yield return null;
        moveEnable = false;
        isMove = 0;
        //anim.Play("defeat");
        yield return null;
        anim.SetFloat("forward",0);
        
        yield return new WaitUntil(()=>!anim.GetCurrentAnimatorStateInfo(0).IsName("defeat_5"));
        
        yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
        //anim.speed = 0;

        

        
        
    }

    public override void OnBreakEnter()
    {
        base.OnBreakEnter();
        SetGravityScale(4);
    }

    public override void OnHurtExit()
    {
        base.OnHurtExit();
        SetVelocity(0,0);
    }

    public override void OnHurtEnter()
    {
        base.OnHurtEnter();
        SetGravityScale(0);
        _tweener?.Kill();
        _extraMoveTween?.Kill();
    }

    public override void StartBreak()
    {
        base.StartBreak();
        
        StopExtraMove();
        
        SetGroundCollision(true);
        
        

        if (Projectile_C001_6_Boss.Instance != null)
        {
            Destroy(Projectile_C001_6_Boss.Instance.gameObject);
        }
        
    }

    protected override IEnumerator BreakWait(float time, float recoverTime = 1.67f)
    {
        SetKBRes(999);
        
        
        var spStat = _statusManager as SpecialStatusManager;
        if (spStat.baseBreak > _baseBreak * 0.5f)
        {
            spStat.baseBreak -= _baseBreak * 0.1f;
        }

        if (spStat.breakDefRate > 0.3f)
        {
            spStat.breakDefRate -= 0.1f;
        }
        
        anim.GetComponent<AnimationEventSender_Enemy>()?.ChangeFaceExpression(0.75f);
        
        _floatEffectInstance.SetActive(false);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("break_loop"));

        yield return new WaitForSeconds((_statusManager as SpecialStatusManager).breakTime - 4.67f);
        
        
        
        anim.Play("break_exit");
        anim.GetComponent<AnimationEventSender_Enemy>()?.ChangeFaceExpression(0);
        yield return new WaitForSeconds(1.67f);
        _floatEffectInstance.SetActive(true);
        yield return new WaitForSeconds(3f);
        ResetGravityScale();
        //ResetKBRes();
        //SetGroundCollision(false);
        
        breakRoutine = null;
        anim.SetBool("break",false);
        
    }

    Vector2 PredictPlayerPosition(Transform targetPlayer)
    {
        // 预测玩家位置
        Vector2 playerVelocity = (Vector2)targetPlayer.position - _lastPlayerPosition;
        return (Vector2)targetPlayer.position + playerVelocity * 0.5f; // 假设玩家会继续当前的移动方向
    }

    public IEnumerator FlyToRelativePoint(Vector2 offset, float rng, float maxTime, Ease ease)
    {
        SetGravityScale(0);
        SetGroundCollision(false);
        
        

        offset += new Vector2(Random.Range(-rng, rng), Random.Range(-rng, rng));
        
        if (transform.position.x < _behavior.targetPlayer.transform.position.x + offset.x)
        {
            SetFaceDir(1);
        }else if(transform.position.x > _behavior.targetPlayer.transform.position.x + offset.x)
            SetFaceDir(-1);

        if (transform.position.x + offset.x > BattleStageManager.Instance.mapBorderR ||
            transform.position.x + offset.x < BattleStageManager.Instance.mapBorderL)
        {
            offset.x *= -1;
        }

        Vector2 position = (Vector2)_behavior.targetPlayer.transform.position + offset;

        if (transform.position.x > _behavior.targetPlayer.transform.position.x - offset.x &&
            transform.position.x < _behavior.targetPlayer.transform.position.x + offset.x)
        {
            position.x = transform.position.x;
        }else if (transform.position.x <= _behavior.targetPlayer.transform.position.x - offset.x)
        {
            position.x = _behavior.targetPlayer.transform.position.x - offset.x;
        }else if (transform.position.x >= _behavior.targetPlayer.transform.position.x + offset.x)
        {
            position.x = _behavior.targetPlayer.transform.position.x + offset.x;
        }
        
        
        position = BattleStageManager.Instance.OutOfRangeCheck(position);

        float timeToReach = Vector2.Distance(position, transform.position) / movespeed;

        timeToReach = Mathf.Min(timeToReach, maxTime);
            
        //anim.SetFloat("forward", 1);
        //anim.Play("fly");

        _tweener = transform.DOMove(position, timeToReach).SetEase(ease).OnComplete(() =>
        {
            SetGroundCollision(true);
            OnMoveFinished?.Invoke(true);
            //anim.SetFloat("forward", 0);
            //anim.Play("idle");
        }).OnKill(() =>
        {
            SetGroundCollision(true);
            OnMoveFinished?.Invoke(false);
            ResetGravityScale();
        }).SetUpdate(UpdateType.Fixed);

        yield return new WaitForSeconds(timeToReach);
        
        QuitMove(true);


    }
    
    
    public IEnumerator FlyToPointZone(float arriveXL, float arriveXR, float absoluteY,
        float speedModifier, Ease ease)
    {
        SetGravityScale(0);
        SetGroundCollision(false);


        Vector2 position = new(transform.position.x, absoluteY);
        
        if (transform.position.x < arriveXL)
        {
            SetFaceDir(1);
            position.x = arriveXL;
        }
        else if (transform.position.x > arriveXR)
        {
            SetFaceDir(-1);
            position.x = arriveXR;
        }
            

        // if (transform.position.x + offset.x > BattleStageManager.Instance.mapBorderR ||
        //     transform.position.x + offset.x < BattleStageManager.Instance.mapBorderL)
        // {
        //     offset.x *= -1;
        // }
        
        
        
        position = BattleStageManager.Instance.OutOfRangeCheck(position);

        float timeToReach = Vector2.Distance(position, transform.position) / (movespeed *  speedModifier);

        //timeToReach = Mathf.Min(timeToReach, maxTime);
        

        _tweener = transform.DOMove(position, timeToReach).SetEase(ease).OnComplete(() =>
        {
            SetGroundCollision(true);
            OnMoveFinished?.Invoke(true);
            //anim.SetFloat("forward", 0);
            //anim.Play("idle");
        }).OnKill(() =>
        {
            SetGroundCollision(true);
            OnMoveFinished?.Invoke(false);
            ResetGravityScale();
        }).SetUpdate(UpdateType.Fixed);

        yield return new WaitForSeconds(timeToReach);
        
        QuitMove(true);


    }

    public override void DisappearRenderer()
    {
        base.DisappearRenderer();
        SetFloatEffectActive(false);
    }

    public override void AppearRenderer()
    {
        base.AppearRenderer();
        SetFloatEffectActive(true);
    }
}
