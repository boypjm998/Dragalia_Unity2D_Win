using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;


public class EnemyControllerFlyingHigh : EnemyControllerFlying
{
      protected StandardGroundSensor groundSensable;
      public bool flyingGrounded => groundSensable.GetCurrentAttachedGroundInfo() != null;
      
      protected override void Awake()
      {
            base.Awake();
            groundSensable = GetComponentInChildren<StandardGroundSensor>();
      }


      public override IEnumerator FlyTowardTarget(GameObject target, float timeToReach, Ease ease)
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

            endPoint.y = target.transform.position.y + GetActorHeight();

            var tweenerCore = transform.DOMove(endPoint, timeToReach).SetEase(ease).OnComplete(() =>
            {
                  SetGroundCollision(true);
                  ResetGravityScale();
                  OnMoveFinished?.Invoke(true);
            });
            yield return new WaitUntil(() => !tweenerCore.IsPlaying());
      }

      public IEnumerator FlyTowardTargetOnSamePlatform(GameObject target, float arriveDistanceX, float allowDistanceY, float maxFollowTime)
      {
            SetGravityScale(0);
            
            var targetCollider = BasicCalculation.CheckRaycastedPlatform(target);
            var targetHeight = target.GetComponent<ActorBase>().GetActorHeight();
            print(GetActorHeight());
            //var myCollider = BasicCalculation.CheckRaycastedPlatform(gameObject);
            Vector2 endPoint;

            float time = 0;
            
            
            
            if (Mathf.Abs(target.transform.position.x - transform.position.x) < arriveDistanceX &&
                Mathf.Abs(target.transform.position.y - targetHeight - transform.position.y + GetActorHeight()) < -allowDistanceY)
            { 
                  OnMoveFinished?.Invoke(true);
                  SetGroundCollision(true);
                  anim.Play("idle");
                  ResetGravityScale();
                  yield break;
            }
            
            anim.Play("fly");

            while (time < maxFollowTime)
            {
                  
                  targetCollider = BasicCalculation.CheckRaycastedPlatform(target);
                  
                  if (((transform.position.y - GetActorHeight()) -
                       target.transform.position.y - targetHeight) > allowDistanceY)
                  {
                        endPoint.y = targetCollider.bounds.max.y + GetActorHeight() + 0.5f;
                        //endPoint.y = transform.position.y + 0.5f;
                  }
                  else if (((target.transform.position.y) - targetHeight) - (transform.position.y - GetActorHeight()) >
                             allowDistanceY)
                   {
                         endPoint.y = targetCollider.bounds.max.y + GetActorHeight() + 0.5f;
                   }
                  else
                  {
                        //endPoint.y = targetCollider.bounds.max.y + GetActorHeight() + 0.5f;
                        endPoint.y = transform.position.y + 0.5f;
                        print("set y with target collider");
                  }


                  if (Mathf.Abs(target.transform.position.x - transform.position.x) < arriveDistanceX)
                  {
                        if (target.transform.position.x > transform.position.x)
                        {
                              print("目标在右边，且x轴到达范围");
                              endPoint.x = Mathf.Max(transform.position.x, targetCollider.bounds.min.x);
                        }
                        else
                        {
                              endPoint.x = Mathf.Min(transform.position.x, targetCollider.bounds.max.x);
                              print("目标在左边，且x轴到达范围");
                        }
                  }
                  else if(target.transform.position.x > transform.position.x)
                  {
                        //目标在右边
                        endPoint.x = Mathf.Max(target.transform.position.x - arriveDistanceX,targetCollider.bounds.min.x) + 0.5f;
                  }else
                  {
                        //目标在左边
                        endPoint.x = Mathf.Min(target.transform.position.x + arriveDistanceX,targetCollider.bounds.max.x) - 0.5f;
                  }
                  
                  //以moveSpeed的速度向endPoint移动
                  Vector2 direction = (endPoint - (Vector2)transform.position).normalized;
                  transform.Translate(direction * moveSpeed * Time.deltaTime);
                  
                  //如果距离目标点的距离小于0.5f,break.
                  
                  
                  TurnMove(target);
                  
                  if (Vector2.Distance(transform.position, endPoint) < 0.5f)
                  { 
                        OnMoveFinished?.Invoke(true);
                        SetGroundCollision(true);
                        anim.Play("idle");
                        ResetGravityScale();
                        yield break;
                  }
                  
                  yield return null;
                  
                  time += Time.deltaTime;
                  print(endPoint);
            }

            SetGroundCollision(true);
            anim.Play("idle");
            ResetGravityScale();
            OnMoveFinished?.Invoke(false);
            
            yield break;

      }

      public IEnumerator FlyToPoint(Vector2 position, float timeToReach, Ease ease)
      {
            SetGravityScale(0);
            SetGroundCollision(false);
            if (transform.position.x < position.x)
            {
                  SetFaceDir(1);
            }else if(transform.position.x > position.x)
                  SetFaceDir(-1);

            position = BattleStageManager.Instance.OutOfRangeCheck(position);
            anim.Play("fly");

            var tweenerCore = transform.DOMove(position, timeToReach).SetEase(ease).OnComplete(() =>
            {
                  SetGroundCollision(true);
                  OnMoveFinished?.Invoke(true);
                  anim.Play("idle");
            }).OnKill(() =>
            {
                  SetGroundCollision(true);
                  OnMoveFinished?.Invoke(false);
                  ResetGravityScale();
            });
            
            yield return new WaitUntil(() => !tweenerCore.IsPlaying());

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
            ResetGravityScale();
        
            isMove = 0;
            BattleEffectManager.Instance.PlayBreakEffect();
            StageCameraController.SwitchMainCamera();
            SetCounter(false);
            print((_statusManager as SpecialStatusManager).breakTime);
            UI_BossODBar.Instance?.ODBarClear();
            breakRoutine = StartCoroutine(BreakWait((_statusManager as SpecialStatusManager).breakTime));
      }

      public override void ResetGravityScale()
      { 
            rigid.gravityScale = _defaultgravityscale;
      }
}
