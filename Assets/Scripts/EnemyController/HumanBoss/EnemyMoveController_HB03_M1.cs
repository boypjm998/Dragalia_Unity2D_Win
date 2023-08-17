using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;
using UnityEditor;

public class EnemyMoveController_HB03_M1 : EnemyMoveManager
{
    protected enum VoiceGroup
    {
        ComboA = 0,
        ComboB = 1,
        ComboC = 2,
        RollAttack = 3,
        ForceStrike = 4,
        GenesisCirclet = 5,
        Intro = 6
    }
    public bool dodgeIsReady = false;
    VoiceControllerEnemy voice;

    
    
    
    

    protected override void Awake()
    {
        base.Awake();
        voice = GetComponentInChildren<VoiceControllerEnemy>();
        ac = GetComponent<EnemyControllerHumanoid>();
        ac.OnDodgeSuccess += RollAttackCounter;
    }
    
    public IEnumerator HB03M1_Action01(bool isBoosted = false)
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        
        anim.Play("force_enter");
        if(isBoosted)
            ForceAttackCharging();
        
        var hint = GenerateWarningPrefab(
            isBoosted?WarningPrefabs[0]:WarningPrefabs[1], transform.position,
            ac.facedir==1?Quaternion.Euler(0,0,0):Quaternion.Euler(0,180,0),
            RangedAttackFXLayer.transform);
        
        yield return new WaitForSeconds(isBoosted?2.5f:1.05f);
        Destroy(hint);
        anim.Play("force_exit");
        voice?.PlayMyVoice((int)VoiceGroup.ForceStrike);

