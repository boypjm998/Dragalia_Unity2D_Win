using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using GameMechanics;
using UnityEngine;

public class EnemyMoveController_DB15V : EnemyMoveManager
{
    public IEnumerator DB15V_Action01()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15V_Action01");
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");

        Instantiate(GetProjectileOfFormatName("action01"), transform.position, Quaternion.identity,
            RangedAttackFXLayer.transform);
        _statusManager.ObtainTimerBuff((int)BasicCalculation.BattleCondition.DefBuff,
            5, 60);
        _statusManager.ObtainHealOverTimeBuff(1, 15, true);
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
     
    }
    
    /// <summary>
    /// Meele combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15V_Action02()
    {

        yield return _canAction;
        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        BattleEffectManager.Instance.SpawnExclamation(gameObject, transform.position + Vector3.up * 5, true);

        yield return new WaitForSeconds(1.5f);

        anim.Play("combo1");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);

        MeeleComboAttack(1);
        _voiceController?.PlayMyVoice(1);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);

        anim.Play("combo2");
        // EnemyAttackPrefabGenerator.GenerateCircEnemyHintBar(ac, transform.position,
        //     RangedAttackFXLayer.transform, 6, Vector2.zero, false, true, 0.35f, 0.1f,
        //     1, true, true, true);

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);

        MeeleComboAttack(2);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo3v");

        yield return new WaitForSeconds(0.3f);

        var endPositionX = Mathf.Clamp(
                transform.position.x - ac.facedir * 10,
                BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);

        if(Mathf.Abs(endPositionX - _behavior.targetPlayer.transform.position.x) < 5)
        {
            endPositionX = Mathf.Clamp(
                transform.position.x + ac.facedir * 10,
                BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);

            ac.SetFaceDir(-ac.facedir);
        }

        _tweener = transform.DOMoveX(endPositionX, 0.7f);

        yield return new WaitForSeconds(0.8f);

        MeeleComboAttack(3);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();


    }
    
    /// <summary>
    /// Range combo
    /// </summary>
    /// <returns></returns>
    public IEnumerator DB15V_Action03()
    {
        yield return _canAction;

        ac.OnAttackEnter(100);
        ac.TurnMove(_behavior.targetPlayer);
        BattleEffectManager.Instance.SpawnExclamation(gameObject, transform.position + Vector3.up * 5, true);
        BattleEffectManager.Instance.SpawnTargetLockIndicator(_behavior.targetPlayer,3);
        
        yield return new WaitForSeconds(1.5f);
        

        anim.Play("combo1");

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);

        RangedComboAttack(1);
        
        _voiceController?.PlayMyVoice(1);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);

        anim.Play("combo2");
        ac.TurnMove(_behavior.targetPlayer);

        yield return null;
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.2f);

        RangedComboAttack(2);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.4f);

        ac.TurnMove(_behavior.targetPlayer);
        anim.Play("combo3");

        yield return null;
        //todo: modify the normalized time according to the animation.
        yield return new WaitForSeconds(0.33f);

        RangedComboAttack(3);
        DashForward();

        yield return new WaitForSeconds(0.45f);

        RangedComboAttack(4);

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.98f);

        anim.Play("idle");
        QuitAttack();


    }

    public IEnumerator DB15V_Action04(bool isMeele)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15V_Action04");

        yield return new WaitForSeconds(0.75f);

        float waitTime = isMeele ? 1.85f : 1.35f;
        Vector2 pos = transform.position;
        if(isMeele)
        {
            AngeticWindMeeleHint(waitTime);
        }
        else
        {
            pos = AngeticWindRangedHint(waitTime);
        }

        if (isMeele)
        {
            yield return new WaitForSeconds(waitTime - 0.75f);
            anim.Play("punch");
            yield return new WaitForSeconds(0.75f);
        }
        else
        {
            yield return new WaitForSeconds(waitTime - 1.25f);
            anim.Play("roll");
            yield return new WaitForSeconds(1.25f);
        }

        _voiceController?.BroadCastMyVoice(2);
        if(isMeele)
        {
            AngeticWindMeele();
        }
        else
        {
            AngeticRanged(pos);
        }

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsName("idle"));

        //anim.Play("idle");
        QuitAttack();
    }


    public IEnumerator DB15V_Action05(bool isMeele)
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15V_Action05");
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");

        yield return new WaitForSeconds(1.2f);
        
        anim.Play("charge_3");

        ConvictionGale(isMeele);
        _voiceController?.PlayMyVoice(3);
        
        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
     
    }
    
    
    
    public IEnumerator DB15V_Action06()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            transform.position + new Vector3(0, 1), RangedAttackFXLayer.transform,
            new Vector2(28, 4), Vector2.zero, true, 0, 1,
            ac.facedir == 1 ? 0 : 180, 0.5f, true);

        yield return new WaitForSeconds(0.75f);

        anim.Play("combo1");
        _voiceController?.PlayMyVoice(1);
        
        yield return new WaitForSeconds(0.3f);
        
        ThrowProjectile();

        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.98f);

        anim.Play("idle");
        QuitAttack();
    }
    
    public IEnumerator DB15V_Action07()
    {
        yield return _canAction;
        ac.OnAttackEnter(999);
        ac.TurnMove(_behavior.targetPlayer);
        bossBanner?.PrintSkillName("DB15V_Action07");
        
        yield return new WaitForSeconds(0.5f);
        
        anim.Play("charge_1");
        ac.SetHitSensor(false);
        _statusManager.DebuffResistance = 999;
        _statusManager.ReliefAllDebuff();

        yield return new WaitForSeconds(1.2f);
        
        var fx = StartShield();
        ac.SetHitSensor(true);
        
        yield return new WaitForSeconds(1f);
        
        var timer = 0f;
        if (_statusManager.HasCondition((int)BasicCalculation.BattleCondition.LifeShield))
        {
            
            while (timer < 14f && _statusManager.HasCondition
                       ((int)BasicCalculation.BattleCondition.LifeShield))
            {
                //print(timer);
                timer += Time.deltaTime;
                yield return null;
            }
        }

        if (timer >= 14f)
        {
            //anim.Play("charge_3");
            BurstAOE();
            yield return new WaitForSeconds(1);
        }
        else
        {
            BattleStageManager.Instance.CauseIndirectDamage(_statusManager, (int)(_statusManager.maxBaseHP * 0.05f), false,
                false, true);
        }
        
        anim.Play("charge_3");
        _statusManager.DebuffResistance = 0;

        yield return null;
        
        if(fx != null)
            Destroy(fx);

        yield return new WaitUntil
        (()=>anim.GetCurrentAnimatorStateInfo(0).IsName("charge_5")
             && anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f);
        
        anim.Play("idle");
        
        QuitAttack();
     
    }
    

    private void MeeleComboAttack(int stage)
    {
        var projectilePrefab = GetProjectileOfFormatName($"action02_{stage}");
        GameObject fx;

        if(stage == 1 || stage == 2)
        {
            fx = InstantiateMeele(projectilePrefab, transform.position+new Vector3(0,1), InitContainer(true));
        }
        else
        {
            fx = InstantiateRanged(projectilePrefab, transform.position + new Vector3(ac.facedir * 0.5f, 0),
                InitContainer(false),ac.facedir);

            var endPositionX = Mathf.Clamp(
                transform.position.x + ac.facedir * 18,
                BattleStageManager.Instance.mapBorderL,
                BattleStageManager.Instance.mapBorderR);

            fx.transform.DOMoveX(endPositionX, 0.6f).SetUpdate(UpdateType.Fixed).SetEase(Ease.OutSine);

        }

    }
    
    private void RangedComboAttack(int stage)
    {
        GameObject prefab;

        if(stage == 1)
        {
            prefab = GetProjectileOfFormatName("action03_1");
            GameObject container = InitContainer(false);

            var fx1 = InstantiateRanged(prefab, transform.position + new Vector3(ac.facedir, 0),
                container, ac.facedir);
            var fx2 = InstantiateRanged(prefab, transform.position + new Vector3(2*ac.facedir, 1),
                container, ac.facedir);
            var fx3 = InstantiateRanged(prefab, transform.position + new Vector3(ac.facedir, 2),
                container, ac.facedir);

            fx1.GetComponent<Rigidbody2D>().DOMoveX(fx1.transform.position.x + ac.facedir * 20, 0.5f).SetDelay(0.1f);
            fx2.GetComponent<Rigidbody2D>().DOMoveX(fx2.transform.position.x + ac.facedir * 20, 0.5f).SetDelay(0.1f);
            fx3.GetComponent<Rigidbody2D>().DOMoveX(fx3.transform.position.x + ac.facedir * 20, 0.5f).SetDelay(0.1f);

        }
        else if(stage == 2)
        {
            prefab = GetProjectileOfFormatName("action03_2");

            var fx = InstantiateRanged(prefab, _behavior.targetPlayer.RaycastedPosition(), InitContainer(false), 1);

        }
        else if(stage == 3)
        {
            prefab = GetProjectileOfFormatName("action03_4");

            var fx = Instantiate(prefab, transform.position+new Vector3(0,4), Quaternion.identity, BuffFXLayer.transform);

        }
        else
        {
            prefab = GetProjectileOfFormatName("action03_3");


            var fx = InstantiateMeele(prefab, gameObject.RaycastedPosition() + new Vector2(ac.facedir*2,1),
                InitContainer(true));

        }

    }

    private void DashForward()
    {
        var distance = _behavior.targetPlayer.transform.position.x - transform.position.x;
        float endPos = transform.position.x;

        if (Mathf.Abs(distance) > 4)
        {
            endPos = distance > 0 ?
                _behavior.targetPlayer.transform.position.x - 4 : _behavior.targetPlayer.transform.position.x + 4;
        }
        
        if (ac.facedir == 1 && distance < 0)
        {
            endPos = transform.position.x + 15;
        }
        else if (ac.facedir == -1 && distance > 0)
        {
            endPos = transform.position.x - 15;
        }
        else if (Mathf.Abs(distance) > 19)
        {
            endPos = transform.position.x + ac.facedir * 15;
        }
        
        endPos = Mathf.Clamp(endPos, BattleStageManager.Instance.mapBorderL, BattleStageManager.Instance.mapBorderR);

        _tweener = transform.DOMoveX(endPos, 0.35f);
    }

    private Vector2 AngeticWindRangedHint(float waitTime)
    {
        var prefab = GetProjectileOfFormatName("action04_2");

        Vector2 pos = _behavior.targetPlayer.RaycastedPosition();

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, pos, RangedAttackFXLayer.transform,
            new Vector2(5, 28), Vector2.zero, false, 0, waitTime, 90, 0.5f, true, false);

        return pos;

    }

    private void AngeticWindMeeleHint(float waitTime)
    {
        Vector2 pos = gameObject.RaycastedPosition();

        EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac, pos, RangedAttackFXLayer.transform,
            new Vector2(15, 18), Vector2.zero, false, 1, waitTime, 90, 0.5f, true, false);


    }

    private void AngeticRanged(Vector2 pos)
    {
        var prefab = GetProjectileOfFormatName("action04_2");

        var proj = InstantiateRanged(prefab, pos, InitContainer(false), 1);
        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Stormlash,
            63, 21, 1), 120);
    }

    private void AngeticWindMeele()
    {
        var prefab = GetProjectileOfFormatName("action04_1");

        Vector2 pos = gameObject.RaycastedPosition();

        var proj = InstantiateRanged(prefab, pos, InitContainer(false), 1);
        var atk = proj.GetComponent<AttackFromEnemy>();
        atk.AddWithConditionAll(new TimerBuff((int)BasicCalculation.BattleCondition.Poison,
            88, 15, 1), 120);
    }

    private void ConvictionGale(bool isMeele)
    {
        var startPos = isMeele ?
            gameObject.RaycastedPosition() + new Vector2(ac.facedir * 3, 0) :
            _behavior.targetPlayer.RaycastedPosition();
        
        var direction = isMeele ? ac.facedir : -ac.facedir;
        
        var firstHint = EnemyAttackPrefabGenerator.GenerateRectEnemyHintBar(ac,
            startPos,
            RangedAttackFXLayer.transform, new Vector2(5, 4), Vector2.zero, 
            true, 1, 1, 90, 0.5f,
            true, false);
        
        var projPrefab = GetProjectileOfFormatName("action05_1");
        var position = startPos;
        var posList = new List<Vector2>();
        posList.Add(position);
        var time = 1f;
        float size = 1;
        float step = 2;
        
        DOVirtual.DelayedCall(1, () =>
        {
            var proj = InstantiateRanged(projPrefab, startPos,
                InitContainer(false), 1);
        }, false);

        
        for (int i = 1; i < 6; i++)
        {
            if (position.x + step * direction * size > BattleStageManager.Instance.mapBorderR)
            {
                position.x = BattleStageManager.Instance.mapBorderR;
                direction = -direction;
            }else if(position.x +step * direction * size < BattleStageManager.Instance.mapBorderL)
            {
                position.x = BattleStageManager.Instance.mapBorderL;
                direction = -direction;
            }
            else
            {
                position.x += step * direction * size;
            }
            
            posList.Add(position);
            var delay = time + i * 2f;
            size += 0.25f;
            var currentSize = size;
            var currentPos = position;
            
            DOVirtual.DelayedCall(delay, () =>
            {
                var proj = InstantiateRanged(projPrefab, currentPos, InitContainer(false), 1);
                proj.transform.localScale = new Vector3(currentSize,currentSize, 1);
                var atk = proj.GetComponent<AttackFromEnemy>();
                atk.attackInfo[0].dmgModifier[0] *= (currentSize);
            }, false);


        }
        
    }

    private void ThrowProjectile()
    {
        var prefab = GetProjectileOfFormatName("action06_1");
        
        var proj = InstantiateDirectionalRanged(prefab, 
            transform.position + new Vector3(ac.facedir * 2, 1),
            InitContainer(false), ac.facedir,0);
    }

    private GameObject StartShield()
    {
        var shieldFX = GetProjectileOfFormatName("action07_1");
        
        var fx = Instantiate(shieldFX, 
            gameObject.RaycastedPosition(),
            Quaternion.identity, BuffFXLayer.transform);
        
        //_statusManager.ReliefAllDebuff();
        _statusManager.AddLifeShield(_statusManager.maxBaseHP,
            99999,false);
        
        return fx;
    }

    private void BurstAOE()
    {
        var prefab = GetProjectileOfFormatName("action07_2");
        
        var proj = InstantiateRanged(prefab, 
            new Vector3(0,BattleStageManager.Instance.mapBorderB),
            InitContainer(false), 1);

        _statusManager.RemoveSpecificTimerbuff((int)BasicCalculation.BattleCondition.LifeShield,
            -1);


    }
    
}