        yield return new WaitForSeconds(0.1f);
        ac.SetKBRes(999);
        ac.SetCounter(false);
        ForceAttack(isBoosted);
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();

    }

    public IEnumerator HB03M1_Action02_Test()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        bool isOnSamePlatformCheck;

        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        BattleEffectManager.Instance.SpawnExclamation(gameObject,transform.position+new Vector3(0,2,0));

        yield return new WaitForSeconds(1f);
        
        anim.Play("combo1");
        yield return null;

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f);
        Combo1();
        voice?.PlayMyVoice((int)VoiceGroup.ComboA);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);



        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }


        anim.Play("combo2");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.23f);
        Combo2();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        anim.Play("combo3");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        Combo3();
        voice?.PlayMyVoice((int)VoiceGroup.ComboB);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.33f);
        Combo3_Dash();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        
        
        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }
        anim.Play("combo4");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.165f);
        Combo4();
        
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        
        
        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }
        anim.Play("combo5");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.05f);
        Combo5();
        voice?.PlayMyVoice((int)VoiceGroup.ComboB);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        
        
        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }
        anim.Play("combo6");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.12f);
        Combo6();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        
        
        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }
        anim.Play("combo7");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.23f);
        Combo7();
        voice?.PlayMyVoice((int)VoiceGroup.ComboC);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.5f);
        
        
        
        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }
        anim.Play("combo8");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.167f);
        Combo8();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.51f);
        
        
        
        if (CheckIfUseRollAttack(5, out isOnSamePlatformCheck))
        {
            anim.Play("roll");
            ac.TurnMove(_behavior.targetPlayer);
            yield return new WaitForSeconds(0.05f);
            
            (ac as EnemyControllerHumanoid).dodging = true;
            RollMove(isOnSamePlatformCheck);
            
            yield return new WaitUntil(()=>anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);
            RollAttack();

            yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);
            
            (ac as EnemyControllerHumanoid).dodging = false;
            anim.Play("idle");
            yield return new WaitUntil(() => ac.hurt == false && ac.grounded);
            
        }
        anim.Play("combo9");
        ac.TurnMove(_behavior.targetPlayer);
        yield return null;
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.05f);
        Combo9A();
        voice?.PlayMyVoice((int)VoiceGroup.ComboB);
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.67f);
        Combo9B();
        
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
        
        anim.Play("idle");
        QuitAttack();


    }

    public IEnumerator HB03M1_Action04()
    {
        yield return new WaitUntil(() => !ac.hurt && anim.GetBool("isGround"));
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        
        bossBanner?.PrintSkillName("HB03M1_Action04");
        

        var hint = GenerateWarningPrefab(
            WarningPrefabs[2], transform.position,
            ac.facedir==1?Quaternion.Euler(0,0,0):Quaternion.Euler(0,180,0),
            RangedAttackFXLayer.transform);
        var hintbar = hint.GetComponentInChildren<EnemyAttackHintBarShine>();

        yield return new WaitForSeconds(0.5f);
        GenesisCircletCharging();
        
        yield return new WaitForSeconds(hintbar.warningTime-0.8f);
        
        anim.Play("s1");
        GenesisCirclet();

        yield return new WaitForSeconds(0.3f);
        voice?.PlayMyVoice((int)VoiceGroup.GenesisCirclet);
        Destroy(hint);
        
        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f);
        
        anim.Play("idle");
        QuitAttack();
    }






    protected bool CheckIfUseRollAttack(float distanceX,out bool onSamePlatform)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2.5f,
            LayerMask.GetMask("AttackPlayer"));
        foreach (Collider2D collider in colliders)
        {
            if (collider.isTrigger)
            {
                onSamePlatform = false;
                return true;
            }

        }
        
        if (BasicCalculation.CheckRaycastedPlatform(_behavior.targetPlayer) ==
            BasicCalculation.CheckRaycastedPlatform(gameObject))
        {
            onSamePlatform = true;
            if(Mathf.Abs(transform.position.x - _behavior.targetPlayer.transform.position.x)
               > distanceX)
            {
                return true;
            }

            return false;
        }
        else
        {
            onSamePlatform = false;
            return true;
        }
    }




    protected void Combo1()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile1,container.transform.position,
            container);
    }

    protected void Combo2()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile2,container.transform.position,
            container);
    }
    
    protected void Combo3()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(projectile3,container.transform.position,
            container,ac.facedir);
    }
    
    protected void Combo3_Dash()
    {
        var targetpos = transform.position + new Vector3(8 * ac.facedir, 0, 0);
        
        //向前发射一条射线，如果碰到Characters的layer，则返回其碰撞点
        var hit = Physics2D.Raycast(transform.position,
            new Vector2(ac.facedir, 0), 8,
            LayerMask.GetMask("Characters"));
        
        if (hit.collider != null)
        {
            if(Mathf.Abs(targetpos.x - transform.position.x) > 2)
                targetpos = hit.point - 2*new Vector2(ac.facedir,0);
            
        }
        
        
        
        targetpos = BattleStageManager.Instance.OutOfRangeCheck(targetpos);
        
        
        StartCoroutine((ac as EnemyControllerHumanoid).HorizontalMoveFixedTime(
            targetpos.x,
            0.3f,"combo3"));
    }
    
    protected void Combo4()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile4,container.transform.position,
            container);
    }
    
    protected void Combo5()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile5,container.transform.position,
            container);
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            11,30,3,100603);
    }
    
    protected void Combo6()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile6,container.transform.position,
            container);
    }
    
    protected void Combo7()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile7,container.transform.position,
            container);
    }
    
    protected void Combo8()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile8,container.transform.position,
            container);
    }
    
    protected void Combo9A()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectile9,container.transform.position,
            container);
        
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.CritDmgBuff,
            11,30,3,100603);
    }
    
    protected void Combo9B()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        var proj = InstantiateRanged(projectile10,
            _behavior.targetPlayer.transform.position,
            container,1);
    }

    protected void RollMove(bool isOnSamePlatform)
    {
        dodgeIsReady = true;
        
        voice?.PlayMyVoice((int)VoiceGroup.RollAttack);
        if(isOnSamePlatform)
            (ac as EnemyControllerHumanoid).InertiaMove(0.6f,ac.facedir);
        else
        {
            var ground = BasicCalculation.CheckRaycastedPlatform(gameObject);
            var rollDistance = 0.6f * (ac as EnemyControllerHumanoid).movespeed;
            if (ac.facedir == 1)
            {
                if (transform.position.x + rollDistance <= ground.bounds.max.x)
                {
                    (ac as EnemyControllerHumanoid).InertiaMove(0.6f,ac.facedir);
                }
                else
                {
                    var maxMoveDistance = Mathf.Min(ground.bounds.max.x - transform.position.x,rollDistance);
                    (ac as EnemyControllerHumanoid).
                        InertiaMove(0.6f*(maxMoveDistance/rollDistance),ac.facedir);
                }
            }
            else
            {
                if (transform.position.x - rollDistance <= ground.bounds.min.x)
                {
                    (ac as EnemyControllerHumanoid).InertiaMove(0.6f,ac.facedir);
                }
                else
                {
                    var maxMoveDistance = Mathf.Min(-ground.bounds.min.x + transform.position.x,rollDistance);
                    (ac as EnemyControllerHumanoid).
                        InertiaMove(0.6f*(maxMoveDistance/rollDistance),ac.facedir);
                }
            }

        }
    }

    protected void RollAttack()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectilePoolEX[0],container.transform.position
            +new Vector3(0,0.5f,0),
            container);
    }

    protected void RollAttackCounter(AttackBase atkBase, GameObject targetPlayer)
    {
        
        if(!dodgeIsReady)
            return;
        
        dodgeIsReady = false;
        
        
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,RangedAttackFXLayer.transform);
        
        var targetPosition = transform.position + new Vector3(ac.facedir*3,0);
        var targetPosX = targetPosition.x;
        if (ac.facedir == 1)
        {
            targetPosX = Mathf.Max(targetPosition.x,targetPlayer.transform.position.x - 2);
        }
        else
        {
            targetPosX = Mathf.Min(targetPosition.x,targetPlayer.transform.position.x + 2);
        }



        var proj = InstantiateRanged(projectilePoolEX[1],
            new Vector3(targetPosX,transform.position.y),
            container,ac.facedir);
        
        
        
    }

    protected void ForceAttack(bool boosted)
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(boosted?projectilePoolEX[4]:projectilePoolEX[5],container.transform.position,
            container);
        var fx1 = Instantiate(projectilePoolEX[3],container.transform.position,
            Quaternion.identity,MeeleAttackFXLayer.transform);

        var goal = BattleStageManager.Instance.
            OutOfRangeCheck(new Vector2(transform.position.x + ac.facedir * 6,
            transform.position.y));
        
        
        
        
        _tweener = transform.DOMoveX(goal.x,0.25f).SetEase(Ease.OutSine);
    }

    protected void ForceAttackCharging()
    {
        Instantiate(projectilePoolEX[2],transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
    }

    protected void GenesisCircletCharging()
    {
        Instantiate(projectilePoolEX[6],transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
    }
    
    protected void GenesisCirclet()
    {
        var container = Instantiate(BattleStageManager.Instance.attackContainerEnemy,
            transform.position,Quaternion.identity,MeeleAttackFXLayer.transform);
        
        var proj = InstantiateMeele(projectilePoolEX[7],container.transform.position,
            container);
    }

    public override void PlayVoice(int id)
    {
        if (id == 1)
        {
            voice?.BroadCastMyVoice((int)VoiceGroup.Intro);
        }
    }
}
